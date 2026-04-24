var hdnOpenedvar;

function CloseCalendar(ctlId)
{
    var calendar = document.getElementById(ctlId + "DivCalendar");
    var TodayRow = document.getElementById(ctlId + "TodayRow");

    calendar.style.visibility = "hidden";
    TodayRow.style.visibility = "hidden";

    document.removeEventListener('click', CloseCalendarFromOutside);
}
function CloseCalendarFromOutside(ctlId)
{
    try {
        var targid = event.target.id;
        var title = event.target.title;
    }
    catch (err) {
        return;
    }

    if (targid.indexOf(ctlId)==0)
        return;

    var id = ctlId + "hdnOpened";
    hdnOpenedvar = document.getElementById(id);

    if ((targid != "" || (targid=="" && title==""))&& hdnOpenedvar != null && hdnOpenedvar.value == "open") {
        CloseCalendar(ctlId);
        hdnOpenedvar.value = "closed";
        
    }
}
function AddListener(ctlId)
{
    if (document.addEventListener)
    document.addEventListener('click', function () { eval("CloseCalendarFromOutside('" + ctlId + "');") }, false)
}

function RemoveListener()
{
    document.removeEventListener('click', CloseCalendarFromOutside);
}
function OpenCalendarDropDown(ctlId)
{
    
    var id = ctlId + "hdnOpened";
    var hdnOpenedvar = document.getElementById(id);
    var calendar = document.getElementById(ctlId + "DivCalendar");
    var TodayRow = document.getElementById(ctlId + "TodayRow");
 
    if (hdnOpenedvar!=null && hdnOpenedvar.value=="closed")
    {
        calendar.style.visibility = "visible";
        TodayRow.style.visibility = "visible";
        //hdnOpenedvar.value = "open";
        //__doPostBack(ctlId, "tdButton_Click");
        /* Attach 'onclick' event to parent document to collapse List when clicked on Document */
        if (document.addEventListener)
            document.addEventListener('click', function () { eval("CloseCalendarFromOutside('" + ctlId + "');") }, false)
        hdnOpenedvar.value = "open";
    }
    else 
    {
        CloseCalendarDropDown(ctlId);
        hdnOpenedvar.value = "closed";
    }
}

function CloseCalendarDropDown(ctlId)
{
    CloseCalendar(ctlId);
    //__doPostBack(ctlId, "tdButton_Click");
}

function txtDateKeyDown(e,ctlId)
{
    
    var evtobj = window.event ? event : e; //distinguish between IE's explicit event object (window.event) and Firefox's implicit.
    var keyEntry = evtobj.charCode ? evtobj.charCode : evtobj.keyCode;
    //alert(keyEntry);
    if (keyEntry != null && (keyEntry==9||keyEntry==191)) {
        var id = ctlId + "hdnOpened";
        hdnOpenedvar = document.getElementById(id);
        if (hdnOpenedvar != null)
        {
            if ( keyEntry==9 && hdnOpenedvar.value == "open") {
                CloseCalendarDropDown(ctlId);
            }
            else if (keyEntry == 191) {
                var txt = document.getElementById(ctlId + "txtDate");
                if (txt.value=="") {
                    evtobj.preventDefault();
                    if (hdnOpenedvar.value=="closed")
                        OpenCalendarDropDown(ctlId);
                }
           }
        }
    }
    return true;
}