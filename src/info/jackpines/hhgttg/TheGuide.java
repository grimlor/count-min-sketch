package info.jackpines.hhgttg;

public class TheGuide {
  public static void TellMeAboutSpace() {
    System.out.println();
    System.out.println(
        "Space is big. You just won't believe how vastly, hugely, mind-bogglingly big it is. I mean, you may think it's a long way down the road to the chemist's, but that's just peanuts to space." );
    System.out.println( "- Douglas Adams" );
    System.out.println();
  }

  public static void TellMeMoreAboutSpace() {
    System.out.println();
    System.out.println(new SpaceProbabilities().toString());
  }
}
