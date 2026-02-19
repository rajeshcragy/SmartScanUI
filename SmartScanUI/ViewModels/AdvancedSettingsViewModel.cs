using SmartScanUI.Helpers;

namespace SmartScanUI.ViewModels
{
    public class AdvancedSettingsViewModel : BaseViewModel
    {
        public bool AutoFeedDetection { get => Get<bool>(); set => Set(value); }
        public bool BlankPageRemoval { get => Get<bool>(); set => Set(value); }

        public AdvancedSettingsViewModel()
        {
            AutoFeedDetection = true;
            BlankPageRemoval = false;
        }
    }
}
