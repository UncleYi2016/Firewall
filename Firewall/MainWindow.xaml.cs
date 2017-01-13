using Firewall.Controllers;
using Firewall.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Firewall {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        public static Dictionary<string, string> connectStatuses = new Dictionary<string, string>(1024);
        private string statusString = "";
        private static List<TCPFirewall> tcps = new List<TCPFirewall>();
        private static List<UDPFirewall> udps = new List<UDPFirewall>();
        public static List<JObject> connectionConfigs = new List<JObject>();
        public static int bandwidth = 1024;
        public static List<int> activicePort = new List<int>();
        ConfigTable ct = new ConfigTable();

        internal static List<TCPFirewall> Tcps {
            get {
                return tcps;
            }

            set {
                tcps = value;
            }
        }

        internal static List<UDPFirewall> Udps {
            get {
                return udps;
            }

            set {
                udps = value;
            }
        }

        public MainWindow() {        
            InitializeComponent();
            loadConfigs();
            Thread refreshThread = new Thread(refreshStatus);
            refreshThread.Start();
        }


        private void Window_Closed(object sender, EventArgs e) {
            Environment.Exit(0);
        }

        private void denyAddBtn_Click(object sender, RoutedEventArgs e) {
            string address = denyAddrText.Text;
            DenyTable dt = new DenyTable();
            dt.write(address);
        }

        private void refreshStatus() {
            while (true) {
                try {
                    statusString = generateStatusString(connectStatuses);
                    statusLbl.Dispatcher.Invoke(new Action(() => { this.statusLbl.Content = statusString; }));
                    Thread.Sleep(1000);
                } catch (Exception ex) { }
                
            }
        }

        private string generateStatusString(Dictionary <string, string> statuses) {
            string statusStr = "";
            try {
                foreach (KeyValuePair<string, string> status in statuses) {
                    statusStr += status.Key + "\tSpeed: " + status.Value + "KB/s \n";
                }
            } catch (Exception ex) {

            }
            return statusStr;
        }

        private void bandwidthBtn_Click(object sender, RoutedEventArgs e) {
            try {
                string bandwidthStr = null;
                float bandwidth = 0;
                int bufferSize = 0;
                bandwidthStr = bandwidthText.Text;
                bandwidth = float.Parse(bandwidthStr);
                switch (bandwidthTypeCombo.SelectedIndex) {
                    case 0:
                        break;
                    case 1:
                        bandwidth *= 1024;
                        break;
                    default:
                        break;
                }
                bufferSize = (int)bandwidth;
                ct.modify("bandwidth", bufferSize.ToString());
                foreach(TCPFirewall tcp in tcps) {
                    tcp.TotalBandWidth = bufferSize;
                }
                
            } catch (Exception ex) {
                string errMsg = ex.Message;
                MessageBox.Show(errMsg);
            }
        }

        private void Grid_Initialized(object sender, EventArgs e) {

        }

        private void newConfigBtn_Click(object sender, RoutedEventArgs e) {
            try {
                TabItem configItem = new TabItem();
                configItem.Header = "New Config";
                string id = Guid.NewGuid().ToString();
                configItem.Content = new DetailSettingControl(id, configItem, tabControl);
                tabControl.Items.Add(configItem);
            } catch (Exception ex) { }
        }

        private void loadConfigs() {
            try {
                JArray configs = JArray.Parse(File.ReadAllText("configs.json"));
                bandwidth = int.Parse(new ConfigTable().Configs["bandwidth"]);
                for (int i = 0; i < configs.Count; i++) {
                    connectionConfigs.Add(configs[i] as JObject);
                }
                foreach (JObject config in connectionConfigs) {
                    TabItem configItem = new TabItem();
                    configItem.Header = config["configName"] == null ? "" : config["configName"].ToString();
                    configItem.Content = new DetailSettingControl(config, configItem, tabControl);
                    tabControl.Items.Add(configItem);
                }
            } catch (Exception ex){
                MessageBox.Show(ex.Message);
            }
          }
    }
}
