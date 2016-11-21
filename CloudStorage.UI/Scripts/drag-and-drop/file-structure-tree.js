//Data structure - save information about previous added data
function FileInfo(fileID, Path, parentID) {
    this.id = fileID;
    this.path = Path;
    this.parentid = parentID;
};
var arr = [];

//Returns parentID, where the next file or folder will be added
function getParentID(pathToFile) {
    for (var i = 0; i < arr.length; i++) {
        if (arr[i].path == pathToFile) {
            return arr[i].id;
        }
    }
    //root folder will be added in chosen by user folder on server
    return $('#currentFolderID').val();
}

//review the overall structure of tree
//sending folder and files on server
function traverseFileTree(item, path) {
    path = path || "";
    if (item.isFile) {
        item.file(function (file) {
            createFormData(file, getParentID(path));
        });
    } else if (item.isDirectory) {
        var itemPath = path + item.name + "/";
        // Add folder and extract fileID from database
        var parentIDForThisElement = getParentID(path);
        AddFolderOnServer(item.name, parentIDForThisElement);
        arr.push(new FileInfo($('#uploadFolderRootID').val(), itemPath, parentIDForThisElement));
        
        // Get folder contents
        var dirReader = item.createReader();
        dirReader.readEntries(function (entries) {
            for (var i = 0; i < entries.length; i++) {
                traverseFileTree(entries[i], path + item.name + "/");
            }
        });
    }
}
window.addEventListener("load", function (event) {
    //Drag and drop folders and files
    var dropzone = document.getElementById('dropzone');
    dropzone.ondrop = function (e) {
        var items = e.dataTransfer.items;
        for (var i = 0; i < items.length; i++) {
            var item = items[i].webkitGetAsEntry();
            if (item) {
                //handle all input files and folders
                traverseFileTree(item);
            }
        }
    }
});
