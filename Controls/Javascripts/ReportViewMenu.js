var menuobj = document.getElementById("pnlMenu");
var menudrop = document.getElementById("divDropMenu");
var CurrentField = null;
var currentSection = "divDrop";
var miID = "";

function showImageMenu(e) {
    e.stopPropagation()
    menuobj = document.getElementById("divImageFieldMenu");
    CurrentField = e.currentTarget
    menuobj.style.left = parseInt(e.x, 10) + "px";
    menuobj.style.top = parseInt(e.y, 10) + "px";

    menuobj.style.visibility = "visible"
    menuobj.style.display = '';
    document.onclick = hidemenu;

    
    return false;

}
function showGroupMenu(e) {
    e.stopPropagation();
    menuobj = document.getElementById("divGroupFieldMenu");
    CurrentField = e.currentTarget

    menuobj.style.left = parseInt(e.x, 10) + "px";
    menuobj.style.top = parseInt(e.y, 10) + "px";

    menuobj.style.visibility = "visible"
    menuobj.style.display = '';
    document.onclick = hidemenu;


    return false;

}
function showSpecialMenu(e) {

    menuobj = document.getElementById("divSpecialFieldMenu");
    CurrentField = e.currentTarget

    var rect = CurrentField.getBoundingClientRect();

    //menuobj.style.left = parseInt(rect.left + 6, 10) + "px";
    menuobj.style.left = parseInt(e.x, 10) + "px";
    menuobj.style.top = parseInt(e.y, 10) + "px";

    menuobj.style.visibility = "visible"
    menuobj.style.display = '';
    document.onclick = hidemenu;
    //var divHeading = document.getElementById("divFieldsMenuHeading");
    //var div = e.currentTarget;

    //currentSection = div.id;

    //divHeading.style.left = e.x + "px";
    //divHeading.style.top = e.y + "px";
    //divHeading.style.display = '';

    //var rectHeading = divHeading.getBoundingClientRect();

    //menuobj = document.getElementById("divFieldsMenu");
    //menuobj.style.left = e.x + "px";
    //menuobj.style.top = e.y + rectHeading.height + "px";
    //menuobj.style.visibility = "visible"
    //menuobj.style.display = '';
    //document.onclick = hideSpecialMenu;

    //e.stopPropagation();
    return false;
}
function showOptions(e) {
    var btn = e.currentTarget;
    var divButtons = btn.parentNode.parentNode;
    var rectBtn = btn.getBoundingClientRect();
    var rectButtons = divButtons.getBoundingClientRect();
    var miClearFields = document.getElementById("ClearFields");
    var miAddLabel = document.getElementById("AddLabel");
    var miSetColumnWidths = document.getElementById("TabularColWidths");
    var miReportFieldLayout = document.getElementById("ReportFieldLayout");
    var reportTemplate = reportView.ReportTemplate;
    var divDrop;
    var nChildren;

    e.stopPropagation();
    if (currentSection != void 0 && currentSection != "" && currentSection != undefined) {
        divDrop = document.getElementById(currentSection);

        menuobj = document.getElementById("divDropMenu");

        miSetColumnWidths.style.display = "none";
        miAddLabel.style.display = "none";

        nChildren = divDrop.children.length;
        if (divDrop.id == "divDrop") nChildren--;

        if (nChildren == 0) {
            miClearFields.style.color = "lightgray";
            
        }
        else {
            miClearFields.style.color = "black";
        }

        //if (currentSection == "divDrop") {
        //    if (reportTemplate == enumReportTemplate.FreeForm) {
        //        miAddLabel.style.display = '';
        //    }
        //    else {
        //        miAddLabel.style.display = "none";
        //    }
        //}

        if (reportTemplate == enumReportTemplate.FreeForm) {
            miSetColumnWidths.style.display = "none";
            miReportFieldLayout.style.display = '';
        }
        else {
            miSetColumnWidths.style.display = '';
            miReportFieldLayout.style.display = "none";
        }

        var left = parseInt(rectButtons.right - 200);
        var top = parseInt(rectButtons.bottom + 2);

        menuobj.style.left = left + "px";
        menuobj.style.top = top + "px";
        //e.stopPropagation;
        menuobj.style.visibility = "visible";
        menuobj.style.display = '';

        menuobj.setAttribute("data-x", left);
        menuobj.setAttribute("data-y", top);

        document.onclick = hidemenu;

    }

}
function showDropMenu(e) {
    var divDrop = e.currentTarget;
    var divMain = document.getElementById("divMain");  //divDrop.parentNode.parentNode;
    var rectMain = divMain.getBoundingClientRect();
    var miClearFields = document.getElementById("ClearFields");
    var miAddLabel = document.getElementById("AddLabel");
    var miSetColumnWidths = document.getElementById("TabularColWidths");
    var miReportFieldLayout = document.getElementById("ReportFieldLayout");
    //var miDetailSettings = document.getElementById("DetailSettings");
    //var miCaptionSettings = document.getElementById("CaptionSettings");

    var reportTemplate = reportView.ReportTemplate;
    currentSection = divDrop.id;
    if (divDrop.children.length == 0) {
        miClearFields.style.color = "lightgray";
     }
    else {
        miClearFields.style.color = "black";
    }

    if (reportTemplate == enumReportTemplate.FreeForm) {
        //miAddLabel.style.display = '';
        miSetColumnWidths.style.display = "none";
        miReportFieldLayout.style.display = '';
    }
    else {
        miAddLabel.style.display = "none";
        miReportFieldLayout.style.display = "none";
        miSetColumnWidths.style.display = '';
    }
       
    menuobj = document.getElementById("divDropMenu");
    //var rectMenu = menuobj.getBoundingClientRect();
    menuobj.style.left =e.x + "px";
    menuobj.style.top =e.y + "px";
    e.stopPropagation();
    menuobj.style.visibility = "visible"
    menuobj.style.display = '';

   var rectMenu = menuobj.getBoundingClientRect();
    var bottom = parseInt(e.y + rectMenu.height);
    var right = parseInt(e.x + rectMenu.width);
    if (rectMenu.bottom>rectMain.bottom)
        menuobj.style.top = parseInt(rectMenu.top - rectMenu.height) + "px";
    if (rectMenu.right>rectMain.right)
        menuobj.style.left = parseInt(rectMenu.left - rectMenu.width) + "px";

    menuobj.setAttribute("data-x", e.x);
    menuobj.setAttribute("data-y", e.y);
    document.onclick = hidemenu;

    return false;
}

