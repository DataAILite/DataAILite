<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ChartGoogle.aspx.vb" Inherits="ChartGoogle" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<script type="text/javascript" src="Controls/Javascripts/OUR.js"></script>
<script type="text/javascript" src="ChartGoogleOne.js.aspx"></script>


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
    </style>
    <title>Google Charts</title>  
   
 <!--Load the AJAX API-->
    <script type="text/javascript" src="https://www.gstatic.com/charts/loader.js"></script>
    <script type="text/javascript">

      // Load the Visualization API and the corechart package.
      google.charts.load('current', {'packages':['<%=chartpckg%>']});

      // Draw the pie chart for Sarah's pizza when Charts is loaded.
      google.charts.setOnLoadCallback(drawSarahChart);

      // Draw the pie chart for the Anthony's pizza when Charts is loaded.
        google.charts.setOnLoadCallback(drawAnthonyChart);

      //draw statistical charts
      var chrt = "<%=charttype%>";

      // Draw the pie chart for the Count when Charts is loaded.
        google.charts.setOnLoadCallback(drawCountChart);
      // Callback that draws the pie chart for Count.
      function drawCountChart() {
        // Create the data table for Count.
        var data = new google.visualization.DataTable();
        data.addColumn('string', '<%=srt%>');
        data.addColumn('number', '<%=y1%>');
        data.addRows([<%=arrCount%>
        ]);
        // Set options for Count's pie chart.
        var options = {title:'<%=ttlCount%>',
                       width:400,
                       height:300};
        // Instantiate and draw the chart for Count.
        var chart = new google.visualization.<%=charttype%>(document.getElementById('Count_chart_div'));
         
        chart.draw(data, options);
        }


      // Draw the pie chart for the Distinct Count when Charts is loaded.
        google.charts.setOnLoadCallback(drawDistCountChart);
      // Callback that draws the pie chart for Distinct Count.
      function drawDistCountChart() {
        // Create the data table for Count.
        var data = new google.visualization.DataTable();
        data.addColumn('string', '<%=srt%>');
        data.addColumn('number', '<%=y1%>');
        data.addRows([<%=arrDistCount%>
        ]);
        // Set options for Count's pie chart.
        var options = {title:'<%=ttlDistCount%>',
                       width:400,
                       height:300};
        // Instantiate and draw the chart for Distinct Count.
        var chart = new google.visualization.<%=charttype%>(document.getElementById('DistCount_chart_div'));
        chart.draw(data, options);
        }


      // Draw the pie chart for the Value when Charts is loaded.
        google.charts.setOnLoadCallback(drawValueChart);
      // Callback that draws the pie chart for Value.
      function drawValueChart() {
        // Create the data table for Value.
        var data = new google.visualization.DataTable();
        data.addColumn('string', '<%=srt%>');
        data.addColumn('number', '<%=y1%>');
        data.addRows([<%=arrValue%>
        ]);
        // Set options for Value's pie chart.
        var options = {title:'<%=ttlValue%>',
                       width:400,
                       height:300};
        // Instantiate and draw the chart for Value.
        var chart = new google.visualization.<%=charttype%>(document.getElementById('Value_chart_div'));
        chart.draw(data, options);
        }



      // Draw the pie chart for the Sum when Charts is loaded.
        google.charts.setOnLoadCallback(drawSumChart);
      // Callback that draws the pie chart for Sum.
      function drawSumChart() {
        // Create the data table for Sum.
        var data = new google.visualization.DataTable();
        data.addColumn('string', '<%=srt%>');
        data.addColumn('number', '<%=y1%>');
        data.addRows([<%=arrSum%>
        ]);
        // Set options for Value's pie chart.
        var options = {title:'<%=ttlSum%>',
                       width:400,
                       height:300};
        // Instantiate and draw the chart for Sum.
        var chart = new google.visualization.<%=charttype%>(document.getElementById('Sum_chart_div'));
        chart.draw(data, options);
        }



      // Draw the pie chart for the Avg when Charts is loaded.
        google.charts.setOnLoadCallback(drawAvgChart);
      // Callback that draws the pie chart for Avg.
      function drawAvgChart() {
        // Create the data table for Avg.
        var data = new google.visualization.DataTable();
        data.addColumn('string', '<%=srt%>');
        data.addColumn('number', '<%=y1%>');
        data.addRows([<%=arrAvg%>
        ]);
        // Set options for Avg's pie chart.
        var options = {title:'<%=ttlAvg%>',
                       width:400,
                       height:300};
        // Instantiate and draw the chart for Avg.
        var chart = new google.visualization.<%=charttype%>(document.getElementById('Avg_chart_div'));
        chart.draw(data, options);
        }



      // Draw the pie chart for the StDev when Charts is loaded.
        google.charts.setOnLoadCallback(drawStDevChart);
      // Callback that draws the pie chart for StDev.
      function drawStDevChart() {
        // Create the data table for StDev.
        var data = new google.visualization.DataTable();
        data.addColumn('string', '<%=srt%>');
        data.addColumn('number', '<%=y1%>');
        data.addRows([<%=arrStDev%>
        ]);
        // Set options for StDev's pie chart.
        var options = {title:'<%=ttlStDev%>',
                       width:400,
                       height:300};
        // Instantiate and draw the chart for StDev.
        var chart = new google.visualization.<%=charttype%>(document.getElementById('StDev_chart_div'));
        chart.draw(data, options);
        }



      // Draw the pie chart for the Max when Charts is loaded.
        google.charts.setOnLoadCallback(drawMaxChart);
      // Callback that draws the pie chart for Max.
      function drawMaxChart() {
        // Create the data table for Max.
        var data = new google.visualization.DataTable();
        data.addColumn('string', '<%=srt%>');
        data.addColumn('number', '<%=y1%>');
        data.addRows([<%=arrMax%>
        ]);
        // Set options for Max's pie chart.
        var options = {title:'<%=ttlMax%>',
                       width:400,
                       height:300};
        // Instantiate and draw the chart for Max.
        var chart = new google.visualization.<%=charttype%>(document.getElementById('Max_chart_div'));
        chart.draw(data, options);
        }



      // Draw the pie chart for the Min when Charts is loaded.
        google.charts.setOnLoadCallback(drawMinChart);
      // Callback that draws the pie chart for Min.
      function drawMinChart() {
        // Create the data table for Min.
        var data = new google.visualization.DataTable();
        data.addColumn('string', '<%=srt%>');
        data.addColumn('number', '<%=y1%>');
        data.addRows([<%=arrMin%>
        ]);
        // Set options for Min's pie chart.
        var options = {title:'<%=ttlMin%>',
                       width:400,
                       height:300};
        // Instantiate and draw the chart for Min.
        var chart = new google.visualization.<%=charttype%>(document.getElementById('Min_chart_div'));
        chart.draw(data, options);
        }



      // Callback that draws the pie chart for Sarah's pizza.
      function drawSarahChart() {

        // Create the data table for Sarah's pizza.
        var data = new google.visualization.DataTable();
        data.addColumn('string', 'Topping');
        data.addColumn('number', 'Slices');
        data.addRows([
          ['Mushrooms', 1],
          ['Onions', 1],
          ['Olives', 2],
          ['Zucchini', 2],
          ['Pepperoni', 1]
        ]);

        // Set options for Sarah's pie chart.
        var options = {title:'How Much Pizza Sarah Ate Last Night',
                       width:500,
                       height:300};

        // Instantiate and draw the chart for Sarah's pizza.
        var chart = new google.visualization.BarChart(document.getElementById('Sarah_chart_div'));
        chart.draw(data, options);
        }
         

      // Callback that draws the pie chart for Anthony's pizza.
      function drawAnthonyChart() {

        // Create the data table for Anthony's pizza.
        var data = new google.visualization.DataTable();
        data.addColumn('string', 'Topping');
        data.addColumn('number', 'Slices');
        data.addRows([<%=arr%>
          //['Mushrooms', 2],
          //['Onions', 2],
          //['Olives', 2],
          //['Zucchini', 0],
          //['Pepperoni', 3]
        ]);

        // Set options for Anthony's pie chart.
        var options = {title:'<%=ttl%>',
                       width:400,
                       height:300};

        // Instantiate and draw the chart for Anthony's pizza.
        var chart = new google.visualization.PieChart(document.getElementById('Anthony_chart_div'));
        chart.draw(data, options);
      }
    </script>

     <!--Load the AJAX API-->
    <script type="text/javascript" src="https://www.gstatic.com/charts/loader.js?key=<%=Session("mapkey")%>"></script>
  <script>
      loadCharts('map');
    google.charts.setOnLoadCallback(drawMap);

    function drawMap() {
      var data = google.visualization.arrayToDataTable([
         ['Lat', 'Long', 'Name'],
    [37.4232, -122.0853, 'Work'],
    [37.4289, -122.1697, 'University'],
    [37.6153, -122.3900, 'Airport'],
    [37.4422, -122.1731, 'Shopping']
      ]);

    var options = {
      showTooltip: true,
      showInfoWindow: true
    };

    var map = new google.visualization.Map(document.getElementById('chart_div'));

    map.draw(data, options);
      };

      loadCharts('geochart');
     google.charts.setOnLoadCallback(drawMarkersMap);

      function drawMarkersMap() {
      var data = google.visualization.arrayToDataTable([
        ['City',   'Population', 'Area'],
        ['Rome',      2761477,    1285.31],
        ['Milan',     1324110,    181.76],
        ['Naples',    959574,     117.27],
        ['Turin',     907563,     130.17],
        ['Palermo',   655875,     158.9],
        ['Genoa',     607906,     243.60],
        ['Bologna',   380181,     140.7],
        ['Florence',  371282,     102.41],
        ['Fiumicino', 67370,      213.44],
        ['Anzio',     52192,      43.43],
        ['Ciampino',  38262,      11]
      ]);

      var options = {
        region: 'IT',
        displayMode: 'markers',
        colorAxis: {colors: ['green', 'blue']}
      };

      var chart = new google.visualization.GeoChart(document.getElementById('geochart_div'));
      chart.draw(data, options);
    };
  </script>
  </head>
  <body>  
            <form id="form1" runat="server">
       <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />      
     &nbsp;&nbsp;&nbsp;&nbsp;
    <asp:LinkButton ID="LinkButtonBack" runat="server">Back</asp:LinkButton>             
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        Chart type:&nbsp;&nbsp;
        <asp:DropDownList ID="DropDownChartType" runat="server" AutoPostBack="True">
                                                <asp:ListItem>PieChart</asp:ListItem>
                                                <asp:ListItem>BarChart</asp:ListItem> 
                                                <asp:ListItem>LineChart</asp:ListItem> 
                                                <asp:ListItem>AreaChart</asp:ListItem>                                              
                                                <asp:ListItem>ComboChart</asp:ListItem>                                                 
                                                <asp:ListItem>ScatterChart</asp:ListItem>
                                                <asp:ListItem>SteppedAreaChart</asp:ListItem>                                                
                                                <asp:ListItem>ColumnChart</asp:ListItem> 
                                                <asp:ListItem>Histogram</asp:ListItem>
                                                
                                            </asp:DropDownList>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                 <asp:LinkButton  ID="lnkbtnReverse" runat="server" Font-Size="Small" Visible="False" Enabled="False">reverse group order</asp:LinkButton>
                  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;       
                 <asp:Label ID="LabelWhere" runat="server" Font-Underline="False" Text=""></asp:Label>
                 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/ListOfReports.aspx" Font-Names="Arial" Font-Size="Small">List of Reports</asp:HyperLink>
                 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Default.aspx" Font-Names="Arial" Font-Size="Small">Log off</asp:HyperLink>
        &nbsp;
      <!--Table and divs that hold the pie charts-->
    <table class="columns" runat="server" id="stats" >
      <tr>
        <td><div id="Count_chart_div" runat="server" style="border: 1px solid #ccc"></div></td>
        <td><div id="DistCount_chart_div" runat="server" style="border: 1px solid #ccc"></div></td>
        <td><div id="Value_chart_div" runat="server" style="border: 1px solid #ccc"></div></td>
        <td><div id="Sum_chart_div" runat="server" style="border: 1px solid #ccc"></div></td>
      </tr>
      <tr>
        <td><asp:LinkButton  ID="lnkbtnCount" runat="server" Font-Size="Small">maximize</asp:LinkButton> </td>
        <td><asp:LinkButton  ID="lnkbtnDistCount" runat="server" Font-Size="Small">maximize</asp:LinkButton> </td>
        <td><asp:LinkButton  ID="lnkbtnValue" runat="server" Font-Size="Small">maximize</asp:LinkButton> </td>
        <td><asp:LinkButton  ID="lnkbtnSum" runat="server" Font-Size="Small">maximize</asp:LinkButton> </td>
      </tr>
      <tr>
        <td><div id="Avg_chart_div" runat="server" style="border: 1px solid #ccc"></div></td>
        <td><div id="StDev_chart_div" runat="server" style="border: 1px solid #ccc"></div></td>
        <td><div id="Max_chart_div" runat="server" style="border: 1px solid #ccc"></div></td>
        <td><div id="Min_chart_div" runat="server" style="border: 1px solid #ccc"></div></td>
      </tr>
      <tr>
        <td><asp:LinkButton  ID="lnkbtnAvg" runat="server" Font-Size="Small">maximize</asp:LinkButton> </td>
        <td><asp:LinkButton  ID="lnkbtnStDev" runat="server" Font-Size="Small">maximize</asp:LinkButton> </td>
        <td><asp:LinkButton  ID="lnkbtnMax" runat="server" Font-Size="Small">maximize</asp:LinkButton> </td>
        <td><asp:LinkButton  ID="lnkbtnMin" runat="server" Font-Size="Small">maximize</asp:LinkButton> </td>
      </tr>
      <%--<tr>
        <td><div id="Sarah_chart_div" style="border: 1px solid #ccc"></div></td>
        <td><div id="Anthony_chart_div" style="border: 1px solid #ccc"></div></td>
        <td><div id="Anthony_chart_div" style="border: 1px solid #ccc"></div></td>
      </tr>--%>
    </table>
    <br />
                <asp:Label ID="LabelError" runat="server" Text=" " ForeColor="Red"></asp:Label>

        <div id="spinner" class="modal" style="display:none;">
            <div id="divSpinner" style="text-align: center; width: 130px; display: inline-block; clip: rect(auto, auto, auto, 50%); position: fixed; margin-top: 300px; margin-right: auto; padding-top: 10px; background-color: #f8f8d3; z-index: 2147483647; left: 50%; top: -5%;">
                <img id="imgSpinner" src="Controls/Images/WaitImage2.gif" style="width: 100px; height: 100px" />
                <br />
                    Please Wait...
            </div>
        </div>

        <%--<ucMsgBox:Msgbox id="MessageBox" runat ="server" > </ucMsgBox:Msgbox>--%>
    </form>
  </body>
</html>

