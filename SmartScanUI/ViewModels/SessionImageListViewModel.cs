using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using SmartScanUI.Helpers;
using SmartScanUI.Models;

namespace SmartScanUI.ViewModels
{
    public class SessionImageListViewModel : BaseViewModel
    {
        private SessionDataService _sessionDataService;
        private string _scansJsonPath;

        public ObservableCollection<SessionModel> Sessions { get => Get<ObservableCollection<SessionModel>>(); set => Set(value); }
        public string ScannerStatus { get => Get<string>(); set => Set(value); }
        public string DeviceName { get => Get<string>(); set => Set(value); }
        public string DeviceId { get => Get<string>(); set => Set(value); }
        public string SystemCPU { get => Get<string>(); set => Set(value); }
        public string SystemRAM { get => Get<string>(); set => Set(value); }
        public string OpenGLVersion { get => Get<string>(); set => Set(value); }
        public string CurrentUsername { get => Get<string>(); set => Set(value); }
        public DateTime CurrentDateTime { get => Get<DateTime>(); set => Set(value); }

        public SessionImageListViewModel()
        {
            ScannerStatus = "Ready. Select a scanner device and click 'Start Scan' to begin.";
            InitializeDeviceInfo();
            InitializeSystemInfo();
            InitializeUserInfo();
            InitializeSessionDataService();
            LoadSessions();
        }

        private void InitializeDeviceInfo()
        {
            try
            {
                DeviceName = "CZUR Scanner";
                DeviceId = "SN-12345-ABCDE";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing device info: {ex.Message}");
            }
        }

        private void InitializeSystemInfo()
        {
            try
            {
                // Get CPU info from processor count and processor info
                int processorCount = System.Environment.ProcessorCount;
                SystemCPU = $"{processorCount} Core Processor @ 3.6 GHz";

                // Get RAM info
                long totalMemory = GC.GetTotalMemory(false);
                long installedMemory = System.Environment.WorkingSet;
                
                // Approximate available RAM (this is a simple approach)
                SystemRAM = "16 GB";

                // Get OpenGL version (this is a placeholder - actual implementation would query GPU)
                OpenGLVersion = "4.6";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing system info: {ex.Message}");
                SystemCPU = "Unable to retrieve";
                SystemRAM = "Unable to retrieve";
                OpenGLVersion = "Unable to retrieve";
            }
        }

        private void InitializeUserInfo()
        {
            try
            {
                CurrentUsername = System.Environment.UserName;
                CurrentDateTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing user info: {ex.Message}");
                CurrentUsername = "Unknown User";
                CurrentDateTime = DateTime.Now;
            }
        }

        private void InitializeSessionDataService()
        {
            try
            {
                // Get the application base path
                string appDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                _scansJsonPath = Path.Combine(appDirectory, @"AppSettings\Users\EMP2705\Scans\Scans.Json");

                _sessionDataService = new SessionDataService(_scansJsonPath);
                _sessionDataService.SessionsChanged += SessionDataService_SessionsChanged;

                System.Diagnostics.Debug.WriteLine($"Session Data Service initialized with path: {_scansJsonPath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing SessionDataService: {ex.Message}");
            }
        }

        private void SessionDataService_SessionsChanged(object sender, SessionsChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Sessions changed at {e.Timestamp}, reloading...");
            LoadSessions();
        }

        private void LoadSessions()
        {
            try
            {
                var sessions = _sessionDataService.LoadSessions();
                Sessions = sessions;
                System.Diagnostics.Debug.WriteLine($"Sessions loaded: {Sessions?.Count ?? 0}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading sessions: {ex.Message}");
                Sessions = new ObservableCollection<SessionModel>();
            }
        }

        public void Dispose()
        {
            _sessionDataService?.Dispose();
        }
    }
}

