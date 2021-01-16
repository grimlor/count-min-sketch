using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace count_min_sketch.RedisShim
{
    public class CountMinSketchShim
    {
        private readonly ConnectionMultiplexer redisConn;

        // StackOverflow.Redis does not have native RedisBloom support.
        // So, I created this shim using it's support for direct execution of arbitrary commands.
        public CountMinSketchShim(ConnectionMultiplexer redisConn)
        {
            this.redisConn = redisConn;
        }

        public string InitByDim(string key, int width, int depth)
        {
            return (string)redisConn.GetDatabase().Execute("CMS.INITBYDIM", key, width, depth);
        }

        public string InitByProb(string key, float error, float probability)
        {
            return (string)redisConn.GetDatabase().Execute("CMS.INITBYPROB", key, error, probability);
        }

        public RedisResult IncrBy(string key, string item, int increment=1)
        {
            return redisConn.GetDatabase().Execute("CMS.INCRBY", key, item, increment);;
        }

        public RedisResult Query(string key, string item)
        {
            return redisConn.GetDatabase().Execute("CMS.QUERY", key, item);;
        }

        public RedisResult Merge(string dest, string[] sources, int[] weights)
        {
            if (sources.Length != weights.Length)
            {
                throw new ArgumentException("sources.Length must equal weights.Length");
            }
            
            return redisConn.GetDatabase().Execute("CMS.MERGE", dest, sources.Length, sources, "WEIGHTS", weights);
        }

        public Task<RedisResult> MergeAsync(string dest, string[] sources, int[] weights)
        {
            if (sources.Length != weights.Length)
            {
                throw new ArgumentException("sources.Length must equal weights.Length");
            }
            
            return redisConn.GetDatabase().ExecuteAsync("CMS.MERGE", dest, sources.Length, sources, "WEIGHTS", weights);
        }
    }
}