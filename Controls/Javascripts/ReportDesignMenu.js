
var menuobj = document.getElementById("divDesignMenu");

function showMenu(e) {
    CurrentField = e.currentTarget; //element the event was attached or the element whose eventListener triggered the event
    var rect = CurrentField.getBoundingClientRect();

    menuobj = document.getElementById("divDesignMenu");

    menuobj.style.left = parseInt(rect.left-50, 10) + "px";
    menuobj.style.top = e.y; //rect.top + rect.height + "px";

    e.stopPropagation();

    menuobj.style.visibility = "visible"
    menuobj.style.display = '';
    document.onclick = hidemenu;
    return false;
}

function highlight(e) {
    var firingobj = e.target
    if (firingobj.className == "menuitems" || firingobj.parentNode.className == "menuitems") {
        if (firingobj.parentNode.className == "menuitems")
            firingobj = firingobj.parentNode //up one node
        firingobj.style.backgroundColor = "highlight"
    }
}

function lowlight(e) {
    var firingobj = e.target
    if (firingobj.className == "menuitems" || firingobj.parentNode.className == "menuitems") {
        if (firingobj.parentNode.className == "menuitems")
            firingobj = firingobj.parentNode //up one node
        firingobj.style.backgroundColor = ""
        window.status = ''
    }
}
function hidemenu(e) {
    menuobj.style.visibility = "hidden"
}


function doOption(e) {
    var parentMenu = e.currentTarget;
    var firingobj = e.target

    if (firingobj.className == "menuitems" || firingobj.parentNode.className == "menuitems") {
        if (firingobj.parentNode.className == "menuitems")
            firingobj = firingobj.parentNode

        if (firingobj != null && firingobj.className == "menuitems") {
            var id = firingobj.id;
            switch (id) {
                case "PositionFields":
                    showColumnOrderDlg();
                    break;
                case "ReportDesigner":
                    __doPostBack('ShowReportDesigner', "data");
                    break;
            }
        }

    }
}