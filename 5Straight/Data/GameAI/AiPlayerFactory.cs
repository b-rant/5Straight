using _5Straight.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _5Straight.Data.GameAI
{
    public static class AiPlayerFactory
    {
        public static List<string> AiPlayerNames = new List<string>()
        {
            "Agnes Bot",
            "Alfred Bot",
            "Archy Bot",
            "Barty Bot",
            "Benjamin Bot",
            "Bertram Bot",
            "Bruni Bot",
            "Buster Bot",
            "Edith Bot",
            "Ester Bot",
            "Flo Bot",
            "Francis Bot",
            "Francisco Bot",
            "Gil Bot",
            "Gob Bot",
            "Gus Bot",
            "Hank Bot",
            "Harold Bot",
            "Harriet Bot",
            "Henry Bot",
            "Jacques Bot",
            "Jorge Bot",
            "Juan Bot",
            "Kitty Bot",
            "Lionel Bot",
            "Louie Bot",
            "Lucille Bot",
            "Lupe Bot",
            "Mabel Bot",
            "Maeby Bot",
            "Marco Bot",
            "Marta Bot",
            "Maurice Bot",
            "Maynard Bot",
            "Mildred Bot",
            "Monty Bot",
            "Mordecai Bot",
            "Morty Bot",
            "Pablo Bot",
            "Seymour Bot",
            "Stan Bot",
            "Tobias Bot",
            "Vivian Bot",
            "Walter Bot",
            "Wilbur Bot"
        };

        private static Random RandomGen = new Random();

        public static Chromosome Balanced = new Chromosome()
        {
            PotentialFiveMultiplyer = 5.3849375342880084,
            OffensiveMultiplyer = 0.57636132374236426,
            DefensiveMultiplyer = 0.45189426202415223,
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

        public static string GetRandomNameNoDuplicates(List<Player> players)
        {
            var nameIndex = RandomGen.Next(AiPlayerNames.Count);
            string name = AiPlayerNames[nameIndex];
            bool isUnique = false;
            do
            {
                if (!players.Where(x => x.PlayerOwner.Equals(name)).Any())
                {
                    isUnique = true;
                    break;
                }
                nameIndex = RandomGen.Next(AiPlayerNames.Count);
            }
            while (!isUnique);
            return name;
        }
    }
}
