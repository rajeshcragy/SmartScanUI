using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SmartScanUI.ViewModels;
using SmartScanUI.Scanner;

namespace SmartScanUI.Views
{

    public partial class ScannerControlView : UserControl
    {
        private SessionImageListViewModel _viewModel;
        public AxCZUROcxLib.AxCZUROcx axCZUROcx1;
        private ScannerHelper _scannerHelper;

        public ScannerControlView()
        {
            InitializeComponent();

            LoadActiveXControl();

            _viewModel = new SessionImageListViewModel();
            this.DataContext = _viewModel;
            
            // Initialize button states when the view loads
            this.Loaded += ScannerControlView_Loaded;
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
            _scannerHelper = new ScannerHelper(axCZUROcx1);
            _scannerHelper.StatusChanged += ScannerHelper_StatusChanged;
            _scannerHelper.ImageEvent += ScannerHelper_ImageEvent;
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
            // Create a new session
            MessageBox.Show("New Session Created!", "New Session", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // You can add your custom logic here to create a new scanning session
            System.Diagnostics.Debug.WriteLine("New Session Started via Button Click or F5 Key Press");
            
            // Example: Add a new session to the view model
            // _viewModel.AddNewSession();
        }
    }
}
