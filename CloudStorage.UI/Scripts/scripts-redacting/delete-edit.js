var itemId;
function test(obj) {
    itemId = obj;

    console.log($('input[name=' + itemId + ']').val());
    $("#" + obj).popr();

};
function del(obj) {
    var a = "123";

    $.ajax({
        url: "/Files/Delete",
        type: "GET",
        dataType: "json",
        data: { id: itemId }
    }

    ).complete(function (partialViewResult) {
        updateTreeview($('#currentFolderID').val());
        showFilesInFolder($('#currentFolderID').val());
    });

};
function redact(obj) {
    console.log("aaa");
    $.ajax({
        url: "/Files/Redact",
        type: "GET",
        dataType: "json",
        data: { id: itemId },

        success: function (data) {
            CKEDITOR.instances['editor1'].setData(data.responseText);
            $("textarea").text(itemId);
            $(".modal-title").text(data.fileName);
            $("#myModal").modal();
        }
    });
};

