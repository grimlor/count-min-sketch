using System;

namespace count_min_sketch
{
    public class TheGuide
    {
        public static void TellMeAboutSpace()
        {
            Console.WriteLine();
            Console.WriteLine("Space is big. You just won't believe how vastly, hugely, mind-bogglingly big it is. I mean, you may think it's a long way down the road to the chemist's, but that's just peanuts to space.");
            Console.WriteLine("- Douglas Adams");
            Console.WriteLine();
        }

        public static void TellMeMoreAboutSpace()
        {
            Console.WriteLine();
            Console.WriteLine(new SpaceProbabilities().ToString());
        }
    }
}