function openFolder(itemID, extension) {
    //set in hidden field new opened folder ID
    $("#currentFolderID").val(itemID);
    $.ajax({
        url: '/Files/OpenFolder',
        type: "POST",
        data: { elementID: itemID },
        dataType: "html",

        success: function (data) {
            //Open this new folder in Treeview
            updateTreeview(itemID);
            //update partial view _BrowsingFiles
            $('div#block_view_files_folders').html(data);
        },
        error: function (xhr) {
            alert(xhr);
        }
    });
}