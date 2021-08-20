Imports System.Windows.Forms
Imports System.Drawing
Imports System.Threading
Friend Class BackApplication : Implements IDisposable
    Private Declare Function ShowWindow Lib "user32" (ByVal handle As IntPtr, ByVal nCmdShow As Integer) As Integer
    Private WithEvents BTT_TRAY As NotifyIcon
    Private WithEvents StripMenu As ContextMenuStrip
    Private WithEvents BTT_CLOSE As ToolStripMenuItem
    Private ReadOnly ProcessList As List(Of Process)
    Friend ReadOnly ProcessName As String
    Friend ReadOnly ApplicationAddress As String
    Private ReadOnly AwaitTimer As Integer
    Private _CloseRequested As Boolean = False
    Friend Property HasError As Boolean = False
    Private IsHidden As Boolean = False
    Friend Sub New()
        Try
            ProcessList = New List(Of Process)
            Dim Args() As String = Environment.GetCommandLineArgs()
            If Not Args Is Nothing AndAlso Args.Length >= 3 Then
                ApplicationAddress = Args(1)
                ProcessName = Args(2)
                If Args.Length = 4 Then Integer.TryParse(Args(3), AwaitTimer)
            Else
                Throw New ArgumentException("The program to run is not specified")
            End If

            If AwaitTimer = 0 Then AwaitTimer = 30

            StripMenu = New ContextMenuStrip With {.UseWaitCursor = False}
            BTT_CLOSE = New ToolStripMenuItem With {.DisplayStyle = ToolStripItemDisplayStyle.Text, .Text = "Close"}
            StripMenu.Items.Add(BTT_CLOSE)
            BTT_TRAY = New NotifyIcon With {.Visible = True, .ContextMenuStrip = StripMenu}

            Dim i As Icon = Icon.ExtractAssociatedIcon(ApplicationAddress)
            If Not i Is Nothing Then BTT_TRAY.Icon = i Else BTT_TRAY.Icon = SystemIcons.Application

            Process.Start(ApplicationAddress)
            CheckProcesses()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical + MsgBoxStyle.OkOnly + MsgBoxStyle.DefaultButton1, "Tray Starter")
            HasError = True
            Dispose(False)
        End Try
    End Sub
#Region "Buttons"
    Private Sub BTT_TRAY_Click(sender As Object, e As EventArgs) Handles BTT_TRAY.Click
        ShowHideAppWindow()
    End Sub
    Private Sub BTT_CLOSE_Click(sender As Object, e As EventArgs) Handles BTT_CLOSE.Click
        _CloseRequested = True
    End Sub
#End Region
    Private Sub ShowHideAppWindow()
        Try
            Dim ShowWindowChanged As Boolean = False
            For Each p As Process In ProcessList
                Try
                    If Not p Is Nothing AndAlso Not p.HasExited Then ShowWindow(p.MainWindowHandle, IIf(IsHidden, 5, 0)) : ShowWindowChanged = True
                Catch pex As Exception
                End Try
            Next
            If ShowWindowChanged Then IsHidden = Not IsHidden
        Catch ex As Exception
        End Try
    End Sub
#Region "Process' worker"
    Private Class ProcessComparer : Implements IEqualityComparer(Of Process)
        Friend Overloads Function Equals(ByVal x As Process, ByVal y As Process) As Boolean Implements IEqualityComparer(Of Process).Equals
            Return x.Id = y.Id
        End Function
        Public Overloads Function GetHashCode(ByVal Obj As Process) As Integer Implements IEqualityComparer(Of Process).GetHashCode
            Throw New NotImplementedException()
        End Function
    End Class
    Friend Async Sub CheckProcesses()
        Try
            Dim NullCount% = 0
            Await Task.Run(Sub()
                               Dim i%
                               Dim p() As Process
                               Dim eq As New ProcessComparer
                               Thread.Sleep(AwaitTimer * 1000)
                               Do While Not disposedValue And NullCount < 5 And Not _CloseRequested And Not HasError
                                   p = Process.GetProcessesByName(ProcessName)
                                   If p.Count > 0 Then
                                       NullCount = 0
                                       If ProcessList.Count > 0 Then
                                           For i = ProcessList.Count - 1 To 0 Step -1
                                               If Not ProcessList(i).HasExited Then
                                                   If Not p.Contains(ProcessList(i), eq) Then
                                                       ProcessList(i).Dispose()
                                                       ProcessList.RemoveAt(i)
                                                   End If
                                               Else
                                                   ProcessList(i).Dispose()
                                                   ProcessList.RemoveAt(i)
                                               End If
                                           Next
                                       End If
                                       For i = 0 To p.Count - 1
                                           If Not ProcessList.Contains(p(i), eq) Then ProcessList.Add(p(i))
                                       Next
                                   Else
                                       NullCount += 1
                                   End If
                                   If _CloseRequested Then Exit Do Else Thread.Sleep(1000)
                               Loop
                           End Sub)
            If NullCount >= 5 Or _CloseRequested Then Dispose()
        Catch ex As Exception
        End Try
    End Sub
    Private Sub CloseProcesses()
        Try
            If ProcessList.Count > 0 Then
                For i% = 0 To ProcessList.Count - 1
                    With ProcessList(i)
                        If Not .HasExited Then .Kill() : .Dispose()
                    End With
                Next
                ProcessList.Clear()
            End If
        Catch ex As Exception
        End Try
    End Sub
#End Region
#Region "IDisposable Support"
    Private disposedValue As Boolean = False
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                BTT_TRAY.Visible = False
                CloseProcesses()
            End If
            ProcessList.Clear()
            LOADER.Dispose()
            disposedValue = True
        End If
    End Sub
    Protected Overrides Sub Finalize()
        Dispose(False)
        MyBase.Finalize()
    End Sub
    Friend Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class