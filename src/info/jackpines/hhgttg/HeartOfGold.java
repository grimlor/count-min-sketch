package info.jackpines.hhgttg;

import info.jackpines.Main;
import info.jackpines.RawConsoleInput;
import info.jackpines.redisshim.CountMinSketchShim;
import java.io.IOException;
import java.util.List;
import java.util.Random;
import redis.client.RedisClient;
import redis.client.RedisClientBase;

public class HeartOfGold {

  public final String LOCATOR_KEY = "locator";

  private final RedisClientBase client;
  private final CountMinSketchShim cms;
  private final Random random = new Random();

  private boolean isScanning;
  private double improbability;

  public boolean isScanning() {
    return isScanning;
  }

  public double getImprobability() {
    return improbability;
  }

  public RedisClient getClient() {
    return (RedisClient) client;
  }

  public HeartOfGold( RedisClientBase client ) throws IOException {
    this.client = client;
    Main.cleanup( getClient(), List.of( LOCATOR_KEY ) );
    RawConsoleInput.resetConsoleMode();

    // The Heart Of Gold uses a count-min sketch to keep track of the anomalies it finds during scans.
    this.cms = new CountMinSketchShim(client);
    cms.initByProb(LOCATOR_KEY, 0.001f, 0.01f);

    TheGuide.TellMeAboutSpace();

    TheGuide.TellMeMoreAboutSpace();

    this.improbability = 1;
  }

  // Mostly startup code, keyboard scanning, and output; will focus on the CMS bits.
  public static void scanForHitchhikers( RedisClientBase client ) throws IOException {
    var hog = new HeartOfGold( client );
    hog.startScanning();

    do {
      int keypress = RawConsoleInput.read( false );

      // This is the power of the Improbability Drive!
      if ( keypress == 10 ) {
        hog.increaseImprobability();
      }

      // It'll keep scanning until we either hit 'q' to quit or we find a hitchhiker.
      if ( ( (char) keypress ) == 'q' ) {
        hog.isScanning = false;
      }
    } while ( hog.isScanning() );

    hog.stopScanning();

    System.out.println();
    System.out.println();
    System.out.println( "Returning to normalcy" );

    hog.outputAllSightings();

    var countHitchhikers = hog.countHitchhikers();
    if (countHitchhikers > 0)
    {
      System.out.printf( "Found %d hitchhiker%s found at an improbability coefficient of %e!%n", countHitchhikers,
          countHitchhikers > 1 ? "s" : "", hog.getImprobability() );
    }

    RawConsoleInput.resetConsoleMode();
    System.out.println("Normalcy returned.");
    Main.cleanup( hog.getClient(), List.of( hog.LOCATOR_KEY ) );
  }

  private void startScanning() {
    isScanning = true;

    // The scan runs asynchronously
    new Thread( this::scanSpace ).start();
  }

  private void stopScanning() {
    isScanning = false;
  }

  private void increaseImprobability() {
    this.improbability *= 10;
  }

  // Here is the improbability engine. (I can dig in but suffice to say it has real probability underpinning it.)
  private void scanSpace() {
    System.out.println();
    System.out.println("Improbability drive scanning space");

    var previousAnomalies = 0L;
    var counter = 0;
    var counterResetVal = 30;
    while ( isScanning() ) {
      var scanResult = Math.abs( random.nextDouble() ) / getImprobability();
      var anomaly = identifyAnomalies( scanResult );

      // Here's where we keep track of our sightings.
      cms.incrBy(LOCATOR_KEY, anomaly.toString());

      counter = counter == counterResetVal ? 0 : counter + 1;
      if (counter % 10 == 0)
      {
        System.out.printf("Improbability level: %e, %e: %s%n", getImprobability(), scanResult, anomaly);
      }

      if ( countAnomalies() > previousAnomalies ) {
        isScanning = countHitchhikers() == 0;
        System.out.printf( "Found an anomaly at improbability level %e! Checking for hitchhikers...%s%n",
            getImprobability(), isScanning ? String.format( "%s found - continuing scan", anomaly ) : "" );
        previousAnomalies = countAnomalies();
      }
    }
  }

  private AnomalyType identifyAnomalies( double scanResult ) {
    if (scanResult <= SpaceProbabilities.HITCHHIKER)
    {
      return AnomalyType.Hitchhiker;
    }

    if (scanResult <= SpaceProbabilities.PETUNIA_WHALE)
    {
      return System.currentTimeMillis() % 2 == 0 ? AnomalyType.Petunia : AnomalyType.Whale;
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

  private long countAnomalies() {
    var stars = cms.query(LOCATOR_KEY, AnomalyType.Star.toString());
    var planets = cms.query(LOCATOR_KEY, AnomalyType.Planet.toString());
    var moons = cms.query(LOCATOR_KEY, AnomalyType.Moon.toString());
    var humans = cms.query(LOCATOR_KEY, AnomalyType.Human.toString());
    var petunias = cms.query(LOCATOR_KEY, AnomalyType.Petunia.toString());
    var whales = cms.query(LOCATOR_KEY, AnomalyType.Whale.toString());
    var hitchhikers = cms.query(LOCATOR_KEY, AnomalyType.Hitchhiker.toString());
    return stars + planets + moons + humans + petunias + whales + hitchhikers;
  }

  private long countHitchhikers() {
    return cms.query(LOCATOR_KEY, AnomalyType.Hitchhiker.toString());
  }

  private void outputAllSightings() {
    System.out.println("All sightings:");
    System.out.printf("Space: %d%n", cms.query(LOCATOR_KEY, AnomalyType.Space.toString()));
    System.out.printf("Stars: %d%n", cms.query(LOCATOR_KEY, AnomalyType.Star.toString()));
    System.out.printf("Planets: %d%n", cms.query(LOCATOR_KEY, AnomalyType.Planet.toString()));
    System.out.printf("Moons: %d%n", cms.query(LOCATOR_KEY, AnomalyType.Moon.toString()));
    System.out.printf("Humans: %d%n", cms.query(LOCATOR_KEY, AnomalyType.Human.toString()));
    System.out.printf("Whales: %d%n", cms.query(LOCATOR_KEY, AnomalyType.Whale.toString()));
    System.out.printf("Petunias: %d%n", cms.query(LOCATOR_KEY, AnomalyType.Petunia.toString()));
    System.out.printf("Hitchhikers: %d%n", cms.query(LOCATOR_KEY, AnomalyType.Hitchhiker.toString()));
  }
}
