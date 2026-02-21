using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SmartScanUI.ViewModels;

namespace SmartScanUI.Views
{

    public partial class ScannerControlView : UserControl
    {
        private SessionImageListViewModel _viewModel;
        public AxCZUROcxLib.AxCZUROcx axCZUROcx1;
        public ScannerControlView()
        {
            InitializeComponent();

            LoadActiveXControl();

            _viewModel = new SessionImageListViewModel();
            this.DataContext = _viewModel;

        }

        private void LoadActiveXControl()
        {
            

                // Create the interop host control.
            System.Windows.Forms.Integration.WindowsFormsHost host =
                    new System.Windows.Forms.Integration.WindowsFormsHost();

            // Create the ActiveX control.
             axCZUROcx1 = new AxCZUROcxLib.AxCZUROcx();

            axCZUROcx1.Height = 100;
            axCZUROcx1.Width = 100;
            // Assign the ActiveX control as the host control's child.
            host.Child = axCZUROcx1;

            // Add the interop host control to the Grid
            // control's collection of child controls.
            this.ActiveXScanner.Children.Add(host);
        }

        private void StartScanButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Start Scan button clicked!", "Start Scan", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void StopScanButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Stop Scan button clicked!", "Stop Scan", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
