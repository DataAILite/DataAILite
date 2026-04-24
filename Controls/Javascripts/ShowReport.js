function searchReport(e) {
    var ddColumns = document.getElementById("DropDownColumns");
    var ddOperators = document.getElementById("DropDownOperator");
    var txtSearch = document.getElementById("TextBoxSearch");

    if (ddColumns != void 0 && ddOperators != void 0 && txtSearch != void 0) {
        var col = ddColumns.options[ddColumns.selectedIndex].value;
        var oper = ddOperators.options[ddOperators.selectedIndex].value;
        var search = txtSearch.value;
        var target = "DoSearch";
        var data;

        if (col != null && oper != null && search != "") {
            data = col + "~" + oper + "~" + search;
            showSpinner();
            __doPostBack(target, data);

        }
    }

}