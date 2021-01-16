namespace count_min_sketch
{
    public class SpaceProbabilities
    {
        private const double MILKY_WAY_SIZE = 9500000000000; // 100000 light years

        private const double MILKY_WAY_STARS =  1000000000;

        private const double HUMAN_POPULATION = 9500000000;

        public const double STAR = MILKY_WAY_STARS / MILKY_WAY_SIZE;

        public const double PLANET = 4 * STAR;

        public const double MOON = 2 * PLANET;

        public const double HUMAN = 10 / HUMAN_POPULATION / MILKY_WAY_SIZE;

        public const double PETUNIA_WHALE = 2 * HUMAN / 10;

        public const double HITCHHIKER = 2 * HUMAN / 10;

        public const double SPACE = 1 - (STAR + PLANET + MOON + HUMAN + PETUNIA_WHALE);

        public override string ToString()
        {
            return "Probabilities of space:\n"
                + $"Space: {SPACE:#.##E+0}\n"
                + $"Stars: {STAR:#.##E+0}\n"
                + $"Planets: {PLANET:#.##E+0}\n"
                + $"Moons: {MOON:#.##E+0}\n"
                + $"Humans: {HUMAN:#.##E+0}\n"
                + $"Petunias: {PETUNIA_WHALE:#.##E+0}\n"
                + $"Whales: {PETUNIA_WHALE:#.##E+0}\n"
                + $"Hitchhikers: {HITCHHIKER:#.##E+0}";
        }
    }
}