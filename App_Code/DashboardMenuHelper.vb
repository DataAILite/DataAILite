Public NotInheritable Class DashboardMenuHelper
    Private Sub New()
    End Sub

    Public Shared Sub ApplyListOfReportsMenu(tree As System.Web.UI.WebControls.TreeView)
        If tree Is Nothing Then Exit Sub

        tree.Nodes.Clear()
        tree.Nodes.Add(New System.Web.UI.WebControls.TreeNode("<b>Log off</b>", "~/Default.aspx"))

        Dim docs As New System.Web.UI.WebControls.TreeNode("<b>Documentation</b>", "DataAIHelp.aspx")
        docs.Expanded = False
        docs.Target = "_blank"
        AddDoc(docs, "Reports Demo", "https://oureports.net/OUReports/Default.aspx?logon=demo&pass=demo")
        AddDoc(docs, "General documentation ", "https://oureports.net/OUReports/OnlineUserReporting.pdf")
        AddDoc(docs, "Advanced Report Designer ", "https://oureports.net/OUReports/AdvancedReportDesigner.pdf#page=4")
        AddDoc(docs, "Video: Advanced Report Designer - Tabular Reports ", "https://oureports.net/OUReports/Videos/AdvancedReportDesigner Tabular.mp4")
        AddDoc(docs, "Video: Advanced Report Designer - HeaderFooter ", "https://oureports.net/OUReports/Videos/AdvancedReportDesigner-HeaderFooter.mp4")
        AddDoc(docs, "Video: Advanced Report Designer - Free Form ", "https://oureports.net/OUReports/Videos/AdvancedReportDesigner-FreeForm.mp4")
        AddDoc(docs, "Charts and Dashboards ", "https://oureports.net/OUReports/GoogleChartsAndDashboards.pdf")
        AddDoc(docs, "Video: DataAI - Data Analytics and Instant Reporting ", "https://oureports.net/OUReports/Videos/DataImport.mp4")
        AddDoc(docs, "Video: Charts, Maps, and Dashboards ", "https://oureports.net/OUReports/Videos/zoom_2.mp4")
        AddDoc(docs, "Video: Quick Start (only email needed) ", "https://oureports.net/OUReports/Videos/QuickStart.mp4")
        AddDoc(docs, "Video: Individual Registration, user database ", "https://oureports.net/OUReports/Videos/UserRegistrationVideo.mp4")
        AddDoc(docs, "Video: Individual Registration, use our database ", "https://oureports.net/OUReports/Videos/RegOurDb.mp4")
        AddDoc(docs, "Video: Company Registration ", "https://oureports.net/OUReports/Videos/UnitRegistrationVideo.mp4")
        AddDoc(docs, "Video: Input from Access ", "https://oureports.net/OUReports/Videos/InputFromAccess.mp4")
        AddDoc(docs, "Video: Matrix Balancing ", "https://oureports.net/OUReports/Videos/MatrixBalance.mp4")
        AddDoc(docs, "Dashboards documentation", "https://oureports.net/OUReports/DashboardHelp.pdf")
        AddDoc(docs, "Sample: Covid 2020 Dashboard", "https://oureports.net/OUReports/default.aspx?srd=30&dash=yes&lgn=d720202024346P906")
        AddDoc(docs, "Sample: Public data", "https://oureports.net/OUReports/UseCasePublic.aspx")
        AddDoc(docs, "Explore data", "https://oureports.net/OUReports/ExploreData.pdf")
        AddDoc(docs, "Matrix Balancing", "https://oureports.net/OUReports/MatrixBalancing.pdf#page=2")
        AddDoc(docs, "More Matrix Balancing Samples", "https://oureports.net/OUReports/MatrixBalancingSamples.pdf")
        AddDoc(docs, "Video: Matrix Balanceing Scenarios 1a and 1b", "https://oureports.net/OUReports/Videos/MatrixBalance1a1b.mp4")
        AddDoc(docs, "Video: Matrix Balanceing Scenarios 2a and 3a", "https://oureports.net/OUReports/Videos/MatrixBalance2a3a.mp4")
        AddDoc(docs, "Video: Matrix Balanceing Scenarios 2b and 2c", "https://oureports.net/OUReports/Videos/MatrixBalance2b2c.mp4")
        AddDoc(docs, "Video: Matrix Balanceing Scenarios 3b and 3c", "https://oureports.net/OUReports/Videos/MatrixBalance3b3c.mp4")
        AddDoc(docs, "Video: Matrix Balanceing Scenarios 4a, 4b, and 4c", "https://oureports.net/OUReports/Videos/MatrixBalance4a4b4c.mp4")
        AddDoc(docs, "Making Google Maps and Earth documentation ", "https://oureports.net/OUReports/MapDefinitionDocumentation.pdf")
        AddDoc(docs, "Task List documentation ", "https://oureports.net/OUReports/Tasklist.pdf")

        tree.Nodes.Add(docs)
    End Sub

    Private Shared Sub AddDoc(parent As System.Web.UI.WebControls.TreeNode, text As String, url As String)
        parent.ChildNodes.Add(New System.Web.UI.WebControls.TreeNode(text, url, "", url, "_blank"))
    End Sub
End Class
