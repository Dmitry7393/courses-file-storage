namespace CloudStorage.Entity.Repositories
{
    using CloudStorage.Entity.Interfaces;
    using CloudStorage.Domain.FileAggregate;
    using System;
    using System.Linq;
    using System.Collections.Generic;

    /// <summary>
    /// Defines implementation of the IFileInfoRepository contract.
    /// </summary>
    public class FileInfoRepository : ContextRepository, IFileInfoRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileInfoRepository"/> class.
        /// </summary>
        public FileInfoRepository()
        { }

        /// <summary>
        /// Adds new file.
        /// </summary>
        /// <param name="newFile">The file for adding.</param>
        public int Add(FileInfo newFile)
        {
            using (var context = CreateContext())
            {
                context.Files.Add(newFile);
                context.SaveChanges();
                return newFile.Id; 
            }
        }
        /// <summary>
        /// Updates specified file.
        /// </summary>
        /// <param name="updatedFile">Updated file.</param>
        public void Update(FileInfo updatedFile)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes file by id.
        /// </summary>
        /// <param name="id">The id of file to remove.</param>
        public void Remove(int id)
        {
            using (var context = CreateContext())
            {
                var file = context.Files.Where(i => i.Id == id).FirstOrDefault();
                if (file != null)
                {
                    context.Files.Remove(file);

                }
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Get information about file by id.
        /// </summary>
        /// <param name="id">Identifier of file.</param>
        /// <returns>Information about file.</returns>
        public FileInfo GetFileById(int id)
        {
            using (var context = CreateContext())
            {
                FileInfo file = context.Files.Where(f => f.Id == id).SingleOrDefault();
                return file;
            }
        }

        /// <summary>
        /// Get information about file by file id and user id.
        /// </summary>
        /// <param name="fileId">Identifier of file.</param>
        /// <param name="userId">Identifier of user.</param>
        /// <returns>Information about file.</returns>
        public FileInfo GetFileById_UserId(int fileId, string userId)
        {
            using (var context = CreateContext())
            {
                FileInfo file = context.Files
                                    .Where(f => f.Id == fileId).Where(user => user.OwnerId == userId)
                                    .SingleOrDefault();

                return file;
            }
        }

        //Returns files in order to display file system in treeview
        public List<FileInfo> GetFilesByUserId(string iserId)
        {
            //Select all files logged in user
            using (var context = CreateContext())
            {
                return context.Files.Where(u => u.OwnerId == iserId).ToList();
            }
        }

        //Returns name of files in order to display them in view
        public List<FileInfo> GetFilesInFolderByUserID(int currentFolder, string userID)
        {
            using (var context = CreateContext())
            {
                //Select name of files which belong to current user in specific folder
                return context.Files.Where(u => u.ParentID == currentFolder).Where(user => user.OwnerId == userID).ToList();
            }
        }

        // Recursive search subfolders using current folderID
        // search stops when root directory (0) have been reached
        public IEnumerable<int> FindSubFoldersID(CloudStorageDbContext context, int id, bool isIdAdded)
        {
            if (isIdAdded)
                yield return id;
            int parentID = context.Files.Where(u => u.Id == id).Select(field => field.ParentID).SingleOrDefault();
            if (parentID != 0)
            {
                yield return parentID;
                foreach (int n in FindSubFoldersID(context, parentID, false))
                {
                   yield return n;
                }
            }
        }
        // Returns list with ID subfolders, which have to be opened in treeview
        // after updating treeview with new files and folders
        public List<int> GetSubFolders(int folderID)
        {
            using (var context = CreateContext())
            {
                return FindSubFoldersID(context, folderID, true).ToList();
            }
        }

        //Returns list with subfolders in folder
        public List<int> GetNestedFolders(int folderID)
        {
            using (var context = CreateContext())
            {
                //Select nested folders from specific folder
                return context.Files.Where(u => u.ParentID == folderID).Where(s => s.Extension == null).Select(field => field.Id).ToList();
            }
        }
    }
}
