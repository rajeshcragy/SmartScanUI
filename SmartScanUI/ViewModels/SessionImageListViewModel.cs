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

        public SessionImageListViewModel()
        {
            InitializeSessionDataService();
            LoadSessions();
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
