using _5Straight.Data.Models;

namespace _5Straight.Data.GameAI
{
    public static class AiPlayerFactory
    {
        public static Chromosome Balanced = new Chromosome()
        {
            PotentialFiveMultiplyer = 5.3849375342880084,
            OffensiveMultiplyer = 0.57636132374236426,
            DefensiveMultiplyer = 0.50189426202415223,
            DrawMultiplyer = 16.597734339813577,
            CardLocationDifferenceMultiplyer = 0.0037065690018732891,
            CardValueMultiplyer = 0.005720001515336335
        };

        public static Chromosome Offensive = new Chromosome()
        {
            PotentialFiveMultiplyer = 5.3849375342880084,
            OffensiveMultiplyer = 0.57636132374236426,
            DefensiveMultiplyer = 0.24189426202415223,
            DrawMultiplyer = 16.597734339813577,
            CardLocationDifferenceMultiplyer = 0.0037065690018732891,
            CardValueMultiplyer = 0.005720001515336335
        };

        public static AiPlayer BuildAi(Player player, Game game)
        {
            return new AiPlayer(Balanced, player, game);
        }
    }
}
