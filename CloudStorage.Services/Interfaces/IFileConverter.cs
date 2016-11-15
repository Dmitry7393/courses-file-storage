using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudStorage.Services.Interfaces
{
    // <summary>
    // Defines a contract for classes that enabled editing of file.
    public interface IFileConverter
    {
        // <summary>
        // Defines a method for converting file to HTML format.
        //
        // Parameters:
        //   pathToFile:
        // Path on server to editing file

        string ToHtml(string pathToFile);

        // <summary>
        // Defines a method for converting file to HTML format.
        //
        // Parameters:
        //   pathToFile:
        // Path on server to editing file
        //   htmlData:
        // Changed text of file in HTML format 
        void FromHtml(string pathToFile, string htmlData);
    }

}