function showReportFieldLayoutMenu() {
    menuobj = document.getElementById("divFieldLayoutMenu");

    var divContent = menuobj.parentNode;
    var rectContent = divContent.getBoundingClientRect();
    var menufield = document.getElementById("ReportFieldLayout");
    var ckInline = document.getElementById("ckInline");
    var ckBlock = document.getElementById("ckBlock");
    var rect = menufield.getBoundingClientRect();
    var reportType = reportView.ReportTemplate;
    var fieldFormat = reportView.ReportFieldLayout;

    CurrentField = null;

    if (fieldFormat == "Block") {
        ckBlock.innerHTML = "&#10004";
        ckInline.innerHTML = "&nbsp;&nbsp;";
    }
    else {
        ckBlock.innerHTML = "&nbsp;&nbsp;";
        ckInline.innerHTML = "&#10004";
    }
    var left = parseInt(rect.right, 10);
    var top = parseInt(rect.top + 5, 10);

    menuobj.style.left = left + "px"
    menuobj.style.top = top + "px";
    menuobj.style.visibility = "visible"
    menuobj.style.display = '';

    var rectMenu = menuobj.getBoundingClientRect();
    if (rectMenu.bottom > rectContent.bottom)
        menuobj.style.top = parseInt(top - rectMenu.height) + "px";
    if (rectMenu.right > rectContent.right)
        menuobj.style.left = parseInt(rect.left - rectMenu.width) + "px";

    document.onclick = hidemenu;
}

