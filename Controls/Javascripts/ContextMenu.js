// xtreeView JScript File



var ie5=document.all&&document.getElementById
var ns6=document.getElementById&&!document.all

var menuobj = document.getElementById("pnlMenu");

function showMenu(e) {
    var selectednode = TreeView1.SelectedNode;
    var pnlBody = document.getElementById(prefix + "pnlBody");
    var targ = e.target;
    var rect = targ.getBoundingClientRect();
    var rect2 = pnlBody.getBoundingClientRect();
    var x = rect.left - rect2.left;
    var y = (rect.top - rect2.top) + rect.height;
    var condData = null

    if (selectednode != null)
        condData = selectednode.ConditionData;

    if (selectednode != null && condData != null && condData.GroupName != "") {
        var optRenameGroup = document.getElementById("RenameGroup");
        var optUngroup = document.getElementById("Ungroup");
        if (selectednode.ChildNodes.length > 0) 
            optUngroup.innerText = "Ungroup";
        else
            optUngroup.innerText = "Delete Group";

        if (selectednode == null) {
            optRenameGroup.style.display = "none";
            optUngroup.style.display = "none";
        }
        else {
            optRenameGroup.style.display = "";
            optUngroup.style.display = "";
        }
        menuobj = document.getElementById("pnlMenu");
        ie5 = document.all && document.getElementById
        ns6 = document.getElementById && !document.all

        menuobj.style.left = x + "px";
        menuobj.style.top = y + "px";


        e.stopPropagation();

        menuobj.style.visibility = "visible"
        menuobj.style.display = '';
        document.onclick = hidemenu;
        return false;
    }


}

 function hidemenu(e){
    menuobj.style.visibility="hidden"
}

function highlight(e){
    var firingobj=e.target
    if (firingobj.className=="menuitems"||firingobj.parentNode.className=="menuitems"){
    if (firingobj.parentNode.className=="menuitems") 
       firingobj=firingobj.parentNode //up one node
    firingobj.style.backgroundColor="highlight"
    }
}

function lowlight(e){
    var firingobj=e.target
    if (firingobj.className=="menuitems"||firingobj.parentNode.className=="menuitems"){
        if (firingobj.parentNode.className=="menuitems") 
            firingobj=firingobj.parentNode //up one node
        firingobj.style.backgroundColor=""
        window.status=''
    }
}
function deleteNode(node) {
    if (node != null) {
        var level = node.Level();
        var index = node.Index();
        var parent
        if (level > 0)
            parent = node.Parent;
        else
            parent = node;
        if (parent == node)
            TreeView1.Nodes.Remove(node);
        else
            parent.ChildNodes.Remove(node);
    }
}
function addNode(node) {
    var newnode = new TreeNode("New Node", "New Node");

    if (node != null) {
        var level = node.Level();
        var index = node.Index();
        var parent
        if (level > 0)
          parent = node.Parent;
        else
          parent = node;
        if (parent == node) {
            TreeView1.Nodes.AddAt(index+1, newnode);
        }
        else {
            parent.ChildNodes.AddAt(parent.Index()+1,newnode);
        }
    }
    else {
        TreeView1.Nodes.Add(newnode);
    }
    TreeView1.EditLabel = true;
    newnode.Select();
}
function jumpto(e){
    var firingobj = e.target
    var node = TreeView1.SelectedNode;
    if (firingobj.className=="menuitems"||firingobj.parentNode.className=="menuitems"){
        if (firingobj.parentNode.className=="menuitems") 
            firingobj = firingobj.parentNode

        if (firingobj != null && firingobj.className == "menuitems") {
            var option = firingobj.textContent;
            switch (option) {
                case "Delete Group":
                case "Ungroup":
                    deleteGroup();
                    break;
                case "Rename Group":
                    node.TreeView().EditLabel = true;
                    node.BeginEdit();
                    break
            }
        }

    }
}


