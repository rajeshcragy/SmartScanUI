using System.Windows;
using SmartScanUI.Helpers;
using SmartScanUI.Views;

namespace SmartScanUI.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ScannerConfigViewModel ScannerConfig { get; }
        public AdvancedSettingsViewModel AdvancedSettings { get; }
        public SidebarViewModel Sidebar { get; }

        private UIElement _currentWorkspace;
        public UIElement CurrentWorkspace
        {
            get => Get<UIElement>();
            set => Set(value);
        }

        public MainViewModel()
        {
            ScannerConfig = new ScannerConfigViewModel();
            AdvancedSettings = new AdvancedSettingsViewModel();
            Sidebar = new SidebarViewModel(this);

            NavigateToScanner();
        }

        public void NavigateToScanner()
        {
            CurrentWorkspace = new ScannerConfigView { DataContext = ScannerConfig };
        }

        public void NavigateToPricing()
        {
            CurrentWorkspace = new PricingView();
        }

        public void NavigateToUserProfile()
        {
            CurrentWorkspace = new UserProfileView();
        }

        public void NavigateToConfigurations()
        {
            CurrentWorkspace = new AdvancedSettingsView { DataContext = AdvancedSettings };
        }

        public void NavigateToAdmin()
        {
            CurrentWorkspace = new AdminView();
        }
    }
}
