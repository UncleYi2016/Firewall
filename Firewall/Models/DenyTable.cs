using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firewall.Models {
    class DenyTable : ITable {
        private string _filePath = "./deny.txt";
        public string FilePath {
            get { return _filePath; }
            set { _filePath = value; }
        }

        public DenyTable() {

        }

        public DenyTable(string filePath) {
            try {
                FilePath = filePath;
            } catch (Exception ex) {

            }
        }

        public void write(string content) {
            while (true) {
                try {
                    StreamWriter sw = File.AppendText(FilePath);
                    sw.WriteLine(content);
                    sw.Close();
                    break;
                } catch {
                    continue;
                }
            }
            
        }

        public ArrayList read() {
            StreamReader sr = new StreamReader(FilePath);
            ArrayList denies = new ArrayList();
            while(sr.Peek() >= 0) {
                denies.Add(sr.ReadLine());
            }
            sr.Close();
            return denies;
        }
        public void modify() {
            throw new NotImplementedException();
        }

        public void delete() {
            throw new NotImplementedException();
        }
    }
}
