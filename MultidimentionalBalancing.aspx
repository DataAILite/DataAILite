<%@ Page Language="VB" AutoEventWireup="false" CodeFile="MultidimentionalBalancing.aspx.vb" Inherits="MultidimentionalBalancing" %>
<%@ Register TagPrefix="uc1" TagName="DropDownColumns" Src="Controls/uc1.ascx" %>

<script type="text/javascript" src="Controls/Javascripts/OUR.js"></script>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Multidimensional Balancing</title>
<style type="text/css">
        .auto-style1 {
            width: 107px;
            height: 26px;
        }
        .auto-style2 {
            width: 300px;
            height: 26px;
        }
        .auto-style3 {
            width: 247px;
            height: 26px;
        }
        .auto-style4 {
            margin-left: 0px;
        }
.modal
{
    position: fixed;
    z-index: 2147483647;
    height: 100%;
    width: 100%;
    top: 0;
    background-color: #f8f8d3;
    opacity: 0.8;
}
.box{
    border: 2px solid white; /* Border around the box */
    padding: 20px;          /* Space inside the box */
    margin: 20px 0;         /* Space outside the box, 20px top and bottom, 0 left and right */
    width: auto;           /* Width of the box */
    box-shadow: 3px 3px 10px rgba(0,0,0,0.1); /* Optional: shadow effect */
    transition: all 0.3s;   /* Optional: smooth transitions */
    background-color: #E5E5E5;
}
.masterbox {

    border: 2px solid white;
    padding: 15px;
    width: 100%;   /*auto - Adjust as necessary */
    display: inline-block;
    background-color: #E5E5E5;
}

.majorbox {
    border: 2px solid white;
    width: 100%;
    padding: 5px;

}
.boxed-table {
    border-collapse: collapse;   /* Remove spacing between table cells */
    width: 95%;                  /* Set to your preferred width */
    margin: 10px auto;           /* Center the table on the page with some margin */

}
.newtable {
    border: 2px solid white;     /* Outer border for the table */
    width: 100%;                  /* Set to your preferred width */
    margin: 10px auto;           /* Center the table on the page with some margin */
    padding: 10px;
}
.boxed-table td {
    border: 2px solid white;     /* Border for individual cells */
    padding: 10px;               /* Space inside each cell */
}
.flex-container {
    display: flex;
    justify-content: space-between; /* Optional: it adds space between the boxes */
}
.masterbox {
    margin-right: 20px; /* Adjust as needed */
}
.masterbox:not(:last-child) {
    margin-right: 20px; /* Adjust as needed */
}
.container {
    display: flex;          /* Enables flexbox */
    justify-content: space-between; /* Optional: Spacing between the boxes */
    flex-wrap: nowrap;      /* Prevents the boxes from wrapping to the next line */
}
.input-row {
    display: flex;
    align-items: center; /* this aligns items vertically in the middle, useful if one of them gets taller */
    margin-bottom: 10px; /* add space between rows */
}

.input-label {
    flex-grow: 1;   /* Allow the label to grow */
    flex-shrink: 0; /* Don't allow the label to shrink */
    margin-right: 10px; 
}
.checkbox-row {
    margin-bottom: 8px;  /* Space between each checkbox */
    cursor: pointer;  /* Change cursor to hand on hover for better interactivity */
}

