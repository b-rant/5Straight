using _5Straight.Data.GameAI;
using _5Straight.Data.Models;
using _5Straight.Data.Proxies;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace _5Straight.Data
{
    public class Game : TableEntity
    {
        public readonly List<Delegate> Clients;

        public string GameId => PartitionKey;

        public bool GameHasStarted { get; set; }

        public string GameName { get; set; }

        public bool Won { get; set; }

        public bool DeadCardDraw { get; set; }

        public int TurnNumber { get; set; }

        public int HighestPlayable { get; set; }

        //Serializable properties:
        [IgnoreProperty]
        public List<int> Deck { get; set; }

        //Un-serializable: (need to manually rehydrate)
        [IgnoreProperty]
        public List<BoardLocation> Board { get; set; }

        [IgnoreProperty]
        public List<Play> Plays { get; set; }

        [IgnoreProperty]
        public List<Team> Teams { get; set; }

        [IgnoreProperty]
        public List<Player> Players { get; set; }

        [IgnoreProperty]
        public Player CurrentPlayer { get; set; }

        [IgnoreProperty]
        public Player WinningPlayer { get;  set; }

        public delegate void SaveGameCallback(Game game);
        public SaveGameCallback SaveGameDelegate;

        #region Constructors
        public Game()
        {
            //Empty constructor required by Table Storage.
            Clients = new List<Delegate>();
        }

        public Game(string partitionKey, string gameName, List<BoardLocation> board, List<int> deck, List<Team> teams, List<Player> players)
        {
            PartitionKey = partitionKey;
            RowKey = "Game";
            GameName = gameName;
            Board = board;
            Deck = deck;
            Teams = teams;
            Players = players;
            CurrentPlayer = Players[TurnNumber % Players.Count];
            Plays = new List<Play>();
            TurnNumber = 0;
            Won = false;
            HighestPlayable = 99;
            Clients = new List<Delegate>();
        }
        #endregion

        #region StorageFunctions
        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            //write the simple properties:
            var results = base.WriteEntity(operationContext);

            //serialize the complex properties:
            results["Deck"] = new EntityProperty(JsonConvert.SerializeObject(Deck));

            return results;
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);

            Deck = JsonConvert.DeserializeObject<List<int>>(properties["Deck"].StringValue);
        }
        #endregion

        // Public Functions

        public bool OwnPlayerSlot(int playerNumber, string userId, string userGivenName)
        {
            var playerToOwn = Players.Where(x => x.PlayerNumber.Equals(playerNumber)).First();
            if (string.IsNullOrWhiteSpace(playerToOwn.PlayerId))
            {
                foreach (var player in Players.Where(x => !string.IsNullOrWhiteSpace(x.PlayerId) && x.PlayerId.Equals(userId)))
                {
                    player.PlayerId = "";
                }
                playerToOwn.PlayerId = userId;
                playerToOwn.PlayerName = userGivenName;
                UpdateEveryone();
                return true;
            }
            return false;
        }

        public bool RemovePlayerSlot(int playerNumber)
        {
            var playerToOwn = Players.Where(x => x.PlayerNumber.Equals(playerNumber)).First();
            if (!string.IsNullOrWhiteSpace(playerToOwn.PlayerId))
            {
                playerToOwn.PlayerId = "";
                UpdateEveryone();
                return true;
            }
            return false;
        }

        public void OwnPlayerSlotForAi(int playerNumber)
        {
            var playerToOwn = Players.Where(x => x.PlayerNumber.Equals(playerNumber)).First();
            if (string.IsNullOrWhiteSpace(playerToOwn.PlayerId))
            {
                playerToOwn.PlayerId = Guid.NewGuid().ToString(); 
                playerToOwn.PlayerName = AiPlayerFactory.GetRandomNameNoDuplicates(Players);
                playerToOwn.Npc = AiPlayerFactory.BuildAi(playerToOwn, this);
                UpdateEveryone();
            }
        }

        public void RemoveAiFromPlayerSlot(int playerNumber)
        {
            var playerToClear = Players.Where(x => x.PlayerNumber.Equals(playerNumber)).First();
            if (!string.IsNullOrWhiteSpace(playerToClear.PlayerId) && playerToClear.Npc != null)
            {
                playerToClear.Npc = null;
                playerToClear.PlayerId = "";
            }
            UpdateEveryone();
        }

        public Player GetPlayerByNumber(int playerNumber)
        {
            return Players[playerNumber];
        }

        public List<Play> GetMostRecentPlays(int turnNumber)
        {
            return Plays.Where(x => x.TurnNumber >= turnNumber).ToList();
        }

        public string PlayLocation(Player player, int location, int card)
        {
            if (!CanPlay(location, card, player))
            {
                return CantPlayMessage(location, card, player);
            }

            RemoveCardFromHand(player, card);
            FillLocation(player, location);

            if (location.Equals(HighestPlayable))
            {
                DetermineHighestPlayable();
            }

            Plays.Add(new Play()
            {
                CardNumber = card,
                PlayedLocationNumber = location,
                Draw = false,
                PlayerHand = player.Hand,
                PlayerNumber = player.PlayerNumber,
                TurnNumber = TurnNumber
            });

            if (CheckWinCondition(location, player.Team))
            {
                Won = true;
                WinningPlayer = player;
            }
            else
            {
                CheckForAllDeadCards();
            }

            NextTurn();
            return "";
        }

        public string PlayDrawCard(Player player)
        {
            if (!CanDraw(player))
            {
                return CantDrawMessage(player);
            }

            var cardDrew = DrawCard();
            AddCardToHand(player, cardDrew);

            Plays.Add(new Play()
            {
                CardNumber = cardDrew,
                PlayedLocationNumber = -1,
                Draw = true,
                PlayerHand = player.Hand,
                PlayerNumber = player.PlayerNumber,
                TurnNumber = TurnNumber
            });

            CheckForAllDeadCards();

            NextTurn();
            return "";
        }

        public async Task RunAI()
        {
            if (Won)
            {
                // Game is over, no reason to keep playing
                return;
            }

            if (CurrentPlayer.Npc != null)
            {
                var play = await CurrentPlayer.Npc.DeterminePlay();

                if (play == null)
                {
                    // AI has no valid plays to make, game Draw
                    Won = true;
                    //TODO: Game should be over with draw not win, need a draw state instead of win only state...
                    WinningPlayer = Players[0];
                }

                if (play.Draw)
                {
                    PlayDrawCard(CurrentPlayer);
                }
                else
                {
                    PlayLocation(CurrentPlayer, play.PlayedLocationNumber, play.CardNumber);
                }
            }
        }

        public bool ValidateAndStartGame()
        {
            if (Players.Where(x => string.IsNullOrWhiteSpace(x.PlayerId)).Any())
            {
                return false;
            }
            else
            {
                GameHasStarted = true;
                UpdateEveryone();
                return true;
            }
        }


        // Private Functions

        private int DrawCard()
        {
            var card = Deck[0];
            Deck.RemoveAt(0);
            return card;
        }

        private void AddCardToHand(Player player, int card)
        {
            player.Hand.Add(card);
        }

        private void RemoveCardFromHand(Player player, int card)
        {
            player.Hand.Remove(card);
        }

        private void NextTurn()
        {
            TurnNumber++;
            CurrentPlayer = Players[TurnNumber % Players.Count];
            UpdateEveryone();
        }

        private async void UpdateEveryone()
        {
            foreach (Delegate d in Clients)
            {
                await Task.Run(() => d.DynamicInvoke());
            }

            SaveGameDelegate.Invoke(this);
        }

        private void FillLocation(Player player, int locationNumber)
        {
            Board[locationNumber].Filled = true;
            Board[locationNumber].FilledBy = player;
            Board[locationNumber].FilledByPlayerNumber = player.PlayerNumber;
        }

        private bool CheckWinCondition(int location, Team team)
        {
            var NW_SE = RecursiveBoardWinSearch(location, team, 0) + RecursiveBoardWinSearch(location, team, 4);
            var N_S = RecursiveBoardWinSearch(location, team, 1) + RecursiveBoardWinSearch(location, team, 5);
            var NE_SW = RecursiveBoardWinSearch(location, team, 2) + RecursiveBoardWinSearch(location, team, 6);
            var W_E = RecursiveBoardWinSearch(location, team, 3) + RecursiveBoardWinSearch(location, team, 7);

            if (NW_SE >= 6 || N_S >= 6 || NE_SW >= 6 || W_E >= 6)
            {
                return true;
            }
            return false;
        }

        private int RecursiveBoardWinSearch(int location, Team team, int direction)
        {
            //if the location is filled by the current user
            if (team.Players.Contains(Board[location].FilledBy))
            {
                //if it is, then check the next locaiton.
                var tempDirection = Board[location].AdjacentLocations[direction];
                //if the next location is undefined, it is out of the board so stop looking
                if (tempDirection == null)
                {
                    return 1;
                };
                return (1 + RecursiveBoardWinSearch((int)tempDirection, team, direction));
            }
            return 0;
        }

        private bool CanPlay(int location, int card, Player player)
        {
            if (!Won
                && location >= 0 
                && location <= 99
                && location >= card 
                && !Board[location].Filled 
                && player.Hand.Contains(card)
                && CurrentPlayer.Equals(player))
            {
                return true;
            }
            return false;
        }

        private string CantPlayMessage(int location, int card, Player player)
        {
            if (Won)
            {
                return "Failed to Play: Game is already over!";
            }
            if (location < 0 || location > 99)
            {
                return "Failed to Play: Location does not exist on the board.";
            }
            if (location < card)
            {
                return $"Failed to Play: Cannot play {card} in the {location}, location number is lower than card number.";
            }
            if (Board[location].Filled)
            {
                return $"Failed to Play: Cannot play in {location}, it is already owned by another team.";
            }
            if (!player.Hand.Contains(card))
            {
                return $"Failed to Play: You do not have the {card} card in your hand.";
            }
            if (!CurrentPlayer.Equals(player))
            {
                return "Failed to Play: It is not your turn to play.";
            }
            return "Failed to Play: ERROR unknown failure reason...";
        }

        private bool CanDraw(Player player)
        {
            if (!Won 
                && player.Hand.Count < 4 
                && CurrentPlayer.Equals(player))
            {
                return true;
            }
            return false;
        }

        private string CantDrawMessage(Player player)
        {
            if (Won)
            {
                return "Failed to Draw: Game is already over!";
            }
            if (player.Hand.Count >= 4)
            {
                return "Failed to Draw: You cannot draw with a full hand.";
            }
            if (!CurrentPlayer.Equals(player))
            {
                return "Failed to Draw: It is not your turn to play.";
            }
            return "Failed to Draw: ERROR unknown failure reason...";
        }

        private void DetermineHighestPlayable()
        {
            for (int i = HighestPlayable; i > 0; i--)
            {
                if (!Board[i].Filled)
                {
                    HighestPlayable = i;
                    return;
                }
            }
            HighestPlayable = 0;
            return;
        }

        private void CheckForAllDeadCards()
        {
            foreach (var player in Players)
            {
                int numDeadCards = player.Hand.Where(x => x > HighestPlayable).Count();
                if (numDeadCards == 4 || (!Deck.Any() && numDeadCards.Equals(player.Hand.Count())))
                {
                    Won = true;
                    DeadCardDraw = true;
                    return;
                }
            }
        }
    }
}
