Public Class Gallery

#Region "Global Declarations"

    Public WithEvents ThemesList As New ListView
    Dim CurrentTheme As Integer = 0
    Dim ThemeIndex As Integer = 0

#End Region

#Region "Main Form events"

    Private Sub Gallery_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        GetThemes.RunWorkerAsync()
    End Sub

#End Region

#Region "Events caused by the controls on the main form"

    Private Sub PreviousTheme_Click(sender As System.Object, e As System.EventArgs) Handles PreviousTheme.Click
        NextTheme.Enabled = True
        CurrentTheme -= 1
        ThemeImage.Image = Nothing
        Marquee.Show()
        RetryImage.Hide()
        ThemeInfo.Text = ThemesList.Items.Item(CurrentTheme).SubItems.Item(1).Text & " by " & ThemesList.Items.Item(CurrentTheme).SubItems.Item(2).Text
        If GetImage.IsBusy = False Then
            GetImage.RunWorkerAsync()
        End If

        If CurrentTheme - 1 < 0 Then
            PreviousTheme.Enabled = False
        End If
    End Sub

    Private Sub DownloadTheme_Click(sender As System.Object, e As System.EventArgs) Handles DownloadTheme.Click
        If DownloadTheme.Text = "Retry" = False Then
            DownloadTheme.Text = "Please wait..."
            DownloadTheme.Enabled = False
            NextTheme.Enabled = False
            PreviousTheme.Enabled = False
            GetTheme.RunWorkerAsync()
        Else
            GetThemes.RunWorkerAsync()
            DownloadTheme.Enabled = False
            Marquee.Show()
            ThemeInfo.Text = "Please wait, download themes info..."
        End If
    End Sub

    Private Sub NextTheme_Click(sender As System.Object, e As System.EventArgs) Handles NextTheme.Click
        PreviousTheme.Enabled = True
        CurrentTheme += 1
        ThemeImage.Image = Nothing
        Marquee.Show()
        RetryImage.Hide()
        ThemeInfo.Text = ThemesList.Items.Item(CurrentTheme).SubItems.Item(1).Text & " by " & ThemesList.Items.Item(CurrentTheme).SubItems.Item(2).Text
        If GetImage.IsBusy = False Then
            GetImage.RunWorkerAsync()
        End If

        If CurrentTheme + 1 > ThemesList.Items.Count - 1 Then
            NextTheme.Enabled = False
        End If
    End Sub

    Private Sub RetryImage_Click(sender As System.Object, e As System.EventArgs) Handles RetryImage.Click
        GetImage.RunWorkerAsync()
        RetryImage.Hide()
        Marquee.Show()
    End Sub

    Private Sub ThemeImage_LoadCompleted(sender As System.Object, e As System.ComponentModel.AsyncCompletedEventArgs) Handles ThemeImage.LoadCompleted
        If ThemeImage.Image Is Nothing Then
            RetryImage.Show()
        Else
            RetryImage.Hide()
        End If
    End Sub

#End Region

#Region "Background Workers"

    Private Sub GetThemes_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles GetThemes.DoWork
        Dim file As String = Player.exeFolder & "\themesinfo"
        Dim WebClient As Net.WebClient = New Net.WebClient()
        WebClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded")
        Dim request As Byte() = System.Text.Encoding.ASCII.GetBytes("db=1")

        Try
            Dim returnedData As Byte() = WebClient.UploadData("http://tobiass.eu/themes/browse", "POST", request)

            Dim writer As New IO.StreamWriter(file)
            writer.Write(System.Text.Encoding.UTF8.GetString(returnedData).Replace("},", "}" & vbNewLine))
            writer.Close()
            writer.Dispose()

            Dim reader As New IO.StreamReader(file)
            Do While (reader.Peek > -1)
                Dim whole As String = reader.ReadLine

                Dim splitter() As String = Split(whole, ":")

                ThemesList.Items.Add(splitter(3).Replace("""", Nothing).Replace(",name", Nothing))
                ThemesList.Items.Item(ThemesList.Items.Count - 1).SubItems.Add(splitter(4).Replace("""", Nothing).Replace(",creation", Nothing))
                ThemesList.Items.Item(ThemesList.Items.Count - 1).SubItems.Add(splitter(8).Replace("""", Nothing).Replace("}", Nothing))
            Loop
            reader.Close()
            reader.Dispose()

            Kill(file)

            ThemeInfo.Text = ThemesList.Items.Item(CurrentTheme).SubItems.Item(1).Text & " by " & ThemesList.Items.Item(CurrentTheme).SubItems.Item(2).Text
            ThemeImage.Load("http://tobiass.eu/themes/files/" & ThemesList.Items.Item(CurrentTheme).Text & ".png")
            NextTheme.Enabled = True
        Catch ex As Exception
            ThemeInfo.Text = "Couldn't download themes list. Please retry."
            DownloadTheme.Text = "Retry"
        End Try

        Marquee.Hide()
        DownloadTheme.Enabled = True
    End Sub

    Private Sub GetImage_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles GetImage.DoWork
startover:
        ThemeIndex = CurrentTheme

        Try
            ThemeImage.Load("http://tobiass.eu/themes/files/" & ThemesList.Items.Item(CurrentTheme).Text & ".png")
        Catch
        End Try


        If ThemeIndex = CurrentTheme = False Then
            ThemeImage.Image = Nothing
            GoTo startover
        End If

        Marquee.Hide()
    End Sub

    Private Sub GetTheme_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles GetTheme.DoWork
        Dim download As New Net.WebClient
        My.Computer.FileSystem.CreateDirectory(Player.exeFolder & "\themes")

        If My.Computer.FileSystem.FileExists(Player.exeFolder & "\themes\" & ThemesList.Items.Item(CurrentTheme).SubItems.Item(1).Text & ".cth") = False Then
            download.DownloadFile("http://tobiass.eu/themes/browse?dl=" & ThemesList.Items.Item(CurrentTheme).SubItems.Item(1).Text, Player.exeFolder & "\themes\" & ThemesList.Items.Item(CurrentTheme).SubItems.Item(1).Text & ".cth")
        End If
    End Sub

    Private Sub GetTheme_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles GetTheme.RunWorkerCompleted
        Player.ReadThemeFile(Player.exeFolder & "\themes\" & ThemesList.Items.Item(CurrentTheme).SubItems.Item(1).Text & ".cth")
        Player.ApplyTheme()
        DownloadTheme.Text = "Download and apply"
        DownloadTheme.Enabled = True

        If CurrentTheme < ThemesList.Items.Count - 1 Then
            NextTheme.Enabled = True
        End If

        If CurrentTheme > 0 Then
            PreviousTheme.Enabled = True
        End If

    End Sub

#End Region

End Class