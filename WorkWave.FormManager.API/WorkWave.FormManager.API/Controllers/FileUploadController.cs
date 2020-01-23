using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Http;
using WebApiFileuploadDemo.UploadFile;
using WorkWave.FormManager.Core.Model;
using WorkWave.FormManager.Domain.BusinessManager;

namespace WorkWave.FormManager.API.Controllers
{
    public class FileUploadController : ApiController
    {
        [HttpPost]
        public async Task<ResponseObject> Post()
        {
            FileUploadManager fileUploadManager = new FileUploadManager();
            ResponseObject responseObject = new ResponseObject();
            try
            {

                string email = WebConfigurationManager.AppSettings["pdfixEmail"];
                string password = WebConfigurationManager.AppSettings["pdfixPassword"];
                string fileSize = WebConfigurationManager.AppSettings["fileSize"];
                string fileExtension = WebConfigurationManager.AppSettings["fileExtension"];

                var resourcesDir = HttpContext.Current.Server.MapPath("~/UploadedFiles/");
                var outputDir = HttpContext.Current.Server.MapPath("~/JSON/");

                if (!Directory.Exists(resourcesDir))
                {
                    Directory.CreateDirectory(resourcesDir);
                }

                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }


                string fileName = string.Empty;

                System.Web.HttpFileCollection httpFileCollection = System.Web.HttpContext.Current.Request.Files;

                for (int count = 0; count <= httpFileCollection.Count - 1; count++)
                {
                    System.Web.HttpPostedFile postedFile = httpFileCollection[count];
                    bool result = Convert.ToBoolean(fileSize.CompareTo(Convert.ToString(postedFile.ContentLength)));

                    if (postedFile.ContentLength > 0)
                    {
                        if (!result)
                        {
                            responseObject.flag = false;
                            responseObject.message = "Document size cannot be more than 8MB";
                        }
                        if (postedFile.ContentType != fileExtension)
                        {
                            responseObject.flag = false;
                            responseObject.message = "File type is not supported";
                            return responseObject;
                        }

                        fileName = postedFile.FileName;
                        var filePath = HttpContext.Current.Server.MapPath("~/UploadedFiles/" + postedFile.FileName);
                        postedFile.SaveAs(filePath);

                    }
                    else
                    {
                        responseObject.flag = false;
                        responseObject.message = "Please upload file";
                        return responseObject;
                    }
                }


                responseObject = await fileUploadManager.TemplateFileUpload(email, password, outputDir, resourcesDir, fileName);

            }
            catch (Exception ex)
            {
                responseObject.flag = false;
                responseObject.message = "Document import failed";
                responseObject.exceptionMsg = ex.Message;
            }

            return responseObject;
        }
    }
}
