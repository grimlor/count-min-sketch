using System;
using System.Collections.Generic;
using count_min_sketch.RedisShim;
using StackExchange.Redis;

namespace count_min_sketch
{
    public class SimpleCmsExample
    {
        public static void RunDemo(ConnectionMultiplexer redisConn)
        {
            // I created a shim to make typos less likely.
            var cms = new CountMinSketchShim(redisConn);

            var key1 = "key1";
            var key2 = "key2";
            var key3 = "merged";

            try
            {
                // There are 2 ways to initialize the count-min sketch
                var initResult = (string)cms.InitByDim(key1, 1000, 10);
                if (initResult.Equals("OK"))
                {
                    Console.WriteLine($"{key1} init: {initResult}");
                }

                // The first float is an estimate of allowable error as a percent of total counted items; affects width
                // The second is the probability for an inflated count, aka false positives
                initResult = cms.InitByProb(key2, 0.001f, 0.01f);
                if (initResult.Equals("OK"))
                {
                    Console.WriteLine($"{key2} init: {initResult}");
                }

                // Now to check that key1 doesn't have a value for "a"
                var result = cms.Query(key1, "a");
                Console.WriteLine($"Current {key1}.a value: {(int)result}");
                
                // Let's increment the value stored for "a"
                cms.IncrBy(key1, "a", 100);
                // Now to requery and see it has that value
                result = cms.Query(key1, "a");
                Console.WriteLine($"{key1}.a value after incrementing: {(int)result}");

                // Let's check that key2 has no value stored for "b"
                result = cms.Query(key2, "b");
                Console.WriteLine($"current {key2}.b value: {(int)result}");

                // Now let's increment "b"; cms.IncrBy(...) also returns the value
                result = cms.IncrBy(key2, "b", 200);
                Console.WriteLine($"{key2}.b value after incrementing: {(int)result}");

                // Finally, let's check to see that there's no cross-over
                result = cms.Query(key1, "b");
                Console.WriteLine($"current {key1}.b value: {(int)result}");

                result = cms.Query(key2, "a");
                Console.WriteLine($"current {key2}.a value: {(int)result}");

                // Unfortunately, StackOverflow.Redis throws a timeout exception for cms.Merge(...)
                initResult = cms.InitByProb(key3, 0.001f, 0.01f);
                if (initResult.Equals("OK"))
                {
                    Console.WriteLine($"{key3} init: {initResult}");
                }

                result = cms.Merge(key3, new string[] {key1, key2}, new int[]{1, 1});
                Console.WriteLine($"current {key3}.a value: {(int)result}");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
            finally
            {
                Cleanup(redisConn, new List<string>(new [] {key1, key2, key3}));
            }
        }

        private static void Cleanup(ConnectionMultiplexer redisConn, List<string> cmsKeys)
        {
            // There is nothing special about the CMS keys; they're just Redis keys so we use a standard delete
            // call to delete them from Redis.
            var db = redisConn.GetDatabase();
            foreach (var key in cmsKeys)
            {
                // Supports a RedisKey[] but not string[]; will implicitly convert a single string to a RedisKey, though.
                db.KeyDelete(key);
            }
            Console.WriteLine("Cleanup complete");
        }
    }
}