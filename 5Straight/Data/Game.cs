using _5Straight.Data.Models;
using _5Straight.Data.Proxies;
using Microsoft.Azure.Cosmos.Table;
using System.Linq;

namespace _5Straight.Data
{
    public class Game : TableEntity
    {
        public readonly GameState GameState;
        public readonly string GameName;

        public Game(string gameName, GameState initialState)
        {
            GameName = gameName;
            GameState = initialState;
        }

        // Public Functions

        public Player GetPlayerByNumber(int playerNumber)
        {
            return GameState.Players[playerNumber];
        }

        public bool PlayLocation(Player player, int location, int card)
        {
            if (!CanPlay(location, card, player))
            {
                return false;
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

            if (CheckWinCondition(location, player))
            {
                GameState.Won = true;
                GameState.WinningPlayer = GameState.CurrentPlayer;
            }
            else
            {
                CheckForAllDeadCards();
            }

            NextTurn();
            return true;
        }

        public bool PlayDrawCard(Player player)
        {
            if (!CanDraw(player))
            {
                return false;
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
            return true;
        }


        // Private Functions

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
        }

        private void FillLocation(Player player, int locationNumber)
        {
            GameState.Board[locationNumber].Filled = true;
            GameState.Board[locationNumber].FilledBy = player;
        }

        private bool CheckWinCondition(int location, Player player)
        {
            var NW_SE = RecursiveBoardWinSearch(location, player, 0) + RecursiveBoardWinSearch(location, player, 4);
            var N_S = RecursiveBoardWinSearch(location, player, 1) + RecursiveBoardWinSearch(location, player, 5);
            var NE_SW = RecursiveBoardWinSearch(location, player, 2) + RecursiveBoardWinSearch(location, player, 6);
            var W_E = RecursiveBoardWinSearch(location, player, 3) + RecursiveBoardWinSearch(location, player, 7);

            if (NW_SE >= 6 || N_S >= 6 || NE_SW >= 6 || W_E >= 6)
            {
                return true;
            }
            return false;
        }

        private int RecursiveBoardWinSearch(int location, Player player, int direction)
        {
            //if the location is filled by the current user
            if (GameState.Board[location].FilledBy == player)
            {
                //if it is, then check the next locaiton.
                var tempDirection = GameState.Board[location].AdjacentLocations[direction];
                //if the next location is undefined, it is out of the board so stop looking
                if (tempDirection == null)
                {
                    return 1;
                };
                return (1 + RecursiveBoardWinSearch((int)tempDirection, player, direction));
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
