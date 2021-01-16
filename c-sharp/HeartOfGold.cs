using System;
using System.Threading.Tasks;
using count_min_sketch.RedisShim;
using StackExchange.Redis;

namespace count_min_sketch
{
    // Let's bop around the galaxy a bit and see if we can find a hitchhiker.
    public class HeartOfGold
    {
        public const string LOCATOR_KEY = "locator";

        private readonly ConnectionMultiplexer redisConn;
        private readonly CountMinSketchShim cms;
        private readonly Random random = new Random();
        public bool isScanning { get; private set; }
        public double Improbability { get; private set; }

        public HeartOfGold(ConnectionMultiplexer redisConn)
        {
            this.redisConn = redisConn;
            this.Cleanup();

            // The Heart Of Gold uses a count-min sketch to keep track of the anomalies it finds during scans.
            this.cms = new CountMinSketchShim(redisConn);
            cms.InitByProb(LOCATOR_KEY, 0.001f, 0.01f);

            TheGuide.TellMeAboutSpace();

            TheGuide.TellMeMoreAboutSpace();

            this.Improbability = 1;
        }

        // Mostly startup code, keyboard scanning, and output; will focus on the CMS bits.
        public static void ScanForHitchhikers(ConnectionMultiplexer redisConn) 
        {
                var hog = new HeartOfGold(redisConn);
                hog.StartScanning();
                bool exitNow = false;

                var previousAnomalies = 0;
                do
                {
                    while (!Console.KeyAvailable && !exitNow)
                    {
                        if (hog.CountAnomalies() > previousAnomalies)
                        {
                            exitNow = hog.CountHitchhikers() > 0;
                            Console.WriteLine($"Found an anomaly! Checking for hitchhikers...{(!exitNow ? "none found - continuing scan" : "")}");
                            previousAnomalies = hog.CountAnomalies();
                        }
                    }

                    if (Console.KeyAvailable)
                    {
                        var keypress = Console.ReadKey(true);

                        // This is the power of the Improbability Drive!
                        if (keypress.KeyChar == ']')
                        {
                            hog.IncreaseImprobability();
                        }
    
                        // It'll keep scanning until we either hit 'q' to quit or we find a hitchhiker.
                        if (keypress.KeyChar == 'q')
                        {
                            exitNow = true;
                        }
                    }
                } while (!exitNow);

                hog.StopScanning();

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Returning to normalcy");
                
                hog.OutputAllSightings();

                var countHitchhikers = hog.CountHitchhikers();
                if (countHitchhikers > 0)
                {
                    Console.WriteLine($"Found {countHitchhikers} hitchhiker{(countHitchhikers > 1 ? "s" : "")} found at an improbability coefficient of {hog.Improbability}!");
                }

                Console.WriteLine("Normalcy returned.");
                hog.Cleanup();
        }

        public void StartScanning()
        {
            isScanning = true;

            // The scanning runs asynchronously
            Task.Run((Action)ScanSpace);
        }

        public void StopScanning()
        {
            isScanning = false;
        }

        public void Cleanup()
        {
            var db = redisConn.GetDatabase();
            db.KeyDelete(LOCATOR_KEY);
        }

        public void IncreaseImprobability()
        {
            this.Improbability *= 10;
        }

        // Here is the improbability engine. (I can dig in but suffice to say it has real probability underpinning it.)
        private void ScanSpace()
        {
            Console.WriteLine();
            Console.WriteLine("Improbability drive scanning space");

            var counter = 0;
            var counterResetVal = 30;
            while (isScanning)
            {
                var scanResult = Math.Abs(random.NextDouble()) / Improbability;
                var anomaly = IdentifyAnomalies(scanResult);

                // Here's where we keep track of our sightings.
                cms.IncrBy(LOCATOR_KEY, anomaly.ToString());

                counter = counter == counterResetVal ? 0 : counter + 1;
                if (counter % 100 == 0)
                {
                    Console.CursorLeft = 0;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write($"Improbability level: {Improbability}, {scanResult}: {anomaly}     ");
                }
            }
        }

        // To see what we stored in the sketch, we'll output everything we saw as we bopped around the galaxy.
        public void OutputAllSightings()
        {
            Console.WriteLine("All sightings:");
            Console.WriteLine($"Space: {(int)cms.Query(LOCATOR_KEY, AnomalyType.Space.ToString())}");
            Console.WriteLine($"Stars: {(int)cms.Query(LOCATOR_KEY, AnomalyType.Star.ToString())}");
            Console.WriteLine($"Planets: {(int)cms.Query(LOCATOR_KEY, AnomalyType.Planet.ToString())}");
            Console.WriteLine($"Moons: {(int)cms.Query(LOCATOR_KEY, AnomalyType.Moon.ToString())}");
            Console.WriteLine($"Humans: {(int)cms.Query(LOCATOR_KEY, AnomalyType.Human.ToString())}");
            Console.WriteLine($"Whales: {(int)cms.Query(LOCATOR_KEY, AnomalyType.Whale.ToString())}");
            Console.WriteLine($"Petunias: {(int)cms.Query(LOCATOR_KEY, AnomalyType.Petunia.ToString())}");
            Console.WriteLine($"Hitchhikers: {(int)cms.Query(LOCATOR_KEY, AnomalyType.Hitchhiker.ToString())}");
        }

        // As you see above and below, we're using the cms.Query(...) command to get the (approximate) counts
        // of anomalies (and empty space) we've seen.
        public int CountAnomalies()
        {
            var stars = (int)cms.Query(LOCATOR_KEY, AnomalyType.Star.ToString());
            var planets = (int)cms.Query(LOCATOR_KEY, AnomalyType.Planet.ToString());
            var moons = (int)cms.Query(LOCATOR_KEY, AnomalyType.Moon.ToString());
            var humans = (int)cms.Query(LOCATOR_KEY, AnomalyType.Human.ToString());
            var petunias = (int)cms.Query(LOCATOR_KEY, AnomalyType.Petunia.ToString());
            var whales = (int)cms.Query(LOCATOR_KEY, AnomalyType.Whale.ToString());
            var hitchhikers = (int)cms.Query(LOCATOR_KEY, AnomalyType.Hitchhiker.ToString());
            return stars + planets + moons + humans + petunias + whales + hitchhikers;
        }

        // Anytime we encounter an anomaly, we check to see if it's a hitchhiker.
        public int CountHitchhikers()
        {
            return (int)cms.Query(LOCATOR_KEY, AnomalyType.Hitchhiker.ToString());
        }

        private AnomalyType IdentifyAnomalies(double scanResult)
        {
            if (scanResult <= SpaceProbabilities.HITCHHIKER)
            {
                return AnomalyType.Hitchhiker;
            }

            if (scanResult <= SpaceProbabilities.PETUNIA_WHALE)
            {
                return DateTime.Now.Ticks % 2 == 0 ? AnomalyType.Petunia : AnomalyType.Whale;
            }

            if (scanResult <= SpaceProbabilities.HUMAN)
            {
                return AnomalyType.Human;
            }

            if (scanResult <= SpaceProbabilities.STAR)
            {
                return AnomalyType.Star;
            }

            if (scanResult <= SpaceProbabilities.PLANET)
            {
                return AnomalyType.Planet;
            }

            if (scanResult <= SpaceProbabilities.MOON)
            {
                return AnomalyType.Moon;
            }

            return AnomalyType.Space;
        }
    }
}
