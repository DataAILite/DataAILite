function showSpinner() {

    setTimeout(function () { document.getElementById("spinner").style.display = ""; document.images["imgSpinner"].src = "Controls/Images/WaitImage2.gif"; }, 200);
}
    
function redirect(e) {
    var url = e.currentTarget.href;
    showSpinner();
    //location.href = url;
    //location.replace(url);
    //location.assign(url);
    window.open(url, "_self")
 }

function ClearTextbox(ctlId) {
    //var txt = document.getElementById(ctlId);
    //if (txt != null)
    //  txt.value = '';
    SetText(ctlId, '');
}
function ClearNewPswd()
{
    ClearTextbox("txtNew");
    ClearTextbox("txtRepeat");
}
function ClearCurrentPswd()
{
    ClearTextbox("txtCurrent")
}
function SetText(ctlId,text)
{
    var txt = document.getElementById(ctlId);
    if (txt != null)
        txt.value = text;
}

function chkOldColumnChanged(e) {
    var txtFriendlyName = document.getElementById("TextBoxFriendlyNameField");
    var ddFields = document.getElementById("DropDownListFields");
    var selectedIndex = ddFields.selectedIndex;
    var text = ddFields.options[selectedIndex].text;
    var frName = ddFields.options[selectedIndex].value;
    if (e.target.checked) {
        txtFriendlyName.value = frName;
        txtFriendlyName.disabled = true;
    }
    else {
        txtFriendlyName.disabled = false;
        txtFriendlyName.value = "";
        txtFriendlyName.focus();
    }
}

function changeShowLinks() {
    var ckShowLinks = document.getElementById("chkShowLinks");
    var hdnShowLinks = document.getElementById("hdnShowLinks");

    if (ckShowLinks != void 0 && hdnShowLinks != void 0)
        hdnShowLinks.value = ckShowLinks.checked;

}
function changeShowCircles() {
    var ckShowCircles = document.getElementById("chkShowCircles");
    var hdnShowCircles = document.getElementById("hdnShowCircles");

    if (ckShowCircles != void 0 && hdnShowCircles != void 0)
        hdnShowCircles.value = ckShowCircles.checked;

}
function changeShowPins() {
    var ckShowPins = document.getElementById("chkShowPins");
    var hdnShowPins = document.getElementById("hdnShowPins");

    if (ckShowPins != void 0 && hdnShowPins != void 0)
        hdnShowPins.value = ckShowPins.checked;

}
function changeInitAltitude() {
    var txtInitAltitude = document.getElementById("txtInitialAltit");
    var hdnInitAltitude = document.getElementById("hdnInitAltitude");

    if (txtInitAltitude != void 0 && hdnInitAltitude != void 0)
        hdnInitAltitude.value = txtInitAltitude.value;
}
function changeLineWidth() {
    var txtLineWidth = document.getElementById("txtLineWidth");
    var hdnLineWidth = document.getElementById("hdnLineWidth");
    if (txtLineWidth != void 0 && hdnLineWidth != void 0)
        hdnLineWidth.value = txtLineWidth.value;
}
function clickFileUpload() {
    var fileUpload = document.getElementById("FileRDL");
    if (fileUpload != null) {       
        fileUpload.click();        
    }
}

function round(value, decimals) {
    return Number(Math.round(value + 'e' + decimals) + 'e-' + decimals);
}

