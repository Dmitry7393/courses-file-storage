using CloudStorage.Services.Interfaces;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CloudStorage.Services.Services.ConverterServices
{
    // <summary>
    // Defines an implementation of IFileConverter contract.
    public class PdfFileConverter : IFileConverter
    {
        // <summary>
        // Defines a method for converting file to HTML format.
        //
        // Parameters:
        //   pathToFile:
        // Path on server to editing file
        public string ToHtml(string pathToFile)
        {
            try
            {
                string documentText = string.Empty;
                //<summary>
                //Create object of PdfReader to read from Pdf file
                PdfReader reader = new PdfReader(pathToFile);
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    documentText += iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, page);
                }
                reader.Close();
                //documentText = HttpUtility.HtmlEncode(documentText);

                // <summary>
                // Replace control symbols to HTML symbols    
                documentText = documentText.Replace("\r\n", "\r");
                documentText = documentText.Replace("\n", "\r");
                documentText = documentText.Replace("\r", "<br>\r\n");
                documentText = documentText.Replace("  ", " &nbsp;");
                return documentText;
            }
            catch (Exception)
            {
                
                throw;
            }
          
        }

        // <summary>
        // Defines a method for converting file to HTML format.
        //
        // Parameters:
        //   pathToFile:
        // Path on server to editing file
        //   htmlData:
        // Changed text of file in HTML format 
        public void FromHtml(string pathToFile, string htmlData)
        {
            try
            {
                // <summary>
                // Replace  HTML symbols to control symbols of Txt file
                htmlData = htmlData.Replace("\r\n", "\r");
                htmlData = htmlData.Replace("\n", "\r");
                htmlData = htmlData.Replace("<br>\r\n", "\r");
                htmlData = htmlData.Replace("<br />", "\n");
                htmlData = htmlData.Replace("&nbsp;", " ");
                // <summary>
                // Create string patter for clearing text from HTML tags
                string clearHtmlTags = @"(<[^>]+>)|\&\w+;";
                Regex regular = new Regex(clearHtmlTags);
                htmlData = regular.Replace(htmlData, "");
                Spire.Doc.Document doc = new Spire.Doc.Document();
                byte[] textArray = Encoding.UTF8.GetBytes(htmlData);
                // <summary>
                // Save new text data to Pdf file from memory stream
                using (MemoryStream mr = new MemoryStream(textArray, 0, textArray.Length))
                {
                    doc.LoadText(mr, Encoding.UTF8);
                    doc.SaveToFile(pathToFile, Spire.Doc.FileFormat.PDF);
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            

        }
    }
}
