var imgHasFocus = false;

function OnDropDownTablesChange(e) {
    var list = document.getElementById("DropDownTables");

    if (list != void 0) {
        var selectedIndex = list.selectedIndex;
        var val = list.options[selectedIndex].value;
        var data = val + "." + selectedIndex;
        showSpinner();
        __doPostBack("GetColumns",data);
    }
}

function onImageButtonFocus(event) {
    var divSearch = document.getElementById("divSearch");
    divSearch.style.backgroundColor = "palegoldenrod";
    imgHasFocus = true;
}

function onImageButtonBlur(event) {
    var divSearch = document.getElementById("divSearch");
    divSearch.style.backgroundColor = "white";
    imgHasFocus = false;
}

function hoverState(InorOut) {
    var divSearch = document.getElementById("divSearch");
    var imgSearch = document.getElementById("imgSearch");

     if (!imgHasFocus) {
        if (InorOut == "In")
            divSearch.style.backgroundColor = "#f8f8d3";
        else
            divSearch.style.backgroundColor = "white";
    }
}

function pressState(UporDown) {
    var e = event || window.event;
    var divSearch = document.getElementById("divSearch");
    if (e != void 0 && e.button == "0") {
        if (UporDown == "Down")
            divSearch.style.backgroundColor = "lightgray";
        else
            divSearch.style.backgroundColor = "palegoldenrod";
    }
}

function clearListbox(lb) {
    if (lb != void 0 && lb != undefined) {
        for (var i = lb.options.length - 1; i >= 0; i--) {
            lb.remove(i);
        }
    }
}
function loadListbox(lb, ar) {
    var n = ar.length;

    if (n > 0) {
        if (lb != void 0 && lb != undefined) {
            var parts;
            var opt;
            var nSelected = 0;

            clearListbox(lb);
            for (var i = 0; i < n; i++) {
                parts = ar[i].split("~");
                var opt = document.createElement("OPTION");
                opt.value =parts[1];
                opt.text = parts[0];
                if (TableInReport(parts[1])) nSelected = i;
                //if (i == 0) opt.selected = true;
                lb.add(opt);
            }
            lb.selectedIndex = nSelected;
        }
    }
    

}
function TableInReport(tbl) {
    var arReportTables = document.getElementById("hdnReportTables").value.split(",");
    var n = arReportTables.length;

    for (var i = 0; i < n; i++) {
        if (arReportTables[i] == tbl)
            return true;
    }
    return false;
}

function DoSearch(strSearch) {
    var list = document.getElementById("DropDownTables");
    var hdnAllTables = document.getElementById("hdnAllTables");
    var arTables = hdnAllTables.value.split(",");
    var n = arTables.length;

    if (strSearch != '') {
        if (list != void 0 && list != undefined) {
            var matches = 0;
            var arMatches = [];
            var tblData;

            if (n > 0) {
                for (var i = 0; i < n; i++) {
                      tblData = arTables[i];
                      if (tblData.toUpperCase().includes(strSearch.toUpperCase())) {
                          arMatches[matches] = tblData;
                          matches++;
                     }
                }
                tblData = "Not Found";
                if (arMatches.length > 0) {
                    tblData = arMatches.toString();
                }
                showSpinner();
                __doPostBack("LoadTables", tblData);
            }
        }
    }
    else if (list != void 0 && list != undefined && n != list.options.length)  {
        showSpinner();
        __doPostBack("LoadTables", "All");
    }

}

function OnSelectFieldsClick(e) {

    showSpinner();
    __doPostBack("SelectFields", "Data");
}

function OnFieldDeleteClick(e) {
    var btnField = e.target;

    if (btnField != void 0) {
        var ret = btnField.id.split("^");
        showSpinner();
        __doPostBack("FieldDelete", ret[1]);
    }
}

function OnSearchButtonClick(e) {
    var txtSearch = document.getElementById("txtSearch");
    var value = txtSearch.value.trim();
    var list = document.getElementById("DropDownTables");
    var hdnAllTables = document.getElementById("hdnAllTables");
    var hdnSearchText = document.getElementById("hdnSearchText");
    var OldSearchText = hdnSearchText.value;
     var arTables = hdnAllTables.value.split(",");
     var n = arTables.length;

     if (txtSearch != void 0 && OldSearchText != value) {
         if (n == list.options.length && value == "")
             return false;
         hdnSearchText.value = value;
         DoSearch(value);
    }
}

