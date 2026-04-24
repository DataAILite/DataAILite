
function showIPFilterDialog() {
    var ipDlg = document.getElementById("divIPFilterDlgBackground");

    ipDlg.style.display = "";
    loadIPFilters();
}

function clearIPFilters() {
    var tblIPFilters = document.getElementById("tblIPFilters");
    rowCount = tblIPFilters.rows.length;
    if (rowCount > 1) {
        for (var i = rowCount-1; i >0; i--) {
            tblIPFilters.deleteRow(i);
        }
    }
}
function loadIPFilters() {
    var tblIPFilters = document.getElementById("tblIPFilters");
    var rowCount;
    var row;
    var cellCond;
    var cellEdit;
    var cellDelete;
    var cellFilter;
    var button;
    var hdnIPFilters = document.getElementById("hdnIPFilters");
    var sIPFilters = hdnIPFilters.value;
    var parts;
    var part;
    var data;
    var filterID;
    var condition;
    var filter;

    clearIPFilters();
    tblIPFilters.setAttribute("data-selectedrow", "-1");
    if (sIPFilters != void 0 && sIPFilters != "") {
        parts = sIPFilters.split(",")
        if (parts.length > 0) {
            for (var i = 0; i < parts.length; i++) {
                part = parts[i];
                data = part.split(":")
                if (data.length == 3) {
                    filterID = data[0];
                    condition = data[1];
                    filter = data[2];
                    rowCount = tblIPFilters.rows.length;
                    row = tblIPFilters.insertRow(rowCount)
                    row.setAttribute("data-filterdata", part);
                    row.setAttribute("data-rownum", rowCount);

                   cellCond = row.insertCell(0);
                   cellCond.innerHTML = condition;

                    cellFilter= row.insertCell(1);
                    cellFilter.innerHTML = filter;

                    cellEdit = row.insertCell(2);
                    button = document.createElement("input");
                    button.type = "button";
                    button.name = "btnEdit" + (rowCount);
                    button.setAttribute('value', 'Edit');
                    button.addEventListener("click", editFilter);
                    button.className = "dlgboxbutton";
                    cellEdit.appendChild(button);

                    cellDelete = row.insertCell(3);
                    button = document.createElement("input");
                    button.type = "button";
                    button.name = "btnDelete" + (rowCount);
                    button.setAttribute('value', 'Delete');
                    button.addEventListener("click", deleteFilter);
                    button.className = "dlgboxbutton";
                    cellDelete.appendChild(button);
                }
            }
        }
    }
}
function reIndexTable() {
    var tblIPFilters = document.getElementById("tblIPFilters");
    var rowCount = tblIPFilters.rows.length;
    if (rowCount > 1) {
        for (var i = 1; i < rowCount; i++) {
            var row = tblIPFilters.rows[i];
            row.dataset.rownum = i;
        }
    }
}


function editFilter(e) {
    if (e != void 0) {
        var button = e.target;
        var row = button.parentNode.parentNode;
        var rownum = row.dataset.rownum;
        var filterData = row.dataset.filterdata;
        var data = filterData.split(":");
        var condition = data[1];
        var filter = data[2];
        var tblIPFilters = document.getElementById("tblIPFilters");
        var addDlg = document.getElementById("divAddDlgBackground");
        var ddConditions = document.getElementById("conditions");
        var addDlgHeading = document.getElementById("lblAddFilterHeading");
        var btnAddOK = document.getElementById("btnAddDlgOK");
        var txtIPFilter = document.getElementById("txtIPFilter");

        tblIPFilters.dataset.selectedrow = rownum;
        txtIPFilter.value = filter;
        findOption("conditions",condition)
        addDlg.style.display = "";
        btnAddOK.setAttribute("data-action", "edit");
        addDlgHeading.textContent = "Edit Contact Email Filter";
        txtIPFilter.style.color = "Black";
        ddConditions.focus();
    }
}

