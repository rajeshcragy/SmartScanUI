using AxCZUROcxLib;
using Newtonsoft.Json;
using NLog;
using SmartScanUI.Scanner.CZUR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartScanUI.Scanner
{
    public class ScannerHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        CZUR.CzurEngine engine;

        public event EventHandler<StatusChangedEventArgs> StatusChanged;

        public ScannerHelper(AxCZUROcxLib.AxCZUROcx activeXScanner)
        {
            engine = new CZUR.CzurEngine(activeXScanner);
            activeXScanner.CZUR_EVENT_IMAGE += CZUROCX_EVENT_IMAGE;
            //axCZUROcx1.CZUR_EVENT_PDF += CZUROCX_EVENT_PDF;
            //axCZUROcx1.CZUR_EVENT_HTTP += CZUROCX_EVENT_HTTP;
            //axCZUROcx1.CZUR_EVENT_RECORD += CZUROCX_EVENT_RECORD;
            //axCZUROcx1.CZUR_EVENT_BASE64 += CZUROCX_EVENT_BASE64;
            //axCZUROcx1.CZUR_EVENT_GRAB += CZUROCX_EVENT_GRAB;
            //axCZUROcx1.CZUR_EVENT_THUMBNAIL += CZUROCX_EVENT_THUMBNAIL;
            //axCZUROcx1.CZUR_EVENT_OCR += CZUROCX_EVENT_OCR;
            //axCZUROcx1.CZUR_EVENT_MULTIOBJ += CZUROCX_EVENT_MULTIOBJ;
            //axCZUROcx1.CZUR_EVENT_CVR += CZUROCX_EVENT_CVR;
            //axCZUROcx1.CZUR_EVENT_CFM += CZUROCX_EVENT_CFM;
        }

        private void OnStatusChanged(string status)
        {
            StatusChanged?.Invoke(this, new StatusChangedEventArgs { Status = status, Timestamp = DateTime.Now });
        }

        public string Initialize()
        {
            string status = String.Empty;

            OnStatusChanged("Initializing scanner...");
            Logger.Info("Initializing scanner...");
            status = engine.Initialize();
            if (!string.IsNullOrEmpty(status))
            {
                OnStatusChanged($"Initialization failed: {status}");
                Logger.Error("Initialization failed: {0}", status);
                return status;
            }
            OnStatusChanged("Scanner initialized. Opening device...");
            Logger.Info("Scanner initialized. Opening device...");

            status = engine.OpenDevice(0, 1536, 1152, 50, 50);
            if (!string.IsNullOrEmpty(status))
            {
                OnStatusChanged($"Failed to open device: {status}");
                Logger.Error("Failed to open device: {0}", status);
                return status;
            }
            OnStatusChanged("Device opened. Configuring process type...");
            Logger.Info("Device opened. Configuring process type...");

            status = engine.SetProcessType(4);
            if (!string.IsNullOrEmpty(status))
            {
                OnStatusChanged($"Failed to set process type: {status}");
                Logger.Error("Failed to set process type: {0}", status);
                return status;
            }

            OnStatusChanged("Scanner ready. Waiting for scan...");
            Logger.Info("Scanner ready. Waiting for scan...");
            return string.Empty;
        }

        public void UnInitialize()
        {
            try
            {
                string status = String.Empty;
                status = engine.Uninitialize();
                OnStatusChanged("Scanner Reset Successful.");
                Logger.Info("Scanner Reset Successful");
            }
            catch (Exception ex)
            {
                OnStatusChanged($"Error while resetting the scanner:{ex.Message}");
                Logger.Error(ex, "Error while resetting the scanner");
            }
        }

        public class StatusChangedEventArgs : EventArgs
        {
            public string Status { get; set; }
            public DateTime Timestamp { get; set; }
        }
        public class BarCodeItem
        {
            public string barCode { get; set; }
            public int type { get; set; }
        }
        private void CZUROCX_EVENT_IMAGE(object sender, AxCZUROcxLib._DCZUROcxEvents_CZUR_EVENT_IMAGEEvent e)
        {
            CzurImageEventStatus eventStatus = (CzurImageEventStatus)e.error;
            switch (eventStatus)
            {
                case CzurImageEventStatus.Success:
                    {
                        List<BarCodeItem> barCodeItems = JsonConvert.DeserializeObject<List<BarCodeItem>>(e.bsrBarcode);
                        if (barCodeItems != null)
                        {
                            if (barCodeItems.Count() > 1)
                            {
                                Logger.Debug("BarCodes Found: {0} barcodes detected", barCodeItems.Count());
                                //BarCodes Found, you can handle multiple barcodes here
                            }
                            else
                            {
                                Logger.Debug("No barCode Found");
                                //No barCode Found, you can handle this case here
                            }
                        }
                    }
                    break;
                case CzurImageEventStatus.ImageProcessingError:
                    {
                        Logger.Error("Image processing error occurs");
                    }
                    break;
                case CzurImageEventStatus.InsufficientSystemMemory:
                    {
                        Logger.Error("Insufficient system memory, image processing failed");
                    }
                    break;
                case CzurImageEventStatus.BlankImageDetected:
                    {
                        Logger.Warn("Blank image detected. The file cannot be saved");
                    }
                    break;
                case CzurImageEventStatus.FailedToSaveImageFile:
                    {
                        Logger.Error("Failed to save image file, please check if the path is valid and has sufficient permissions");
                    }
                    break;
                case CzurImageEventStatus.PDFPasswordProtected:
                    {
                        Logger.Error("Existing PDFs are password protected. Images cannot be saved");
                    }
                    break;
            }
        }

    }
}
