<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Parameters.aspx.vb" Inherits="Parameters" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Filters/Parameters</title>
</head>
<body>
    <form id="form1" runat="server" method ="post" action="AddParameters.aspx">
         
     <table id="main" runat="server" border="0" >
     <tr>
        <td >
       <strong style="color: Gray; font-family: Arial; text-transform: uppercase;">Select REPORT Parameters/filters:</strong><br />
        </td>
     </tr>
         </table>
    <table id="TableParams" runat="server" border="0" >
     <tr>
         
         <td>
             
        </td>
         
         <td>  </td>
    </tr>
    </table>

   
        <p>
            <input id="Submit1" type="submit" value="Submit Parameters" name="submit"/>
        </p>
        <p>
            <asp:label runat="server" ID="LabelError" Font-Bold="True" ForeColor="Red"></asp:label>&nbsp;</p>
        
    </form>
</body>
</html>

