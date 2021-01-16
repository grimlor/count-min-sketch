using StackExchange.Redis;

namespace count_min_sketch
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var redisConn = ConnectionMultiplexer.Connect("localhost"))
            {
                SimpleCmsExample.RunDemo(redisConn);
            }
        }
    }
}
