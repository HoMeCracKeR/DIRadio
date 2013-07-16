Public Class Export

#Region "Global Declarations"

    Dim KeysArray As New ListView
    Dim use12hs As Boolean = True
    Dim path As String

    Public dataFolder As String

#End Region

#Region "Main Form Events"

    Private Sub Export_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If String.IsNullOrEmpty(My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\DI Radio", "installDir", Nothing)) = True Then
            Dim executable As String = Application.ExecutablePath
            Dim tabla() As String = Split(executable, "\")
            dataFolder = Application.ExecutablePath.Replace(tabla(tabla.Length - 1), Nothing)
        Else
            dataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) & "\DI Radio\"
        End If

        Dim item As String

        For Each item In Player.SelectedChannel.Items
            If item = "My Favorites" = False Then
                ChannelsList.Items.Add(item)
            End If
        Next

        If Player.SelectedChannel.Text = "My Favorites" Then
            ChannelsList.SelectedItem = Player.SelectedServer.Text
        Else
            ChannelsList.SelectedItem = Player.SelectedChannel.Text
        End If

        Dim readerChdb As New IO.StreamReader(dataFolder & "servers\Digitally Imported\channels.db")

        Do While (readerChdb.Peek > -1)
            Dim line = readerChdb.ReadLine()
            Dim splitter = Split(line, "|")
            KeysArray.Items.Add(splitter(1))
            KeysArray.Items.Item(KeysArray.Items.Count - 1).Tag = splitter(0)
            KeysArray.Items.Item(KeysArray.Items.Count - 1).Name = splitter(2)
        Loop

        readerChdb.Close()
        readerChdb.Dispose()

    End Sub

#End Region

#Region "Events caused by the controls in the main form"

    Private Sub Label4_Click(sender As System.Object, e As System.EventArgs) Handles Label4.Click
        If SetReminder.Checked = True Then
            SetReminder.Checked = False
        Else
            SetReminder.Checked = True
        End If
    End Sub

    Private Sub ChannelsList_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ChannelsList.SelectedIndexChanged

        AvailableBox.Items.Clear()
        AvailableBox.Enabled = False
        ToDown.Enabled = False
        AvailableBox.Items.Add("Please wait...")
        ExportLabel.Text = "Status: Downloading events..."

        If GetEvents.IsBusy = False Then
            GetEvents.RunWorkerAsync()
        End If

    End Sub

    Private Sub ToDown_Click(sender As System.Object, e As System.EventArgs) Handles ToDown.Click

        Dim item As ListViewItem

        For Each item In AvailableBox.SelectedItems
            SaveBox.Items.Add(item.Text)
            SaveBox.Items.Item(SaveBox.Items.Count - 1).Tag = item.Tag
            AvailableBox.Items.Remove(item)
        Next

        SaveFile.Enabled = True

    End Sub

    Private Sub ToUp_Click(sender As System.Object, e As System.EventArgs) Handles ToUp.Click

        Dim item As ListViewItem

        For Each item In SaveBox.SelectedItems

            If item.Tag.ToString.StartsWith(ChannelsList.Text) Then
                AvailableBox.Items.Add(item.Text)
                AvailableBox.Items.Item(AvailableBox.Items.Count - 1).Tag = item.Tag
            End If

            SaveBox.Items.Remove(item)
        Next

        If SaveBox.Items.Count = 0 Then
            ToUp.Enabled = False
            SaveFile.Enabled = False
        End If

    End Sub

    Private Sub AvailableBox_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles AvailableBox.SelectedIndexChanged

        If AvailableBox.SelectedItems.Count > 0 Then
            ToDown.Enabled = True
        Else
            ToDown.Enabled = False
        End If

    End Sub

    Private Sub SaveBox_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles SaveBox.SelectedIndexChanged

        If SaveBox.SelectedItems.Count > 0 Then
            ToUp.Enabled = True
        Else
            ToUp.Enabled = False
        End If

    End Sub

    Private Sub SaveFile_Click(sender As System.Object, e As System.EventArgs) Handles SaveFile.Click

        If SaveFile.Text = "Save to file" Then
            ExportICS.FileName = ""

            If ExportICS.ShowDialog = Windows.Forms.DialogResult.OK Then
                Exporter.RunWorkerAsync()
                ExportLabel.Text = "Status: Exporting (0/" & SaveBox.Items.Count & ")"
                ExportLabel.Show()
                OptionsBox.Enabled = False
                ChannelsList.Enabled = False
                AvailableBox.Enabled = False
                SaveBox.Enabled = False
                ToUp.Enabled = False
                ToDown.Enabled = False
                SaveFile.Text = "Cancel"
            End If
        Else
            Exporter.CancelAsync()
            SaveFile.Enabled = False
            ExportLabel.Text += ", cancelling..."
        End If

    End Sub

    Private Sub ReminderMinutes_ValueChanged(sender As System.Object, e As System.EventArgs) Handles ReminderMinutes.ValueChanged

        If ReminderMinutes.Value > 0 Then
            SetReminder.Checked = True
        Else
            SetReminder.Checked = False
        End If


    End Sub

    Private Sub Retry_Click(sender As System.Object, e As System.EventArgs) Handles Retry.Click

        GetEvents.RunWorkerAsync()
        Retry.Hide()

    End Sub

#End Region

#Region "Background Workers"

    Private Sub GetEvents_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles GetEvents.DoWork
        Dim WebClient As Net.WebClient = New Net.WebClient()
        Dim eventsfile As String = ""
        Dim channel As String
        Dim success As Boolean = False
startover:
        channel = ChannelsList.Text
        AvailableBox.Enabled = False

        Try
            eventsfile = WebClient.DownloadString("http://api.audioaddict.com/v1/di/events/channel/" & KeysArray.Items.Item(ChannelsList.Text).Tag)
            success = True
        Catch ex As Exception
            AvailableBox.Items.Clear()
            AvailableBox.Items.Add("Couldn't download events. Please try again.")
            Retry.Show()
            success = False
        End Try

        If success Then

            Dim reader As New IO.StringReader(eventsfile.Replace("{", Nothing).Replace("[", Nothing).Replace("}", Nothing).Replace("]", Nothing).Replace(",""", vbNewLine))

            AvailableBox.Items.Clear()

            ExportLabel.Text = "Status: Reading events file..."

            Dim id As String = ""
            Dim start_at As Date
            Dim end_at As Date
            Dim name As String = ""
            Dim artists_tagline As String

            Do While (reader.Peek > -1)

                If channel = ChannelsList.Text = False Then
                    GoTo startover
                End If

                Dim line As String = reader.ReadLine


                If line.StartsWith("""id""") OrElse line.StartsWith("id""") Then
                    id = Split(line, ":")(1)
                ElseIf line.StartsWith("start_at") Then
                    start_at = Split(line, """:""")(1).Replace("""", Nothing)
                ElseIf line.StartsWith("end_at") Then
                    end_at = Split(line, """:""")(1).Replace("""", Nothing)
                ElseIf line.StartsWith("name") Then
                    name = Split(line, """:""")(1).Replace("""", Nothing)
                ElseIf line.StartsWith("artists_tagline") Then
                    artists_tagline = Split(line, """:""")(1).Replace("""", Nothing)

                    AvailableBox.Items.Add(Player.ReturnDate((start_at - New DateTime(1970, 1, 1)).TotalSeconds, "fulldate") & ": " & name)
                    AvailableBox.Items.Item(AvailableBox.Items.Count - 1).Tag = ChannelsList.Text
                    AvailableBox.Items.Item(AvailableBox.Items.Count - 1).Tag += "|" & id
                    AvailableBox.Items.Item(AvailableBox.Items.Count - 1).Tag += "|" & (start_at - New DateTime(1970, 1, 1)).TotalSeconds
                    AvailableBox.Items.Item(AvailableBox.Items.Count - 1).Tag += "|" & (end_at - New DateTime(1970, 1, 1)).TotalSeconds
                    AvailableBox.Items.Item(AvailableBox.Items.Count - 1).Tag += "|" & name
                    AvailableBox.Items.Item(AvailableBox.Items.Count - 1).Tag += "|" & artists_tagline

                End If

            Loop

            reader.Close()
            reader.Dispose()

            If AvailableBox.Items.Count > 0 Then
                AvailableBox.Enabled = True
            Else
                AvailableBox.Items.Add("There are no future events for this channel")
            End If

            ExportLabel.Text = "Status: Idle"

        End If

    End Sub

    Private Sub Exporter_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles Exporter.DoWork
        Dim item As ListViewItem
        Dim firstDay As DateTime = #1/1/1970#
        Dim writer As New IO.StreamWriter(ExportICS.FileName)
        Dim cancelled As Boolean = False
        writer.WriteLine("BEGIN:VCALENDAR")
        writer.WriteLine("VERSION:2.0")
        writer.WriteLine("PRODID:-//" & Player.Text)

        For Each item In SaveBox.Items

            If Exporter.CancellationPending = True Then
                cancelled = True
                Exit For
            End If

            Dim splitter() As String = Split(SaveBox.Items.Item(item.Index).Tag.ToString(), "|")

            Dim time As DateTime = firstDay.AddSeconds(splitter(2))

            Dim starttime As String = String.Format("{0:00}{1:00}{2:00}T{3:00}{4:00}{5:00}Z", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second)

            time = firstDay.AddSeconds(splitter(3))

            Dim endtime As String = String.Format("{0:00}{1:00}{2:00}T{3:00}{4:00}{5:00}Z", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second)

            writer.WriteLine("BEGIN:VEVENT")
            writer.WriteLine("UID:" & starttime & "_" & splitter(1) & "@Digitally Imported")
            writer.WriteLine("DTSTAMP:" & starttime)
            writer.WriteLine("DTSTART:" & starttime)
            writer.WriteLine("DTEND:" & endtime)
            writer.WriteLine("SUMMARY:" & splitter(4))

            Dim description As String = ""

            If IncludeDescription.Checked = True Then

                Try

                    Dim downloadDescription As Net.WebClient = New Net.WebClient()
                    description = downloadDescription.DownloadString("http://api.audioaddict.com/v1/di/events/" & splitter(1))

                    Dim reader As New IO.StringReader(description.Replace("{", Nothing).Replace("[", Nothing).Replace("}", Nothing).Replace("]", Nothing).Replace(",""", vbNewLine))

                    Do While (reader.Peek > -1)

                        Dim line As String = reader.ReadLine

                        If line.StartsWith("description""") Then
                            description = Split(line, """:""")(1).Replace("**", Nothing).Replace("\""", """").Replace("_", Nothing)
                            description = description.Remove(description.Length - 1, 1)
                            Exit Do
                        End If

                    Loop

                Catch

                End Try

            End If

            Dim URL As String = ""

            If IncludeLink.Checked = True Then
                URL = "\n http://www.di.fm/calendar/event/" & splitter(1)
            End If
            writer.WriteLine("DESCRIPTION:" & description & URL)

            Dim transparency As String = "TRANSP"

            If ScheduleAsBusy.Checked = True Then
                transparency = "OPAQUE"
            End If

            writer.WriteLine("TRANSP:" & transparency)

            If SetReminder.Checked = True And ReminderMinutes.Value > 0 Then
                writer.WriteLine("BEGIN:VALARM")
                writer.WriteLine("ACTION:DISPLAY")
                writer.WriteLine("TRIGGER:-PT" & ReminderMinutes.Value & "M")
                writer.WriteLine("END:VALARM")
            End If

            writer.WriteLine("END:VEVENT")

            ExportLabel.Text = "Status: Exporting (" & item.Index + 1 & "/" & SaveBox.Items.Count & ")"

        Next

        writer.Write("END:VCALENDAR")
        writer.Close()
        writer.Dispose()

        OptionsBox.Enabled = True
        ChannelsList.Enabled = True
        AvailableBox.Enabled = True
        AvailableBox_SelectedIndexChanged(Me, Nothing)
        SaveBox_SelectedIndexChanged(Me, Nothing)
        SaveBox.Enabled = True
        SaveFile.Text = "Save to file"
        SaveFile.Enabled = True

        If cancelled Then
            Kill(ExportICS.FileName)
            ExportLabel.Text = "Status: Cancelled"
        Else
            ExportLabel.Text = "Status: Finished"
        End If

    End Sub

#End Region

#Region "Others"

    Private Sub ExportICS_HelpRequest(sender As System.Object, e As System.EventArgs) Handles ExportICS.HelpRequest
        MessageBox.Show("Exporting events to an iCalendar file means that you will be able to load it using almost any calendar application (such as Outlook or Thunderbird with Lightning) or website (such as Google Calendar or Live Calendar) to automatically create events in the date and time the radio shows start until they end.", "Event exporting", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

#End Region

End Class