Partial Class MarketInventory
    Inherits MarketAnalysisBase

    Protected Overrides ReadOnly Property MarketTitle As String
        Get
            Return "Market Inventory Model"
        End Get
    End Property

    Protected Overrides ReadOnly Property MarketModel As String
        Get
            Return "Inventory"
        End Get
    End Property
End Class
