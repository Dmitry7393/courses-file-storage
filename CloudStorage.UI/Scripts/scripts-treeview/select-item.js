function selectItemFileInTreeview(itemInTreeview) {
    //Unselect previous item
    var current = document.getElementsByClassName("selected")[0];
    if (current) {
        current.classList.remove("selected");
    }
    //Select a chosen item
    itemInTreeview.classList.add("selected");
}