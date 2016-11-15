$(document).ready(function () {
    var itemId;
    var ckEditor = CKEDITOR.replace('editor1');
    ckEditor.on("instanceReady", function () {
        ckEditor.addCommand("save", {
            modes: { wysiwyg: 1, source: 1 },
            exec: function () {
                var htmlText = ckEditor.getData();
                var id = $("textarea").text();
                $.ajax({
                    url: "/Files/Redact",
                    type: "POST",
                    dataType: "json",
                    data: { id: id, htmlText: htmlText }
                });
            }
        })
    }


    );

    function test(obj) {
        itemId = obj;
        $("#" + obj).popr();
    };
    function del(obj) {
        //console.log(itemId);
    }
    function redact(obj) {
        $.ajax({
            url: "/Files/Redact",
            type: "GET",
            dataType: "json",
            data: { id: itemId },
            success: function (data) {
                CKEDITOR.instances['editor1'].setData(data.responseText);
                console.log($("textarea").text());
                $("#myModal").modal();
            }
        });
    };

    //CKEDITOR.on('instanceReady',
    //function (evt) {
    //    var editor = evt.editor;
    //    editor.execCommand('maximize');
    //});
});