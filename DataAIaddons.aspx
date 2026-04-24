<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DataAIaddons.aspx.vb" Inherits="DataAIaddons" %>

<script type="text/javascript" src="Controls/Javascripts/OUR.js"></script>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>DataAI In Memory</title>
    <style type="text/css">
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
        .auto-style3 {
            color: #808080;
            background-color: #2ecc71;
            padding: 10px;
            border: none 0px transparent;
            font-size: 25px;
            font-weight: lighter;
            /*webkit-border-radius: 2px 16px 16px 16px;*/
            -moz-border-radius: 2px 16px 16px 16px;
            border-radius: 2px 16px 16px 16px;
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
        </style>
</head>
<body>
    <div id="divreg">
    <form id="form1" runat="server">
              
  <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" /> 
      <asp:UpdatePanel ID="udpAddOns" runat ="server">
          <ContentTemplate>
               <asp:HiddenField ID="hfSizeLimit" runat="Server" Value="4096" />              
         <div style="font-size: x-large; font-style: normal; font-weight: bold; background-color: #e5e5e5; text-align:left; height: 40px; line-height:40px; width:100%;">
             <asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reporting"></asp:Label>
         </div>
              <%--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>&nbsp; <asp:HyperLink ID="HyperLinkLogOff" runat="server" NavigateUrl="Default.aspx" CssClass="NodeStyle" Font-Names="Arial">Log off</asp:HyperLink> 
         <center>
                 <h1>DataAI Lite in memory</h1>
                    

             <asp:Label ID="LblInvalid" runat="server"  Height="25px" Width="800px" style ="font-family:Tahoma;font-size:14px;font-weight:bold;" ForeColor="#990033"></asp:Label>
                                              
              <%-- <span style ="font-family:Tahoma;font-size:14px;font-weight:bold;color:Red" >
                    <span style="display: inline !important; float: none; background-color: rgb(255, 255, 153); color: rgb(153, 0, 0); font-family: Arial; font-size: 13.33px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;">Please read </span><a href="disclaimer.htm" style="background-color: transparent; color: rgb(0, 102, 204); font-family: Arial; font-size: 13.33px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-decoration: underline; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><font color="#0000ff" style="background-color: transparent; color: rgb(0, 0, 255); font-size: 13.33px; text-align: left;">Disclaimer</font></a><span style="display: inline !important; float: none; background-color: rgb(255, 255, 153); color: rgb(153, 0, 0); font-family: Arial; font-size: 13.33px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"> 
                and </span><a href="PrivacyPolicy.htm" style="background-color: transparent; color: rgb(0, 102, 204); font-family: Arial; font-size: 13.33px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-decoration: underline; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><font color="#0000ff" style="background-color: transparent; color: rgb(0, 0, 255); font-size: 13.33px; text-align: left;">Privacy Policy</font></a><span style="display: inline !important; float: none; background-color: rgb(255, 255, 153); color: rgb(153, 0, 0); font-family: Arial; font-size: 13.33px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"> first. </span>
              
              </span>--%> 
              <br /> 
             <%--<h2><b>Free for individual user and developer</b></h2>--%>
             
         </center>
         <table border="0" cellpadding="1" cellspacing="0" width="100%">
              <tr>
                    <td align="center" valign="top">
                       <%-- Already registered - &nbsp;<asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Default.aspx">Sign in</asp:HyperLink>
                         <br />--%>
                         <br />
                 <asp:Button ID="ButtonVideo" runat="server" Text="Quick Start Video" OnClientClick="target='_blank'" CssClass="auto-style3" BackColor="#FFFFCC" BorderColor="#CC9900" BorderStyle="Ridge" Font-Bold="True" Width="200px" Height="38px" Font-Size="Medium" ToolTip="Video Demo" BorderWidth="3px" Visible="False" Enabled="False" />
            
             <br />
             <br />

                    </td>
              </tr>
               <tr>
                    <td align="center" valign="top">
                        <asp:Label ID="Label1" runat="server"  Height="25px" Width="800px" style ="font-family:Tahoma;font-size:14px;font-weight:bold;" ForeColor="Gray" >Optional - to use the AI functionality(!) in your data analysis, enter your OpenAI account setting below:</asp:Label>
                       
                    </td>
              </tr>
              
              <tr>
                   <td align="center" style="height: 144px">                                      
                        <table id="Registr" runat="server" border="0" cellpadding="0" cellspacing="0" class="auto-style5">


                            <tr id="trOpenAI" runat="server"  valign="top" visible="true" bgcolor="#e5e5e5"  align="center" >
                               <td  align="center">
                                    
                                    &nbsp;<asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="https://platform.openai.com/" ToolTip="To use AI functionality you should enter your OpenAI credencials">click to sign for new OpenAI account if needed</asp:HyperLink>
                                    <br />
                                <table>
                                    <tr>
                                        <td align="right" class="auto-style2">
                                           &nbsp;&nbsp;<span style ="font-family:Tahoma;font-size:12px; font-weight: bold; color: Gray;">&nbsp;OpenAI key:&nbsp;</span>
                                        </td>
     
                                        <td align ="left" class="auto-style1">
                                            <asp:TextBox runat="server" ID="txtOpenAIkey" type="text" style="width: 350px" TextMode="Password" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;                      
                       
                                         </td>
                                    </tr>
                                    <tr>
                                        <td align="right" class="auto-style2">
                                           &nbsp;&nbsp;<span style ="font-family:Tahoma;font-size:12px; font-weight: bold; color: Gray;">&nbsp;OpenAI organization code:&nbsp;</span>
                                        </td>
     
                                        <td align ="left" class="auto-style1">
                                            <asp:TextBox runat="server" ID="txtOpenAIorgcode" type="text" style="width: 350px" TextMode="Password" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;                      
                       
                                         </td>
                                    </tr>
                                    <tr>
                                        <td align="right" class="auto-style2">
                                           &nbsp;&nbsp;<span style ="font-family:Tahoma;font-size:12px; font-weight: bold; color: Gray;">&nbsp;OpenAI URL:&nbsp;</span>
                                        </td>
     
                                        <td align ="left" class="auto-style1">
                                            <asp:TextBox runat="server" ID="txtOpenAIurl" type="text" style="width: 350px" TextMode="Url" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;                      
              
                                         </td>
                                   </tr>
                                    <tr>
                                        <td align="right" class="auto-style2">
                                           &nbsp;&nbsp;<span style ="font-family:Tahoma;font-size:12px; font-weight: bold; color: Gray;">&nbsp;OpenAI model:&nbsp;</span>
                                        </td>
     
                                        <td align ="left" class="auto-style1">
                                            <asp:TextBox runat="server" ID="txtOpenAImodel" type="text" style="width: 350px" Text="gpt-4o-mini" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;                      
                       
                                         </td>
                                    </tr>
                                    <tr>
                                        <td align="right" class="auto-style2">
                                           &nbsp;&nbsp;<span style ="font-family:Tahoma;font-size:12px; font-weight: bold; color: Gray;">&nbsp;OpenAI maximum tokens limit:&nbsp;</span>
                                        </td>
     
                                        <td align ="left" class="auto-style1">
                                            <asp:TextBox runat="server" ID="txtOpenAImaxtokens" type="text" style="width: 350px" Text="0" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;                      
                       
                                         </td>
                                    </tr>
                                    
                                </table>  
                               </td>
                            </tr>
                           </table> 
                       <br /><br /><%--<br /><br />--%>
                           <table> 
                           
                             <tr style="border-color:#ffffff"  align="left">
                                  <td align="right" class="auto-style2">
                                      &nbsp;&nbsp;
                                      <%-- <span style ="font-family:Tahoma;font-size:12px; font-weight: bold; color: #990033;">&nbsp;*Email:&nbsp;</span>--%>
                                      <asp:Label ID="Label2" runat="server" Text="Email/Logon/User ID:" style ="font-family:Tahoma;font-size:12px; font-weight: bold; color: #990033;" ToolTip="Optional. If empty it will be assign as timestamp."></asp:Label>
                                  </td>
                                  
                                  <td align ="left" class="auto-style1">
                                       <asp:TextBox runat="server" ID="txtEmail" type="text" style="width: 350px" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;                       
                       
                                  </td>
                             </tr>
                             <tr style="border-color:#ffffff"  align="left">
                                    <td align="right" class="auto-style2">
                                        <%--<br /><br />--%>
                                        &nbsp;&nbsp;                                        
                                        <asp:Label ID="Label3" runat="server" Text="Folder to upload temporary files:" style ="font-family:Tahoma;font-size:12px; font-weight: bold; color: #990033;" ToolTip="Optional. If not entered the Temp subfolder in Application folder will be used to keep temporary files. It should have permisions to upload files by application."></asp:Label>
                                    </td>
     
                                    <td align ="left" class="auto-style1">
                                        <%--<br /><br />--%>
                                        <asp:TextBox runat="server" ID="txtUploads" type="text" style="width: 350px" ToolTip="Should have permisions to upload files by application" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;                       
                       
                                    </td>
                            </tr>

                          </table>
                       <br />

                        <asp:Label ID="Label4" runat="server"  Height="25px" Width="800px" style ="font-family:Tahoma;font-size:16px;font-weight:bold;" ForeColor="#990033" >*Data Upload:</asp:Label>
                       
                       <br /><br />

                        &nbsp;&nbsp;<asp:Label ID="lblSelectFile" runat="server" Text="Select file to upload:" style ="font-family:Tahoma;font-size:12px; font-weight: bold; color: #990033;" ></asp:Label>
                        &nbsp;&nbsp;<asp:Button ID="btnBrowseAddOns" runat="server" Text="local file" ToolTip="Browse for CSV, XML, JSON, XLS to input data into the table in memory."  Width="152px" />
                        &nbsp;&nbsp;<asp:Label ID="lblFileChosen" runat="server" Text=" " ToolTip=" "></asp:Label>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;                      
                       
                       <br />
                       <br />
                          <span style ="font-family:Tahoma;font-size:12px; font-weight: bold; color: #990033;">&nbsp;&nbsp;or upload file from url:</span>
                          <asp:TextBox runat="server" ID="txtURI" type="text" style="width: 350px" ValidateRequestMode="Enabled" Text="https://"/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                       <br />
                       <br />

                        <asp:Label ID="Label5" runat="server" Text="or connect to user database:" style ="font-family:Tahoma;font-size:12px; font-weight: bold; color: #990033;"></asp:Label>
                        <%--<br /> 
                       
                       <asp:Label ID="LabelProv" runat="server" Text="User Database provider:" style ="font-family:Tahoma;font-size:12px; font-weight: bold; color: #990033;"></asp:Label>--%>
                                 
                                  
                                       <asp:DropDownList runat="server" Font-Size="Smaller" ID="dropdownDatabases" AutoPostBack="True" >
                                           <asp:ListItem Value="System.Data.SqlClient">SQL Server</asp:ListItem>
                                           <asp:ListItem Value="MySql.Data.MySqlClient">MySQL</asp:ListItem>
                                           <asp:ListItem Value="Oracle.ManagedDataAccess.Client">Oracle</asp:ListItem>
                                           <asp:ListItem Value="InterSystems.Data.CacheClient">Intersystems Cache</asp:ListItem>
                                           <asp:ListItem Value="InterSystems.Data.IRISClient">Intersystems IRIS</asp:ListItem>
                                           <asp:ListItem Value="System.Data.Odbc">ODBC</asp:ListItem>
                                           <asp:ListItem Value="System.Data.OleDb">OleDb</asp:ListItem>
                                           <asp:ListItem Value="Npgsql">PostgreSQL</asp:ListItem>
                                          
                                       </asp:DropDownList>
                                      
                                <br />
                                      
                                        <asp:Label ID="Label6" runat="server" Text="with connection string:" style ="font-family:Tahoma;font-size:12px; font-weight: bold; color: #990033;"></asp:Label>
                                
                                       <asp:TextBox ID="ConnStr" runat="server" Width="550px"></asp:TextBox>

                                    <asp:Label ID="LabelDBpassword" runat="server" Text="password: " style ="font-family:Tahoma;font-size:12px; font-weight:bold; color: #990033;" ToolTip="Requested to make initial reports and analytics immediately."></asp:Label>
                               
                                     <asp:TextBox ID="DBpass" runat="server" Width="100px" TextMode="Password" ></asp:TextBox>









                       <br /><asp:CheckBox runat="server" ID="chkSavedata" Text="save data in our database (optional)" ToolTip="Save data for future use. If unchecked the data will be analyzed but not saved in DataAI. " Visible="False" Enabled="False" />
                       <br />
                       <br /><asp:Button ID="btStart" runat="server" text="     Start     " style="font-family: Tahoma; font-size: 14px; font-weight: bold; color: #990033; background-color:aquamarine; border-color:aquamarine" />
                       <br /> 
                       <br /><br />
                   </td>
              </tr>

            

              <tr>
                    <td align="center" valign="top">
                        <%--<asp:Label ID="LblInvalid" runat="server"  Height="25px" Width="800px" style ="font-family:Tahoma;font-size:14px;font-weight:bold;" ForeColor="#990033"></asp:Label>--%>

                       
                    </td>
              </tr>
              <tr>
                   <td align="center" valign="top">
                       
                       <br /><br />
                       &nbsp;<asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="http://DataAI.link">Documentation, downloads, and full version at DataAI.link</asp:HyperLink>
          
                   </td>
             </tr>

         </table>  
               <ucMsgBox:Msgbox id="MessageBox" runat ="server" > </ucMsgBox:Msgbox> 
          </ContentTemplate>
          <Triggers>
                <asp:PostBackTrigger  ControlID ="btStart"/>
          </Triggers>

      </asp:UpdatePanel>
        <asp:FileUpload id="FileAddOns" runat ="server" style="display:none;" />
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpAddOns">
            <ProgressTemplate >
            <div class="modal" >
                <div class="center">
                    <asp:Image ID="imgProgress" runat="server"  ImageAlign="AbsMiddle" ImageUrl="~/Controls/Images/WaitImage2.gif" />
                    Please Wait...
                </div>
            </div>
            </ProgressTemplate>
      </asp:UpdateProgress>                  
        
       
    </form>
    </div> 
</body>
</html>
