//using System;
//using System.IO;
//using System.Net;
//using System.Linq;
//using System.Threading;
//using System.Net.Sockets;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using System.Text;
//using Serilog;
//using UTanksServer.Extensions;

//namespace UTanksServer.Services.Servers.Game.Connection
//{
//    public class PlayerSocketConnection : IPlayerConnection
//    {
//        private static readonly ILogger Logger = Log.Logger.ForType<PlayerSocketConnection>();

//        public Player Player { get; set; }

//        public bool IsConnected { get; protected set; }

//        public Socket Socket { get; }
//        public IPEndPoint Endpoint => (IPEndPoint)Socket.RemoteEndPoint;

//        public AsyncQueue<ICommand> CommandQueue { get; }

//        private readonly CancellationTokenSource _cancellationTokenSource;

//        public PlayerSocketConnection(Socket socket)
//        {
//            Socket = socket;
//            CommandQueue = new AsyncQueue<ICommand>();

//            _cancellationTokenSource = new CancellationTokenSource();
//        }

//        public async Task Init()
//        {
//            Player.ClientSession = ClientSessionTemplate.CreateEntity(
//              "AI5q8XLJibe9vwx50OoS4A6nHai3oNd6U3ct96535B3azEoHfWKXQYOV6CbJfXUOBAoUvDzVbJGiOXPED9k0jAM=:AQAB"
//            );

//            await SendCommands(new InitTimeCommand());
//            Player.ShareEntities(Player.ClientSession);
//        }

//        public async Task ReceivePackets()
//        {
//            byte[] buffer = GC.AllocateArray<byte>(4096, true);
//            Memory<byte> memory = buffer.AsMemory();

//            while (true)
//            {
//                try
//                {
//                    int bytesReceived = await Socket.ReceiveAsync(memory, SocketFlags.None, _cancellationTokenSource.Token);
//                    if (bytesReceived == 0)
//                    {
//                        Logger.WithPlayer(Player).Verbose(
//                          "Lost connection"
//                        );
//                        _cancellationTokenSource.Cancel();
//                        break;
//                    }

//                    // Logger.WithPlayer(Player).Verbose(
//                    //   "Received: {@Data}",
//                    //   memory[..bytesReceived].ToArray()
//                    // );

//                    await using MemoryStream stream = new MemoryStream(buffer);
//                    using BinaryReader reader = new BigEndianBinaryReader(stream);
//                    DataDecoder decoder = new DataDecoder(reader);

//                    List<ICommand> commands = await decoder.DecodeCommands(Player);

//                    foreach (ICommand command in commands)
//                    {
//                        await command.OnReceive(Player);
//                    }

//                    Logger.WithPlayer(Player).Verbose(
//                      "Received commands: {{\n{Commands}\n}}",
//                      string.Join(",\n", commands.Select((command) => $"    {command}"))
//                    );
//                }
//                catch (InvalidPacketHeaderException exception)
//                {
//                    Logger.WithPlayer(Player).Warning(
//                      "Invalid packet header. Expected data: {ExpectedData}. Actual data: {ActualData}. Is HTTP request: {IsHttp}",
//                      InvalidPacketHeaderException.GetHex(exception.ExpectedData),
//                      InvalidPacketHeaderException.GetHex(exception.ActualData),
//                      exception.IsHttp
//                    );

//                    if (exception.IsHttp)
//                    {
//                        // Send user-friendly error message

//                        string content = "<html>\n" +
//                                         "<head>\n" +
//                                         "  <meta charset=\"utf-8\">\n" +
//                                         "  <title>Araumi</title>\n" +
//                                         "</head>\n" +
//                                         "<body>\n" +
//                                         "  <h1>400 Bad Request</h1>\n" +
//                                         "  <h2>" +
//                                         "    You tried to access Araumi game server instead of the static server.<br />" +
//                                         "    If you weren't expecting this error, contact project administration" +
//                                         "  </h2>\n" +
//                                         "  <hr />\n" +
//                                         "  <h3>Araumi, <a href=\"https://github.com/Araumi-sh/Araumi\" target=\"_blank\" rel=\"noopener\">https://github.com/Araumi-sh/Araumi</a></h3>" +
//                                         "</body>\n" +
//                                         "</html>";
//                        string response = $"HTTP/1.1 400 Bad Request\r\n" +
//                                          $"Server: Araumi\r\n" +
//                                          $"Connection: close\r\n" +
//                                          $"Content-Length: {content.Length}\r\n" +
//                                          $"\r\n" +
//                                          $"{content}";

//                        await Socket.SendAsync(Encoding.UTF8.GetBytes(response), SocketFlags.None);
//                        Socket.Shutdown(SocketShutdown.Both);
//                        Socket.Close();

//                        break;
//                    }
//                }
//                catch (MissingTypeUidException exception)
//                {
//                    Logger.WithPlayer(Player).Warning(
//                      "Missing TypeUid attribute for type {Type}",
//                      exception.Type
//                    );
//                }
//                catch (UnknownTypeUidException exception)
//                {
//                    Logger.WithPlayer(Player).Warning(
//                      "Unknown TypeUid: {TypeUid}",
//                      exception.TypeUid
//                    );
//                }
//                catch (Exception exception)
//                {
//                    Logger.Error(
//                      exception,
//                      "Unexpected exception"
//                    );
//                    break;
//                }
//            }
//        }

//        public async Task SendPackets()
//        {
//            await foreach (ICommand command in CommandQueue)
//            {
//                await SendCommands(command);
//            }
//        }

//        public void QueueCommands(params ICommand[] commands)
//        {
//            foreach (ICommand command in commands)
//            {
//                CommandQueue.Enqueue(command);
//            }
//        }

//        public async Task SendCommands(params ICommand[] commands)
//        {
//            await using MemoryStream stream = new MemoryStream();
//            await using BinaryWriter writer = new BigEndianBinaryWriter(stream);
//            DataEncoder encoder = new DataEncoder(writer);

//            await encoder.EncodeCommands(commands);

//            writer.Flush();

//            Logger.Verbose(
//              "Sending commands: {{\n{Commands}\n}}",
//              string.Join(",\n", commands.Select((command) => $"    {command}"))
//            );

//            // Logger.Verbose("Sending {@Data}...", stream.ToArray());

//            await Socket.SendAsync(stream.ToArray(), SocketFlags.None, _cancellationTokenSource.Token);
//        }
//    }
//}
