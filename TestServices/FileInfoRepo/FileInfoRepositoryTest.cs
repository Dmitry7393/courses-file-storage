using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudStorage.Entity.Repositories;
using CloudStorage.Domain.FileAggregate;
using System.Collections.Generic;
using CloudStorage.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using CloudStorage.Domain;

namespace TestServices.FileInfoRepo
{
    [TestClass]
    public class FileInfoRepositoryTest
    {
        FileInfoRepository repository;
        List<FileInfo> testedFiles;
        CloudStorageDbContext testedFileContext;
        IdentityDbContext<User> userContext;
        User testedUser;

        [TestInitialize]
        public void Inititalize()
        {
            userContext = new IdentityDbContext<User>("CloudStorageConnection");
            testedFileContext = new CloudStorageDbContext("CloudStorageConnection");
            testedUser = new User() { UserName = "TEST!" };
            userContext.Users.Add(testedUser);
            userContext.SaveChanges();

            testedFileContext = new CloudStorageDbContext();
            repository = new FileInfoRepository();
            testedFiles = new List<FileInfo>()
            {
               new FileInfo(){ OwnerId = testedUser.Id, Extension ="txt", Name ="testedFile1", CreationDate = DateTime.Now, ParentID = 0, Link="1233" },
               new FileInfo(){ OwnerId = testedUser.Id, Extension ="dll", Name ="testedFile2", CreationDate = DateTime.Now, ParentID = 0 },
               new FileInfo(){ OwnerId = testedUser.Id, Extension ="md", Name ="testedFile3", CreationDate = DateTime.Now, ParentID = 0 }
            };
            testedFileContext.Files.AddRange(testedFiles);
            testedFileContext.SaveChanges();
        }
        [TestCleanup]
        public void CleanUp()
        {
            userContext.Users.Remove(testedUser);
            userContext.SaveChanges();
            testedFileContext.Files.RemoveRange(testedFiles);
            testedFileContext.SaveChanges();
        }
        [TestMethod]
        public void Repository_Add_New_File_Test()
        {
            //arrange
            int notExpected = 0;
            //act
            var result = repository.Add(new FileInfo() { CreationDate = DateTime.Now });
            //assert
            Assert.AreNotSame(notExpected, result);
        }
        [TestMethod]
        public void Repository_Get_File_By_Id_Test()
        {
            //arrange
            int expected = testedFiles[0].Id;
            //act
            var result = repository.GetFileById(testedFiles[0].Id);
            //assert
            Assert.AreEqual(expected, result.Id);
        }
        [TestMethod]
        public void Repository_Get_File_By_User_Id_Test()
        {
            //arrange
            int expected = testedFiles[0].Id;
            //act
            var result = repository.GetFileById_UserId(testedFiles[0].Id, testedUser.Id);
            //assert
            Assert.AreEqual(expected, result.Id);
        }
        [TestMethod]
        public void Repository_Get_File_By_Link_Test()
        {
            //arrange
            int expected = testedFiles[0].Id;
            //act
            var result = repository.GetFileByLink(testedFiles[0].Link);
            //assert
            Assert.AreEqual(expected, result.Id);
        }

    }
}