function showFieldLayoutMenu() {
    menuobj = document.getElementById("divFieldLayoutMenu");

    var divContent = menuobj.parentNode;
    var rectContent = divContent.getBoundingClientRect();
    var menufield = document.getElementById("CaptionFieldLayout");
    var ckInline = document.getElementById("ckInline");
    var ckBlock = document.getElementById("ckBlock");
    var rect = menufield.getBoundingClientRect();
    var reportType = reportView.ReportTemplate;
    var fieldFormat = CurrentField.dataset.fieldformat;

    if (fieldFormat == "Block") {
        ckBlock.innerHTML = "&#10004";
        ckInline.innerHTML = "&nbsp;&nbsp;";
    }
    else {
        ckBlock.innerHTML = "&nbsp;&nbsp;";
        ckInline.innerHTML = "&#10004";
    }
    var left = parseInt(rect.right, 10);
    var top = parseInt(rect.top + 5, 10);

    menuobj.style.left = left + "px"
    menuobj.style.top = top + "px";
    menuobj.style.visibility = "visible"
    menuobj.style.display = '';

    var rectMenu = menuobj.getBoundingClientRect();
    if (rectMenu.bottom > rectContent.bottom)
        menuobj.style.top = parseInt(top - rectMenu.height) + "px";
    if (rectMenu.right > rectContent.right)
        menuobj.style.left = parseInt(rect.left - rectMenu.width) + "px";

    document.onclick = hidemenu;
}

function showOrientationMenu() {
    menudrop = document.getElementById("ReportOrientation");
    menuobj = document.getElementById("divOrientationMenu");
    
    var divContent = menuobj.parentNode;
    var rectContent = divContent.getBoundingClientRect();
    var ckPortrait = document.getElementById("ckPortrait");
    var ckLandscape = document.getElementById("ckLandscape");
    var rect = menudrop.getBoundingClientRect();
    var reportOrientation = reportView.Orientation;
    var menutemplate = document.getElementById("divTemplateMenu");
    var menuFieldLayout = document.getElementById("divFieldLayoutMenu");

    menutemplate.style.display = 'none';
    menuFieldLayout.style.display = 'none';

    if (reportOrientation == "portrait") {
        ckPortrait.innerHTML = "&#10004";
        ckLandscape.innerHTML = "&nbsp;&nbsp;";
    }
    else {
        ckPortrait.innerHTML = "&nbsp;&nbsp;";
        ckLandscape.innerHTML = "&#10004";
    }
    var left = parseInt(rect.right + 1, 10);
    var top = parseInt(rect.top, 10);
    menuobj.style.left = left + "px"
    menuobj.style.top = top + "px";
    menuobj.style.visibility = "visible"
    menuobj.style.display = '';
    var rectMenu = menuobj.getBoundingClientRect();
    if (rectMenu.bottom > rectContent.bottom)
        menuobj.style.top = parseInt(top - rectMenu.height) + "px";
    if (rectMenu.right > rectContent.right)
        menuobj.style.left = parseInt(rect.left - rectMenu.width) + "px";

    document.onclick = hidemenu;

}
function showResizeMenu(menuID) {
    var saveField = CurrentField;

    miID = "";
    if (menuID == "ResizeLabel") {
        menudrop = document.getElementById("divLabelMenu");
    }
    else if (menuID == "ResizeSpecialField") {
        menudrop = document.getElementById("divSpecialFieldMenu");
    }
    else if (menuID == "ResizeImageField") {
        menudrop = document.getElementById("divImageFieldMenu");
    }
    else if (menuID == "ResizeCaption" || menuID == "ResizeDetail") {
        menudrop = document.getElementById("pnlMenu");
        if (CurrentField.id.startsWith("div_")) {
            if (menuID == "ResizeCaption")
                CurrentField = CurrentField.children[0]; // Caption
            else
                CurrentField = CurrentField.children[1]; // Detail
        }
        miID = menuID;
   }


    menuobj = document.getElementById("divResizeMenu");

    var divContent = menuobj.parentNode;
    var rectContent = divContent.getBoundingClientRect();
    var menuItem = document.getElementById(menuID);
    var ckWidth = document.getElementById("ckWidth");
    var ckHeight = document.getElementById("ckHeight");
    var ckBoth = document.getElementById("ckBoth");
    var rect = menuItem.getBoundingClientRect();
    var resize;

    resize = CurrentField.dataset.resize;
    CurrentField = saveField;

    if (resize == "horizontal") {
        ckWidth.innerHTML = "&#10004";
        ckHeight.innerHTML = "&nbsp;&nbsp;";
        ckBoth.innerHTML = "&nbsp;&nbsp;";
    }
    else if (resize == "vertical") {
        ckWidth.innerHTML = "&nbsp;&nbsp;";
        ckHeight.innerHTML = "&#10004";
        ckBoth.innerHTML = "&nbsp;&nbsp;";
    }
    else if (resize == "both") {
        ckWidth.innerHTML = "&nbsp;&nbsp;";
        ckHeight.innerHTML = "&nbsp;&nbsp;";
        ckBoth.innerHTML = "&#10004";
    }
    var left = parseInt(rect.right, 10);
    var top = parseInt(rect.top + 5, 10);

    menuobj.style.display = 'none';
    menuobj.style.left = left + "px"
    menuobj.style.top = top + "px";
    menuobj.style.visibility = "visible"
    menuobj.style.display = '';

    var rectMenu = menuobj.getBoundingClientRect();
    if (rectMenu.bottom > rectContent.bottom)
        menuobj.style.top = parseInt(top - rectMenu.height) + "px";
    if (rectMenu.right > rectContent.right)
        menuobj.style.left = parseInt(rect.left - rectMenu.width) + "px";

    document.onclick = hidemenu;

} 

