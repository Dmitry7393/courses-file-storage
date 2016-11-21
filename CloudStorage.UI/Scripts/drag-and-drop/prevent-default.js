//Prevent opening files and folders in browser
$("html").on("dragover", function (event) {
    event.preventDefault();
    event.stopPropagation();
});

$("html").on("dragleave", function (event) {
    event.preventDefault();
    event.stopPropagation();
});

$("html").on("drop", function (event) {
    event.preventDefault();
    event.stopPropagation();
});