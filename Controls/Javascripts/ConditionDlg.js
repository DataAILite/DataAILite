function showTextFields(ctlPrefix, showAll) {
    document.getElementById(ctlPrefix + "trText").style.display = "";
    document.getElementById(ctlPrefix + "trCalendar").style.display = "none";
    if (showAll) {
        document.getElementById(ctlPrefix + "lblValue1").style.display = "none";
        document.getElementById(ctlPrefix + "lblValue2").style.display = "";
        document.getElementById(ctlPrefix + "ddValField1").style.display = "none";
        document.getElementById(ctlPrefix + "txtValue1").style.display = "";
        document.getElementById(ctlPrefix + "divValue2").style.display = "";
        document.getElementById(ctlPrefix + "ddValField2").style.display = "none";
        document.getElementById(ctlPrefix + "txtValue2").style.display = "";
        document.getElementById(ctlPrefix + "btnText1").value = "Fields";
        document.getElementById(ctlPrefix + "btnText2").value = "Fields";
        document.getElementById(ctlPrefix + "txtValue1").focus();
    }
    else {
        document.getElementById(ctlPrefix + "lblValue1").style.display = "";
        document.getElementById(ctlPrefix + "lblValue2").style.display = "none";
        document.getElementById(ctlPrefix + "ddValField1").style.display = "none";
        document.getElementById(ctlPrefix + "txtValue1").style.display = "";
        document.getElementById(ctlPrefix + "divValue2").style.display = "none";
        document.getElementById(ctlPrefix + "btnText1").value = "Fields";
        document.getElementById(ctlPrefix + "txtValue1").focus();

    }
}
function showDateFields(ctlPrefix, showAll) {
    document.getElementById(ctlPrefix + "trText").style.display = "none";
    document.getElementById(ctlPrefix + "trCalendar").style.display = "";
    if (showAll) {
        document.getElementById(ctlPrefix + "lblCal1").style.display = "none";
        document.getElementById(ctlPrefix + "lblCal2").style.display = "";
        document.getElementById(ctlPrefix + "divCalendar1").style.display = "";
        document.getElementById(ctlPrefix + "divDateField1").style.display = "none";
        document.getElementById(ctlPrefix + "divCal2").style.display = "";
        document.getElementById(ctlPrefix + "divCalendar2").style.display = "";
        document.getElementById(ctlPrefix + "divDateField2").style.display = "none";
        document.getElementById(ctlPrefix + "btnCal1").value = "Fields";
        document.getElementById(ctlPrefix + "btnCal2").value = "Fields";
        document.getElementById(ctlPrefix + "Date1").focus();
    }
    else {
        document.getElementById(ctlPrefix + "lblCal1").style.display = "";
        document.getElementById(ctlPrefix + "lblCal2").style.display = "none";
        document.getElementById(ctlPrefix + "divCalendar1").style.display = "";
        document.getElementById(ctlPrefix + "divDateField1").style.display = "none";
        document.getElementById(ctlPrefix + "divCal2").style.display = "none";
        document.getElementById(ctlPrefix + "btnCal1").value = "Fields";
        document.getElementById(ctlPrefix + "Date1").focus();
    }
}
function handleOtherFieldsChange(ctlPrefix, ctlId) {
    var id = ctlPrefix + ctlId
    var ddFields = document.getElementById(id);
    var selectedField = ddFields.options[ddFields.selectedIndex].innerHTML;

    var hdnValField1 = document.getElementById(ctlPrefix + "hdnValField1");
    var hdnValField2 = document.getElementById(ctlPrefix + "hdnValField2");
    var hdnDateField1 = document.getElementById(ctlPrefix + "hdnDateField1");
    var hdnDateField2 = document.getElementById(ctlPrefix + "hdnDateField2");

    switch (ctlId) {
        case "ddValField1":
            hdnValField1.value = selectedField;
            break;
        case "ddValField2":
            hdnValField2.value = selectedField;
            break;
        case "ddDateField1":
            hdnDateField1.value = selectedField;
            break;
        case "ddDateField2":
            hdnDateField2.value = selectedField;
            break;
    }
}
function SetDropDownIndex(ctlPrefix, ddName ) {
    var ddCtl = document.getElementById(ctlPrefix + ddName);
    var hdnFieldIndex = document.getElementById(ctlPrefix + "hdnFieldIndex");
    if (ddCtl != null) {
        ddCtl.selectedIndex = hdnFieldIndex.value;
    }
}
function handleFieldChange(ctlPrefix)
{
    var ddField = document.getElementById(ctlPrefix + "ddField");
    var ddOperator = document.getElementById(ctlPrefix + "ddOperator");
    var oper = ddOperator.options[ddOperator.selectedIndex].value;
    var hdnFieldIndex = document.getElementById(ctlPrefix + "hdnFieldIndex");

    if (oper != null && ddField.selectedIndex > 0) {
        var selectedType = ddField.value.toUpperCase();
        var isDate = (selectedType != "TIME" && (selectedType.indexOf("DATE") > -1 || selectedType.indexOf("TIME") > -1));
        hdnFieldIndex.value = ddField.selectedIndex;

        if (oper == "between") {
            if (isDate) {
                showDateFields(ctlPrefix, true);
            }
            else {
                showTextFields(ctlPrefix, true);
            }
        }
        else {
            if (isDate) {
                showDateFields(ctlPrefix, false);
            }
            else {
                showTextFields(ctlPrefix, false);
            }
        }
        ddOperator.focus();
    }

}