.checkbox-row:hover {
    background-color: #E5E5E5;  /* Subtle hover effect */
}
.center
{
    z-index: 2147483647;
    margin: 300px auto;
    padding-left:25px;
    padding-top:10px;
    width: 130px;
    background-color:#f8f8d3;
    border-radius: 10px;
}
.center img
{
    height: 100px;
    width: 100px;
}
.NodeStyle
{
    color: #0066FF;
    font-size:12px;
    font-weight:normal;
    text-decoration:none;
}
.NodeStyle:hover
{
    text-decoration:underline;
    color:darkblue;
}
.ticketbutton 
{
  width: 630px;
  height: 25px;
  font-size: 12px;
  border-radius: 5px;
  border-style :solid;
  border-color: #4e4747 ;
  color: black;
  border-width: 1px;
  /*background-color: ButtonFace;*/
  background-image: linear-gradient(to bottom, rgba(211, 211, 211,0),rgba(211, 211, 250,3));

  padding: 3px;
  margin:5px;
  z-index: 9999; 
}
.other-ticketbutton 
{
  width: 630px;
  height: 25px;
  font-size: 12px;
  border-radius: 5px;
  border-style :solid;
  border-color: #4e4747 ;
  color: black;
  border-width: 1px;
  /*background-color: ButtonFace;*/
  background-image: linear-gradient(to bottom, rgba(211, 211, 211,0),rgba(211, 211, 250,3));

  padding: 3px;
  margin-left:20px;
  margin-top:10px;
  margin-right:10px;
  margin-bottom:10px;
  z-index: 9999; 

}
.other-ticketbutton2
{
  width: 630px;
  height: 25px;
  font-size: 12px;
  border-radius: 5px;
  border-style :solid;
  border-color: #4e4747 ;
  color: black;
  border-width: 1px;
  background-image: linear-gradient(to bottom, rgba(211, 211, 211,0),rgba(211, 211, 250,3));

  padding: 3px;
  margin-left:20px;
  margin-top:27px;
  margin-right:10px;
  margin-bottom:10px;
  z-index: 9999; 

}
        </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />      
    <asp:UpdatePanel ID="udpAdvancedAnalytics" runat ="server" >
       <ContentTemplate>
            <div>
           <table>
      <tr>
          <td colspan="3" style="font-size:x-large; font-style:normal; font-weight:bold; background-color: #e5e5e5; vertical-align:middle; text-align: left; height: 40px;">
              <asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reporting"></asp:Label>
          </td>
      </tr> 
        <tr>
            <td style="font-size: x-small; font-style: normal; font-weight: normal; background-color: #e5e5e5; vertical-align: top; text-align: left; width: 15%;">
                    <div id="tree" style="font-size: x-small; font-weight: normal; font-style: normal">
                        <%--<br /><br />--%>

     <asp:TreeView ID="TreeView1"  runat="server" Width="100%" NodeIndent="10" Font-Names="Times New Roman"  EnableTheming="True" ImageSet="BulletedList">
          <Nodes>  
             <asp:TreeNode Text="&lt;b&gt;Log Off&lt;/b&gt;"  Value="Default.aspx" Expanded="True" ></asp:TreeNode>

             <asp:TreeNode Text="&lt;b&gt;List of Reports&lt;/b&gt;"  Value="ListOfReports.aspx" Expanded="True" ></asp:TreeNode>                              
            
             <asp:TreeNode Text="&lt;b&gt;Report Definition&lt;/b&gt;" Value="ReportEdit.aspx?tne=2" Expanded="True" ></asp:TreeNode>

             <asp:TreeNode Text="&lt;b&gt;Report Data Query&lt;/b&gt;"  Value="SQLquery.aspx?tnq=0" Expanded="True" ></asp:TreeNode>
              
             <asp:TreeNode Text="&lt;b&gt;Report Format Definition&lt;/b&gt;"  Value="RDLformat.aspx?tnf=0" Expanded="True" ></asp:TreeNode>

             <%--<asp:TreeNode Text="&lt;b&gt;Advanced Report Designer&lt;/b&gt;"  Value="ReportDesigner.aspx" Expanded="True" ></asp:TreeNode>--%>

            <asp:TreeNode Text="Explore Report Data"  Value="ShowReport.aspx?srd=0" Expanded="True" >
                <asp:TreeNode Text="Export Data to Excel"  Value="datatoExcel" NavigateUrl="ShowReport.aspx?srd=1" > </asp:TreeNode>
                <asp:TreeNode Text="Export Data to CSV"    Value="datatoCSV" NavigateUrl="ShowReport.aspx?srd=2" > </asp:TreeNode>
                <asp:TreeNode Text="Export Data to Delimited File"  value="ShowReport" Navigateurl="ShowReport.aspx?srd=10" > </asp:TreeNode>
                <asp:TreeNode Text="Export Data to XML"    Value="datatoXML" NavigateUrl="ShowReport.aspx?srd=14" > </asp:TreeNode> 
            </asp:TreeNode>

                <asp:TreeNode Text="Show Report"  Value="ShowReport.aspx?srd=3" Expanded="True" >
                  <asp:TreeNode Text="Show Generic Report"  Value="ReportViews.aspx?gen=yes" > </asp:TreeNode>
                 <asp:TreeNode Text="Show Report Charts"  Value="ShowReport.aspx?srd=17" > </asp:TreeNode>
                <asp:TreeNode Text="Export Report to Excel"  Value="reptoExcel" NavigateUrl="ShowReport.aspx?srd=4" > </asp:TreeNode>
                <asp:TreeNode Text="Export Report to Word"    Value="reptoWord" NavigateUrl="ShowReport.aspx?srd=5" > </asp:TreeNode>
                <asp:TreeNode Text="Export Report to PDF"  Value="reptoPDF" NavigateUrl="ShowReport.aspx?srd=6" > </asp:TreeNode>
                <asp:TreeNode Text="See Crystal Report"    Value="ShowReport.aspx?srd=7"> </asp:TreeNode>
                </asp:TreeNode>

                <asp:TreeNode Text="Show Analytics"  Value="ShowReport.aspx?srd=11" Expanded="True" >
                <asp:TreeNode Text="See Data Overall Statistics"  Value="ShowReport.aspx?srd=8" > </asp:TreeNode>
                <asp:TreeNode Text="Export Overall Statistics to Excel"  Value="reptoExcel" NavigateUrl="ShowReport.aspx?srd=9" > </asp:TreeNode>
                <asp:TreeNode Text="See Groups Statistics"  Value="ReportViews.aspx?grpstats=yes" > </asp:TreeNode>
                <asp:TreeNode Text="See Fields Correlation"  Value="ShowReport.aspx?srd=12" > </asp:TreeNode>
                <asp:TreeNode Text="Matrix Balancing"  Value="ShowReport.aspx?srd=13" > </asp:TreeNode>
                </asp:TreeNode>
        </Nodes>
        <RootNodeStyle HorizontalPadding="2px" Font-Bold="True" Font-Underline="False" />
        <NodeStyle CssClass="NodeStyle" />
        <ParentNodeStyle Font-Bold="True" />
     </asp:TreeView>
    </div>
            </td>
            <td width="5px"></td>
   <td id="main" style="width: 85%; text-align: left; vertical-align: top"> 
    <div style="text-align: center;width:100%;">
    <div style="text-align: left;">
      <%--  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
           <asp:HyperLink ID="HyperLinkListOfReports" runat="server" NavigateUrl="~/ListOfReports.aspx" Enabled="True" Visible="True" CssClass="NodeStyle" Font-Names="Arial">List of Reports</asp:HyperLink>
            
           <asp:LinkButton ID="LinkButtonRefresh" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="May take a long time...">Recalculate Analytics</asp:LinkButton> 
     --%>
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/ShowReport.aspx?srd=12" CssClass="NodeStyle" Font-Names="Arial">Correlation</asp:HyperLink>
        
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/ShowReport.aspx?srd=8" CssClass="NodeStyle" Font-Names="Arial">Data and Statistics</asp:HyperLink>

         &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:HyperLink ID="HyperLink5" runat="server" NavigateUrl="~/ShowReport.aspx?srd=3" CssClass="NodeStyle" Font-Names="Arial">Report and Charts</asp:HyperLink>
              
         &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:HyperLink ID="HyperLinkListOfDashboards" runat="server" NavigateUrl="~/ListOfDashboards.aspx" CssClass="NodeStyle" Font-Names="Arial">List of User Dashboards</asp:HyperLink>
         
         &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:HyperLink ID="HyperLinkAnalytics" runat="server" NavigateUrl="~/Analytics.aspx" CssClass="NodeStyle" Font-Names="Arial">Analytics</asp:HyperLink>
     
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/FriendlyNames.aspx" CssClass="NodeStyle" Font-Names="Arial" Enabled="False" Visible="False">FriendlyNames</asp:HyperLink>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp; 
        <asp:HyperLink ID="HyperLink6" runat="server" NavigateUrl="DataAIHelp.aspx?hilt=Matrix_Balancing" Target="_blank" CssClass="NodeStyle" Font-Names="Arial">Matrix Balancing Help</asp:HyperLink>&nbsp;&nbsp;
              
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp; 
        <asp:HyperLink ID="HyperLinkHelp" runat="server" NavigateUrl="DataAIHelp.aspx" Target="_blank" CssClass="NodeStyle" Font-Names="Arial">Help</asp:HyperLink>&nbsp;&nbsp;
                 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
       &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Default.aspx" CssClass="NodeStyle" Font-Names="Arial">Log off</asp:HyperLink>            
        
    <table border="0" cellpadding="1" cellspacing="0" width="100%">
    
      <tr id="trMessage" runat ="server" visible ="true" >
       <td align="left" valign="top">
         <asp:Label ID="LabelMessage" runat="server" Font-Size="Larger" ForeColor="#CC0000" Font-Names="Arial"></asp:Label>
       </td>
      </tr>
     <tr>
       <td align="left" valign="top">
         
         <asp:Label ID="lblHeader" runat="server"  Font-Size="22px" Font-Names="Arial" >Advanced Analytics:</asp:Label>        

       </td>
      </tr>
      
        <tr>
            <td align="left" style="font-weight: bold; color: black; font-family: Arial; background-color: #e5e5e5; font-size:small;" class="auto-style1">
                        <asp:Label ID="Label30" runat="server" Text="Select Scenario: " ForeColor="Black" Font-Size="Medium" ></asp:Label>
                        <asp:DropDownList ID="DropDownListScenarios" runat="server" ToolTip="Scenarios of matrix balancing" AutoPostBack="True">
                                                <asp:ListItem>   </asp:ListItem> 
                                              
                                                <asp:ListItem Value="1a">1a: Starting Matrix of aggregated field1 values to balance by manually entered sums by rows and sums by columns</asp:ListItem> 
                                                <asp:ListItem Value="1b">1b: Starting Matrix of rows by matrix group field and selected multiple columns to balance by manually entered sums by rows and sums by columns</asp:ListItem> 
                                                <asp:ListItem Value="2a">2a: Starting Matrix of aggregated field1 to balance for sums of rows and columns of the Target Matrix of the aggregated field2</asp:ListItem>                                              
                                                <asp:ListItem Value="2b">2b: Balancing matrix of aggregated field1 for iterations of starting and target values of the field2</asp:ListItem>                                                 
                                                <asp:ListItem Value="2c">2c: Get balancing coefficients for Starting Matrix of field1 for all iterations between starting and target values of the field2</asp:ListItem>
                                                <asp:ListItem Value="3a">3a: Balancing coefficients for matrix of aggregated field1 values and for iterations of multiple selected aggregated fields</asp:ListItem>
                                                <asp:ListItem Value="3b">3b: Balancing matrix of rows and multiple columns for iterations of starting and target values of the field2</asp:ListItem>
                                                <asp:ListItem Value="3c">3c: Balancing coefficients for matrix of rows and multiple cols for iterations between start and target of field2 values</asp:ListItem>
                                                <asp:ListItem Value="4a">4a: Starting Matrix of aggregated field1 to balance for sums of selected columns of the Target Matrix of the aggregated field2</asp:ListItem>
                                                <asp:ListItem Value="4b">4b: The starting value of field2 to get the Starting matrix of field1 values to balance for sums of selected columns of target matrix of field2 </asp:ListItem>
                                                <asp:ListItem Value="4c">4c: Starting Matrix of aggregated field1 to balance by manually entered sums by selected columns </asp:ListItem>
                        </asp:DropDownList>&nbsp;
                         
                <br />
                       
                   <table  runat="server" id="maintable"  border=0 style="font-size: 12px; font-family: Arial; color: black;">
                      <tr  id="tr1a" runat ="server" >
                       <td>
                           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="Label28" runat="server" Text="Scenario 1a: Starting Matrix of aggregated field1 values to balance by manually entered sums by rows and sums by columns " ToolTip="Scenario 1a: Select both group fields (for rows and columns) and the field1 with aggregation function. Starting matrix of aggregated field1 balances by manually entered sums by rows and columns"></asp:Label>
                         
                       </td>
                      </tr>
                       <tr  id="tr1b" runat ="server" >
                       <td>
                           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="Label29" runat="server" Text="Scenario 1b: Starting Matrix of rows by group field for rows and selected columns from the multiple fields to balance by manually entered sums by rows and sums by columns " ToolTip="Scenario 1b: Select the group field for rows and multiple matrix columns. Starting matrix of rows by matrix group field for rows and columns from selected multiple fields balances by manually entered sums by rows and columns.  For Scenarios 1b, 3b, and 3c the selected columns are columns in the matrix."></asp:Label>
                         
                       </td>
                      </tr>
                      <tr  id="tr2a" runat ="server" >
                       <td>
                           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="Label22" runat="server" Text="Scenario 2a: Starting Matrix of aggregated field1 to balance for sums of rows and columns of the Target Matrix of the aggregated field2" ToolTip="Scenario 2a. Select both group fields for rows and columns, field1 with aggregation function for items of Starting Matrix, field2 with aggregation function for items in Target Matrix. Starting Matrix of aggregated field1 to balance for sums of rows and columns of the Target Matrix of the aggregated field2"></asp:Label>
                        
                       </td>
                      </tr>
                      <tr  id="tr2b" runat ="server" >
                       <td>
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="Label6" runat="server" Text="Scenario 2b: The starting value of field2 to get the Starting matrix of field1 values and target value of field2 to get Target matrix" ToolTip="Scenario 2b. Select both group fields for rows and columns, field1 with aggregation function for matrix items, and field2 with starting and target values. Starting field2 value as restriction to get the field1 values for starting and other values to get other iteration Matrix, used in Scenarios 2b, 2c, 3b, 3c where values of the field2 used as restrictions on data to get iterations of Matrix. Not used in Scenario 3a, the multiple selected fields aggregation function used there instead."></asp:Label>
                          
                       </td>
                      </tr>
                      <tr  id="tr2c" runat ="server" >
                       <td>
                           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="Label26" runat="server" Text="Scenario 2c: Get balancing coefficients for Starting Matrix of field1 for all iterations between starting and target values of the field2" ToolTip="Scenario 2c. Select both group fields for rows and columns, field1 with aggregation function for matrix items, and field2 with starting and target values. Get balancing coefficients for Starting Matrix of aggregated field1 for all iterations between starting and target values of the field2"></asp:Label>
                         
                       </td>
                      </tr>
                      <tr  id="tr3a" runat ="server" >
                       <td>
                           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="Label23" runat="server" Text="Scenario 3a: Get balancing coefficients for Starting Matrix of aggregated values of field1 and multiple Target Matrix of aggregated selected fields" ToolTip="Scenario 3a. Select both group fields, field1 with aggregation function, and multiple fields with aggregation function. Matrix balancing for iterations for multiple fields values. Field2 is not used."></asp:Label>
                      
                       </td>
                      </tr>
                      <tr  id="tr3b" runat ="server" >
                       <td>
                           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="Label25" runat="server" Text="Scenario 3b: Starting Matrix as rows by matrix group field for rows and selected multiple columns to balance iterations from starting to target values of the field2" ToolTip="Scenario 3b. Select group field for rows, multiple fields for columns, and field2 with starting and target values. Matrix balancing for iterations by starting and target values of the field2. For Scenarios 1b, 3b, and 3c the selected multiple columns are columns in the matrix, and iterations are done using field2 values as restrictions for iterations."></asp:Label>
                      
                       </td>
                      </tr>
                      <tr  id="tr3c" runat ="server" >
                       <td>
                           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="Label24" runat="server" Text="Scenario 3c: Get balancing coefficients for Starting Matrix as rows by matrix group field for rows and columns from selected multiple fields, for all iterations between starting and target of the field2 values" ToolTip="Scenario 3c. Select group field for rows, multiple fields for columns, and field2 with starting and target values. Get balancing coefficients for Starting Matriix as rows by matrix group field for rows and the selected multiple columns, and all iterations between starting and target of the field2 values. For Scenarios 1b, 3b, and 3c the selected columns are columns in the matrix, and iterations are done using field2 values as restrictions for iterations. "></asp:Label>
                           
                       </td>
                      </tr>
                      <tr  id="tr4a" runat ="server" >
                       <td>
                           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="Label35" runat="server" Text="Scenario 4a: Starting Matrix of aggregated field1 to balance for sums of selected columns of the Target Matrix of the aggregated field2" ToolTip="Scenario 4a. Select columns to balance by, field1 with aggregation function for items of Starting Matrix, field2 with aggregation function for items in Target Matrix. Starting Matrix of aggregated field1 to balance for sums of selected columns of the Target Matrix of the aggregated field2"></asp:Label>
                        
                       </td>
                      </tr>
                      <tr  id="tr4b" runat ="server" >
                       <td>
                           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="Label36" runat="server" Text="Scenario 4b: The starting value of field2 to get the Starting matrix of field1 values and target value of field2 to get Target matrix, and balance by sums of selected columns" ToolTip="Scenario 4b. Select columns to balance by, field1 with aggregation function for matrix items, and field2 with starting and target values. Starting field2 value as restriction to get the field1 values for Starting Matrix, and the target value of the field2 as restriction to get the field1 values for Target Matrix"></asp:Label>
                          
                       </td>
                      </tr>
                      <tr  id="tr4c" runat ="server" >
                       <td>
                           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="Label37" runat="server" Text="Scenario 4c: Starting Matrix of aggregated field1 to balance by manually entered sums by selected columns" ToolTip="Scenario 4c. Select columns to balance by, field1 with aggregation function for items of Starting Matrix, manually enter sums for Target Matrix. Starting Matrix of aggregated field1 to balance for sums of selected columns in the Target Matrix"></asp:Label>
                        
                       </td>
                      </tr>
                     
                       <tr  id="trBlank" runat ="server" >
                       <td>
                           <asp:Label ID="Label31" runat="server" Text="  " ToolTip="Diffrent scenarios require different sets of parameters. "></asp:Label>
                           
                       </td>
                      </tr>

