using CloudStorage.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudStorage.Services.Services.ConverterServices.Factory
{
    public class FactoryConverter
    {
        // Summary:
        //     Get the new instance of IFileConverter intarface that depend on file extension
        //
        // Parameters:
        //   file:
        //     Extension represents the extension of redact file
        public static IFileConverter CreateConveterInstace(string extension)
        {
            const string pdf = ".pdf";
            const string txt = ".txt";
            const string docx = ".docx";
            const string doc = ".doc";

            try
            {
                switch (extension)
                {
                    case pdf:
                        return new PdfFileConverter();
                    case doc:
                        return new DocFileConverter();
                    case docx:                   
                        return new DocxFileConverter();

                    case txt:
                        return new TxtFileConverter();

                    default:
                        throw new ArgumentException();
                }
            }
            catch (Exception)
            {
                throw;
            }


        }
    }
}
