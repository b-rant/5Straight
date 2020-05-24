using _5Straight.Data.GameAI;
using _5Straight.Data.Models;
using _5Straight.Data.Proxies;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _5Straight.Data
{
    public class Game : TableEntity
    {
        //public readonly GameState GameState;
        public readonly string GameName;

        public bool GameHasStarted { get; private set; }

        public bool Won { get; set; }

        public int TurnNumber { get; set; }

        public int HighestPlayable { get; set; }

        public List<BoardLocation> Board { get; set; }

        public List<int> Deck { get; set; }

        public List<Play> Plays { get; set; }

        public List<Team> Teams { get; set; }

        public List<Player> Players { get; set; }

        public Player CurrentPlayer { get; set; }

        public Player WinningPlayer { get; internal set; }

        public readonly List<Delegate> Clients; 

        public Game()
        {
            //Empty constructor required by Table Storage.
        }

        public Game(Guid partitionKey, string gameName, List<BoardLocation> board, List<int> deck, List<Team> teams, List<Player> players)
        {
            PartitionKey = partitionKey.ToString();
            RowKey = "Game";
            GameName = gameName;
            Board = board;
            Deck = deck;
            Teams = teams;
            Players = players;
            CurrentPlayer = players[0];
            Plays = new List<Play>();
            TurnNumber = 0;
            Won = false;
            HighestPlayable = 99;
            Clients = new List<Delegate>();
        }

        // Public Functions

        public bool OwnPlayerSlot(int playerNumber, string userName)
        {
            var playerToOwn = Players.Where(x => x.PlayerNumber.Equals(playerNumber)).First();
            if (string.IsNullOrWhiteSpace(playerToOwn.PlayerOwner))
            {
                foreach (var player in Players.Where(x => !string.IsNullOrWhiteSpace(x.PlayerOwner) && x.PlayerOwner.Equals(userName)))
                {
                    player.PlayerOwner = "";
                }
                playerToOwn.PlayerOwner = userName;
                ValidateAndStartGame();
                UpdateEveryone();
                return true;
            }
            return false;
        }

        public void OwnPlayerSlotForAi(int playerNumber)
        {
            var playerToOwn = Players.Where(x => x.PlayerNumber.Equals(playerNumber)).First();
            if (string.IsNullOrWhiteSpace(playerToOwn.PlayerOwner))
            {
                playerToOwn.PlayerOwner = AiPlayerFactory.GetRandomNameNoDuplicates(Players);
                playerToOwn.Npc = AiPlayerFactory.BuildAi(playerToOwn, this);
                ValidateAndStartGame();
                UpdateEveryone();
            }
        }

        public void RemoveAiFromPlayerSlot(int playerNumber)
        {
            var playerToClear = Players.Where(x => x.PlayerNumber.Equals(playerNumber)).First();
            if (!string.IsNullOrWhiteSpace(playerToClear.PlayerOwner) && playerToClear.Npc != null)
            {
                playerToClear.Npc = null;
                playerToClear.PlayerOwner = "";
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
                WinningPlayer = CurrentPlayer;
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


        // Private Functions

        private async void RunAI()
        {
            if (Won)
            {
                // Game is over, no reason to keep playing
                return;
            }

            if (CurrentPlayer.Npc != null)
            {
                var play = await CurrentPlayer.Npc.DeterminePlay();

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

        private bool ValidateAndStartGame()
        {
            if (Players.Where(x => string.IsNullOrWhiteSpace(x.PlayerOwner)).Any())
            {
                return false;
            }
            else
            {
                GameHasStarted = true;
                RunAI();
                return true;
            }
        }

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
            RunAI();
            UpdateEveryone();
        }

        private async void UpdateEveryone()
        {
            foreach (Delegate d in Clients)
            {
                await Task.Run(() => d.DynamicInvoke());
            }
        }

        private void FillLocation(Player player, int locationNumber)
        {
            Board[locationNumber].Filled = true;
            Board[locationNumber].FilledBy = player;
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
                if (player.Hand.Where(x => x > HighestPlayable).Count() == 4)
                {
                    Won = true;
                    WinningPlayer = Players[(player.PlayerNumber + 1) % 2];
                    return;
                }
            }
        }
    }
}
