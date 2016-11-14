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
    public class TxtFileConverter : IFileConverter
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
                string documentText = GetEncodedText(pathToFile);
                //documentText = HttpUtility.HtmlEncode(documentText);
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
                //documentText = HttpUtility.HtmlDecode(documentText);
                htmlData = htmlData.Replace("\r\n", "\r");
                htmlData = htmlData.Replace("\n", "\r");
                htmlData = htmlData.Replace("<br>\r\n", "\r");
                htmlData = htmlData.Replace("&nbsp;", " ");
                string clearHtmlTags = @"(<[^>]+>)|\&\w+;";
                Regex regular = new Regex(clearHtmlTags);
                htmlData = regular.Replace(htmlData, "");
                using (StreamWriter writer = new StreamWriter(pathToFile))
                {
                    writer.Write(htmlData);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        // <summary>
        // Defines a method for reading text from file with correct Encoding.
        //
        // Parameters:
        //   pathToFile:
        // Path on server to editing file
        // Return:
        //  Returns string represent of file text.
        private string GetEncodedText(string pathToFile)
        {
            try
            {
                BinaryReader instr = new BinaryReader(File.OpenRead(pathToFile));
                byte[] data = instr.ReadBytes((int)instr.BaseStream.Length);
                if (!IsTextFormat(data))
                {
                    throw new FormatException();
                }
                instr.Close();

                // определяем BOM (EF BB BF)
                if (data.Length > 2 && data[0] == 0xef && data[1] == 0xbb && data[2] == 0xbf)
                {
                    if (data.Length != 3) return Encoding.UTF8.GetString(data, 3, data.Length - 3);
                    else return "";
                }

                int i = 0;
                while (i < data.Length - 1)
                {
                    if (data[i] > 0x7f)
                    { // не ANSI-символ
                        if ((data[i] >> 5) == 6)
                        {
                            if ((i > data.Length - 2) || ((data[i + 1] >> 6) != 2))
                                return Encoding.GetEncoding(1251).GetString(data);
                            i++;
                        }
                        else if ((data[i] >> 4) == 14)
                        {
                            if ((i > data.Length - 3) || ((data[i + 1] >> 6) != 2) || ((data[i + 2] >> 6) != 2))
                                return Encoding.GetEncoding(1251).GetString(data);
                            i += 2;
                        }
                        else if ((data[i] >> 3) == 30)
                        {
                            if ((i > data.Length - 4) || ((data[i + 1] >> 6) != 2) || ((data[i + 2] >> 6) != 2) || ((data[i + 3] >> 6) != 2))
                                return Encoding.GetEncoding(1251).GetString(data);
                            i += 3;
                        }
                        else
                        {
                            return Encoding.GetEncoding(1251).GetString(data);
                        }
                    }
                    i++;
                }
                return Encoding.UTF8.GetString(data);

            }
            catch (Exception)
            {
                throw;
            }
        }


        // <summary>
        // Defines a method for check is file text format or binary
        //
        // Parameters:
        //   file:
        // All file content in byte array.
        // Return:
        //  Returns true if file isn't binary and false if it is.
        private bool IsTextFormat(byte[] file)
        {
            foreach (byte item in file)
            {
                if (item == 0x00)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
