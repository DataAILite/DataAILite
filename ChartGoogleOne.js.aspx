<%@ Page Language="VB" AutoEventWireup="false" ContentType="text/javascript" %>

function loadCharts(packages) {
    google.charts.load('current',{'packages':[packages],mapsApiKey:'<%=Session("mapkey")%>'})
}
