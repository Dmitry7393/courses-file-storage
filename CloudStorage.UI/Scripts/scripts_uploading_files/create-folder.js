﻿function createFolder() {
    //get current folder id from hidden field
    var folderID = $('#currentFolderID').val();
    $("#dialog").dialog("open");
    $('#dialog').unbind('submit').bind('submit', function () {
        var name = $("#name").val();
        // requestAddFolder(name, folderID);
        if (name != "" && name != null && name != undefined) {
            AddFolderOnServer(name, folderID);
            $("#dialog").dialog("close");
        }
    });
}
