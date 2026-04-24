<%@ Page Language="VB" AutoEventWireup="false" CodeFile="MapReport.aspx.vb" Inherits="MapReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script type="text/javascript" src="Controls/Javascripts/OUR.js"></script>

    <title>Map format</title>
    <style type="text/css">
        .ColumnTitle{
            height: 27px;
            width: 100%;
            font-weight: bold;
            color: White; 
            font-family: Arial;
            background-color: #e5e5e5;
            font-size: medium;
        }

        .ColumnHeader {
             white-space: nowrap;
             text-align: left;
             height:24px;
             font-weight: bold;
             font-size: small;
             color: white;
             font-family: Arial; 
             background-color: #e5e5e5;
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
.center
{
    z-index: 2147483647;
    margin: 300px auto;
    padding-left:25px;
    padding-top:10px;
    width: 130px;
    background-color:#f8f8d3;
    border-radius: 10px;
    opacity: 1;
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

        .auto-style1 {
            margin-left: 0px;
        }

        .auto-style5 {
            width: 59px;
        }

        .auto-style9 {
            width: 1289px;
        }
        .auto-style10 {
            width: 514px;
        }
.ticketbutton 
{
  width: 80px;
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
        </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:HiddenField ID="hdnShowLinks" runat="server" Value="False" />
        <asp:HiddenField ID="hdnShowCircles" runat="server" Value="False" />
        <asp:HiddenField ID="hdnShowPins" runat="server" Value="False" />

        <asp:HiddenField ID="hdnLineWidth" runat="server" Value="" />
        <asp:HiddenField ID="hdnInitAltitude" runat="server" Value="" />

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
        <asp:UpdatePanel ID="udpMAPformat" runat ="server">
            <ContentTemplate>

 <table>
      <tr>
          <td colspan="3" style="font-size:x-large; font-style:normal; font-weight:bold; background-color: #e5e5e5; vertical-align:middle; text-align: left; height: 40px;">
              <asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reporting"></asp:Label>
          </td>
      </tr> 
        <tr>
        <td style="font-size: x-small; font-style: normal; font-weight: normal; background-color: #e5e5e5; vertical-align: top; text-align: left; width: 15%;">
           <div id="tree">
             <asp:TreeView ID="TreeView1"  runat="server" Width="100%" NodeIndent="10" Font-Names="Times New Roman"  EnableTheming="True" ImageSet="BulletedList">
                    <Nodes>  
              <asp:TreeNode Text="&lt;b&gt;Log Off;&lt;/b&gt;"  Value="Default.aspx" Expanded="True" ></asp:TreeNode>

              <asp:TreeNode Text="&lt;b&gt;List of Reports&lt;/b&gt;"  Value="ListOfReports.aspx" Expanded="True" ></asp:TreeNode>                              
                    
              <asp:TreeNode Text="&lt;b&gt;Report Definition&lt;/b&gt;"  Value="ReportEdit.aspx?tne=2" Expanded="True" ></asp:TreeNode>

              <asp:TreeNode Text="&lt;b&gt;Report Parameters&lt;/b&gt;"  Value="ReportEdit.aspx?tne=3" Expanded="True" ></asp:TreeNode>

              <%--<asp:TreeNode Text="&lt;b&gt;Share Report (Users)&lt;/b&gt;"  Value="ReportEdit.aspx?tne=4" Expanded="True" ></asp:TreeNode>--%>
 
            <asp:TreeNode Text="Report Data Query"  Value="SQLquery.aspx?tnq=0" Expanded="True" >
                 <asp:TreeNode Text="Data fields" Value="SQLquery.aspx?tnq=0" > </asp:TreeNode>
                 <asp:TreeNode Text="Joins" Value="SQLquery.aspx?tnq=1"> </asp:TreeNode>
                 <asp:TreeNode Text="Filters"     Value="SQLquery.aspx?tnq=2" > </asp:TreeNode>
                 <asp:TreeNode Text="Sorting"     Value="SQLquery.aspx?tnq=3" > </asp:TreeNode>                 
            </asp:TreeNode>
            
            <asp:TreeNode Text="Report Format Definition"  Value="RDLformat.aspx?tnf=0" Expanded="True" >
                 <asp:TreeNode Text="Advanced Report Designer"  Value="ReportDesigner.aspx" > </asp:TreeNode>
                 <asp:TreeNode Text="Columns, Expressions"  Value="RDLformat.aspx?tnf=0" > </asp:TreeNode>
                 <asp:TreeNode Text="Groups, Total"          Value="RDLformat.aspx?tnf=1" > </asp:TreeNode>
                 <asp:TreeNode Text="Combine Values"          Value="RDLformat.aspx?tnf=2" > </asp:TreeNode>
                 <asp:TreeNode Text="Map Definition"          Value="MapReport.aspx" > </asp:TreeNode>
            </asp:TreeNode>
           
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
       <RootNodeStyle HorizontalPadding="2px" Font-Bold="True" />
       <NodeStyle CssClass="NodeStyle" />
       <ParentNodeStyle Font-Bold="True" />
     </asp:TreeView>
    
    </div>
    </td>
  <td style="width: 5px"></td>
  <td id="main" style="width: 85%; text-align: left; vertical-align: top"> 
                <table id="tblTitle" style="width:100%; font-family: Arial;background-color: #e5e5e5;">
                  <tr>
                    <td style="width:61%;text-align:right">
                       <asp:Label ID="LabelAlert0" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Large" ForeColor="Gray" Text="Map Report Definition"></asp:Label>  &nbsp;&nbsp;
                       <asp:Label ID="LabelReportName" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Large" ForeColor="Gray" Text=" "></asp:Label>
                       <br /><asp:Label ID="LabelAlert" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Large" ForeColor="Gray" Text=" "></asp:Label>
                    </td>
                    <td style="width:15%;text-align:center;">
                        &nbsp;</td>
<%--                    <td style="width:13%;text-align:center;">
                      <asp:HyperLink ID="HyperLink6" runat="server" NavigateUrl="~/ShowReport.aspx?srd=3" ToolTip="Run Report" CssClass="NodeStyle" Font-Size="12px" Visible="False" Enabled="False">Show report data</asp:HyperLink>    
                    </td>--%>
                    <td style="width:20%;text-align:center;">
                      <asp:HyperLink ID="HyperLinkHelp" runat="server" NavigateUrl="DataAIHelp.aspx?hilt=Map_Definition" Target="_blank" CssClass="NodeStyle" Font-Size="12px">Help</asp:HyperLink>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:HyperLink ID="HyperLinkLogOff" runat="server" NavigateUrl="~/Default.aspx" CssClass="NodeStyle" Font-Names="Arial">Log off</asp:HyperLink> 
                    </td>                                          
                  </tr>
                </table> 

         <div>

            <table style="border-style:inherit;border-width: 0px; padding:0px;margin:0px;background-color:#e5e5e5;border:medium double #FFFFFF;width:100%;">
                        <tr valign="top">
                            <td  style="color: black; font-family: Arial; background-color: #e5e5e5;border-width: 0px;width:100%;">
                                <table id="Table1" runat="server" bgcolor="#e5e5e5;" border="0" width="100%" style="vertical-align: middle; text-align: left; background-color: #e5e5e5; border-style: inherit; border-width: 0px; border-color: #e5e5e5" >
                                    <tr> 

                                        <td style="color: black; font-family: Arial; letter-spacing: normal;
                                                 background-color: #e5e5e5;border-width: 0px;">
                                           <asp:Label ID="Label2" runat="server" Font-Underline="False" Text="   Map type:"></asp:Label>
                                            &nbsp;&nbsp;&nbsp;<asp:Label ID="Label3" runat="server" Text=" "></asp:Label>
                                        <%--</td>
                                         <td  style="color: black; font-family: Arial; letter-spacing: normal;
                                                 background-color: #e5e5e5;border-width: 0px;">--%>
                                            &nbsp;&nbsp;<asp:DropDownList ID="DropDownMapType" runat="server" AutoPostBack="True">
                                                <asp:ListItem>Pins</asp:ListItem>
                                                <asp:ListItem>Circles</asp:ListItem> 
                                                <asp:ListItem>Paths</asp:ListItem> 
                                               <%-- <asp:ListItem>Extruded Paths</asp:ListItem>  --%>                                             
                                                <asp:ListItem>Polygons</asp:ListItem> 
                                                <%--<asp:ListItem>Extruded Polygons</asp:ListItem>--%>
                                                <asp:ListItem>Tours</asp:ListItem>
                                                <%--                                                                                                      
                                                    <asp:ListItem>Floating placemark</asp:ListItem>
                                                    <asp:ListItem>Extruded placemark</asp:ListItem> 
                                                --%>
                                            </asp:DropDownList>
                                            &nbsp;
                                        </td>
                                        <td  style="color: black; font-family: Arial; letter-spacing: normal;
                                                 background-color: #e5e5e5;border-width: 0px;">
                                            <asp:Label ID="Label4" runat="server" Font-Underline="False" Text="Maps:"></asp:Label>
                                            &nbsp;<asp:Label ID="Label5" runat="server" Text=" "> </asp:Label>
                                    
                                       <%-- </td>
                                        <td style="color: #ffffff; font-family: Arial; letter-spacing: normal;
                                                 background-color: #e5e5e5;border-width: 0px; ">--%>
                                            &nbsp;<asp:DropDownList ID="DropDownMapNames" runat="server" AutoPostBack="True" ToolTip="If selected map name included * the map was from the history">
                                                <asp:ListItem> </asp:ListItem>
                                                
                                            </asp:DropDownList>
                                            &nbsp;
                                        </td>                                       
                                        <td style="color: black; font-family: Arial; letter-spacing: normal;
                                                 background-color: #e5e5e5;border-width: 0px;  ">
                                            <asp:Label ID="Label1" runat="server" Font-Underline="False" Text="Map Name:"></asp:Label>
                                           <asp:textbox ID="txtMapName" runat="server" Text=" " Width="356px" ToolTip='If map name included # the map was from the history'></asp:textbox> &nbsp;
                                             <asp:Button ID="ButtonAddMapName" runat="server" CssClass="ticketbutton" Text="add/update" />&nbsp;&nbsp;&nbsp;
                                            <asp:Button ID="ButtonDeleteMap" runat="server" CssClass="ticketbutton" Text="del" />&nbsp;&nbsp;&nbsp;
                                        </td>                                  
                                       
                                  
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr valign="top">
                            <td align="left" class="auto-style9">
                                <table id="Table2" runat="server" bgcolor="#e5e5e5" rules="rows" width="1400px" style="border:medium double #FFFFFF; font-size: small;
                                        color: black; font-family: Arial; background-color: #e5e5e5; vertical-align: top;" >
                                  <tr valign="top" runat ="server"  border="3" style="color: black; font-family: Arial; border:medium double #FFFFFF; background-color: #e5e5e5;border-width: 2px;width:100%;" >
                                    
                                    <td>

                                      <table style="border: 2px solid #FFFFFF; width: 700px;">
                                          <tr>
                                              <td style="font-size: medium; color: black; border:medium double #FFFFFF;
                                                 background-color: #e5e5e5; font-weight: bold;">
                                              &nbsp;&nbsp;Fields for placemarks:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;
                                             </td>
                                          </tr>

                                        <tr>
                                         <td style="font-size: small; color: black; border:medium double #FFFFFF;
                                                 background-color: #e5e5e5; font-weight: normal;">
                                              &nbsp;&nbsp;Field:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;
                                          <br />
                                              &nbsp;&nbsp;<asp:DropDownList ID="DropDownMapFields" runat="server" AutoPostBack="False">
                                            </asp:DropDownList>
                                          <br />

                                            &nbsp;<asp:Button ID="btnPlacemarkName" runat="server" CssClass="ticketbutton" Text="Placemark Name" Width="190px" />
                                           <br />
                                            &nbsp;<asp:Button ID="btnPlacemarkDescription" runat="server" CssClass="ticketbutton" Text="Placemark Description" Width="16px" Visible="False" />
                                            <br />
                                            &nbsp;<asp:Button ID="btnPlacemarkLongitude" runat="server" CssClass="ticketbutton" Text="Placemark Longitude" Width="190px" ToolTip="If pin coordinates define by address, select the address field as longitude and latitude. " />                                            
                                            &nbsp;<asp:Button ID="btnPlacemarkLatitude" runat="server" CssClass="ticketbutton" Text="Placemark Latitude" Width="190px" ToolTip="If pin coordinates define by address, select the address field as longitude and latitude." />
                                          <br />  
                                           &nbsp;<asp:Button ID="btnPlacemarkLongitude2" runat="server" CssClass="ticketbutton" Text="Placemark Longitude End" Width="190px" ToolTip="If pin coordinates define by address, select the address field as longitude and latitude." />
                                            &nbsp;<asp:Button ID="btnPlacemarkLatitude2" runat="server" CssClass="ticketbutton" Text="Placemark Latitude End" Width="190px" ToolTip="If pin coordinates define by address, select the address field as longitude and latitude." />
                                           <br />&nbsp;or if coordinates are in geolocation format:  POINT(latitude,longitude) or (latitude,longitude): 
                                           
                                            &nbsp;<asp:CheckBox ID="chkLatLonOrLonLat" runat="server" Checked="False" Text="(lat,lon)" ToolTip="Coordinates in order (lat,lon) or (lon,lat)" AutoPostBack="True" />
                                            <br />
                                           &nbsp;<asp:Button ID="btnPlacemarkGeolocation" runat="server" CssClass="ticketbutton" Text="Placemark Geolocation" Width="190px" ToolTip="If coordinates are in geolocation format:  POINT(latitude,longitude) or (latitude,longitude)." />                                         
                                            
                                            &nbsp;<asp:Button ID="btnPlacemarkGeolocation2" runat="server" CssClass="ticketbutton" Text="Placemark Geolocation End" Width="190px" ToolTip="If coordinates are in geolocation format:  POINT(latitude,longitude) or (latitude,longitude)." />
                                          <br />
                                            &nbsp;<asp:Button ID="btnPlacemarkStartTime" runat="server" CssClass="ticketbutton" Text="Placemark Start Time" Width="190px" />
                                            &nbsp;<asp:Button ID="btnPlacemarkEndTime" runat="server" CssClass="ticketbutton" Text="Placemark End Time" Width="190px" />
                                          <br />
                                        </td>

                                      <tr valign="top"  runat ="server"  id="trAddDescr" border="3" >
                                        
                                        <td style="font-size: small; color: black;    border:medium double #FFFFFF;
                                                 background-color: #e5e5e5; font-weight: normal;"  >
                                            <br />
                                            &nbsp;&nbsp;Text for description in balloon:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;
                                            <br /> &nbsp;&nbsp;<asp:TextBox ID="txtDescr" runat="server" CssClass="auto-style1" Width="400px"></asp:TextBox>
                                            <br /> &nbsp;&nbsp;Fields for description in balloon:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;
                                            <br />
                                             &nbsp;&nbsp;<asp:DropDownList ID="DropDownMapFields1" runat="server" AutoPostBack="False">
                                            </asp:DropDownList>
                                            <br />
                                            &nbsp;<asp:Button ID="btnAddToDescription" runat="server" CssClass="ticketbutton" Text="Add To Description" Width="190px" />    
                                           <br /> &nbsp;
                                        </td>
                                      </tr>
                                      <tr valign="top"  runat ="server"  id="trAddPoints" border="3" >
                                        
                                        <td align="left" style="font-size: small; color: black;    border:medium double #FFFFFF;
                                                 background-color: #e5e5e5; font-weight: normal;" valign="top" class="auto-style10" >
                                         <br />
                                         &nbsp;&nbsp;Fields for additional Placemarks:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;
                                         <br />
                                            <%--Longitude:--%>
                                            &nbsp;&nbsp;<asp:DropDownList ID="DropDownMapFields2" runat="server" AutoPostBack="False">
                                            </asp:DropDownList>
                                           &nbsp; <%--Latitude:--%>
                                           <asp:DropDownList ID="DropDownMapFields3" runat="server" AutoPostBack="False" Visible="False">
                                           </asp:DropDownList>
                                            <%-- &nbsp;Select for additional placemark coordinates:--%>
                                           <br />
                                            &nbsp;&nbsp;<asp:Button ID="btnAddPointLong" runat="server" CssClass="ticketbutton" Text="Add Placemark Longitude" Width="200px" />                                            
                                            &nbsp;&nbsp;<asp:Button ID="btnAddPointLat" runat="server" CssClass="ticketbutton" Text="Add Placemark Latitude" Width="200px" />                                            
                                           <br />   
                                           &nbsp;&nbsp;or if coordinates are in geolocation format:  POINT(latitude,longitude):
                                           &nbsp;&nbsp;<asp:CheckBox ID="chkLatLonOrLonLatAdd" runat="server" Checked="False" Text="(lat,lon)" ToolTip="Coordinates in order (lat,lon) or (lon,lat)" AutoPostBack="True" />
                                           <br />
                                            &nbsp;&nbsp;<asp:Button ID="btnAddPointGeolocation" runat="server" CssClass="ticketbutton" Text="Add Placemark Geolocation" Width="200px" ToolTip="If coordinates are in geolocation format:  POINT(latitude,longitude)."/>                                            
                                          
                                            <br /> &nbsp;

                                        </td>


                                      </tr>

                                       <tr valign="top"  runat ="server"  id="trKeyFields" border="3" >
                                       <td align="left" style="font-size: small; color: black;    border:medium double #FFFFFF;
                                                 background-color: #e5e5e5; font-weight: normal;" valign="top" class="auto-style10" >
                                          <br />
                                           &nbsp;&nbsp;Key Fields for additional records from data table :&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;
                                           <br />
                                           &nbsp;&nbsp;<asp:DropDownList ID="DropDownKeyFields" runat="server" AutoPostBack="False">
                                            </asp:DropDownList>
                                           <br /> 
                                           &nbsp;&nbsp;<asp:Button ID="btnAddKeyField" runat="server" CssClass="ticketbutton" Text="Add Key Fields for additional records" Width="312px" />                                                                                      
                                           <br /> &nbsp;
                                        </td>


                                      </tr>

                                      <tr>


                                      </tr>
                                   </table
                                 </td>
                                        
                                 <td>

                                    <table style="border: 2px solid #FFFFFF; width: 700px;">
                                         <tr>
                                              <td style="font-size: medium; color: black; border:medium double #FFFFFF;
                                                 background-color: #e5e5e5; font-weight: bold;">
                                              &nbsp;&nbsp;Map presentation:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;
                                             </td>
                                          </tr>
                                        <tr>
                                         <td align="left"  nowrap="nowrap" style="font-weight: normal; font-size: small; color: black; border:medium double #FFFFFF;
                                                font-family: Arial; background-color: #e5e5e5; " valign="top" class="auto-style116">
                                             &nbsp;&nbsp;:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                            <br />
                                            &nbsp;&nbsp;<asp:CheckBox ID="chkShowPins" runat="server" Checked="False" Text="show pins" ToolTip="Show placemark pins on the map" />
                                           
                                            <br />
                                            &nbsp;&nbsp;<asp:CheckBox ID="chkShowCircles" runat="server" Checked="False" Text="show circles around pins" ToolTip="Show circles around pins" />
                                            <br />                                                                                                                            
                                          
                                            &nbsp;&nbsp;<asp:CheckBox ID="chkShowLinks" runat="server" Text="show links to data reports in ballons" ToolTip="Show link to reports in the description balloon" />
                                            <br /><br />
                                            &nbsp;
                                            Geo restrictions: <asp:TextBox ID="txtGeoRestrictions" runat="server" CssClass="auto-style1" Width="100px" ToolTip="Format: country:US[&state:AZ]..." AutoPostBack="True"></asp:TextBox>
                                            &nbsp;<asp:Button ID="btnUpdateGeoRestrictions" runat="server" CssClass="ticketbutton" Text="Update GeoRestrictions" Width="150px" Visible="False" />                                            
                                                                                      
                                           
                                            <br /> &nbsp;
                                         </td>
                                        </tr>
                                        <tr valign="top"  runat ="server"  id="trStyle" border="3" >
                                       
                                         <td align="left"  nowrap="nowrap" style="font-size: small; color: black;    border:medium double #FFFFFF;
                                                font-family: Arial; background-color: #e5e5e5; " valign="top" class="auto-style116">                                        
                                             <br />
                                           &nbsp;&nbsp;Numeric field for color density and circle radius:&nbsp;<asp:DropDownList ID="DropDownListColorDens" runat="server">
                                            </asp:DropDownList>&nbsp;
                                           <br />                                      
                                          
                                           &nbsp;&nbsp;Highest density color:
                                           
                                           <input type="color" name="colornum" value="<%=Session("color") %>" id="colornum" class="auto-style5">
                                           &nbsp;<asp:Label ID="LabelColorSaved" runat="server" Text="saved" BackColor="White" Font-Bold="True" ToolTip="The color is selected for highest density. Low density - lighter color."></asp:Label>
                                          
                                            <br />
                                            &nbsp; Multiply to adjust the radius by:
                                           <asp:TextBox ID="txtMultiplyBy0" runat="server" Width="31px">0.01</asp:TextBox>

                                           <br /> &nbsp;<asp:Button ID="btnColorDensity" runat="server" CssClass="ticketbutton" Text="Update Color and Field for color density" Width="313px" />                                                                                      
                                           <br /> &nbsp;
                                         </td>
                                        </tr>
                                        <tr valign="top"  runat ="server"  id="trExtruded" border="3" >
                                       
                                         <td align="left" valign="top"  nowrap="nowrap" style="font-size: small; color: black;  border:medium double #FFFFFF;
                                                font-family: Arial; background-color:  #e5e5e5; "  >
                                           
                                          <br />
                                           &nbsp;&nbsp;Numeric field for extruded altitude:&nbsp;
                                            <asp:DropDownList ID="DropDownListExtruded" runat="server">
                                            </asp:DropDownList>
                                            <br />&nbsp; Multiply by:
                                           <asp:TextBox ID="txtMultiplyBy" runat="server" Width="31px">10</asp:TextBox>
                                           &nbsp;
                                            <br />
                                           &nbsp;&nbsp;Initial altitude:<asp:TextBox ID="txtInitialAltit" runat="server" Width="38px">4000</asp:TextBox>
                                           &nbsp; Line width:<asp:TextBox ID="txtLineWidth" runat="server" Width="25px">4</asp:TextBox>
                                           <br /> 
                                            &nbsp;&nbsp;Extruded color based on value in the field:&nbsp;&nbsp;
                                           <asp:DropDownList ID="DropDownListExtrudedColor" runat="server" ToolTip="Select the field contains the color code. If not selected the density color will be used.">
                                           </asp:DropDownList>
                                           &nbsp; 
                                           <br /> &nbsp;<asp:Button ID="btnExtrudedAltitude" runat="server" CssClass="ticketbutton" Text="Update field for extruded altitude and color" Width="314px" />                                                                                      
                                            <br />&nbsp;
                                         </td>
                                        </tr> 
                                        <tr>

                                        </tr>
                                     </table
                                 </td>
                               </tr>
                                                             
                                   
                              </table>
                            </td>
                        </tr>
                        <tr>
                            <td align="left" bgcolor="#999999" class="auto-style9" nowrap="nowrap" style=" font-size: small; color: white;  border:medium double #FFFFFF;
                                    font-family: Arial; background-color: #e5e5e5; " valign="top">
                                <table id="MapFields" runat="server" bgcolor="#e5e5e5;" border="1" rules="rows" style=" font-size: small; color: black; font-family: Arial; background-color: #ffffff" width="100%"> 
                                    <tr>                                        
                                        <td align="left" nowrap="nowrap" style="font-weight: bold; font-size: small; color: black;
                                            font-family: Arial; background-color: #e5e5e5; width: 28%; " >
                                            Fields selected:&nbsp;
                                        </td>                
                                        <td align="left" nowrap="nowrap" style="font-weight: bold; font-size: small; color: black;
                                            font-family: Arial; background-color: #e5e5e5; width: 28%; "  >
                                            In Map for:
                                        </td> 
                                        <td align="left"  nowrap="nowrap" style="font-size: small; color: black;
                                            font-family: Arial; background-color: #e5e5e5; font-weight: bold; width: 29%;"  >
                                            Text:&nbsp;
                                        </td>
                                        <td align="left"  nowrap="nowrap" style="font-size: small; color: black;
                                            font-family: Arial; background-color: #e5e5e5; font-weight: bold; width: 29%;" >
                                            &nbsp;#&nbsp;
                                        </td>
                                        <td align="left"  nowrap="nowrap" style="font-size: small; color: black;
                                            font-family: Arial; background-color:#e5e5e5; font-weight:bold; width: 5%;"  >
                                            Delete
                                        </td>                                                                                 
                                        
                                    </tr>
                                </table>
                                <br />
                                <table id="tblKeyFields" runat="server" bgcolor="#e5e5e5;" border="1" rules="rows" style=" font-size: small; color: black; font-family: Arial; background-color: #ffffff" width="100%"> 
                                    <tr>                                        
                                        <td align="left" nowrap="nowrap" style="font-weight:bold;font-size:small;color:#000000; font-family:Arial; background-color:#e5e5e5; width:28%;">
                                            Key Fields selected:&nbsp;
                                        </td>                
                                        
                                        <td align="left"  nowrap="nowrap" style="font-size: small; color: black;
                                            font-family: Arial; background-color: #e5e5e5; font-weight: bold; width: 29%;" >
                                            Friendly Names/text&nbsp;
                                        </td>
                                       
                                        <td align="left"  nowrap="nowrap" style="font-size: small; color: black;
                                            font-family: Arial; background-color: #e5e5e5; font-weight: bold; width: 5%;" >
                                            Delete
                                        </td>                                                                                 
                                        
                                    </tr>
                                </table>
                            </td>
                        </tr>
                       <%-- <tr>
                            <td class="auto-style9" style="font-weight: bold; font-size: medium; color: #e5e5e5;
                                font-family: Arial; background-color: #e5e5e5; text-decoration: underline;">
                               
                            </td>
                        </tr>--%>
                        <tr>
                            <td align="left" style="font-weight: bold; font-size: small; color: white; font-family: Arial; background-color: #e5e5e5; " >
                                <%--<asp:Label ID="LabelSQLsort" runat="server" Text="SQL query:" ForeColor="Black"></asp:Label>--%>
                            </td>
                        </tr>
                        <tr valign="top">
                            <td class="auto-style9" >
                                <table id="Table10" runat="server" bgcolor="#e5e5e5;" border="0" style="vertical-align: middle; text-align: left; background-color: #e5e5e5; border-style: inherit; border-width: 2px; border-color: #FFFFFF" >
                                    <tr>
                                        
                                        <td align="left" style="color: black; font-family: Arial; letter-spacing: normal; font-size: small;">
                                            <%-- &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>
                                            <asp:Button ID="btnOpenGoogleChartMapReport" runat="server" CssClass="ticketbutton" Width="280px" Text="Open Google Map Chart Report" Font-Size="Small" Visible="True"></asp:Button>
                                             &nbsp;&nbsp;&nbsp;<%--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>
                                            <asp:Button ID="btnOpenGoogleMap" runat="server" CssClass="ticketbutton" Width="310px" Text="Make simplified kml file and open it in Google Map" Font-Size="Small" Visible="True"></asp:Button>
                                           
                                            &nbsp;&nbsp;&nbsp;<%--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%><asp:Button ID="ButtonSubmit" Visible="false" runat="server" CssClass="ticketbutton" Text="Generate KML for Google Earth Pro" Width="280px" ToolTip="Make kml file for Google Earth Pro and download it, then right click on it in your Downloads folder to open it with Google Earth Pro - the local application (install it if needed from the https://www.google.com/earth/versions/#earth-pro) " />
                                            &nbsp;<asp:Button ID="btnLinkDown" runat="server"  CssClass="ticketbutton" Width="350px" Text="Make and download kmz file to open it with Google Earth Pro" PostBackUrl="MapReport.aspx?downkml=yes" ToolTip="Make kml file for Google Earth Pro and download it, then right click on it in your Downloads folder to open it with Google Earth Pro - the local application (install it if needed from the https://www.google.com/earth/versions/#earth-pro)" ></asp:Button>
                                             &nbsp; &nbsp; (<asp:Label ID="Label7" runat="server" Text="To install the Google Earth Pro app click:" Font-Size="Small"></asp:Label>&nbsp;                                           
                                              <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="https://www.google.com/earth/versions/#earth-pro" Text="here" Font-Size="Small"></asp:HyperLink>)

                                            <br /><br />&nbsp;<%--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>To save KML definition add the comments for history: <asp:TextBox ID="txtComments" runat="server" Width="472px"></asp:TextBox> 
                                            &nbsp;and click:&nbsp;<asp:Button ID="ButtonSaveHistory" runat="server" Width="280px" CssClass="ticketbutton" Text="Save Map definition for future use" ToolTip="Save kml setting in the database" Visible="True" />
                                        </td>
                                          

                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
        </div>
        <div>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Label ID="LabelReprtID" runat="server" Font-Bold="False" Font-Names="Arial" Font-Size="XX-Small" ForeColor="Black" Text=" "></asp:Label>
        </div>
      </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpMAPformat">
            <ProgressTemplate >
            <div class="modal">
                <div class="center">
                    <asp:Image ID="imgProgress" runat="server"  ImageAlign="AbsMiddle" ImageUrl="~/Controls/Images/WaitImage2.gif" />
                    Please Wait...
                </div>
            </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
                <ucmsgbox:msgbox id="MessageBox" runat ="server" > </ucmsgbox:msgbox>
    </form>
</body>
</html>
