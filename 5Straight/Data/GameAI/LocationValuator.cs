using _5Straight.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _5Straight.Data.GameAI
{
    public class LocationValuator
    {
        private Chromosome Chromosome;

        public LocationValuator(Chromosome chromosome)
        {
            Chromosome = chromosome;
        }

        public double CalculateLocationValue(List<BoardLocation> board, int location, int playerNum)
        {
            // Determine rows around location
            List<int> NW_SE = GetPrePlayRowValues(board, location, 0, new List<int>());
            List<int> N_S = GetPrePlayRowValues(board, location, 1, new List<int>());
            List<int> NE_SW = GetPrePlayRowValues(board, location, 2, new List<int>());
            List<int> W_E = GetPrePlayRowValues(board, location, 3, new List<int>());

            // Determine index of chosen location
            int NW_SE_index = NW_SE.IndexOf(location);
            int N_S_index = N_S.IndexOf(location);
            int NE_SW_Index = NE_SW.IndexOf(location);
            int W_E_Index = W_E.IndexOf(location);

            // Convert location numbers into location values
            ConvertLocationsToValues(board, NW_SE);
            ConvertLocationsToValues(board, N_S);
            ConvertLocationsToValues(board, NE_SW);
            ConvertLocationsToValues(board, W_E);

            // Calculate each row value
            var sum = CalculateCombinedPlayerRowValue(NW_SE, NW_SE_index, playerNum)
                + CalculateCombinedPlayerRowValue(N_S, N_S_index, playerNum)
                + CalculateCombinedPlayerRowValue(NE_SW, NE_SW_Index, playerNum)
                + CalculateCombinedPlayerRowValue(W_E, W_E_Index, playerNum);

            return sum;
        }

        private List<int> ConvertLocationsToValues(List<BoardLocation> board, List<int> locations)
        {
            for (int i = 0; i < locations.Count; i++)
            {
                locations[i] = board[locations[i]].FilledBy?.Team.TeamNumber ?? -1;
            }
            return locations;
        }

        private List<int> GetPrePlayRowValues(List<BoardLocation> board, int location, int direction, List<int> row, int counter = 0 , bool reverse = false)
        {
            if (!reverse)
            {
                var nextLocation = board[location].AdjacentLocations[direction];
                if (nextLocation == null || counter == 4)
                {
                    counter += 5;
                    return GetPrePlayRowValues(board, location, direction, row, counter, true);
                }
                counter++;
                return GetPrePlayRowValues(board, (int)nextLocation, direction, row, counter, reverse);
            }
            else
            {
                row.Add(location);
                counter--;
                var nextLocation = board[location].AdjacentLocations[direction+4];
                if (nextLocation == null)
                {
                    return row;
                }
                if (counter > 0)
                {
                    return GetPrePlayRowValues(board, (int)nextLocation, direction, row, counter, reverse);
                }
            }
            return row;
        }


        public double CalculateCombinedPlayerRowValue(List<int> prePlayRowValues, int locationIndex, int playerNum)
        {
            // Create PostPlayRows
            List<int> postPlayRowValuesPlayer1 = new List<int>();
            List<int> postPlayRowValuesPlayer2 = new List<int>();

            for (int i = 0; i < prePlayRowValues.Count; i++)
            {
                if (i == locationIndex)
                {
                    postPlayRowValuesPlayer1.Add(0);
                    postPlayRowValuesPlayer2.Add(1);
                }
                else
                {
                    postPlayRowValuesPlayer1.Add(prePlayRowValues[i]);
                    postPlayRowValuesPlayer2.Add(prePlayRowValues[i]);
                }
            }

            // Implement aggression and defensive multiplyers
            double player1Value = CalculatePlayerRowValue(prePlayRowValues, postPlayRowValuesPlayer1, 0);
            double player2Value = CalculatePlayerRowValue(prePlayRowValues, postPlayRowValuesPlayer2, 1);

            double weightedRowValue;

            if(playerNum == 0)
            {
                weightedRowValue = player1Value * Chromosome.OffensiveMultiplyer + player2Value * Chromosome.DefensiveMultiplyer;
            }
            else
            {
                weightedRowValue = player2Value * Chromosome.OffensiveMultiplyer + player1Value * Chromosome.DefensiveMultiplyer;
            }

            return weightedRowValue;
        }

        private double CalculatePlayerRowValue(List<int> prePlayRowValues, List<int> postPlayRowValues, int playerNum)
        {
            // Calculate the row value before a move would be made by the player
            var prePlayValue = CalculateRowValue2(prePlayRowValues, playerNum);

            // Calculate the row value after a move would be made by the player
            var postPlayValue = CalculateRowValue2(postPlayRowValues, playerNum);

            // Return the difference plus some extra to keep positive numbers
            return postPlayValue - prePlayValue + Chromosome.PotentialFiveMultiplyer * Configuration._LocationValueBump;
        }

        private double CalculateRowValue2(List<int> rowValues, int playerNum)
        {
            if (rowValues.Count < 5)
            {
                return 0;
            }

            var chunkScores = new List<int>();
            while (rowValues.Count >= 5)
            {
                var chunk = rowValues.Take(5);
                if (!chunk.Contains((playerNum + 1) % 2))
                {
                    chunkScores.Add(chunk.Where(x => x == playerNum).Count());
                }
                // Remove the first number and loop
                rowValues.RemoveAt(0);
            }

            if (!chunkScores.Any())
            {
                // This location has no offensive importance
                return 0;
            }

            // Calculate the value based on Chromosome Weight
            double value = 0.0;
            foreach (var score in chunkScores)
            {
                value += Math.Pow(Chromosome.PotentialFiveMultiplyer, score);
            }

            return value;
        }
    }
}
