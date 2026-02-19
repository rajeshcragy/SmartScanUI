using System.Windows;
using SmartScanUI.Helpers;

namespace SmartScanUI.ViewModels
{
    public class SidebarViewModel : BaseViewModel
    {
        public RelayCommand HomeCommand { get; }
        public RelayCommand ScannerCommand { get; }
        public RelayCommand PricingCommand { get; }
        public RelayCommand UserProfileCommand { get; }
        public RelayCommand ConfigurationsCommand { get; }
        public RelayCommand AdminCommand { get; }
        public RelayCommand LogoutCommand { get; }

        public SidebarViewModel(MainViewModel parentViewModel)
        {
            HomeCommand = new RelayCommand(_ => parentViewModel.NavigateToHome());
            ScannerCommand = new RelayCommand(_ => parentViewModel.NavigateToScannerControl());
            PricingCommand = new RelayCommand(_ => parentViewModel.NavigateToPricing());
            UserProfileCommand = new RelayCommand(_ => parentViewModel.NavigateToUserProfile());
            ConfigurationsCommand = new RelayCommand(_ => parentViewModel.NavigateToConfigurations());
            AdminCommand = new RelayCommand(_ => parentViewModel.NavigateToAdmin());
            LogoutCommand = new RelayCommand(_ => Application.Current.Shutdown());
        }
    }
}