function OnSearchTextChanged(e) {
    var txtSearch = e.currentTarget;
    if (txtSearch != void 0) {
        var imgSearch = document.getElementById("imgSearch");

        imgSearch.focus();
        imgSearch.click();
    }
}

function showSpinner() {

    setTimeout(function () { document.getElementById("spinner").style.display = ""; document.images["imgSpinner"].src = "Controls/Images/WaitImage2.gif"; }, 200);
}

function OnSearchTextKeyDown(e) {
    var keyCode = e.keyCode;
   
    if (keyCode == 13) {
        OnSearchButtonClick(event);
    }
}


function OnChangeChecklistDropDown(e) {
    var chklist = e.currentTarget;
    var allChecked = (chklist.dataset.allchecked == "true") ? true : false;
    var noneChecked = (chklist.dataset.nonechecked == "true") ? true : false;
    var btnSelectAll = document.getElementById("btnSelectAll");
    var btnUnselectAll = document.getElementById("btnUnselectAll");

    if (allChecked) {
        btnSelectAll.disabled = true;
        btnSelectAll.className = "DataButtonDisabled";
        btnUnselectAll.disabled = false;
        btnUnselectAll.className = "DataButtonEnabled";
    }
    else if (noneChecked) {
        btnSelectAll.disabled = false;
        btnSelectAll.className = "DataButtonEnabled";
        btnUnselectAll.disabled = true;
        btnUnselectAll.className = "DataButtonDisabled";
    }
    else {
        btnSelectAll.disabled = false;
        btnSelectAll.className = "DataButtonEnabled";
        btnUnselectAll.disabled = false;
        btnUnselectAll.className = "DataButtonEnabled";
    }
}
function selectAll(cklistID) {
    var chklist = document.getElementById(cklistID + "_Checklist");
    var arCheckBoxes = chklist.getElementsByTagName("input");
    var txtValue = document.getElementById(cklistID + "_txtValue");
    var hdnSelectedData = document.getElementById(cklistID + "_hdnSelectedData");
    var arLabels = chklist.getElementsByTagName("label");
    var strDisplay = "";
    //var evt = document.createEvent("HTMLEvents");
    //evt.initEvent("change", true, true);


    for (var i = 0; i < arCheckBoxes.length; i++) {
        arCheckBoxes[i].checked = true;
        strDisplay = (strDisplay == "") ? arLabels[i].innerText : strDisplay + "," + arLabels[i].innerText;
    }

    txtValue.value = "ALL";
    txtValue.title = strDisplay;

    chklist.setAttribute("data-itemdescription", "All Checked");
    chklist.setAttribute("data-itemchecked", "true");
    chklist.setAttribute("data-allchecked", "true");
    chklist.setAttribute("data-nonechecked", "false");

    hdnSelectedData.value = "All Checked~true~true~false"
    
    //chklist.dispatchEvent(evt);
}
function unSelectAll(cklistID) {
    var chklist = document.getElementById(cklistID + "_Checklist");
    var arCheckBoxes = chklist.getElementsByTagName("input");
    var txtValue = document.getElementById(cklistID + "_txtValue");
    var hdnSelectedData = document.getElementById(cklistID + "_hdnSelectedData");
    //var evt = document.createEvent("HTMLEvents");
    //evt.initEvent("change", true, true);

    for (var i = 0; i < arCheckBoxes.length; i++) {
        arCheckBoxes[i].checked = false;
    }

    txtValue.value = "Please select...";
    txtValue.title = "";

    chklist.setAttribute("data-itemdescription", "All Unchecked");
    chklist.setAttribute("data-itemchecked", "false");
    chklist.setAttribute("data-allchecked", "false");
    chklist.setAttribute("data-nonechecked", "true");
    hdnSelectedData.value = "All Unchecked~false~false~true"
    //chklist.dispatchEvent(evt);

}