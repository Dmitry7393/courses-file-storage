//Creating FormData object
//FormData helps to send files, value to server through AJAX request.
function createFormData(file, folderID) {
    var formData = new FormData();
    formData.append("file" + 0, file);
    //sending files through AJAX request in specific folder
    uploadFiles(formData, folderID);
}