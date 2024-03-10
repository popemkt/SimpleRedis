using System.Net;
using System.Net.Sockets;
using System.Text;

// Uncomment this block to pass the first stage
TcpListener server = new TcpListener(IPAddress.Any, 6379);
server.Start();
while (true)
{
    var socket = server.AcceptSocket(); // wait for client
    await socket.SendAsync((ArraySegment<byte>)Encoding.UTF8.GetBytes("+PONG\r\n"), SocketFlags.None);
    socket.Close();
}
