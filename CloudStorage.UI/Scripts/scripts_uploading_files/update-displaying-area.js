function updateAreaWithFiles(folderID) {
    $.ajax({
        url: '/Files/UpdateAreaWithFiles',
        type: "GET",
        data: {currentFolderID: folderID },
        dataType: "html",
        success: function (data) {
            $('div#block_view_files_folders').html(data);
        },
        error: function (xhr) {
            alert('Error getting data.');
        }
    });
}