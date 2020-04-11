using _5Straight.Data.Models;
using System.Linq;

namespace _5Straight.Data
{
    public class Game
    {
        private readonly GameData GameData;

        public Game(GameData game)
        {
            GameData = game;
            Deal();
        }

        // Public Functions

        public Player GetPlayerByNumber(int playerNumber)
        {
            return GameData.Players[playerNumber];
        }

        public bool MakePlay(Play play)
        {
            if (play.Draw)
            {
                return PlayDrawCard(GameData.Players[play.PlayerNumber]);
            }
            else
            {
                return PlayLocation(GameData.Players[play.PlayerNumber], play.PlayedLocationNumber, play.CardNumber);
            }
        }

        public bool PlayLocation(Player player, int location, int card)
        {
            if (!CanPlay(location, card, player))
            {
                return false;
            }

            RemoveCardFromHand(player, card);
            FillLocation(player, location);

            if (location.Equals(GameData.HighestPlayable))
            {
                DetermineHighestPlayable();
            }

            GameData.Plays.Add(new Play()
            {
                CardNumber = card,
                PlayedLocationNumber = location,
                Draw = false,
                PlayerHand = player.Hand,
                PlayerNumber = player.PlayerNumber,
                TurnNumber = GameData.TurnNumber
            });

            if (CheckWinCondition(location, player))
            {
                GameData.Won = true;
                GameData.WinningPlayer = GameData.CurrentPlayer;
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

            GameData.Plays.Add(new Play()
            {
                CardNumber = cardDrew,
                PlayedLocationNumber = -1,
                Draw = true,
                PlayerHand = player.Hand,
                PlayerNumber = player.PlayerNumber,
                TurnNumber = GameData.TurnNumber
            });

            CheckForAllDeadCards();

            NextTurn();
            return true;
        }


        // Private Functions

        private void Deal()
        {
            // deals 4 cards round robin
            for (int i = 0; i < 4; i++)
            {
                foreach (var player in GameData.Players)
                {
                    player.Hand.Add(DrawCard());
                }
            }
        }

        private int DrawCard()
        {
            var card = GameData.Deck[0];
            GameData.Deck.RemoveAt(0);
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
            GameData.TurnNumber++;
            GameData.CurrentPlayer = GameData.Players[GameData.TurnNumber % GameData.Players.Count];
        }

        private void FillLocation(Player player, int locationNumber)
        {
            GameData.Board[locationNumber].Filled = true;
            GameData.Board[locationNumber].FilledBy = player.PlayerNumber;
        }

        private bool CheckWinCondition(int location, Player player)
        {
            var NW_SE = RecursiveBoardWinSearch(location, player.PlayerNumber, 0) + RecursiveBoardWinSearch(location, player.PlayerNumber, 4);
            var N_S = RecursiveBoardWinSearch(location, player.PlayerNumber, 1) + RecursiveBoardWinSearch(location, player.PlayerNumber, 5);
            var NE_SW = RecursiveBoardWinSearch(location, player.PlayerNumber, 2) + RecursiveBoardWinSearch(location, player.PlayerNumber, 6);
            var W_E = RecursiveBoardWinSearch(location, player.PlayerNumber, 3) + RecursiveBoardWinSearch(location, player.PlayerNumber, 7);

            if (NW_SE >= 6 || N_S >= 6 || NE_SW >= 6 || W_E >= 6)
            {
                return true;
            }
            return false;
        }

        private int RecursiveBoardWinSearch(int location, int playerNumber, int direction)
        {
            //if the location is filled by the current user
            if (GameData.Board[location].FilledBy == playerNumber)
            {
                //if it is, then check the next locaiton.
                var tempDirection = GameData.Board[location].AdjacentLocations[direction];
                //if the next location is undefined, it is out of the board so stop looking
                if (tempDirection == null)
                {
                    return 1;
                };
                return (1 + RecursiveBoardWinSearch((int)tempDirection, playerNumber, direction));
            }
            return 0;
        }

        private bool CanPlay(int location, int card, Player player)
        {
            if (!GameData.Won
                && location >= 0 
                && location <= 99
                && location >= card 
                && !GameData.Board[location].Filled 
                && player.Hand.Contains(card)
                && GameData.CurrentPlayer.Equals(player))
            {
                return true;
            }
            return false;
        }

        private bool CanDraw(Player player)
        {
            if (!GameData.Won 
                && player.Hand.Count < 4 
                && GameData.CurrentPlayer.Equals(player))
            {
                return true;
            }
            return false;
        }

        private void DetermineHighestPlayable()
        {
            for (int i = GameData.HighestPlayable; i > 0; i--)
            {
                if (!GameData.Board[i].Filled)
                {
                    GameData.HighestPlayable = i;
                    return;
                }
            }
            GameData.HighestPlayable = 0;
            return;
        }

        private void CheckForAllDeadCards()
        {
            foreach (var player in GameData.Players)
            {
                if (player.Hand.Where(x => x > GameData.HighestPlayable).Count() == 4)
                {
                    GameData.Won = true;
                    GameData.WinningPlayer = GameData.Players[(player.PlayerNumber + 1) % 2];
                    return;
                }
            }
        }
    }
}