function deleteFilter(e) {
    if (e != void 0) {
        var button = e.target;
        var row = button.parentNode.parentNode;
        var rownum =row.dataset.rownum;
        var tblIPFilters = document.getElementById("tblIPFilters");

        tblIPFilters.deleteRow(rownum);
        reIndexTable();
        tblIPFilters.dataset.selectedrow = "-1";

    }
}
function findOption(dropdown, strToFind) {
    if (dropdown != void 0 && strToFind != void 0) {
        var dd = document.getElementById(dropdown);
        var count = dd.options.length;
        for (var i = 0; i < count; i++) {
            if (dd.options[i].value == strToFind) {
                dd.options[i].selected = true;
                return;
            }
        }
    }
}

function addOKClick(e) {
    if (e != void 0) {
        //var addDlg = document.getElementById("divAddDlgBackground");
        var ddConditions = document.getElementById("conditions");
        var txtIPFilter = document.getElementById("txtIPFilter");
        var tblIPFilters = document.getElementById("tblIPFilters");
        var ipDlg = document.getElementById("divIPFilterDlgBackground");

        //var rowCount;
        var condition = ddConditions.options[ddConditions.selectedIndex].value;
        var selectedRow = tblIPFilters.dataset.selectedrow;
        var ipFilter = txtIPFilter.value;
        var filterID = "";
        var filterData;
        var btnOK = e.target;
        var bOK = true;
        var regex;
        var row;
        var data;

        if (selectedRow > -1) {
            row = tblIPFilters.rows[selectedRow];
            filterData = row.dataset.filterdata;
            data = filterData.split(":");
            filterID = data[0];
        }

        if (txtIPFilter.value == "") {
            txtIPFilter.value = "Please enter an IP Filter value!"
            txtIPFilter.style.color = "Red";
            txtIPFilter.setSelectionRange(0, 0);
            txtIPFilter.focus();
            bOK = false;
        }
        else if (condition == "equals") {
        
            regex = new RegExp(/^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/)
            bOK = regex.test(txtIPFilter.value);
                
           if (!bOK) {
               txtIPFilter.value = "Please enter a valid IP address!"
               txtIPFilter.style.color = "Red";
               txtIPFilter.setSelectionRange(0, 0);
               txtIPFilter.focus();
           }
        }
        else if (condition == "startswith") {
            bOK=false;

            regex = new RegExp(/^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.?$/);
            bOK = regex.test(txtIPFilter.value);

            if (!bOK) {
                regex = new RegExp(/^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){1}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.?$/);
                bOK = regex.test(txtIPFilter.value);
            }
            if (!bOK) {
                var regex = new RegExp(new RegExp(/^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){2}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.?$/));
                bOK = regex.test(txtIPFilter.value);
            }

            if (!bOK) {
                regex = new RegExp(/^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/)
                bOK = regex.test(txtIPFilter.value);
            }

            if (!bOK) {
                txtIPFilter.value = "IP Filter format is not correct!"
                txtIPFilter.style.color = "Red";
                txtIPFilter.setSelectionRange(0, 0);
                txtIPFilter.focus();
            }
        }

        if (bOK) {
            condition = ddConditions.options[ddConditions.selectedIndex].value;
            ipFilter = txtIPFilter.value;
            filterData = filterID + ":" + condition + ":" + ipFilter; // filterID is blank if adding

            if (btnOK.dataset.action == "add") {
                 if (!filterExists(condition, ipFilter)) {
                    addRow(condition, ipFilter, filterData);
                    closeDialog("divAddDlgBackground");
                }
                else {
                    txtIPFilter.value = "Entered IP Filter already exists!"
                    txtIPFilter.style.color = "Red";
                    txtIPFilter.setSelectionRange(0, 0);
                    txtIPFilter.focus();
                }
            }
            else if (btnOK.dataset.action == "edit") {
                if (filterDifferent(selectedRow,filterData)) {
                    editRow(condition, ipFilter, filterData);
                }
                closeDialog("divAddDlgBackground");
            }
        }

    }

}

function filterExists(condition, ipFilter) {
    var tblIPFilters = document.getElementById("tblIPFilters");
    var rowCount = tblIPFilters.rows.length;
    
    if (rowCount > 1) {
        for (var i = 1; i <rowCount; i++) {
            if (tblIPFilters.rows[i].cells[0].textContent == condition && tblIPFilters.rows[i].cells[1].textContent == ipFilter)
                return true;
        }
    }
    return false;
}