function handleOperatorChange(ctlPrefix)
{
    var ddOperator = document.getElementById(ctlPrefix + "ddOperator");
    var txtVal,ddVal,btn
    var opt = ddOperator.options[ddOperator.selectedIndex].value;
    var ddField = document.getElementById(ctlPrefix + "ddField");
    var mode = document.getElementById(ctlPrefix + "hdnMode").value;
    var hdnOperator = document.getElementById(ctlPrefix + "hdnOperator");
    if (opt != null && ddField.selectedIndex>0)
    {
        var oldOpt = hdnOperator.value;
        hdnOperator.value = opt;
        //var selectedField = ddField.options[ddField.selectedIndex].innerHTML;
        var selectedType = ddField.value.toUpperCase();
        //var parts = selectedField.split(".");
        var isDate = (selectedType != "TIME" && (selectedType.indexOf("DATE") > -1 || selectedType.indexOf("TIME") > -1));
        
        if (opt == "IS NULL") {
            document.getElementById(ctlPrefix + "trText").style.display = "none";
            document.getElementById(ctlPrefix + "trCalendar").style.display = "none";
        }
        else {
            if (opt == "between") {
                if (isDate) {
                    if (mode == "Add") {
                        showDateFields(ctlPrefix, true);
                    }
                    else if (mode == "Edit") {
                        showDateFields(ctlPrefix, true);
                        ////document.getElementById(ctlPrefix + "trText").style.display = "none";
                        ////document.getElementById(ctlPrefix + "trCalendar").style.display = "";
                        btn = document.getElementById(ctlPrefix + "btnCal1");
                        if (btn.value == "Date") {
                            ddVal = document.getElementById(ctlPrefix + "ddDateField1");
                            ddVal.tabIndex = 0;
                            ddVal.focus();
                        }
                        else {
                            txtVal = document.getElementById(ctlPrefix + "Date1");
                            txtVal.focus();
                        }
                    }
                }
                else {
                    if (mode == "Add") {
                        showTextFields(ctlPrefix, true);
                    }
                    else if (mode == "Edit") {
                        showTextFields(ctlPrefix, true);
                        //document.getElementById(ctlPrefix + "trText").style.display = "";
                        //document.getElementById(ctlPrefix + "trCalendar").style.display = "none";
                        btn = document.getElementById(ctlPrefix + "btnText1");
                        if (btn.value == "Text") {
                            ddVal = document.getElementById(ctlPrefix + "ddValField1");
                            ddVal.tabIndex = 0;
                            ddVal.focus();
                        }
                        else {
                            txtVal = document.getElementById(ctlPrefix + "txtValue1");
                            txtVal.focus();
                        }
                    }
                }
            }
            else {
                if (isDate) {
                    if (mode == "Add") {
                        showDateFields(ctlPrefix, false);
                    }
                    else if (mode == "Edit") {
                        showDateFields(ctlPrefix, false);
                        //document.getElementById(ctlPrefix + "trText").style.display = "none";
                        //document.getElementById(ctlPrefix + "trCalendar").style.display = "";
                        btn = document.getElementById(ctlPrefix + "btnCal1");
                        if (btn.value == "Date") {
                            ddVal = document.getElementById(ctlPrefix + "ddDateField1");
                            ddVal.tabIndex = 0;
                            ddVal.focus();
                        }
                        else {
                            txtVal = document.getElementById(ctlPrefix + "Date1");
                            txtVal.focus();
                        }
                    }
                }
                else {
                    if (mode == "Add") {
                        showTextFields(ctlPrefix, false);
        }
                    else if (mode == "Edit") {
                        showTextFields(ctlPrefix, false);
                        //document.getElementById(ctlPrefix + "trText").style.display = "";
                        //document.getElementById(ctlPrefix + "trCalendar").style.display = "none";

                        btn = document.getElementById(ctlPrefix + "btnText1");
                        if (btn.value == "Text") {
                            ddVal = document.getElementById(ctlPrefix + "ddValField1");
                            ddVal.tabIndex = 0;
                            ddVal.focus();
                        }
                        else {
                            txtVal = document.getElementById(ctlPrefix + "txtValue1");
                            txtVal.focus();
                        }
                    }
                }
            }
        }
        //if (opt == "between") {
        //    if (isDate) {
        //        if (mode == "Add") {
        //            showDateFields(ctlPrefix, true);
        //        }
        //        else if (mode == "Edit") {
        //            btn = document.getElementById(ctlPrefix + "btnCal1");
        //            if (btn.value == "Date") {
        //                ddVal = document.getElementById(ctlPrefix + "ddDateField1");
        //                ddVal.tabIndex = 0;
        //                ddVal.focus();
        //            }
        //            else {
        //                txtVal = document.getElementById(ctlPrefix + "Date1");
        //                txtVal.focus();
        //            }
        //        }
        //    }
        //    else {
        //        if (mode == "Add") {
        //            showTextFields(ctlPrefix, true);
        //        }
        //        else if (mode == "Edit") {
        //            btn = document.getElementById(ctlPrefix + "btnText1");
        //            if (btn.value == "Text") {
        //                ddVal = document.getElementById(ctlPrefix + "ddValField1");
        //                ddVal.tabIndex = 0;
        //                ddVal.focus();
        //            }
        //            else {
        //                txtVal = document.getElementById(ctlPrefix + "txtValue1");
        //                txtVal.focus();
        //            }
        //        }
        //    }
        //}
        //else {
        //    if (isDate) {
        //        if (mode == "Add") {
        //            showDateFields(ctlPrefix,false);
        //        }
        //        else if (mode == "Edit") {
        //            btn = document.getElementById(ctlPrefix + "btnCal1");
        //            if (btn.value == "Date") {
        //                ddVal = document.getElementById(ctlPrefix + "ddDateField1");
        //                ddVal.tabIndex = 0;
        //                ddVal.focus();
        //            }
        //            else {
        //                txtVal = document.getElementById(ctlPrefix + "Date1");
        //                txtVal.focus();
        //            }
        //        }
        //    }
        //    else {
        //        if (mode == "Add") {
        //            showTextFields(ctlPrefix, false);
        //        }
        //        else if (mode == "Edit") {
        //            btn = document.getElementById(ctlPrefix + "btnText1");
        //            if (btn.value == "Text") {
        //                ddVal = document.getElementById(ctlPrefix + "ddValField1");
        //                ddVal.tabIndex = 0;
        //                ddVal.focus();
        //            }
        //            else {
        //                txtVal = document.getElementById(ctlPrefix + "txtValue1");
        //                txtVal.focus();
        //            }
        //        }
        //    }
        //}

    }
}
function showTextorFields(ctlPrefix, ctlId) {
    var btnId = ctlPrefix + ctlId;
    var btn = document.getElementById(btnId);
    var ddField = document.getElementById(ctlPrefix + "ddField");
    var ddVal
    var txtVal
    var hdnBtnText1 = document.getElementById(ctlPrefix + "hdnBtnText1");
    var hdnBtnText2 = document.getElementById(ctlPrefix + "hdnBtnText2");
    var hdnBtnCal1 = document.getElementById(ctlPrefix + "hdnBtnCal1");
    var hdnBtnCal2 = document.getElementById(ctlPrefix + "hdnBtnCal2");

    if (ddField.selectedIndex > 0)
    {
        if (btn.value == "Fields") {
            switch (ctlId) {
                case "btnText1":
                    ddVal = document.getElementById(ctlPrefix + "ddValField1");
                    txtVal = document.getElementById(ctlPrefix + "txtValue1");
                    ddVal.style.display = "";
                    txtVal.style.display = "none";
                    ddVal.selectedIndex = 0;
                    ddVal.tabIndex = 0;
                    ddVal.focus();
                    btn.value = "Text";
                    btn.title = "Enter text value";
                    hdnBtnText1.value = btn.value;
                    break;
                case "btnText2":
                    ddVal = document.getElementById(ctlPrefix + "ddValField2");
                    txtVal = document.getElementById(ctlPrefix + "txtValue2");
                    ddVal.style.display = "";
                    txtVal.style.display = "none";
                    ddVal.selectedIndex = 0;
                    ddVal.tabIndex = 0;
                    ddVal.focus();
                    btn.value = "Text";
                    btn.title = "Enter text value";
                    hdnBtnText2.value = btn.value;
                    break;
                case "btnCal1":
                    ddVal = document.getElementById(ctlPrefix + "ddDateField1");
                    document.getElementById(ctlPrefix + "divDateField1").style.display = "";
                    document.getElementById(ctlPrefix + "divCalendar1").style.display = "none";
                    ddVal.selectedIndex = 0;
                    ddVal.tabIndex = 0;
                    ddVal.focus();
                    btn.value = "Date";
                    btn.title = "Enter date value";
                    hdnBtnCal1.value = btn.value;
                    break;
                case "btnCal2":
                    ddVal = document.getElementById(ctlPrefix + "ddDateField2");
                    document.getElementById(ctlPrefix + "divDateField2").style.display = "";
                    document.getElementById(ctlPrefix + "divCalendar2").style.display = "none";
                    ddVal.selectedIndex = 0;
                    ddVal.tabIndex = 0;
                    ddVal.focus();
                    btn.value = "Date";
                    btn.title = "Enter date value";
                    hdnBtnCal2.value = btn.value;
                    break;
            }
        }
        else {
            switch (ctlId) {
                case "btnText1":
                    ddVal = document.getElementById(ctlPrefix + "ddValField1");
                    txtVal = document.getElementById(ctlPrefix + "txtValue1");
                    ddVal.style.display = "none";
                    txtVal.value = "";
                    txtVal.style.display = "";
                    txtVal.focus();
                    btn.value = "Fields";
                    btn.title = "Choose field";
                    hdnBtnText1.value = btn.value;
                    break;
                case "btnText2":
                    ddVal = document.getElementById(ctlPrefix + "ddValField2");
                    txtVal = document.getElementById(ctlPrefix + "txtValue2");
                    ddVal.style.display = "none";
                    txtVal.value = "";
                    txtVal.style.display = "";
                    txtVal.focus();
                    btn.value = "Fields";
                    btn.title = "Choose field";
                    hdnBtnText2.value = btn.value;
                    break;
                case "btnCal1":
                    txtVal = document.getElementById(ctlPrefix + "Date1");
                    document.getElementById(ctlPrefix + "divDateField1").style.display = "none";
                    document.getElementById(ctlPrefix + "divCalendar1").style.display = "";
                    txtVal.value = "";
                    txtVal.focus();
                    btn.value = "Fields";
                    btn.title = "Choose field";
                    hdnBtnCal1.value = btn.value;
                    break;
                case "btnCal2":
                    txtVal = document.getElementById(ctlPrefix + "Date2");
                    document.getElementById(ctlPrefix + "divDateField2").style.display = "none";
                    document.getElementById(ctlPrefix + "divCalendar2").style.display = "";
                    txtVal.value = "";
                    txtVal.focus();
                    btn.value = "Fields";
                    btn.title = "Choose field";
                    hdnBtnCal2.value = btn.value;
                    break;
            }

        }
    }
}