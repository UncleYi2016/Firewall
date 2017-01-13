using Firewall.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace Firewall.Controllers {
    class TCPFirewall : Firewall {
        private int bindingPort;
        private string serverIP;
        private int serverPort;
        private LogTable lt;
        private DenyTable dt;
        private List<Socket> sockets = new List<Socket>();
        private IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());

        public int ServerPort {
            get {
                return serverPort;
            }

            set {
                serverPort = value;
            }
        }

        public string ServerIP {
            get {
                return serverIP;
            }

            set {
                serverIP = value;
            }
        }

        internal LogTable Lt {
            get {
                return lt;
            }
        }

        internal DenyTable Dt {
            get {
                return dt;
            }
        }

        public bool setLogTable(string filePath) {
            try {
                lt = new LogTable(filePath);
                return true;
            } catch {
                return false;
            }
        }

        public bool setLogTable() {
            try {
                lt = new LogTable();
                return true;
            } catch {
                return false;
            }
        }

        public bool setDenyTable(string filePath) {
            try {
                dt = new DenyTable(filePath);
                return true;
            } catch {
                return false;
            }
        }

        public bool setDenyTable() {
            try {
                dt = new DenyTable();
                return true;
            } catch {
                return false;
            }
        }

        public TCPFirewall(int port, string serverIP, int serverPort) : base() {
            bindingPort = port;
            this.serverIP = serverIP;
            this.serverPort = serverPort;
            setLogTable();
            setDenyTable();
            startListenThread();
        }

        public TCPFirewall(int port, string serverIP, int serverPort, int totalBindWidth) : base(totalBindWidth) {
            bindingPort = port;
            this.serverIP = serverIP;
            this.serverPort = serverPort;
            setLogTable();
            setDenyTable();
            startListenThread();
        }

        private void startListenThread() {
            Thread listen = new Thread(listenThread);
            listen.Start();
        }

        //Listen incoming connections.
        private void listenThread() {
            Socket clientSocket = null;
            string clientAddrStr = null;
            try {
                if (MainWindow.activicePort.Contains(bindingPort)) {
                    MessageBox.Show("Port has been used.");
                    throw new Exception();
                }
                Socket hostSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                foreach(IPAddress ipa in IpEntry.AddressList) {
                    if (ipa.AddressFamily == AddressFamily.InterNetwork) {
                        IPEndPoint ipe = new IPEndPoint(ipa, bindingPort);
                        hostSocket.Bind(ipe);
                        break;
                    } 
                }
                hostSocket.Listen(10);
                sockets.Add(hostSocket);
                Success = true;
                MainWindow.activicePort.Add(bindingPort);
                
                while (true) {
                    clientSocket = hostSocket.Accept();
                    ArrayList denies = Dt.read();
                    IPEndPoint clientEnd = clientSocket.RemoteEndPoint as IPEndPoint;
                    Lt.write("********************************");
                    Lt.write("Time: " + DateTime.Now.ToString());
                    Lt.write("Connect IP: " + clientEnd.Address.ToString());
                    Lt.write("Connect port: " + clientEnd.Port.ToString());
                    Lt.write("Destination Port: " + bindingPort);
                    Lt.write("Protocol: TCP");
                    if (denies.Contains(clientEnd.Address.ToString())) {
                        clientSocket.Close();
                        Lt.write("Operation: Reject");
                        Lt.write("********************************");
                        continue;
                    }
                    Lt.write("Operation: Accept");
                    Lt.write("********************************");
                    sockets.Add(clientSocket);
                    Count++;
                    IPAddress ip = IPAddress.Parse(ServerIP);
                    Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    serverSocket.Connect(new IPEndPoint(ip, ServerPort)); //配置服务器IP与端口 
                    sockets.Add(serverSocket);
                    clientSocket.SendTimeout = 5000;
                    serverSocket.ReceiveTimeout = 5000;
                    Socket[] socketPair = { clientSocket, serverSocket };
                    clientAddrStr = clientEnd.Address + ": " + clientEnd.Port;
                    try {
                        if (!MainWindow.connectStatuses.ContainsKey(clientAddrStr)) {
                            MainWindow.connectStatuses.Add(clientAddrStr, "0");
                        }
                    } catch (IndexOutOfRangeException indexEx) {
                    }
                    Thread receiveThread = new Thread(receiveMessage);
                    Thread sendThread = new Thread(sendMessage);
                    receiveThread.Start(socketPair);
                    receiveThread.Name = "receive:" + clientEnd.Port;
                    sendThread.Start(socketPair);
                    sendThread.Name = "send:" + clientEnd.Port;
                    
                }
            } catch (Exception ex) {
                //MessageBox.Show("Proxy falied!");
                Count--;
                if (MainWindow.connectStatuses.ContainsKey(clientAddrStr)) {
                    MainWindow.connectStatuses.Remove(clientAddrStr);
                }
                if (clientSocket != null && clientSocket.Connected) {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
                
            } finally {
                Success = false;
                MainWindow.activicePort.Remove(bindingPort);
                stop();
            }
        }

        //Thread of sending packets to server. ( Client --> Firewall --> Server )
        private void sendMessage(object socketPair) {
            Socket[] sockets = (Socket[])socketPair;
            Socket clientSocket = sockets[0];
            Socket serverSocket = sockets[1];
            IPEndPoint point = clientSocket.RemoteEndPoint as IPEndPoint;
            byte[] clientRecv = new byte[BufferSize];
            byte[] serverRecv = new byte[BufferSize];

            try {
                int receiveNumber = 0;
                int sendNumber = 0;
                do {
                    receiveNumber = clientSocket.Receive(clientRecv);
                    sendNumber = serverSocket.Send(clientRecv, receiveNumber, SocketFlags.None);
                } while (receiveNumber != 0);
            } catch (Exception ex) {
                Console.Write(ex.Message);
            } finally {
                if (clientSocket != null && clientSocket.Connected) {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
                if (serverSocket != null && serverSocket.Connected) {
                    serverSocket.Shutdown(SocketShutdown.Both);
                    serverSocket.Close();
                }
            }
            Thread.CurrentThread.Abort();

        }

        //Thread of sending packets to client. ( Server --> Firewall --> Client )
        private void receiveMessage(object socketPair) {
            Socket[] sockets = (Socket[])socketPair;
            Socket clientSocket = sockets[0];
            Socket serverSocket = sockets[1];
            IPEndPoint point = clientSocket.RemoteEndPoint as IPEndPoint;
            int receiveNumber = 0;
            int sendNumber = 0;
           
            try {
                do {
                    byte[] clientRecv = new byte[BufferSize];
                    byte[] serverRecv = new byte[BufferSize];
                    receiveNumber = serverSocket.Receive(serverRecv);
                    sendNumber = clientSocket.Send(serverRecv, receiveNumber, SocketFlags.None);
                    if (MainWindow.connectStatuses.ContainsKey(point.Address + ": " + point.Port)) {
                        MainWindow.connectStatuses[point.Address + ": " + point.Port] = ((float)sendNumber / 1024).ToString("0.00");
                    } else {
                        MainWindow.connectStatuses.Add(point.Address + ": " + point.Port, ((float)sendNumber / 1024).ToString("0.00"));
                    }
                    Thread.Sleep(1000);
                } while (receiveNumber != 0);
            } catch (Exception ex) {
                Console.Write(ex.Message);
            } finally {
                Count--;
                if (MainWindow.connectStatuses.ContainsKey(point.Address + ": " + point.Port)) {
                    MainWindow.connectStatuses.Remove(point.Address + ": " + point.Port);
                }
                if (clientSocket != null && clientSocket.Connected) {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
                if (serverSocket != null && serverSocket.Connected) {
                    serverSocket.Shutdown(SocketShutdown.Both);
                    serverSocket.Close();
                }
            }
            
        }

        //Stop all connections and sockets.
        public void stop() {
            try {
                foreach (Socket s in sockets) {
                    if (s != null) {
                        if (s.Connected) {
                            s.Shutdown(SocketShutdown.Both);
                        }
                        s.Close();
                    }
                }
                sockets.Clear();
                Success = false;
            } catch (Exception ex) {

            } finally {
                MainWindow.activicePort.Remove(bindingPort);
            }
        }
    }
}
