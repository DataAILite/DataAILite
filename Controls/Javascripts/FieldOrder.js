// ******************************************* FieldOrder *********************************
function FieldOrder() {
    this.fieldOrder = {
        OrderItems: [],
        _init: function() {
            this.OrderItems.AddAt = function (index, item) {
                if (item == void 0 || index.toString().indexOf('-') >= 0)
                    return null;
                if (index > this.length)
                    index = this.length;
                this.splice(index, 0, item);
                return item;
            };
            this.OrderItems.Add = function (item) {
                return this.AddAt(this.length, item);
            };
            this.OrderItems.GetItem = function (itemID) {

                for (var index = 0; index < this.length; index++) {
                    var _item = this[index];
                    if (_item.ItemID == itemID)
                        return _item;
                }
                return null;
            };
            this.OrderItems.Remove = function (item) {
                if (item != void 0) {
                    if (this.length > 0) {
                        var index = 0;

                        for (index = 0; index <= this.length - 1; index++) {
                            var _item = this[index];

                            if (item == _item) {
                                this.splice(index, 1);
                                break;
                            };
                        };
                    };
                    if (this.length > 0) {
                        for (index = 0; index <= this.length - 1; index++) {
                            this[index].ItemOrder = (index + 1)
                        }
                    }
                }
            };
            this.OrderItems.RemoveAt = function (index) {
                if (index != void 0 && index.toString().indexOf('-') == -1 && index <= this.length-1) {
                   var item = this[index];
                   this.Remove(item);
                }
            };
            this.OrderItems.Clear = function () {
                while (this.length > 0)
                    this.RemoveAt(0);
            };
        },
        ToJSON: function () {
            var __fieldOrder = {
                OrderItems: null
            };
            if (this.OrderItems.length > 0) {
                __fieldOrder.OrderItems = [];
                for (var index = 0; index <= this.OrderItems.length - 1; index++)
                    __fieldOrder.OrderItems.push(this.OrderItems[index].ToJSON())
            }
            return __fieldOrder;
        }
    }
    this.fieldOrder._init();
    return this.fieldOrder;
}

FieldOrder.ParseJSON = function (json, fieldOrder) {
    fieldOrder = fieldOrder || new FieldOrder
    if (json.OrderItems != void 0) {
        for (var index = 0; index <= json.OrderItems.length - 1; index++) {
            var _orderItem = OrderItem.ParseJSON(json.OrderItems[index]);
            if (_orderItem != void 0) fieldOrder.OrderItems.push(_orderItem);
        };
    }
}
// ******************************************* OrderItem *******************************
function OrderItem() {
    this.orderItem = {
        Field: '',
        ItemOrder: '0',
        ToJSON: function () {
            var __orderItem = {
                Field: this.Field || "",
                ItemOrder: this.ItemOrder || "0"
            };
            return __orderItem;
        }
    };
    return this.orderItem;
}

OrderItem.ParseJSON = function(json,orderItem) {
    if (json.Field != void 0) {
        orderItem = orderItem || new OrderItem()
        if (json.Field != void 0 && typeof (json.Field) == "string" && json.Field.length > 0)
            orderItem.Field = json.Field;
        if (json.ItemOrder != void 0 && typeof (json.ItemOrder) == "string" && json.ItemOrder.length > 0)
            orderItem.ItemOrder = json.ItemOrder;

        return orderItem;
    }
    return null;
}

//**************************************** Other functions ******************************
var fieldOrder = new FieldOrder();

function populateFieldOrder(data) {
    if (data != null) {
        data = data.replace(/\\/g, "");
        FieldOrder.ParseJSON(JSON.parse(data), fieldOrder);
    }
}

function showColumnOrderDlg() {
    fieldOrder = fieldOrder || new FieldOrder();
    var hdnFieldOrder = document.getElementById("hdnFieldOrder");
    var divColOrderDlgBackground = document.getElementById("divColumnOrderDlgBackground");

    populateFieldOrder(hdnFieldOrder.value);

    divColOrderDlgBackground.style.display = '';
    setEnabled('btnOrderUp', false);
    setEnabled('btnOrderDown', false);
}

