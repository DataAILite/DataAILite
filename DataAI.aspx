<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DataAI.aspx.vb" Inherits="DataAI" %>
<%@ Register TagPrefix="uc1" TagName="DropDownColumns" Src="Controls/uc1.ascx" %>
<script type="text/javascript" src="Controls/Javascripts/OUR.js"></script>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>DataAI</title>
    <style type="text/css">
        .auto-style7 {
            width: 100%;
            height: 40px;
        }
        .auto-style9 {
            height: 40px;
        }
        .auto-style6 {
            width: 900px;
            height: 30px;
        }
        .auto-style19 {
            height: 27px;
            width: 900px;
            background-color:#C0C0C0;
            color:white;
            font-family:Arial;
            font-size:small;
            /*wrap:none;*/
        }
        .auto-style21 {
            margin-left: 101px;
        }
        .auto-style22 {
            width: 375px;
        }
        .auto-style40 {
            width: 21%;
            height: 50px;
        }
        .auto-style41 {
            width: 39%;
            height: 20px;
        }
        .tr {
        background-color: #C0C0C0;
        border-bottom: 1px solid black;
    }
   .inlinebottom {
           display:inline-block;
           vertical-align:bottom;
       }
   .inline {
           display:inline-block;
       }
   .conditions {
       width:100%;
       border:1px solid black;
       background-color:white;
       color:black;
       font-family:Arial;
       font-size :small;
   }
   .columns {
       height: 23px;
       border-width: 0px;
       padding: 0px;
       margin: 0px;
       font-family: Arial;
       font-size: small;
       font-weight: bold;
       color: white;
       background-color: #C0C0C0;
       table-layout:fixed;
       overflow:hidden;
   }
    .ButtonStyleSubmit {
        width: 100px;
        height: 30px;
        font-size: 12px;
        border-radius: 6px;
        border-style :solid;
        border-color: ButtonFace;
        color: black;
        border-width: 1px;
        /*background-repeat: no-repeat;
        background-position:center;*/
        background-color: ButtonFace ;
        padding:0px;
        margin:0px;
        z-index: 9999;
        /*background-image: url("Images\DDImageDown.bmp");*/
        /*padding: 5px 4px 0px 5px*/
     }
    .divSearch {
        display:inline-block;
        margin-bottom:3px;
        /*margin-right: 8px;*/
        padding: 3px;
        height:18px;
        width:18px;
        outline-style: none;
    }
    .btnSearch {
        width: 22px;
        height: 22px;
        /*font-size: 16px;*/
        border-style :none;
        outline-style: none;
        background-color: white;
        margin-bottom: 3px;
        /*margin-right: 8px;*/
        padding: 0px;
    }
    .imgSearch {
        outline-style: none;
    }

    
  .imgSearch:focus {
     outline-style: dashed;
     outline-color: gray;

 }
    
     .ButtonStyle2 {
        height: 25px;
        font-size: 12px;
        border-radius: 5px;
        border-style :solid;
        border-color: ButtonFace ;
        color: black;
        border-width: 1px;
        /*background-repeat: no-repeat;
        background-position:center;*/
        background-color: ButtonFace;
        padding: 3px;
        margin:0px;
        z-index: 9999;
        /*background-image: url("Images\DDImageDown.bmp");*/
        /*padding: 5px 4px 0px 5px*/
     }
        p.MsoListParagraph
	{margin-top:0in;
	margin-right:0in;
	margin-bottom:8.0pt;
	margin-left:.5in;
	line-height:107%;
	font-size:11.0pt;
	font-family:"Calibri",sans-serif;
	}
        .auto-style84 {
            width: 509px;
            height: 21px;
        }
        .auto-style86 {
            width: 900px;
        }
        .auto-style87 {
            height: 27px;
            width: 900px;
        }
        .auto-style88 {
            width: 342px;
        }
        .auto-style94 {
            width: 900px;
            height: 44px;
        }
        .auto-style96 {
            width: 900px;
        }
        .auto-style104 {
            height: 37px;
        }
        .auto-style105 {
            width: 13%;
            height: 50px;
        }
        .auto-style106 {
            height: 27px;
        }
        .auto-style107 {
            width: 20%;
            height: 50px;
        }
        .auto-style113 {
            width: 273px;
            height: 21px;
        }
        .auto-style114 {
            height: 21px;
        }
        .auto-style115 {
            width: 409px;
            height: 21px;
        }
        .auto-style116 {
            height: 52px;
        }
        .auto-style117 {
            width: 268px;
            height: 50px;
        }
        .auto-style118 {
            height: 50px;
        }
 .PleaseWait {
    position: fixed;
    background-color: #FAFAFA;
    z-index: 2147483647 !important;
    opacity: 0.8;
    overflow: hidden;
    text-align: center;
    /*height: 100%;*/
    /*width: 100%;*/
    padding-top:20%;
        /*height: 64px;*/
        /*width: 100%;*/
        /*background-image: url(Controls/Images/WaitImage2.gif );*/
        /*background-repeat: no-repeat;*/
        /*padding-left: 70px;*/
        /*line-height :32px;*/
    }
        .auto-style119 {
            height: 71px;
            width: 270px;
        }
        .auto-style120 {
            height: 10px;
        }
        .auto-style121 {
            width: 900px;
            height: 70px;
        }
        .auto-style122 {
            width: 17%;
            height: 22px;
        }
        .auto-style123 {
            border: 1px solid ButtonFace;
            font-size: 12px;
            border-radius: 6px;
            color: black;
/*background-repeat: no-repeat;
        background-position:center;*/background-color: ButtonFace;
            padding: 0px;
            margin: 0px;
            z-index: 9999;
        /*background-image: url("Images\DDImageDown.bmp");*/
        /*padding: 5px 4px 0px 5px*/
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
.DataButtonEnabled
{
  width: 80px;
  height: 18px;
  font-size: small;
  border-radius: 5px;
  border-style :solid;
  border-color: #4e4747 ;
  color: black;
  border-width: 1px;
  /*background-image: linear-gradient(to bottom, rgba(158, 188, 250,0),rgba(158, 188, 250,1));*/
  background-image: linear-gradient(to bottom, white,rgb(230,236,255),rgb(189,206,255));
  padding: 0px;
  margin-top:12px;
  margin-bottom:3px;
  margin-left:5px;
  margin-right:5px;
  z-index: 9999; 
}

.DataButtonEnabled:hover {
    background-image: linear-gradient(to bottom, white,rgb(253,236,138),rgb(252,233,118));
}
.DataButtonEnabled:active { /* Mouse Down */
    background-image: linear-gradient(to bottom, rgb(189,206,255),rgb(230,236,255),white);
    border-color:black;
}

.DataButtonDisabled
{
  width: 80px;
  height: 18px;
  font-size: small;
  border-radius: 5px;
  border-style :solid;
  border-color: gray ;
  color:gray;
  border-width: 1px;
  /*background-image: linear-gradient(to bottom, rgba(158, 188, 250,0),rgba(158, 188, 250,1));*/
  background-image: linear-gradient(to bottom, white,rgb(239,242,246),rgb(189,206,255));
  padding: 0px;
  margin-top:12px;
  margin-bottom:3px;
  margin-left:5px;
  margin-right:5px;
  z-index: 9999; 
}

        .auto-style124 {
            height: 71px;
            width: 333px;
        }
.ticketbutton 
{
  width: 30px;
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

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />  

    <asp:UpdatePanel ID="udpDataImport" runat ="server" >
       <ContentTemplate>
           <asp:HiddenField ID="hfSizeLimit" runat="Server" Value="4096" />
           <asp:HiddenField ID ="hdnFileName" runat ="server" />
           <asp:HiddenField ID ="hdnPath" runat ="server" />

<table style="vertical-align: top; text-align: left;">
      <tr>
          <td colspan="2" style="font-size:x-large; font-style:normal; font-weight:bold; background-color: #e5e5e5; vertical-align:middle; text-align: left; height: 40px;">
              <asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reporting"></asp:Label>
          </td>
      </tr> 
        <tr>
            
            <td width="5px"></td>
   <td id="main" style="width: 100%; text-align: left; vertical-align: top"> 
     
        <%--<div style="width: 100%">--%>              
             
               &nbsp;<asp:Label ID="LabelAlert" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Large" ForeColor="Maroon" Text=" "></asp:Label>  
              <br />
              
              <%-- &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>
              <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/ListOfReports.aspx" onclick="redirect(event); return false;" CssClass="NodeStyle" Font-Names="Arial">List of Reports</asp:HyperLink> 

               &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
              <asp:HyperLink ID="HyperLinkData" runat="server" NavigateUrl="~/ShowReport.aspx?srd=0&didata=yes" onclick="redirect(event); return false;"  ToolTip="Explore Report Data" CssClass="NodeStyle" Font-Names="Arial">Data</asp:HyperLink> 
             
             
              <br /><br />
              Data to analyze:
              <br />
       <table>
        <tr style="height: 80px">
            <td bgcolor="lightgrey" align="left"> 
                     
            <asp:Label ID="LabelField" runat="server" Text="Select fields for analytics: " Font-Italic="True" Font-Bold="False" ForeColor="Black" ToolTip="Select some columns."></asp:Label>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
               <asp:Button id="btnSelectAll" runat="server" CssClass="DataButtonEnabled" Text="Select All"  />
               <asp:Button id="btnUnselectAll" runat="server" CssClass="DataButtonEnabled" Text="UnselectAll"  />
              <%--&nbsp;&nbsp; <asp:Button ID="ButtonSelectFields" runat="server" Text="Select Fields For Analytics" Width="200px" CssClass="ticketbutton"/>--%>
              &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                 <asp:Label ID="Label3" runat="server" Text=" or " Font-Italic="True" Font-Bold="False" ForeColor="Black" Visible="False"></asp:Label>        
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Label ID="LabelSearch" runat="server" Text="Filter data: " Font-Italic="True" Font-Bold="False" ForeColor="Black" ToolTip="Select some rows."></asp:Label>        
              
              <asp:DropDownList ID="DropDownColumns" runat="server" Width="150px" >
              </asp:DropDownList>&nbsp; 
              <asp:DropDownList ID="DropDownOperator" runat="server"  ToolTip="Numeric or Text operators">
                <asp:ListItem></asp:ListItem>
                <asp:ListItem>=</asp:ListItem>
                <asp:ListItem>&lt;&gt;</asp:ListItem>
                <asp:ListItem>&lt;</asp:ListItem>
                <asp:ListItem>&lt;=</asp:ListItem>
                <asp:ListItem>&gt;</asp:ListItem>
                <asp:ListItem>&gt;=</asp:ListItem>
                <asp:ListItem>IN</asp:ListItem>
                <asp:ListItem>Not IN</asp:ListItem>
                <asp:ListItem>Contains</asp:ListItem>
                <asp:ListItem>Not Contains</asp:ListItem>
                <asp:ListItem>StartsWith</asp:ListItem>
                <asp:ListItem>Not StartsWith</asp:ListItem>
                <asp:ListItem>EndsWith</asp:ListItem>
                <asp:ListItem>Not EndsWith</asp:ListItem>
              </asp:DropDownList>&nbsp;
            <asp:TextBox ID="TextBoxSearch" runat="server" Width="100px"></asp:TextBox>
            <asp:Button ID="ButtonSearch" runat="server" CssClass="ticketbutton" Text="Search" ToolTip="Show data selected" AutoPostBack="true" Width="80px" /> 
         
               
                <%-- &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>
            
               <div  id="divColumns" runat="server" style="display: inline; margin-left:400px;z-index: 100; position: relative; "> 
               
                  <uc1:DropDownColumns ID="DropDownColumns1" runat="server" Width="400px" ClientIDMode="Predictable" FontName="arial" FontSize="Small" BorderColor="Silver" ForeColor="Black" BorderWidth="1" DropDownHeight="190px" FontBold="False" DropDownButtonHeight="22px" TextBoxHeight="22px" PostBackType="OnClose" DropDownBackColor="White" />
               </div>                            
    
            <br /> <br />
            <asp:label runat="server" ID="LabelRowCount" Font-Bold="False" ForeColor="Black" ToolTip="Row Count" Font-Italic="True"></asp:label>
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;

             <%--<asp:HyperLink ID="HyperLinkGridAI" runat="server" onclick="redirect(event); return false;" Target="_blank" ToolTip="Analyze resulting data with AI" Font-Bold="True" Visible="False" NavigateUrl="~/ChatAI.aspx" CssClass="NodeStyle" Font-Names="Arial">AI</asp:HyperLink>--%> 
             <asp:LinkButton ID="lnkGridAI" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Ask AI to analyze the data. May take a long time..." Font-Bold="True" Visible="True">AI</asp:LinkButton>
            
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
             <asp:LinkButton ID="lnkExportGrid1" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Export to CSV file. May take a long time..." PostBackUrl="ShowReport.aspx?export=GridData">Export to Excel</asp:LinkButton>
             
             &nbsp;<asp:Label ID="lblColumnsAlert" runat="server" Text=" " Visible="False" Enabled="False" Width="300px" Height="22px" BorderWidth="1px" BorderStyle="Solid" BorderColor="Silver" BackColor="White" ForeColor="Silver"></asp:Label>
                      
           
           
        
        </td>

        </tr>
        
        <tr>
            <td bgcolor="white">
            <div  style="border: thin solid #C0C0C0; height: 200px; position: relative; overflow: scroll;z-index: 10;">
              <asp:GridView ID="GridView1" runat="server" AllowSorting="True" BackColor="WhiteSmoke" Font-Names="Arial" Font-Size="Small"  Height="100px" Width="100%" Visible="False">  <%--AllowPaging="True" PageSize="10"--%>
                   <AlternatingRowStyle BackColor="WhiteSmoke" />
                   <RowStyle BackColor="White" />
              </asp:GridView>
            </div>
           </td>
         
        </tr>
      </table>
            <br /><br />   
              Result:
              &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
              <asp:LinkButton ID="lnkbtnDownload" runat="server" Text="Download" CssClass="NodeStyle" Visible="True" Enabled="True" Font-Names="Arial"></asp:LinkButton>
     
            
              &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                 <%--<asp:HyperLink ID="HyperLinkChatAI" runat="server" onclick="redirect(event); return false;" Target="_blank" ToolTip="Analyze resulting data with AI" Font-Bold="True" Visible="False" NavigateUrl="~/ChatAI.aspx" Font-Names="Arial" CssClass="NodeStyle">AI</asp:HyperLink>--%> 
                 <asp:LinkButton ID="lnkTextAI" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Ask AI to analyze the data. May take a long time..." Font-Bold="True" Visible="True">AI</asp:LinkButton>
            
             
             &nbsp;<asp:TextBox ID="TextboxResult" runat="server" Enabled="False" Height="1200px" TextMode="MultiLine" Width="100%"  Wrap="True" BorderColor="Black" > </asp:TextBox>
              
        </div>
        
         
        <br />
             
       </td>
       <td>
           &nbsp;

       </td>
       
     </tr>
     
    </table>
    &nbsp;<asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Large" ForeColor="Gray" Text=" "></asp:Label>         


        <ucMsgBox:Msgbox id="MessageBox" runat ="server" > </ucMsgBox:Msgbox>
        <ucDlgTextbox:DlgTextbox id="dlgTextbox" runat="server" />
      </ContentTemplate>

        <Triggers> 
            <asp:PostBackTrigger ControlID="lnkbtnDownload"/>            
        </Triggers>

      </asp:UpdatePanel>   

             <div id="spinner" class="modal" style="display:none;">
                <div id="divSpinner" style="text-align: center; width: 130px; display: inline-block; clip: rect(auto, auto, auto, 50%); position: fixed; margin-top: 300px; margin-right: auto; padding-top: 10px; background-color: #f8f8d3; z-index: 2147483647; left: 50%; top: -5%;">
                  <img id="imgSpinner" src="Controls/Images/WaitImage2.gif" style="width: 100px; height: 100px" />
                    <br />
                      Please Wait...
                </div>
            </div>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpDataImport" DisplayAfter="500">
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

