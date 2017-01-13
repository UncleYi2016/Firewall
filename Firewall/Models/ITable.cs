using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firewall.Models {
    interface ITable {
        void write(string content);
        ArrayList read();
        void modify();
        void delete();
    }
}
