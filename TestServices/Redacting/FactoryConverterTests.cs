using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudStorage.Services.Interfaces;
using CloudStorage.Services.Services;
using CloudStorage.Services.Services.ConverterServices;
using CloudStorage.Services.Services.ConverterServices.Factory;

namespace TestServices
{
    [TestClass]
    public class FactoryConverterTests
    {
        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Test_Method_With_Wrong_Extension()
        {
            FactoryConverter.CreateConveterInstace(".avi");
        }
        [TestMethod]
        public void Test_Method_With_Correct_Extension()
        {
            var expected = new TxtFileConverter();

            var result = FactoryConverter.CreateConveterInstace(".txt");

            Assert.AreEqual(expected.GetType(), result.GetType());
        }
    }
}
