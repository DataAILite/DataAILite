function ListItem() {
    this.listItem = {
        text: "",
        value: "",
        selected: false
    }
    return this.listItem;
}
var isExactMatch;
var chartType;
var defaultDashboard;
var defaultDashboardAvailable;

function addToDashboardClicked() {
    var dlgDashBoard = document.getElementById("divPopupBackground");
    var txtSearch = document.getElementById("txtSearch");
    var divChart = document.getElementById("chart_div");
    var btnSave = document.getElementById("btnSubmit");
    var ddDashboards = document.getElementById("ddDashboards_Checklist");
    var lstItems = document.getElementById("lstItems");
    var ddChartType = document.getElementById("DropDownChartType");

    defaultDashboard = document.getElementById("hdnDefaultDashboard").value;
    defaultDashboardAvailable = document.getElementById("hdnDefaultDashboardAvailable").value;

    chartType = ddChartType.value;

    copyListItems(ddDashboards, lstItems);

    divChart.style.display = "none";
    dlgDashBoard.style.display = "";
    setEnabled(btnSave, false);
    txtSearch.focus();

}

function txtSearchKeyDown() {
    var e=event
    var txtSearch = document.getElementById("txtSearch");
    var key = e.charCode ? e.charCode : e.keyCode;
    if (txtSearch != null) {
        if (key == 13) {
            DoSearch("txtSearch");
            e.preventDefault(); // prevents return key from triggering submit button
        }
    }
}

function btnSubmitClicked() {
    var lstItems = document.getElementById("lstItems");
    var selected = getSelectedString(lstItems);

    if (selected != "") {
        closeDialog()
        var target = "AddDashboards~" + chartType;
        __doPostBack(target, selected)
    }
}

function ChecklistIndexChanged(e) {
    var ctlID = e.currentTarget.id;
    var ckList = document.getElementById(ctlID);
    var btnSave = document.getElementById("btnSubmit");
    var arCheckboxes = ckList.getElementsByTagName("input");

    setEnabled(btnSave, false);
    if (ckList != null && arCheckboxes != null && arCheckboxes.length > 0) {
        for (var i = 0; i < arCheckboxes.length; i++) {
            var ckbx = arCheckboxes[i];
            if (ckbx.checked) {
                setEnabled(btnSave, true);
                break;
            }
        }
    }
}

function showSpinner() {

    setTimeout(function () { document.getElementById("spinner").style.display = ""; document.images["imgSpinner"].src = "Controls/Images/WaitImage2.gif"; }, 200);
}
function DoSearch(textBoxID) {
    var txtBox = document.getElementById(textBoxID);
    var ddDashboards = document.getElementById("ddDashboards_Checklist");
    var lstItems = document.getElementById("lstItems");
    var txtSearch = document.getElementById("txtSearch");
    var btnSave = document.getElementById("btnSubmit");

    if (txtBox != null && txtBox.value.length > 0) {
        var searchFor = txtBox.value;
        var arListItems = [];
        var arSearch = searchFor.split(",");
        var bExactMatch = true;
        var bNoMatch = false;
        var vals = "";
        var n = arSearch.length;

        for (var i = 0; i < n; i++) {
            var str = arSearch[i].trim();
            var arItems = searchChecklist(ddDashboards, str);
            if (!isExactMatch && bExactMatch)
                bExactMatch = false;
            if (arItems.length > 0)
                vals = vals == "" ? str : vals + "," + str;
            if (arItems.length == 0) {
                var li = new ListItem();
                li.text = str;
                li.selected = false;
                arItems[0] = li;
                bNoMatch = true;
                bExactMatch = false;
                if (n == 1) {
                    var ckBox = addToList(lstItems, str, str)
                    if (ckBox != null) ckBox.checked = false;
                    txtSearch.value = "";
                    vals = "";
                    txtBox.focus();
                    continue;
                }
            }
            addToArray(arListItems, arItems);
        }
        if (textBoxID == "txtSearch") {
            if (arListItems.length > 0) {
                    txtSearch.value = "";
                    vals = "";
                    loadListItems(lstItems, arListItems);
                    txtBox.focus();
                    setEnabled(btnSave, true);
            }
        }
    }
    else if (textBoxID == "txtSearch") {
        copyListItems(ddDashboards, lstItems);
    }
}

