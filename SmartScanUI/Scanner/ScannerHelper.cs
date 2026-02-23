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

        public ScannerHelper(AxCZUROcxLib.AxCZUROcx activeXScanner)
        {
            engine = new CZUR.CzurEngine(activeXScanner);
        }

        private void OnStatusChanged(string status)
        {
            StatusChanged?.Invoke(this, new StatusChangedEventArgs { Status = status, Timestamp = DateTime.Now });
        }

        public string Initialize()
        {
            string status = String.Empty;

            OnStatusChanged("Initializing scanner...");
            status = engine.Initialize();
            if (!string.IsNullOrEmpty(status))
            {
                OnStatusChanged($"Initialization failed: {status}");
                return status;
            }
            OnStatusChanged("Scanner initialized. Opening device...");

            status = engine.OpenDevice(0, 1536, 1152, 50, 50);
            if (!string.IsNullOrEmpty(status))
            {
                OnStatusChanged($"Failed to open device: {status}");
                return status;
            }
            OnStatusChanged("Device opened. Configuring process type...");

            status = engine.SetProcessType(4);
            if (!string.IsNullOrEmpty(status))
            {
                OnStatusChanged($"Failed to set process type: {status}");
                return status;
            }

            OnStatusChanged("Scanner ready. Waiting for scan...");
            return string.Empty;
        }

        public void UnInitialize()
        {
            try
            {
                string status = String.Empty;
                status = engine.Uninitialize();
                OnStatusChanged($"Scanner Reset Successful.");
            }
            catch (Exception ex)
            {
                OnStatusChanged($"Error while resetting the scanner:{ex.Message}");
            }
        }

        public class StatusChangedEventArgs : EventArgs
        {
            public string Status { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}
