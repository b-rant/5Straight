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
        public readonly GameState GameState;
        public readonly string GameName;
        public readonly List<Delegate> Clients;

        public bool GameHasStarted { get; private set; }

        public Game(string gameName, GameState initialState)
        {
            GameName = gameName;
            GameState = initialState;
            Clients = new List<Delegate>();
        }

        // Public Functions

        public bool OwnPlayerSlot(int playerNumber, string userName)
        {
            var playerToOwn = GameState.Players.Where(x => x.PlayerNumber.Equals(playerNumber)).First();
            if (string.IsNullOrWhiteSpace(playerToOwn.PlayerOwner))
            {
                foreach (var player in GameState.Players.Where(x => !string.IsNullOrWhiteSpace(x.PlayerOwner) && x.PlayerOwner.Equals(userName)))
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
            var playerToOwn = GameState.Players.Where(x => x.PlayerNumber.Equals(playerNumber)).First();
            if (string.IsNullOrWhiteSpace(playerToOwn.PlayerOwner))
            {
                playerToOwn.PlayerOwner = "AI Player";
                playerToOwn.Npc = AiPlayerFactory.BuildAi(playerToOwn, this);
                ValidateAndStartGame();
                UpdateEveryone();
            }
        }

        public Player GetPlayerByNumber(int playerNumber)
        {
            return GameState.Players[playerNumber];
        }

        public List<Play> GetMostRecentPlays(int turnNumber)
        {
            return GameState.Plays.Where(x => x.TurnNumber >= turnNumber).ToList();
        }

        public string PlayLocation(Player player, int location, int card)
        {
            if (!CanPlay(location, card, player))
            {
                return CantPlayMessage(location, card, player);
            }

            RemoveCardFromHand(player, card);
            FillLocation(player, location);

            if (location.Equals(GameState.HighestPlayable))
            {
                DetermineHighestPlayable();
            }

            GameState.Plays.Add(new Play()
            {
                CardNumber = card,
                PlayedLocationNumber = location,
                Draw = false,
                PlayerHand = player.Hand,
                PlayerNumber = player.PlayerNumber,
                TurnNumber = GameState.TurnNumber
            });

            if (CheckWinCondition(location, player.Team))
            {
                GameState.Won = true;
                GameState.WinningPlayer = GameState.CurrentPlayer;
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

            GameState.Plays.Add(new Play()
            {
                CardNumber = cardDrew,
                PlayedLocationNumber = -1,
                Draw = true,
                PlayerHand = player.Hand,
                PlayerNumber = player.PlayerNumber,
                TurnNumber = GameState.TurnNumber
            });

            CheckForAllDeadCards();

            NextTurn();
            return "";
        }


        // Private Functions

        private void RunAI()
        {
            if (GameState.CurrentPlayer.Npc != null)
            {
                var play = GameState.CurrentPlayer.Npc.DeterminePlay();

                if (play.Draw)
                {
                    PlayDrawCard(GameState.CurrentPlayer);
                }
                else
                {
                    PlayLocation(GameState.CurrentPlayer, play.PlayedLocationNumber, play.CardNumber);
                }
            }
        }

        private bool ValidateAndStartGame()
        {
            if (GameState.Players.Where(x => string.IsNullOrWhiteSpace(x.PlayerOwner)).Any())
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
            var card = GameState.Deck[0];
            GameState.Deck.RemoveAt(0);
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
            GameState.TurnNumber++;
            GameState.CurrentPlayer = GameState.Players[GameState.TurnNumber % GameState.Players.Count];
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
            GameState.Board[locationNumber].Filled = true;
            GameState.Board[locationNumber].FilledBy = player;
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
            if (team.Players.Contains(GameState.Board[location].FilledBy))
            {
                //if it is, then check the next locaiton.
                var tempDirection = GameState.Board[location].AdjacentLocations[direction];
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
            if (!GameState.Won
                && location >= 0 
                && location <= 99
                && location >= card 
                && !GameState.Board[location].Filled 
                && player.Hand.Contains(card)
                && GameState.CurrentPlayer.Equals(player))
            {
                return true;
            }
            return false;
        }

        private string CantPlayMessage(int location, int card, Player player)
        {
            if (GameState.Won)
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
            if (GameState.Board[location].Filled)
            {
                return $"Failed to Play: Cannot play in {location}, it is already owned by another team.";
            }
            if (!player.Hand.Contains(card))
            {
                return $"Failed to Play: You do not have the {card} card in your hand.";
            }
            if (!GameState.CurrentPlayer.Equals(player))
            {
                return "Failed to Play: It is not your turn to play.";
            }
            return "Failed to Play: ERROR unknown failure reason...";
        }

        private bool CanDraw(Player player)
        {
            if (!GameState.Won 
                && player.Hand.Count < 4 
                && GameState.CurrentPlayer.Equals(player))
            {
                return true;
            }
            return false;
        }

        private string CantDrawMessage(Player player)
        {
            if (GameState.Won)
            {
                return "Failed to Draw: Game is already over!";
            }
            if (player.Hand.Count >= 4)
            {
                return "Failed to Draw: You cannot draw with a full hand.";
            }
            if (!GameState.CurrentPlayer.Equals(player))
            {
                return "Failed to Draw: It is not your turn to play.";
            }
            return "Failed to Draw: ERROR unknown failure reason...";
        }

        private void DetermineHighestPlayable()
        {
            for (int i = GameState.HighestPlayable; i > 0; i--)
            {
                if (!GameState.Board[i].Filled)
                {
                    GameState.HighestPlayable = i;
                    return;
                }
            }
            GameState.HighestPlayable = 0;
            return;
        }

        private void CheckForAllDeadCards()
        {
            foreach (var player in GameState.Players)
            {
                if (player.Hand.Where(x => x > GameState.HighestPlayable).Count() == 4)
                {
                    GameState.Won = true;
                    GameState.WinningPlayer = GameState.Players[(player.PlayerNumber + 1) % 2];
                    return;
                }
            }
        }
    }
}
