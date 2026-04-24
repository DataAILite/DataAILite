function operatorChanged(ctlId) {
    var rb = document.getElementById(prefix + ctlId);
    var node = TreeView1.SelectedNode;
    var logop,cond;

    if (node != null && node.ConditionData==null) {
        if (rb.checked) {
            if (ctlId == "rbAnd")
                logop="And"
            else
                logop = "Or"
            var nodes = node.Parent != void 0 ? node.Parent.ChildNodes : node.TreeView().Nodes;
            for (var i = 0; i < nodes.length; i++) {
                node = nodes[i];
                cond=node.ConditionData;
                if (cond != null)
                    cond.LogicalOperator = logop;
                else
                    node.SetText(logop);
            }
        }
    }

}

function setEnabled(Id, enable) {

    var ctl = document.getElementById(prefix + Id);

    if (ctl != null) {
        if (Id == "btnUp") {
            if (enable) {
                ctl.disabled = false;
                ctl.style.backgroundImage = "url(Controls/Images/arrow-up-black.png)";
            }
            else {
                ctl.disabled = true;
                ctl.style.backgroundImage = "url(Controls/Images/arrow-up-gray.png)";
            }
        }
        else if (Id == "btnDown") {
            if (enable) {
                ctl.disabled = false;
                ctl.style.backgroundImage = "url(Controls/Images/arrow-down-black.png)";
            }
            else {
                ctl.disabled = true;
                ctl.style.backgroundImage = "url(Controls/Images/arrow-down-gray.png)";
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

function selectedNodeChanged(event) {
    var node = TreeView1.SelectedNode;

    if (node != null) {
        var rbAnd = document.getElementById(prefix + "rbAnd");
        var rbOr = document.getElementById(prefix + "rbOr")
        var btnUp = document.getElementById(prefix + "btnUp");
        var btnDown = document.getElementById(prefix + "btnDown");
        var btnUngroup = document.getElementById(prefix + "btnUngroup");
        var btnAddGroup = document.getElementById(prefix + "btnAddGroup");
        var hdnConditionCount = document.getElementById(prefix + "hdnConditionCount");
        var nConditions = parseInt(hdnConditionCount.value);
        var cond = node.ConditionData;
        var prevNode = getPrevNode(node);
        var nextNode = getNextNode(node);

        if (cond == null) {
            var nodeText = node.Text;

            if (rbAnd != null && rbOr != null) {
                setEnabled("rbAnd", true);
                setEnabled("rbOr", true);

                if (nodeText == "And") {
                    rbAnd.checked = true;
                    rbOr.checked = false;
                }
                else if (nodeText == "Or") {
                    rbAnd.checked = false;
                    rbOr.checked = true;
                }

                if (nConditions > 2) {
                    setEnabled("btnUp", false);
                    setEnabled("btnDown", false);
                    setEnabled("btnUngroup", false);
                    btnUngroup.value = "Ungroup";
                }
            }
        }
        else {
            if (nConditions > 2) {
                setEnabled("btnUp", true);
                setEnabled("btnDown", true);
                setEnabled("btnUngroup", false);

                if (prevNode == null)
                    setEnabled("btnUp", false);
                if (nextNode == null && cond.ContainedBy == "")
                    setEnabled("btnDown", false);
                if (cond.GroupName != "") {
                    setEnabled("btnUngroup", true);
                    if (node.ChildNodes != null && node.ChildNodes.length > 0)
                        btnUngroup.value = "Ungroup";
                    else
                        btnUngroup.value = "Delete Group";
                }
            }
            rbAnd.checked = false;
            rbOr.checked = false;
            setEnabled("rbAnd", false);
            setEnabled("rbOr", false);
         }
    }
}

function moveNodeUp(fromNode, toNode) {

    if (fromNode != null && toNode != null) {
        var n = TreeView1.GetNodeCount(false);
        var tnCopy = fromNode.Copy();
        var ntn, node, cond, condFrom, condTo, op;
        var exit = false;
        var nextNode = fromNode.NextNode(); // logical operator node

        if (nextNode!=null && nextNode.Parent != fromNode.Parent)
            nextNode = null;

        condFrom = fromNode.ConditionData;
        condTo = toNode.ConditionData;
        if (condFrom != null & condTo != null) {
            //if (condFrom.GroupName == "" && condTo.GroupName != "" && condFrom.ContainedBy != condTo.GroupName)
            if (condFrom.GroupName == "" && condTo.GroupName != "") {
                if (condFrom.ContainedBy == "") {
                    op = "And"
                    if (toNode.ChildNodes != null && toNode.ChildNodes.length > 0) {
                        ntn = new TreeNode();
                        node = toNode.ChildNodes[toNode.ChildNodes.length - 1];
                        cond = node.ConditionData;
                        op = cond.LogicalOperator;
                        ntn.SetText(op);
                        ntn.SetDraggable(false);
                        ntn.SetTextColor("ForestGreen");
                        toNode.ChildNodes.Add(ntn)
                    }
                    cond = tnCopy.ConditionData;
                    cond.ContainedBy = condTo.GroupName;
                    cond.LogicalOperator = op;
                    toNode.ChildNodes.Add(tnCopy);
                    tnCopy.Select(true);
                    toNode.Expand();
                }
                else {
                    cond = tnCopy.ConditionData;
                    cond.ContainedBy = "";
                    n = toNode.Index();
                    TreeView1.Nodes.AddAt(n, tnCopy);
                    tnCopy.Select(true);
                    ntn = new TreeNode();
                    ntn.SetDraggable(false);
                    ntn.SetText(condTo.LogicalOperator);
                    ntn.SetTextColor("ForestGreen");
                    TreeView1.Nodes.AddAt(n + 1, ntn);
            }
            }
            else if (condFrom.GroupName != "" && condTo.ContainedBy == "" &&
                     condTo.GroupName == "") {
                n = toNode.Index();
                TreeView1.Nodes.AddAt(n, tnCopy);
                tnCopy.Select(true);
                tnCopy.Expand();
                ntn = new TreeNode();
                ntn.SetDraggable(false);
                ntn.SetText(condFrom.LogicalOperator);
                ntn.SetTextColor("ForestGreen");
                TreeView1.Nodes.AddAt(n + 1, ntn);
            }
            else if ((condFrom.GroupName=="" && condFrom.ContainedBy=="" && condTo.GroupName=="" && condTo.ContainedBy=="") ||
                     (condFrom.GroupName != "" && condTo.GroupName != "") ||
                     (condFrom.ContainedBy !="" && condTo.GroupName=="" && condTo.ContainedBy=="") ||
                     (condFrom.ContainedBy == "" && condFrom.GroupName == "" && condTo.ContainedBy != "")) {
                cond = tnCopy.ConditionData;
                cond.ContainedBy = condTo.ContainedBy;
                cond.LogicalOperator = condTo.LogicalOperator;
                n = toNode.Index();
                node = toNode.Parent;
                if (node == null) {
                    TreeView1.Nodes.AddAt(n, tnCopy);
                }
                else {
                    node.ChildNodes.AddAt(n, tnCopy);
                }
                tnCopy.Select(true);
                if (condFrom.GroupName != "")
                    tnCopy.Expand();
                ntn = new TreeNode();
                ntn.SetDraggable(false);
                ntn.SetText(condTo.LogicalOperator);
                ntn.SetTextColor("ForestGreen");
                if (node == null) {
                    TreeView1.Nodes.AddAt(n + 1, ntn);
                }
                else {
                    node.ChildNodes.AddAt(n + 1, ntn);
                }
            }
            else {
                exit = true;
            }
            if (!exit) {
                node = fromNode.Parent;
                var parent = node;
                if (node == null)
                    TreeView1.Nodes.Remove(fromNode);
                else
                    node.ChildNodes.Remove(fromNode);

                if (nextNode != null) {
                    if (node==null)
                        TreeView1.Nodes.Remove(nextNode);
                    else
                        node.ChildNodes.Remove(nextNode);
                }
                    
                if (node == null) {
                    node = TreeView1.Nodes[TreeView1.Nodes.length - 1];
                }
                else if (node.ChildNodes != null && node.ChildNodes.length > 0) {
                    node = node.ChildNodes[node.ChildNodes.length - 1];
                }
                else
                    node = null;
                if (node != null && node.ConditionData == null)
                    if (parent == null)
                        TreeView1.Nodes.Remove(node);
                    else
                        parent.ChildNodes.Remove(node);
            }
        }
    }
}

function moveUp() {
    var node = TreeView1.SelectedNode;
    //var btnUp = document.getElementById(prefix + "btnUp");
    //var btnDown = document.getElementById(prefix + "btnDown");
    var prevNode, nextNode, treeNodes, cond, prevCond, tnCopy;
    if (node != null) {
        prevNode = getPrevNode(node);
        nextNode = node.NextNode();
        treeNodeCollection = node.Parent != void 0 ? node.Parent.ChildNodes : node.TreeView().Nodes;
        tnCopy = node.Copy();
        cond = node.ConditionData;

        if (prevNode != null) {
            prevCond = prevNode.ConditionData;
            if (prevCond.GroupName != "" && cond.ContainedBy == prevCond.GroupName) {
                var n = prevNode.Index();
                var ntn = new TreeNode();
                var op = prevCond.LogicalOperator;
                cond = tnCopy.ConditionData;
                cond.LogicalOperator = op;
                cond.ContainedBy = "";
                TreeView1.Nodes.AddAt(n, tnCopy);
                ntn.SetDraggable(false);
                ntn.SetText(op);
                ntn.SetTextColor("ForestGreen");
                TreeView1.Nodes.AddAt(n + 1, ntn);
                prevNode.ChildNodes.Remove(node);
                if (nextNode != null)
                    prevNode.ChildNodes.Remove(nextNode);
                tnCopy.Select(true);
                if (cond.GroupName != "")
                    tnCopy.Expand();
            }
            else {
                moveNodeUp(node, prevNode);
            }
        }

    }

}

function moveNodeDown(fromNode, toNode) {

    if (fromNode != null && toNode != null) {
        var n = TreeView1.GetNodeCount(false);
        var nextNode = fromNode.NextNode();
        var tnCopy = fromNode.Copy();
        var condFrom = fromNode.ConditionData;
        var condTo = toNode.ConditionData;
        var exit = false;
        var ntn, node, cond, op, nxtnode;

        if (condFrom != null && condTo != null) {

            if (nextNode != null && nextNode.Parent != fromNode.Parent)
                nextNode = null;

            if (condFrom.GroupName == "" &&
                condTo.GroupName != "" &&
                condFrom.ContainedBy != condTo.GroupName) {

                if (condFrom.ContainedBy == "") {
                    op = condTo.LogicalOperator;
                    if (toNode.ChildNodes != null && toNode.ChildNodes.length > 0) {
                        ntn = new TreeNode();
                        ntn.SetDraggable(false);
                        node = toNode.ChildNodes[toNode.ChildNodes.length - 1];
                        cond = node.ConditionData;
                        op = cond.LogicalOperator;
                        ntn.SetDraggable(false);
                        ntn.SetText(op);
                        ntn.SetTextColor("ForestGreen");
                        toNode.ChildNodes.Add(ntn);
                    }
                    
                    cond = tnCopy.ConditionData;
                    cond.ContainedBy = condTo.GroupName;
                    cond.LogicalOperator = op;
                    toNode.ChildNodes.Add(tnCopy);
                    tnCopy.Select(true);
                    toNode.Expand();
                }
                else {
                    cond = tnCopy.ConditionData;
                    cond.ContainedBy = "";
                    n = toNode.Index();
                    TreeView1.Nodes.AddAt(n, tnCopy);
                    tnCopy.Select(true);

                    ntn = new TreeNode();
                    ntn.SetDraggable(false);
                    ntn.SetText = condTo.LogicalOperator;
                    ntn.SetTextColor("ForestGreen");
                    TreeView1.Nodes.AddAt(n + 1, ntn);
                }
            }
            else if ((condFrom.ContainedBy == "" && condTo.ContainedBy == "" &&
                     condTo.GroupName == "") ||
                     (condFrom.GroupName != "" && condTo.GroupName != "") ||
                     (condFrom.ContainedBy != "" && condTo.ContainedBy != "") ||
                     (condFrom.ContainedBy != "" && condTo.GroupName == "" && condTo.ContainedBy == "") ||
                     (condFrom.ContainedBy == "" && condFrom.GroupName == "" && condTo.ContainedBy != "")) {

                if (condFrom.ContainedBy != "" && condTo.Containedby == "" && condTo.GroupName == "")
                    tnCopy.ConditionData.ContainedBy = "";

                if (condTo.ContainedBy != "") {
                    if (condFrom.ContainedBy != "") {
                    nxtnode = toNode.NextNode();
                    if (nxtnode != null)
                        nxtnode = getNextNode(toNode);
                }
                else
                        nxtnode = null;
                }
                else
                    nxtnode = getNextNode(toNode);

                if (nxtnode == null) {
                    ntn = new TreeNode();
                    ntn.SetDraggable(false);
                    ntn.SetText(condTo.LogicalOperator);
                    ntn.SetTextColor("ForestGreen");
                    if (condFrom.ContainedBy == "" && condFrom.GroupName == "" && condTo.ContainedBy != "") {
                        tnCopy.ConditionData.ContainedBy = condTo.ContainedBy;
                        n = toNode.Index();
                        toNode.Parent.ChildNodes.AddAt(n, ntn);
                        toNode.Parent.ChildNodes.AddAt(n , tnCopy);
                    }
                    else {
                    if (toNode.Parent == null) {
                        TreeView1.Nodes.Add(ntn);
                        TreeView1.Nodes.Add(tnCopy);
                    }
                    else {
                        toNode.Parent.ChildNodes.Add(ntn);
                        toNode.Parent.ChildNodes.Add(tnCopy);
                    }
                    if (condFrom.GroupName != "")
                        tnCopy.Expand();
                    }
                    tnCopy.Select(true);
                }
                else {
                    n = nxtnode.Index();
                    if (toNode.Parent == null)
                        TreeView1.Nodes.AddAt(n, tnCopy);
                    else
                        toNode.Parent.ChildNodes.AddAt(n, tnCopy);
                    tnCopy.Select(true);
                    if (condFrom.GroupName != "")
                        tnCopy.Expand();
                    ntn = new TreeNode();
                    ntn.SetDraggable(false);
                    ntn.SetText(condFrom.LogicalOperator);
                    ntn.SetTextColor("ForestGreen");
                    if (toNode.Parent == null)
                        TreeView1.Nodes.AddAt(n + 1, ntn);
                    else
                        toNode.Parent.ChildNodes.AddAt(n + 1, ntn);
            }

            }
            else
                exit = true;

            if (!exit) {
                // remove fromNode and it's logical operator node
                node = fromNode.Parent;
                var parent=node;

                if (node == null) {
                    TreeView1.Nodes.Remove(fromNode);
                    if (nextNode != null)
                        TreeView1.Nodes.Remove(nextNode);
                    node = TreeView1.Nodes[TreeView1.Nodes.length - 1];
                }
                else {
                    node.ChildNodes.Remove(fromNode);
                    if (nextNode != null && node.ChildNodes.length>0) 
                        node.ChildNodes.Remove(nextNode);
                    if (node.ChildNodes != null && node.ChildNodes.length>0)
                        node = node.ChildNodes[node.ChildNodes.length - 1];
                    else
                        node=null;
                }
                // remove logical operator hanging at end
                if (node != null && node.ConditionData == null) {
                    if (parent == null)
                        TreeView1.Nodes.Remove(node);
                    else
                        parent.ChildNodes.Remove(node);
                }
            }
        }

    }
}

function getPrevNode(node) {
    var prevNode = node;
    var lastNode = null;
    do {
        lastNode = prevNode;
        prevNode = prevNode.PrevNode();
        if (prevNode == null && lastNode != null)
            prevNode = lastNode.Parent;
    }
    while (prevNode !=null && prevNode.ConditionData==null)
    return prevNode;
}

function getNextNode(node) {
    var nextNode = node
    var lastNode = null;
    do {
        lastNode = nextNode;
        nextNode = nextNode.NextNode();
        if (nextNode == null && lastNode != null && lastNode.Parent != null) {
            nextNode = lastNode.Parent.NextNode();
        }
        else if (nextNode != null && lastNode != null && nextNode.Parent != lastNode.Parent && nextNode.ConditionData == null) {
            nextNode = nextNode.NextNode();
        }
    } while (nextNode != null && nextNode.ConditionData == null )

    return nextNode;
}

function moveDown() {
    var node = TreeView1.SelectedNode;
    var btnUp = document.getElementById(prefix + "btnUp");
    var btnDown = document.getElementById(prefix + "btnDown");
    if (node != null) {
        var treeNodeCollection = node.Parent != void 0 ? node.Parent.ChildNodes : node.TreeView().Nodes;
        var nodesLength = treeNodeCollection.length;

        var tnNext = getNextNode(node);
        var tnCopy = node.Copy();
        var tnLast = null;
        var condDataLast = null;
        var condDataCopy = tnCopy.ConditionData;

        if (nodesLength > 0) {
            tnLast = treeNodeCollection[nodesLength - 1];
            condDataLast = tnLast.ConditionData;
        }

        if (tnNext != null) {
            if ((node.Parent == null && tnNext.Parent == null) ||
                (node.Parent != null && tnNext.Parent != null)) {
                if (node.Parent == null) {
                    var cond = tnNext.ConditionData;
                    if (cond == null || cond.GroupName == "") {
                        //tnNext = getNextNode(tnNext);
                    }
                    else if (cond.GroupName != "") {
                        cond = tnCopy.ConditionData;
                        if (cond != null)
                            cond.ContainedBy = "";
                    }
                }
                else {
                    if (tnNext.Index() == node.Parent.ChildNodes.length - 1) {
                        tnNext = null;
                    }
                    else {
                        tnNext = getNextNode(tnNext);
                    }
                }
                if (tnNext == null) {
                    tnLast = new TreeNode();
                    tnLast.SetText(condDataLast.LogicalOperator);
                    tnLast.SetTextColor("ForestGreen");

                    tnLast.SetDraggable(false);
                    if (node.Parent == null)
                        TreeView1.Nodes.Add(tnLast);
                    else
                        node.Parent.ChildNodes.Add(tnLast);

                    if (tnCopy.ConditionData != null)
                        tnCopy.ConditionData.LogicalOperator = condDataLast.LogicalOperator;

                    if (node.Parent == null)
                        TreeView1.Nodes.Add(tnCopy);
                    else
                        node.Parent.ChildNodes.Add(tnCopy);

                    tnNext = node.NextNode();

                    if (node.Parent == null) {
                        TreeView1.Nodes.Remove(tnNext);
                        TreeView1.Nodes.Remove(node);
                    }
                    else {
                        node.Parent.ChildNodes.Remove(tnNext);
                        node.Parent.ChildNodes.Remove(node);
                    }
                    tnCopy.Select(true);
                    if (tnCopy.ConditionData.GroupName != "")
                        tnCopy.Expand();
                }
                else {
                    moveNodeDown(node, tnNext);
                }
            }
            else if (node.Parent != null && tnNext.Parent == null) {
                moveNodeDown(node, tnNext);
            }

            //treeNodeCollection.Remove(node);
            //treeNodeCollection.AddAt(tnNext.Index() + 1, tnCopy);
        }
        else {
            
        }
    }
}

function adjustButtons() {
    var hdnConditionCount = document.getElementById(prefix + "hdnConditionCount");
    var nConditions = parseInt(hdnConditionCount.value);
    var rbAnd = document.getElementById(prefix + "rbAnd");
    var rbOr = document.getElementById(prefix + "rbOr")
    var btnAddGroup = document.getElementById(prefix + "btnAddGroup");
    var btnUngroup = document.getElementById(prefix + "btnUngroup");
    var btnUp = document.getElementById(prefix + "btnUp");
    var btnDown = document.getElementById(prefix + "btnDown");

    if (nConditions < 3) {
        btnAddGroup.style.display = "none";
        btnUngroup.style.display = "none";
        btnUp.style.display = "none";
        btnDown.style.display = "none";
    }
    else {
        btnUngroup.value = "Ungroup";
        setEnabled("btnUngroup", false);
    }
    rbAnd.checked = false;
    rbOr.checked = false;
    setEnabled("rbAnd", false);
    setEnabled("rbOr", false);
}

function deleteGroup() {
    var hdnGroupCount = document.getElementById(prefix + "hdnGroupCount");
    var nGroups = parseInt(hdnGroupCount.value) - 1;
    var btnUngroup = document.getElementById(prefix + "btnUngroup");
    var node = TreeView1.SelectedNode;
    var n = node.Index();
    var cond = node.ConditionData;
    var nextNode = node.NextNode();
    var tn, opn;

    if (cond.GroupName != "") {
        if (node.ChildNodes != null && node.ChildNodes.length > 0) {
            var op = cond.LogicalOperator;
            for (var i = 0; i < node.ChildNodes.length; i++) {
                if (node.ChildNodes[i].ConditionData != null) {
                    tn = node.ChildNodes[i].Copy();
                    cond = tn.ConditionData;
                    cond.LogicalOperator = op;
                    cond.ContainedBy = "";
                    TreeView1.Nodes.AddAt(n, tn);
                    n++;
                    if (i < node.ChildNodes.length - 1 || nextNode !=null) {
                        opn = new TreeNode();
                        opn.SetDraggable(false);
                        opn.SetText(op);
                        opn.SetTextColor("ForestGreen");
                        TreeView1.Nodes.AddAt(n, opn);
                        n++;
                    }
                }
            }
        }
        deletedGroups.Groups.push(node.ConditionData);
        TreeView1.Nodes.Remove(node);
        if (nextNode != null)
            TreeView1.Nodes.Remove(nextNode);
        n = TreeView1.Nodes.length - 1;
        var tnLast = TreeView1.Nodes[n];
        if (tnLast.ConditionData == null)
            TreeView1.Nodes.RemoveAt(n);
        if (tn != null)
            tn.Select(true);
        btnUngroup.value = "Ungroup";
        setEnabled("btnUngroup", false);
        hdnGroupCount.value = nGroups.toString();
    }
}

function addGroup() {
    var hdnGroupCount = document.getElementById(prefix + "hdnGroupCount");
    var nGroups = parseInt(hdnGroupCount.value) + 1;
    var node, cond, op
    var n = TreeView1.GetNodeCount(false);
    if (n > 0) {
        node = TreeView1.Nodes[n - 1];
        cond = node.ConditionData;

        if (cond != null) {
            op = cond.LogicalOperator;
            node = new TreeNode();
            node.SetDraggable(false);
            node.SetText(op);
            node.SetTextColor("ForestGreen");
            TreeView1.Nodes.Add(node);

        }
        node = new TreeNode();
        node.SetText("Group " + nGroups);
        node.SetTextColor("Purple");
        cond = { Condition: "", LogicalOperator: op, GroupName: "Group " + nGroups, ContainedBy: "", RecordOrder: "", ConditionId: "new" }
        node.SetConditionData(cond);
        TreeView1.Nodes.Add(node);
        hdnGroupCount.value = nGroups;
        node.Select(true);
    }
}

function cleanTree() {
    var cond, subCond, node, subNode
    var recno = 0;
    var grpno = 0;
    var grp = {};
    var group = "";
    //var subno;

    for (var i = 0; i < TreeView1.Nodes.length; i++) {
        node = TreeView1.Nodes[i];
        cond = node.ConditionData;
        if (cond != null && cond.GroupName != "" && node.ChildNodes.length==0) {
            node.Select(true);
            deleteGroup();
            i--;
            continue;
        }
        if (cond != null) {
            recno++;
            cond.RecordOrder = recno.toString();
            if (node.ChildNodes.length > 0) {
                grpno++;
                if (grp[cond.GroupName] != "1")
                    grp[cond.GroupName] = "1";
                else {
                    cond.GroupName = "Group " + grpno.toString();
                    node.SetText(cond.GroupName);
                    grp[cond.GroupName] = "1";
                }
                group = cond.GroupName;
                //subno = 0;
                for (var j = 0; j < node.ChildNodes.length; j++) {
                    subnode = node.ChildNodes[j];
                    subCond = subnode.ConditionData;
                    if (subCond != null) {
                        recno++;
                        subCond.RecordOrder = recno.toString();
                        subCond.ContainedBy = group;
                    }
                }
            }
        }
    }
    return true;
}

function getTreeData() {
    var hdnJson = document.getElementById(prefix + "hdnJson");
    var hdnDeletedData = document.getElementById(prefix + "hdnDeletedData");
    if (cleanTree()) {
        var json = TreeView1.ToJSON();
        hdnJson.value = JSON.stringify(json);
        if (deletedGroups.Groups.length > 0)
            hdnDeletedData.value = JSON.stringify(deletedGroups);
        isLoaded = false;
        return true;
    }
    return false;
}
function loadTreeView(ctlId) {
    if (!isLoaded) {
        TreeView1 = new TreeView();
        prefix = ctlId;
        TreeView1.SetContainer("divTreeView");
        var hdnJson = document.getElementById(prefix + "hdnJson");
        
        populateTreeView(hdnJson.value);
        adjustButtons();
        isLoaded = true;

        TreeView1.SelectedNodeChanged = function (event) {
            selectedNodeChanged(event);
        }
        TreeView1.BeforeLabelEdit = function (event) {
            return true;
        }

        TreeView1.AfterLabelEdit = function (event) {
            TreeView1.EditLabel = false;
            var node = TreeView1.SelectedNode;
            if (node != null) {
                var cond = node.ConditionData;
                if (cond.GroupName != "") {
                    cond.GroupName = node.Text;
                    if (node.ChildNodes.length > 0) {
                        for (var i = 0; i < node.ChildNodes.length; i++) {
                            cond = node.ChildNodes[i].ConditionData;
                            if (cond != null)
                                cond.ContainedBy = node.Text;
                        }
                    }
                }
            }
            return true;
        }
        TreeView1.NodeDropped = function (event, sourceElement, targetElement) {
            onNodeDropped(event, sourceElement, targetElement);
        }
    }
}
function onNodeDropped(e, srcElem, targElem) {
    
    if (srcElem != null && targElem != null) {
        var tnSrc = srcElem.treeNode;
        var tnTarg = targElem.treeNode;
        if (tnSrc != null && tnTarg != null) {
            if (tnSrc != tnTarg) {
                var dir = getDirection(tnSrc, tnTarg);
                if (dir == "DOWN")
                    moveNodeDown(tnSrc, tnTarg);
                else
                    moveNodeUp(tnSrc, tnTarg);
            }
        }
    }

}

function getDirection(fromNode, toNode) {
    var fromParent, toParent, nFrom, nTo;
    var ret = "DOWN";
    
    nFrom = fromNode.Index();
    nTo = toNode.Index();
    if (fromNode.Parent == null && toNode.Parent == null) {
        if (nFrom > nTo)
            ret = "UP";
    }
    else if (fromNode.Parent != null && toNode.Parent != null) {
        fromParent = fromNode.Parent;
        toParent = toNode.Parent;
        if (fromParent.Index() == toParent.Index()) {
            if (nFrom > nTo)
                ret = "UP";
        }
        else if (fromParent.Index() > toParent.Index())
            ret = "UP";
    }
    else if (fromNode.Parent != null) {
        if (toNode == fromNode.Parent || fromNode.Parent.Index() > nTo)
            ret = "UP";
    }
    else if (nFrom > toNode.Parent.Index()) {
        ret = "UP";
    }
    return ret;
}
//        };
//    };
//};


