using Firewall.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Firewall {
    /// <summary>
    /// DetailSettingControl.xaml 的交互逻辑
    /// </summary>
    public partial class DetailSettingControl : UserControl {
        private string id = null;
        private string bindAddr = null;
        private string bindPort = null;
        private string fwPort = null;
        private string configName = null;
        private int type = 0;
        private TCPFirewall tcp = null;
        private UDPFirewall udp = null;
        private TabItem thisTab = null;
        private TabControl thisControl = null;
        public DetailSettingControl(string id, TabItem thisTab, TabControl thisControl) {
            InitializeComponent();
            this.id = id;
            this.thisTab = thisTab;
            this.thisControl = thisControl;
            Thread checkStatus = new Thread(checkSuccess);
            checkStatus.Name = "check:" + bindAddr + ":" + bindPort;
            checkStatus.Start();
        }

        public DetailSettingControl(JObject config, TabItem thisTab, TabControl thisControl) {
            InitializeComponent();
            bindAddrText.Text = config["bindAddr"] == null ? "" : config["bindAddr"].ToString();
            bindPortText.Text = config["bindPort"] == null ? "" : config["bindPort"].ToString();
            fwPortText.Text = config["fwPort"] == null ? "" : config["fwPort"].ToString();
            nameText.Text = config["configName"] == null ? "" : config["configName"].ToString();
            TypeCombo.SelectedIndex = config["type"] == null ? 0 : int.Parse(config["type"].ToString());
            id = config["id"] == null ? Guid.NewGuid().ToString() : config["id"].ToString();
            bindAddr = bindAddrText.Text;
            bindPort = bindPortText.Text;
            fwPort = fwPortText.Text;
            configName = nameText.Text;
            type = TypeCombo.SelectedIndex;
            this.thisTab = thisTab;
            this.thisControl = thisControl;
            Thread checkStatus = new Thread(checkSuccess);
            checkStatus.Name = "check:" + bindAddr + ":" + bindPort;
            checkStatus.Start();
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e) {
            JObject config = new JObject();
            bool modify = false;
            bindAddr = bindAddrText.Text;
            bindPort = bindPortText.Text;
            fwPort = fwPortText.Text;
            configName = nameText.Text;
            type = TypeCombo.SelectedIndex;
            config.Add("bindAddr", bindAddr);
            config.Add("bindPort", bindPort);
            config.Add("fwPort", fwPort);
            config.Add("configName", configName);
            config.Add("type", type);
            config.Add("id", id);
            for(int i = 0; i < MainWindow.connectionConfigs.Count; i++) {
                if (MainWindow.connectionConfigs[i]["id"].ToString().Equals(id)) {
                    MainWindow.connectionConfigs[i] = config;
                    modify = true;
                    break;
                }
            }
            if (!modify) {
                MainWindow.connectionConfigs.Add(config);
            }
            thisTab.Header = configName;
            string output = JsonConvert.SerializeObject(MainWindow.connectionConfigs);
            File.WriteAllText("configs.json", output);

        }

        private void startBtn_Click(object sender, RoutedEventArgs e) {
            if( bindAddr == null || bindPort == null || fwPort == null) {
                MessageBox.Show("Address, port and firewall port are required!");
                return;
            }
            try {
                if (type == 0) {
                    tcp = new TCPFirewall(int.Parse(fwPort), bindAddr, int.Parse(bindPort), MainWindow.bandwidth);
                    MainWindow.Tcps.Add(tcp);
                } else if (type == 1) {
                    udp = new UDPFirewall(int.Parse(fwPort), bindAddr, int.Parse(bindPort));
                    MainWindow.Udps.Add(udp);
                }
            }catch {
                MessageBox.Show("Proxy start failed.");
            }
        }

        private void stopBtn_Click(object sender, RoutedEventArgs e) {
            if(tcp != null) {
                tcp.stop();
            }
            if(udp != null) {
                udp.stop();
            }
            
        }

        private void delBtn_Click(object sender, RoutedEventArgs e) {
            try {
                if (tcp != null) {
                    tcp.stop();
                }
                if (udp != null) {
                    udp.stop();
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
            
            for (int i = 0; i < MainWindow.connectionConfigs.Count; i++) {
                if (MainWindow.connectionConfigs[i]["id"].ToString().Equals(id)) {
                    MainWindow.connectionConfigs.RemoveAt(i);
                    break;
                }
            }
            thisControl.Items.Remove(thisTab);
            string output = JsonConvert.SerializeObject(MainWindow.connectionConfigs);
            File.WriteAllText("configs.json", output);
        }

        private void checkSuccess() {
            while (true) {
                if(tcp != null && tcp.Success == true) {
                    stopBtn.Dispatcher.Invoke(new Action(() => { this.stopBtn.Visibility = Visibility.Visible; }));
                    startBtn.Dispatcher.Invoke(new Action(() => { this.startBtn.Visibility = Visibility.Hidden; }));
                } else if(udp!= null && udp.Success == true) {
                    stopBtn.Dispatcher.Invoke(new Action(() => { this.stopBtn.Visibility = Visibility.Visible; }));
                    startBtn.Dispatcher.Invoke(new Action(() => { this.startBtn.Visibility = Visibility.Hidden; }));
                } else {
                    stopBtn.Dispatcher.Invoke(new Action(() => { this.stopBtn.Visibility = Visibility.Hidden; }));
                    startBtn.Dispatcher.Invoke(new Action(() => { this.startBtn.Visibility = Visibility.Visible; }));
                }
                Thread.Sleep(500);
            }
        }

        private void bindPortText_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            Regex re = new Regex("[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void fwPortText_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            Regex re = new Regex("[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
        }
    }
}
