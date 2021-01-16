package info.jackpines;

import info.jackpines.redisshim.CountMinSketchShim;
import java.util.List;
import redis.client.RedisClient;
import redis.client.RedisClientBase;

public class SimpleCmsExample {

  public static void runDemo( RedisClientBase client ) {
    // I created a shim to make typos less likely
    var cms = new CountMinSketchShim( client );

    var key1 = "key1";
    var key2 = "key2";

    try {
      // There are 2 ways to initialize the count-min sketch
      var initResult = cms.initByDim( key1, 1000, 10 );
      if ( initResult.equals( "OK" ) ) {
        System.out.printf( "%s init: %s%n", key1, initResult );
      }

      // The first float is an estimate of allowable error as a percent of total counted items; affects width
      // The second is the probability for an inflated count, aka false positives
      initResult = cms.initByProb( key2, 0.001f, 0.01f );
      if ( initResult.equals( "OK" ) ) {
        System.out.printf( "%s init: %s%n", key2, initResult );
      }

      // Now to check that key1 doesn't have a value for "a"
      var result = cms.query( key1, "a" );
      System.out.printf( "Current %s.a value: %d%n", key1, result );

      // Let's increment the value stored for "a"
      cms.incrBy( key1, "a", 100 );
      // Now to re-query and see it has that value
      result = cms.query( key1, "a" );
      System.out.printf( "%s.a value after incrementing: %d%n", key1, result );

      // Let's check that key2 has no value stored for "b"
      result = cms.query( key2, "b" );
      System.out.printf( "current %s.b value: %d%n", key2, result );

      // Now let's increment "b"; cms.IncrBy(...) also returns the value
      result = cms.incrBy( key2, "b", 200 );
      System.out.printf( "%s.b value after incrementing: %d%n", key2, result );

      // Finally, let's check to see that there's no cross-over
      result = cms.query( key1, "b" );
      System.out.printf( "current %s.b value: %d%n", key1, result );

      result = cms.query( key2, "a" );
      System.out.printf( "current %s.a value: %d%n", key2, result );
    } catch ( Exception e ) {
      System.out.println( e.getMessage() );
    } finally {
      Main.cleanup( (RedisClient) client, List.of( key1, key2 ) );
    }
  }
}
