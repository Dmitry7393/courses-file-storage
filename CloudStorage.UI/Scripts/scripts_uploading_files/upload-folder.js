//Adds virtual folder in table FileInfo
//Returns id of added folder
function AddFolderOnServer(name, parentID) {
    $.ajax({
        url: '/Files/AddFolder',
        type: "POST",
        async: false,
        data: { folderName: name, currentFolderID: parentID },
        success: function (data) {
            var id = parseInt(data.data);
            $('#uploadFolderRootID').val(id);

            updateTreeview($('#currentFolderID').val());
            updateAreaWithFiles($('#currentFolderID').val());
        },
        error: function (xhr) {
            alert('AddFolderOnServer Error getting data.');
        }
    });
}