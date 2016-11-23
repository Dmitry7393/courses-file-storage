using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CloudStorage.Services.Interfaces;
using CloudStorage.UI.Controllers;
using System.Web.Mvc;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Configuration;
using Microsoft.AspNet.Identity.Owin;
using CloudStorage.Domain;
using CloudStorage.Entity;
using CloudStorage.UI.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using CloudStorage.Domain.FileAggregate;
using System.Web;
using System.Security.Principal;
using CloudStorage.Services.Services;
using CloudStorage.Entity.Repositories;


namespace TestServices
{
    [TestClass]
    public class FileServiceTest
    {
        IdentityDbContext<User> userContext;
        User testedUser;
        List<FileInfo> testedFiles;
        CloudStorageDbContext testedFileContext;
        FilesController controller;
        FileService service;

        [TestInitialize]
        public void Initialize()
        {
            service = new FileService(new FileInfoRepository());
            userContext = new IdentityDbContext<User>("CloudStorageConnection");
            testedFileContext = new CloudStorageDbContext("CloudStorageConnection");
            testedUser = new User() { UserName = "TEST!" };
            userContext.Users.Add(testedUser);
            userContext.SaveChanges();


            testedFiles = new List<FileInfo>()
            {
               new FileInfo(){  OwnerId = testedUser.Id, Extension ="txt", Name ="testedFile1", CreationDate = DateTime.Now, ParentID = 0, Link="1233" },
               new FileInfo(){  OwnerId = testedUser.Id, Extension ="dll", Name ="testedFile2", CreationDate = DateTime.Now, ParentID = 0 },
               new FileInfo(){  OwnerId = testedUser.Id, Extension ="md", Name ="testedFile3", CreationDate = DateTime.Now, ParentID = 0 }
            };
            testedFileContext.Files.AddRange(testedFiles);
            testedFileContext.SaveChanges();
            var moq = new Mock<IFileService>();

            controller = new FilesController(moq.Object);
        }

        [TestCleanup]
        public void Clean()
        {
            userContext.Users.Remove(testedUser);
            userContext.SaveChanges();
            testedFileContext.Files.RemoveRange(testedFiles);
            testedFileContext.SaveChanges();
        }

        [TestMethod]
        public void FileService_Get_File_By_Id_Test()
        {
            //arrange
            var expected = testedFiles[0].Id;
            //act
            var fileResult = service.GetFileById(testedFiles[0].Id, testedUser.Id);
            //assert
            Assert.AreEqual(expected, fileResult.Id);
        }

        [TestMethod]
        public void FileService_Get_Files_By_User_Id_Test()
        {
            //arrange
            var expected = testedFiles.Count;
            //act
            var fileResult = service.GetFilesByUserID(testedUser.Id);
            //assert
            Assert.AreEqual(expected, fileResult.Count);
        }

        [TestMethod]
        public void FileService_Get_Files_By_Link_Test()
        {
            //arrange 
            var expected = testedFiles[0];
            //act
            var fileResult = service.GetFileByLink(service.GetFileById(testedFiles[0].Id, testedUser.Id).Link);
            //Assert
            Assert.AreEqual(expected.Id, fileResult.Id);
        }
        [TestMethod]
        public void FileService_Get_Files_In_Folder_By_User_Id_Test()
        {
            //arrange          
            var expected = testedFiles;
            //act
            var fileResult = service.GetFilesInFolderByUserID(0, testedUser.Id);
            //Assert
            Assert.AreEqual(expected.Count, fileResult.Count);
        }

        [TestMethod]
        public void FileService_Get_Subfolders_By_Folder_Id_Test()
        {
            //arrange          
            var expected = 1;
            //act
            var fileResult = service.GetSubfoldersByFolderID(0);
            //Assert
            Assert.AreEqual(expected, fileResult.Count);
        }
    }
}
