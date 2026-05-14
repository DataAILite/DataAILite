Partial Class MarketProfit
    Inherits MarketAnalysisBase

    Protected Overrides ReadOnly Property MarketTitle As String
        Get
            Return "Market Profitability Model"
        End Get
    End Property

    Protected Overrides ReadOnly Property MarketModel As String
        Get
            Return "Profit"
        End Get
    End Property
End Class
