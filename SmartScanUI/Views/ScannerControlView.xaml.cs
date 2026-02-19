using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
