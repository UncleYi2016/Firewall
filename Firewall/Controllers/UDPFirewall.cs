using Firewall.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Firewall.Controllers {
    class UDPFirewall : Firewall{
        private int bindPort;
        private string serverIP;
        private int serverPort;
        private LogTable lt;
        private DenyTable dt;
        private Dictionary<int, IPEndPoint> proxyTable = new Dictionary<int, IPEndPoint>();
        private List<Socket> sockets = new List<Socket>();
        private IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());



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


        public UDPFirewall(string serverIP, int serverPort) : base() {
            bindPort = 8001;
            this.serverIP = serverIP;
            this.serverPort = serverPort;
            setLogTable();
            setDenyTable();
            startListenThread();
        }

        public UDPFirewall(string serverIP, int serverPort, int totalBineWidth):base(totalBineWidth) {
            bindPort = 8001;
            this.serverIP = serverIP;
            this.serverPort = serverPort;
            setLogTable();
            setDenyTable();
            startListenThread();
        }

        public UDPFirewall(int bindPort, string serverIP, int serverPort) : base() {
            this.bindPort = bindPort;
            this.serverIP = serverIP;
            this.serverPort = serverPort;
            setLogTable();
            setDenyTable();
            startListenThread();
        }

        public UDPFirewall(int bindPort, string serverIP, int serverPort, int totalBineWidth):base(totalBineWidth) {
            this.bindPort = bindPort;
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
            Socket serverSock = null;
            try {
                int recvNum;
                byte[] data = new byte[10240];
                IPEndPoint localIp = null;
                foreach (IPAddress ipa in IpEntry.AddressList) {
                    if (ipa.AddressFamily == AddressFamily.InterNetwork) {
                        localIp = new IPEndPoint(ipa, bindPort);
                        break;
                    }
                }
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

                if (MainWindow.activicePort.Contains(bindPort)) {
                    MessageBox.Show("Port has been used.");
                    throw new Exception();
                }

                serverSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                serverSock.Bind(localIp);
                Success = true;
                MainWindow.activicePort.Add(bindPort);
                sockets.Add(serverSock);
                EndPoint server = serverEndPoint;
                EndPoint remote = new IPEndPoint(IPAddress.Any, 0);

                int port = 23000;

                while (true) {
                    ArrayList denies = dt.read();
                    if (port > 23500) {
                        port = 23000;
                    }
                    recvNum = serverSock.ReceiveFrom(data, ref remote);
                    Lt.write("********************************");
                    Lt.write("Time: " + DateTime.Now.ToString());
                    Lt.write("Connect IP: " + (remote as IPEndPoint).Address.ToString());
                    Lt.write("Connect port: " + (remote as IPEndPoint).Port.ToString());
                    Lt.write("Destination Port: " + bindPort);
                    Lt.write("Protocol: UDP");
                    if (denies.Contains((remote as IPEndPoint).Address.ToString())) {
                        Lt.write("Operation: Reject");
                        Lt.write("********************************");
                        Console.WriteLine("Message received from " + remote.ToString() + "\t reject.");
                        continue;
                    }
                    Lt.write("Operation: Accept");
                    Lt.write("********************************");
                    if (!remote.ToString().Equals(server.ToString())) {
                        while (proxyTable.ContainsKey(port)) {
                            port++;
                        }
                        IPEndPoint IPRemote = remote as IPEndPoint;
                        string ip = IPRemote.Address.ToString();
                        int tarPort = IPRemote.Port;
                        proxyTable.Add(port, IPRemote);
                        Thread proxy = new Thread(proxyThread);
                        object[] param = new object[5] { port, data, server, tarPort, recvNum };
                        port++;
                        proxy.Start(param);
                    }
                }
            } catch (Exception ex){
                
            } finally {
                if (serverSock != null) {
                    serverSock.Close();
                }
                Success = false;
                MainWindow.activicePort.Remove(bindPort);
                stop();
            }
            
        }

        //Act as proxy
        private void proxyThread(object paramObj) {
            object[] param = (object[])paramObj;
            int port = (int)param[0];
            int tarPort = (int)param[3];
            byte[] data = (byte[])param[1];
            byte[] data2 = new byte[1024];
            EndPoint server = (EndPoint)param[2];
            int recv = (int)param[4];
            Socket proxySocket = null;
            try {
                IPEndPoint proxyIp = new IPEndPoint(IPAddress.Any, port);
                proxySocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                proxySocket.Bind(proxyIp);
                sockets.Add(proxySocket);
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                EndPoint remote = sender;
                recv = proxySocket.SendTo(data, recv, SocketFlags.None, server);
                recv = proxySocket.ReceiveFrom(data2, ref remote);
                if (proxyTable.ContainsKey(port)) {
                    IPEndPoint targetIpRemote = proxyTable[port];
                    //string data2Str = ASCIIEncoding.Unicode.GetString(data2);
                    //int count = data2Str.Length;
                    recv = proxySocket.SendTo(data2, recv, SocketFlags.None, targetIpRemote);
                    proxyTable.Remove(port);
                }
            } catch (Exception ex){
                //MessageBox.Show("Proxy falied!");
            } finally {
                if(proxySocket != null) {
                    proxySocket.Close();
                    sockets.Remove(proxySocket);
                }
                Thread.CurrentThread.Abort();
            }
        }

        public void stop() {
            try {
                foreach (Socket s in sockets) {
                    if (s != null) {
                        s.Close();
                    }
                }
                sockets.Clear();
            } catch (Exception ex){ } finally {
                MainWindow.activicePort.Remove(bindPort);
            }
        }


        public int BindPort {
            get {
                return bindPort;
            }

            set {
                bindPort = value;
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

        public int ServerPort {
            get {
                return serverPort;
            }

            set {
                serverPort = value;
            }
        }
    }
}
