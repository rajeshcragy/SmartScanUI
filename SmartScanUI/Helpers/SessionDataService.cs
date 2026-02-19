using System;
using System.IO;
using SmartScanUI.Models;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SmartScanUI.Helpers
{
    public class SessionDataService
    {
        private FileSystemWatcher _fileWatcher;
        private string _jsonFilePath;
        private object _fileLock = new object();
        public event EventHandler<SessionsChangedEventArgs> SessionsChanged;

        public SessionDataService(string jsonFilePath)
        {
            _jsonFilePath = jsonFilePath;
            InitializeFileWatcher();
        }

        private void InitializeFileWatcher()
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(_jsonFilePath);
                string fileName = Path.GetFileName(_jsonFilePath);

                if (!Directory.Exists(directoryPath))
                {
                    System.Diagnostics.Debug.WriteLine("Directory not found: " + directoryPath);
                    return;
                }

                _fileWatcher = new FileSystemWatcher(directoryPath)
                {
                    //Filter = fileName,
                    NotifyFilter = NotifyFilters.LastWrite 
                };

                _fileWatcher.Changed += OnFileChanged;
                _fileWatcher.EnableRaisingEvents = true;

                System.Diagnostics.Debug.WriteLine("FileWatcher initialized for: " + _jsonFilePath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error initializing FileWatcher: " + ex.Message);
            }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("File changed detected: " + e.Name);
            OnSessionsChanged();
        }

        public ObservableCollection<SessionModel> LoadSessions()
        {
            ObservableCollection<SessionModel> sessions = new ObservableCollection<SessionModel>();

            lock (_fileLock)
            {
                try
                {
                    if (!File.Exists(_jsonFilePath))
                    {
                        System.Diagnostics.Debug.WriteLine("File not found: " + _jsonFilePath);
                        return sessions;
                    }

                    // Add delay to ensure file is released
                    System.Threading.Thread.Sleep(100);

                    string jsonContent = File.ReadAllText(_jsonFilePath);
                    
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        DateFormatString = "yyyy-MM-ddTHH:mm:szzz"
                    };

                    var root = JsonConvert.DeserializeObject<SessionDataRoot>(jsonContent, settings);

                    if (root != null && root.Sessions != null)
                    {
                        foreach (var session in root.Sessions)
                        {
                            sessions.Add(session);
                        }
                    }

                    System.Diagnostics.Debug.WriteLine("Loaded " + sessions.Count + " sessions from JSON");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error loading sessions: " + ex.Message);
                }
            }

            return sessions;
        }

        protected virtual void OnSessionsChanged()
        {
            SessionsChanged?.Invoke(this, new SessionsChangedEventArgs { Timestamp = DateTime.Now });
        }

        public void Dispose()
        {
            if (_fileWatcher != null)
            {
                _fileWatcher.Dispose();
            }
        }
    }

    public class SessionsChangedEventArgs : EventArgs
    {
        public DateTime Timestamp { get; set; }
    }
}
