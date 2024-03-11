using System.Net;
using System.Net.Sockets;
using System.Text;

// Uncomment this block to pass the first stage
TcpListener server = new TcpListener(IPAddress.Any, 6379);
server.Start();
while (true)
{
    var socket = server.AcceptSocket(); // wait for client
    await Task.Run(() => HandleClient(socket));
    async Task HandleClient(Socket socket)
    {
        var buffer = new byte[1024];
        while (true)
        {
            _ = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
            await socket.SendAsync(Encoding.UTF8.GetBytes("+PONG\r\n"), SocketFlags.None);
        }
    }
}