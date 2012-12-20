Public Class Changelog

    Private Sub GetChangelog_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles GetChangelog.DoWork
        Dim WebClient As Net.WebClient = New Net.WebClient()
        Dim Text As String

        Try
            Text = WebClient.DownloadString("http://www.tobiass.eu/files/Changelog.txt")
        Catch ex As Exception
            Text = "Couldn't download the changelog due to the following error:" & vbNewLine & vbNewLine & ex.Message & vbNewLine & vbNewLine & "Please try again later."
        End Try

        Changelog.Text = Text
    End Sub

    Private Sub Form3_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyUp
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    Private Sub Form3_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        GetChangelog.RunWorkerAsync()
    End Sub

    Private Sub Changelog_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles Changelog.KeyUp
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    Private Sub Changelog_LinkClicked(sender As Object, e As System.Windows.Forms.LinkClickedEventArgs) Handles Changelog.LinkClicked
        Process.Start(e.LinkText)
    End Sub

End Class