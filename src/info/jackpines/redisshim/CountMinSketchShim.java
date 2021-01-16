package info.jackpines.redisshim;

import java.nio.charset.StandardCharsets;
import redis.Command;
import redis.client.RedisClientBase;
import redis.reply.MultiBulkReply;
import redis.reply.StatusReply;

public class CountMinSketchShim {
  private final RedisClientBase client;

  public CountMinSketchShim( RedisClientBase client ) {
    this.client = client;
  }

  private static final String INITBYDIM = "CMS.INITBYDIM";
  private static final byte[] INITBYDIM_BYTES = INITBYDIM.getBytes( StandardCharsets.US_ASCII );

  public String initByDim( String key, int width, int depth ) {
    return ( (StatusReply) client.execute( INITBYDIM, new Command( INITBYDIM_BYTES, key, width, depth ) ) ).data();
  }

  private static final String INITBYPROB = "CMS.INITBYPROB";
  private static final byte[] INITBYPROB_BYTES = INITBYPROB.getBytes( StandardCharsets.US_ASCII );

  public String initByProb( String key, float error, float probability ) {
    return ( (StatusReply) client.execute( INITBYPROB, new Command( INITBYPROB_BYTES, key, error, probability ) ) )
        .data();
  }

  private static final String INCRBY = "CMS.INCRBY";
  private static final byte[] INCRBY_BYTES = INCRBY.getBytes(StandardCharsets.US_ASCII);

  public long incrBy( String key, String item ) {
    return incrBy( key, item, 1 );
  }

  public long incrBy( String key, String item, int increment ) {
    return Long.parseLong(
        ( (MultiBulkReply) client.execute( INCRBY, new Command( INCRBY_BYTES, key, item, increment ) ) )
            .asStringList( StandardCharsets.UTF_8 ).get( 0 ) );
  }

  private static final String QUERY = "CMS.QUERY";
  private static final byte[] QUERY_BYTES = QUERY.getBytes( StandardCharsets.US_ASCII );

  public long query( String key, String item ) {
    return Long.parseLong( ( (MultiBulkReply) client.execute( QUERY, new Command( QUERY_BYTES, key, item ) ) )
        .asStringList( StandardCharsets.UTF_8 ).get( 0 ) );
  }
}
