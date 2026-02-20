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

        public ScannerControlView()
        {
            InitializeComponent();
            _viewModel = new SessionImageListViewModel();
            this.DataContext = _viewModel;
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
