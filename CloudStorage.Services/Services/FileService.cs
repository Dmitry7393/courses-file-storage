namespace CloudStorage.Services.Services
{
    using CloudStorage.Domain.FileAggregate;
    using CloudStorage.Entity.Interfaces;
    using CloudStorage.Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Defines an implementation of <see cref="IFileService"/> contract.
    /// </summary>
    public class FileService : IFileService
    {
        private readonly IFileInfoRepository _fileInfoRepository;

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
        public List<Domain.FileAggregate.FileInfo> GetFilesInFolderByUserID(int currentFolder, string userID)
        {
             return _fileInfoRepository.GetFilesInFolderByUserID(currentFolder, userID);
        }
        public void Write(Stream from, Stream to)
        {
            for (int a = from.ReadByte(); a != -1; a = from.ReadByte())
                to.WriteByte((byte)a);
        }
        public void AddNewFolder(Domain.FileAggregate.FileInfo folder)
        {
             _fileInfoRepository.Add(folder);
        }

        /// <summary>
        /// Deletes file by its identifier.
        /// </summary>
        /// <param name="id">Identifier of file.</param>
        public void Delete(int id, string userId)
        {
            // NOTE
            // 
            // For correct deleting file from server need send a Server.MapPath

            var file = _fileInfoRepository.GetFileById(id);
            if (file.Extension != null)
            {
                _fileInfoRepository.Remove(id);
            }
            else
            {
                //this.GetFilesInFolderByUserID();
                var nestedFolders = this.GetSubfoldersByFolderID(id);
                var nestedFiles = _fileInfoRepository.GetFilesInFolderByUserID(id, userId);
                foreach (var item in nestedFiles)
                {
                    _fileInfoRepository.Remove(item.Id);
                }
                if (nestedFolders != null)
                {
                    foreach (var item in nestedFolders)
                    {
                        Delete(item, userId);
                    }
                }
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
    }
}