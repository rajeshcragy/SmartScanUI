using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartScanUI.Scanner
{
    public class ScannerHelper
    {
        CZUR.CzurEngine engine;

        public event EventHandler<StatusChangedEventArgs> StatusChanged;
        
        public ScannerHelper(AxCZUROcxLib.AxCZUROcx activeXScanner) {
            engine = new CZUR.CzurEngine(activeXScanner);
        }

        private void OnStatusChanged(string status)
        {
            StatusChanged?.Invoke(this, new StatusChangedEventArgs { Status = status, Timestamp = DateTime.Now });
        }

        public string Initialize()
        {
            string status = "Scanner Initializing...";
            OnStatusChanged(status);

            status = engine.Initialize();
            if (string.IsNullOrEmpty(status))
            {
                status = "Scanner initialized successfully.";
            }
            else
            {
                OnStatusChanged($"Initialization failed: {status}");
                return status;
            }
            OnStatusChanged(status);

            status = engine.OpenDevice(0, 1536, 1152, 50, 50);
            if (string.IsNullOrEmpty(status))
            {
                status = "Device opened successfully.";
            }
            else
            {
                OnStatusChanged($"Failed to open device: {status}");
                return status;
            }
            OnStatusChanged(status);


            return string.Empty;
        }
    }

    public class StatusChangedEventArgs : EventArgs
    {
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
