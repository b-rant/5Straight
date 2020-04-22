namespace _5Straight.Data.GameAI
{
    public static class Configuration
    {

        // Chromosome
        public const int _MinimumNumberOfMutations = 1;

        public const int _MaxNumberOfMutations = 6;

        public const int _NumberOfChromosomesInGeneration = 30;

        public const int _NumberOfGamesPlayedPerChromosomePerGeneration = 10;

        public const double _MutationRate = .09;

        // Location Valuator
        public const int _LocationValueBump = 10;

        // Play Valuator
        public const int _HandSize = 4;

        public const int _LargeCardPreference = 50;
    }
}
