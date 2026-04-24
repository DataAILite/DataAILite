<%@ Page Language="VB" AutoEventWireup="false" CodeFile="MapGoogle.aspx.vb" Inherits="MapGoogle" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
 <head>
    <meta name="viewport" content="initial-scale=1.0, user-scalable=no">
    <meta charset="utf-8">
    <title>Google Map</title>
    <style>
      html, body {
        height: 800px;
        padding: 0;
        margin: 0;
        }
      #map {
       height: 800px;
       width: 1080px;
       overflow: hidden;
       float: left;
       border: thin solid #333;
       }
      #capture {
       height: 800px;
       width: 400px;
       overflow: hidden;
       float: left;
       background-color: #ECECFB;
       border: thin solid #333;
       border-left: none;
       vertical-align:central;
       }
       #leftmarg {
       height: 800px;
       width: 20px;
       overflow: hidden;
       float: left;
       background-color: #ECECFB;
       border: thin solid #333;
       border-left: none;
       }
    </style>
  </head>
  <body>
      <form id="form1" runat="server">
     &nbsp;&nbsp;&nbsp;&nbsp;
    <asp:LinkButton ID="LinkButtonBack" runat="server">Back</asp:LinkButton>
           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
         <%-- <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/ListOfReports.aspx">List of Reports</asp:HyperLink>--%>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Default.aspx" Font-Names="Arial" Font-Size="Small">Log off</asp:HyperLink> 
          </form>
     <%-- <br />--%>
    <div id="leftmarg">        
    </div>
    <div id="map"></div>
    <div id="capture"></div>
    <script>
      var map;
      var src = '<%=kmlfile%>';

      function initMap() {
        map = new google.maps.Map(document.getElementById('map'), {
          center: new google.maps.LatLng(37.09, -95.71),
          zoom: 5,
          mapTypeId: 'terrain'
        });

        var kmlLayer = new google.maps.KmlLayer(src, {
          suppressInfoWindows: true,
          preserveViewport: false,
          map: map
        });
        kmlLayer.addListener('click', function(event) {
          var content = event.featureData.infoWindowHtml;
          var testimonial = document.getElementById('capture');
          testimonial.innerHTML = content;
        });
      }
    </script>
    <script async defer
   <%--     key=<%=Session("mapkey")%>&--%>
    src="https://maps.googleapis.com/maps/api/js?sensor=false&callback=initMap">
    </script>
  </body>
</html>
