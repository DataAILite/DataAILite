<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ScheduledImportsCalendar.aspx.vb" Inherits="ScheduledImportsCalendar" %>
<%@ Register TagPrefix="uc1" TagName="DropDownColumns" Src="Controls/uc1.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Scheduled Imports Calendar</title>

    <style type="text/css">
        /* My Comments ----------------------------------------------------------------------------------------------------------------- My Comments */
        .masterbox {
            border: 2px solid white;
            padding: 15px;
            width: auto;
            display: inline-block;
            background-color: #e5e5e5;
        }

        .boxed-table {
            border-collapse: collapse;   /* Remove spacing between table cells */
            width: 95%;                  /* Set to your preferred width */
            margin: 20px auto;           /* Center the table on the page with some margin */
        }

        .boxed-table td {
            border: 2px solid white;     /* Border for individual cells */
            padding: 10px;               /* Space inside each cell */
        }

        .masterbox {
            margin-right: 20px; /* Adjust as needed */
        }
        .masterbox:not(:last-child) {
            margin-right: 20px; /* Adjust as needed */
        }

        /* My Comments ----------------------------------------------------------------------------------------------------------------- My Comments */
    </style>
</head>
<body style="height: 800px">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
          <asp:UpdatePanel ID="udpSchedRep" runat ="server">
          <ContentTemplate>
              <%--<h2>Under construction. We are using this page only for development and testing!</h2>--%>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;
            <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="ScheduledImports.aspx?calndr=yes&tn=">List of Scheduled Imports</asp:HyperLink>
          
              <br />
               &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;
          
             <table width="100%" align="center"><tr><td>

              <!-- Table Start -->
            <div class="masterbox">
                <%--<table class="boxed-table">--%>
                <table border="1" style="border:medium double #FFFFFF; font-size: small;
                                        color: black; font-family: Arial; background-color: #e5e5e5; vertical-align: top;">
                

                                    <tr valign="top">
                                        <td align="left"  style="font-weight: bold; font-size: small; color: black; font-family: Arial; background-color: #e5e5e5" valign="top">
                                            <h2>Schedule import: </h2>
                                        </td>
                                    </tr>
                        <tr valign="top">
                            <td align="left" width="100%">
                                <table style="border:medium double #FFFFFF; font-size: small;
                                        color: black; font-family: Arial; background-color: #e5e5e5; vertical-align: top;">
                                    <tr valign="top">
                                        <td align="left"  style="font-weight: bold; font-size: small; color: black; font-family: Arial; background-color: #e5e5e5" valign="top">
                                            Import the file from the web site:&nbsp; <asp:TextBox ID="txtURI" runat="server" Width="400px" AutoPostBack="True" ToolTip="It should end with extension .CSV, .XML, .JSON, .TXT, .XLS, .XLSX, .MDB, .ACCDB">https://</asp:TextBox>
                                            <br />
                                        </td>
                                    </tr>

                                    <tr>
                                       <td align="left"  style="font-weight: bold; font-size: small; color: black; font-family: Arial; background-color: #e5e5e5" valign="top">
                                            <br />
                                          <asp:Label ID="LabelTableName" runat="server" style="font-weight: bold; font-size: small; color: black; font-family: Arial; background-color: #e5e5e5" Text="Upload data into new table and check if table exists:"></asp:Label>
                                          <asp:TextBox ID="txtTableName" runat="server" AutoPostBack="True" Height="20px" Width="200px"> </asp:TextBox>             
                                          <br />
                                         
                                          <asp:Label ID="LabelTables" runat="server" Font-Bold="False" Font-Names="Arial" Font-Size="Small" ForeColor="Gray" Text="or select existing table to upload data:"></asp:Label>
                                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                    <asp:Label ID="Label3" runat="server"  Font-Bold="False" Font-Names="Arial" Font-Size="Small" ForeColor="Gray" Text="search:"></asp:Label>                        
                                                    <asp:TextBox ID="txtSearch" runat="server" Visible="true" Height="20px" width="160px"></asp:TextBox>
                                                    <asp:Button ID="ButtonSearch" runat="server" CssClass="ticketbutton" Text="Search" Visible="true" valign="center"/>
                                         <br /> &nbsp;                 
                                         <asp:DropDownList ID="DropDownTables" runat="server" AutoPostBack="True" Height="22px">
                                          </asp:DropDownList>

                                         </td>
                                    </tr>
                                    <tr valign="top" runat ="server"  border="3" style="color: black; font-family: Arial; border:medium double #FFFFFF; background-color: #e5e5e5;border-width: 2px;width:100%;" >                                    
                                          <td align="left">
                                           
                                              &nbsp;&nbsp;<asp:CheckBox ID="chkboxClearTable" runat="server" AutoPostBack="True" ForeColor="Gray" Text="delete all records from the table before upload" ToolTip="All records will be deteted from the selected table and saved in the table with DELETED attached to original table name." Visible="False" />
                                              &nbsp;&nbsp;&nbsp;&nbsp;<asp:CheckBox ID="chkboxUniqueFields" runat="server" AutoPostBack="False" Checked="False" Enabled="False" Text="field names in upload file are unique" ToolTip="The field names in file to upload have unique names." Visible="False" />
                                              &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;<asp:Label ID="LabelDelimiter" runat="server" ForeColor="Gray" Text="csv file delimiter:"></asp:Label>
                                              <asp:TextBox ID="TextboxDelimiter" runat="server" Height="18px" Width="16px">,</asp:TextBox>
              
                                           </td>
                                    </tr>
                                    <tr valign="top">
                                        <td align="left" bgcolor="#999999" nowrap="nowrap" style="font-weight: bold; font-size: small; color: black; font-family: Arial; background-color: #e5e5e5; " valign="top">
                                            Select recurrence:&nbsp;
                                            <asp:DropDownList ID="DropDownRecurse" runat="server">
                                                <asp:ListItem>daily</asp:ListItem>
                                                <asp:ListItem>weekly</asp:ListItem>
                                                <asp:ListItem>monthly</asp:ListItem>
                                                <asp:ListItem>yearly</asp:ListItem>
                                            </asp:DropDownList>
                                            <br />
                                        </td>
                                    </tr>

                                    <tr valign="top">
                                        <td align="left"  style="font-weight: bold; font-size: small; color: black; font-family: Arial;background-color: #e5e5e5; " valign="top">
                                            Time: <asp:TextBox runat = "server" ID="runtime" Width="200px" ToolTip="Time shows the time on our Server! Military time format, default time shows the time on the server Now. Keep this in mind while scheduling.">HH:mm:00</asp:TextBox>
                                            <br />
                                        </td>
                                    </tr>

                                    <tr valign="top">
                                        <td align="left"  style="font-weight: bold; font-size: small; color: black; font-family: Arial;background-color: #e5e5e5; " valign="top"  >
                                        How many times to repeat: <asp:TextBox runat = "server" ID="ntimes" Width="31px">0</asp:TextBox>
                                        <br />
                                        </td>
                                    </tr>
                                    <tr valign="top">
                                        <td align="left" style="font-weight: bold; font-size: small; color: black; font-family: Arial;background-color: #e5e5e5; "  >
                                            Send email notification to: <br />
                                            <uc1:DropDownColumns ID="DropDownColumns" runat="server" Width="350px" ClientIDMode="Predictable" FontName="arial" FontSize="Small" BorderColor="Red" ForeColor="Black" BorderWidth="1" DropDownHeight="190px"/>
                                        <br /><br /><br />
                                        </td>
                                    </tr>
                                    

                                </table>
                            </td>
                       </tr>
                      
                       <tr>
                                            <td align="left" bgcolor="#999999" nowrap="nowrap" style="font-weight: bold; font-size: small; color: black; font-family: Arial; background-color: #e5e5e5; " valign="top">
                                                Click on day number in the calendar below to schedule the import starting on the selected day:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                            </td>
                        </tr>
                        

                 </table>
                  <br />
                </td> 
                  <td id="tdreports" runat="server"  style="width: 48%; text-align: left; vertical-align: top"   visible="false" >
                   <br />&nbsp;&nbsp;&nbsp; <br />
                   <h2 ><asp:Label ID="LabelReports" runat="server" Text="Reports:"></asp:Label></h2>
                   <div>
                       <table runat="server" id="list" style="border: 5px solid #FFFFFF; font-size: 12px; font-family: Arial" >
                        <tr runat="server" id="trheaders"  style="text-align: left; vertical-align: top">
                            <td align="left" style="font-weight:bold"> Data </td>               
                            <td align="center"  style="font-weight:bold"> Analytics </td> 
                            <td align="center"  style="font-weight:bold"> Report </td> 
                        </tr>                        
                       </table>
       
                   </div>
               </td>
            </tr>

             </table>

               <asp:Label ID="LabelAlert" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Medium" ForeColor="Red" Text=" "></asp:Label>
            </div>

              <!-- Table Ending -->


        
