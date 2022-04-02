/*
using Helios.Net;
using Helios.Net.Bootstrap;
using Helios.Ops.Executors;
using Helios.Reactor.Bootstrap;
using Helios.Topology;
using Helios.Reactor;
*/
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Collections.Generic;

using Newtonsoft.Json;
using System.Windows;

namespace container_capturer.scrip_lib
{
    /*
    internal class tcpSeverWithHelios
    {
        [Obsolete]
        private static List<IConnection> listConnectTCP = new List<IConnection>();
        [Obsolete]
        private static IReactor server = null;
        /// <summary>
        /// Khởi tạo TCP server - mỗi POS có trạng thái RUN = true thì khởi tạo server đó
        /// </summary>
        [Obsolete]
        public static void CreateTCPServer(IPAddress ip, int port)
        {
            try
            {
                var host = ip;
                Console.WriteLine(string.Format("Starting server on {0}:{1}", host, port));
                var executor = new TryCatchExecutor(exception => Console.WriteLine(string.Format("Unhandled exception: {0}", exception)));
                var bootstrapper = new ServerBootstrap().WorkerThreads(2).Executor(executor).SetTransport(TransportType.Tcp).Build();
                server = bootstrapper.NewReactor(NodeBuilder.BuildNode().Host(host).WithPort(port));

                server.OnConnection += (address, connection) =>
                {
                    Console.WriteLine(string.Format("Connected: " + connection.RemoteHost));
                    connection.BeginReceive(ReceivedCallback);
                    listConnectTCP.Add(connection);
                };

                server.OnDisconnection += (reason, connection) =>
                {
                    Console.WriteLine(string.Format("Disconnected: {0}; Reason: {1}", connection.RemoteHost, reason.Type));
                    listConnectTCP.Remove(connection);
                };
                server.Start();
                Console.WriteLine(string.Format("Started server on {0}:{1}", host, port));
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Start server error: "));
                Console.WriteLine(ex.ToString());
            }
        }

        [Obsolete]
        private static void ReceivedCallback(NetworkData data, IConnection channel)
        {
            Console.WriteLine(string.Format("Raw command: {0}", data.ToString()));
            try
            {
                var sData = Encoding.UTF8.GetString(data.Buffer);

                switch (sData)
                {
                    case "capture":
                        if (parametters.setCommand("capture"))
                        {
                            sendToClient("Command've been set", channel);
                            while (parametters.getCommand() == sData)
                            {
                                Console.WriteLine("Doing {0}", sData);
                                Thread.Sleep(1000);
                            }
                            sendToClient("Command done", channel);
                        }
                        else
                        {
                            sendToClient("Setting command failed", channel);
                        }
                        break;
                    case "auto":
                        if (parametters.setCommand("auto"))
                        {
                            sendToClient("Command've been set", channel);
                            while (parametters.getCommand() == sData)
                            {
                                Console.WriteLine("Doing {0}", sData);
                                Thread.Sleep(1000);
                            }
                            sendToClient("Command done", channel);
                        }
                        else
                        {
                            sendToClient("Setting command failed", channel);
                        }
                        break;
                    case "get":
                        if (parametters.setCommand("get"))
                        {
                            while (parametters.getCommand() == sData)
                            {
                                Console.WriteLine("Doing {0}", sData);
                                Thread.Sleep(1000);
                            }
                            sendToClient(parametters.getCommandResult(), channel);
                        }
                        else
                        {
                            sendToClient("Setting command failed", channel);
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid command: {0}", sData);
                        sendToClient(string.Format("Invalid command: {0}", sData), channel);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        [Obsolete]
        private static void sendToClient(String message, IConnection channel)
        {
            byte[] messageInByte = Encoding.UTF8.GetBytes(message);
            channel.Send(new NetworkData { Buffer = messageInByte, Length = messageInByte.Length });
        }

        [Obsolete]
        private static void sendToClient(Image img, IConnection channel)
        {
            ImageConverter converter = new ImageConverter();
            byte[] messageInByte = (byte[])converter.ConvertTo(img, typeof(byte[]));
            channel.Send(new NetworkData { Buffer = messageInByte, Length = messageInByte.Length });
        }

        [Obsolete]
        private static void sendToClient(byte[] messageInByte, IConnection channel)
        {
            channel.Send(new NetworkData { Buffer = messageInByte, Length = messageInByte.Length });
        }

        [Obsolete]
        public void protocolLoop()
        {
            while (true)
            {
                if (parametters.getConnectingFlag())
                {
                    if (server != null)
                    {
                        server.Dispose(true);
                    }
                    CreateTCPServer(parametters.getSeverIP(), parametters.getseverPort());
                    parametters.setConnectingFlag(false);
                }
                Thread.Sleep(1000);
            }
        }
    }
    */
    /*
internal class tcpClientWithHelios
{
    [Obsolete]
    private IConnection iConnect;

    [Obsolete]
    private void CreateTCPClient(IPAddress _ip, int _port)
    {
        IPAddress host = _ip;
        var port = _port;
        var bootstrapper = new ClientBootstrap().SetTransport(TransportType.Tcp).Build();
        iConnect = bootstrapper.NewConnection(Node.Empty(), NodeBuilder.BuildNode().Host(host).WithPort(port));
        iConnect.OnConnection += (address, connection) =>
        {
            Console.WriteLine("Connected with host.");
            connection.BeginReceive(ReceivedCallback);
        };
        LoopConnect();
        iConnect.OnDisconnection += OnDisconnection;
    }

    /// <summary>
    /// Ngắt kết nối TCP tới POS khi không còn mở form
    /// </summary>
    [Obsolete]
    public void Disconnection()
    {
        iConnect.Close();
    }

    [Obsolete]
    private void OnDisconnection(Helios.HeliosException address, IConnection reason)
    {
        Disconnection();
        Console.WriteLine("Disconnected.");
        //CreateTCPClient(yourIp, yourPort);
    }

    [Obsolete]
    private void ReceivedCallback(NetworkData data, IConnection responseChannel)
    {
        String sReceive = Encoding.UTF8.GetString(data.Buffer);
        Console.WriteLine("Reading message: {0}", sReceive);
        if (sReceive == "Unrecognized command")
            return;
        switch (sReceive)
        {
            case "process":
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Gửi lệnh GET dữ liệu
    /// </summary>
    [Obsolete]
    public void LoopWrite(String Message)
    {
        var command = Encoding.UTF8.GetBytes(Message);
        while (iConnect.IsOpen())
        {
            iConnect.Send(new NetworkData { Buffer = command, Length = command.Length });
        }
        Console.WriteLine("Connection closed.");
    }

    /// <summary>
    /// Tự động kết nối lại khi mất kết nối
    /// </summary>
    [Obsolete]
    private void LoopConnect()
    {
        //int iRun = 0;
        while (!iConnect.IsOpen())
        {
            try
            {
                iConnect.Open();
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("re-connectting...");
        }
        Console.WriteLine("Connected");
    }

}
*/

