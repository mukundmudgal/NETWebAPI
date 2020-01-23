using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WorkWave.FormManager.Core.Model;
using WorkWave.FormManager.Domain.PDFix;

namespace WorkWave.FormManager.Domain.BusinessManager
{
    public class FileUploadManager
    {
        public async Task<ResponseObject> TemplateFileUpload(string email, string licenseKey, string jsonPath, string filePath, string uploadingFileName)
        {
            ResponseObject responseObject = new ResponseObject();
            ConvertPDFtoImage convertPDFtoImage = new ConvertPDFtoImage();
            ConvertPDFtoJSON convertPDFtoJSON = new ConvertPDFtoJSON();
            List<string> imageList = new List<string>();
            try
            {
                string[] fileName = uploadingFileName.Split('\\');
                int length = fileName.Length - 1;

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                if (!Directory.Exists(jsonPath))
                {
                    Directory.CreateDirectory(jsonPath);
                }


                filePath = filePath +"\\"+ fileName[length];

                string fileExtension = Path.GetExtension(filePath);


                imageList = await convertPDFtoImage.ExtractImage(email, licenseKey,filePath, jsonPath, 1.0);
                responseObject = await convertPDFtoJSON.ExtractJsonFromPDF(email, licenseKey, filePath, imageList);
            }
            catch (Exception ex)
            {
                responseObject.flag = false;
                responseObject.message = "Document Import Failed";
                responseObject.exceptionMsg = ex.Message;
            }
            return responseObject;
        }
    }
}
