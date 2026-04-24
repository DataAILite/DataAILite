<%@ Page Language="VB" AutoEventWireup="false" CodeFile="GoogleMap.aspx.vb" Inherits="GoogleMap" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Map</title>
    <meta name="viewport" content="initial-scale=1.0, user-scalable=no">   
    <meta charset="utf-8">   
    <style type="text/css">   
      html, body, #map {   
        height: 100%;   
        margin: 0px;   
        padding: 0px   
      }   
    </style>   
    <script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?v=3.exp"></script>   
    <script type="text/javascript">   
        var map;   
        function LoadGoogleMAP() {   
            var SetmapOptions = {   
                zoom: 10,   
                center: new google.maps.LatLng(-34.397, 150.644)   
            };   
            map = new google.maps.Map(document.getElementById('map'),   
      SetmapOptions);   
        }   
   
        google.maps.event.addDomListener(window, 'load', LoadGoogleMAP);   
   
    </script>  
</head>
<body>
    <form id="form1" runat="server">
        <div id="map">

        </div>
    </form>
</body>
</html>
