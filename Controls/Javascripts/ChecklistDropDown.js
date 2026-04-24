		var arrayOfCheckBoxes;
		var arrayOfCheckBoxLabels;
		var rows;
		var lastPressedKey = null;
		var lastSelectedIdx = null;
	    var rowHeight = 25;
		var minlistHeight = 2;
		var hdnFieldvar;
		var justopened = false;
		var listenercreated = false;

		function AdjustScroll(ctlId) {
		    var hdnScrollTop = document.getElementById(ctlId + "hdnScrollTop");
		    var divCheckList = document.getElementById(ctlId + "DivChecklist");
		    var hdnDropDown = document.getElementById(ctlId + "hdnDropDown");
		    if (divCheckList != void 0 && hdnDropDown != void 0) {
		        if (hdnDropDown.value == "closed" && divCheckList.className == "DropDownOpened")
		            CloseListBox(ctlId);
		    }

            if (hdnScrollTop != void 0)
		        divCheckList.scrollTop = hdnScrollTop.value;

		}

	function CheckListChanged(e, ctlId) {
		var txtValue = document.getElementById(ctlId + "txtValue");
	    var chklist = document.getElementById(ctlId + "Checklist");
	    var divChecklist = document.getElementById(ctlId + "DivChecklist");
	    var hdnSelectedData = document.getElementById(ctlId + "hdnSelectedData");
	    var hdnScrollTop = document.getElementById(ctlId + "hdnScrollTop");
	    var hdnAutoPostBack = document.getElementById(ctlId + "hdnAutoPostBack");
	    var hdnPostBackType = document.getElementById(ctlId + "hdnPostBackType");
		var arCheckBoxes = chklist.getElementsByTagName("input");
		//var arLabels = chklist.getElementsByTagName("label");
		var targ = e.target;
		var target;
		var data;
		var itemDesc = "";
		var itemChecked = "";
		var allChecked = "true";
		var noneChecked = "true";
		var scrollTop = ScrollTop(chklist);

		var AutoPostBack = hdnAutoPostBack.value == "1" ? true : false;

		var PostBackType = hdnPostBackType.value;
		var evt = document.createEvent("HTMLEvents"); //=new Event("change",{"bubbles":true,"cancelable":true})
		evt.initEvent("change", true, true);

                    // determine if all or none checked
		            for (var i = 0; i < arCheckBoxes.length; i++) {
		                if (arCheckBoxes[i].checked)
		                    noneChecked = "false";
		                else
		                    allChecked = "false";
		            }
		if (!AutoPostBack && PostBackType == "2") {
		    if (allChecked=="true")
		        txtValue.value = "ALL";
		    else if (noneChecked=="true")
		        txtValue.value = "Please select...";
		}

       if (targ.tagName == "INPUT" && targ.type == "checkbox") {
		        var parent = targ.parentElement;
		        if (parent != void 0 && parent.tagName == "TD") {
		            itemDesc = parent.children[1].textContent;
		            itemChecked = parent.children[0].checked ? "true" : "false";
		            target = ctlId + "CheckItemChanged"
		            data = itemDesc + "~" + itemChecked + "~" + allChecked + "~" + noneChecked + "~" + scrollTop;
		            hdnSelectedData.value = data;
		            hdnScrollTop.value = scrollTop;

                    
		            chklist.setAttribute("data-itemdescription", itemDesc);
		            chklist.setAttribute("data-itemchecked", itemChecked);
		            chklist.setAttribute("data-allchecked", allChecked);
		            chklist.setAttribute("data-nonechecked", noneChecked);
		        }
       }
       else if (!AutoPostBack) {
           itemDesc = "CheckListDropDown Closed";
           itemChecked = false;
           target = ctlId + "CheckItemChanged"
           data = itemDesc + "~" + itemChecked + "~" + allChecked + "~" + noneChecked + "~" + scrollTop;
           hdnSelectedData.value = data;
           hdnScrollTop.value = scrollTop;
           chklist.setAttribute("data-itemdescription", itemDesc);
           chklist.setAttribute("data-itemchecked", itemChecked);
           chklist.setAttribute("data-allchecked", allChecked);
           chklist.setAttribute("data-nonechecked", noneChecked);

           chklist.dispatchEvent(evt);
           if (PostBackType == "1")
           __doPostBack(target, data);


		    }
    }
	/* Gets the selected items and assigns to a Text Box */
	function CheckedIndexChanged(eventsrc,ctlId)
	{	
		var txtValue = document.getElementById(ctlId+"txtValue");
		var chklist = document.getElementById(ctlId + "Checklist");
		hdnText = document.getElementById(ctlId+"hdnText");
		/* get the Check box items and corresponding lables  */
		arrayOfCheckBoxes = chklist.getElementsByTagName("input");
		arrayOfCheckBoxLabels = chklist.getElementsByTagName("label");
		rows = chklist.getElementsByTagName("tr");
		
		var strSelectedItems = '';
		
		var chkBoxAll;
		
		// to handle rowclicked events
		if(eventsrc.srcElement.tagName == 'TD')
		{
			chkBoxAll = document.getElementById(eventsrc.srcElement.firstChild.id);
		}
		// To handle checkbox change events
		if (eventsrc.srcElement != null && eventsrc.srcElement.id.length > 0)
		{
			chkBoxAll = document.getElementById(eventsrc.srcElement.id);
		}			
	
		if(arrayOfCheckBoxes.length > 0 )
		{
			if(chkBoxAll != null && chkBoxAll.id == arrayOfCheckBoxes[0].id && arrayOfCheckBoxLabels[0].innerText.toUpperCase() == 'ALL')
			{
				if(chkBoxAll.checked)
				{
					/* loop through each items and check all the items */
					for(var i=0;i<arrayOfCheckBoxes.length;i++)
					{
						arrayOfCheckBoxes[i].checked = 'checked';
						// To remove the 'All' text from the text box
						if( arrayOfCheckBoxLabels[i].innerText.toUpperCase() != 'ALL')
						strSelectedItems += arrayOfCheckBoxLabels[i].innerText+',';
					}
				}
				else
				{
					/* loop through each items and uncheck all the items */
					for(var i=0;i<arrayOfCheckBoxes.length;i++)
					{
						arrayOfCheckBoxes[i].checked = false;
						strSelectedItems = '';
					}
				}
			}
			else
			{			
				var checkAll = true;	
				/* loop through each items and select the checked items */
				/* the first item 'ALL' is excluded for checking */
				for(var i=0;i<arrayOfCheckBoxes.length;i++)
				{
					if(arrayOfCheckBoxLabels[i].innerText.toUpperCase() != 'ALL')
					{
						if(arrayOfCheckBoxes[i].checked)
						{
							strSelectedItems += arrayOfCheckBoxLabels[i].innerText+',';
						}
						else
							checkAll = false;	
					}
				}
				
				//Set the 'ALL' checkbox to unchecked/checked
				if(arrayOfCheckBoxLabels[0].innerText.toUpperCase() == 'ALL')
					{
				arrayOfCheckBoxes[0].checked = checkAll;
			}
			}
					
			if (strSelectedItems.length > 0)
				strSelectedItems = strSelectedItems.substring(0,strSelectedItems.length-1);
			else
				strSelectedItems = 'Please select...';
		    
			/* assign the selected text to a Text Box */
			txtValue.title = strSelectedItems;
			if(arrayOfCheckBoxes[0].checked && arrayOfCheckBoxLabels[0].innerText.toUpperCase() == 'ALL')
				txtValue.value = 'ALL';
			else
				txtValue.value = strSelectedItems;
				
			txtValue.title = strSelectedItems;
			txtValue.tooltip = strSelectedItems;
			if (strSelectedItems != "Please select...")
			    hdnText.value = strSelectedItems;
			else
			    hdnText.value = "";
		  //alert("X :" +event.clientX  + "  Y:" + event.clientY);
		 
			if (eventsrc.stopPropagation) { eventsrc.stopPropagation(); }
			CheckListChanged(eventsrc, ctlId);
		}
	}    
	
	/* Opens the DropDown List */
	function OpenCheckListBox(ctlId)
	{
	    var hdnDropDown = document.getElementById(ctlId + "hdnDropDown");
	    var list = document.getElementById(ctlId + "DivChecklist");
	    var btn = document.getElementById(ctlId + "btnDropDown");
	    //var e = event;

	    if (hdnDropDown != null) {  //&& hdnDropDown.value == "closed") {
	        if (hdnDropDown.value == "closed") {
                list.className = "DropDownOpened"
	            list.focus();
	            btn.className = "ButtonUpStyle";
	            hdnDropDown.value = "open";
	            if (!listenercreated) {
	                listenercreated = true;
	                if (document.addEventListener)
	                    document.addEventListener('click', function () { eval("Collapse('" + ctlId + "');") },true)
	            }

	            //CallServer(ctlId,"open", "");
	        }
	        else {
	            CloseListBox(ctlId);
	        }
	    }
        //var id=ctlId+"DivChecklist"
	    //var list = document.getElementById(id);
	    //if (list != null && list.className == "CheckboxListClosed") {
	    //    var btn = document.getElementById(ctlId + "btnDD");
	    //    var chklist = document.getElementById(ctlId + "Checklist")
	    //    var txtval = document.getElementById(ctlId + "txtValue")
        //    btn.background="Images\\DDImageUp.bmp"
	    //    list.className="CheckboxListOpen";
	    //    list.focus();

	    //    if (chklist != null) {
	    //        if (chklist.rows != null && chklist.rows.length > 0) {
	    //            var maxDDHeight = chklist.rows.length * rowHeight;
	    //            if (maxDDHeight < 330) {
	    //                list.style.height = maxDDHeight;
	    //                list.style.overflow = 'hidden';
	    //            }

	    //        }
	    //        else if (chklist.innerText.length > 0) {
	    //            //if the repeat direction is "flow"
	    //            var itemCount = chklist.innerText.split("\n");
	    //            if (itemCount.length > 0) {
	    //                list.style.height = itemCount.length * rowHeight;
	    //                list.style.overflow = 'hidden';
	    //            }
	    //        }

	    //        if (chklist.rows == null || chklist.innerText.length == 0) {
	    //            list.style.height = minlistHeight;
	    //        }
	    //    }
	    //    else {
	    //        list.style.height = minlistHeight;
	    //    }
	    //    justopened = true;
	    //    ///* Attach 'onclick' event to parent document to collapse List when clicked on Document */
        //    //// only create one listener
	    //    //if (!listenercreated)
	    //    //{
	    //    //    listenercreated=true;
	    //    //    // all browsers except IE before version 9
	    //    //    if (document.addEventListener)
	    //    //        document.addEventListener('click', function () { eval("Collapse('" + ctlId + "');") })
	    //    //    // IE before version 9
	    //    //    if (document.attachEvent)
	    //    //        document.attachEvent('onclick', function () { eval("Collapse('" + ctlId + "');") });
	    //    //}
	    //    //__doPostBack(ctlId, "opened");
            
	    //}
	    //else {
	    //    CloseListBox(ctlId);
	    //}

	}
	//function CallServer(ctlId,arg,context)
	//{
	//    var idx = ctlId.indexOf("_");
	//    var id = ctlId.substring(0, idx);
	//    WebForm_DoCallback(id, arg, ReceiveServerData, context, false);
	//}
	function ReceiveServerData(rValue) {

	}
    /* Hides the DropDown List */
	function CloseListBox(ctlId)
	{
	    var e = event;
	    var hdnDropDown = document.getElementById(ctlId + "hdnDropDown");
	    var list = document.getElementById(ctlId + "DivChecklist");
	    var btn = document.getElementById(ctlId + "btnDropDown");

	    list.className = "DropDownClosed";
	    btn.className = "ButtonDownStyle";
	    hdnDropDown.value = "closed";
	    document.removeEventListener('click', Collapse(ctlId), true);
	    listenercreated = false;
	    CheckListChanged(e,ctlId);
	    //CallServer("closed", "");
	    //__doPostBack(ctlId, "closed");
	}
	
	//Highlight Item on MouseOver
	function ChangeColor(idx,ctlId)
	{
		var arrayOfItems = document.getElementById(ctlId+'MSDDLB_cblst').getElementsByTagName("tr");
		var arrayOfchkbox = document.getElementById(ctlId+'MSDDLB_cblst').getElementsByTagName("input");
		
		//Unselect the item when selected on keypress
		if(lastSelectedIdx != null && lastSelectedIdx < arrayOfItems.length)
		{
			arrayOfItems[lastSelectedIdx].className = 'ItemUnSelected';	
			arrayOfchkbox[lastSelectedIdx].className = 'ItemUnSelected';
		}
			
			if(arrayOfItems[idx].className == 'ItemUnSelected'){
				arrayOfItems[idx].className = 'ItemSelected';
				arrayOfchkbox[idx].className = 'ItemSelected'; }
			else{
				arrayOfItems[idx].className = 'ItemUnSelected';	
				arrayOfchkbox[idx].className = 'ItemUnSelected';}
		
	}
	
	/* Closes drop down list when clicked outside List */
	function Collapse(ctlId)
    {
	    try {
	        var targid = event.target.id;
	    }
	    catch (err) {
	        return;
	    }
	    if (targid.indexOf(ctlId) == 0)
	        return;
	    var hdnDropDown = document.getElementById(ctlId + "hdnDropDown")
	    if (hdnDropDown!=null && hdnDropDown.value=="open") {
	        CloseListBox(ctlId);
	    }
	//{
	//    if (justopened)
	//    {
	//        justopened = false;
	//        return;
	//    }
	//    //document.removeEventListener('click', function () { eval("Collapse('" + ctlId + "');") });
	//    var list = document.getElementById(ctlId + "DivChecklist");
		
	//    if (list.className == "CheckboxListOpen")
	//	{ 
	//        var txt = document.getElementById(ctlId + "tblDD");
	//        var txtheight = 0;
	//        if (txt != null)
	//            txtheight = txt.style.height;
	//		//get mouse X & Y positions 
	//		var mX = event.clientX;
	//			mX += ScrollLeft(list);
	//		var mY = event.clientY; 
	//			mY += ScrollTop(list);
	//		//get Left anf Top positions
	//		var listLeft = Left(list);
	//		var listWidth = list.offsetWidth;
	//		var listTop = Top(list) - parseInt(txtheight);
	//		var listHeight = list.offsetHeight + parseInt(txtheight);
			
	//		if ((mX != listLeft) && (mX < listLeft ||(mX > (listLeft + listWidth)) ))
	//			CloseListBox(ctlId)
	//		else if ((mY != listTop) && (mY < listTop ||(mY > (listTop + listHeight)) ))
	//			CloseListBox(ctlId)
	//	}
	}
	function OnKeyPress(ctlId,e)
	{			
		var evtobj=window.event? event : e; //distinguish between IE's explicit event object (window.event) and Firefox's implicit.
		var unicode=evtobj.charCode? evtobj.charCode : evtobj.keyCode;
		var actualkey=String.fromCharCode(unicode);
		actualkey = actualkey.toUpperCase();
		
		var arrayOfRows = document.getElementById(ctlId+'MSDDLB_cblst').getElementsByTagName("TR");
		var arrayOfchkbox = document.getElementById(ctlId+'MSDDLB_cblst').getElementsByTagName("input");		
		var divList = document.getElementById(ctlId+"MSDDLB_div");
		
		var i = 0;
		var scrollTop = null;
		while(i<arrayOfRows.length)
		{
			lbl = arrayOfRows[i].getElementsByTagName("label");			
			if(lbl[0].innerText.substring(0,1).toUpperCase() == actualkey)
			{
				scrollTop = i * (divList.scrollHeight/arrayOfRows.length);	
				if((actualkey == lastPressedKey && divList.scrollTop < parseInt(scrollTop,10)) ||
				   (actualkey != lastPressedKey && divList.scrollTop != parseInt(scrollTop,10)))
				{
					divList.scrollTop = scrollTop;
							
					arrayOfRows[i].className = 'ItemSelected';	
					arrayOfchkbox[i].className = 'ItemSelected';	
					
					if(lastSelectedIdx != null && lastSelectedIdx < arrayOfRows.length)
					{
						arrayOfRows[lastSelectedIdx].className = 'ItemUnSelected';	
						arrayOfchkbox[lastSelectedIdx].className = 'ItemUnSelected';	
					}
						
					lastPressedKey = actualkey;
					lastSelectedIdx = i;
					break;
				}
			}
			i += 1;
		}
	}
	
	//Returns 'Left' position of the given Object  
	function Left(obj)
	{
		var curleft = 0;
		if (obj.parentElement!= null)
		{
			while (obj.parentElement!= null)
			{
				curleft += obj.parentElement.offsetLeft;
				if(obj.parentElement.tagName.toUpperCase() == "TABLE")
				    {
				        curleft -= Number(obj.parentElement.getAttribute('cellspacing'));
				        //curleft -= Number(obj.parentElement.getAttribute('cellpadding'));
			        }
				obj = obj.parentElement;
			}
		}
		else if (obj.offsetLeft)
			curleft += obj.offsetLeft;
		return curleft;
	}
	
	//Returns 'Top' position of the given Object
	function Top(obj)
	{
		var curtop = 0;
		if (obj.offsetParent)
		{
			while (obj.offsetParent)
			{
				curtop += obj.offsetTop
				obj = obj.offsetParent;
			}
		}
		else if (obj.offsetTop)
			curtop += obj.offsetTop;
		return curtop;
	}


	//On Row clicked 
	function RowClicked(eventsrc,id,ctlid)
	{
		if(eventsrc.srcElement.id.length == 0 && eventsrc.srcElement.tagName.toUpperCase() == 'TD')
		{
			var chkBox = document.getElementById(ctlid+'MSDDLB_cblst_'+id);
				if(chkBox.checked)
					chkBox.checked = false;
				else
					chkBox.checked = 'checked';
		}
	}
	
    //Returns 'Scroll Left' position of the given Object  
	function ScrollLeft(obj)
	{
		var curleft = 0;
		if (obj.parentElement!= null)
		{
			//obj = obj.parentElement;
			while (obj.parentElement!= null)
			{
			    if(obj.scrollLeft >0)
				    curleft += obj.scrollLeft;
				obj = obj.parentElement;
			}
		}
		else if (obj.scrollLeft)
			curleft += obj.scrollLeft;
		return curleft;
	}
	
    //Returns 'Scroll Top' position of the given Object
	function ScrollTop(obj)
	{
		var curtop = 0;
		obj = obj.offsetParent;
		
		if (obj == void 0)
		    return curtop;

		if (obj.offsetParent)
		{
			while (obj.offsetParent)
			{
			    if(obj.scrollTop >0)
				    curtop += obj.scrollTop;
				obj = obj.offsetParent;
			}
		}
		else if (obj.scrollTop != void 0)
			curtop += obj.scrollTop;
		return curtop;
	}