    public class CommandJson
    {
        public string Command { get; set; }
        public IList<string> cameraLinks { get; set; }
    }

    /// <summary>
    /// Thư viện quản lí kết nối TCP/IP
    /// </summary>
    internal class tcpServer
    {
        private static TcpListener myServer;
        public tcpServer(IPAddress ip, int port)
        {
            myServer = new TcpListener(ip, port);
            myServer.Start();
            Console.Write("Open TCP gate at IP:{0}  Gate:{1} ", IPAddress.Any, port);
        }

        public tcpServer()
        {
        }

        /// <summary>
        /// Tạo sever tcp
        /// </summary>
        public void server_start()
        {
            try
            {
                myServer.Start();
                Console.Write("Open TCP gate");
                myServer.BeginAcceptTcpClient(new AsyncCallback(handle_connection), myServer);
            }
            catch (Exception)
            {
                MessageBox.Show("#Không thể kết nối tới SERVER. Vui lòng kiểm tra IP hoặc PORT.");
            }

        }

        /// <summary>
        /// Lấy đường dẫn đến camera từ string, lưu vào list và return;
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static List<string> getLinkCameraFromServer(string id)
        {
            List<string> linkCamera = new List<string>();

            /*
                 Lấy các đường linkCamera từ server và trả về vào list "linkCamera" này
            */

            // Code ví dụ:
            string[] links = id.Split(';');
            foreach (string link in links)
            {
                Console.WriteLine(link);
                linkCamera.Add(link);
            }

            return linkCamera;
        }

