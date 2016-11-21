namespace CloudStorage.Entity.Interfaces
{
    using CloudStorage.Domain.FileAggregate;
    using System.Collections.Generic;

    /// <summary>
    /// Defines specific contract for FileInfoRepository
    /// </summary>
    public interface IFileInfoRepository
    {
        int Add(FileInfo newFile);

        void Update(FileInfo updatedFile);

        void Remove(int id);

        FileInfo GetFileById(int id);

        FileInfo GetFileById_UserId(int fileId, string userId);

        List<FileInfo> GetFilesByUserId(string iserId);

        List<FileInfo> GetFilesInFolderByUserID(int currentFolder, string userID);

        List<int> GetSubFolders(int folderID);

        List<int> GetNestedFolders(int folderID);

        List<int> GetSubFoldersInCertainFolder(int folderID, int rootDirectoryID);
    }
}