function showTextAlignMenu(menuID) {
    var saveField = CurrentField;
    var captionAlign = reportView.ReportCaptionAlign;
    var detailAlign = reportView.ReportDetailAlign;
    var reportTemplate = reportView.ReportTemplate;
    var divAuto = document.getElementById("Auto");
    
    miID = "";
    divAuto.style.display = "none";

    if (menuID == "LabelTextAlign") {
        menudrop = document.getElementById("divLabelMenu");
    }
    else if (menuID == "CaptionTextAlign" || menuID == "DetailTextAlign") {
        menudrop = document.getElementById("pnlMenu");
        if (CurrentField.id.startsWith("div_")) {
            if (menuID == "CaptionTextAlign")
                CurrentField = CurrentField.children[0]; // Caption
            else
                CurrentField = CurrentField.children[1]; // Detail
        }
        miID = menuID;
    }
    else if (menuID == "ReportCaptionAlign" || menuID == "ReportDetailAlign") {
        menudrop = document.getElementById("divDropMenu");
        miID = menuID;

        if (reportTemplate == enumReportTemplate.Tabular && menuID == "ReportDetailAlign" )
            divAuto.style.display="inline-block"
    }
    else
        menudrop = document.getElementById("divSpecialFieldMenu");

    menuobj = document.getElementById("divTextAlignMenu");

    var divContent = menuobj.parentNode;
    var rectContent = divContent.getBoundingClientRect();
    var menuItem = document.getElementById(menuID);
    var ckLeft = document.getElementById("ckLeft");
    var ckCenter = document.getElementById("ckCenter");
    var ckRight = document.getElementById("ckRight");
    var ckAuto = document.getElementById("ckAuto");
    var rect = menuItem.getBoundingClientRect();
    var textAlign;

    if (menuID == "ReportCaptionAlign") {
        textAlign = captionAlign;
        saveField = menuItem;
    }
    else if (menuID == "ReportDetailAlign") {
        textAlign = detailAlign;
        saveField = menuItem;
    }
    else
        textAlign = CurrentField.dataset.textalign;

    CurrentField = saveField;
    if (textAlign == "Left") {
        ckLeft.innerHTML = "&#10004";
        ckCenter.innerHTML = "&nbsp;&nbsp;";
        ckRight.innerHTML = "&nbsp;&nbsp;";
    }
    else if (textAlign == "Center") {
        ckLeft.innerHTML = "&nbsp;&nbsp;";
        ckCenter.innerHTML = "&#10004;";
        ckRight.innerHTML = "&nbsp;&nbsp;";
    }
    else if (textAlign == "Right") {
        ckLeft.innerHTML = "&nbsp;&nbsp;";
        ckCenter.innerHTML = "&nbsp;&nbsp;";
        ckRight.innerHTML = "&#10004;";
    }
    else if (textAlign == "Auto") {
        ckLeft.innerHTML = "&nbsp;&nbsp;";
        ckCenter.innerHTML = "&nbsp;&nbsp;";
        ckRight.innerHTML = "&nbsp;&nbsp;";
        ckAuto.innerHTML = "&#10004;";
    }

    var left = parseInt(rect.right, 10);
    var top = parseInt(rect.top + 5, 10);

    menuobj.style.display = 'none';
    menuobj.style.left = left + "px"
    menuobj.style.top = top + "px";
    menuobj.style.visibility = "visible"
    menuobj.style.display = '';

    var rectMenu = menuobj.getBoundingClientRect();
    if (rectMenu.bottom > rectContent.bottom)
        menuobj.style.top = parseInt(top - rectMenu.height) + "px";
    if (rectMenu.right > rectContent.right)
        menuobj.style.left = parseInt(rect.left - rectMenu.width) + "px";


    document.onclick = hidemenu;
}