<tr>
    <td style="border: 2px solid #FFFFFF">
       <div id="devtrControls" runat="server" style="display:none;">
        <table style="border: 2px solid #FFFFFF; width: 1000px; height: auto;" >
            <tr>                      
               <td style="width: 750px; vertical-align: top;">
                       
                  <table style="border: 2px solid #FFFFFF; width: 750px;  height: auto; vertical-align: top;">


                      <tr  id="trRowsCols" runat ="server"  style="border: 1px solid #FFFFFF; height: 2px; vertical-align: top;" >
                       <td>
                           <%--<table>
                               <tr>
                                   <td>--%>
                         
                                <asp:Label ID="lblGroups1" runat="server" Text="Matrix rows by: " ForeColor="Black"></asp:Label>
                                <asp:DropDownList ID="DropDownList1" runat="server" ToolTip="Group row field. Group field for Matrix rows to get Starting and Target matrix" AutoPostBack="True"></asp:DropDownList>&nbsp;
                           <%--</td>
                           <td>--%>
                               <asp:Label ID="lblGroups2" runat="server" Text=" columns by: " ForeColor="Black"></asp:Label>
                               <asp:DropDownList ID="DropDownList2" runat="server" ToolTip="Group column field. Group field for Matrix columns to get Starting and Target matrix for Scenarios 1, 2a, 2b, 2c, 3a. Scenarios 3b and 3c are using the selected multiple columns." AutoPostBack="True"></asp:DropDownList>&nbsp;
                           
                          <%--</td>                        
                           
                           </tr>
                           </table>--%>
                       </td>
                      </tr>

                     <tr  id="trColumnsDim" runat ="server"  style="border: 1px solid #FFFFFF" >
                         <td style="padding: 10px; margin: 10px; border: 2px solid #FFFFFF;">
                             <asp:Label ID="lblGroups" runat="server" Text="additional dimensions by: " ToolTip="Group field(s) for Matrix additional dimensions. Do not use row group field nor column group field if apply."  ForeColor="Black"></asp:Label>
                               <div  id="divColumnsDim" runat="server" style="display: inline; margin-left:425px;">                               
                               
                                       <uc1:DropDownColumns ID="DropDownColumnsDim" runat="server" Width="700px" ClientIDMode="Predictable" FontName="arial" FontSize="Small" BorderColor="Silver" ForeColor="Black" BorderWidth="1" DropDownHeight="190px" ToolTip="Group field(s) for Matrix additional dimensions." PostBackType="OnClose" TextBoxHeight="20px" DropDownButtonHeight="22px" />
                               
                               </div>
                               <br /><br />
                         </td>
                       </tr>
                       <tr  id="tr1MultiCols" runat ="server"  style="border: 1px solid #FFFFFF; height: 2px;" >
                         <td>
                            <div id="divMultiCols" runat="server" style="display:none; margin-left:20px; margin-bottom:10px;">
                             <asp:Label ID="Label17" runat="server" Text="Multiple fields: " ToolTip="In Scenario 3a the Starting Matrix has values of field1, the selected columns are used to get iterations of Matrix where for each iteration the values of items are values of the selected field. For Scenarios 3b and 3c the selected columns are columns in the matrix, and iterations are done using field2 values as restrictions for iterations. "></asp:Label>
                             &nbsp;&nbsp;&nbsp;<asp:CheckBox ID="CheckBoxSelectAllFields" runat="server" AutoPostBack="True" Text="select all fields" /> 
                             &nbsp;&nbsp;&nbsp;&nbsp;<asp:CheckBox ID="CheckBoxUnselectAllFields" runat="server" AutoPostBack="True" Text="unselect all fields" />
                              &nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:Label ID="Label18" runat="server" Text=" aggregation function: "  ToolTip="aggregation function - used only in 3a. "></asp:Label>
                                         <asp:DropDownList ID="DropDownList10" runat="server"  ToolTip="Numeric or Text aggregate functions if needed"></asp:DropDownList>
                                         <br />
                                <div id="divColumns" runat="server" style="display: inline; margin-left:25px;">
                                      
                                         <uc1:DropDownColumns ID="DropDownColumns" runat="server" Width="600px" ClientIDMode="Predictable" FontName="arial" FontSize="Small" BorderColor="Silver" ForeColor="Black" BorderWidth="1" DropDownHeight="190px" ToolTip="Finishing matrixs are for values of the fields." PostBackType="OnClose" TextBoxHeight="20px" DropDownButtonHeight="22px" />
                                     
                                     
                                 </div>
                            </div>
                         </td>
                       </tr>


                      <tr  id="trField1" runat ="server"   style="border: 1px solid #FFFFFF">
                       <td style="padding: 10px; margin: 2px; border: 2px solid #FFFFFF;">
                           <%--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>
                           <asp:Label ID="Label3" runat="server" Text="Matrix items by field1: " ToolTip="Field value to calculate Matrix items for Scenarios 1a, 2a, 2b, 2c, 3a. Scenarios 1b, 3b and 3c are using the values in selected multiple columns from the given matrix."></asp:Label>
                           <asp:DropDownList ID="DropDownList3" runat="server" ToolTip="Field1 value to calculate Matrix items for Scenarios 1a, 2a, 2b, 2c, 3a. Scenarios 1b, 3b and 3c are using the values in selected multiple columns from inputed matrix." AutoPostBack="True"></asp:DropDownList>     
                          <br/> <asp:Label ID="Label4" runat="server" Text=" aggregation function: "  ToolTip="aggregation function for field1 values, used in Scenarios 1a, 2a, 2b, 2c, 3a. Scenarios 1b, 3b and 3c are using the values in selected multiple columns from the given matrix, in scenario 3a with their aggregation function."></asp:Label>
                           <asp:DropDownList ID="DropDownList4" runat="server"  ToolTip="Numeric or Text aggregation functions, used in Scenarios 1a, 2a, 2b, 2c, 3a. Scenarios 1b, 3b and 3c are using the values in selected multiple columns from inputed matrix and their aggregation function."></asp:DropDownList>
                        
                       </td>
                      </tr>
                      <tr  id="trSumsByRows" runat ="server"   style="border: 1px solid #FFFFFF">
                       <td>
                           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                         <asp:Label ID="Label21" runat="server" Text="Enter sums by rows: " ToolTip="Array of sums for each row, comma separated."></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                         <asp:TextBox ID="TextBoxSumsByRows" runat="server" AutoPostBack="True" Width="500px" Text=" " ToolTip="Array of sums for each row, comma separated."></asp:TextBox>&nbsp;
                         
                       </td>
                      </tr>
                      <tr  id="trSumsByCols" runat ="server"   style="border: 1px solid #FFFFFF">
                       <td>
                           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                         <asp:Label ID="Label10" runat="server" Text=" Enter sums by columns: " ToolTip="Array of sums for each column, comma separated."></asp:Label>
                         <asp:TextBox ID="TextBoxSumsByCols" runat="server" AutoPostBack="True" Width="500px" Text=" " ToolTip="Array of sums for each column, comma separated."></asp:TextBox>&nbsp;
                         
                       </td>
                      </tr>
                      <tr  id="trField2" runat ="server"   style="border: 1px solid #FFFFFF">
                       <td style="padding: 10px; margin: 10px; border: 2px solid #FFFFFF;">
                           
                           <asp:Label ID="Label5" runat="server" Text="Iterations by the field2: " ToolTip="Matrix balancing for iterations by field2 values. Not used in Scenario 3a, the multiple selected fields used there instead."></asp:Label>
                          <asp:DropDownList ID="DropDownList5" runat="server" ToolTip="Matrix balancing for iterations by field2 values. Not used in Scenario 1b,3a, the multiple selected fields used there instead." AutoPostBack="True"></asp:DropDownList>      
                        <br/>  <asp:Label ID="Label11" runat="server" Text=" aggregation function: "  ToolTip="aggregation function for values in field2. Not used in Scenarios 1b, 2b, 2c, 3b, 3c where values of the field2 used as restrictions on data to get iterations of Matrix. Not used in Scenario 3a, the multiple selected fields aggregation function used there instead."></asp:Label>
                          <asp:DropDownList ID="DropDownList8" runat="server"  ToolTip="Numeric or Text aggregation functions if needed.  Not used in Scenarios 2b, 2c, 3b, 3c where values of the field2 used as restrictions on data to get iterations of Matrix. Not used in Scenario 3a, the multiple selected fields aggregation function used there instead."></asp:DropDownList>
                        <br/>&nbsp;&nbsp;
                            <asp:Label ID="Label1" runat="server" Text="starting value: " ToolTip="Starting field2 value as restriction to get the field1 values for starting and each iteration Matrix, used in Scenarios 2b, 2c, 3b, 3c where values of the field2 used as restrictions on data to get iterations of Matrix. Not used in Scenario 3a, the multiple selected fields aggregation function used there instead."></asp:Label>
                            <asp:DropDownList ID="DropDownList6" runat="server" ToolTip="Starting value of the field2 for getting Starting Matrix of items of field1 with selected aggregation. Used in Scenarios 2b, 2c, 3b, 3c where values of the field2 used as restrictions on data to get iterations of Matrix. Not used in Scenario 3a, the multiple selected fields aggregation function used there instead." AutoPostBack="True"></asp:DropDownList>      
                        &nbsp;&nbsp;
                        <asp:Label ID="Label7" runat="server" Text="and target value: " ToolTip="Target field2 value as restriction to get field1 values for target Matrix"></asp:Label>
                        <asp:DropDownList ID="DropDownList7" runat="server" ToolTip="Target value of the field2 for getting target matrix of items of field1 with selected aggregation. Used in Scenarios 2b, 2c, 3b, 3c where values of the field2 used as restrictions on data to get iterations of Matrix. Not used in Scenario 3a, the multiple selected fields aggregation function used there instead." AutoPostBack="True"></asp:DropDownList>      
                        <br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                       </td>
                      </tr>
                     
                       <tr id="trEntryTbl" runat ="server"   style="border: 1px solid #FFFFFF; background-color: #E5E5E5;"> 
                           <td style="font-size: 16px; font-family: Arial; color: black; padding: 15px; margin: 2px; border: 2px solid #FFFFFF;">
                              
                               Click 
                               <asp:LinkButton ID="btnShowEntryTbl4c" runat="server"  CssClass="NodeStyle" valign="center" ToolTip="(4c) Show table to manually enter the sums by selected fields before clicking the button (4c) below and after selecting balancing fields and field1 above. Entered sums will be adjusted during balancing to total sum for first selected field or to started sums depending on checkbox in the top of the page." Font-Bold="True">here</asp:LinkButton>
                               to enter the target sums by selected balancing fields.
                               <br />
                               <table id="EntryTable" runat="server" border=1 style="font-size: 12px; font-family: Arial; font-weight: bold;">                                   
                                 <tr>
                                   <td>
                                   </td>
                                 </tr>
                               </table>
                        
                           </td>
                       </tr>


 </table>
                      </td>        


                     
                       <td  style="width: 240px; vertical-align: top;">            

              <div class="box"  style="width: 250px">
                 <div class="input-row">
                         <asp:Label ID="Label8" runat="server" Text="Steps: " ToolTip="Maximum number of balancing steps"  Font-Size="Smaller" Font-Italic="True" ForeColor="#3333CC"></asp:Label>
                         <asp:TextBox ID="TextBoxNumberSteps" runat="server" AutoPostBack="True" Width="30px" Text="100"></asp:TextBox>&nbsp;                 
                        
                 </div>
                 <div class="input-row">
                         <asp:CheckBox ID="chkSeeSteps" runat="server" Text="see all steps"  Font-Size="Smaller" Font-Italic="True" ForeColor="#3333CC" />             
                        
                 </div>
                  <div class="input-row">
                         <asp:CheckBox ID="chkSeeLastSteps" runat="server" Text="only last three steps"  Font-Size="Smaller" Font-Italic="True" ForeColor="#3333CC" />          
                        
                 </div>
                 <div class="input-row">
                         <asp:Label ID="Label9" runat="server" Text="Precision: " ToolTip="Total Precision" Font-Size="Smaller" Font-Italic="True" ForeColor="#3333CC"></asp:Label>
                         <asp:TextBox ID="TextBoxPrecision" runat="server" AutoPostBack="True" Width="30px" Text="1"></asp:TextBox>&nbsp;                       
                 
                </div>
                        
                <div class="input-row">
                    <asp:CheckBox ID="chkAdjustByStart" runat="server" Text="adjust by start matrix"  Font-Size="Smaller" Font-Italic="True" ForeColor="#3333CC"/>
                       
                </div>

                  <div class="input-row">
                        <asp:Label ID="Label16" runat="server" Text="Partial rows/columns: " ToolTip="Balancing left top corner of Matrix and right low corner of Matrix applying coefficients to whole Matrix " Font-Size="Smaller" Font-Italic="True" ForeColor="#3333CC"></asp:Label>
                        <asp:TextBox ID="TextBoxUV" runat="server" AutoPostBack="True" Width="10px" Text="0,0" ToolTip="u,v - where u>1 rows and v>1 columns in the left top corner matrix, the right low corner matrix will start from u+1 row and v+1 column"></asp:TextBox>&nbsp;
                 </div>



             </div>
             </td>
             </tr>
            </table>
          </div>    
    </td>
