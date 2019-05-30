using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignSkrip.UploadRepo
{
    public class FileUploadDetails
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public long FileLength { get; set; }
        public string FileCreatedTime { get; set; }
        public int IdSign { get; set; }
    }
}