function showTemplateMenu() {
    menudrop = document.getElementById("ReportTemplate");
    menuobj = document.getElementById("divTemplateMenu");

    var divContent = menuobj.parentNode;
    var rectContent = divContent.getBoundingClientRect();
    var ckTabular = document.getElementById("ckTabular");
    var ckFreeForm = document.getElementById("ckFreeForm");
    var rect = menudrop.getBoundingClientRect();
    var reportType = reportView.ReportTemplate;
    var menuOrientation = document.getElementById("divOrientationMenu");
    var menuFieldLayout = document.getElementById("divFieldLayoutMenu");

    menuOrientation.style.display = 'none';
    menuFieldLayout.style.display = 'none';
    if (reportType == enumReportTemplate.Tabular) {
        ckTabular.innerHTML = "&#10004";
        ckFreeForm.innerHTML = "&nbsp;&nbsp;";
    }
    else {
        ckTabular.innerHTML = "&nbsp;&nbsp;";
        ckFreeForm.innerHTML = "&#10004";
    }
    var left = parseInt(rect.right + 1, 10);
    var top = parseInt(rect.top, 10);
    menuobj.style.left = left + "px"
    menuobj.style.top = top + "px";
    menuobj.style.visibility = "visible"
    menuobj.style.display = '';

    var rectMenu = menuobj.getBoundingClientRect();
    if (rectMenu.bottom > rectContent.bottom)
        menuobj.style.top = parseInt(top - rectMenu.height) + "px";
    if (rectMenu.right > rectContent.right) 
        menuobj.style.left = parseInt(rect.left - rectMenu.width) + "px";

        document.onclick = hidemenu;
}
function showLabelMenu(e) {
    menuobj = document.getElementById("divLabelMenu");

    CurrentField = e.currentTarget

    if (!e.currentTarget.id.startsWith("divLabel")) // text div triggered the event, so get it's parent
        CurrentField = CurrentField.parentNode;

    var rect = CurrentField.getBoundingClientRect();
    var divContent = menuobj.parentNode;
    var rectContent = divContent.getBoundingClientRect();
    var left = parseInt(e.x, 10);
    var top = parseInt(e.y, 10);

    menuobj.style.left = left + "px";
    menuobj.style.top = top + "px";
    menuobj.style.visibility = "visible"
    menuobj.style.display = '';

    var rectMenu = menuobj.getBoundingClientRect();
    if (rectMenu.bottom > rectContent.bottom)
        menuobj.style.top = parseInt(top - rectMenu.height) + "px";
    if (rectMenu.right > rectContent.right)
        menuobj.style.left = parseInt(left - rectMenu.width) + "px";

    document.onclick = hidemenu;
    e.stopPropagation();
    return false;
}

