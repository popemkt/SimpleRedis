using System.Text;

namespace codecrafters_redis;

public class RedisRespParser
{
    private const char SimpleStringChar = '+';
    private const char BulkStringChar = '$';
    private const char ArrayChar = '*'; 
    private readonly StreamReader _reader;

    public RedisRespParser(Stream stream)
    {
        _reader = new StreamReader(stream, Encoding.UTF8);
    }

    public RedisData Parse()
    {
        char dataType = (char)_reader.Read();

        switch (dataType)
        {
            case SimpleStringChar:
                return ParseSimpleString();
            case BulkStringChar:
                return ParseBulkString();
            case ArrayChar:
                return ParseArray();
            default:
                throw new FormatException($"Unsupported data type: {dataType}");
        }
    }

    private RedisSimpleString ParseSimpleString()
    {
        return new RedisSimpleString { Value = _reader.ReadLine() };
    }

    private RedisBulkString ParseBulkString()
    {
        int length = int.Parse(_reader.ReadLine());

        if (length == -1)
            return null;

        char[] buffer = new char[length];
        _reader.Read(buffer, 0, length);
        _reader.ReadLine(); // Consume the trailing \r\n

        return new RedisBulkString { Value = new string(buffer) };
    }

    private RedisArray ParseArray()
    {
        int count = int.Parse(_reader.ReadLine());

        if (count == -1)
            return null;

        RedisData[] elements = new RedisData[count];

        for (int i = 0; i < count; i++)
        {
            elements[i] = Parse();
        }

        return new RedisArray { Value = elements };
    }

    public RedisCommand ParseCommand()
    {
        _reader.Read();
        RedisArray array = ParseArray();

        if (array == null || array.Value.Length == 0)
            return null;

        RedisBulkString commandName = array.Value[0] as RedisBulkString;

        if (commandName == null)
            throw new FormatException("Invalid command format");

        RedisData[] arguments = new RedisData[array.Value.Length - 1];
        Array.Copy(array.Value, 1, arguments, 0, arguments.Length);

        return new RedisCommand
        {
            Name = commandName.Value,
            Arguments = arguments
        };
    }
}