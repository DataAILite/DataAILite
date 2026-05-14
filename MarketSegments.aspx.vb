Partial Class MarketSegments
    Inherits MarketAnalysisBase

    Protected Overrides ReadOnly Property MarketTitle As String
        Get
            Return "Market Segmentation Model"
        End Get
    End Property

    Protected Overrides ReadOnly Property MarketModel As String
        Get
            Return "Segments"
        End Get
    End Property
End Class
