using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.WebSockets;

namespace DX.AzureAD.EasyOAuth.Controllers
{
    public class TokenController : ApiController
    {
        public HttpResponseMessage Get()
        {
            if (HttpContext.Current.IsWebSocketRequest)
                HttpContext.Current.AcceptWebSocketRequest(ProcessSocket);
            return new HttpResponseMessage(HttpStatusCode.SwitchingProtocols);
        }

        private static readonly Dictionary<string, WebSocket> Clients = new Dictionary<string, WebSocket>();

        internal static async Task SendTokenToClient(string socketId, string token)
        {
            //get the socket
            var socket = Clients[socketId];

            //send the token over the socket
            await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(token)), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task ProcessSocket(AspNetWebSocketContext context)
        {
            //generate a new ID for this socket connection
            var socketId = Guid.NewGuid().ToString();

            //get socket off context and send the socketId back to client
            WebSocket socket = context.WebSocket;
            await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(socketId)), WebSocketMessageType.Text, true, CancellationToken.None);

            //add the socket to the dictionary
            Clients.Add(socketId, socket);

            //keep the connection open which checking for notifications to go out
            while (true)
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
                WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                if (socket.State == WebSocketState.Open)
                {
                    string userMessage = Encoding.UTF8.GetString(
                        buffer.Array, 0, result.Count);
                    userMessage = "You sent: " + userMessage + " at " +
                        DateTime.Now.ToLongTimeString();
                    buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(userMessage));
                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else
                {
                    break;
                }
            }
        }
    }
}