function showMenu(e) {
    menuobj = document.getElementById("pnlMenu");

    var divContent = menuobj.parentNode;
    var rectContent = divContent.getBoundingClientRect();
    var reportType = reportView.ReportTemplate;
    var captionFieldLayout = document.getElementById("CaptionFieldLayout");
    var moveWithArrowKeys = document.getElementById("MoveWithArrowKeys");
    var resizeCaption = document.getElementById("ResizeCaption");
    var resizeDetail = document.getElementById("ResizeDetail");


    if (reportType == enumReportTemplate.Tabular) {
        captionFieldLayout.style.display = "none";
        moveWithArrowKeys.style.display = "none";
        resizeCaption.style.display = "none";
        resizeDetail.style.display = "none";
    }
    else {
        captionFieldLayout.style.display = '';
        moveWithArrowKeys.style.display = '';
        resizeCaption.style.display = '';
        resizeDetail.style.display = '';

    }
    //element the event was attached to or the element whose event listener triggered the event
    CurrentField = e.currentTarget; 

    var rect = CurrentField.getBoundingClientRect();
    var left = parseInt(rect.left + 6, 10);
    var top = parseInt(e.y, 10);

    menuobj.style.left = left + "px"
    menuobj.style.top = top + "px";
    menuobj.style.visibility = "visible"
    menuobj.style.display = '';

    e.stopPropagation();

    // can't get dimensions of menu until it is visible
    var rectMenu = menuobj.getBoundingClientRect();
    if (rectMenu.bottom > rectContent.bottom)
        menuobj.style.top = parseInt(top - rectMenu.height) + "px";
    if (rectMenu.right > rectContent.right)
        menuobj.style.left = parseInt(left - rectMenu.width) + "px";

    document.onclick = hidemenu;
    return false;
}

function hideSpecialMenu(e) {
    var divHeading = document.getElementById("divFieldsMenuHeading");

    divHeading.style.display = 'none';
    hidemenu(e);
}
function hidemenu(e) {
    var menutemplate = document.getElementById("divTemplateMenu");
    var menuParent;

     menuobj.style.visibility = "hidden"
     menuobj.style.display = 'none';
     if (menuobj.id == "divTemplateMenu" || menuobj.id == "divOrientationMenu") {
         menuParent = document.getElementById("divDropMenu");
         menuParent.style.display = 'none';
     }
     else if (menuobj.id == "divFieldLayoutMenu") {
         menuParent = document.getElementById("pnlMenu");
         menuParent.style.display = 'none';
     }
     else if (menuobj.id == "divTextAlignMenu" || menuobj.id == "divResizeMenu") {
         menuParent = menudrop;
         menuParent.style.display = 'none';
     }
     else
         menutemplate.style.display = 'none';
}

function highlight(e) {
    var menuID = e.currentTarget.id;
    var firingobj = e.target
    var targName = firingobj.tagName;
    var menulayout;
    var menuTextAlign;
    var menuResize;

    // find the table if over a table element
    if (targName == 'TBODY') {
        firingobj = firingobj.parentNode;
    }
    else if (targName == 'TR') {
        firingobj = firingobj.parentNode.parentNode;
    }
    else if (targName == 'TD') {
        firingobj = firingobj.parentNode.parentNode.parentNode;
    }
    if (firingobj.className=="menuitems"||firingobj.parentNode.className=="menuitems"){
      if (firingobj.parentNode.className=="menuitems") 
       firingobj=firingobj.parentNode //up one node
      firingobj.style.backgroundColor = "highlight"

        if (firingobj.id == "ReportTemplate") {
            showTemplateMenu();
        }
        else if (firingobj.id == "ReportOrientation") {
            showOrientationMenu();
        }
        else if (firingobj.id == "CaptionFieldLayout") {
            menuTextAlign = document.getElementById("divTextAlignMenu");
            menuResize = document.getElementById("divResizeMenu");
            menuResize.style.display = 'none';
            menuTextAlign.style.display = 'none';
            showFieldLayoutMenu();
        }
        else if (firingobj.id == "LabelTextAlign" ||
            firingobj.id == "SpecialFieldTextAlign") {
            menuResize = document.getElementById("divResizeMenu");
            menuResize.style.display = 'none';
            showTextAlignMenu(firingobj.id);
        }
        else if (firingobj.id == "ResizeLabel" ||
            firingobj.id == "ResizeSpecialField") {
            menuTextAlign = document.getElementById("divTextAlignMenu");
            menuTextAlign.style.display = 'none';
            showResizeMenu(firingobj.id);
        }
        else if (firingobj.id == "ResizeImageField") {
            showResizeMenu(firingobj.id);
        }
        else if (menuID == "pnlMenu") {
            menuTextAlign = document.getElementById("divTextAlignMenu");
            menulayout = document.getElementById("divFieldLayoutMenu");
            menuResize = document.getElementById("divResizeMenu");
            menulayout.style.display = 'none';
            menuTextAlign.style.display = 'none';
            menuResize.style.display = 'none';
            if (firingobj.id == "CaptionTextAlign" || firingobj.id == "DetailTextAlign") {
                showTextAlignMenu(firingobj.id);
            }
            if (firingobj.id == "ResizeCaption" || firingobj.id == "ResizeDetail") {
                showResizeMenu(firingobj.id);
            }
        }
        else if (menuID == "divDropMenu") {
            menuTextAlign = document.getElementById("divTextAlignMenu");
            var menuOrientation = document.getElementById("divOrientationMenu");
            var menuTemplate = document.getElementById("divTemplateMenu");
            var menuFieldLayout = document.getElementById("divFieldLayoutMenu");

            menuOrientation.style.display = 'none';
            menuTextAlign.style.display = 'none';
            menuTemplate.style.display = 'none';
            menuFieldLayout.style.display = 'none';

            if (firingobj.id == "ReportCaptionAlign" || firingobj.id == "ReportDetailAlign") {
                showTextAlignMenu(firingobj.id);
            }
            if (firingobj.id == "ReportFieldLayout") {
                showReportFieldLayoutMenu();
            }
        }
        else if (menuID == "divLabelMenu" || menuID == "divSpecialFieldMenu") {
            menuTextAlign = document.getElementById("divTextAlignMenu");
            menuTextAlign.style.display = 'none';
            menuResize = document.getElementById("divResizeMenu");
            menuResize.style.display = 'none';
        }
        else if (menuID == "divImageFieldMenu") {
            menuResize = document.getElementById("divResizeMenu");
            menuResize.style.display = 'none';
        }
  }
}

