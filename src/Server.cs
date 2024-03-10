using System.Net;
using System.Net.Sockets;
using System.Text;

// Uncomment this block to pass the first stage
TcpListener server = new TcpListener(IPAddress.Any, 6379);
server.Start();
var socket = server.AcceptSocket(); // wait for client
socket.Send(Encoding.UTF8.GetBytes("+PONG\r\n"));