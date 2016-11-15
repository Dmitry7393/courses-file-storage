$(document).ready(function () {
    //Show files in root folder
    showFilesInFolder(0);
})

//Download preview in chosen folder
function showFilesInFolder(itemID)
{
    $("#currentFolderID").val(itemID);
    $.ajax({
        url: '/Files/ShowUserFiles',
        type: "POST",
        data: { fileSystemStructureID: itemID },
        dataType: "html",

        success: function (data) {
            //update partial view _BrowsingFiles
            $('div#block_view_files_folders').html(data);
        },
        error: function (xhr) {
            alert(xhr);
        }
    });
}