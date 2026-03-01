using AxCZUROcxLib;
using Newtonsoft.Json;
using NLog;
using SmartScanUI.Models;
using SmartScanUI.Scanner.CZUR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SmartScanUI.Scanner
{
    public class ScannerHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        CZUR.CzurEngine engine;
        MasterSettingsModel masterSettingsModel;
        ScannerSettingsModel activeScannerSettings;
        CollageModel activeCollageSettings;
        String activeUserDetailsPath;
        String activeScanningPath;
        public SessionModel sessionModel;


        public event EventHandler<StatusChangedEventArgs> StatusChanged;
        public event EventHandler<CZUR.CzurEngine.CzurImageEventArgs> ImageEvent;
        public event EventHandler<CZUR.CzurEngine.CzurHttpEventArgs> HttpEvent;
        public event EventHandler<CZUR.CzurEngine.CzurGrabEventArgs> GrabEvent;
        public event EventHandler<CZUR.CzurEngine.CzurMultiObjEventArgs> MultiObjEvent;
        public event EventHandler<CZUR.CzurEngine.CzurOcrEventArgs> OcrEvent;
        public event EventHandler<CZUR.CzurEngine.CzurPdfEventArgs> PdfEvent;
        public event EventHandler<CZUR.CzurEngine.CzurRecordEventArgs> RecordEvent;
        public event EventHandler<CZUR.CzurEngine.CzurCfmEventArgs> CfmEvent;
        public event EventHandler<CZUR.CzurEngine.CzurCvrEventArgs> CvrEvent;
        public event EventHandler<CZUR.CzurEngine.CzurThumbnailEventArgs> ThumbnailEvent;

        public ScannerHelper(AxCZUROcxLib.AxCZUROcx activeXScanner, MasterSettingsModel masterSettingsModel)
        {
            this.engine = new CZUR.CzurEngine(activeXScanner);
            this.masterSettingsModel = masterSettingsModel;
            this.activeCollageSettings=masterSettingsModel.Collage.Where(CollageModel => CollageModel.IsActive == 1).FirstOrDefault();
            this.activeScannerSettings=masterSettingsModel.Scanner.Where(ScannerSettingsModel => ScannerSettingsModel.IsActive == 1).FirstOrDefault();
            this.activeUserDetailsPath = masterSettingsModel.UserDetailsPath;
            this.activeScanningPath = masterSettingsModel.ScansPath;

            // Subscribe to all engine events
            engine.ImageEvent += Engine_ImageEvent;
            engine.HttpEvent += Engine_HttpEvent;
            engine.GrabEvent += Engine_GrabEvent;
            engine.MultiObjEvent += Engine_MultiObjEvent;
            engine.OcrEvent += Engine_OcrEvent;
            engine.PdfEvent += Engine_PdfEvent;
            engine.RecordEvent += Engine_RecordEvent;
            engine.CfmEvent += Engine_CfmEvent;
            engine.CvrEvent += Engine_CvrEvent;
            engine.ThumbnailEvent += Engine_ThumbnailEvent;
        }

        #region "Event Handlers"
        /// <summary>
        /// Resets and initializes a new SessionModel for a fresh scanning session
        /// </summary>
        private void Reset_SessionModel()
        {
            try
            {
                // Create a new session with unique ID and current timestamp
                this.sessionModel = new SessionModel
                {
                    id = Guid.NewGuid().ToString(),
                    StartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    EndTime = string.Empty,
                    PageCount = 0,
                    Pages = new List<PageModel>(),
                    Barcodes = new List<BarcodeModel>()
                };

                Logger.Info("New Session Created - Session ID: {0}, Organization: {1}, User Path: {2}", 
                    sessionModel.id, activeCollageSettings?.Name ?? "Unknown", activeScanningPath);
                Prepare_SessionPage();
                OnStatusChanged($"New Session Started - ID: {sessionModel.id}");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error creating new session model");
                OnStatusChanged($"Error creating new session: {ex.Message}");
            }
        }

        private void Prepare_ScanningDirectory()
        {
            string FailedDirectoryPath = System.IO.Path.Combine(activeScanningPath, "FailedScans");
            string SuccessDirectoryPath = System.IO.Path.Combine(activeScanningPath, "SuccessfulScans");
            string ReadytoUploadDirectoryPath = System.IO.Path.Combine(activeScanningPath, "ReadyToUpload");
            try
            {
                if(Directory.Exists(activeScanningPath) == false)
                {
                    Directory.CreateDirectory(activeScanningPath);
                    Logger.Info("Created scanning directory at path: {0}", activeScanningPath);
                }
                if(Directory.Exists(FailedDirectoryPath) == false)
                {
                    Directory.CreateDirectory(FailedDirectoryPath);
                    Logger.Info("Created FailedScans directory at path: {0}", FailedDirectoryPath);
                }
                if(Directory.Exists(SuccessDirectoryPath) == false)
                {
                    Directory.CreateDirectory(SuccessDirectoryPath);
                    Logger.Info("Created SuccessfulScans directory at path: {0}", SuccessDirectoryPath);
                }
                if(Directory.Exists(ReadytoUploadDirectoryPath) == false)
                {
                    Directory.CreateDirectory(ReadytoUploadDirectoryPath);
                    Logger.Info("Created ReadyToUpload directory at path: {0}", ReadytoUploadDirectoryPath);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error preparing scanning directory");
                OnStatusChanged($"Error preparing scanning directory: {ex.Message}");
            }
        }
        private void Prepare_SessionPage()
        {
            try
            {
                int newPageNo = sessionModel.PageCount + 1;
                string newPagePath = System.IO.Path.Combine(activeScanningPath, $"{sessionModel.id}_page{newPageNo}.jpg");
                Prepare_ScanningDirectory();
                PageModel newPage = new PageModel
                {
                    PageNo = newPageNo,
                    Path = newPagePath,
                    StartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    EndTime = string.Empty,
                    FirstPage = newPageNo == 1,
                    LastPage = false,
                    BookViewPage = !(newPageNo == 1),
                };
                activeScannerSettings.BarCodeRecognition = newPage.FirstPage ? 1 : 0; // Enable barcode recognition only for the first page
                activeScannerSettings.ProcessType = newPage.FirstPage ? 2 :4; 
                sessionModel.Pages.Add(newPage);
                sessionModel.PageCount = newPageNo;
                SetScannerProcessType();
                System.IO.File.WriteAllText(System.IO.Path.Combine(activeScanningPath, $"{sessionModel.id}_session.json"), JsonConvert.SerializeObject(sessionModel));
                Logger.Info("Prepared new page for scanning - Page No: {0}, Path: {1}", newPage.PageNo, newPage.Path);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error preparing session page");
                OnStatusChanged($"Error preparing page for scanning: {ex.Message}");
            }

        }

        private void Engine_ImageEvent(object sender, CZUR.CzurEngine.CzurImageEventArgs e)
        {
            ImageEvent?.Invoke(this, e);
            if (e.BarCodeItems != null && e.BarCodeItems.Count > 0)
            {
                foreach (var item in e.BarCodeItems)
                {
                    BarcodeModel barcode = new BarcodeModel
                    {
                        id = Guid.NewGuid().ToString(),
                        Type = item.type.ToString(),
                        Value = item.barCode,
                        BarCodePage = sessionModel.Pages.LastOrDefault()?.PageNo ?? 0
                    };
                    sessionModel.Barcodes.Add(barcode);
                    Logger.Info("Barcode detected - Type: {0}, Value: {1}, Page: {2}", barcode.Type, barcode.Value, barcode.BarCodePage);
                }
            }
            // Play beep sound based on image capture success
            if (e.EventStatus == CZUR.CzurImageEventStatus.Success)
            {
                // Success beep - multiple high frequency beeps for longer sound
                Console.Beep(1000, 400); // 1000 Hz for 400ms
                Logger.Info("Image event successful - Success beep played");
                OnStatusChanged("Scanned page Successfully, Ready to Scan");
            }
            else
            {
                // Failed beep - longer low frequency beeps
                Console.Beep(600, 500);  // 600 Hz for 500ms
                Console.Beep(400, 500);  // 400 Hz for 500ms - descending tone
                Logger.Warn("Image event failed with status: {0} - Failed beep played", e.EventStatus);
                OnStatusChanged("Scanned page Failed, Please check");
            }
            Prepare_SessionPage();
        }


        private void Engine_GrabEvent(object sender, CZUR.CzurEngine.CzurGrabEventArgs e)
        {
            GrabEvent?.Invoke(this, e);
            if(e.Type == 1) // Assuming Type 1 indicates a successful grab
            {
                Logger.Info("Grab event received: {0}", e.Description);

                GrabImage(
                    activeScannerSettings.CameraIndex,
                    sessionModel.Pages.LastOrDefault()?.Path,
                    //"C:\\ScannedImages\\scan.jpg",
                    activeScannerSettings.DPI, activeScannerSettings.ColorMode, activeScannerSettings.Rotation,
                    activeScannerSettings.AutoAdjust,activeScannerSettings.BarCodeRecognition,activeScannerSettings.BlankPageDetection,
                    activeScannerSettings.JpgQuality, activeScannerSettings.Compression);
            }
            else
            {
                Logger.Warn("Grab event with non-success type: {0}, Description: {1}", e.Type, e.Description);
            }
        }
        private void Engine_HttpEvent(object sender, CZUR.CzurEngine.CzurHttpEventArgs e)
        {
            HttpEvent?.Invoke(this, e);
        }


        private void Engine_MultiObjEvent(object sender, CZUR.CzurEngine.CzurMultiObjEventArgs e)
        {
            MultiObjEvent?.Invoke(this, e);
        }

        private void Engine_OcrEvent(object sender, CZUR.CzurEngine.CzurOcrEventArgs e)
        {
            OcrEvent?.Invoke(this, e);
        }

        private void Engine_PdfEvent(object sender, CZUR.CzurEngine.CzurPdfEventArgs e)
        {
            PdfEvent?.Invoke(this, e);
        }

        private void Engine_RecordEvent(object sender, CZUR.CzurEngine.CzurRecordEventArgs e)
        {
            RecordEvent?.Invoke(this, e);
        }

        private void Engine_CfmEvent(object sender, CZUR.CzurEngine.CzurCfmEventArgs e)
        {
            CfmEvent?.Invoke(this, e);
        }

        private void Engine_CvrEvent(object sender, CZUR.CzurEngine.CzurCvrEventArgs e)
        {
            CvrEvent?.Invoke(this, e);
        }

        private void Engine_ThumbnailEvent(object sender, CZUR.CzurEngine.CzurThumbnailEventArgs e)
        {
            ThumbnailEvent?.Invoke(this, e);
        }
        #endregion

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

            status = engine.OpenDevice(activeScannerSettings.CameraIndex, activeScannerSettings.Width,
                                        activeScannerSettings.Height, activeScannerSettings.HP, activeScannerSettings.VP);
            if (!string.IsNullOrEmpty(status))
            {
                OnStatusChanged($"Failed to open device: {status}");
                Logger.Error("Failed to open device: {0}", status);
                return status;
            }
            OnStatusChanged("Device opened. Configuring process type...");
            Logger.Info("Device opened. Configuring process type...");

            status = SetScannerProcessType();
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

        private string SetScannerProcessType()
        {
            return engine.SetProcessType(activeScannerSettings.ProcessType);
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

        /// <summary>
        /// Grab an image from the scanner
        /// </summary>
        /// <param name="deviceIndex">Camera index (0=main, 1=secondary)</param>
        /// <param name="imageFilePath">Output file path for the captured image</param>
        /// <param name="dpi">Resolution in DPI (150, 200, 300, etc.)</param>
        /// <param name="colorMode">Color mode: 0=Auto, 1=B&W, 2=Gray, 3=Color</param>
        /// <param name="rotation">Rotation: 0=None, 90, 180, 270 degrees</param>
        /// <param name="autoAdjust">Enable auto-adjust: 0=Off, 1=On</param>
        /// <param name="barCodeRecognition">Enable barcode recognition: 0=Off, 1=On</param>
        /// <param name="blankPageDetection">Enable blank page detection: 0=Off, 1=On</param>
        /// <param name="jpgQuality">JPG quality level: 1-100</param>
        /// <param name="compression">Image format: 0=JPG, 1=PNG, 2=TIFF</param>
        /// <returns>Error code (0=Success, non-zero=Error)</returns>
        public int GrabImage(int deviceIndex, string imageFilePath, int dpi = 200, int colorMode = 0, 
            int rotation = 0, int autoAdjust = 1, int barCodeRecognition = 1, 
            int blankPageDetection = 1, int jpgQuality = 85, int compression = 0)
        {
            try
            {
                OnStatusChanged("Capturing image from scanner...");
                Logger.Info("GrabImage initiated - File: {0}, DPI: {1}, ColorMode: {2}", imageFilePath, dpi, colorMode);
                
                int errorCode = engine.GrabImage(deviceIndex, imageFilePath, dpi, colorMode, rotation, 
                    autoAdjust, barCodeRecognition, blankPageDetection, jpgQuality, compression);

                if (errorCode == 0)
                {
                    OnStatusChanged("Image captured successfully. Processing...");
                    Logger.Info("Image captured successfully: {0}", imageFilePath);
                    engine.Add_Thumbnail(imageFilePath.Replace("scan.jpg","scan_left.jpg"));
                }
                else
                {
                    OnStatusChanged($"Image capture failed with error code {errorCode}");
                    Logger.Error("GrabImage failed with error code: {0}", errorCode);
                }

                return errorCode;
            }
            catch (Exception ex)
            {
                OnStatusChanged($"Error during image capture: {ex.Message}");
                Logger.Error(ex, "Error during image capture");
                return -1;
            }
        }

        public class StatusChangedEventArgs : EventArgs
        {
            public string Status { get; set; }
            public DateTime Timestamp { get; set; }
        }

        /// <summary>
        /// Creates and initializes a new scanning session
        /// Should be called when user clicks "New Session" button or presses F5
        /// </summary>
        public SessionModel CreateNewSession()
        {
            try
            {
                Reset_SessionModel();
                Logger.Info("New Session Created Successfully");
                return sessionModel;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to create new session");
                OnStatusChanged($"Failed to create new session: {ex.Message}");
                return null;
            }
        }
       

    }
}