function searchChecklist(list, searchFor) {

    var rowcount = 0;
    if (list != null) rowcount = list.rows.length;

    var arHits = [];
    var sSearch = searchFor.toUpperCase();
    var bContains = sSearch.indexOf("[") == 0 ? true : false;

    sSearch = !bContains ? sSearch : sSearch.substring(1);
    isExactMatch = true;

    if (rowcount > 0) {
        for (var i = 0; i < rowcount; i++) {
            var row = list.rows[i];
            var lbl = row.getElementsByTagName("label")[0];
            var ckbox = row.getElementsByTagName("input")[0];
            var sCheck = lbl.innerText.toUpperCase();
            if (sCheck == sSearch || (bContains && sCheck.indexOf(sSearch) != 0)) {
                if (sCheck != sSearch && isExactMatch)
                    isExactMatch = false;
                var li = new ListItem();
                li.text = lbl.innerText;
                li.value = ckbox.value;
                li.selected = true; //ckbox.checked;
                arHits.unshift(li);
            }
        }
    }
    return arHits;
}

function addToArray(arTo, arFrom) {

    if (arFrom != null && arFrom.length > 0) {
        var l = arTo.length;

        for (var i = 0; i < arFrom.length; i++) {
            arTo[l] = arFrom[i];
            l++;
        }
    }
}

function loadListItems(list, listitems) {
    if (list != null && listitems != null && listitems.length > 0) {
        clearChecklist(list);
        for (var i = 0; i < listitems.length; i++) {
            li = listitems[i];
            var ckBox = addToList(list, li.text, li.value);
            if (ckBox != null)
                ckBox.checked = li.selected;
        }
    }

}

function closeDialog() {
    var divPopupDisplay = document.getElementById("divPopupBackground");
    var divChart = document.getElementById("chart_div");

    divPopupDisplay.style.display = "none";
    divChart.style.display = "";
}

function setEnabled(elem, enable) {
    if (elem != null) {
        if (enable && elem.disabled) {
            elem.disabled = false;
            elem.style.color = "black";
        }
        else if (!enable && !elem.disabled) {
            elem.disabled = true;
            elem.style.color = "gray";
        }
    }
}

function copyListItems(copyFrom, copyTo) {

    clearChecklist(copyTo);
    if (copyFrom != null && copyTo != null)
    {
        var arCheckBoxes = copyFrom.getElementsByTagName("input");
        var arLabels = copyFrom.getElementsByTagName("label");
        if (arCheckBoxes.length > 0) {
            for (var i = 0; i < arCheckBoxes.length; i++) {
                if (arLabels[i].innerText.toUpperCase() != "ALL")
                    addToList(copyTo, arLabels[i].innerText, arCheckBoxes[i].value,arCheckBoxes[i].disabled);
            }
        }
        else if (defaultDashboardAvailable=="true") {
            addToList(copyTo, defaultDashboard, defaultDashboard);
        }
    }
    else if (copyTo != null && defaultDashboardAvailable == "true") {
        addToList(copyTo, defaultDashboard, defaultDashboard);
    }
     
}

function clearChecklist(list) {
    if (list != null) {
        var rowcount = list.rows.length;
        if (rowcount > 0) {
            for (var i = rowcount - 1; i > -1; i--) {
                list.deleteRow(i);
            }
        }
    }

}

function addToList(list, text, value,isDisabled) {
    var row = list.insertRow();
    var cell = row.insertCell();
    var checkBox = document.createElement("input");
    var label = document.createElement("label");

    if (isDisabled == void 0) isDisabled = false

    checkBox.type = "checkbox";
    label.innerText = text;
    checkBox.value = value;

    checkBox.disabled = isDisabled;
    cell.appendChild(checkBox);
    cell.appendChild(label);
    return checkBox;
}
function getSelectedString(list) {
    var selected = "";
    if (list != null) {
        var rowcount = list.rows.length;
        if (rowcount > 0) {
            for (var i = 0; i < rowcount; i++) {
                var row = list.rows[i];
                var lbl = row.getElementsByTagName("label")[0];
                var ckbox = row.getElementsByTagName("input")[0];
                if (ckbox.checked) {
                    selected = selected == "" ? lbl.innerText : selected + "," + lbl.innerText;
                }
            }
        }
    }

    return selected;
}