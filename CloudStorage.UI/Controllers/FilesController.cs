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
    using CloudStorage.Services.Services.ConverterServices.Factory;

    /// <summary>
    /// Defines FilesController
    /// </summary>
    [Authorize]
    [RequireHttps]
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
        public PartialViewResult OpenFolder(int elementID)
        {
            return PartialView("_BrowsingFiles", _fileService.GetFilesInFolderByUserID(elementID, User.Identity.GetUserId()));
        }

        [HttpPost]
        public PartialViewResult UploadFile(int currentFolderID)
        {
            //transfer uploaded files to Service
            string fileNameWithoutExtension;
            foreach (string fileName in Request.Files)
            {
                fileNameWithoutExtension = Path.GetFileNameWithoutExtension(Request.Files[fileName].FileName);
                _fileService.Create(new Domain.FileAggregate.FileInfo()
                                                                    {
                                                                        Name = fileNameWithoutExtension,
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
            int fileID = _fileService.AddNewFolder(new Domain.FileAggregate.FileInfo()
                                                    {
                                                        Name = folderName,
                                                        CreationDate = DateTime.Now,
                                                        OwnerId = User.Identity.GetUserId(),
                                                        ParentID = currentFolderID
                                                    });
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
            ViewBag.FolderID = currentFolderID;
            //List with subfolders which have to opened after adding files or folders
            ViewBag.ListSubfoldersID = _fileService.GetSubfoldersByFolderID(currentFolderID);
            return PartialView("_Treeview", _fileService.GetFilesByUserID(User.Identity.GetUserId()));
        }
        //Returns the physical path to user folder on server
        private string getPathToUserFolder()
        {
            return Path.Combine(ConfigurationManager.AppSettings[PATH_USER_FOLDER].ToString(), User.Identity.GetUserId());
        }
        private string getPathToUser_Data()
        {
            return ConfigurationManager.AppSettings[PATH_USER_FOLDER].ToString();
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
        [HttpGet]
        public ActionResult Download(int id)
        {
            var file = this._fileService.GetFileById(id, User.Identity.GetUserId());

            if (file == null)
            {
                return HttpNotFound();
            }

            if (file.Extension != null)
            {
                string path = Path.Combine(getPathToUser_Data(), file.PathToFile);

                //Return file
                return File(Url.Content(Path.Combine(getPathToUser_Data(), file.PathToFile))
                                                 , GetContentType(file.Extension)
                                                 , file.FullName);
            }
            else
            {
                //Return archive with files and subfolders
                return File(_fileService.GetZipArchive(Server.MapPath(getPathToUserFolder()), id, User.Identity.GetUserId()), "application/zip", file.Name + ".zip");
            }
        }



        // Summary
        //  Redact text file
        //
        // Parameters:
        //   id:
        // Identifier of redacting file
        [HttpGet]
        public JsonResult Redact(int id)
        {
            // Summary:
            //     Get the new instance of redacting FileInfo object
            var file = this._fileService.GetFileById(id, User.Identity.GetUserId());

            // Summary:
            //     If such file doesn't exist return error
            if (file == null)
            {
                return Json(new { error = true, reason = "File not found" }, JsonRequestBehavior.AllowGet);
            }
            IFileConverter converter = null;
            try
            {
                // Summary:
                //     Get the new instance of IFileConverter intarface that depend on file extension
                converter = FactoryConverter.CreateConveterInstace(file.Extension);

                // Summary:
                //     Create fileName that actual stored on server
                var name = file.Name.Substring(0, file.Name.IndexOf("."));
                name += ".dat";

                // Summary:
                //     htmlText string that represent all text from redacting file
                string htmlText = converter.ToHtml(Server.MapPath(getPathToUserFolder()) + "\\" + file.Id + ".dat");
                return Json(new { success = true, responseText = htmlText, fileName = file.Name }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, reason = ex.Message }, JsonRequestBehavior.AllowGet);
            }

           
        }

        [ValidateInput(false)]
        [HttpPost]
        public JsonResult Redact(int id, string htmlText)
        {
            // Summary:
            //     Get the new instance of redacting FileInfo object
            var file = this._fileService.GetFileById(id, User.Identity.GetUserId());

            IFileConverter converter;
            try
            {
                // Summary:
                //     Get the new instance of IFileConverter intarface that depend on file extension
                converter = FactoryConverter.CreateConveterInstace(file.Extension);

                // Summary:
                //     Create fileName that actual stored on server

                var name = file.Name.Substring(0, file.Name.IndexOf("."));
                name += ".dat";

                // Summary:
                //    Save all changed text to file
                converter.FromHtml(Server.MapPath(getPathToUserFolder()) + "\\" + file.Id + ".dat", htmlText);
                return Json(new { success = true, responseText = htmlText, fileName = file.Name }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, reason = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            Domain.FileAggregate.FileInfo file = null;
            try
            {
                // Summary:
                //     Get the new instance of FileInfo object that will be deleted
                file = _fileService.GetFileById(id, User.Identity.GetUserId());

                // Summary:
                //    Delete file
                this._fileService.Delete(id, User.Identity.GetUserId(), Server.MapPath(getPathToUserFolder()));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", ex);
            }
            // Summary:
            //     Return PartialView and using created instace of file for getting current folder
            return PartialView("_BrowsingFiles", _fileService.GetFilesInFolderByUserID(file.ParentID, User.Identity.GetUserId()));

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



        [HttpGet]
        public PartialViewResult Info(int id = 0)
        {

            Domain.FileAggregate.FileInfo file = null;

            UI.Models.AboutFileInfoModel infoModel = new UI.Models.AboutFileInfoModel();
            try
            {
                // Summary:
                //     Get the new instance of FileInfo object that will be deleted
                file = _fileService.GetFileById(id, User.Identity.GetUserId());
                if (file != null)
                {
                    infoModel.Name = file.Name;
                    infoModel.Type = GetContentType(file.Extension);
                    infoModel.CreateDate = file.CreationDate;
                    if (file.ParentID == 0)
                    {
                        infoModel.Folder = "MyCloud";
                    }
                    else
                    {

                        infoModel.Folder = _fileService.GetFileById(file.ParentID, User.Identity.GetUserId()).Name;
                    }
                    string pathToFile = Server.MapPath(getPathToUserFolder()) + "\\" + file.Id + ".dat";
                    System.IO.FileInfo fs = new FileInfo(pathToFile);
                    infoModel.LastTimeChanged = fs.LastWriteTime;
                    string unit = "";
                    string size = GetSizeOfFile(fs.Length, out unit) + " " + unit;
                    infoModel.Size = size;
                    infoModel.ShareLink = file.Link;
                    ViewData["partialModel"] = infoModel;

                }
                ViewBag.Info = infoModel;
            }
            catch (Exception ex)
            {

            }
            // Summary:
            //     Return PartialView and using created instace of file for getting current folder
            return PartialView("_Info");
        }

        private static string GetSizeOfFile(long lenght, out string unit)
        {
            double kb = 1026;
            double mb = 1054661;
            float gb = 1080963085.517241F;
            string size = "";
            if (lenght >= gb)
            {
                unit = "GB";
                size = (lenght / gb).ToString();
            }
            else if (lenght >= mb)
            {
                unit = "MB";
                size = (lenght / mb).ToString();
            }
            else
            {
                unit = "KB";
                size = (lenght / kb).ToString();
            }
            return size.Substring(0, size.IndexOf(',') + 3);
        }
	}
}