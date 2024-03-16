using System.Net;
using System.Net.Sockets;
using System.Text;
using codecrafters_redis;

// Uncomment this block to pass the first stage
TcpListener server = new TcpListener(IPAddress.Any, 6379);
var internalCache = new Cache();
server.Start();

while (true)
{
    var socket = await server.AcceptSocketAsync(); // wait for client
    _ = HandleClient(socket);
}

async Task HandleClient(Socket socket)
{
    var buffer = new byte[1024];
    while (true)
    {
        var bytes = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
        var parser = new RedisRespParser(new MemoryStream(buffer));
        var command = parser.ParseCommand();

        var response = HandleCommand(command);

        Response HandleCommand(RedisCommand redisCommand)
        {
            switch (redisCommand.Name.ToLower())
            {
                case "ping":
                    return new Response { Data = new RedisSimpleString { Value = "PONG" } };
                case "echo":
                    return new Response { Data = redisCommand.Arguments[0] };
                case "set":
                    internalCache.Set((redisCommand.Arguments[0] as RedisBulkString).Value,
                        redisCommand.Arguments[1]);
                    return new Response { Data = new RedisSimpleString { Value = "OK" } };
                case "get":
                    var value = internalCache.Get((redisCommand.Arguments[0] as RedisBulkString).Value);
                    return new Response
                    {
                        Data = value
                    };
                default:
                    throw new Exception($"Unsupported command: {redisCommand.Name}");
            }

            throw new NotImplementedException();
        }

        await socket.SendAsync(Encoding.UTF8.GetBytes(response.Data.ToResponse()), SocketFlags.None);
    }
}

public class Response
{
    public RedisData Data { get; set; }
}