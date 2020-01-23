using PDFixSDK.Pdfix;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkWave.FormManager.Core.Model;

namespace WorkWave.FormManager.Domain.PDFix
{
    public class ConvertPDFtoJSON
    {
        private static int _tabOrder = 0;
        public static string GetFieldType(PdfFormField field)
        {
            PdfFieldType elemType = field.GetType_();
            string valueType = String.Empty;
            switch (elemType.ToString())
            {

                case "kFieldText":
                    valueType = "Text";
                    break;

                case "kFieldRadio":
                    valueType = "Radio";
                    break;

                case "kFieldCheck":
                    valueType = "Checkbox";
                    break;

                case "kFieldCombo":
                    valueType = "Dropdown";
                    break;

                case "kFieldSignature":
                    valueType = "Signature";
                    break;
                case "kFieldButton":
                    valueType = "ImageFile";
                    break;
            }

            return valueType;
        }

        public static string GetBase64String(string imgPath)
        {
            string base64String = string.Empty;
            byte[] imgbytes = File.ReadAllBytes(imgPath + ".jpg");
            return base64String = "data:image/jpeg;base64," + Convert.ToBase64String(imgbytes);
            return base64String;

        }

        public async Task<ResponseObject> ExtractJsonFromPDF(string email, string licenseKey, string filePath, List<string> imageList)
        {
            ResponseObject responseObject = new ResponseObject();
            List<string> errorList = new List<string>();
            try
            {
                Pdfix pdfix = new Pdfix();
                if (pdfix == null)
                    throw new Exception("Pdfix initialization fail");

                if (!pdfix.Authorize(email, licenseKey))
                    throw new Exception(pdfix.GetError());

                PdfDoc doc = pdfix.OpenDoc(filePath, "");
                if (doc == null)
                    throw new Exception();

                int pageCount = doc.GetNumPages();

                List<PDF> pdfList = new List<PDF>();

                for (int i = 0; i < doc.GetNumPages(); i++)
                {
                    List<PDFObject> pdfObjectList = new List<PDFObject>();

                    PdfPage page = doc.AcquirePage(i);
                    PDF pdfObj = new PDF();

                    pdfObj.page = i.ToString();
                    pdfObj.imageUrl = GetBase64String(imageList[i]);
                    _tabOrder = 0;
                    int annots = page.GetNumAnnots();

                    for (int j = 0; j < page.GetNumAnnots(); j++)
                    {
                        PdfAnnot pdfAnnot = page.GetAnnot(j);
                        PdfAnnotSubtype pdfAnnotSubtype = pdfAnnot.GetSubtype();

                        PdfFormField field = null;
                        bool isChecked = false;

                        if (pdfAnnotSubtype == PdfAnnotSubtype.kAnnotLink)
                        {
                            var widget = (PdfLinkAnnot)pdfAnnot;
                            field = doc.GetFormField(j);
                            isChecked = field.GetValue() == field.GetWidgetExportValue(widget);
                        }
                        if (pdfAnnotSubtype == PdfAnnotSubtype.kAnnotWidget)
                        {
                            var widget = (PdfWidgetAnnot)pdfAnnot;
                            field = widget.GetFormField();
                            if (field == null)
                            {
                                field = doc.GetFormField(j);
                            }

                            isChecked = field.GetValue() == field.GetWidgetExportValue(widget);
                        }

                        if (pdfAnnotSubtype == PdfAnnotSubtype.kAnnotHighlight)
                        {
                            var widget = (PdfTextMarkupAnnot)pdfAnnot;
                            field = doc.GetFormField(j);
                            isChecked = field.GetValue() == field.GetWidgetExportValue(widget);
                        }

                        if (field == null)
                        {
                            field = doc.GetFormField(j);
                            string fieldName = field.GetFullName();
                            errorList.Add(fieldName);
                            throw new Exception();
                        }

                        PDFObject pdfObject = new PDFObject();
                        pdfObject.fieldName = field.GetFullName();
                        pdfObject.fieldValue = field.GetValue();
                        pdfObject.maxLength = field.GetMaxLength();
                        pdfObject.tooltip = field.GetTooltip();
                        pdfObject.displayName = field.GetDefaultValue();

                        pdfObject.multiLine = ((field.GetFlags() & Pdfix.kFieldFlagMultiline) != 0) ? true : false;
                        pdfObject.isFormatted = ((field.GetAAction(PdfActionEventType.kActionEventFieldFormat)) != null) ? true : false;
                        pdfObject.required = ((field.GetFlags() & Pdfix.kFieldFlagRequired) != 0) ? true : false;
                        pdfObject.readOnly = ((field.GetFlags() & Pdfix.kFieldFlagReadOnly) != 0) ? true : false;
                        pdfObject.tabOrder = _tabOrder++;
                        pdfObject.isChecked = isChecked;
                        pdfObject.fieldType = GetFieldType(field);

                        List<string> dropdownList = new List<string>();
                        for (int k = 0; k < field.GetOptionCount(); k++)
                        {
                            string optionValue = field.GetOptionValue(k);
                            dropdownList.Add(optionValue);
                        }

                        pdfObject.optionList = dropdownList;

                        PdfRect bbox = pdfAnnot.GetBBox();

                        PdfAnnotAppearance pdfAnnotAppearance = pdfAnnot.GetAppearance();
                        PdfPageView pageView = page.AcquirePageView(1.0, PdfRotate.kRotate0);
                        if (pageView == null)
                            throw new Exception(pdfix.GetError());

                        var devRect = pageView.RectToDevice(bbox);

                        var x = devRect.left;
                        var y = devRect.top;
                        var width = devRect.right - devRect.left;
                        var height = devRect.bottom - devRect.top;

                        var pageWidth = pageView.GetDeviceWidth();
                        var pageHeight = pageView.GetDeviceHeight();

                        var pdfvalue = ((double)x / pageWidth) * 100;
                        var percentage = Convert.ToInt32(Math.Round(pdfvalue, 2));

                        pdfObject.x = ((double)devRect.left / pageView.GetDeviceWidth()) * 100;
                        pdfObject.y = ((double)devRect.top / pageView.GetDeviceHeight()) * 100;
                        pdfObject.width = ((double)(devRect.right - devRect.left) / pageView.GetDeviceWidth()) * 100;
                        pdfObject.height = ((double)(devRect.bottom - devRect.top) / pageView.GetDeviceHeight()) * 100;

                        pageView.Release();

                        pdfObjectList.Add(pdfObject);
                    }
                    pdfObj.pdfObjList = pdfObjectList;
                    pdfObj.width = 927;
                    pdfObj.height = 1200;
                    pdfList.Add(pdfObj);
                }


                responseObject.flag = true;
                responseObject.data = pdfList;
                responseObject.message = "Document Import Successfully";

                doc.Close();
                pdfix.Destroy();
            }
            catch (Exception ex)
            {
                responseObject.errorList = errorList;
                throw ex;
            }

            return responseObject;
        }
    }
}
