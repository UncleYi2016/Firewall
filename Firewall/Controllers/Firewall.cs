using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firewall.Controllers {
    class Firewall {
        private int totalBandWidth;
        private int bufferSize;
        private int count;
        private bool success = false;

        public Firewall() {
            TotalBandWidth = 1024;
        }

        public Firewall(int totalBandWidth) {
            TotalBandWidth = totalBandWidth;
            Count = 0;
        }

        public int BufferSize {
            get {
                return bufferSize;
            }
        }

        public int TotalBandWidth {
            get {
                return totalBandWidth;
            }
            set {
                totalBandWidth = value;
                if (count < 1) {
                    bufferSize = TotalBandWidth;
                } else {
                    bufferSize = TotalBandWidth / count;
                }
            }
        }

        public int Count {
            get {
                return count;
            }
            set {
                count = value;
                if(count < 1) {
                    bufferSize = TotalBandWidth;
                } else {
                    bufferSize = TotalBandWidth / count;
                }
            }
        }

        public bool Success {
            get {
                return success;
            }

            set {
                success = value;
            }
        }
    }
}