        /// <summary>
        /// Lọc các thành phần của lệnh gửi đến:
        ///     Lệnh "auto" : tên lệnh và đường dẫn đến camera
        ///     Lệnh khác   : tên lệnh
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static string messageDecoder(string message)
        {
            string command = "";
            if (message.Contains(parametters.auto)) // kiểm tra chuỗi "auto" có trong tin nhắn không (phát hiện lệnh auto)
            {
                command = parametters.auto; //gán lệnh là auto
                message = message.Remove(0, parametters.auto.Length+1); // xóa chuỗi "auto," có trong tin nhắn ( "auto,128" -> "128")
                string idBarie = message; //còn lại là string chứa id barrier

                List<string> linkCamera = getLinkCameraFromServer(idBarie); // hàm lấy link camera

                int i = 0;
                foreach (string linkCameraItem in linkCamera)
                {
                    parametters.setCamLink(linkCameraItem, i);
                    i++;
                }
            }
            else
            {
                command = message;
            }
            return command;
        }

        /// <summary>
        /// Hàm được gọi mỗi khi có client kết nối: nhận và trả lời các messages
        /// </summary>
        /// <param name="result"></param>
        private static void handle_connection(IAsyncResult result)  //the parameter is a delegate, used to communicate between threads
        {
            TcpListener listener = (TcpListener)result.AsyncState;
            TcpClient client = listener.EndAcceptTcpClient(result);  //creates the TcpClient
            myServer.BeginAcceptTcpClient(new AsyncCallback(handle_connection), myServer); //Tiếp tục nhận kết nối client trong thread khác
            string clientIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            string clientPort = ((IPEndPoint)client.Client.RemoteEndPoint).Port.ToString();
            try
            {
                //while (client.Connected)
                //{
                //sendToClient("Still there?\n",client);
                //Thread.Sleep(1000);
                //if (client.Available > 0) {

                byte[] buffer = new byte[512];
                NetworkStream data = client.GetStream(); // Stream quản lí các tác vụ đọc, ghi
                data.ReadTimeout = 20000; // Thời gian chờ client gửi message
                data.Read(buffer, 0, buffer.Length);
                string message = Encoding.Default.GetString(buffer).Trim((char)(00)); //chuyển các byte nhận được thành String
                Console.WriteLine("\nMessage from {0} in {1}: ", clientIp, clientPort);

                string command = messageDecoder(message);
                
                Console.WriteLine(command);
                switch (command) // Thiết lập lệnh theo message nhận được
                {
                    case "capture": //Lệnh chụp ảnh
                        if (parametters.setCommand(command))
                        {
                            sendToClient("Command've been set", client);
                            while (parametters.getCommand() == command) // Chờ lệnh hoàn thành
                            {
                                Console.WriteLine("Doing {0}", command);
                                Thread.Sleep(200);
                            }
                            sendToClient("Command done", client);
                        }
                        else
                        {
                            sendToClient("#Server đang thực hiện lệnh chụp ảnh do Client khác gửi lên. Vui lòng thực hiện lại sau khi Server thực hiện xong.", client);
                            break;
                        }
                        break;
                    case "auto": // Lệnh tự động
                        if (parametters.setCommand("capture")) // Chụp ảnh từ camera
                        {
                            //sendToClient("Command've been set", client);
                            while (parametters.getCommand() == "capture")
                            {
                                Console.WriteLine("Doing {0}", "capture");
                                Thread.Sleep(200);
                            }
                        }
                        else
                        {
                            sendToClient("#Server đang thực hiện lệnh chụp ảnh do Client khác gửi lên. Vui lòng thực hiện lại sau khi Server thực hiện xong.", client);
                            break;
                        }

                        if (parametters.setCommand("auto")) // Cắt, ghép ảnh
                        {
                            while (parametters.getCommand() == "auto")
                            {
                                Console.WriteLine("Doing {0}", "auto");
                                Thread.Sleep(200);
                            }
                        }
                        else
                        {
                            sendToClient("#Server đang thực hiện lệnh chụp ảnh do Client khác gửi lên. Vui lòng thực hiện lại sau khi Server thực hiện xong.", client);
                            break;
                        }

                        if (parametters.setCommand("get")) // trả về ảnh kết quả qua TCP
                        {
                            while (parametters.getCommand() == "get")
                            {
                                Console.WriteLine("Doing {0}", "get");
                                Thread.Sleep(200);
                            }
                            sendToClient(parametters.getCommandResult(), client);
                        }
                        else
                        {
                            sendToClient("#Server đang thực hiện lệnh chụp ảnh do Client khác gửi lên. Vui lòng thực hiện lại sau khi Server thực hiện xong.", client);
                            break;
                        }
                        break;
                    case "get": // lệnh lấy ảnh
                        if (parametters.setCommand(command))
                        {
                            //sendToClient("Command've been set", client);
                            while (parametters.getCommand() == command)
                            {
                                Console.WriteLine("Doing {0}", command);
                                Thread.Sleep(200);
                            }
                            sendToClient(parametters.getCommandResult(), client);
                            sendToClient("Command done", client);
                        }
                        else
                        {
                            sendToClient("#Server đang thực hiện lệnh chụp ảnh do Client khác gửi lên. Vui lòng thực hiện lại sau khi Server thực hiện xong.", client);
                            break;
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid command: {0}", message);
                        sendToClient("#Thông số chạy trên SERVER chưa được cài đặt hoặc thông số chưa đúng. Vui lòng liền hệ với IT để kiểm tra.", client);
                        break;
                }
                //}
                client.Close();
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine("Tcp sever had been failed: {0}", ex.Message);
                Console.WriteLine("Client {0} in {1} 've been in our memories", clientIp, clientPort);
                string ms = "";
                ms += "#Tcp sever had been failed: " + ex.Message + Environment.NewLine;
                ms += "Client " + clientIp + " in " + clientPort + " 've been in our memories";
                sendToClient(ms, client);
                client.Close();
            }
        }

        /// <summary>
        /// Gửi xuống client ở dạng String
        /// </summary>
        /// <param name="message"></param>
        /// <param name="chanel"></param>
        private static void sendToClient(string message, TcpClient chanel)
        {
            //byte[] buffer = new byte[message.Length];            
            NetworkStream data = chanel.GetStream();
            //buffer = Encoding.Default.GetBytes(message);
            byte[] buffer = UTF8Encoding.UTF8.GetBytes(message);
            data.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Gửi xuống client ở dạng ảnh (Image)
        /// </summary>
        /// <param name="img"></param>
        /// <param name="chanel"></param>
        private static void sendToClient(Image img, TcpClient chanel)
        {
            ImageConverter converter = new ImageConverter();
            byte[] buffer = (byte[])converter.ConvertTo(img, typeof(byte[]));
            NetworkStream data = chanel.GetStream();
            data.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Gửi xuống client ở dạng byte[]
        /// </summary>
        /// <param name="message"></param>
        /// <param name="chanel"></param>
        private static void sendToClient(byte[] message, TcpClient chanel)
        {
            if (message != null)
            {
                Console.WriteLine(message.Length);
                NetworkStream data = chanel.GetStream();
                for (int i = 0; i < message.Length; i++)
                {
                    if (data.CanWrite)
                    {
                        data.WriteByte(message[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Hàm chạy trong thread, quản lí việc khởi tạo/chỉnh sửa sever
        /// </summary>
        public void protocolLoop()
        {
            while (true)
            {
                if (parametters.getConnectingFlag())
                {
                    if (myServer != null)
                    {
                        myServer.Stop();
                    }
                    myServer = new TcpListener(parametters.getSeverIP(), parametters.getseverPort());
                    server_start();
                    parametters.setConnectingFlag(false);
                }
                if (parametters.killThread)
                {
                    Console.WriteLine("Say goodbye to {0}", Thread.CurrentThread.Name.ToString());
                    break;
                }
                Thread.Sleep(500);
            }
        }
    }
}