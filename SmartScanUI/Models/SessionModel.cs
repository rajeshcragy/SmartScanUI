using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SmartScanUI.Models
{
    public class SessionModel
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("StartTime")]
        public string StartTime { get; set; }

        [JsonProperty("EndTime")]
        public string EndTime { get; set; }

        [JsonProperty("PageCount")]
        public int PageCount { get; set; }

        [JsonProperty("Pages")]
        public List<PageModel> Pages { get; set; }

        [JsonProperty("Barcodes")]
        public List<BarcodeModel> Barcodes { get; set; }

        public SessionModel()
        {
            Pages = new List<PageModel>();
            Barcodes = new List<BarcodeModel>();
        }

        // Properties for easy binding
        public string Id => id;
        public DateTime StartTimeValue => ParseDateTime(StartTime);
        public DateTime EndTimeValue => ParseDateTime(EndTime);
        public int PageCountValue => PageCount;

        private DateTime ParseDateTime(string dateTimeStr)
        {
            if (DateTime.TryParse(dateTimeStr, out var result))
            {
                return result;
            }
            return DateTime.Now;
        }
    }

    public class PageModel
    {
        [JsonProperty("PageNo")]
        public int PageNo { get; set; }

        [JsonProperty("Path")]
        public string Path { get; set; }

        [JsonProperty("StartTime")]
        public string StartTime { get; set; }

        [JsonProperty("EndTime")]
        public string EndTime { get; set; }

        [JsonProperty("FirstPage")]
        public bool FirstPage { get; set; }

        [JsonProperty("LastPage")]
        public bool LastPage { get; set; }

        [JsonProperty("BookViewPage")]
        public bool BookViewPage { get; set; }
    }

    public class BarcodeModel
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("Value")]
        public string Value { get; set; }

        [JsonProperty("BarCodePage")]
        public int BarCodePage { get; set; }

        public string Id => id;
    }

    public class SessionDataRoot
    {
        [JsonProperty("Sessions")]
        public List<SessionModel> Sessions { get; set; }

        public SessionDataRoot()
        {
            Sessions = new List<SessionModel>();
        }
    }
}
