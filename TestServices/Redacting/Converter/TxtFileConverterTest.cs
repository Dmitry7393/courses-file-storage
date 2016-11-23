using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudStorage.Services.Services.ConverterServices;
using System.IO;

namespace TestServices.Redacting.Converters
{
    [TestClass]
    public class TxtFileConverterTest
    {
        TxtFileConverter converter;
        FileInfo tempFile;
        string tempFileData;
        [TestInitialize]
        public void Initialize()
        {
            tempFile = new FileInfo("test.txt");
            //tempFile.Create();
            var stream = tempFile.OpenWrite();
            converter = new TxtFileConverter();
            tempFileData = "TestedString";
            //FileStream fs = new FileStream(tempFile.FullName, FileMode.Create);
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(tempFileData);
                writer.Close();
            }
            //fs.Dispose();
        }
        [TestCleanup]
        public void CleanUp()
        {
            File.Delete(tempFile.FullName);
        }
        [TestMethod]
        [ExpectedException(typeof(System.IO.FileNotFoundException))]
        public void Txt_File_Converter_To_Html_Wrong_Path()
        {
            converter.ToHtml("test");
        }
        [TestMethod]
        public void Txt_File_Converter_To_Html_Correct_Path()
        {

            var result = converter.ToHtml(tempFile.FullName);
            Assert.AreEqual(tempFileData, result);
        }
        [TestMethod]
        public void Txt_File_Converter_From_Html_Correct_Path()
        {
            converter.FromHtml(tempFile.FullName, tempFileData);
        }
    }
}
