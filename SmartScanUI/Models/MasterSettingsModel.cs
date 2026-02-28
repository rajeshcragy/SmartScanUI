using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SmartScanUI.Models
{
    /// <summary>
    /// Master settings model representing the structure of Master.json configuration file
    /// </summary>
    public class MasterSettingsModel
    {
        [JsonProperty("Collage")]
        public List<CollageModel> Collage { get; set; }

        [JsonProperty("Scanner")]
        public List<ScannerSettingsModel> Scanner { get; set; }

        [JsonProperty("Routes")]
        public List<RouteModel> Routes { get; set; }

        [JsonProperty("UserDetailsPath")]
        public string UserDetailsPath { get; set; }

        [JsonProperty("ScansPath")]
        public string ScansPath { get; set; }

        public MasterSettingsModel()
        {
            Collage = new List<CollageModel>();
            Scanner = new List<ScannerSettingsModel>();
            Routes = new List<RouteModel>();
        }
    }

    /// <summary>
    /// Collage/Organization details model
    /// </summary>
    public class CollageModel
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Address1")]
        public string Address1 { get; set; }

        [JsonProperty("Address2")]
        public string Address2 { get; set; }

        [JsonProperty("City")]
        public string City { get; set; }

        [JsonProperty("State")]
        public string State { get; set; }

        [JsonProperty("Pincode")]
        public string Pincode { get; set; }

        [JsonProperty("Country")]
        public string Country { get; set; }

        [JsonProperty("Code")]
        public string Code { get; set; }

        [JsonProperty("IsActive")]
        public int IsActive { get; set; }

        /// <summary>
        /// Gets the full address combining all address parts
        /// </summary>
        public string GetFullAddress()
        {
            return $"{Address1}, {Address2}, {City}, {State} {Pincode}, {Country}";
        }

        /// <summary>
        /// Gets whether this collage is active
        /// </summary>
        public bool IsActiveValue => IsActive == 1;
    }

    /// <summary>
    /// Scanner configuration settings model
    /// </summary>
    public class ScannerSettingsModel
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Model")]
        public string Model { get; set; }

        [JsonProperty("DriverVersion")]
        public string DriverVersion { get; set; }

        [JsonProperty("CameraIndex")]
        public int CameraIndex { get; set; }

        [JsonProperty("IsActive")]
        public int IsActive { get; set; }

        [JsonProperty("Width")]
        public int Width { get; set; }

        [JsonProperty("Height")]
        public int Height { get; set; }

        [JsonProperty("HP")]
        public int HP { get; set; }

        [JsonProperty("VP")]
        public int VP { get; set; }

        [JsonProperty("ColorMode")]
        public int ColorMode { get; set; }

        [JsonProperty("DPI")]
        public int DPI { get; set; }

        [JsonProperty("Rotation")]
        public int Rotation { get; set; }

        [JsonProperty("AutoAdjust")]
        public int AutoAdjust { get; set; }

        [JsonProperty("BarCodeRecognition")]
        public int BarCodeRecognition { get; set; }

        [JsonProperty("BlankPageDetection")]
        public int BlankPageDetection { get; set; }

        [JsonProperty("JpgQuality")]
        public int JpgQuality { get; set; }

        [JsonProperty("Compression")]
        public int Compression { get; set; }

        [JsonProperty("ProcessType")]
        public int ProcessType { get; set; }

        /// <summary>
        /// Gets whether this scanner is active
        /// </summary>
        public bool IsActiveValue => IsActive == 1;

        /// <summary>
        /// Gets a friendly name for the color mode
        /// </summary>
        public string ColorModeDescription
        {
            get
            {
                switch (ColorMode)
                {
                    case 0:
                        return "Auto";
                    case 1:
                        return "Black & White";
                    case 2:
                        return "Grayscale";
                    case 3:
                        return "Color";
                    default:
                        return "Unknown";
                }
            }
        }

        /// <summary>
        /// Gets a friendly name for the compression type
        /// </summary>
        public string CompressionDescription
        {
            get
            {
                switch (Compression)
                {
                    case 0:
                        return "JPG";
                    case 1:
                        return "PNG";
                    case 2:
                        return "TIFF";
                    default:
                        return "Unknown";
                }
            }
        }

        /// <summary>
        /// Gets the scanner specifications as a formatted string
        /// </summary>
        public string GetSpecifications()
        {
            return $"{Name} - {Model}\n" +
                   $"Resolution: {Width}x{Height}\n" +
                   $"DPI: {DPI}, Color Mode: {ColorModeDescription}\n" +
                   $"Format: {CompressionDescription}, Quality: {JpgQuality}";
        }
    }

    /// <summary>
    /// API Route/Endpoint configuration model
    /// </summary>
    public class RouteModel
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("TokenURI")]
        public string TokenURI { get; set; }

        [JsonProperty("Creditntials")]
        public string Credentials { get; set; }

        [JsonProperty("Method")]
        public string Method { get; set; }

        [JsonProperty("Headers")]
        public string Headers { get; set; }

        [JsonProperty("URI")]
        public string URI { get; set; }

        /// <summary>
        /// Gets whether this is a GET request
        /// </summary>
        public bool IsGetRequest => Method?.ToUpper() == "GET";

        /// <summary>
        /// Gets whether this is a POST request
        /// </summary>
        public bool IsPostRequest => Method?.ToUpper() == "POST";

        /// <summary>
        /// Gets whether this route requires authentication token
        /// </summary>
        public bool RequiresAuthentication => !string.IsNullOrEmpty(TokenURI);
    }
}
