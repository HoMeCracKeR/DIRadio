Public Class Export
    ' All this code shall be revised before 1.16 goes final
    ' to look for optimizations and remove code duplicates
    ' and such.

    Dim KeysArray As New ListView
    Dim use12hs As Boolean = True
    Dim path As String

    Private Sub Export_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
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

        Dim readerChdb As New IO.StreamReader(Player.exeFolder & "\servers\Digitally Imported\channels.db")

        Do While (readerChdb.Peek > -1)
            Dim line = readerChdb.ReadLine()
            Dim splitter = Split(line, "|")
            KeysArray.Items.Add(splitter(0))
            KeysArray.Items.Item(KeysArray.Items.Count - 1).Tag = splitter(2)
            KeysArray.Items.Item(KeysArray.Items.Count - 1).Name = splitter(0)
        Loop

        readerChdb.Close()
        readerChdb.Dispose()

        If New DateTime(2000, 1, 1, 13, 0, 0).ToString.Contains("13") Then
            use12hs = False
        End If
    End Sub

    Private Sub ChannelsList_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ChannelsList.SelectedIndexChanged
        AvailableBox.Items.Clear()
        AvailableBox.Enabled = False
        ToDown.Enabled = False

        If GetEvents.IsBusy = False Then
            GetEvents.RunWorkerAsync()
        End If
    End Sub

    Private Sub GetEvents_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles GetEvents.DoWork
        AvailableBox.Items.Add("Please wait...")

        Dim WebClient As Net.WebClient = New Net.WebClient()
        Dim eventsfile As String
        Dim file As String = Player.exeFolder & "\servers\exporttemp"
        Dim writer As New IO.StreamWriter(file)

        eventsfile = WebClient.DownloadString("http://a.pi1.nl/calendar/di/filter/channel/" & KeysArray.Items.Item(ChannelsList.Text).Tag)

        writer.Write(eventsfile)
        writer.Close()
        writer.Dispose()

        Dim reader As New IO.StreamReader(file)

        AvailableBox.Items.Clear()

        Do While (reader.Peek > -1)

            Dim line As String = reader.ReadLine
            Dim splitter() As String = Split(line, "|&|")

            Dim firstDay As DateTime = #1/1/1970#
            Dim time As DateTime = firstDay.AddSeconds(splitter(1))
            Dim numeral As String

            If time.ToLocalTime.Day.ToString.EndsWith("1") And time.ToLocalTime.Day < 11 Then
                numeral = "st"
            ElseIf time.ToLocalTime.Day.ToString.EndsWith("2") And time.ToLocalTime.Day < 12 Then
                numeral = "nd"
            ElseIf time.ToLocalTime.Day.ToString.EndsWith("3") And time.ToLocalTime.Day < 13 Then
                numeral = "rd"
            Else
                numeral = "th"
            End If

            Dim ampm As String
            Dim hour As String

            If use12hs Then

                If time.ToLocalTime.Hour >= 12 Then
                    ampm = "pm"
                    If time.ToLocalTime.Hour = 12 Then
                        hour = time.ToLocalTime.Hour
                    Else
                        hour = time.ToLocalTime.Hour - 12
                    End If
                Else
                    ampm = "am"
                    hour = time.ToLocalTime.Hour

                    If hour = "0" Then
                        hour = "12"
                    End If
                End If
            Else
                hour = time.ToLocalTime.Hour
            End If

            AvailableBox.Items.Add(time.ToLocalTime.Day & numeral & ", " & String.Format("{0:00}:{1:00}" & ampm, hour, time.ToLocalTime.Minute) & ": " & splitter(3))
            AvailableBox.Items.Item(AvailableBox.Items.Count - 1).Tag = ChannelsList.Text
            AvailableBox.Items.Item(AvailableBox.Items.Count - 1).Tag += "|" & splitter(0)
            AvailableBox.Items.Item(AvailableBox.Items.Count - 1).Tag += "|" & splitter(1)
            AvailableBox.Items.Item(AvailableBox.Items.Count - 1).Tag += "|" & splitter(2)
            AvailableBox.Items.Item(AvailableBox.Items.Count - 1).Tag += "|" & splitter(3)
            AvailableBox.Items.Item(AvailableBox.Items.Count - 1).Tag += "|" & splitter(4)

            Try

            Catch

            End Try
        Loop

        reader.Close()
        reader.Dispose()
        Kill(file)

        AvailableBox.Enabled = True
    End Sub

    Private Sub ToDown_Click(sender As System.Object, e As System.EventArgs) Handles ToDown.Click
        Dim item As ListViewItem

        For Each item In AvailableBox.SelectedItems
            SaveBox.Items.Add(item.Text)
            SaveBox.Items.Item(SaveBox.Items.Count - 1).Tag = item.Tag
            AvailableBox.Items.Remove(item)
        Next

        If SaveBox.Items.Count > 0 Then
            SaveFile.Enabled = True
        Else
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
            If ExportICS.ShowDialog = Windows.Forms.DialogResult.OK Then
                Exporter.RunWorkerAsync()
                ExportLabel.Text = "Exporting (0/" & SaveBox.Items.Count & ")"
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
            SaveFile.Enabled = True
        End If

    End Sub

    Private Sub ToUp_Click(sender As System.Object, e As System.EventArgs) Handles ToUp.Click
        Dim item As ListViewItem

        For Each item In SaveBox.SelectedItems

            If item.Tag.ToString.StartsWith(ChannelsList.Text) Then
                AvailableBox.Items.Add(item.Text)
                AvailableBox.Items.Item(SaveBox.Items.Count - 1).Tag = item.Tag
            End If

            SaveBox.Items.Remove(item)
        Next
    End Sub

    Private Sub ReminderMinutes_ValueChanged(sender As System.Object, e As System.EventArgs) Handles ReminderMinutes.ValueChanged
        SetReminder.Checked = True
    End Sub

    Private Sub Exporter_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles Exporter.DoWork
        Dim item As ListViewItem
        Dim firstDay As DateTime = #1/1/1970#
        Dim writer As New IO.StreamWriter(ExportICS.FileName)
        writer.WriteLine("BEGIN:VCALENDAR")
        writer.WriteLine("VERSION:2.0")
        writer.WriteLine("PRODID:-//" & Player.Text)

        For Each item In SaveBox.Items

            If Exporter.CancellationPending = True Then
                Exit For
            End If

            Dim splitter() As String = Split(SaveBox.Items.Item(item.Index).Tag.ToString(), "|")

            Dim time As DateTime = firstDay.AddSeconds(splitter(2))

            Dim starttime As String = String.Format("{0:00}{1:00}{2:00}T{3:00}{4:00}{5:00}Z", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second)

            time = firstDay.AddSeconds(splitter(3))

            Dim endtime As String = String.Format("{0:00}{1:00}{2:00}T{3:00}{4:00}{5:00}Z", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second)

            writer.WriteLine("BEGIN:VEVENT")
            writer.WriteLine("UID:" & starttime & "@Digitally Imported")
            writer.WriteLine("DTSTAMP:" & starttime)
            writer.WriteLine("DTSTART:" & starttime)
            writer.WriteLine("DTEND:" & endtime)
            writer.WriteLine("SUMMARY:" & splitter(4))

            Dim link As String = ""

            If IncludeLink.Checked = True Then
                link = "\n http://www.di.fm/calendar/event/" & splitter(1)
            End If

            Dim description As String = ""

            If IncludeDescription.Checked = True Then

                Try

                    Dim downloadDescription As Net.WebClient = New Net.WebClient()
                    description = downloadDescription.DownloadString("http://a.pi1.nl/calendar/di/filter/event/" & splitter(1))
                    Dim splitterDesc() As String = Split(description, "|&|")
                    description = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.Default.GetBytes(splitterDesc(5).Replace("**", Nothing).Replace("_", Nothing).Replace(Chr(10), vbNewLine)))
                Catch

                End Try

            End If
            writer.WriteLine("DESCRIPTION:" & description & link)

            Dim transparency As String = "TRANSP"

            If ScheduleAsBusy.Checked = True Then
                transparency = "OPAQUE"
            End If

            writer.WriteLine("TRANSP:" & transparency)

            If SetReminder.Checked = True And ReminderMinutes.Value > 0 Then
                writer.WriteLine("BEGIN:VALARM")
                writer.WriteLine("ACTION:DISPLAY")
                writer.WriteLine("TRIGGER;VALUE=DURATION:-PT" & ReminderMinutes.Value & "M")
                writer.WriteLine("END:VALARM")
            End If

            writer.WriteLine("END:VEVENT")

            ExportLabel.Text = "Exporting (" & item.Index + 1 & "/" & SaveBox.Items.Count & ")"

        Next

        writer.Write("END:VCALENDAR")
        writer.Close()
        writer.Dispose()

        OptionsBox.Enabled = True
        ChannelsList.Enabled = True
        AvailableBox.Enabled = True
        SaveBox.Enabled = True
        ToUp.Enabled = True
        ToDown.Enabled = True
        SaveFile.Text = "Save to file"
        SaveFile.Enabled = True
    End Sub
End Class