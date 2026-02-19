using System;
using System.Windows;
using System.Windows.Controls;
using SmartScanUI.Helpers;
using SmartScanUI.Views;

namespace SmartScanUI.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ScannerConfigViewModel ScannerConfig { get; }
        public AdvancedSettingsViewModel AdvancedSettings { get; }
        public SidebarViewModel Sidebar { get; }

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

            NavigateToHome();
        }

        public void NavigateToHome()
        {
            CurrentWorkspace = CreateViewByName("HomeView");
        }

        public void NavigateToScannerControl()
        {
            CurrentWorkspace = CreateViewByName("ScannerControlView");
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

        private UIElement CreateViewByName(string viewName)
        {
            try
            {
                var viewType = Type.GetType($"SmartScanUI.Views.{viewName}");
                if (viewType != null)
                {
                    return (UIElement)Activator.CreateInstance(viewType);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating view {viewName}: {ex.Message}");
            }
            return new Grid(); // fallback
        }
    }
}
