namespace Store.API.Helper;

public sealed class RateLimitCounter
{
    public DateTime Timestamp { get; set; }
    public int Count { get; set; }
}