function filterDifferent(selectedRow,filterData) {
    if (selectedRow != "-1") {
        var tblIPFilters = document.getElementById("tblIPFilters");
        var row = tblIPFilters.rows[selectedRow];
        if (filterData != row.dataset.filterdata)
            return true;
    }
    return false;
}
function editRow(condition,ipFilter,filterData) {
    var tblIPFilters = document.getElementById("tblIPFilters");
    var selected = tblIPFilters.dataset.selectedrow;
    var row = tblIPFilters.rows[selected];
    
    row.dataset.filterdata = filterData;
    row.cells[0].innerHTML = condition;
    row.cells[1].innerHTML = ipFilter;
}
function addRow(condition, ipFilter, filterData) {
    var tblIPFilters = document.getElementById("tblIPFilters");
    var rowCount = tblIPFilters.rows.length;
    var row = tblIPFilters.insertRow(rowCount);
    var cellCond;
    var cellFilter;
    var cellEdit;
    var cellDelete;
    var button;

    row.setAttribute("data-filterdata", filterData);
    row.setAttribute("data-rownum", rowCount);

    cellCond = row.insertCell(0);
    cellCond.innerHTML = condition;

    cellFilter = row.insertCell(1);
    cellFilter.innerHTML = ipFilter;

    cellEdit = row.insertCell(2);
    button = document.createElement("input");
    button.type = "button";
    button.name = "btnEdit" + (rowCount);
    button.setAttribute('value', 'Edit');
    button.addEventListener("click", editFilter);
    button.className = "dlgboxbutton";
    cellEdit.appendChild(button);

    cellDelete = row.insertCell(3);
    button = document.createElement("input");
    button.type = "button";
    button.name = "btnDelete" + (rowCount);
    button.setAttribute('value', 'Delete');
    button.addEventListener("click", deleteFilter);
    button.className = "dlgboxbutton";
    cellDelete.appendChild(button);
}

function blockSubmit(e) {
    var key = e.charCode || e.keyCode || 0;
    if (key == 13) {
        e.preventDefault();
    }
}
function eraseMessage(txtBox) {
    var tb = document.getElementById(txtBox)
    var key = event.charCode || event.keyCode || 0
    if (tb != void 0 && tb != 'undefined') {
        var color = tb.style.color.toUpperCase();
        if (color == "RED") {
            tb.style.color = "Black";
            tb.value = "";
        }
    }
}

function addFilter(e) {
    if (e != void 0) {
        var addDlg = document.getElementById("divAddDlgBackground");
        var ddConditions = document.getElementById("conditions");
        var addDlgHeading = document.getElementById("lblAddFilterHeading");
        var btnAddOK = document.getElementById("btnAddDlgOK");
        var txtIPFilter = document.getElementById("txtIPFilter");
        var tblIPFilters = document.getElementById("tblIPFilters");

        addDlg.style.display = "";
        btnAddOK.setAttribute("data-action", "add");
        addDlgHeading.textContent = "Add Contact Email Filter";
        txtIPFilter.style.color = "Black";
        txtIPFilter.value = "";
        tblIPFilters.dataset.selectedrow = "-1";
        ddConditions.selectedIndex = 0;
        ddConditions.focus();
    }
}

function saveFilters() {
    var tblIPFilters = document.getElementById("tblIPFilters");
    var rowCount = tblIPFilters.rows.length;
    var sFilters = "";
    var row;
    var data;

    if (rowCount > 1) {
        for (var i = 1; i < rowCount; i++) {
            row = tblIPFilters.rows[i];
            data = row.dataset.filterdata;
            sFilters = (i == 1) ? data : sFilters + "," + data;
        }
    }
    closeDialog("divIPFilterDlgBackground");
  __doPostBack("SaveFilters", sFilters);
}

function closeDialog(dialog) {
    if (dialog != void 0) {
        var divDialog = document.getElementById(dialog);
        if (divDialog != void 0) divDialog.style.display = "none";
    }
}
