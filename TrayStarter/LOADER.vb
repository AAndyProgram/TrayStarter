Public Class LOADER
    Public Sub New()
        InitializeComponent()
    End Sub
    Private Sub LOADER_Load(sender As Object, e As EventArgs) Handles Me.Load
        Hide()
        MyApp = New BackApplication
    End Sub
End Class