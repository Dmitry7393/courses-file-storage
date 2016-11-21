var itemId;
function test(obj) {
    itemId = obj;
    $.ajax({

        url: "/Files/Info",
        type: "GET",
        dataType: "html",
        data: { id: itemId },
        sucess: function (data) {
            console.log("sucess");

        }

    }).done(function (data) {
        console.log("done");
        $('div#info').html(data);
    });
    console.log($('input[name=' + itemId + ']').val());
    $("#" + obj).popr();

};


function download() {
    $.ajax({
        url: "/Files/Download",
        type: "GET",
        dataType: "json",
        data: { id: itemId }
    });
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

