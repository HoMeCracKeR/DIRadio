Public Class Changelog

#Region "Main form events"

    Private Sub Changelog_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        GetChangelog.RunWorkerAsync()
    End Sub

#End Region

#Region "Events caused by the controls in the main form"

    Private Sub Changelog_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyUp
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    Private Sub ChangelogText_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles ChangelogText.KeyUp
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    Private Sub ChangelogText_LinkClicked(sender As Object, e As System.Windows.Forms.LinkClickedEventArgs) Handles ChangelogText.LinkClicked
        Process.Start(e.LinkText)
    End Sub

    Private Sub Retry_Click(sender As System.Object, e As System.EventArgs) Handles Retry.Click
        ChangelogText.Text = "Please wait, downloading changelog..."
        GetChangelog.RunWorkerAsync()
        Retry.Hide()
    End Sub

#End Region

#Region "Background Worker"

    Private Sub GetChangelog_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles GetChangelog.DoWork
        Dim WebClient As Net.WebClient = New Net.WebClient()
        Dim Text As String

        Try
            Text = WebClient.DownloadString("http://www.tobiass.eu/files/Changelog.txt")
        Catch ex As Exception
            Text = "Couldn't download the changelog due to the following error:" & vbNewLine & vbNewLine & ex.Message
        End Try

        ChangelogText.Text = Text
    End Sub

    Private Sub GetChangelog_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles GetChangelog.RunWorkerCompleted
        If ChangelogText.Text.ToLower.StartsWith("couldn't download") Then
            Retry.Show()
        End If
    End Sub

#End Region

End Class