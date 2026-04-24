<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ChartGoogleOne.aspx.vb" Inherits="ChartGoogleOne" %>

<%@ Register TagPrefix="uc1" TagName="DropDownColumns" src="~/Controls/uc1.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script type="text/javascript" src="Controls/Javascripts/ChartGoogleOne.js"></script>

    <script type="text/javascript" src="ChartGoogleOne.js.aspx"></script>

    <style type="text/css">
        .PopupBackground {
            background-color: rgba(158, 188, 250,0.5);
        }
        .Popup {
            position:absolute;
            top:25%;
            left:25%;
            width:50%;
            height:320px;
            background-color:#e6eefa ;
            font-family:Arial;
            z-index: 2147483650;
            border:1px solid #222222;
        }
        .close {
            color: white;
            float: right;
            font-size: 20px;
            font-weight: bold;
            padding-right:10px;
        }
        .close:hover,
        .close:focus {
            color: blue;
            text-decoration: none;
            cursor: pointer;
        }

        .clearfix::after {
          content: "";
          clear: both;
          display: table;
        }
        .column {
          float: left;
          padding: 0px;
        }
        .content {
          width: 80%;
          height:400px;
        }
        .dlgboxbutton 
        {
          width: 80px;
          height: 25px;
          font-size: 12px;
          border-radius: 5px;
          border-style :solid;
          border-color: #4e4747 ;
          color: black;
          border-width: 1px;
          background-image: linear-gradient(to bottom, rgba(158, 188, 250,0),rgba(158, 188, 250,1));
          padding: 3px;
          margin:5px;
          z-index: 9999; 
        }
        .buttons {
          width: 19%;
          height:120px;
          text-align:center;
        }
        .btnClose {
        position:absolute;
        top:75px;
        left:10px;
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

        .auto-style2 {
            height: 850px;
        }

    </style>

    <title>Google Charts</title>  
   
 <!--Load the AJAX API-->
    <script type="text/javascript" src="https://www.gstatic.com/charts/loader.js"></script>
    <script type="text/javascript">

      // Load the Visualization API and the corechart, map, etc... packages.
        //loadCharts('<%=chartpckg%>');
      google.charts.load('current', {'packages':['<%=chartpckg%>']});
      var chrt = "<%=charttype%>";

      // Draw the chart when loaded.
       
            google.charts.setOnLoadCallback(drawChart);
       
      // Callback that draws the chart.
        function drawChart()
        {
            
           // Create the data table for chart. 
            
                var data = google.visualization.arrayToDataTable([<%=Session("arr")%>]);
            
            

           // Set options for chart.
            var url = 'https://icons.iconarchive.com/icons/icons-land/vista-map-markers/48/';
            if ('<%=charttype%>' == 'Map')
            {            
                var options =
                {
                    title: '<%=ttl%>',
                    titleTextStyle: { color: 'black', fontSize: 16, bold: true },
                    tooltip: '<%=ttl%>',

                    width: <%=chartwidth%>,
                    height:  <%=chartheght%>,
                    left: 0,
                    top: 0,
                    chartArea: { left: 130, top: 100, right: 180, bottom: 30 },
                    //colors: ['blue', 'gold', 'lightgreen', 'red', 'yellow', 'darkred','lightblue', 'green','#e5e4e2', 'orange', 'darkgreen', 'pink','darkblue', 'lightyellow', 'maroon', 'salmon', 'darkorange'],
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
                    },

                    //zoomLevel: 6,
                    //zoomControl: true,
                    //zoomControlOptions: {
                    //      style: google.maps.ZoomControlStyle.SMALL
                    //},
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
            }
            else
            {
                if ('<%=charttype%>' == 'GeoChart')
                {
                    var options =
                    {
                        title: '<%=ttl%>',
                        titleTextStyle: { color: 'black', fontSize: 16, bold: true },
                        tooltip: '<%=ttl%>',
                        
                        width: <%=chartwidth%>,
                        height: <%=chartheght%>,
                        left: 0,
                        top: 0,
                        chartArea: { left: 130, top: 100, right: 180, bottom: 30 },
                        region: '<%=chartregn%>',
                        displayMode: 'markers',
                        colorAxis: { colors: ['green', 'blue'] },
                        //zoomControl: true,
                        //zoomControlOptions: {
                        //  style: google.maps.ZoomControlStyle.SMALL
                        //}
                    };
                }
                else
                {
                    if ('<%=charttype%>' == 'CandlestickChart') {
                        var options = {
                            legend: 'none',
                            bar: { groupWidth: '100%' }, // Remove space between bars.
                            candlestick: {
                                fallingColor: { strokeWidth: 0, fill: '#a52714' }, // red
                                risingColor: { strokeWidth: 0, fill: '#0f9d58' }   // green
                            }
                        };
                    }
                    else {
                    
                        var options =
                        {
                            title: '<%=ttl%>',
                            titleTextStyle: { color: 'black', fontSize: 16, bold: true },
                            width: <%=chartwidth%>,
                            height: <%=chartheght%>,
                            left: 0,
                            top: 0,
                            chartArea: { left: 130, top: 100, right: 180, bottom: 30 },
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
                            //,
                            //sankey: {
                            //    node: {
                            //        colors: colors
                            //    },
                            //    link: {
                            //        colorMode: 'gradient',
                            //        colors: colors
                            //    }
                            //}

                        };
                    };
                };
            };

            <%--if ('<%=charttype%>' == 'Map') {

                //options.zoomLevel = 6;

                zoomControl: true;
                zoomControlOptions: {
                          style: google.maps.ZoomControlStyle.SMALL
                };
            };--%>

             if ('<%=charttype%>' == 'Gauge') {
                    
                 var options = {
                    width: 1900, height: 2000,
                    redFrom: 90, redTo: 100,
                    yellowFrom:75, yellowTo: 90,
                    minorTicks: 5
                };              
            };
            if ('<%=charttype%>' == 'Sankey') {               

                 //var colors = ['#a6cee3', '#b2df8a', '#fb9a99', '#fdbf6f', '#cab2d6', '#ffff99', '#1f78b4', '#33a02c'];
                var colors = ['blue', 'gold', 'lightgreen', 'red', 'yellow', 'darkred', 'lightblue', 'green', '#e5e4e2', 'orange', 'darkgreen', 'pink', 'darkblue', 'lightyellow', 'maroon', 'salmon', 'darkorange'];
                //var colors = ['blue', 'gold', 'lightgreen', 'red', 'yellow', 'darkred', 'lightblue', 'green'];  //, '#e5e4e2', 'orange', 'darkgreen', 'pink', 'darkblue', 'lightyellow', 'maroon', 'salmon', 'darkorange'];
                          

                var options = {
                        width: <%=chartwidth%>,
                        height: <%=chartheght%>,
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
            };
           
            // Instantiate and draw the chart.
           
               var chart = new google.visualization.<%=charttype%>(document.getElementById('chart_div'));                

           
                chart.draw(data, options);
           
            
            
             <%--if ('<%=charttype%>' == 'Gauge') {
                   
                // setInterval(function() {
                //    data.setValue(0, 1, 40 + Math.round(60 * Math.random()));
                //    chart.draw(data, options);
                //    }, 13000);
                //setInterval(function() {
                //    data.setValue(1, 1, 40 + Math.round(60 * Math.random()));
                //    chart.draw(data, options);
                //    }, 5000);
                //setInterval(function() {
                //    data.setValue(2, 1, 60 + Math.round(20 * Math.random()));
                //    chart.draw(data, options);
                //    }, 26000);
            };--%>
        }

    </script>

  </head>
  <body>  
            <form id="form1" runat="server">
                <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />      
   <%-- <asp:UpdatePanel ID="udpChart" runat ="server" >
      <ContentTemplate>--%>
    <asp:HiddenField id="hdnDefaultDashboard" runat ="server" Value=""/>
    <asp:HiddenField id="hdnDefaultDashboardAvailable" runat="server" Value="true"/>
     &nbsp;&nbsp;
    <asp:LinkButton ID="LinkButtonBack" runat="server">Back</asp:LinkButton>                     
             &nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;               
       <asp:LinkButton  ID="LinkButtonData" runat="server" Font-Size="Small">Data</asp:LinkButton>
            &nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
       <asp:LinkButton  ID="LinkButtonRefresh" runat="server" Font-Size="Small">Refresh</asp:LinkButton>           
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton  ID="lnkbtnReverse" runat="server" Font-Size="Small" Visible="False" Enabled="False">reverse group order</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                     <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/ListOfReports.aspx" Font-Size="Small">List of Reports</asp:HyperLink> 
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton  ID="lnkbtnAddToDashboard" runat="server" Font-Size="Small">add to dashboard</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton  ID="lnkDownloadARR" runat="server" Font-Size="Small">download chart data</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                  
        <asp:HyperLink ID="HyperLinkDataAI" runat="server" NavigateUrl="~/DataAI.aspx?pg=charts" CssClass="NodeStyle" Font-Names="Arial" Font-Size="12px" Visible="True" ToolTip="DataAI analytics" Font-Bold="False">DataAI</asp:HyperLink>
    <%--&nbsp;&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;--%>
                <asp:LinkButton  ID="lnkDataAI" runat="server" Font-Size="Small" Visible="False">DataAI</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Default.aspx" Font-Names="Arial" Font-Size="Small">Log off</asp:HyperLink>
                <br />
                &nbsp;&nbsp;
         <asp:Label ID="LabelWhere" runat="server" Font-Underline="False" Text="" Font-Bold="True" ForeColor="#990033" Font-Size="Small"></asp:Label>
                <br /> <br />
               <%-- &nbsp;&nbsp;--%>
                   &nbsp;&nbsp;
        Chart type:
        &nbsp;&nbsp;
        <asp:DropDownList ID="DropDownChartType" runat="server" AutoPostBack="True">
                                                <asp:ListItem>PieChart</asp:ListItem>
                                                <asp:ListItem>BarChart</asp:ListItem> 
                                                <asp:ListItem>Column</asp:ListItem> 
                                                <asp:ListItem>LineChart</asp:ListItem>
                                                <asp:ListItem>AreaChart</asp:ListItem>                                              
                                                <asp:ListItem>ComboChart</asp:ListItem>                                                 
                                                <asp:ListItem>ScatterChart</asp:ListItem>
                                                <asp:ListItem>SteppedAreaChart</asp:ListItem>
                                                 <asp:ListItem>BubbleChart</asp:ListItem>
                                                <asp:ListItem>CandlestickChart</asp:ListItem>                                                
                                                <asp:ListItem>Gauge</asp:ListItem>
                                                <asp:ListItem>Histogram</asp:ListItem>
                                                <asp:ListItem>Sankey</asp:ListItem>
                                                <asp:ListItem>GeoChart</asp:ListItem>
                                                <asp:ListItem>MapChart</asp:ListItem>
                                                
                                            </asp:DropDownList>
                 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;       
                
                <br />
                <table id="tableM">
                    <tr>
                        <td>
                           &nbsp;&nbsp;                    

                        </td>
                        <td>                    
                                   <asp:Label ID="LabelX" runat="server" Text="X Axis:" ForeColor="Black" ToolTip="X axes will show combined values of the selected fields"></asp:Label>
                                   &nbsp;&nbsp;&nbsp;&nbsp;
                                   <div>
                                       <uc1:DropDownColumns ID="DropDownColumnsX" runat="server" Width="300px" ClientIDMode="Predictable" FontName="arial" FontSize="Small" BorderColor="Silver" ForeColor="Black" BorderWidth="1" DropDownHeight="190px" ToolTip="X axes will show combined values of the selected fields." PostBackType="OnClose" TextBoxHeight="20px" DropDownButtonHeight="22px" />
                                   </div>
                                   &nbsp;&nbsp;&nbsp;&nbsp;
               
                         </td>
                         <td> 
                                   <asp:Label ID="LabelY" runat="server" Text="Y Axis:" ForeColor="Black" ToolTip="Y axes will show value of the selected field. If multi fields selected, there will be multilines the each selected field"></asp:Label>
                                   &nbsp;&nbsp;&nbsp;&nbsp;
                                   <div>
                                       <uc1:DropDownColumns ID="DropDownColumnsY" runat="server" Width="300px" ClientIDMode="Predictable" FontName="arial" FontSize="Small" BorderColor="Silver" ForeColor="Black" BorderWidth="1" DropDownHeight="190px" ToolTip="Y axes will show value of the selected field. If multi fields selected, there will be multilines the each selected field." PostBackType="OnClose" TextBoxHeight="20px" DropDownButtonHeight="22px" />
                                   </div> 
                                  
                                   &nbsp;&nbsp;<%--&nbsp;&nbsp;     --%>                  
                              
                        </td>    
                        <td>
                            <br />
                             <asp:CheckBox ID="chkboxNumeric" runat="server" Checked="False" Text="numeric" Font-Names="Arial" Font-Size="X-Small"  AutoPostBack="True" Enabled="True" ToolTip="Axis Y field values are numeric. Do all statistics as for numeric field." />
                             &nbsp;&nbsp;<%--&nbsp;&nbsp;--%>
                        </td>
                        <td>
                                    <asp:Label ID="LabelA" runat="server" Text=" Aggregation function: "  ToolTip="Aggregation functions: For Text fields (Count, Count Distinct only) and for Numeric fields (Sum, Avg, Max, Min, StDev, Value as well)"></asp:Label>
                                    &nbsp;&nbsp;&nbsp;&nbsp; <br /> 
                                    <asp:DropDownList ID="DropDownListA" runat="server"  ToolTip="For Text fields (Count, Count Distinct only) and for Numeric fields (Sum, Avg, Max, Min, StDev, Value as well) aggregate functions"></asp:DropDownList>
                                    &nbsp;&nbsp;&nbsp;&nbsp;
                        </td>
                        <td>
                                     &nbsp;&nbsp;&nbsp;&nbsp;
                                    <asp:Label ID="LabelM" runat="server" Text="Multilines by field:" ForeColor="Black" ToolTip="Optional - Multilines the each value of selected field"></asp:Label>
                                    &nbsp;&nbsp;&nbsp;&nbsp; <br /> &nbsp;&nbsp;&nbsp;&nbsp;
                                    <asp:DropDownList ID="DropDownListM" runat="server" ToolTip="Optional - Multilines the each value of selected field" AutoPostBack="True"></asp:DropDownList>  
                          
                        <td> 
                                   <asp:Label ID="LabelV" runat="server" Text="Values:" ForeColor="Black" ToolTip="Optional - Multilines the each selected value of the field"></asp:Label>
                                   &nbsp;&nbsp;&nbsp;&nbsp;
                                   &nbsp;&nbsp;&nbsp;<asp:CheckBox ID="CheckBoxSelectAll" runat="server" AutoPostBack="True" Text="select all" /> 
                                   &nbsp;&nbsp;&nbsp;&nbsp;<asp:CheckBox ID="CheckBoxUnselectAll" runat="server" AutoPostBack="True" Text="unselect all" />
                                   <div>
                                       <uc1:DropDownColumns ID="DropDownColumnsV" runat="server" Width="300px" ClientIDMode="Predictable" FontName="arial" FontSize="Small" BorderColor="Silver" ForeColor="Black" BorderWidth="1" DropDownHeight="190px" ToolTip="Optional - Multilines the each selected value of the field" PostBackType="OnClose" TextBoxHeight="20px" DropDownButtonHeight="22px" />
                                   </div> 
                                   &nbsp;&nbsp;&nbsp;&nbsp;
                         </td>
                        <td>
                            <br />
                           &nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnShowChart" runat="server" CssClass="ticketbutton" Text="show chart" Visible="true" valign="center" ToolTip="Show Chart" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        </td>
                    </tr>
                </table>  
                         
          <%--*********************************** Holds Defined Dashboards ***************************--%>
        <div id="divDDDashBoards" runat="server" style="width:200px; display:none;">
             <uc1:DropDownColumns id="ddDashboards" runat="server" width="200px" DropDownHeight="150px"/>
        </div>
        <%--*******************************Lookup Dialog******************************--%>
        <div id="divPopupBackground" runat="server" class="PopupBackground" style="display:none;height:100%;width:100%;position:absolute;top:0px;left:0px;">
          <div id="divPopup" runat="server" class="Popup"  >
            <div id="divHeading" style="font-size: small; text-align: center; background-color: gray; width: 100%; height: 22px; line-height:22px; color: white">
                Add To Dashboard
                <span id="spnClose" runat ="server" class="close" title="close dialog">&times;</span>
            </div>
            <div id="divPopupBody " class="clearfix" >
                <div id="divContent" class="column content" runat="server" style="margin-left: 5px;">
                    <div id="divSearch" runat ="server" style=" display:inline; width: 100%; height: 30px; padding-top: 5px; padding-bottom: 5px; ">
                        Name:
                        <asp:Textbox ID="txtSearch" runat="server" Wrap="False" Width="200px"></asp:Textbox>
                        <asp:Button ID="btnFind" runat="server" CssClass="dlgboxbutton" Text="Find" CausesValidation="false" UseSubmitBehavior="True" />
                    </div>
                    <div id="divListHeader" runat="server" style="background-color: darkgray; border: 1px solid #808080; height: 20px; line-height:20px; padding-left: 8px; color: #FFFFFF;">
                        <asp:label ID="lblListHeader" runat="server" Text="Dashboards"></asp:label>
                    </div> 
                    <div id="divList" runat="server" style="border-style: none solid solid solid; border-right-width: 1px; border-bottom-width: 1px; border-left-width: 1px; border-right-color: #808080; border-bottom-color: #808080; border-left-color: #808080; height:225px; overflow-y:scroll;">
                        <asp:CheckBoxList ID="lstItems" runat="server" Width="100%" BorderStyle="None">
                            <asp:ListItem></asp:ListItem>
                        </asp:CheckBoxList>
                    </div>  
                </div>
                <div id="divPopupButtons" class="column buttons" >
                    <asp:Button ID="btnSubmit" runat="server" CssClass="dlgboxbutton" Text="Add" CausesValidation="false" />
                    <br />
                    <asp:Button ID="btnClose" runat="server" CssClass="dlgboxbutton btnCancel" Text="Cancel" CausesValidation="false" />
                </div>
            </div>
          </div>
        </div>

        <%--***********************************Spinner************************************--%>
            <div id="spinner" class="modal" style="display:none;">
                <div id="divSpinner" style="text-align: center; width: 130px; display: inline-block; clip: rect(auto, auto, auto, 50%); position: fixed; margin-top: 300px; margin-right: auto; padding-top: 10px; background-color: #f8f8d3; z-index: 2147483647; left: 50%; top: -5%;">
                  <img id="imgSpinner" src="Controls/Images/WaitImage2.gif" style="width: 100px; height: 100px" />
                    <br />
                      Please Wait...
                </div>
            </div>
      <!--Table and divs that hold the chart-->
  
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="LabelInfo1" runat="server" Font-Underline="False" Text="* PieChart shows data for the one first selected field from the Y Axis list." Font-Size="X-Small"></asp:Label>
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="LabelInfo2" runat="server" Font-Underline="False" Text="* BubbleChart, Gauge, Sankey show data for two selected fields from the X Asis list and one selected field from the Y Axis list." Font-Size="X-Small"></asp:Label>
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="LabelInfo3" runat="server" Font-Underline="False" Text="* If multiple values for selected categories the Average return instead." ToolTip="If multiple values for selected categories the Average return instead." Font-Size="X-Small"></asp:Label>
                
                <br />
                <br /><br /><br /><br /><br /><br /><br /><br />
                 <asp:Label ID="LabelError" runat="server" Text=" " ForeColor="Red"></asp:Label> <br />

    <div id="chart_div"  runat="server" class="auto-style2"></div>
     

        <%--<ucMsgBox:Msgbox id="MessageBox" runat ="server" > </ucMsgBox:Msgbox>--%>
<%--      </ContentTemplate>
    </asp:UpdatePanel>   
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpChart">
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

