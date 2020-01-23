using System;
using System.Collections.Generic;
using System.Text;

namespace WorkWave.FormManager.Core.Model
{
    public class RequestObject
    {
        public string templateName { get; set; }
        public int categoryId { get; set; }
        public int appId { get; set; }
        public int templatestatus { get; set; }
        public List<PDF> data { get; set; }
    }

    public class PDF
    {
        public string page { get; set; }
        public List<PDFObject> pdfObjList { get; set; }
        public string imageUrl { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class PDFObject
    {
        public string fieldName { get; set; }
        public string fieldValue { get; set; }
        public string optionName { get; set; }
        public int tabOrder { get; set; }
        public int maxLength { get; set; }
        public bool multiLine { get; set; }
        public bool isFormatted { get; set; }
        public bool isChecked { get; set; }
        public bool required { get; set; }
        public bool readOnly { get; set; }
        public bool isCalculation { get; set; }
        public string fieldType { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public double width { get; set; }
        public double height { get; set; }
        public string tooltip { get; set; }
        public string displayName { get; set; }
        public string calculations { get; set; }
        public List<string> optionList { get; set; }
        public MappedData mappedData { get; set; }
        public List<Permission> permission { get; set; }
    }

    public class MappedData
    {
        public string datasource { get; set; }
        public string readOnly { get; set; }
        public string required { get; set; }
    }

    public class Permission
    {
        public string userId { get; set; }
        public string userName { get; set; }
        public string userType { get; set; }
    }

    public class ResponseObject
    {
        public List<PDF> data { get; set; }
        public bool flag { get; set; }
        public string message { get; set; }
        public string exceptionMsg { get; set; }
        public List<string> errorList { get; set; }
    }
}
