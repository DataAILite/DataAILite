function ListItem() {
    this.listItem = {
    text:"",
    value:"",
    selected:false
    }
    return this.listItem;
}
var isExactMatch;

function handleStatusChange(ctlPrefix) {
    var ddStatus = document.getElementById(ctlPrefix + "ddStatus");
    var txtOtherStatus = document.getElementById(ctlPrefix + "txtOtherStatus")
    var txtComments = document.getElementById(ctlPrefix + "txtComments");
    var divOtherStatus = document.getElementById(ctlPrefix + "divOtherStatus");
    var status = ddStatus.options[ddStatus.selectedIndex].value;
    if (status != null) {
        if (status == "other") {
            divOtherStatus.style.display = "";
            txtOtherStatus.value = "";
            txtOtherStatus.focus();
        }
        else {
            divOtherStatus.style.display = "none";
            txtComments.focus();
        }
    }
}
function clickFileUpload(ctlPrefix) {
    var fileUpload = document.getElementById(ctlPrefix + "FileUpload");
    if (fileUpload != null) fileUpload.click();
}

function getAttachedFile(ctlPrefix) {
    var fileUpload = document.getElementById(ctlPrefix + "FileUpload");
    if (fileUpload != null) {
        var lblAttached = document.getElementById(ctlPrefix + "lblAttached");
        lblAttached.innerText = fileUpload.files[0].name;
    }
}
// *************************************** Search Popup *********************************
function addToList(list, text, value) {
    var row = list.insertRow();
    var cell = row.insertCell();
    var checkBox = document.createElement("input");
    var label = document.createElement("label");

    checkBox.type = "checkbox";
    label.innerText = text;
    checkBox.value = value;

    cell.appendChild(checkBox);
    cell.appendChild(label);
    return checkBox;
}
function clearChecklist(list) {
    var rowcount = list.rows.length;
    if (rowcount > 0) {
        for (var i = rowcount - 1; i > -1; i--) {
            list.deleteRow(i);
        }
    }
}
function checkSelectedItems(list, selected) {
    if (selected != void 0 && list != void 0) {
        var arSelected = selected.split(",");
        var rowcount = list.rows.length;
        if (rowcount > 0) {
            for (var i = 0; i < rowcount; i++) {
                var row = list.rows[i];
                var lbl = row.getElementsByTagName("label")[0];
                var ckbox = row.getElementsByTagName("input")[0];
                for (var ii = 0; ii < arSelected.length; ii++) {
                    if (arSelected[ii].toUpperCase() == lbl.innerText.toUpperCase()) {
                        ckbox.checked = true;
                    }
                }
            }
        }
    }
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
function searchChecklist(list,searchFor) {
    var rowcount = list.rows.length;
    var arHits = [];
    var sSearch = searchFor.toUpperCase();
    isExactMatch = true;
    if (rowcount > 0) {
        for (var i = 0; i < rowcount; i++) {
            var row = list.rows[i];
            var lbl = row.getElementsByTagName("label")[0];
            var ckbox = row.getElementsByTagName("input")[0];
            var sCheck = lbl.innerText.toUpperCase();
            if (sCheck == sSearch || sCheck.indexOf(sSearch) == 0) {
                if (sCheck != sSearch && isExactMatch)
                    isExactMatch = false;
                var li = new ListItem();
                li.text = lbl.innerText;
                li.value = ckbox.value;
                li.selected = ckbox.checked;
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

function btnSubmitClicked(ctlPrefix) {
    var lstItems = document.getElementById(ctlPrefix + "lstItems");
    var txtLookup = document.getElementById(ctlPrefix + "txtLookup");
    var btnBrowse = document.getElementById(ctlPrefix + "btnBrowse");
    var selected = getSelectedString(lstItems);
    
    if (selected != "") {
        closePopup(ctlPrefix);
        txtLookup.value = selected;
        btnBrowse.focus();
    }
}

function DoSearch(ctlPrefix, textBoxID) {
    var txtBox = document.getElementById(ctlPrefix + textBoxID);
    var ddSendTo = document.getElementById(ctlPrefix + "ddSendTo_Checklist");
    var lstItems = document.getElementById(ctlPrefix + "lstItems");
    var txtLookup = document.getElementById(ctlPrefix + "txtLookup");
    var txtSearch = document.getElementById(ctlPrefix + "txtSearch");
    var btnSave = document.getElementById(ctlPrefix + "btnSubmit");
    var btnBrowse = document.getElementById(ctlPrefix + "btnBrowse");

    if (txtBox != null && txtBox.value.length > 0) {
        var searchFor = txtBox.value;
        var arListItems = [];
        var arSearch = searchFor.split(",");
        var bExactMatch = true;
        var bNoMatch = false;
        var vals = "";

        for (var i = 0; i < arSearch.length;i++) {
            var str = arSearch[i].trim();
            var arItems = searchChecklist(ddSendTo, str);
            if (!isExactMatch && bExactMatch)
                bExactMatch = false;
            if (arItems.length > 0)
                vals = vals == "" ? str : vals + "," + str;
            if (arItems.length == 0) {
                var li = new ListItem();
                li.text = str;
                li.selected = true;
                arItems[0] = li;
                bNoMatch = true;
            }
            addToArray(arListItems, arItems);
        }
        if (textBoxID == "txtSearch") {
            if (arListItems.length > 1) {
                if (!bExactMatch) {
                    txtSearch.value = vals;
                    vals = "";
                    loadListItems(lstItems, arListItems);
                    txtBox.focus();
                }
                else {
                    vals=""
                    for (var i = 0; i < arListItems.length; i++) {
                        vals = vals == "" ? arListItems[i].text : vals + "," + arListItems[i].text;
                    }
                    txtLookup.value = vals;
                    closePopup(ctlPrefix);
                    //txtLookup.value = txtBox.value;
                    btnBrowse.focus();
                }
            }
            else if (arListItems.length == 1) {
                closePopup(ctlPrefix);
                txtLookup.value = arListItems[0].text;
                btnBrowse.focus();
            }
        }
        else if (textBoxID == "txtLookup") {
            if (arListItems.length > 1) {
                if (!bExactMatch) {
                    showPopup(ctlPrefix);
                    loadListItems(lstItems, arListItems);
                    txtSearch.focus();
                    txtSearch.value = vals;
                    vals = "";
                    //txtSearch.value = txtBox.value;
                }
                else {
                    vals = "";
                    for (var i = 0; i < arListItems.length; i++) {
                        vals = vals == "" ? arListItems[i].text : vals + "," + arListItems[i].text;
                    }
                    txtLookup.value = vals;
                    btnBrowse.focus();
                }
            }
            else if (arListItems.length == 1) {
                txtLookup.value = arListItems[0].text;
                btnBrowse.focus();
            }
            else {
                txtLookup.value = "";
                txtLookup.focus();
            }
        }
    }
    else if (textBoxID == "txtSearch") {
        copyListItems(ddSendTo, lstItems);
    }
    if (bNoMatch)
        setEnabled(btnSave, true);
    else
        setEnabled(btnSave, false);
}

function copyListItems(copyFrom, copyTo) {
    if (copyFrom != null && copyTo != null) {
        clearChecklist(copyTo);
        var arCheckBoxes = copyFrom.getElementsByTagName("input");
        var arLabels = copyFrom.getElementsByTagName("label");
        if (arCheckBoxes.length > 0) {
            for (var i = 0; i < arCheckBoxes.length; i++) {
                if (arLabels[i].innerText.toUpperCase() != "ALL")
                    addToList(copyTo, arLabels[i].innerText, arCheckBoxes[i].value);
            }
        }
    }
}

function loadListItems(list, listitems) {
    if (list != null && listitems != null && listitems.length > 1) {
        clearChecklist(list);
        for (var i = 0; i < listitems.length; i++) {
            li = listitems[i];
            var ckBox = addToList(list, li.text, li.value);
            if (ckBox != null)
                ckBox.checked = li.selected;
        }
    }

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
function txtSearchChanged(e,ctlPrefix) {
    var ctlID = e.currentTarget.id;
    var txtSearch = document.getElementById(ctlID);
    if (txtSearch != null) {
        var btnFind = document.getElementById(ctlPrefix + "btnFind");
        var btnClose = document.getElementById(ctlPrefix + "btnClose");

        if (txtSearch.value != "") {
            btnFind.focus();
        }
        else {
            btnClose.focus();
        }
    }

}
function isAllTextSelected(input) {
    var startPos = input.selectionStart;
    var endPos = input.selectionEnd;
    var doc = document.selection;

    if (doc && doc.createRange().text.length == input.value.length) {
        return true;
    } else if (!doc && input.value.substring(startPos, endPos).length == input.value.length) {
        return true;
    }
    return false;
}
function isPrintable(keyCode) {
    if (keyCode == 32 ||
        (keyCode > 47 && keyCode < 91) ||
        (keyCode > 95 && keyCode < 112) ||
        (keyCode > 159 && keyCode < 177) ||
        (keyCode > 185 && keyCode < 193)) {
        return true;
    }
    return false;
}
function txtLookupKeyDown(e, ctlPrefix) {
    var ctlID = e.currentTarget.id;
    var txtLookup = document.getElementById(ctlID);
    var key = e.charCode ? e.charCode : e.keyCode;

    if (txtLookup != null && key==13) {
        DoSearch(ctlPrefix, "txtLookup");
        e.preventDefault();
    }
}
function txtSearchKeyDown(e, ctlPrefix) {
    var ctlID = e.currentTarget.id;
    var txtSearch = document.getElementById(ctlID);
    var key = e.charCode ? e.charCode : e.keyCode;
    if (txtSearch != null) {
        //var btnFind = document.getElementById(ctlPrefix + "btnFind");
        //if (((key == 8 || key == 46) && txtSearch.value.length < 2) || (key == 46 && isAllTextSelected(txtSearch)))
        //    setEnabled(btnFind, false);
        //else if (key == 13) {

        //}
        //else if (isPrintable(key))
        //    setEnabled(btnFind, true);
        if (key == 13) {
            DoSearch(ctlPrefix, "txtSearch");
            e.preventDefault(); // prevents return key from triggering submit button
        }
    }
}
function ChecklistIndexChanged(e, ctlPrefix) {
    var ctlID = e.currentTarget.id;
    var ckList = document.getElementById(ctlID);
    var btnSave = document.getElementById(ctlPrefix + "btnSubmit");
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

function showAll(ctlPrefix) {
    var txtSearch = document.getElementById(ctlPrefix + "txtSearch");
    var txtLookup = document.getElementById(ctlPrefix + "txtLookup");
    var ddSendTo = document.getElementById(ctlPrefix + "ddSendTo_Checklist");
    var lstItems = document.getElementById(ctlPrefix + "lstItems");
    var btnSave = document.getElementById(ctlPrefix + "btnSubmit");
    var lookupString = txtLookup.value;

    showPopup(ctlPrefix)
    copyListItems(ddSendTo, lstItems);
    if (lookupString != void 0) {
        // parse lookupString and check applicable check boxes
        checkSelectedItems(lstItems, lookupString)
    }
    txtSearch.value = "";
    txtSearch.focus();
    setEnabled(btnSave, false);
    event.stopPropagation();

}
function showPopup(ctlPrefix) {
    var pnlBody = document.getElementById(ctlPrefix + "pnlBody");
    var divPopup = document.getElementById(ctlPrefix + "divPopup");
    var divPopupDisplay = document.getElementById(ctlPrefix + "divPopupBackground");
    //var txtSearch = document.getElementById(ctlPrefix + "txtSearch");
    //var ddSendTo = document.getElementById(ctlPrefix + "ddSendTo_Checklist");
    //var lstItems = document.getElementById(ctlPrefix + "lstItems");
    //var btnSave = document.getElementById(ctlPrefix + "btnSubmit");

    divPopupDisplay.style.display = "";
    var rectBody = pnlBody.getBoundingClientRect();
    var rectPopup = divPopup.getBoundingClientRect();
    var x = (rectBody.width - rectPopup.width) / 2;
    var y = (rectBody.height - rectPopup.height) / 2;

    divPopup.style.left = parseInt(x) + "px";
    divPopup.style.top = parseInt(y) + "px";
    //event.stopPropagation();
    //copyListItems(ddSendTo, lstItems);
    //txtSearch.value = "";
    //txtSearch.focus();
    //setEnabled(btnSave, false);

}
function btnCloseClicked(ctlPrefix) {
    var txtLookup = document.getElementById(ctlPrefix + "txtLookup");
    closePopup(ctlPrefix);
    txtLookup.focus();
}
function closePopup(ctlPrefix) {
    var divPopupDisplay = document.getElementById(ctlPrefix + "divPopupBackground");
    divPopupDisplay.style.display = "none";
}