using CloudStorage.Services.Interfaces;
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
    public class DocFileConverter : IFileConverter
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
                /// <summary>
                // Create an instance of editing document and getting text from it            
                Spire.Doc.Document doc = new Spire.Doc.Document();
                doc.LoadFromFile(pathToFile, Spire.Doc.FileFormat.Doc);
                string documentText = doc.GetText();

                // <summary>
                // Replace control symbols to HTML symbols            
                documentText = documentText.Replace("\r\n", "\r");
                documentText = documentText.Replace("\n", "\r");
                documentText = documentText.Replace("\r", "<br>\r\n");
                documentText = documentText.Replace(" ", " &nbsp;");

                // <summary>
                // Return html view of Docx file            
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
                // Replace  HTML symbols to control symbols of Docx file

                htmlData = htmlData.Replace("\r\n", "\r");
                htmlData = htmlData.Replace("\n", "\r");
                htmlData = htmlData.Replace("<br>\r\n", "\r");
                htmlData = htmlData.Replace("<br />", "\n");
                htmlData = htmlData.Replace("&nbsp;", " ");

                // <summary>
                // Create string patter for clearing text from HTML tags

                string clearHtmlTags = @"(<[^>]+>)|\&\w{3,5};";
                Regex regular = new Regex(clearHtmlTags);
                htmlData = regular.Replace(htmlData, "");
                Spire.Doc.Document doc = new Spire.Doc.Document();
                byte[] array = Encoding.UTF8.GetBytes(htmlData);

                // <summary>
                // Save new text data to Docx file

                using (MemoryStream mr = new MemoryStream(array, 0, array.Length))
                {
                    doc.LoadText(mr, Encoding.UTF8);
                    doc.SaveToFile(pathToFile, Spire.Doc.FileFormat.Docx2010);
                }
            }
            catch (Exception)
            {
                
                throw;
            }
          

           

        }
    }
}
