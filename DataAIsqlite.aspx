<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DataAIsqlite.aspx.vb" Inherits="DataAIsqlite" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>DataAI in memory</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" /> 
        <div>
             <asp:HyperLink ID="HyperLinkLogOff" runat="server" NavigateUrl="Default.aspx" CssClass="NodeStyle" Font-Names="Arial">Log off</asp:HyperLink> 
<center>
        <h1>DataAI Lite in memory</h1>          
    <h3 style ="font-family:Tahoma;font-size:14px; font-weight: bold; color: #990033;">Your setting will be used only to analyze your data in memory and no setting nor data will be kept after Log Off is clicked.</h3>    
</center>
<table border="0" cellpadding="1" cellspacing="0" width="100%">
     <tr>
           <td align="center" valign="top">
            
           <br /><br />
            <table>                   
                   <tr style="border-color:#ffffff"  align="left">
                        <td align="right" class="auto-style2">
                            &nbsp;&nbsp;
                            <asp:Label ID="Label2" runat="server" Text="Email/Logon/User ID:" style ="font-family:Tahoma;font-size:13px; font-weight: bold; color: #990033;" ToolTip="Optional. If empty it will be assign as timestamp."></asp:Label>
                        </td>        
                        <td align ="left" class="auto-style1">
                             <asp:TextBox runat="server" ID="txtEmail" type="text" style="width: 350px" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;  
                        </td>
                   </tr>                
             </table>
                 <br /><br /><br />
           </td>
     </tr>
      <tr>
           <td align="center" valign="top">
               <asp:Label ID="Label1" runat="server"  Height="25px" Width="800px" style ="font-family:Tahoma;font-size:14px;font-weight:bold;" ForeColor="Gray" >Optional - to use the AI functionality(!) in your data analysis, enter your OpenAI account setting below:</asp:Label>
                <%--<br />--%>
               <%-- &nbsp;<asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="https://platform.openai.com/">click to sign for new OpenAI account if needed</asp:HyperLink>
                <br />--%>
           </td>
     </tr>
     
     <tr>
          <td align="center" style="height: 144px">  

               <table id="Registr" runat="server" border="0" cellpadding="0" cellspacing="0" class="auto-style5">


                   <tr id="trOpenAI" runat="server"  valign="top" visible="true" bgcolor="#e5e5e5"  align="center" >
                      <td  align="center">
     
                         &nbsp;<asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="https://platform.openai.com/" ToolTip="To use AI functionality you should enter your OpenAI credencials">click to sign for new OpenAI account if needed</asp:HyperLink>
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
                                   &nbsp;&nbsp;<span style ="font-family:Tahoma;font-size:12px; font-weight: bold; color: Gray;">&nbsp;OpenAI Base URL:&nbsp;</span>
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
                                   <asp:TextBox runat="server" ID="txtOpenAImodel" type="text" style="width: 350px" Text="" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;                      
              
                                </td>
                           </tr>
                           <tr>
                               <td align="right" class="auto-style2">
                                  &nbsp;&nbsp;<span style ="font-family:Tahoma;font-size:12px; font-weight: bold; color: Gray;">&nbsp;OpenAI maximum tokens limit:&nbsp;</span>
                               </td>
     
                               <td align ="left" class="auto-style1">
                                   <asp:TextBox runat="server" ID="txtOpenAImaxtokens" type="text" style="width: 350px" Text="" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;                      
              
                                </td>
                           </tr>
                           
                       </table>  
                      </td>
                   </tr>
                  </table> 
                    
                 
              <br /><asp:Button ID="btStart" runat="server" text="Continue" style="font-family: Tahoma; font-size: 14px; font-weight: bold; color: #990033; background-color:aquamarine; border-color:aquamarine " />
                    <%--<br /><br />
                    &nbsp;<asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="https://platform.openai.com/">sign for new OpenAI account</asp:HyperLink>
                    <br />--%>
               
              <br /><br />
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

        </div>
    </form>
</body>
</html>
