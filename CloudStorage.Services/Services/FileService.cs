﻿namespace CloudStorage.Services.Services
{
    using CloudStorage.Domain.FileAggregate;
    using CloudStorage.Entity.Interfaces;
    using CloudStorage.Services.Interfaces;
    using Ionic.Zip;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Defines an implementation of <see cref="IFileService"/> contract.
    /// </summary>
    public class FileService : IFileService
    {
        private const int LINK_SIZE = 30;
        private const string CHARS
             = "%!abcdefghijklmnopqrstuvwxyz1234567890?ABCDEFGHIJKLMNOPQRSTUVWXYZ^&";

        private readonly IFileInfoRepository _fileInfoRepository;
        
        private const string BASE_EXTENSION = ".dat";
        /// <summary>
        /// Initializes a new instance of the <see cref="FileService"/> class.
        /// </summary>
        /// <param name="fileInfoRepository">Instance of class which implements <see cref="IFileInfoRepository"/>.</param>
        public FileService(IFileInfoRepository fileInfoRepository)
        {
            this._fileInfoRepository = fileInfoRepository;
        }

        /// <summary>
        /// Creates a new file.
        /// </summary>
        /// <param name="file">File to create.</param>
        public void Create(Domain.FileAggregate.FileInfo file, Stream fileStream, string pathToUserFolder)
        {
            file.Link = this.GenerateLink();

            //Adding information about file to database using FileInfoRepository
            //and return fileID of added file
            int fileID = _fileInfoRepository.Add(file);
            var fileName = fileID + ".dat";

            //Folder for user's files will be created when user adds file
            if (!Directory.Exists(pathToUserFolder))
                Directory.CreateDirectory(pathToUserFolder);

            //save file on server in user's folder
            using (Stream destination = File.Create(Path.Combine(pathToUserFolder, fileName)))
                Write(fileStream, destination);
            
        }
        
        public List<Domain.FileAggregate.FileInfo> GetFilesByUserID(string userId)
        {
            //This list will be returned to view Treeview.cshtml
            return _fileInfoRepository.GetFilesByUserId(userId);
        }

        public Domain.FileAggregate.FileInfo GetFileByLink(string link)
        {
            return _fileInfoRepository.GetFileByLink(link);
        }

        public List<Domain.FileAggregate.FileInfo> GetFilesInFolderByUserID(int currentFolder, string userID)
        {
             return _fileInfoRepository.GetFilesInFolderByUserID(currentFolder, userID);
        }
        public void Write(Stream from, Stream to)
        {
            for (int a = from.ReadByte(); a != -1; a = from.ReadByte())
                to.WriteByte((byte)a);
        }
        public int AddNewFolder(Domain.FileAggregate.FileInfo folder)
        {
            folder.Link = this.GenerateLink();
            return _fileInfoRepository.Add(folder);
        }

        /// <summary>
        /// Deletes file by its identifier.
        /// </summary>
        /// <param name="id">Identifier of file.</param>
        public void Delete(int id, string userId, string pathToUserFolder)
        {
            var nestedFolders = _fileInfoRepository.GetNestedFolders(id);
            if (nestedFolders != null)
            {
                foreach (var item in nestedFolders)
                {
                     Delete(item, userId, pathToUserFolder);
                }
                var nestedFiles = _fileInfoRepository.GetFilesInFolderByUserID(id, userId);
                foreach (var itemFile in nestedFiles)
                {
                    if (itemFile.Extension != null)
                    {
                        RemoveFileFromServer(pathToUserFolder, itemFile.Id);
                        _fileInfoRepository.Remove(itemFile.Id);
                    }
                }
                _fileInfoRepository.Remove(id);
            }
        }
        private void RemoveFileFromServer(string pathToUserFolder, int fileID)
        {
            if (File.Exists(Path.Combine(pathToUserFolder, fileID+BASE_EXTENSION)))
            {
                File.Delete(Path.Combine(pathToUserFolder, fileID+BASE_EXTENSION));
            }
        }
        /// <summary>
        /// Get information about file by id.
        /// </summary>
        /// <param name="fileId">Identifier of file.</param>
        /// <param name="userId">Identifier of user.</param>
        /// <returns>Information about file.</returns>
        public Domain.FileAggregate.FileInfo GetFileById(int fileId, string userId)
        {
            return this._fileInfoRepository.GetFileById_UserId(fileId, userId);
        }

        public void Edit(Domain.FileAggregate.FileInfo file)
        {
            throw new NotImplementedException();
        }
        // Returns list with ID subfolders, which have to be opened in treeview
        // after updating treeview with new files and folders
        public List<int> GetSubfoldersByFolderID(int folderID)
        {
            return _fileInfoRepository.GetSubFolders(folderID);
        }

        //return a byte array of the image
        public byte[] GetImageBytes(int fileID, string pathToUserFolder)
        {
            string path = Path.Combine(pathToUserFolder, fileID.ToString() + ".dat");
            return File.ReadAllBytes(path);
        }

        //Returns archive with nested files and folders
        public MemoryStream GetZipArchive(string pathToUserFolder, int folderID, string userID)
        {
            var outputStream = new MemoryStream();
            var folderFile = GetFileById(folderID, userID);
            using (var zip = new ZipFile())
            {
                FindFiles(zip, folderID, userID, pathToUserFolder, folderFile.Name, folderFile.ParentID);
                zip.Save(outputStream);
            }
            outputStream.Position = 0;
            return outputStream;
        }

        //Recursive search of files in subdirectories.
        //Adding all files and folders in zip archive 
        public void FindFiles(ZipFile zip, int id, string userId, string pathToUserFolder, string pathInArchive, int rootDirID)
        {
            var nestedFolders = _fileInfoRepository.GetNestedFolders(id);
            var nestedFiles = _fileInfoRepository.GetFilesInFolderByUserID(id, userId);

            //Adding current directory in zip file
            zip.AddDirectoryByName(pathInArchive);
            foreach (var item in nestedFiles)
            {
                //Add file into zip archive with the actual name
                if (item.Extension != null)
                {
                    var e = zip.AddFile(Path.Combine(pathToUserFolder, item.Id + ".dat"), pathInArchive);
                    e.FileName = Path.Combine(pathInArchive, item.Name + item.Extension);
                }
            }
            if (nestedFolders != null)
            {
                foreach (var item in nestedFolders)
                {
                    //Getting path in subfolders
                    List<int> listSubFolders = _fileInfoRepository.GetSubFoldersInCertainFolder(item, rootDirID);
                    listSubFolders.Reverse();
                    string pathFolderInZip = "";
                    foreach (var fileid in listSubFolders)
                    {
                        pathFolderInZip = Path.Combine(pathFolderInZip, GetFileById(fileid, userId).Name);
                    }
                    FindFiles(zip, item, userId, pathToUserFolder, pathFolderInZip, rootDirID);
                }
            }
        }

        private string GenerateLink()
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < LINK_SIZE; i++)
            {
                int index = random.Next(LINK_SIZE);
                builder.Append(CHARS[index]);
            }

            return builder.ToString();
        }
    }
}