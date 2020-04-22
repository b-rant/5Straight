using System;

namespace _5Straight.Data.GameAI
{
    public class Chromosome
    {
        private Random Random;

        public Guid Id { get; set; }

        // Chromosome Properties (these are the changing values to effect gameplay choices)

        public double PotentialFiveMultiplyer { get; set; }

        public double OffensiveMultiplyer { get; set; }

        public double DefensiveMultiplyer { get; set; }

        public double DrawMultiplyer { get; set; }

        public double CardLocationDifferenceMultiplyer { get; set; }

        public double CardValueMultiplyer { get; set; }

        // Constructors for different generation needs

        /// <summary>
        /// Returns a fully random generated new Chromosome
        /// </summary>
        public Chromosome()
        {
            var guid = Guid.NewGuid();
            Random = new Random(guid.GetHashCode());
            Id = guid;
        }

        /// <summary>
        /// Returns a new Chromosome that is a copy, weight wise, of the provided chromosome
        /// </summary>
        /// <param name="toBeCoppied">Chromosome to copy</param>
        public Chromosome(Chromosome toBeCoppied)
        {
            var guid = Guid.NewGuid();
            Random = new Random(guid.GetHashCode());
            Id = guid;
            PotentialFiveMultiplyer = toBeCoppied.PotentialFiveMultiplyer;
            OffensiveMultiplyer = toBeCoppied.OffensiveMultiplyer;
            DefensiveMultiplyer = toBeCoppied.DefensiveMultiplyer;
            DrawMultiplyer = toBeCoppied.DrawMultiplyer;
            CardLocationDifferenceMultiplyer = toBeCoppied.CardLocationDifferenceMultiplyer;
            CardValueMultiplyer = toBeCoppied.CardValueMultiplyer;
        }

    }
}
