public abstract class RedisData<T> : RedisData
{
    public abstract T Value { get; set; }
}
public abstract class RedisData
{
    public abstract string Format();
}

public static class RedisDataExtensions
{
    public static string ToResponse(this RedisData data)
    {
        return $"{data.Format()}\r\n";
    }
}

public class RedisSimpleString : RedisData<string>
{
    public override string Value { get; set; }

    public override string Format()
    {
        return $"+{Value}";
    }
}

public class RedisBulkString : RedisData<string>
{
    public override string Value { get; set; }

    public override string Format()
    {
        return $"${Value.Length}\r\n{Value}";
    }
}

public class RedisArray : RedisData<RedisData[]>
{
    public override RedisData[] Value { get; set; }

    public override string Format()
    {
        return $"*{Value.Length}\r\n{string.Join("\r\n", Value.Select(x => x.Format()))}";
    }

}

public class RedisCommand
{
    public string Name { get; set; }
    public RedisData[] Arguments { get; set; }
}