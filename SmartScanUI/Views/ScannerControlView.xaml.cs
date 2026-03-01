using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SmartScanUI.ViewModels;
using SmartScanUI.Scanner;
using SmartScanUI.Models;
using Newtonsoft.Json;
using NLog;

namespace SmartScanUI.Views
{

    public partial class ScannerControlView : UserControl
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        
        private SessionImageListViewModel _viewModel;
        public AxCZUROcxLib.AxCZUROcx axCZUROcx1;
        private ScannerHelper _scannerHelper;

        private MasterSettingsModel MasterSettingsModel;
        
        public ScannerControlView()
        {
            InitializeComponent();

            LoadActiveXControl();

            _viewModel = new SessionImageListViewModel();
            this.DataContext = _viewModel;
            
            // Load Master Settings from JSON file
            LoadMasterSettings();
            
            // Initialize button states when the view loads
            this.Loaded += ScannerControlView_Loaded;
        }

        /// <summary>
        /// Loads the Master Settings from the JSON configuration file
        /// </summary>
        private void LoadMasterSettings()
        {
            try
            {
                // Get the application startup path
                string appStartupPath = AppDomain.CurrentDomain.BaseDirectory;
                
                // Build the full path to Master.json
                string masterSettingsPath = System.IO.Path.Combine(appStartupPath, @"AppSettings\Users\Master.json");
                
                // Check if file exists
                if (!System.IO.File.Exists(masterSettingsPath))
                {
                    Logger.Error("Master.json file not found at path: {0}", masterSettingsPath);
                    MessageBox.Show($"Configuration file not found at:\n{masterSettingsPath}", 
                        "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                // Read and deserialize the JSON file
                string jsonContent = System.IO.File.ReadAllText(masterSettingsPath);
                this.MasterSettingsModel = JsonConvert.DeserializeObject<MasterSettingsModel>(jsonContent);
                
                if (this.MasterSettingsModel != null)
                {
                    Logger.Info("Master Settings loaded successfully from: {0}", masterSettingsPath);
                    
                    // Log the loaded settings
                    if (this.MasterSettingsModel.Collage != null && this.MasterSettingsModel.Collage.Count > 0)
                    {
                        Logger.Info("Loaded {0} collage settings", this.MasterSettingsModel.Collage.Count);
                    }
                    
                    if (this.MasterSettingsModel.Scanner != null && this.MasterSettingsModel.Scanner.Count > 0)
                    {
                        Logger.Info("Loaded {0} scanner settings", this.MasterSettingsModel.Scanner.Count);
                    }
                }
                else
                {
                    Logger.Warn("Master Settings deserialized but object is null");
                    MessageBox.Show("Failed to parse configuration file. Object is null.", 
                        "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Logger.Error(ex, "Master.json file not found");
                MessageBox.Show($"Configuration file not found:\n{ex.Message}", 
                    "File Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (JsonException ex)
            {
                Logger.Error(ex, "Error parsing Master.json file");
                MessageBox.Show($"Error parsing configuration file:\n{ex.Message}", 
                    "JSON Parse Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unexpected error loading Master Settings");
                MessageBox.Show($"Unexpected error loading configuration:\n{ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ScannerControlView_Loaded(object sender, RoutedEventArgs e)
        {
            // Set initial button states: Start Scan enabled, Reset Scan disabled
            StartScanButton.IsEnabled = true;
            ResetScanButton.IsEnabled = false;
        }

        private void LoadActiveXControl()
        {
            System.Windows.Forms.Integration.WindowsFormsHost host =
                    new System.Windows.Forms.Integration.WindowsFormsHost();

             axCZUROcx1 = new AxCZUROcxLib.AxCZUROcx();

            axCZUROcx1.Height = (int)ActiveXScanner.Height;
            axCZUROcx1.Width = (int)ActiveXScanner.Width;
            host.Child = axCZUROcx1;

            // Add the interop host control to the Grid
            // control's collection of child controls.
            this.ActiveXScanner.Children.Add(host);
        }

        private void StartScanButton_Click(object sender, RoutedEventArgs e)
        {
            if (_scannerHelper == null)
            {
                _scannerHelper = new ScannerHelper(axCZUROcx1, MasterSettingsModel);
                _scannerHelper.StatusChanged += ScannerHelper_StatusChanged;
                _scannerHelper.ImageEvent += ScannerHelper_ImageEvent;
                CreateNewSession();
            }
            _scannerHelper.Initialize();
            
            // Update button states: disable Start Scan, enable Reset Scan
            StartScanButton.IsEnabled = false;
            ResetScanButton.IsEnabled = true;
        }

        private void ScannerHelper_StatusChanged(object sender, SmartScanUI.Scanner.ScannerHelper.StatusChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                _viewModel.ScannerStatus = e.Status;
                System.Diagnostics.Debug.WriteLine($"[{e.Timestamp:HH:mm:ss.fff}] Scanner Status: {e.Status}");
            });
        }

        private void ScannerHelper_ImageEvent(object sender, SmartScanUI.Scanner.CZUR.CzurEngine.CzurImageEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                System.Diagnostics.Debug.WriteLine($"[Image Event] Status: {e.EventStatus}");
                
                if (!string.IsNullOrEmpty(e.ErrorMessage))
                {
                    System.Diagnostics.Debug.WriteLine($"[Image Event] Error: {e.ErrorMessage}");
                }

                if (e.BarCodeItems != null && e.BarCodeItems.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"[Image Event] Bar codes detected: {e.BarCodeItems.Count}");
                    foreach (var barCode in e.BarCodeItems)
                    {
                        System.Diagnostics.Debug.WriteLine($"  - BarCode: {barCode.barCode}, Type: {barCode.type}");
                    }
                }
            });
        }

        private void ResetScanButton_Click(object sender, RoutedEventArgs e)
        {
            if (_scannerHelper != null)
            {
                _scannerHelper.UnInitialize();
            }
            else
            {
                MessageBox.Show("Scanner is not initialized.", "Reset Scan", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
            // Update button states: disable Reset Scan, enable Start Scan
            ResetScanButton.IsEnabled = false;
            StartScanButton.IsEnabled = true;
        }

        private void AddNewSessionButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a new session
            CreateNewSession();
        }

        private void UserControl_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Check if F5 key is pressed
            if (e.Key == System.Windows.Input.Key.F5)
            {
                CreateNewSession();
                e.Handled = true; // Mark the event as handled to prevent default F5 behavior
            }
        }

        private void CreateNewSession()
        {
            try
            {
                // Check if scanner helper is initialized
                if (_scannerHelper != null)
                {
                    // Create a new session through scanner helper
                    var newSession = _scannerHelper.CreateNewSession();
                    
                    if (newSession != null)
                    {
                        Logger.Info("New Session Created - ID: {0}", newSession.id);
                    }
                    else
                    {
                        Logger.Warn("Failed to create new session");
                    }
                }
                else
                {
                    Logger.Warn("Scanner is not initialized. Initialize scanner first.");
                    MessageBox.Show("Please start the scanner before creating a new session", 
                        "Scanner Not Initialized", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error creating new session");
                MessageBox.Show($"Error creating new session: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
