//Upload files on server with FormData
function uploadFiles(formData, folderID) {
    var myUrl = '@Url.Action("Upload", "Files")?folderID=0';
            $.ajax({
                xhr: function () {
                    var xhr = new window.XMLHttpRequest();

                    xhr.upload.addEventListener("progress", function (evt) {
                        if (evt.lengthComputable) {
                            var percentComplete = evt.loaded / evt.total;
                            percentComplete = parseInt(percentComplete * 100);
                            document.getElementById('progresBar').value = percentComplete;
                            if (percentComplete === 100) {
                            }
                        }
                    }, false);
                    return xhr;
                },
                type: "POST",
                url: '/Files/UploadFile?currentFolderID=' + folderID,
                contentType: false,
                processData: false,
                dataType: 'html',
                data: formData,
                success: function (data) {
                    $('div#block_view_files_folders').html(data);
                    updateTreeview($('#currentFolderID').val());
                    showFilesInFolder($('#currentFolderID').val());
                },
                error: function (xhr, status, p3) {
                    alert(xhr.responseText);
                }
            });
}