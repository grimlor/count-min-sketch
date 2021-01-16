package info.jackpines;

import info.jackpines.hhgttg.HeartOfGold;
import java.io.IOException;
import java.util.List;
import redis.client.RedisClient;
import redis.client.RedisClientBase;

public class Main {

    public static void main(String[] args) {
        try {
            RedisClientBase client = new RedisClient( "localhost", 6379 );

            SimpleCmsExample.runDemo( client );

            HeartOfGold.scanForHitchhikers( client );

            client.close();
        } catch ( IOException e ) {
            e.printStackTrace();
        }
    }

    public static void cleanup( RedisClient client, List<String> cmsKeys ) {
        // There is nothing special about the CMS keys; they're just Redis keys so we use a standard delete
        // call to delete them from Redis.
        for (var key : cmsKeys)
        {
            // Supports a RedisKey[] but not string[]; will implicitly convert a single string to a RedisKey, though.
            client.del( key );
        }
        System.out.println("Cleanup complete");
    }
}