function getAttachedFile() {
    var fileUpload = document.getElementById("FileRDL");
    if (fileUpload != null) {
        var lblAttached = document.getElementById("lblFileChosen");
        var txturl = document.getElementById("txtURI");    
       
        var limit = document.getElementById("hfSizeLimit").value;

        if (fileUpload.files.length == 1) {
            lblAttached.innerText = fileUpload.files.length + " file";
            lblAttached.value = fileUpload.files.length + " file";
        }
        if (fileUpload.files.length > 1) {
            lblAttached.innerText = fileUpload.files.length + " files";
            lblAttached.value = fileUpload.files.length + " files";
        }
        
        lblAttached.title = " "

        for (var ii = 0; ii < fileUpload.files.length; ii++) {
            var fileName = fileUpload.files[ii].name;
            lblAttached.title = lblAttached.title + "  " + fileName;

            var fileSize = fileUpload.files[ii].size / 1024;

           if (fileSize > limit) {
                fileSize = round(fileSize / 1024, 1);
                var limitMB = round(limit/1024,1);
                var msg = "The size of " + fileName + " is " + fileSize + "MB. It exceeds the " + limitMB + "MB limit for files on the OUReports server. If the web site is on your company's server, contact your system administrator to increase the allowable size."
                fileUpload.value = "";
                __doPostBack("FileSizeExceeded", msg)

            }

        }
             
        txturl.value = "https://";
        
        var fn = fileUpload.files[0].name.toLowerCase();
        var txttn = document.getElementById("txtTableName");
        var ddtn = document.getElementById("DropDownTables");
        var sqlquerytext = document.getElementById("TextBoxSQLorSPtext");
        txttn.disabled = false;
        ddtn.disabled = false;
        if (fn.endsWith(".json")) {
            txttn.disabled = true;
            ddtn.disabled = true;
        }
        if (fn.endsWith(".xml")) {
            txttn.disabled = true;
            ddtn.disabled = true;
        }
        if (fn.endsWith(".txt")) {
            txttn.disabled = true;
            ddtn.disabled = true;
        }
        if (fn.endsWith(".rdl")) {
            txttn.disabled = true;
            ddtn.disabled = true;
        }
        if (sqlquerytext == null || sqlquerytext.value == null || sqlquerytext.value.trim() == "") {
            txttn.disabled = false;
        }
        if (sqlquerytext == null || sqlquerytext.value.trim() == "SELECT  * FROM") {
            txttn.disabled = false;
        }

    } 
}
function clickFileAddOnsUpload() {
    var fileUpload = document.getElementById("FileAddOns");
    if (fileUpload != null) {
        fileUpload.click();
    }
}
function getAttachedFileAddOns() {
    var fileUpload = document.getElementById("FileAddOns");
    if (fileUpload != null) {
        var lblAttached = document.getElementById("lblFileChosen");
        lblAttached.innerText = fileUpload.files[0].name

    }
}
function clickFileOleDbUpload() {
    var fileUpload = document.getElementById("FileOleDb");
    if (fileUpload != null) {
        fileUpload.click();
    }
}   
function getAttachedFileOleDb() {
    var fileUpload = document.getElementById("FileOleDb");
    if (fileUpload != null) {
        var lblAttached = document.getElementById("lblFileChosen");
        lblAttached.innerText = fileUpload.files[0].name

    }
}   
function getReportDimensions(app) {
    var reportContainer = document.getElementById("viewer");

    if (reportContainer) {
        var pageWidth = round(parseInt(reportContainer.offsetWidth) / 96, 1);
        var pageHeight = round(parseInt(reportContainer.offsetHeight) / 96, 1);

        var msg = pageWidth.toString() + "~" + pageHeight.toString();
        if (app != null) msg = msg + "~" + app;

        if (app != "WORD" && app != "PDF")
            showSpinner();

        __doPostBack("ReportDimensions", msg)
    }
}
function getPageDimensions() {
    var reportContainer = document.getElementById("viewer");

    if (reportContainer) {
        var pageWidth = round(parseInt(reportContainer.offsetWidth) / 96, 1);
        var pageHeight = round(parseInt(reportContainer.offsetHeight) / 96, 1);
        var msg = pageWidth.toString() + "~" + pageHeight.toString();

        __doPostBack("PageDimensions", msg)
    }
}

function buttonSubmitClick() {
    showSpinner();
    __doPostBack("SubmitClick", '');
}