<br />
               
            
       


       <%-- <ucmsgbox:msgbox id="MessageBox" runat ="server" > </ucmsgbox:msgbox>--%>
               </ContentTemplate>
        </asp:UpdatePanel>


         <div>
            <asp:Calendar ID = "Calendar1" runat = "server" Caption="Scheduled Imports Calendar" CellPadding="3" CellSpacing="30" Height="100%" NextPrevFormat="FullMonth" ShowGridLines="True" Width="100%" DayStyle-ForeColor="Black" SelectedDayStyle-ForeColor="Black">
                <DayHeaderStyle Height="30px" />
                <DayStyle Height="80px" HorizontalAlign="Center" VerticalAlign="Middle" />
                <OtherMonthDayStyle BackColor="#EBEBEB" Height="80px" />
                <SelectorStyle BackColor="#FFCC66" />
                <TitleStyle Height="30px" />
                <TodayDayStyle Height="80px" />
            </asp:Calendar>
        </div>
        <%--<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpSchedRep">
            <ProgressTemplate >
            <div class="modal">
                <div class="center">
                    <asp:Image ID="imgProgress" runat="server"  ImageAlign="AbsMiddle" ImageUrl="~/Controls/Images/WaitImage2.gif" />
                    Please Wait...
                </div>
            </div>
            </ProgressTemplate>
        </asp:UpdateProgress> --%>   
    </form>
</body>
</html>
