Partial Class MarketElasticity
    Inherits MarketAnalysisBase

    Protected Overrides ReadOnly Property MarketTitle As String
        Get
            Return "Market Pricing Elasticity Model"
        End Get
    End Property

    Protected Overrides ReadOnly Property MarketModel As String
        Get
            Return "Elasticity"
        End Get
    End Property
End Class
