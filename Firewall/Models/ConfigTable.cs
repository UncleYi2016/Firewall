using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Firewall.Models {
    class ConfigTable : ITable {

        private string _filePath = "./config.txt";

        private Dictionary<string, string> configs = new Dictionary<string, string>();

        public ConfigTable() {
            _toTable();
        }

        public ConfigTable(string filePath) {
            FilePath = filePath;
            _toTable();
        }

        public void delete() {
            configs.Clear();
        }

        public void deleteByKey(string key) {
            if (configs.ContainsKey(key)) {
                configs.Remove(key);
            }
            _toFile();
        }

        public void refresh() {
            _toTable();
        }

        public void modify() {
            throw new NotImplementedException();
        }

        public void modify(string key, string value) {
            if (configs.ContainsKey(key)) {
                configs[key] = value;
            } else {
                configs.Add(key, value);
            }
            _toFile();
        }

        public ArrayList read() {
            throw new NotImplementedException();
        }

        public string read(string key) {
            if (configs.ContainsKey(key)) {
                return configs[key];
            }
            return null;
        }

        public void write(string content) {
            throw new NotImplementedException();
        }

        private void _toFile() {
            FileStream fs = null;
            StreamWriter sw = null;
            try {
                fs = new FileStream(FilePath, FileMode.Open, FileAccess.Write);
                fs.SetLength(0);
                fs.Close();
                sw = File.AppendText(FilePath);
                foreach (KeyValuePair<string, string> config in configs) {
                    sw.WriteLine(config.Key + "=" + config.Value);
                }
            } catch (Exception ex) {

            }finally {
                if(sw != null) {
                    sw.Close();
                }
            }
        }

        private void _toTable() {
            StreamReader sr = null;
            try {
                sr = new StreamReader(FilePath);
            } catch {
                MessageBox.Show("File path error in saving table");
                return;
            }
            ArrayList denies = new ArrayList();
            while (sr.Peek() >= 0) {
                string configStr = sr.ReadLine();
                if(configStr.IndexOf("=") >= 0) {
                    string[] keyValue = configStr.Split(new char[]{'='});
                    if (configs.ContainsKey(keyValue[0])) {
                        configs[keyValue[0]] = keyValue[1];
                    } else {
                        configs.Add(keyValue[0], keyValue[1]);
                    }
                }
            }
            sr.Close();
        }

        public string FilePath {
            get { return _filePath; }
            set { _filePath = value; }
        }

        public Dictionary<string, string> Configs {
            get {
                return configs;
            }
            set {
                configs = value;
            }
        }
    }
}
