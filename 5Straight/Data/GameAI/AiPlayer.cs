using _5Straight.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _5Straight.Data.GameAI
{
    public class AiPlayer
    {
        private readonly Chromosome PlayerChromosome;
        private readonly LocationValuator LocationValuator;
        private readonly PlayValuator PlayValuator;
        private readonly Player Player;
        private readonly Game Game;

        public readonly Guid Id;

        public AiPlayer(Chromosome chromosome, Player player, Game game)
        {
            PlayerChromosome = chromosome;
            LocationValuator = new LocationValuator(PlayerChromosome);
            PlayValuator = new PlayValuator(PlayerChromosome);
            Player = player;
            Game = game;
            Id = chromosome.Id;
        }

        public Play DeterminePlay()
        {
            // Player must draw so short circut here
            if (Player.Hand.Count == 0)
            {
                return new Play()
                {
                    Draw = true
                };
            }

            // Determine the lowest playable card
            var sortedCards = Player.Hand.OrderBy(x => x);

            List<BoardLocation> playableLocations = Game.GameState.Board.Where(x => !x.Filled && x.Number >= sortedCards.FirstOrDefault()).ToList();

            // If there are no playable locations and the hand is full, something is wrong...
            if (!playableLocations.Any() && Player.Hand.Count == 4)
            {
                throw new ApplicationException("No Playable locations found for user with a full hand!!");
            }

            // Calculate list of locationValues
            Dictionary<int, double> boardValueResults = new Dictionary<int, double>();
            foreach (var loc in playableLocations)
            {
                boardValueResults.Add(loc.Number, LocationValuator.CalculateLocationValue(Game.GameState.Board, loc.Number, Player.PlayerNumber));
            }

            // Determine Best Play with the PlayValuator
            var playToMake = PlayValuator.CalculatePlay(boardValueResults, Player);

            return playToMake;
        }
    }
}
