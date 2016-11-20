//Get data from server and update partial view _Treeview
function updateTreeview(folderID) {
    $.ajax({
        url: '/Files/UpdateTreeview?currentFolderID=' + folderID,
        type: "GET",
        dataType: "html",
        success: function (data) {
            //apply new data in treeview
            $('div#partial_view_treeview').html(data);
            //reload data in treeview and show path to current folder
            showTreeview();
        },
        error: function (xhr) {
            alert(xhr);
        }
    });
}