function lowlight(e) {
    var firingobj = e.target
    var targName = firingobj.tagName;

    // find the table if over a table element
    if (targName == 'TBODY') {
        firingobj = firingobj.parentNode;
    }
    else if (targName == 'TR') {
        firingobj = firingobj.parentNode.parentNode;
    }
    else if (targName == 'TD') {
        firingobj = firingobj.parentNode.parentNode.parentNode;
    }

    if (firingobj.className=="menuitems"||firingobj.parentNode.className=="menuitems"){
        if (firingobj.parentNode.className=="menuitems") 
            firingobj=firingobj.parentNode //up one node
        firingobj.style.backgroundColor=""
        window.status=''
    }
}

function doOption(e) {
    var parentMenu = e.currentTarget;
    var firingobj = e.target
    var lblHeader = document.getElementById("lblHeader");
    var reportTitle = document.getElementById("hdnReportTitle").value;

    if (firingobj.className == "menuitems" || firingobj.parentNode.className == "menuitems") {
        if (firingobj.parentNode.className=="menuitems") 
            firingobj = firingobj.parentNode

        if (firingobj != null && firingobj.className == "menuitems") {
            var id = firingobj.id;
            switch (id) {
                case "SaveAndShow":
                    saveAndShow();
                    break;
                case "SaveAndReturn":
                    saveAndReturn();
                    break;
                case "SaveAndClose":
                    saveAndClose();
                    break;
                case "CloseDesigner":
                    __doPostBack("MsgBoxAction", "Close Designer");
                    break;
                case "DeleteField":
                case "DeleteSpecialField":
                case "DeleteImageField":
                    deleteField(CurrentField);
                    CurrentField = null;
                    break;
                case "EditCaption":
                    var divCaption = getCaptionDiv(CurrentField);
                    editCaption(divCaption);
                    break;
                case "AddLabel":
                case "Label":
                    addLabel(e);
                    break;
                case "MoveWithArrowKeys":
                case "MoveSpecialFieldWithArrows":
                case "MoveImageFieldWithArrows":
                    moveWithArrowKeys(CurrentField);
                    CurrentField = null;
                    break;
                case "DeleteLabel":
                    deleteLabel(CurrentField);
                    CurrentField = null;
                    break;
                case "EditLabel":
                    editLabel(CurrentField);
                    break;
                case "LabelSettings":
                case "SpecialFieldSettings":
                case "GroupFieldSettings":
                    showFontDlg(CurrentField);
                    break;
                case "CaptionSettings":
                case "DetailSettings":
                case "HeaderFieldSettings":
                case "FooterFieldSettings":
                    CurrentField = firingobj;
                    showFontDlg(CurrentField);
                    break;
                case "HeaderSettings":
                case "FooterSettings":
                    CurrentField = firingobj;
                    showHeaderFooterDlg(CurrentField);
                    break;
                case "ImageFieldSettings":
                  /*CurrentField = firingobj*/;
                  showImageSettingsDlg(CurrentField);
                   break;
                case "TabularColWidths":
                    showTabularColWidthDlg()
                    break;
                case "MoveWithArrows":
                    moveLabelWithArrows(CurrentField);
                    CurrentField = null;
                    break;
                case "MoveGroupVertically":
                    moveGroupVertically(CurrentField);
                    CurrentField = null;
                    break;
                case "ResizeGroupFieldHeight":
                    CurrentField.dataset.resize ="vertical";
                    createSizer(CurrentField);
                break;
                case "ClearFields":
                    var divDrop;  //= document.getElementById("divDrop");
                    if (currentSection != void 0 && currentSection != "" && currentSection != undefined) {
                        divDrop = document.getElementById(currentSection);
                        var nChildren = divDrop.children.length;
                        if (divDrop.id == "divDrop") nChildren--;
                        if (nChildren > 0)
                            clearFields(divDrop);
                        else {
                            switch (divDrop.id) {
                                case "divDrop":
                                    showMessage("Detail section currently has no fields defined.", "No Fields Defined",enumMessageType.Warning);
                                    break;
                                case "divHeader":
                                    showMessage("Header section currently has no fields defined.", "No Fields Defined",enumMessageType.Warning);
                                    break;
                                case "divFooter":
                                    showMessage("Footer section currently has no fields defined.", "No Fields Defined",enumMessageType.Warning);
                                    break;
                            }
                        }
                    }
                    else
                        showMessage("Current Section Has Not Been Specified.", enumMessageType.Warning);
                    //if (divDrop.children.length > 0)
                    //    clearFields(divDrop);
                    break;
                case "Tabular":
                    if (reportView.ReportTemplate != enumReportTemplate.Tabular) {
                        lblHeader.innerText = "Tabular Report Designer - " + reportTitle;
                        changeTemplate("Tabular");
                        reportView.ReportTemplate = enumReportTemplate.Tabular;
                        showSection("divDrop");
                    }
                    break;
                case "FreeForm":
                    if (reportView.ReportTemplate != enumReportTemplate.FreeForm) {
                        lblHeader.innerText = "Free Form Report Designer - " + reportTitle;
                        changeTemplate("FreeForm");
                        reportView.ReportTemplate = enumReportTemplate.FreeForm;
                       showSection("divDrop");
                    }
                    break;
                case "Portrait":
                case "Landscape":
                    changeOrientation(id);
                     break;
                case "Inline":
                case "Block":
                    changeFieldLayout(CurrentField, id);
                    break;
                case "Left":
                case "Center":
                case "Right":
                case "Auto":
                    changeTextAlign(CurrentField, id);
                    break;
                case "horizontal":
                case "vertical":
                case "both":
                    var save = "";
                    if (miID == "ResizeCaption" || miID == "ResizeDetail") {
                        if (CurrentField.id.startsWith("div_")) {
                            save = CurrentField;
                            if (miID == "ResizeCaption")
                                CurrentField = CurrentField.children[0]; // Caption
                            else
                                CurrentField = CurrentField.children[1]; // Detail
                        }
                    }
                    CurrentField.dataset.resize = id;
                    createSizer(CurrentField);
                    //if (save != "")
                    //    CurrentField = save;
                    break;
                case "Confidentiality":
                case "PageNumber":
                case "PageNofM":
                case "ReportTitle":
                case "ReportUser":
                case "SqlQuery":
                case "ReportComments":
                case "PrintDate":
                case "PrintTime":
                case "PrintDateTime":
                     addSpecialField(id);
                     break;

            }
        }

    }
}