function setEnabled(Id, enable) {

    var ctl = document.getElementById(Id);

    if (ctl != null) {
        if (Id == "btnOrderUp") {
            if (enable) {
                ctl.disabled = false;
                ctl.style.backgroundImage = "url(Controls/Images/arrow-up-black.png)";
                ctl.style.borderColor = "#4e4747"
            }
            else {
                ctl.disabled = true;
                ctl.style.backgroundImage = "url(Controls/Images/arrow-up-gray.png)";
                ctl.style.borderColor = "#bfbfbf"
            }
        }
        else if (Id == "btnOrderDown") {
            if (enable) {
                ctl.disabled = false;
                ctl.style.backgroundImage = "url(Controls/Images/arrow-down-black.png)";
                ctl.style.borderColor = "#4e4747"
            }
            else {
                ctl.disabled = true;
                ctl.style.backgroundImage = "url(Controls/Images/arrow-down-gray.png)";
                ctl.style.borderColor = "#bfbfbf"
            }
        }
        else {
            if (enable) {
                ctl.disabled = false;
                ctl.style.color = "black";
            }
            else {
                ctl.disabled = true;
                ctl.style.color = "gray";
            }
        }
    }

}

function saveColumnOrder() {
    var dlg = document.getElementById("divColumnOrderDlgBackground");
    if (dlg != void 0) {
        var innerList = document.getElementById("lstColumns_InnerList");
        var li, oi, order, field

        fieldOrder.OrderItems.Clear()
        for (var i = 0; i < innerList.children.length; i++) {
            li = innerList.children[i];
            field = li.innerText;
            order = li.getAttribute("dragItemTag");
            oi = new OrderItem;
            oi.Field = field;
            oi.ItemOrder = order;
            fieldOrder.OrderItems.Add(oi);
        }
        var json = fieldOrder.ToJSON();
        var jsonFO = JSON.stringify(json);

        dlg.style.display = "none";

        __doPostBack('SaveColumnOrder', jsonFO);
    }

}


function closeColumnOrderDialog() {
    var trgt = event.currentTarget;
    var dlg = document.getElementById("divColumnOrderDlgBackground");
    if (dlg != void 0) 
        dlg.style.display = "none";

    // since this is a div event, have to force a post back so that column list is reset.
    if (trgt.id.indexOf("div")==0)
        __doPostBack(trgt.id, '');
}

function resetListOrder(list) {
    var li
    for (var i = 0; i < list.children.length; i++) {
        li = list.children[i];
        //li.setAttribute("index", i);
        li.setAttribute("dragitemTag",i+1)
    }
}

function onListChanged(ctlId) {
    var e = event
    var trgt = e.currentTarget

    if (trgt.id.indexOf("lstColumns") > -1) {
        var innerList = document.getElementById("lstColumns_InnerList");
        var sel = parseInt(trgt.dataset.selectedindex, 10);
        var len = innerList.children.length;
        resetListOrder(innerList);
        if (sel == 0) {
            setEnabled("btnOrderDown", true);
            setEnabled("btnOrderUp",false)
        }
        else if (sel == (len - 1)) {
            setEnabled("btnOrderDown", false);
            setEnabled("btnOrderUp", true)
        }
        else if (sel != void 0) {
            setEnabled("btnOrderDown", true);
            setEnabled("btnOrderUp", true)
        }
    }
}

function moveUp() {
    var list = document.getElementById("lstColumns");
    var innerList = document.getElementById("lstColumns_InnerList");
    if (innerList.children.length > 0) {
        var len = innerList.children.length;
        var sel = parseInt(list.dataset.selectedindex, 10);
        if (sel != void 0 && sel > 0) {
            var itm = innerList.children[sel];
            var prev = innerList.children[sel-1];
            innerList.insertBefore(itm, prev); //itm before previous
            reindexList(innerList);
            resetListOrder(innerList);
            sel = sel - 1;
            list.setAttribute("data-selectedindex", sel);
            if (sel == 0) {
                setEnabled("btnOrderDown", true);
                setEnabled("btnOrderUp", false)
            }
            else if (sel == (len - 1)) {
                setEnabled("btnOrderDown", false);
                setEnabled("btnOrderUp", true)
            }
            else {
                setEnabled("btnOrderDown", true);
                setEnabled("btnOrderUp", true)
            }
        }
    }
}

function moveDown() {
    var list = document.getElementById("lstColumns");
    var innerList = document.getElementById("lstColumns_InnerList");

    if (innerList.children.length > 0) {
        var len = innerList.children.length;
        var sel = parseInt(list.dataset.selectedindex, 10);
        if (sel != void 0 && sel < (len-1)) {
            var itm = innerList.children[sel];
            var next = innerList.children[sel + 1];
            innerList.insertBefore(next, itm) //itm after next
            reindexList(innerList);
            resetListOrder(innerList);
            sel = sel + 1;
            list.setAttribute("data-selectedindex", sel);
            if (sel == 0) {
                setEnabled("btnOrderDown", true);
                setEnabled("btnOrderUp", false)
            }
            else if (sel == (len - 1)) {
                setEnabled("btnOrderDown", false);
                setEnabled("btnOrderUp", true)
            }
            else {
                setEnabled("btnOrderDown", true);
                setEnabled("btnOrderUp", true)
            }
        }

    }
}