using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudStorage.UI.Models
{
    public class AboutFileInfoModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Folder { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastTimeChanged { get; set; }
        public string Size { get; set; }
        public string ShareLink { get; set; }
        public AboutFileInfoModel()
        {
            ShareLink = string.Empty;
            Name = string.Empty;
            Type = string.Empty;
            Folder = "My Cloud";
            Size = string.Empty;
            CreateDate = new DateTime();
            LastTimeChanged = new DateTime();

        }
    }
}