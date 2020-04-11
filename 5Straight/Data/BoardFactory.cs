using _5Straight.Data.Models;
using System;
using System.Collections.Generic;

namespace _5Straight.data
{
    public static class BoardFactory
    {
        public static List<BoardLocation> BuildNewBoard()
        {
            return new List<BoardLocation>()
            {
                new BoardLocation() { DisplayNumber = "00", Number = 0, AdjacentLocations = new List<int?>{ null, null, null, null, null, 99, 64, 65 } },
                new BoardLocation() { DisplayNumber = "01", Number = 1, AdjacentLocations = new List<int?>{ 13, 14, 15, 4, 3, 2, 11, 12 } },
                new BoardLocation() { DisplayNumber = "02", Number = 2, AdjacentLocations = new List<int?>{ 12, 1, 4, 3, 8, 9, 10, 11 } },
                new BoardLocation() { DisplayNumber = "03", Number = 3, AdjacentLocations = new List<int?>{ 1, 4, 5, 6, 7, 8, 9, 2 } },
                new BoardLocation() { DisplayNumber = "04", Number = 4, AdjacentLocations = new List<int?>{ 14, 15, 16, 5, 6, 3, 2, 1 } },
                new BoardLocation() { DisplayNumber = "05", Number = 5, AdjacentLocations = new List<int?>{ 15, 16, 35, 34, 33, 6, 3, 4 } },
                new BoardLocation() { DisplayNumber = "06", Number = 6, AdjacentLocations = new List<int?>{ 4, 5, 34, 33, 32, 7, 8, 3 } },
                new BoardLocation() { DisplayNumber = "07", Number = 7, AdjacentLocations = new List<int?>{ 3, 6, 33, 32, 31, 30, 29, 8 } },
                new BoardLocation() { DisplayNumber = "08", Number = 8, AdjacentLocations = new List<int?>{ 2, 3, 6, 7, 30, 29, 28, 9 } },
                new BoardLocation() { DisplayNumber = "09", Number = 9, AdjacentLocations = new List<int?>{ 11, 2, 3, 8, 29, 28, 27, 10 } },
                new BoardLocation() { DisplayNumber = "10", Number = 10, AdjacentLocations = new List<int?>{ 24, 11, 2, 9, 28, 27, 26, 25 } },
                new BoardLocation() { DisplayNumber = "11", Number = 11, AdjacentLocations = new List<int?>{ 23, 12, 1, 2, 9, 10, 25, 24 } },
                new BoardLocation() { DisplayNumber = "12", Number = 12, AdjacentLocations = new List<int?>{ 22, 13, 14, 1, 2, 11, 24, 23 } },
                new BoardLocation() { DisplayNumber = "13", Number = 13, AdjacentLocations = new List<int?>{ 21, 20, 19, 14, 1, 12, 23, 22 } },
                new BoardLocation() { DisplayNumber = "14", Number = 14, AdjacentLocations = new List<int?>{ 20, 19, 18, 15, 4, 1, 12, 13 } },
                new BoardLocation() { DisplayNumber = "15", Number = 15, AdjacentLocations = new List<int?>{ 19, 18, 17, 16, 5, 4, 1, 14 } },
                new BoardLocation() { DisplayNumber = "16", Number = 16, AdjacentLocations = new List<int?>{ 18, 17, 36, 35, 34, 5, 4, 15 } },
                new BoardLocation() { DisplayNumber = "17", Number = 17, AdjacentLocations = new List<int?>{ 61, 62, 63, 36, 35, 16, 15, 18 } },
                new BoardLocation() { DisplayNumber = "18", Number = 18, AdjacentLocations = new List<int?>{ 60, 61, 62, 17, 16, 15, 14, 19 } },
                new BoardLocation() { DisplayNumber = "19", Number = 19, AdjacentLocations = new List<int?>{ 59, 60, 61, 18, 15, 14, 13, 20 } },
                new BoardLocation() { DisplayNumber = "20", Number = 20, AdjacentLocations = new List<int?>{ 58, 59, 60, 19, 14, 13, 22, 21 } },
                new BoardLocation() { DisplayNumber = "21", Number = 21, AdjacentLocations = new List<int?>{ 57, 58, 59, 20, 13, 22, 55, 56 } },
                new BoardLocation() { DisplayNumber = "22", Number = 22, AdjacentLocations = new List<int?>{ 56, 21, 20, 13, 12, 23, 54, 55 } },
                new BoardLocation() { DisplayNumber = "23", Number = 23, AdjacentLocations = new List<int?>{ 55, 22, 13, 12, 11, 24, 53, 54 } },
                new BoardLocation() { DisplayNumber = "24", Number = 24, AdjacentLocations = new List<int?>{ 54, 23, 12, 11, 10, 25, 52, 53 } },
                new BoardLocation() { DisplayNumber = "25", Number = 25, AdjacentLocations = new List<int?>{ 53, 24, 11, 10, 27, 26, 51, 52 } },
                new BoardLocation() { DisplayNumber = "26", Number = 26, AdjacentLocations = new List<int?>{ 52, 25, 10, 27, 48, 49, 50, 51 } },
                new BoardLocation() { DisplayNumber = "27", Number = 27, AdjacentLocations = new List<int?>{ 25, 10, 9, 28, 47, 48, 49, 26 } },
                new BoardLocation() { DisplayNumber = "28", Number = 28, AdjacentLocations = new List<int?>{ 10, 9, 8, 29, 46, 47, 48, 27 } },
                new BoardLocation() { DisplayNumber = "29", Number = 29, AdjacentLocations = new List<int?>{ 9, 8, 7, 30, 45, 46, 47, 28 } },
                new BoardLocation() { DisplayNumber = "30", Number = 30, AdjacentLocations = new List<int?>{ 8, 7, 32, 31, 44, 45, 46, 29 } },
                new BoardLocation() { DisplayNumber = "31", Number = 31, AdjacentLocations = new List<int?>{ 7, 32, 41, 42, 43, 44, 45, 30 } },
                new BoardLocation() { DisplayNumber = "32", Number = 32, AdjacentLocations = new List<int?>{ 6, 33, 40, 41, 42, 31, 30, 7 } },
                new BoardLocation() { DisplayNumber = "33", Number = 33, AdjacentLocations = new List<int?>{ 5, 34, 39, 40, 41, 32, 7, 6 } },
                new BoardLocation() { DisplayNumber = "34", Number = 34, AdjacentLocations = new List<int?>{ 16, 35, 38, 39, 40, 33, 6, 5 } },
                new BoardLocation() { DisplayNumber = "35", Number = 35, AdjacentLocations = new List<int?>{ 17, 36, 37, 38, 39, 34, 5, 16 } },
                new BoardLocation() { DisplayNumber = "36", Number = 36, AdjacentLocations = new List<int?>{ 62, 63, 64, 37, 38, 35, 16, 17 } },
                new BoardLocation() { DisplayNumber = "37", Number = 37, AdjacentLocations = new List<int?>{ 63, 64, 99, 98, 97, 38, 35, 36 } },
                new BoardLocation() { DisplayNumber = "38", Number = 38, AdjacentLocations = new List<int?>{ 36, 37, 98, 97, 96, 39, 34, 35 } },
                new BoardLocation() { DisplayNumber = "39", Number = 39, AdjacentLocations = new List<int?>{ 35, 38, 97, 96, 95, 40, 33, 34 } },
                new BoardLocation() { DisplayNumber = "40", Number = 40, AdjacentLocations = new List<int?>{ 34, 39, 96, 95, 94, 41, 32, 33 } },
                new BoardLocation() { DisplayNumber = "41", Number = 41, AdjacentLocations = new List<int?>{ 33, 40, 95, 94, 93, 42, 31, 32 } },
                new BoardLocation() { DisplayNumber = "42", Number = 42, AdjacentLocations = new List<int?>{ 32, 41, 94, 93, 92, 43, 44, 31 } },
                new BoardLocation() { DisplayNumber = "43", Number = 43, AdjacentLocations = new List<int?>{ 31, 42, 93, 92, 91, 90, 89, 44 } },
                new BoardLocation() { DisplayNumber = "44", Number = 44, AdjacentLocations = new List<int?>{ 30, 31, 42, 43, 90, 89, 88, 45 } },
                new BoardLocation() { DisplayNumber = "45", Number = 45, AdjacentLocations = new List<int?>{ 29, 30, 31, 44, 89, 88, 87, 46 } },
                new BoardLocation() { DisplayNumber = "46", Number = 46, AdjacentLocations = new List<int?>{ 28, 29, 30, 45, 88, 87, 86, 47 } },
                new BoardLocation() { DisplayNumber = "47", Number = 47, AdjacentLocations = new List<int?>{ 27, 28, 29, 46, 87, 86, 85, 48 } },
                new BoardLocation() { DisplayNumber = "48", Number = 48, AdjacentLocations = new List<int?>{ 26, 27, 28, 47, 86, 85, 84, 49 } },
                new BoardLocation() { DisplayNumber = "49", Number = 49, AdjacentLocations = new List<int?>{ 51, 26, 27, 48, 85, 84, 83, 50 } },
                new BoardLocation() { DisplayNumber = "50", Number = 50, AdjacentLocations = new List<int?>{ 80, 51, 26, 49, 84, 83, 82, 81 } },
                new BoardLocation() { DisplayNumber = "51", Number = 51, AdjacentLocations = new List<int?>{ 79, 52, 25, 26, 49, 50, 81, 80 } },
                new BoardLocation() { DisplayNumber = "52", Number = 52, AdjacentLocations = new List<int?>{ 78, 53, 24, 25, 26, 51, 80, 79 } },
                new BoardLocation() { DisplayNumber = "53", Number = 53, AdjacentLocations = new List<int?>{ 77, 54, 23, 24, 25, 52, 79, 78 } },
                new BoardLocation() { DisplayNumber = "54", Number = 54, AdjacentLocations = new List<int?>{ 76, 55, 22, 23, 24, 53, 78, 77 } },
                new BoardLocation() { DisplayNumber = "55", Number = 55, AdjacentLocations = new List<int?>{ 75, 56, 21, 22, 23, 54, 77, 76 } },
                new BoardLocation() { DisplayNumber = "56", Number = 56, AdjacentLocations = new List<int?>{ 74, 57, 58, 21, 22, 55, 76, 75 } },
                new BoardLocation() { DisplayNumber = "57", Number = 57, AdjacentLocations = new List<int?>{ 73, 72, 71, 58, 21, 56, 75, 74 } },
                new BoardLocation() { DisplayNumber = "58", Number = 58, AdjacentLocations = new List<int?>{ 72, 71, 70, 59, 20, 21, 56, 57 } },
                new BoardLocation() { DisplayNumber = "59", Number = 59, AdjacentLocations = new List<int?>{ 71, 70, 69, 60, 19, 20, 21, 58 } },
                new BoardLocation() { DisplayNumber = "60", Number = 60, AdjacentLocations = new List<int?>{ 70, 69, 68, 61, 18, 19, 20, 59 } },
                new BoardLocation() { DisplayNumber = "61", Number = 61, AdjacentLocations = new List<int?>{ 69, 68, 67, 62, 17, 18, 19, 60 } },
                new BoardLocation() { DisplayNumber = "62", Number = 62, AdjacentLocations = new List<int?>{ 68, 67, 66, 63, 36, 17, 18, 61 } },
                new BoardLocation() { DisplayNumber = "63", Number = 63, AdjacentLocations = new List<int?>{ 67, 66, 65, 64, 37, 36, 17, 62 } },
                new BoardLocation() { DisplayNumber = "64", Number = 64, AdjacentLocations = new List<int?>{ 66, 65, 0, 99, 98, 37, 36, 63 } },
                new BoardLocation() { DisplayNumber = "65", Number = 65, AdjacentLocations = new List<int?>{ null, null, null, 0, 99, 64, 63, 66 } },
                new BoardLocation() { DisplayNumber = "66", Number = 66, AdjacentLocations = new List<int?>{ null, null, null, 65, 64, 63, 62, 67 } },
                new BoardLocation() { DisplayNumber = "67", Number = 67, AdjacentLocations = new List<int?>{ null, null, null, 66, 63, 62, 61, 68 } },
                new BoardLocation() { DisplayNumber = "68", Number = 68, AdjacentLocations = new List<int?>{ null, null, null, 67, 62, 61, 60, 69 } },
                new BoardLocation() { DisplayNumber = "69", Number = 69, AdjacentLocations = new List<int?>{ null, null, null, 68, 61, 60, 59, 70 } },
                new BoardLocation() { DisplayNumber = "70", Number = 70, AdjacentLocations = new List<int?>{ null, null, null, 69, 60, 59, 58, 71 } },
                new BoardLocation() { DisplayNumber = "71", Number = 71, AdjacentLocations = new List<int?>{ null, null, null, 70, 59, 58, 57, 72 } },
                new BoardLocation() { DisplayNumber = "72", Number = 72, AdjacentLocations = new List<int?>{ null, null, null, 71, 58, 57, 74, 73 } },
                new BoardLocation() { DisplayNumber = "73", Number = 73, AdjacentLocations = new List<int?>{ null, null, null, 72, 57, 74, null, null } },
                new BoardLocation() { DisplayNumber = "74", Number = 74, AdjacentLocations = new List<int?>{ null, 73, 72, 57, 56, 75, null, null } },
                new BoardLocation() { DisplayNumber = "75", Number = 75, AdjacentLocations = new List<int?>{ null, 74, 57, 56, 55, 76, null, null } },
                new BoardLocation() { DisplayNumber = "76", Number = 76, AdjacentLocations = new List<int?>{ null, 75, 56, 55, 54, 77, null, null } },
                new BoardLocation() { DisplayNumber = "77", Number = 77, AdjacentLocations = new List<int?>{ null, 76, 55, 54, 53, 78, null, null } },
                new BoardLocation() { DisplayNumber = "78", Number = 78, AdjacentLocations = new List<int?>{ null, 77, 54, 53, 52, 79, null, null } },
                new BoardLocation() { DisplayNumber = "79", Number = 79, AdjacentLocations = new List<int?>{ null, 78, 53, 52, 51, 80, null, null } },
                new BoardLocation() { DisplayNumber = "80", Number = 80, AdjacentLocations = new List<int?>{ null, 79, 52, 51, 50, 81, null, null } },
                new BoardLocation() { DisplayNumber = "81", Number = 81, AdjacentLocations = new List<int?>{ null, 80, 51, 50, 83, 82, null, null } },
                new BoardLocation() { DisplayNumber = "82", Number = 82, AdjacentLocations = new List<int?>{ null, 81, 50, 83, null, null, null, null } },
                new BoardLocation() { DisplayNumber = "83", Number = 83, AdjacentLocations = new List<int?>{ 81, 50, 49, 84, null, null, null, 82 } },
                new BoardLocation() { DisplayNumber = "84", Number = 84, AdjacentLocations = new List<int?>{ 50, 49, 48, 85, null, null, null, 83 } },
                new BoardLocation() { DisplayNumber = "85", Number = 85, AdjacentLocations = new List<int?>{ 49, 48, 47, 86, null, null, null, 84 } },
                new BoardLocation() { DisplayNumber = "86", Number = 86, AdjacentLocations = new List<int?>{ 48, 47, 46, 87, null, null, null, 85 } },
                new BoardLocation() { DisplayNumber = "87", Number = 87, AdjacentLocations = new List<int?>{ 47, 46, 45, 88, null, null, null, 86 } },
                new BoardLocation() { DisplayNumber = "88", Number = 88, AdjacentLocations = new List<int?>{ 46, 45, 44, 89, null, null, null, 87 } },
                new BoardLocation() { DisplayNumber = "89", Number = 89, AdjacentLocations = new List<int?>{ 45, 44, 43, 90, null, null, null, 88 } },
                new BoardLocation() { DisplayNumber = "90", Number = 90, AdjacentLocations = new List<int?>{ 44, 43, 92, 91, null, null, null, 89 } },
                new BoardLocation() { DisplayNumber = "91", Number = 91, AdjacentLocations = new List<int?>{ 43, 92, null, null, null, null, null, 90 } },
                new BoardLocation() { DisplayNumber = "92", Number = 92, AdjacentLocations = new List<int?>{ 42, 93, null, null, null, 91, 90, 43 } },
                new BoardLocation() { DisplayNumber = "93", Number = 93, AdjacentLocations = new List<int?>{ 41, 94, null, null, null, 92, 43, 42 } },
                new BoardLocation() { DisplayNumber = "94", Number = 94, AdjacentLocations = new List<int?>{ 40, 95, null, null, null, 93, 42, 41 } },
                new BoardLocation() { DisplayNumber = "95", Number = 95, AdjacentLocations = new List<int?>{ 39, 96, null, null, null, 94, 41, 40 } },
                new BoardLocation() { DisplayNumber = "96", Number = 96, AdjacentLocations = new List<int?>{ 38, 97, null, null, null, 95, 40, 39 } },
                new BoardLocation() { DisplayNumber = "97", Number = 97, AdjacentLocations = new List<int?>{ 37, 98, null, null, null, 96, 39, 38 } },
                new BoardLocation() { DisplayNumber = "98", Number = 98, AdjacentLocations = new List<int?>{ 64, 99, null, null, null, 97, 38, 37 } },
                new BoardLocation() { DisplayNumber = "99", Number = 99, AdjacentLocations = new List<int?>{ 65, 0, null, null, null, 98, 37, 64 } },
            };
        }

        public static List<Player> BuildPlayers()
        {
            var players = new List<Player>();
            for (var i = 0; i < 2; i++)
            {
                var tempPlayer = new Player();
                //Set Player number
                tempPlayer.PlayerNumber = i;
                tempPlayer.Hand = new List<int>();
                players.Add(tempPlayer);
            };
            return players;
        }

        public static List<int> BuildDeck()
        {
            var deck = new List<int>();
            for (int i = 0; i < 100; i++)
            {
                deck.Add(i);
            }
            deck = ShuffleDeck(deck);
            return ShuffleDeck(deck);
        }

        private static List<int> ShuffleDeck(List<int> deck)
        {
            var rand = new Random();
            for (var i = deck.Count - 1; i > 0; i--)
            {
                var j = rand.Next(100);
                var temp = deck[i];
                deck[i] = deck[j];
                deck[j] = temp;
            }
            return deck;
        }
    }
}
