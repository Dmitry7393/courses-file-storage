namespace CloudStorage.UI.Controllers
{
    using Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.AspNet.Identity;
    using System.Configuration;
    using System.IO;
    using Models;
    using System.Web;

    /// <summary>
    /// Defines FilesController
    /// </summary>
    public class FilesController : Controller
    {
        /// <summary>
        /// Holds FileService instance
        /// </summary>
        private readonly IFileService _fileService;
        private const string PATH_USER_FOLDER = "PathUserData";
        
        /// <summary>
        /// Initializes a new instance of the <see cref="FilesController"/> class
        /// </summary>
        /// <param name="fileService">The tournament service</param>
        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }
       
        public ActionResult Index()
        {
            //List with subfolders which have to opened after adding files or folders
            ViewBag.ListSubfoldersID = new List<int>(); //treeview will be closed (folded)

            return View();
        }
 
        //Returns user's files in specific folder 
        public PartialViewResult ShowUserFiles(int fileSystemStructureID)
        {
            return PartialView("_BrowsingFiles", _fileService.GetFilesInFolderByUserID(fileSystemStructureID, User.Identity.GetUserId()));
        }
        [HttpPost]
        public PartialViewResult UploadFile(int currentFolderID)
        {
            //transfer uploaded files to Service
            foreach (string fileName in Request.Files)
            {
                _fileService.Create(new Domain.FileAggregate.FileInfo()
                                                                    {
                                                                        Name = Request.Files[fileName].FileName,
                                                                        CreationDate = DateTime.Now,
                                                                        Extension = Path.GetExtension(Request.Files[fileName].FileName.ToLower()),
                                                                        OwnerId = User.Identity.GetUserId(),
                                                                        ParentID = currentFolderID
                                                                    },
                                                                    Request.Files[fileName].InputStream, Server.MapPath(getPathToUserFolder()));
            }

            return PartialView("_BrowsingFiles", _fileService.GetFilesInFolderByUserID(currentFolderID, User.Identity.GetUserId()));
        }
        //Folder will be added in table FileInfo
        //Returns id of added folder
        [HttpPost]
        public JsonResult AddFolder(string folderName, int currentFolderID)
        {
            System.Diagnostics.Debug.WriteLine("Add Folder on server");
            int fileID = _fileService.AddNewFolder(new Domain.FileAggregate.FileInfo()
                                                    {
                                                        Name = folderName,
                                                        CreationDate = DateTime.Now,
                                                        OwnerId = User.Identity.GetUserId(),
                                                        ParentID = currentFolderID
                                                    });
            System.Diagnostics.Debug.WriteLine("return " + fileID);
            return Json(new { data = fileID });
        }
        //Returns preview of the current files in specific folder
        [HttpGet] 
        public PartialViewResult UpdateAreaWithFiles(int currentFolderID)
        {
            return PartialView("_BrowsingFiles", _fileService.GetFilesInFolderByUserID(currentFolderID, User.Identity.GetUserId()));
        }
        [HttpGet]
        public PartialViewResult UpdateTreeview(int currentFolderID)
        {
            //List with subfolders which have to opened after adding files or folders
            ViewBag.ListSubfoldersID = _fileService.GetSubfoldersByFolderID(currentFolderID);
            return PartialView("_Treeview", _fileService.GetFilesByUserID(User.Identity.GetUserId()));
        }
        //Returns the physical path to user folder on server
        private string getPathToUserFolder()
        {
            return Path.Combine(ConfigurationManager.AppSettings[PATH_USER_FOLDER].ToString(), User.Identity.GetUserId());
        }

        //Returns a thumbnail into view
        public ActionResult GetImage(int fileID)
        {
            Domain.FileAggregate.FileInfo file = _fileService.GetFileById(fileID, User.Identity.GetUserId());

            var dir = Server.MapPath("~/Content/Icons");
            switch (file.Extension)
            {
                //if it is a folder
                case null:
                    return File(Path.Combine(dir, "icon-folder.png"), GetContentType(file.Extension));
                case ".jpg":
                    return File(_fileService.GetImageBytes(fileID, Server.MapPath(getPathToUserFolder())), GetContentType(file.Extension));
                case ".png":
                    return File(_fileService.GetImageBytes(fileID, Server.MapPath(getPathToUserFolder())), GetContentType(file.Extension));
                case ".docx":
                    return File(Path.Combine(dir, "icon-docx.png"), GetContentType(file.Extension));
                case ".txt":
                    return File(Path.Combine(dir, "icon-txt.png"), GetContentType(file.Extension));
                case ".pdf":
                    return File(Path.Combine(dir, "icon-pdf.png"), GetContentType(file.Extension));
            }
            return File(Path.Combine(dir, "icon-file.png"), GetContentType(file.Extension));
        }
        /// <summary>
        /// Download file.
        /// </summary>
        /// <param name="id">Identifier of file.</param>
        /// <returns>File for download.</returns>
        public ActionResult Download(int id)
        {
            var file = this._fileService.GetFileById(id, User.Identity.GetUserId());

            if (file == null)
            {
                return HttpNotFound();
            }

            return File(Url.Content(Server.MapPath(file.PathToFile))
                                    , GetContentType(file.Extension)
                                    , file.FullName);
        }

        /// <summary>
        /// Get type of content that depends on of the file extension.
        /// </summary>
        /// <param name="extension">Extension of file.</param>
        /// <returns>Content type.</returns>
        private string GetContentType(string extension)
        {
            switch (extension)
            {
                case "txt":
                    return "text/plain";
                case "jpeg":
                    return "image/pneg";
                case "jpg":
                    return "image/jpg";
                case "png":
                    return "image/png";
                case "pdf":
                    return "application/pdf";
                case ".flv":
                    return "video/x-flv";
                default:
                    return "application/unknown";
            }
        }
	}
}