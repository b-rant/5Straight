using _5Straight.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _5Straight.Data.GameAI
{
    public class PlayValuator
    {
        private readonly Chromosome Chromosome;

        public PlayValuator(Chromosome chromosome)
        {
            Chromosome = chromosome;
        }

        public Play CalculatePlay(Dictionary<int, double> rankedBoardOptions, Player player, bool deckHasCards)
        {
            Play playDecision = new Play();

            // If Player has no cards, short curcit and automatically draw
            if (player.Hand?.Count == 0 || rankedBoardOptions == null || !rankedBoardOptions.Any())
            {
                playDecision.Draw = true;
                return playDecision;
            }
           
            if (rankedBoardOptions == null || !rankedBoardOptions.Any())
            {
                throw new ApplicationException("No possible plays to choose from!");
            }

            // Calculate Draw value
            int missingCards = Configuration._HandSize - player.Hand.Count;
            double drawValue = Math.Pow(Chromosome.DrawMultiplyer, missingCards);

            // Re-Weigh board options based on difference between board location and required card value to play
            var sortedHand = player.Hand.OrderByDescending(x => x);
            Dictionary<int, double> weightedPlayOptions = new Dictionary<int, double>();
            foreach (var play in rankedBoardOptions)
            {
                foreach (var card in sortedHand)
                {
                    if (card <= play.Key)
                    {
                        // First modify value based on Card value multiplyer
                        // Adding _LargeCardPreference here to make larger cards even more desireable to spend
                        double cardValuePercentWeight = (card + Configuration._LargeCardPreference) * Chromosome.CardValueMultiplyer;

                        // Determine the card difference value multiplyer
                        int difference = play.Key - card;
                        double cardDifferencePercentWeight = 1 - (difference * Chromosome.CardLocationDifferenceMultiplyer);
                        
                        // Implement modifiers
                        weightedPlayOptions.Add(play.Key, play.Value * cardValuePercentWeight * cardDifferencePercentWeight);
                        break;
                    }
                }
            }

            // Determine Play
            var topPlayOption = weightedPlayOptions.OrderByDescending(x => x.Value).FirstOrDefault();
            if (topPlayOption.Value >= drawValue || player.Hand.Count == Configuration._HandSize || !deckHasCards)
            {
                playDecision.Draw = false;
                playDecision.PlayedLocationNumber = topPlayOption.Key;
                foreach (var card in sortedHand)
                {
                    if (card <= topPlayOption.Key)
                    {
                        playDecision.CardNumber = card;
                        break;
                    }
                }
            }
            else
            {
                playDecision.Draw = true;
            }

            return playDecision;
        }

    }
}
