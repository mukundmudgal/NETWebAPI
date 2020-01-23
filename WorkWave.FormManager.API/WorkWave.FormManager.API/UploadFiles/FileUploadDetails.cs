using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiFileuploadDemo.UploadFile
{
    public class FileUploadDetails
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public long FileLength { get; set; }
        public string FileCreatedTime { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }

    public class Status
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}