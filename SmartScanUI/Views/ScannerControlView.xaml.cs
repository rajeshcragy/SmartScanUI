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
        }
    }
}
