<%@ Page Language="VB" AutoEventWireup="false" CodeFile="InstallDataAI.aspx.vb" Inherits="InstallDataAI" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Data AI</title>
    <style>
        #progressBarContainer {
            width: 100%;
            background-color: #ddd;
        }

        #progressBar {
            width: 0%;
            height: 30px;
            background-color: #4CAF50;
            text-align: center;
            color: white;
        }
        .auto-style1 {
            height: 26px;
        }
    </style>
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script>
        function simulateProgress() {
            let width = 0;
            let interval = setInterval(function () {
                if (width >= 100) {
                    clearInterval(interval);
                } else {
                    width++;
                    $('#progressBar').css('width', width + '%');
                    $('#progressBar').text(width + '%');
                }
            }, 50);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />  
        <div style="font-family: Arial, Helvetica, sans-serif; font-size: medium; background-color: #e5e5e5"  >
<center>
<h2>Data AI installation</h2>
</center>

<table style="border: 2px solid #FFFFFF; width: 100%;">

 <tr  id="trRowsCols" runat ="server" width="100%"  style="border: 1px solid #FFFFFF">
  <td  style="padding: 10px; margin: 10px; border: 2px solid #FFFFFF;">
      <h3>Automatic installation. Enter:</h3>
      <table>  
<tr><td colspan="2">&nbsp;&nbsp;1. New Operational database is needed for keeping the report definitions and settings and will be created with credentials entered below. MySql is preferred for operational database.<br />
</td></tr>            
<tr id="tr1" runat ="server"><td>&nbsp;&nbsp;&nbsp;&nbsp;[operational database server]:</td><td> <asp:TextBox ID="txtOperationalDatabaseServer" runat="server" Width="260px" ToolTip="URL or IP address for operational database server"></asp:TextBox><br />
</td></tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;[operational database/namespace name]:</td><td> <asp:TextBox ID="txtOperationalDatabaseName" runat="server" Width="260px"></asp:TextBox><br />
</td></tr><tr><td class="auto-style1">&nbsp;&nbsp;&nbsp;&nbsp;[operational database user ID]:</td><td class="auto-style1"> <asp:TextBox ID="txtoperationaldatabaseuserID" runat="server" Width="260px"></asp:TextBox><br />
</td></tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;[operational database password]:</td><td> <asp:TextBox ID="txtoperationaldatabasepassword" runat="server" Width="260px"></asp:TextBox><br />
</td></tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;[operational database provider name]:</td><td> <asp:TextBox ID="txtoperationaldatabaseprovider" runat="server" Width="260px" ToolTip="as MySql.Data.MySqlClient, System.Data.SqlClient, Npgsql (for Postgre), Oracle.ManagedData.Client, InterSystems.Data.IRISClient, InterSystems.Data.CacheClient"></asp:TextBox> <br />
</td></tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;[operational database port if needed]:</td><td> <asp:TextBox ID="txtoperationaldatabaseport" runat="server" Width="64px" ToolTip="Postgre (usual Port = 5432), InterSystems IRIS (usual Port = 1972 or 51773) and InterSystems Cache (usual Port = 1972)"></asp:TextBox><br /><br />
</td></tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;[system database password if needed]:</td><td> <asp:TextBox ID="txtsystemdatabasepassword" runat="server" Width="260px" ToolTip="Oracle for (User ID=SYS; DBA Privilege=SYSDBA), InterSystems IRIS for (Namespace = %SYS; User ID = _SYSTEM)"></asp:TextBox><br /><br />
</td></tr><tr><td colspan="2">&nbsp;&nbsp;2. Connection string to database with your data. Samples of connection strings to different databases see in the web.config.<br />
</td></tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;[your data server]:</td><td> <asp:TextBox ID="txtyourdataserver" runat="server" Width="260px" ToolTip="URL or IP address for your database server"></asp:TextBox><br />
</td></tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;[your database/namespace name]:</td><td> <asp:TextBox ID="txtyourDatabaseName" runat="server" Width="260px"></asp:TextBox><br />
</td></tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;[your database user ID]:</td><td> <asp:TextBox ID="txtyourdatabaseuserID" runat="server" Width="260px"></asp:TextBox><br />
</td></tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;[your database password]:</td><td> <asp:TextBox ID="txtyourdatabasepassword" runat="server" Width="260px"></asp:TextBox><br />
</td></tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;[your database provider name]:</td><td> <asp:TextBox ID="txtyourdatabaseprovider" runat="server" Width="260px" ToolTip="as MySql.Data.MySqlClient, System.Data.SqlClient, Npgsql (for Postgre), Oracle.ManagedData.Client, InterSystems.Data.IRISClient, InterSystems.Data.CacheClient"></asp:TextBox><br />
</td></tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;[your database port if needed]:</td><td> <asp:TextBox ID="txtyourdatabaseport" runat="server" Width="64px" ToolTip="Postgre (usual Port = 5432), InterSystems IRIS (usual Port = 1972 or 51773) and InterSystems Cache (usual Port = 1972)"></asp:TextBox><br /><br />
</td></tr><tr><td colspan="2">&nbsp;&nbsp;3. Email information in Default Credentials for SMTP:<br />
</td></tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;[your smtp email]:</td><td> <asp:TextBox ID="txtyoursmtpemail" runat="server" Width="260px" ToolTip="needed for sending emails from application"></asp:TextBox><br />
</td></tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;[password for your smtp email]:</td><td> <asp:TextBox ID="txtyoursmtpemailpassword" runat="server" Width="260px"></asp:TextBox><br /><br />
</td></tr><tr><td colspan="2">&nbsp;&nbsp;4. Misc. AppSetting keys additional to previous:<br />
</td></tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;[your web url for DataAI]:</td><td> <asp:TextBox ID="txtyoururlDataAI" runat="server" Width="260px" ToolTip="as &quot;https://[your web]/DataAI/&quot;"></asp:TextBox> 
</td></tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;[your web site title]:</td><td> <asp:TextBox ID="txtyourwebsitetitle" runat="server" Width="260px"></asp:TextBox><br />
</td></tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;[your support email]:</td><td> <asp:TextBox ID="txtyoursupportemail" runat="server" Width="260px" ToolTip="needed to get emails from application and users"></asp:TextBox><br />
</td></tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;[your folder to upload files]:</td><td> <asp:TextBox ID="txtyourfoldertouploadfiles" runat="server" Width="260px" ToolTip="as C:\Uploads\ for example, should have permisions to upload files by application"></asp:TextBox><br />
</td></tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;[your google map key]:</td><td> <asp:TextBox ID="txtyourgooglemapkey" runat="server" Width="260px" ToolTip="needed for using in reports with geo locations if such reports exist" Height="16px"></asp:TextBox> <br />
</td></tr><tr><td colspan="2">&nbsp;&nbsp;5. OpenAI setting (you need to have OpenAI account):<br />
</td></tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;[your OpenAI key]:</td><td> <asp:TextBox ID="txtyourOpenAIkey" runat="server" Width="260px"></asp:TextBox><br />
</td></tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;[your OpenAI model]:</td><td> <asp:TextBox ID="txtyourOpenAImodel" runat="server" Width="260px" ToolTip="gpt-4o or gpt-4o-mini or o3 or o4 etc..."></asp:TextBox> <br />

</td></tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;[OpenAI maximum tokens limit]:</td><td> <asp:TextBox ID="txtyourmaximumtokens" runat="server" Width="260px" ToolTip="for now 128000 or 200000 or else depending on gpt version"></asp:TextBox> <br />

</td></tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;[your OpenAI organization code]:</td><td> <asp:TextBox ID="txtyourOpenAIorganizationcode" runat="server" Width="260px"></asp:TextBox><br /><br />

</td></tr><tr><td colspan="2">&nbsp;&nbsp;6. All other settings are better to leave as they are to prevent misfunctions.<br /><br />

</td></tr><tr><td colspan="2">&nbsp;&nbsp;7. Clicking the button below will update the configuration and open the next page. <br />
</td></tr></table>

<br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="ButtonInstall" runat="server" Text="Install DataAI" Width="150px" ToolTip="Update web.config and run Default.aspx page" Font-Size="Medium" Font-Bold="True" BorderStyle="Double" BorderColor="#009900" />                                       
           
      <%--<progress id="progressBar" value="0" max="100"></progress>--%> 
      <div id="progressBarContainer">
            <div id="progressBar">0%</div>
      </div>
</td>
       <td  style="padding: 10px; margin: 10px; border: 2px solid #FFFFFF; vertical-align: top;">
           <h3>Manual installation</h3>
1. After downloading the DataAI.zip unzip it into the folder C:\inetpub\wwwroot\DataAI\ on your web server.
             <br /><br />
2. Make the folder DataAI in wwwroot as IIS application in IIS manager.
            <br /><br />
3. Update the web.config file with your information manually in wwwroot\DataAI\ folder 
        <br /><br />   
4. Open the page https://[your web]/DataAI/Default.aspx in web browser
        <br />   
<%--&nbsp;&nbsp;&nbsp;&nbsp;It will cost $100 for installation of our operational database and setting up the DataAI intelligence. <br />
&nbsp;&nbsp;&nbsp;&nbsp;When it finishes, the Default page will open for registration and login.<br />
&nbsp;&nbsp;&nbsp;&nbsp;Register user(s) and start using the system.<br /><br />--%>


                                    
                                    
</td>
     </tr>
    <tr><td colspan=2 style="background-color: e5e5e5; border-color: #FFFFFF; text-align: center; font-size: medium; font-family: Arial, Helvetica, sans-serif"  >
&nbsp;&nbsp;&nbsp;&nbsp;It will cost $100 for installation of our operational database and setting up the DataAI intelligence. <br />
&nbsp;&nbsp;&nbsp;&nbsp;When it finishes, the Default page will open for registration and login.<br />
&nbsp;&nbsp;&nbsp;&nbsp;Register user(s) and start using the system.<br /><br />
&nbsp;&nbsp;&nbsp;&nbsp;If you need further assistance, please <a href="https://oureports.net/OUReports/ContactUs.aspx" target="_blank">contact us</a> <br />


        </td></tr>
    </table>

        </div>
        <ucMsgBox:Msgbox id="MessageBox" runat ="server" > </ucMsgBox:Msgbox>
    </form>
</body>
</html>
