<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Dashboard.aspx.vb" Inherits="Dashboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script type="text/javascript" src="ChartGoogleOne.js.aspx"></script>

    <title>Dashboard</title>  
   
    <script type="text/javascript" src="https://www.gstatic.com/charts/loader.js"></script>
    <script type="text/javascript">
        function options(chrt) {                             
                                if (chrt == 'Map')
                                {
                                    var url = 'https://icons.iconarchive.com/icons/icons-land/vista-map-markers/48/';                                    
                                    var optionsx =
                                    {   
                                        titleTextStyle: { color: 'black', fontSize: 8, bold: true },
                                        width: 400,
                                        height: 300,
                                        left: 0,
                                        top: 0,
                                        chartArea: { left: 10, top: 10, right: 10, bottom: 10 },
                                        legend: {
                                              textStyle: { color: 'black', fontSize: 8 }
                                        },                                        
                                        hAxis: {
                                            textStyle: { color: 'black', fontSize: 8 },
                                            legend: {
                                                textStyle: { color: 'red', fontSize: 8 }
                                            }
                                        },
                                        vAxis: {
                                            textStyle: { color: 'black', fontSize: 8 },
                                            legend: {
                                                textStyle: { color: 'red', fontSize: 8 }
                                            }
                                        },
                                        //zoomLevel: 6,
                                        center: {lat: 37.06, lng: -95.68},
                                        showTooltip: true,
                                        showInfoWindow: true,
                                        useMapTypeControl: true,
                                        icons: {
                                            blue: {
                                                normal: url + 'Map-Marker-Ball-Azure-icon.png',
                                                selected: url + 'Map-Marker-Ball-Right-Azure-icon.png'
                                            },
                                            green: {
                                                normal: url + 'Map-Marker-Push-Pin-1-Chartreuse-icon.png',
                                                selected: url + 'Map-Marker-Push-Pin-1-Right-Chartreuse-icon.png'
                                            },
                                            pink: {
                                                normal: url + 'Map-Marker-Ball-Pink-icon.png',
                                                selected: url + 'Map-Marker-Ball-Right-Pink-icon.png'
                                            }
                                        }
                                    };
                                    return optionsx;
                                };
                                if (chrt == 'GeoChart') {
                                        var optionsx =
                                        {
                                            width:400,
                                            height:300,
                                            left: 0,
                                            top: 0,
                                            center: {lat: 37.06, lng: -95.68},
                                            chartArea: { left: 130, top: 100, right: 180, bottom: 30 },
                                            //chartArea: { left: 10, top: 10, right: 10, bottom: 10 },
                                            displayMode: 'markers',
                                            colorAxis: { colors: ['green', 'blue'] }
                                    };
                                    return optionsx;
                                };                            
                                if (chrt == 'CandlestickChart') {
                                            var optionsx = {
                                                legend: 'none',
                                                bar: { groupWidth: '100%' }, // Remove space between bars.
                                                candlestick: {
                                                    fallingColor: { strokeWidth: 0, fill: '#a52714' }, // red
                                                    risingColor: { strokeWidth: 0, fill: '#0f9d58' }   // green
                                                }
                                    };
                                    return optionsx;
                                };
                                if (chrt == 'Gauge') {
                    
                                     var optionsx = {
                                        width: 400, height: 300,
                                        redFrom: 90, redTo: 100,
                                        yellowFrom:75, yellowTo: 90,
                                        minorTicks: 5
                                    }; 
                                    return optionsx;
                                };
                                if (chrt == 'Sankey') { 
                                     //var colors = ['#a6cee3', '#b2df8a', '#fb9a99', '#fdbf6f', '#cab2d6', '#ffff99', '#1f78b4', '#33a02c'];
                                    var colors = ['blue', 'gold', 'lightgreen', 'red', 'yellow', 'darkred', 'lightblue', 'green', '#e5e4e2', 'orange', 'darkgreen', 'pink', 'darkblue', 'lightyellow', 'maroon', 'salmon', 'darkorange'];
                                    //var colors = ['blue', 'gold', 'lightgreen', 'red', 'yellow', 'darkred', 'lightblue', 'green'];  //, '#e5e4e2', 'orange', 'darkgreen', 'pink', 'darkblue', 'lightyellow', 'maroon', 'salmon', 'darkorange'];
                                    var optionsx = {
                                        width: 400,
                                        height: 300,
                                        sankey: {
                                            node: {
                                                colors: colors
                                            },
                                            link: {
                                                colorMode: 'gradient',
                                                colors: colors
                                            }
                                        }
                                    };
                                    return optionsx;
                                };
                                var optionsx ={                                    
                                                width:400,
                                                height:300,
                                                //left: 0,
                                                //top: 0,
                                                //chartArea: { left: 10, top: 10, right: 10, bottom: 10 },
                                                //colors: ['blue', 'gold', 'lightgreen', 'red', 'yellow', 'darkred','lightblue', 'green','#e5e4e2', 'orange', 'darkgreen', 'pink','darkblue', 'lightyellow', 'maroon', 'salmon', 'darkorange'],
                                                //colors = ['#a6cee3', '#b2df8a', '#fb9a99', '#fdbf6f','#cab2d6', '#ffff99', '#1f78b4', '#33a02c'],
                                                legend: {
                                                    textStyle: { color: 'black', fontSize: 12 }
                                                },

                                                hAxis: {
                                                    textStyle: { color: 'black', fontSize: 8 },
                                                    legend: {
                                                        textStyle: { color: 'red', fontSize: 8 }
                                                    }
                                                },
                                                vAxis: {
                                                    textStyle: { color: 'black', fontSize: 8 },
                                                    legend: {
                                                        textStyle: { color: 'red', fontSize: 8 }
                                                    }
                                                }                                    
                                };
                                return optionsx;
        }

        var n = <%=nrec%>;
        var url = 'https://icons.iconarchive.com/icons/icons-land/vista-map-markers/48/';

        // ----------------------------------- 0    -------------------------------------------------------
       
        var chrt = '<%=charttypes(0)%>';
        var options0=options(chrt);
        // Load the Visualization API and the corechart, map, etc... packages.
        //loadCharts('<%=chartpckgs(0)%>');
        google.charts.load('current', {'packages':['<%=chartpckgs(0)%>']});
                // Draw the chart when loaded.
        google.charts.setOnLoadCallback(drawChart0);
                    // Callback that draws the chart.
        function drawChart0()
                    {  // Create the data table for chart. 
                        var data = google.visualization.arrayToDataTable([<%=arrs(0)%>]);                  
                       // Set options for chart.
                        var options = options0;                        
                        options.title = '<%=ttls(0)%>';
                        options.region = '<%=chartregns(0)%>';
                        // Instantiate and draw the charts.     
                        var chart = new google.visualization.<%=charttypes(0)%>(document.getElementById('chart_div_0'));                
                        chart.draw(data, options); 
        }

    // ----------------------------------- 1    ------------------------------------------------------- 
    if (n > 1) {              
        var chrt = '<%=charttypes(1)%>';
        var options1=options(chrt);
        // Load the Visualization API and the corechart, map, etc... packages.
        //loadCharts('<%=chartpckgs(1)%>');
        google.charts.load('current', {'packages':['<%=chartpckgs(1)%>']});
                // Draw the chart when loaded.
        google.charts.setOnLoadCallback(drawChart1);
                    // Callback that draws the chart.
        function drawChart1()
                    {  // Create the data table for chart. 
                        var data = google.visualization.arrayToDataTable([<%=arrs(1)%>]);                  
                       // Set options for chart.
                        var options = options1;                        
                        options.title = '<%=ttls(1)%>';
                        options.region = '<%=chartregns(1)%>';
                        // Instantiate and draw the charts.     
                        var chart = new google.visualization.<%=charttypes(1)%>(document.getElementById('chart_div_1'));                
                        chart.draw(data, options); 
        }
    }
        // ----------------------------------- 2    -------------------------------------------------------
        if (n > 2) {
            var chrt = '<%=charttypes(2)%>';
            var options2=options(chrt);
            // Load the Visualization API and the corechart, map, etc... packages.
            //loadCharts('<%=chartpckgs(2)%>');
            google.charts.load('current', {'packages':['<%=chartpckgs(2)%>']});
            // Draw the chart when loaded.
            google.charts.setOnLoadCallback(drawChart2);
            // Callback that draws the chart.
            function drawChart2() {  // Create the data table for chart. 
                var data = google.visualization.arrayToDataTable([<%=arrs(2)%>]);
                // Set options for chart.
                var options = options2;
                options.title = '<%=ttls(2)%>';
                options.region = '<%=chartregns(2)%>';
                // Instantiate and draw the charts.     
                var chart = new google.visualization.<%=charttypes(2)%>(document.getElementById('chart_div_2'));
                chart.draw(data, options);
            }
        }
        // ----------------------------------- 3    -------------------------------------------------------
        if (n > 3) {
            var chrt = '<%=charttypes(3)%>';
            var options3=options(chrt);
            // Load the Visualization API and the corechart, map, etc... packages.
            //loadCharts('<%=chartpckgs(3)%>');
            google.charts.load('current', {'packages':['<%=chartpckgs(3)%>']});
            // Draw the chart when loaded.
            google.charts.setOnLoadCallback(drawChart3);
            // Callback that draws the chart.
            function drawChart3() {  // Create the data table for chart. 
                var data = google.visualization.arrayToDataTable([<%=arrs(3)%>]);
                // Set options for chart.
                var options = options3;
                options.title = '<%=ttls(3)%>';
                options.region = '<%=chartregns(3)%>';
                // Instantiate and draw the charts.     
                var chart = new google.visualization.<%=charttypes(3)%>(document.getElementById('chart_div_3'));
                chart.draw(data, options);
            }
        }
         // ----------------------------------- 4    -------------------------------------------------------
        if (n > 4) {
            var chrt = '<%=charttypes(4)%>';
            var options4=options(chrt);
            // Load the Visualization API and the corechart, map, etc... packages.
            //loadCharts('<%=chartpckgs(4)%>');
            google.charts.load('current', {'packages':['<%=chartpckgs(4)%>']});
            // Draw the chart when loaded.
            google.charts.setOnLoadCallback(drawChart4);
            // Callback that draws the chart.
            function drawChart4() {  // Create the data table for chart. 
                var data = google.visualization.arrayToDataTable([<%=arrs(4)%>]);
                // Set options for chart.
                var options = options4;
                options.title = '<%=ttls(4)%>';
                options.region = '<%=chartregns(4)%>';

                // Instantiate and draw the charts.     
                var chart = new google.visualization.<%=charttypes(4)%>(document.getElementById('chart_div_4'));
                chart.draw(data, options);
            }
        }
         // ----------------------------------- 5    -------------------------------------------------------
        if (n > 5) {
            var chrt = '<%=charttypes(5)%>';
            var options5=options(chrt);
            // Load the Visualization API and the corechart, map, etc... packages.
            //loadCharts('<%=chartpckgs(5)%>');
            google.charts.load('current', {'packages':['<%=chartpckgs(5)%>']});
            // Draw the chart when loaded.
            google.charts.setOnLoadCallback(drawChart5);
            // Callback that draws the chart.
            function drawChart5() {  // Create the data table for chart. 
                var data = google.visualization.arrayToDataTable([<%=arrs(5)%>]);
                // Set options for chart.
                var options = options5;
                options.title = '<%=ttls(5)%>';
                options.region = '<%=chartregns(5)%>';

                // Instantiate and draw the charts.     
                var chart = new google.visualization.<%=charttypes(5)%>(document.getElementById('chart_div_5'));
                chart.draw(data, options);
            }
        }
         // ----------------------------------- 6    -------------------------------------------------------
        if (n > 6) {
            var chrt = '<%=charttypes(6)%>';
            var options6=options(chrt);
            // Load the Visualization API and the corechart, map, etc... packages.
            //loadCharts('<%=chartpckgs(6)%>');
            google.charts.load('current', {'packages':['<%=chartpckgs(6)%>']});
            // Draw the chart when loaded.
            google.charts.setOnLoadCallback(drawChart6);
            // Callback that draws the chart.
            function drawChart6() {  // Create the data table for chart. 
                var data = google.visualization.arrayToDataTable([<%=arrs(6)%>]);
                // Set options for chart.
                var options = options6;
                options.title = '<%=ttls(6)%>';
                options.region = '<%=chartregns(6)%>';

                // Instantiate and draw the charts.     
                var chart = new google.visualization.<%=charttypes(6)%>(document.getElementById('chart_div_6'));
                chart.draw(data, options);
            }
        }
         // ----------------------------------- 7    -------------------------------------------------------
        if (n > 7) {
            var chrt = '<%=charttypes(7)%>';
            var options7=options(chrt);
            // Load the Visualization API and the corechart, map, etc... packages.
            //loadCharts('<%=chartpckgs(7)%>');
            google.charts.load('current', {'packages':['<%=chartpckgs(7)%>']});
            // Draw the chart when loaded.
            google.charts.setOnLoadCallback(drawChart7);
            // Callback that draws the chart.
            function drawChart7() {  // Create the data table for chart. 
                var data = google.visualization.arrayToDataTable([<%=arrs(7)%>]);
                // Set options for chart.
                var options = options7;
                options.title = '<%=ttls(7)%>';
                options.region = '<%=chartregns(7)%>';

                // Instantiate and draw the charts.     
                var chart = new google.visualization.<%=charttypes(7)%>(document.getElementById('chart_div_7'));
                chart.draw(data, options);
            }
        }
         // ----------------------------------- 8    -------------------------------------------------------
        if (n > 8) {
            var chrt = '<%=charttypes(8)%>';
            var options8=options(chrt);
            // Load the Visualization API and the corechart, map, etc... packages.
            //loadCharts('<%=chartpckgs(8)%>');
            google.charts.load('current', {'packages':['<%=chartpckgs(8)%>']});
            // Draw the chart when loaded.
            google.charts.setOnLoadCallback(drawChart8);
            // Callback that draws the chart.
            function drawChart8() {  // Create the data table for chart. 
                var data = google.visualization.arrayToDataTable([<%=arrs(8)%>]);
                // Set options for chart.
                var options = options8;
                options.title = '<%=ttls(8)%>';
                options.region = '<%=chartregns(8)%>';

                // Instantiate and draw the charts.     
                var chart = new google.visualization.<%=charttypes(8)%>(document.getElementById('chart_div_8'));
                chart.draw(data, options);
            }
        }
         // ----------------------------------- 9    -------------------------------------------------------
        if (n > 9) {
            var chrt = '<%=charttypes(9)%>';
             options9=options(chrt);
            // Load the Visualization API and the corechart, map, etc... packages.
            //loadCharts('<%=chartpckgs(9)%>');
            google.charts.load('current', {'packages':['<%=chartpckgs(9)%>']});
            // Draw the chart when loaded.
            google.charts.setOnLoadCallback(drawChart9);
            // Callback that draws the chart.
            function drawChart9() {  // Create the data table for chart. 
                var data = google.visualization.arrayToDataTable([<%=arrs(9)%>]);
                // Set options for chart.
                var options = options9;
                options.title = '<%=ttls(9)%>';
                options.region = '<%=chartregns(9)%>';

                // Instantiate and draw the charts.     
                var chart = new google.visualization.<%=charttypes(9)%>(document.getElementById('chart_div_9'));
                chart.draw(data, options);
            }
        }
         // ----------------------------------- 10    -------------------------------------------------------
        if (n > 10) {
            var chrt = '<%=charttypes(10)%>';
             options10=options(chrt);
            // Load the Visualization API and the corechart, map, etc... packages.
            //loadCharts('<%=chartpckgs(10)%>');
            google.charts.load('current', {'packages':['<%=chartpckgs(10)%>']});            // Draw the chart when loaded.
            google.charts.setOnLoadCallback(drawChart10);
            // Callback that draws the chart.
            function drawChart10() {  // Create the data table for chart. 
                var data = google.visualization.arrayToDataTable([<%=arrs(10)%>]);
                // Set options for chart.
                var options = options10;
                options.title = '<%=ttls(10)%>';
                options.region = '<%=chartregns(10)%>';

                // Instantiate and draw the charts.     
                var chart = new google.visualization.<%=charttypes(10)%>(document.getElementById('chart_div_10'));
                chart.draw(data, options);
            }
        }
         // ----------------------------------- 11    -------------------------------------------------------
        if (n > 11) {
            var chrt = '<%=charttypes(11)%>';
            var options11=options(chrt);
            // Load the Visualization API and the corechart, map, etc... packages.
            //loadCharts('<%=chartpckgs(11)%>');
            google.charts.load('current', {'packages':['<%=chartpckgs(11)%>']});
            // Draw the chart when loaded.
            google.charts.setOnLoadCallback(drawChart11);
            // Callback that draws the chart.
            function drawChart11() {  // Create the data table for chart. 
                var data = google.visualization.arrayToDataTable([<%=arrs(11)%>]);
                // Set options for chart.
                var options = options11;
                options.title = '<%=ttls(11)%>';
                options.region = '<%=chartregns(11)%>';

                // Instantiate and draw the charts.     
                var chart = new google.visualization.<%=charttypes(11)%>(document.getElementById('chart_div_11'));
                chart.draw(data, options);
            }
        }
       </script>    
   

  </head>
  <body>  
      <form id="form1" runat="server" width="100%" height="100%">
                <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />      
                <asp:UpdatePanel ID="udpDashboard" runat ="server" >
                 <ContentTemplate>
     &nbsp;&nbsp;&nbsp;&nbsp;
    <asp:LinkButton ID="LinkButtonBack" runat="server" >Back</asp:LinkButton>                     
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                     <asp:LinkButton  ID="LinkButtonRefresh" runat="server" Font-Size="Small">Refresh</asp:LinkButton>
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
             &nbsp;&nbsp;<asp:DropDownList ID="DropDownChartType" runat="server" AutoPostBack="True" Visible="False" Enabled="False">
                                                <asp:ListItem>PieChart</asp:ListItem>
                                                <asp:ListItem>BarChart</asp:ListItem> 
                                                <asp:ListItem>LineChart</asp:ListItem> 
                                                <asp:ListItem>AreaChart</asp:ListItem>                                              
                                                <asp:ListItem>ComboChart</asp:ListItem>                                                 
                                                <asp:ListItem>ScatterChart</asp:ListItem>
                                                <asp:ListItem>SteppedAreaChart</asp:ListItem>
                                                 <asp:ListItem>BubbleChart</asp:ListItem>
                                                <asp:ListItem>CandlestickChart</asp:ListItem>
                                                <asp:ListItem>Column</asp:ListItem> 
                                                    <asp:ListItem>Gauge</asp:ListItem>
                                                    <asp:ListItem>Histogram</asp:ListItem>
                                                <asp:ListItem>Sankey</asp:ListItem>
                                                <asp:ListItem>Map</asp:ListItem>
                                                <asp:ListItem>GeoChart</asp:ListItem>
                                            </asp:DropDownList>
               <%---------- UNDER CONSTRUCTION ----------%>
                 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;       
                 <asp:Label ID="LabelWhere" runat="server" Font-Underline="False" Text=""></asp:Label>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton  ID="lnkbtnAddToDashboard" runat="server" Font-Size="Small" Visible="False" Enabled="False">add to dashboard</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                     <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/ListOfReports.aspx" Font-Names="Arial" Font-Size="Small">List of Reports</asp:HyperLink>
               &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;    
               <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Default.aspx" Font-Names="Arial" Font-Size="Small">Log off</asp:HyperLink>                              &nbsp;
         
      <!--Table and divs that hold the chart-->
  
    <br />
    <table  runat="server" id="list"  border=0 style="font-size: 12px; font-family: Arial">
      <tr  id="tr1" runat ="server" >
        <td><div id="chart_div_0"   runat="server" style="border: 1px solid #ccc; width: 400px; height: 300px;"></div></td>
        <td><div id="chart_div_1"   runat="server" style="border: 1px solid #ccc; width: 400px; height: 300px;"></div></td>
        <td><div id="chart_div_2"   runat="server" style="border: 1px solid #ccc; width: 400px; height: 300px;"></div></td>
        <td><div id="chart_div_3"   runat="server" style="border: 1px solid #ccc; width: 400px; height: 300px;"></div></td>        
      </tr>
      <tr  id="trmax1" runat ="server" >
        <td><asp:LinkButton  ID="lnkbtn_0" runat="server" Font-Size="Small">maximize</asp:LinkButton> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:LinkButton  ID="lnkbtn_del_0" runat="server" Font-Size="Small" ToolTip="delete from dashboard">delete from dashboard</asp:LinkButton></td>
        <td><asp:LinkButton  ID="lnkbtn_1" runat="server" Font-Size="Small">maximize</asp:LinkButton> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:LinkButton  ID="lnkbtn_del_1" runat="server" Font-Size="Small" ToolTip="delete from dashboard">delete from dashboard</asp:LinkButton></td>
        <td><asp:LinkButton  ID="lnkbtn_2" runat="server" Font-Size="Small">maximize</asp:LinkButton> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:LinkButton  ID="lnkbtn_del_2" runat="server" Font-Size="Small" ToolTip="delete from dashboard">delete from dashboard</asp:LinkButton></td>
        <td><asp:LinkButton  ID="lnkbtn_3" runat="server" Font-Size="Small">maximize</asp:LinkButton> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:LinkButton  ID="lnkbtn_del_3" runat="server" Font-Size="Small" ToolTip="delete from dashboard">delete from dashboard</asp:LinkButton></td>
        
      </tr>
      <tr  id="tr2" runat ="server" >
        <td><div id="chart_div_4"  runat="server"  style="border: 1px solid #ccc; width: 400px; height: 300px;"></div></td>
        <td><div id="chart_div_5"  runat="server"  style="border: 1px solid #ccc; width: 400px; height: 300px;"></div></td>
        <td><div id="chart_div_6"  runat="server"  style="border: 1px solid #ccc; width: 400px; height: 300px;"></div></td>
        <td><div id="chart_div_7"  runat="server"  style="border: 1px solid #ccc; width: 400px; height: 300px;"></div></td>
        
      </tr>
      <tr  id="trmax2" runat ="server" >
        <td><asp:LinkButton  ID="lnkbtn_4" runat="server" Font-Size="Small">maximize</asp:LinkButton> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:LinkButton  ID="lnkbtn_del_4" runat="server" Font-Size="Small" ToolTip="delete from dashboard">delete from dashboard</asp:LinkButton></td>
        <td><asp:LinkButton  ID="lnkbtn_5" runat="server" Font-Size="Small">maximize</asp:LinkButton> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:LinkButton  ID="lnkbtn_del_5" runat="server" Font-Size="Small" ToolTip="delete from dashboard">delete from dashboard</asp:LinkButton></td>
        <td><asp:LinkButton  ID="lnkbtn_6" runat="server" Font-Size="Small">maximize</asp:LinkButton> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:LinkButton  ID="lnkbtn_del_6" runat="server" Font-Size="Small" ToolTip="delete from dashboard">delete from dashboard</asp:LinkButton></td>
        <td><asp:LinkButton  ID="lnkbtn_7" runat="server" Font-Size="Small">maximize</asp:LinkButton> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:LinkButton  ID="lnkbtn_del_7" runat="server" Font-Size="Small" ToolTip="delete from dashboard">delete from dashboard</asp:LinkButton></td>
        
      </tr>
      <tr  id="tr3" runat ="server" >
        <td><div id="chart_div_8"  runat="server"  style="border: 1px solid #ccc; width: 400px; height: 300px;"></div></td>
        <td><div id="chart_div_9"  runat="server"  style="border: 1px solid #ccc; width: 400px; height: 300px;"></div></td>
        <td><div id="chart_div_10"  runat="server"  style="border: 1px solid #ccc; width: 400px; height: 300px;"></div></td>
        <td><div id="chart_div_11"  runat="server"  style="border: 1px solid #ccc; width: 400px; height: 300px;"></div></td>
        
      </tr>
      <tr  id="trmax3" runat ="server" >
        <td><asp:LinkButton  ID="lnkbtn_8" runat="server" Font-Size="Small">maximize</asp:LinkButton> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:LinkButton  ID="lnkbtn_del_8" runat="server" Font-Size="Small" ToolTip="delete from dashboard">delete from dashboard</asp:LinkButton></td>
        <td><asp:LinkButton  ID="lnkbtn_9" runat="server" Font-Size="Small">maximize</asp:LinkButton> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:LinkButton  ID="lnkbtn_del_9" runat="server" Font-Size="Small" ToolTip="delete from dashboard">delete from dashboard</asp:LinkButton></td>
        <td><asp:LinkButton  ID="lnkbtn_10" runat="server" Font-Size="Small">maximize</asp:LinkButton> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:LinkButton  ID="lnkbtn_del_10" runat="server" Font-Size="Small" ToolTip="delete from dashboard">delete from dashboard</asp:LinkButton></td>
        <td><asp:LinkButton  ID="lnkbtn_11" runat="server" Font-Size="Small">maximize</asp:LinkButton> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:LinkButton  ID="lnkbtn_del_11" runat="server" Font-Size="Small" ToolTip="delete from dashboard">delete from dashboard</asp:LinkButton></td>
        
      </tr>
      
    </table>
                      &nbsp; &nbsp; &nbsp; &nbsp;<br /><br /> 
                        <asp:Label ID="LabelShare" runat="server" Text=" Send dashboard link to email address:" ForeColor="black" Font-Size="Small"></asp:Label>&nbsp;
                        <asp:TextBox ID="txtShareEmail" runat="server" ToolTip="Enter email address and link to dashboard will be sent.  Everybody who has the link will be able to see it."></asp:TextBox>&nbsp;<asp:Button ID="btnShare" runat="server" Text="Share" Font-Size="X-Small" />
                        <br /><br />
    <br />
    <div id="chart_div"></div>
      <asp:Label ID="LabelError" runat="server" Text=" " ForeColor="Red"></asp:Label> 

      <ucMsgBox:Msgbox id="MessageBox" runat ="server" > </ucMsgBox:Msgbox>
        
      </ContentTemplate>
      </asp:UpdatePanel>   
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpDashboard">
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

