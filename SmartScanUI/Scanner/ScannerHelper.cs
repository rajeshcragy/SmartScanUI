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

        
        public ScannerHelper(AxCZUROcxLib.AxCZUROcx activeXScanner) {
            engine = new CZUR.CzurEngine(activeXScanner);
        }


        public string Initialize()
        {
            string status = String.Empty;
            status = engine.Initialize();
            status = engine.OpenDevice(0, 1536, 1152, 50, 50);
            status = engine.SetProcessType(4);
            return string.Empty;
        }
    }

    public class StatusChangedEventArgs : EventArgs
    {
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