</tr>





                      
                      <tr  id="tr1aBtn" runat ="server" >
                       <td>
                         &nbsp;&nbsp;&nbsp;
                         <asp:Button ID="btnBalanceSumsRowsCols1a" runat="server" CssClass="ticketbutton" Text="(1a) Balancing matrix of field1 for given above sums by rows and by columns" Visible="true" valign="center" ToolTip="Scenario 1a. Starting Matrix is for values of field1 aggregated by selected function, balancing matrix should have the sums of rows and sums of columns proportional to the manually entered ones. Field2 is ignored." />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                   
                       </td>
                      </tr>
                       <tr  id="tr1bBtn" runat ="server" >
                       <td>
                         &nbsp;&nbsp;&nbsp;
                         <asp:Button ID="btnBalanceMatrixSumsRowsCols1b" runat="server" CssClass="ticketbutton" Text="(1b) Balancing matrix of rows and multiple columns for given above sums by rows and by columns" Visible="true" valign="center" ToolTip="Scenario 1b. Starting Matrix of rows and multiple columns, balancing matrix should have the sums of rows and sums of columns proportional to the manually entered ones.  For Scenarios 1b, 3b, and 3c the selected columns are columns in the matrix." />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                   
                       </td>
                      </tr>
                      <tr  id="tr2aBtn" runat ="server" >
                       <td>
                           &nbsp;&nbsp;&nbsp;
                           <asp:Button ID="btnBalanceFlds2a" runat="server" CssClass="ticketbutton" Text="(4a) Balancing matrix of field1 for the sums by selected columns of the matrix of field2" Visible="true" valign="center" ToolTip="Scenario 2a. Starting matrix of aggregated field1 balances by sums of rows and columns of the target matrix of aggregated field2. Separate aggregations of field1 and field2." />&nbsp;
                   
                       </td>
                      </tr>
                      <tr  id="tr2bBtn" runat ="server" >
                       <td>
                           &nbsp;&nbsp;&nbsp;
                           <asp:Button ID="btnBalanceVals2b" runat="server" CssClass="ticketbutton" Text="(4b) Balancing matrix for the sums by selected columns of aggregated field1 for iterations of starting and target values of the field2" Visible="true" valign="center" ToolTip="Scenario 2b. Starting Matrix of the aggregated field1 is data with restriction of field2 value equal to starting value of field2, Target Matrix of the aggregated field1 is data with restrictions of field2 value equal to target value of the field2. Aggregation of field1 only." />&nbsp;
                         
                       </td>
                      </tr>
                      <tr  id="tr2cBtn" runat ="server" >
                       <td>
                           &nbsp;&nbsp;&nbsp;
                           <asp:Button ID="btnGetCoeffsByVals2c" runat="server" CssClass="ticketbutton" Text="(2c) Balancing coefficients for matrix of field1 values and all iterations between starting and target of the field2 values" Visible="true" valign="center" ToolTip="(2c) Balancing coefficients for matrix of field1 and all iterations by values between starting and target of the field2 values. Aggregation of field1." />&nbsp;
                       
                       </td>
                      </tr>
                      <tr  id="tr3aBtn" runat ="server" >
                       <td>
                            &nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnGetCoeffsByFields3a" runat="server" CssClass="ticketbutton" Text="(3a) Balancing coefficients for matrix of aggregated field1 values and for multiple selected aggregated fields " Visible="true" valign="center" ToolTip="Scenario 3a. Select group fields for rows and columns, field1 with aggregation function for items of Starting Matrix, and multiple fields with aggregation function for each iteration matrix. Separate aggregations of field1 and fields." />&nbsp;
                
                       </td>
                      </tr>
                      <tr  id="tr3bBtn" runat ="server" >
                       <td>
                           &nbsp;&nbsp;&nbsp;
                           <asp:Button ID="btnBalanceMatrixColumns3b" runat="server" CssClass="ticketbutton" Text="(3b) Balancing matrix of rows and multiple columns for iterations of starting and target values of the field2 " Visible="true" valign="center" ToolTip="Scenario 3b. Matrix balancing with aggregation for iterations by starting and target values of the field2. For Scenarios 1b, 3b, and 3c the selected columns are columns in the matrix, and iterations are done using field2 values as restrictions for iterations. " />&nbsp;
                 
                       </td>
                      </tr>
                      <tr  id="tr3cBtn" runat ="server" >
                       <td>
                           &nbsp;&nbsp;&nbsp;
                           <asp:Button ID="btnGetCoeffsMatrixColumns3c" runat="server" CssClass="ticketbutton" Text="(3c) Balancing coefficients for matrix of rows and multiple cols for iterations between start and target of field2 values" Visible="true" valign="center" ToolTip="Scenario 3c. Starting Matrix of rows and multiple columns balances by sums of rows and columns of the iteration matrix by values of the field2. For Scenarios 1b, 3b and 3c the selected columns are columns in the matrix, and iterations are done using field2 values as restrictions for iterations. " />&nbsp;
                 
                       </td>
                      </tr>
                      <tr  id="tr4cBtn" runat ="server" >
                       <td>
                           &nbsp;&nbsp;&nbsp;
                           <asp:Button ID="btnManuallyEntered4c" runat="server" CssClass="ticketbutton" Text="(4c) Starting Matrix of aggregated field1 to balance by manually entered sums by selected columns" Visible="true" valign="center" ToolTip="Scenario 4c. Select columns to balance by, field1 with aggregation function for items of Starting Matrix, manually enter sums for Target Matrix. Starting Matrix of aggregated field1 to balance for sums of selected columns in the Target Matrix.  " />&nbsp;
                 
                       </td>
                      </tr>
                   </table>

                        
                  
                     
            </td>
        </tr>
        <tr>
            <td align="left">
                <asp:Label ID="Label2" runat="server" Text="Balancing result:" ToolTip="Balancing result"></asp:Label>&nbsp;&nbsp;
                <br />
                <asp:Label ID="LabelResult" runat="server" Text=" " ToolTip="Balancing result"  Font-Size="Medium" ForeColor="#CC0000"></asp:Label>
               
                <asp:Label ID="LabelError" runat="server" Font-Size="Larger" ForeColor="Red"> </asp:Label>
            </td>

        </tr>
                <tr>
            <td align="left">
                <b><asp:Label ID="Label39" runat="server" Text="Target Matrix is actual or requested, Balancing Matrix is balanced expectations based on Starting Matrix and keeping the same sums for categories as in the Target Matrix. " ToolTip="See Matrix Balancing Help for more explanations."></asp:Label>&nbsp;&nbsp;
                </b>
            </td>

        </tr>
        <tr>
            <td>
                <table id="mainGrids" runat="server" width="100%" border="1" brcolor="grey">
                    <tr id="tr1">
                        <td bgcolor="lightgreen"><b><asp:Label ID="Label12" runat="server" Text="Starting Matrix" ToolTip="Starting Matrix - color compared to Target Matrix"></asp:Label></b>
                            &nbsp;&nbsp;&nbsp;&nbsp;<asp:LinkButton ID="lnkExportGrid1" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Export to CSV file. May take a long time...">Export to Excel</asp:LinkButton>
                             &nbsp;&nbsp;&nbsp;&nbsp;<asp:LinkButton ID="lnkGrid1AI" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="AI analytics. May take a long time...">AI</asp:LinkButton>  
                           
                        </td>
                        <td id="top1" width="15px"><asp:Label ID="Label19" runat="server" Text="=>"></asp:Label></td>
                        <td id="topTarget" bgcolor="lightyellow"><b><asp:Label ID="Label13" runat="server" Text="Target Matrix" ToolTip="Target Matrix - color compared to Balanced Matrix"></asp:Label></b>
                            &nbsp;&nbsp;&nbsp;&nbsp;<asp:LinkButton ID="lnkExportGrid2" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Export to CSV file. May take a long time...">Export to Excel</asp:LinkButton> 
                            &nbsp;&nbsp;&nbsp;&nbsp;<asp:LinkButton ID="lnkGrid2AI" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="AI analytics. May take a long time...">AI</asp:LinkButton> 

                        </td>

                    </tr>
                    <tr id="tr2" valign="top">
                        <td bgcolor="white" brcolor="lightgreen">
                            <asp:GridView ID="GridView1" runat="server" BackColor="WhiteSmoke" Font-Names="Arial" Font-Size="Small" PageSize="10">
                            <AlternatingRowStyle BackColor="WhiteSmoke" />
                            <RowStyle BackColor="White" />
                            </asp:GridView><br />
                        </td>
                        <td id="row1" width="15px">&nbsp;&nbsp;</td>
                        <td id="rowtarget" bgcolor="white" brcolor="lightyellow">
                            <asp:GridView ID="GridView2" runat="server" BackColor="WhiteSmoke" Font-Names="Arial" Font-Size="Small" PageSize="10">
                            <AlternatingRowStyle BackColor="WhiteSmoke" />
                            <RowStyle BackColor="White" />
                            </asp:GridView><br />
                        </td>
                    </tr>
                    
                    <tr id="tr3">
                        <td bgcolor="white"> </td> 
                        <td width="15px">&nbsp;&nbsp;</td>
                        <td bgcolor="white"><%--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>
                            <asp:LinkButton ID="lnkCompareTargetBalancingSums" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="AI analytics. May take a long time...">AI: Target Sums <=> Balancing Sums</asp:LinkButton>

                        </td>

                    </tr>
                    <tr id="tr4">
                        <td bgcolor="Cyan"><b><asp:Label ID="Label27" runat="server" Text="Balancing coefficients" ToolTip="Starting Values by selected fields and final Balancing Coefficients, Precisions, and Values. Original values color compared to final balancing values."></asp:Label>
                            &nbsp;&nbsp;&nbsp;&nbsp;<asp:HyperLink ID="HyperLinkChart" runat="server" NavigateUrl="" Target="_blank" CssClass="NodeStyle" Font-Names="Arial">Chart</asp:HyperLink></b>
                            &nbsp;&nbsp;&nbsp;&nbsp;<asp:LinkButton ID="lnkExportGrid4" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Export to CSV file. May take a long time...">Export to Excel</asp:LinkButton> 
                             &nbsp;&nbsp;&nbsp;&nbsp;<asp:LinkButton ID="lnkGrid4AI" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="AI analytics. May take a long time...">AI</asp:LinkButton>
            
                        </td>
                        <td id="top2" width="15px">&nbsp;&nbsp;</td>
                        <td id="topBalance" bgcolor="#f4dd98"><b><asp:Label ID="Label14" runat="server" Text="Balancing Matrix" ToolTip="Balancing Matrix - color compared to Target Matrix"></asp:Label></b> 
                            &nbsp;&nbsp;&nbsp;&nbsp;<asp:LinkButton ID="lnkExportGrid3" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Export to CSV file. May take a long time...">Export to Excel</asp:LinkButton> 
                             &nbsp;&nbsp;&nbsp;&nbsp;<asp:LinkButton ID="lnkGrid3AI" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="AI analytics. May take a long time...">AI</asp:LinkButton>

                        </td>
                    </tr>

                    <tr id="tr5" valign="top">
                        <td bgcolor="white" brcolor="lightpink">
                            <asp:GridView ID="GridView4" runat="server" BackColor="WhiteSmoke" Font-Names="Arial" Font-Size="Small" PageSize="10">
                            <AlternatingRowStyle BackColor="WhiteSmoke" />
                            <RowStyle BackColor="White" />
                            </asp:GridView><br />
                        </td>
                         <td id="row2" width="15px">&nbsp;&nbsp;</td>
                        <td id="rowBalance" bgcolor="white" brcolor="lightyellow">
                            <asp:GridView ID="GridView3" runat="server" BackColor="WhiteSmoke" Font-Names="Arial" Font-Size="Small" PageSize="10">
                            <AlternatingRowStyle BackColor="WhiteSmoke" />
                            <RowStyle BackColor="White" />
                            </asp:GridView><br />
                        </td>
                    </tr>
                    <tr id="trbal1"><td bgcolor="white"  runat="server">&nbsp;</td><td bgcolor="white" >&nbsp;</td><td bgcolor="white" >&nbsp;</td> </tr>
                    <tr id="trbal2">
                        <td bgcolor="Cyan"><b><asp:Label ID="Label20" runat="server" Text="Balancing coefficients for Whole Matrix" ToolTip="Balancing coefficients for Whole Matrix"></asp:Label></b>
                            &nbsp;&nbsp;&nbsp;&nbsp;<asp:LinkButton ID="lnkExportGrid6" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Export to CSV file. May take a long time...">Export to Excel</asp:LinkButton> 
                             &nbsp;&nbsp;&nbsp;&nbsp;<asp:LinkButton ID="lnkGrid6AI" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="AI analytics. May take a long time...">AI</asp:LinkButton>

                        </td>
                        <td bgcolor="white" >&nbsp;</td>                       
                        <td bgcolor="#f4dd98"  runat="server" ><b><asp:Label ID="Label15" runat="server" Text="Balancing of Whole Matrix" ToolTip="Balancing Whole Matrix - not partional, color compare to partially balanced"></asp:Label></b>
                           &nbsp;&nbsp;&nbsp;&nbsp;<asp:LinkButton ID="lnkExportGrid5" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Export to CSV file. May take a long time...">Export to Excel</asp:LinkButton> 
                            &nbsp;&nbsp;&nbsp;&nbsp;<asp:LinkButton ID="lnkGrid5AI" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="AI analytics. May take a long time...">AI</asp:LinkButton> 


                        </td>
                        
                    </tr>
                    <tr valign="top" id="trbal3"  runat="server">
                        <td bgcolor="white" brcolor="lightpink" >
                            <asp:GridView ID="GridView6" runat="server" BackColor="WhiteSmoke" Font-Names="Arial" Font-Size="Small" PageSize="10">
                            <AlternatingRowStyle BackColor="WhiteSmoke" />
                            <RowStyle BackColor="White" />
                            </asp:GridView><br />
                        </td>  
                        <td bgcolor="white" >&nbsp;</td>
                        <td bgcolor="white" brcolor="lightyellow">
                            <asp:GridView ID="GridView5" runat="server" BackColor="WhiteSmoke" Font-Names="Arial" Font-Size="Small" PageSize="10">
                            <AlternatingRowStyle BackColor="WhiteSmoke" />
                            <RowStyle BackColor="White" />
                            </asp:GridView><br />
                        </td>
                    </tr>
                   <tr  id="trbal4"  runat="server">
                       <td align="left" colspan="3">               
                          <asp:Label ID="LabelCompare" runat="server" Text=" " ToolTip="Compare partially ballanced with whole matrix balanced"  Font-Size="Medium" ForeColor="#CC0000"></asp:Label>               
                       </td>
                   </tr>
                </table>
            </td>  
        </tr>
        
    </table>       
            
      <table id="TableHelp" runat="server" width="100%" border="1" brcolor="white">
           <tr>
               <td >
                   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                   <asp:Label ID="Label32" runat="server" Text="Iterations and balancing of starting matrix to target(s)" Font-Size="Large" ForeColor="Maroon" Font-Bold="true"></asp:Label>
               </td>
               <td colspan="3">
                   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                   <asp:Label ID="Label33" runat="server" Text="Matrix structure (rows, columns, items)" Font-Size="Large" ForeColor="Maroon" Font-Bold="true"></asp:Label>
               </td>
           </tr>
           <tr>
               <td>
                   <asp:Label ID="Label34" runat="server" Text="Sums by rows and sums by columns (2-dimensional balancing):" Font-Size="Large" ForeColor="Maroon" Font-Bold="true"></asp:Label>
                                                    
               </td>
               <td>
                   Rows defined by values in group row field.
               </td>
               <td>
                   Columns defined by values in group column field. Starting matrix item is aggregated field1 value for row/column group.
               </td>
               <td>
                   Columns selected from the list of data columns. Starting matrix item is the value of the row/column item.
               </td>
           </tr>
            <tr>
               <td>
                   Balance to sums by rows and sums by columns entered manually.  
               </td>
               <td>
                   1a, 1b, 2a, 2b, 2c, 3a, 3b, 3c
               </td>
               <td>                   
                   <asp:HyperLink ID="HyperLink7" runat="server" NavigateUrl="https://oureports.net/oureports/MatrixBalancing.pdf#page=5" CssClass="NodeStyle" Font-Names="Arial" Target="_blank">Scenario 1a</asp:HyperLink>
               </td>
               <td>
                   <asp:HyperLink ID="HyperLink8" runat="server" NavigateUrl="https://oureports.net/oureports/MatrixBalancing.pdf#page=5" CssClass="NodeStyle" Font-Names="Arial" Target="_blank">Scenario 1b</asp:HyperLink>
               </td>
           </tr>
            <tr>
               <td>
                   Balance to sums by rows and sums by columns of the target matrix. Target matrix item is aggregated field2 value for row/column group. 
               </td>
               <td>
                   1a, 1b, 2a, 2b, 2c, 3a, 3b, 3c
               </td>
               <td>
                   <asp:HyperLink ID="HyperLink9" runat="server" NavigateUrl="https://oureports.net/oureports/MatrixBalancing.pdf#page=5" CssClass="NodeStyle" Font-Names="Arial" Target="_blank">Scenario 2a</asp:HyperLink>
               </td>
               <td>
                  <%-- <asp:HyperLink ID="HyperLink16" runat="server" NavigateUrl="~/MatrixBalancing.pdf#page=8" CssClass="NodeStyle" Font-Names="Arial" Target="_blank">Scenario 4a (balancing by multiple fields)</asp:HyperLink>--%>
              
               </td>
           </tr>
            <tr>
               <td>
                   Multiple balancing to get balancing coefficients as multiple 2a scenarios for each selected field from the list of columns. Balance to sums by rows and sums by columns for target matrices. Target matrix item is aggregated selected field value for row/column group.

               </td>
               <td>
                   1a, 1b, 2a, 2b, 2c, 3a, 3b, 3c
               </td>
               <td>
                  <asp:HyperLink ID="HyperLink10" runat="server" NavigateUrl="https://oureports.net/oureports/MatrixBalancing.pdf#page=7" CssClass="NodeStyle" Font-Names="Arial" Target="_blank">Scenario 3a</asp:HyperLink>
               </td>
               <td>                  
                  <%--<asp:HyperLink ID="HyperLink15" runat="server" NavigateUrl="~/MatrixBalancing.pdf#page=8" CssClass="NodeStyle" Font-Names="Arial" Target="_blank">Scenario 4b (balancing by multiple fields)</asp:HyperLink>--%>
               </td>
           </tr>
            <tr>
               <td>
                   Field2 starting and target values used as condition on data to get starting and target matrices. 
               </td>
               <td>
                   1a, 1b, 2a, 2b, 2c, 3a, 3b, 3c
               </td>
               <td>
                   <asp:HyperLink ID="HyperLink11" runat="server" NavigateUrl="https://oureports.net/oureports/MatrixBalancing.pdf#page=6" CssClass="NodeStyle" Font-Names="Arial" Target="_blank">Scenario 2b</asp:HyperLink>
               </td>
               <td>
                   <asp:HyperLink ID="HyperLink12" runat="server" NavigateUrl="https://oureports.net/oureports/MatrixBalancing.pdf#page=7" CssClass="NodeStyle" Font-Names="Arial" Target="_blank">Scenario 3b</asp:HyperLink>               
               </td>
           </tr>
            <tr>
               <td>
                   Multiple balancing to get balancing coefficients as multiple scenarios of the scenario in the row above. Field2 starting value used as condition on data to get starting matrix and set of target matrices defined by each value between starting and target values of field2.

               </td>
               <td>
                   1a, 1b, 2a, 2b, 2c, 3a, 3b, 3c
               </td>
               <td>
                   <asp:HyperLink ID="HyperLink13" runat="server" NavigateUrl="https://oureports.net/oureports/MatrixBalancing.pdf#page=6" CssClass="NodeStyle" Font-Names="Arial" Target="_blank">Scenario 2c</asp:HyperLink>
               </td>
               <td>
                   <asp:HyperLink ID="HyperLink14" runat="server" NavigateUrl="https://oureports.net/oureports/MatrixBalancing.pdf#page=7" CssClass="NodeStyle" Font-Names="Arial" Target="_blank">Scenario 3c</asp:HyperLink>
               </td>
           </tr>
            <tr>
               <td>
                   <asp:Label ID="Label38" runat="server" Text="Multidimensional balancing of sums by multiple selected columns:" Font-Size="Large" ForeColor="Maroon" Font-Bold="true"></asp:Label>
                   
               </td>
               <td>
                   Sums by selected columns entered manually. Starting matrix item is the aggregated field1 value for particular values in selected columns.&nbsp;<br />
                   <asp:HyperLink ID="HyperLink19" runat="server" NavigateUrl="https://oureports.net/oureports/MatrixBalancing.pdf#page=8" CssClass="NodeStyle" Font-Names="Arial" Target="_blank">Scenario 4a</asp:HyperLink>
               </td>
               <td>
                   Starting matrix item is the aggregated field1 value for particular values in selected columns. Target matrix item is aggregated field2 value for particular values in selected columns.
                  <br /> <asp:HyperLink ID="HyperLink17" runat="server" NavigateUrl="https://oureports.net/oureports/MatrixBalancing.pdf#page=8" CssClass="NodeStyle" Font-Names="Arial" Target="_blank">Scenario 4b</asp:HyperLink>
               </td>
               <td>
                   Field2 starting and target values used as condition on data to get starting and target matrices of field1 aggregated values for particular values in selected columns.  
                  <br /> <asp:HyperLink ID="HyperLink18" runat="server" NavigateUrl="https://oureports.net/oureports/MatrixBalancing.pdf#page=9" CssClass="NodeStyle" Font-Names="Arial" Target="_blank">Scenario 4c</asp:HyperLink>
               </td>
           </tr>
       </table>
          <br /><br />
        <br /> 
         <div align="left" backcolor="Gray"  style="background-color: gray; font-family: Arial, Helvetica, sans-serif; font-size: medium; font-weight: bold; color: #FFFFFF;">
                     &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                   

         
         </div>
        
        <br />
        <br />
        <br />
        <br />
        <br />
        </div>

 </div>
                  </td>
        </tr>

    </table>


    <div id="spinner" class="modal" style="display:none;">
       <div id="divSpinner" style="text-align: center; width: 130px; display: inline-block; clip: rect(auto, auto, auto, 50%); position: fixed; margin-top: 300px; margin-right: auto; padding-top: 10px; background-color: #f8f8d3; z-index: 2147483647; left: 50%; top: -5%;">
           <img id="imgSpinner" src="Controls/Images/WaitImage2.gif" style="width: 100px; height: 100px" />
           <br />
           Please Wait...
       </div>
    </div>   

        <ucMsgBox:Msgbox id="MessageBox" runat ="server" > </ucMsgBox:Msgbox>
        <ucDlgTextbox:DlgTextbox id="dlgTextbox" runat="server" />
      </ContentTemplate>
           <Triggers> 
            <asp:PostBackTrigger ControlID="btnBalanceFlds2a"/>
            <asp:PostBackTrigger ControlID="btnBalanceMatrixColumns3b"/>
            <asp:PostBackTrigger ControlID="btnBalanceMatrixSumsRowsCols1b"/>
            <asp:PostBackTrigger ControlID="btnBalanceSumsRowsCols1a"/>
            <asp:PostBackTrigger ControlID="btnBalanceVals2b"/>
            <asp:PostBackTrigger ControlID="btnGetCoeffsByFields3a"/>
            <asp:PostBackTrigger ControlID="btnGetCoeffsByVals2c"/>
            <asp:PostBackTrigger ControlID="btnGetCoeffsMatrixColumns3c"/>
          </Triggers>
      </asp:UpdatePanel>   
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpAdvancedAnalytics">
                <ProgressTemplate >
                <div class="modal">
                    <div class="center">
                       <asp:Image ID="imgProgress" runat="server"  ImageAlign="AbsMiddle" ImageUrl="~/Controls/Images/WaitImage2.gif" />
                       Please Wait...
                   </div>
                </div>
                </ProgressTemplate>
        </asp:UpdateProgress>     
    </form>
</body>
</html>
