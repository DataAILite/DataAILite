Partial Class MarketBasket
    Inherits MarketAnalysisBase

    Protected Overrides ReadOnly Property MarketTitle As String
        Get
            Return "Market Basket Model"
        End Get
    End Property

    Protected Overrides ReadOnly Property MarketModel As String
        Get
            Return "Basket"
        End Get
    End Property
End Class
