package info.jackpines.hhgttg;

public class SpaceProbabilities {
  private static double MILKY_WAY_SIZE = 9_500_000_000_000.0; // 100000 light years

  private static double MILKY_WAY_STARS = 1_000_000_000.0;

  private static double HUMAN_POPULATION = 9_500_000_000.0;

  public static double STAR = MILKY_WAY_STARS / MILKY_WAY_SIZE;

  public static double PLANET = 4 * STAR;

  public static double MOON = 2 * PLANET;

  public static double HUMAN = 10 / HUMAN_POPULATION / MILKY_WAY_SIZE;

  public static double PETUNIA_WHALE = 2 * HUMAN / 10;

  public static double HITCHHIKER = 2 * HUMAN / 10;

  public static double SPACE = 1 - (STAR + PLANET + MOON + HUMAN + PETUNIA_WHALE);

  @Override
  public String toString()
  {
    return String.format( "Probabilities of space:%n"
            + "Space: %e%n"
            + "Stars: %e%n"
            + "Planets: %e%n"
            + "Moons: %e%n"
            + "Humans: %e%n"
            + "Petunias: %e%n"
            + "Whales: %e%n"
            + "Hitchhikers: %e",
        SPACE, STAR, PLANET, MOON, HUMAN, PETUNIA_WHALE, PETUNIA_WHALE, HITCHHIKER );
  }
}
