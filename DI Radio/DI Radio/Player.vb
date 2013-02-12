' DI Radio Player by ViRUS
'
'
' I've done my best to document the most relevant parts of this source code, but if you still find yourself having problems
' with it, feel free to drop me an e-mail at newvirus@live.com.ar
'
' This source code is protected by the BSD license and you should have received a "BSD License.txt" file with it.
' If you haven't, please drop me an e-mail at newvirus@live.com.ar with information on where you downloaded the source code
' 
'
' I'm currently struggling getting custom Hotkeys to work properly. Sometimes they do work, other times a hotkey will do other action
' than the one set to and most of the times they simply don't work. If while you're poking around here you come up with a solution,
' please drop me an e-mail. I'll give you a mention on the About tab and in the forums (plus, helping other people out always feels good)
'
'
' One last note. This source code will compile as-is, but Bass.Net will display a popup when you start the player since I've erased my
' e-mail and registration key (you can't publicly share it). To get a registration key head over to http://bass.radio42.com/bass_register.html
' Registering is free and shouldn't take you more than 5 minutes. Once you have your registration key, go into the Application Events and into
' the Startup event. You'll find where to put your registration info there.
'
' Have fun!

Imports Un4seen.Bass
Imports Un4seen.Bass.Misc

Public Class Player

#Region "Global Declarations"

#Region "Options"

    ' Most of these get a value once the LoadOptions function is called.
    ' If there's no need for a default value, then it simply doesn't have one.
    ' They are ordered in the same way as they appear in the Options dialog.

    Public NotificationTitle As Boolean
    Public PlayNewOnChannelChange As Boolean
    Public NotificationIcon As Boolean
    Public NoTaskbarButton As Boolean
    Public GoogleSearch As Boolean

    Public PremiumFormats As Boolean
    Public DIFormat As Integer = 0
    Public SKYFormat As Integer = 0
    Public JazzFormat As Integer = 0
    Public ListenKey As String

    Public UpdatesAtStart As Boolean
    Public BetaVersions As Boolean

    Public Visualisation As Boolean
    Public VisualisationType As Integer
    Public HighQualityVis As Boolean
    Public LinealRepresentation As Boolean
    Public FullSoundRange As Boolean
    Public Smoothness As Integer
    Public MainColour As Integer
    Public SecondaryColour As Integer
    Public PeakColour As Integer
    Public BackgroundColour As Integer
    Public ChangeWholeBackground As Boolean

    Public MultimediaKeys As Boolean

    Public Band0 As Integer = 0
    Public Band1 As Integer = 0
    Public Band2 As Integer = 0
    Public Band3 As Integer = 0
    Public Band4 As Integer = 0
    Public Band5 As Integer = 0

    ' These are not on the Options dialog but are saved anyway.

    Public DIChannel As Integer = 0     ' -> Last used Digitally Imported channel
    Public SKYChannel As Integer = 0    ' -> Last used SKY.FM channel
    Public JazzChannel As Integer = 0   ' -> Last used JazzRadio channel
    Public RockChannel As Integer = 0   ' -> Last used RockRadio channel

    Public RadioStation As String = "Digitally Imported"    ' -> Last used radio station

    Public OldFav As String ' -> This value is only used if My Favourites was chosen when the user closed the player

#End Region

#Region "Hotkeys"

    ' HumanModifiers    -> It's used so that, for example, 393216 becomes CTRL+ALT+Space in the options dialog
    ' Modifiers         -> Used to register the combination of modifier keys. For example, 3 registers CTRL+ALT as modifiers
    ' Key               -> The keycode of the non-modifier key. For example, 32 is the space bar

    Dim KeyConverter As New KeysConverter

    Public HumanModifiersPlayStop As Integer
    Public ModifiersPlayStop As Integer
    Public KeyPlayStop As Integer

    Public HumanModifiersVolumeUp As Integer
    Public ModifiersVolumeUp As Integer
    Public KeyVolumeUp As Integer

    Public HumanModifiersVolumeDown As Integer
    Public ModifiersVolumeDown As Integer
    Public KeyVolumeDown As Integer

    Public HumanModifiersMuteUnmute As Integer
    Public ModifiersMuteUnmute As Integer
    Public KeyMuteUnmute As Integer

    Public HumanModifiersShowHide As Integer
    Public ModifiersShowHide As Integer
    Public KeyShowHide As Integer

    Public Declare Function RegisterHotKey Lib "user32" (ByVal hwnd As IntPtr, ByVal id As Integer, ByVal fsModifiers As Integer, ByVal vk As Integer) As Integer
    Public Declare Function UnregisterHotKey Lib "user32" (ByVal hwnd As IntPtr, ByVal id As Integer) As Integer
    Public Const WM_HOTKEY As Integer = &H312

#End Region

#Region "Update stuff"

    Public AtStartup As String = False          ' -> Used to tell the GetUpdates background worker that it's looking for updates at startup. Only becomes True if UpdatesAtStart is true
    Public TotalVersionString As String         ' -> Used to store the TotalVersion returned by the server
    Public LatestVersionString As String        ' -> Used to store the actual version number returned by the server
    Public TotalVersionFixed As Integer = 36    ' -> For commodity, I don't use the actual version number of the application to know when there's an update. Instead I check if this number is higher.
    Public UpdaterDownloaded As Boolean = False ' -> Used when the updater file has been downloaded in this run, to avoid having to download it again

#End Region

#Region "Other"

    Public drawing As New Un4seen.Bass.Misc.Visuals ' -> Used to draw the vis
    Public RestartPlayback As Boolean               ' -> Used to know if playback should be restarted after an operation has completed (changing channels, for example)
    Public stream As Integer                        ' -> The stream that is passed to BASS so it plays it
    Public oldvol As Integer                        ' -> This stores the volume when the user clicks the Mute button; to know which volume level should be used when the user clicks the Unmute button
    Dim ServersArray As New ListView                ' -> Used to store a list of available servers for a particular channel
    ' v  The following list of channels don't have a forum board and should disable the Forums button when selected
    Dim NoForumsChannel As String = "Cosmic Downtempo;Deep Nu-Disco;Vocal Chillout;Deep House;Epic Trance;Hands Up;Club Dubstep;Progressive Psy;80's Rock Hits;Club Bollywood;Compact Discoveries;Hard Rock;Metal;Modern Blues;Modern Rock;Pop Rock;Relaxing Excursions;Ska;Smooth Lounge;Soft Rock;Glitch Hop;Deep Tech;Liquid Dubstep;Classic EuroDisco;Dark DnB;90's Hits;Mellow Jazz;Café de Paris;Christmas Channel"
    Private _mySync As SYNCPROC                     ' -> Sync so BASS says when the stream title has changed
    ' v Used to get command line arguments
    Dim CommandLineArgs As System.Collections.ObjectModel.ReadOnlyCollection(Of String) = My.Application.CommandLineArgs
    Dim TotalCommandLine As String = ""

    Dim channelKey As Integer
    Dim KeysArray As New ListView
    Dim UpdateWait As Integer = 0

    Delegate Sub MsgBoxSafe(ByVal text As String, ByVal style As MsgBoxStyle, ByVal title As String)

    Private EqBands As Integer() = {0, 0, 0, 0, 0, 0}
    Dim Eq As New BASS_DX8_PARAMEQ()

    Public WasPlaying As String

    Dim executable As String = Application.ExecutablePath
    Dim tabla() As String = Split(executable, "\")
    Public exeFolder As String = Application.ExecutablePath.Replace(tabla(tabla.Length - 1), Nothing)

    Public HotkeysSet As Boolean = False

#End Region

#End Region

#Region "Main Form events"

    Private Sub Player_DragEnter(sender As Object, e As System.Windows.Forms.DragEventArgs) Handles Me.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then

            Dim filepaths() As String = e.Data.GetData(DataFormats.FileDrop)

            If filepaths.Length = 1 And filepaths(0).ToLower.EndsWith("cth") Then
                e.Effect = DragDropEffects.Move
            Else
                e.Effect = DragDropEffects.None
            End If

        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub Player_DragDrop(sender As Object, e As System.Windows.Forms.DragEventArgs) Handles Me.DragDrop
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim filepath() As String = e.Data.GetData(DataFormats.FileDrop)
            Dim reader As New IO.StreamReader(filepath(0))
            Dim lineNumber As Integer = 0

            Do While (reader.Peek > -1)
                Dim line As String = reader.ReadLine

                If lineNumber = 0 Then
                    MainColour = line

                    If Options.Visible = True Then
                        Options.MainColour.BackColor = Color.FromArgb(line)
                    End If
                ElseIf lineNumber = 1 Then
                    SecondaryColour = line

                    If Options.Visible = True Then
                        Options.SecondaryColour.BackColor = Color.FromArgb(line)
                    End If
                ElseIf lineNumber = 2 Then
                    PeakColour = line

                    If Options.Visible = True Then
                        Options.PeakColour.BackColor = Color.FromArgb(line)
                    End If
                ElseIf lineNumber = 3 Then
                    BackgroundColour = line

                    If Options.Visible = True Then
                        Options.BackgroundColour.BackColor = Color.FromArgb(line)
                    End If
                ElseIf lineNumber = 4 Then
                    ChangeWholeBackground = line

                    If Options.Visible = True Then
                        Options.ChangeWholeBackground.Checked = line
                    End If
                End If

                lineNumber += 1
            Loop

            reader.Close()
            reader.Dispose()

            If ChangeWholeBackground = True Then

                Me.BackColor = Color.FromArgb(BackgroundColour)
                ToolStrip1.BackColor = Color.FromArgb(BackgroundColour)
                StationChooser.BackColor = Color.FromArgb(BackgroundColour)
                Label1.BackColor = Color.FromArgb(BackgroundColour)
                Label2.BackColor = Color.FromArgb(BackgroundColour)
                RadioString.BackColor = Color.FromArgb(BackgroundColour)

                If BackgroundColour < -8323328 Then
                    RadioString.ForeColor = Color.White
                    TimerString.ForeColor = Color.White
                Else
                    RadioString.ForeColor = Color.Black
                    TimerString.ForeColor = Color.Black
                End If

                If BackgroundColour < -7105537 Then
                    EditFavorites.LinkColor = Color.White
                    RefreshFavorites.LinkColor = Color.White
                Else
                    EditFavorites.LinkColor = Color.Blue
                    RefreshFavorites.LinkColor = Color.Blue
                End If

            End If
        End If
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        ' I know how to check my threads. Don't need VS babysitting me
        Control.CheckForIllegalCrossThreadCalls = False

        DownloadingMessage.SelectedIndex = 0
        DownloadingMessage.Show()

        ' Try to open a device for BASS
        If Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, Me.Handle, Nothing) = False Then

            If Bass.BASS_ErrorGetCode = 23 Then
                MsgBox("No audio devices available." & vbNewLine & "The application will close now.", MsgBoxStyle.Critical, "Critical error")
            Else
                MsgBox("The application can't start due to the error number " & Bass.BASS_ErrorGetCode, MsgBoxStyle.Critical, "Critical error")
            End If

            End

        End If

        ' Get the actual EXE path, create the servers folder and a folder for each station. Then - in case the user has just upgraded from a version
        ' prior to 1.10, erase everything in all servers folders.

        My.Computer.FileSystem.CreateDirectory(exeFolder & "\servers")
        My.Computer.FileSystem.CreateDirectory(exeFolder & "\servers\" & DIFM.Text)
        My.Computer.FileSystem.CreateDirectory(exeFolder & "\servers\" & JazzRadio.Text)
        My.Computer.FileSystem.CreateDirectory(exeFolder & "\servers\" & SKYFM.Text)
        My.Computer.FileSystem.CreateDirectory(exeFolder & "\servers\" & RockRadio.Text)

        If My.Computer.FileSystem.FileExists(exeFolder & "\servers\" & DIFM.Text & "\*.pls") Then
            Kill(exeFolder & "\servers\" & DIFM.Text & "\*.pls")
        End If

        If My.Computer.FileSystem.FileExists(exeFolder & "\servers\" & DIFM.Text & "\*.asx") Then
            Kill(exeFolder & "\servers\" & DIFM.Text & "\*.asx")
        End If

        If My.Computer.FileSystem.FileExists(exeFolder & "\servers\" & JazzRadio.Text & "\*.pls") Then
            Kill(exeFolder & "\servers\" & JazzRadio.Text & "\*.*")
        End If

        If My.Computer.FileSystem.FileExists(exeFolder & "\servers\" & JazzRadio.Text & "\*.asx") Then
            Kill(exeFolder & "\servers\" & JazzRadio.Text & "\*.*")
        End If

        If My.Computer.FileSystem.FileExists(exeFolder & "\servers\" & SKYFM.Text & "\*.pls") Then
            Kill(exeFolder & "\servers\" & SKYFM.Text & "\*.*")
        End If

        If My.Computer.FileSystem.FileExists(exeFolder & "\servers\" & SKYFM.Text & "\*.asx") Then
            Kill(exeFolder & "\servers\" & SKYFM.Text & "\*.*")
        End If

        If My.Computer.FileSystem.FileExists(exeFolder & "\servers\" & RockRadio.Text & "\*.pls") Then
            Kill(exeFolder & "\servers\" & RockRadio.Text & "\*.*")
        End If

        If My.Computer.FileSystem.FileExists(exeFolder & "\servers\" & RockRadio.Text & "\*.asx") Then
            Kill(exeFolder & "\servers\" & RockRadio.Text & "\*.*")
        End If

        ' Load plugins for WMA and AAC support
        Bass.BASS_PluginLoad("basswma.dll")
        Bass.BASS_PluginLoad("bassaac.dll")

        ' Accomodate internal version number to my own weird numbering scheme. It goes as follows:
        ' Version 1.0.0.0 is 1.0
        ' Version 1.0.1.0 is 1.1 Beta 1, 1.0.2.0 is 1.1 Beta 2 and so on
        ' Version 1.1 is 1.1
        ' Version 1.1.1.0 is 1.2 Beta 1, 1.1.2.0 is 1.2 Beta 2 and so on
        Me.Text += My.Application.Info.Version.Major.ToString & "." & My.Application.Info.Version.Minor.ToString

        If My.Application.Info.Version.Build.ToString > 0 Then
            Me.Text += " Beta " & My.Application.Info.Version.Build.ToString

            If My.Application.Info.Version.Minor.ToString = 9 Then
                Me.Text = Me.Text.Replace(My.Application.Info.Version.Major.ToString & "." & My.Application.Info.Version.Minor.ToString, My.Application.Info.Version.Major.ToString + 1 & ".0")
            Else
                Me.Text = Me.Text.Replace(My.Application.Info.Version.Major.ToString & "." & My.Application.Info.Version.Minor.ToString, My.Application.Info.Version.Major.ToString & "." & My.Application.Info.Version.Minor.ToString + 1)
            End If
        End If

        ' Load the options.ini file
        LoadOptions()

        ' Check on the command line arguments if playback should start right at startup (i.e. we just got an update!)
        Dim commandline As String = CommandLineArgs.ToString

        For Each commandline In CommandLineArgs
            TotalCommandLine += commandline
        Next

        If TotalCommandLine.Contains("*StartPlaying*") Then
            RestartPlayback = True
        End If

    End Sub

    Private Sub Form1_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        If Me.WindowState = FormWindowState.Minimized Then

            VisTimer.Stop()
            VisualisationBox.Image = Nothing

            If NotificationIcon = True Then
                TrayIcon.Visible = True
                Me.Hide()

                If MultimediaKeys = True Then

                    RegisterHotKey(Me.Handle, 1, ModifiersPlayStop, KeyPlayStop)
                    RegisterHotKey(Me.Handle, 2, ModifiersVolumeUp, KeyVolumeUp)
                    RegisterHotKey(Me.Handle, 3, ModifiersVolumeDown, KeyVolumeDown)
                    RegisterHotKey(Me.Handle, 4, ModifiersMuteUnmute, KeyMuteUnmute)

                    If ModifiersPlayStop = Nothing And KeyPlayStop = Keys.MediaPlayPause Then
                        RegisterHotKey(Me.Handle, 5, Nothing, Keys.MediaStop)
                    End If


                    If ModifiersShowHide = 0 And KeyShowHide = 0 Then
                        RegisterHotKey(Me.Handle, 6, 6, Keys.Home)
                    Else
                        RegisterHotKey(Me.Handle, 6, ModifiersShowHide, KeyShowHide)
                    End If

                    HotkeysSet = True

                End If
            End If


        Else

            If Visualisation = True And PlayStop.Tag = "Stop" Then
                VisTimer.Start()
            End If

            If HotkeysSet = True Then

                UnregisterHotKey(Me.Handle, 1)
                UnregisterHotKey(Me.Handle, 2)
                UnregisterHotKey(Me.Handle, 3)
                UnregisterHotKey(Me.Handle, 4)
                UnregisterHotKey(Me.Handle, 5)
                UnregisterHotKey(Me.Handle, 6)
                HotkeysSet = False

            End If


        End If
    End Sub

    Private Sub Form1_TextChanged(sender As Object, e As System.EventArgs) Handles Me.TextChanged

        If TrayIcon.Text.StartsWith("DI Radio") OrElse TrayIcon.Text.StartsWith("JazzRadio Radio") OrElse TrayIcon.Text.StartsWith("SKY.FM Radio") OrElse TrayIcon.Text.StartsWith("RockRadio Radio") Then
            TrayIcon.Text = Me.Text
        End If

    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        Dim file As String = exeFolder & "\options.ini"

        If FadeOut.Enabled = False Then
            Try

                Dim writer As New IO.StreamWriter(file, False)
                writer.WriteLine(Options.NotificationTitle.Name & "=" & NotificationTitle)
                writer.WriteLine(Options.PlayNewOnChannelChange.Name & "=" & PlayNewOnChannelChange)
                writer.WriteLine(Options.NotificationIcon.Name & "=" & NotificationIcon)
                writer.WriteLine(Options.NoTaskbarButton.Name & "=" & NoTaskbarButton)
                writer.WriteLine(Options.GoogleSearch.Name & "=" & GoogleSearch)
                writer.WriteLine(Options.PremiumFormats.Name & "=" & PremiumFormats)
                writer.WriteLine("DIFormat=" & DIFormat)
                writer.WriteLine("SKYFormat=" & SKYFormat)
                writer.WriteLine("JazzFormat=" & JazzFormat)
                writer.WriteLine(Options.ListenKey.Name & "=" & ListenKey)
                writer.WriteLine(Options.BetaVersions.Name & "=" & BetaVersions)
                writer.WriteLine(Options.UpdatesAtStart.Name & "=" & UpdatesAtStart)
                writer.WriteLine(Options.Visualisation.Name & "=" & Visualisation)
                writer.WriteLine(Options.VisualisationType.Name & "=" & VisualisationType)
                writer.WriteLine(Options.HighQualityVis.Name & "=" & HighQualityVis)
                writer.WriteLine(Options.LinealRepresentation.Name & "=" & LinealRepresentation)
                writer.WriteLine(Options.FullSoundRange.Name & "=" & FullSoundRange)
                writer.WriteLine(Options.Smoothness.Name & "=" & Smoothness)
                writer.WriteLine(Options.MainColour.Name & "=" & MainColour)
                writer.WriteLine(Options.SecondaryColour.Name & "=" & SecondaryColour)
                writer.WriteLine(Options.PeakColour.Name & "=" & PeakColour)
                writer.WriteLine(Options.BackgroundColour.Name & "=" & BackgroundColour)
                writer.WriteLine(Options.ChangeWholeBackground.Name & "=" & ChangeWholeBackground)
                writer.WriteLine(Options.MultimediaKeys.Name & "=" & MultimediaKeys)
                writer.WriteLine(Options.HotkeyPlayStop.Name & "=" & HumanModifiersPlayStop & "=" & ModifiersPlayStop & "=" & KeyPlayStop)
                writer.WriteLine(Options.HotkeyVolumeUp.Name & "=" & HumanModifiersVolumeUp & "=" & ModifiersVolumeUp & "=" & KeyVolumeUp)
                writer.WriteLine(Options.HotkeyVolumeDown.Name & "=" & HumanModifiersVolumeDown & "=" & ModifiersVolumeDown & "=" & KeyVolumeDown)
                writer.WriteLine(Options.HotkeyMuteUnmute.Name & "=" & HumanModifiersMuteUnmute & "=" & ModifiersMuteUnmute & "=" & KeyMuteUnmute)
                writer.WriteLine(Options.HotkeyShowHide.Name & "=" & HumanModifiersShowHide & "=" & ModifiersShowHide & "=" & KeyShowHide)
                writer.WriteLine(Options.Band0.Name & "=" & Band0)
                writer.WriteLine(Options.Band1.Name & "=" & Band1)
                writer.WriteLine(Options.Band2.Name & "=" & Band2)
                writer.WriteLine(Options.Band3.Name & "=" & Band3)
                writer.WriteLine(Options.Band4.Name & "=" & Band4)
                writer.WriteLine(Options.Band5.Name & "=" & Band5)
                writer.WriteLine(StationChooser.Name & "=" & StationChooser.Text)
                writer.WriteLine("DIChannel=" & DIChannel)
                writer.WriteLine("SkyChannel=" & SKYChannel)
                writer.WriteLine("JazzChannel=" & JazzChannel)
                writer.WriteLine("RockChannel=" & RockChannel)

                ' If volume is above 0 then save the volume, else save it as 50 so the application doesn't start muted the next time the user launches it
                If Volume.Value > 0 Then
                    writer.WriteLine(Volume.Name & "=" & Volume.Value)
                ElseIf Volume.Value = 0 Then
                    writer.WriteLine(Volume.Name & "=50")
                End If

                ' If My Favourites is selected, save the current selected favourite so that it can be selected again automatically on the next application launch
                If SelectedChannel.Text = "My Favorites" Then
                    writer.WriteLine(SelectedServer.Name & "=" & SelectedServer.Text)
                End If

                writer.Close()
                writer.Dispose()

            Catch ex As Exception

                ' If the options file couldn't be written, display an error message and cancel closing so the user tries again
                MsgBox("Your options couldn't be saved due to the following error:" & vbNewLine & ex.Message & vbNewLine & vbNewLine & "Please try closing the application again.", MsgBoxStyle.Exclamation)
                e.Cancel = True

            End Try
        End If

        ' Unregister hotkeys if they were set

        If HotkeysSet = True Then
            UnregisterHotKey(Me.Handle, 1)
            UnregisterHotKey(Me.Handle, 2)
            UnregisterHotKey(Me.Handle, 3)
            UnregisterHotKey(Me.Handle, 4)
            UnregisterHotKey(Me.Handle, 5)
            UnregisterHotKey(Me.Handle, 6)
        End If

        ' If Volume is above 0 and music is playing, cancel closing and fade out the volume. Else just go away
        If PlayStop.Tag = "Stop" And Volume.Value > 0 Then
            e.Cancel = True
            Me.Enabled = False
            TrayIcon.Visible = False
            FadeOut.Start()
        Else
            TrayIcon.Visible = False
            Bass.BASS_Free()
            Bass.BASS_PluginFree(0)
        End If
    End Sub

#End Region

#Region "Events caused by the controls on the main form"

    Private Sub RadioString_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioString.Click

        If GoogleSearch = True Then

            If RadioString.Text = Nothing = False And RadioString.Text.Contains("Connection is taking some time") = False And RadioString.Text.Contains("Lost connection to") = False And RadioString.Text.Contains("Connecting, please wait...") = False And RadioString.Text.Contains("Couldn't connect to") = False Then
                GoogleSearchToolStripMenuItem_Click(Me, Nothing)
            End If

        End If

    End Sub

    Private Sub RadioString_TextChanged(sender As Object, e As System.EventArgs) Handles RadioString.TextChanged

        If RadioString.Text = "" = False Then

            Dim raw As String = RadioString.Text.Replace("&", "&&&")

            If raw.Length >= 64 Then
                TrayIcon.Text = raw.Remove(60, raw.Length - 60) & "..."
            Else
                TrayIcon.Text = raw
            End If

            If Me.WindowState = FormWindowState.Minimized And NotificationTitle = True And _
             RadioString.Text.ToLower.Contains("photonvps.com") = False And _
             RadioString.Text.ToLower.Contains("adwtag") = False And _
             RadioString.Text.ToLower.Contains("job opportunity") = False And _
             RadioString.Text.ToLower = "get digitally imported premium" = False And _
             RadioString.Text.ToLower = "unleash the full potential of sky.fm. get sky.fm premium now!" = False And _
             RadioString.Text.ToLower = "it gets even better! jazzradio premium - www.jazzradio.com/join" = False And _
             RadioString.Text.ToLower = "more of the show after these messages" = False And _
             RadioString.Text.ToLower = "blood pumping mind twisting di radio" = False And _
             RadioString.Text.ToLower = "love thy neighbour as thyself - turn up the volume (t)" = False And _
             RadioString.Text.ToLower = "di - where the beat is always on" = False And _
             RadioString.Text.ToLower = "di - serving you non-stop trance" = False And _
             RadioString.Text.ToLower = "di - energizing" = False And _
             RadioString.Text.ToLower = "you are listening to di radio, the best of trance on the net (b)" = False And _
             RadioString.Text.ToLower = "digitally imported radio (captivating)" = False Then

                TrayIcon.BalloonTipText = RadioString.Text

                If RadioString.Text.Contains("Connecting, please wait...") Then
                    TrayIcon.BalloonTipTitle = "Info:"
                    TrayIcon.BalloonTipIcon = ToolTipIcon.Info
                ElseIf RadioString.Text.Contains("Connection is taking some time") Then
                    TrayIcon.BalloonTipTitle = "Warning:"
                    TrayIcon.BalloonTipIcon = ToolTipIcon.Warning
                ElseIf RadioString.Text.Contains("Lost connection to") OrElse RadioString.Text.Contains("Couldn't connect to") Then
                    TrayIcon.BalloonTipTitle = "Error:"
                    TrayIcon.BalloonTipIcon = ToolTipIcon.Error
                Else
                    TrayIcon.BalloonTipTitle = "Now playing:"
                    TrayIcon.BalloonTipIcon = ToolTipIcon.Info
                End If

                TrayIcon.ShowBalloonTip(1500)

            End If

        Else
            TrayIcon.Text = Me.Text
        End If

        CopyTitleMenu_Opening(Me, Nothing)
        TrayMenu_Opening(Me, Nothing)
        ToolTip.SetToolTip(RadioString, RadioString.Text)

        UpdateWait = 0

    End Sub

    Public Sub PlayStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PlayStop.Click

        If PlayStop.Tag = "Play" And SelectedServer.Items.Count > 0 Then

            RadioString.Text = "Connecting, please wait..."

            If ChangeWholeBackground = True Then
                RadioString.BackColor = Color.FromArgb(BackgroundColour)
            Else
                RadioString.BackColor = SystemColors.Control
            End If

            If BackgroundColour < -8323328 Then
                RadioString.ForeColor = Color.White
                TimerString.ForeColor = Color.White
            Else
                RadioString.ForeColor = Color.Black
                TimerString.ForeColor = Color.Black
            End If

            PlayStop.Enabled = False
            StationChooser.Enabled = False
            Bufer.RunWorkerAsync()
            TimePassed.Start()

            If Visualisation = True Then
                VisTimer.Start()
            End If

            SelectedChannel.Enabled = False
            SelectedServer.Enabled = False

        Else

            If ChangeWholeBackground = True Then
                RadioString.BackColor = Color.FromArgb(BackgroundColour)
            Else
                RadioString.BackColor = SystemColors.Control
            End If

            If BackgroundColour < -8323328 Then
                RadioString.ForeColor = Color.White
                TimerString.ForeColor = Color.White
            Else
                RadioString.ForeColor = Color.Black
                TimerString.ForeColor = Color.Black
            End If

            If Bufer.IsBusy = True Then

                RadioString.Text = "Please wait..."
                Bufer.CancelAsync()
                PlayStop.Enabled = False

            Else
                RadioString.Text = Nothing
                PlayStop.Image = My.Resources.StartPlayback
                PlayStop.Tag = "Play"
                ToolTip.SetToolTip(PlayStop, "Play")

                If SelectedServer.Items.Count < 1 Then
                    PlayStop.Enabled = False
                    SelectedServer.Enabled = False
                Else
                    SelectedServer.Enabled = True
                End If
            End If

            If ServersDownloader.IsBusy = False Then
                SelectedChannel.Enabled = True
            End If

            Bass.BASS_ChannelStop(stream)
            VisTimer.Stop()
            TimePassed.Stop()
            VisualisationBox.Image = Nothing
            TimerString.Text = "00:00"
            TrayIcon.Text = Me.Text
        End If
    End Sub

    Private Sub OptionsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OptionsButton.Click
        Dim X As Integer
        Dim Y As Integer

        ' If main form is visible, get its boundaries, make sure the Options panel won't appear out of screen
        ' and then open it and bring it to front just in case. Otherwise simply center it on screen and
        ' bring it.

        If Me.Visible = True Then

            If Me.Location.X + 21 > Screen.PrimaryScreen.WorkingArea.Size.Width Then
                X = Screen.PrimaryScreen.WorkingArea.Size.Width - OptionsButton.Size.Width
            ElseIf Me.Location.X + 21 < 0 Then
                X = 0
            Else
                X = Me.Location.X + 21
            End If


            If Visualisation = True Then
                If Me.Location.Y - 12 > Screen.PrimaryScreen.WorkingArea.Size.Height Then
                    Y = Screen.PrimaryScreen.WorkingArea.Size.Height - OptionsButton.Size.Height
                ElseIf Me.Location.Y - 12 < 0 Then
                    Y = 0
                Else
                    Y = Me.Location.Y - 4
                End If
            Else
                If Me.Location.Y - 180 > Screen.PrimaryScreen.WorkingArea.Size.Height Then
                    Y = Screen.PrimaryScreen.WorkingArea.Size.Height - OptionsButton.Size.Height
                ElseIf Me.Location.Y - 180 < 0 Then
                    Y = 0
                Else
                    Y = Me.Location.Y - 180
                End If
            End If

            Options.Location = New Point(X, Y)

        Else

            Options.StartPosition = FormStartPosition.CenterScreen

        End If

        Options.Show()
        Options.BringToFront()

    End Sub

    Private Sub Calendar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Calendar.Click
        ' Get the current channel, remove spaces and convert names to their URL counterparts if necessary. Then open DI's calendar

        Dim channel As String

        If SelectedChannel.Text = "My Favorites" Then
            channel = SelectedServer.Text.ToLower.Replace(" ", Nothing)
        Else
            channel = SelectedChannel.Text.ToLower.Replace(" ", Nothing)
        End If

        If channel = "classicelectronica" Then
            channel = "classictechno"
        ElseIf channel = "clubsounds" Then
            channel = "club"
        ElseIf channel = "deepnu-disco" Then
            channel = "deepnudisco"
        ElseIf channel = "drum'nbass" Then
            channel = "drumandbass"
        ElseIf channel = "electrohouse" Then
            channel = "electro"
        ElseIf channel = "goa-psytrance" Then
            channel = "goapsy"
        ElseIf channel = "spacedreams" Then
            channel = "spacemusic"
        End If

        Process.Start("http://www.di.fm/calendar/week/" & channel)
    End Sub

    Private Sub Forums_Click(sender As System.Object, e As System.EventArgs) Handles Forums.Click
        ' Get the current channel, remove spaces and convert names to their URL counterparts if necessary. Then open the URL
        ' of the currently-chosen radio station. SKY.FM uses numbers to identify their boards so all channels need to be
        ' changed. There has to be a better way of doing that but this is what I came up with


        Dim forum As String

        If SelectedChannel.Text = "My Favorites" Then
            forum = SelectedServer.Text.ToLower.Replace(" ", "-")
        Else
            forum = SelectedChannel.Text.ToLower.Replace(" ", "-")
        End If

        If StationChooser.Text = DIFM.Text Then

            If forum = "classic-electronica" Then
                forum = "oldschool-electronica"
            ElseIf forum = "drum-'n-bass" Then
                forum = "drum-n-bass"
            ElseIf forum = "goa-psy-trance" Then
                forum = "goa-and-psy"
            End If

        ElseIf StationChooser.Text = SKYFM.Text Then

            If forum = "smooth-jazz" Then
                forum = "forumdisplay.php?f=6"
            ElseIf forum = "new-age" Then
                forum = "forumdisplay.php?f=10"
            ElseIf forum = "best-of-the-80's" Then
                forum = "forumdisplay.php?f=5"
            ElseIf forum = "mostly-classical" Then
                forum = "forumdisplay.php?f=8"
            ElseIf forum = "hit-70's" Then
                forum = "forumdisplay.php?f=9"
            ElseIf forum = "uptempo-smooth-jazz" Then
                forum = "forumdisplay.php?f=26"
            ElseIf forum = "classic-rap" Then
                forum = "forumdisplay.php?f=24"
            ElseIf forum = "top-hits" Then
                forum = "forumdisplay.php?f=7"
            ElseIf forum = "classic-rock" Then
                forum = "forumdisplay.php?f=29"
            ElseIf forum = "world" Then
                forum = "forumdisplay.php?f=22"
            ElseIf forum = "classical-guitar" Then
                forum = "forumdisplay.php?f=20"
            ElseIf forum = "oldies" Then
                forum = "forumdisplay.php?f=23"
            ElseIf forum = "roots-reggae" Then
                forum = "forumdisplay.php?f=19"
            ElseIf forum = "country" Then
                forum = "forumdisplay.php?f=21"
            ElseIf forum = "love-music" Then
                forum = "forumdisplay.php?f=39"
            ElseIf forum = "indie-rock" Then
                forum = "forumdisplay.php?f=27"
            ElseIf forum = "solo-piano" Then
                forum = "forumdisplay.php?f=34"
            ElseIf forum = "jazz-classics" Then
                forum = "forumdisplay.php?f=11"
            ElseIf forum = "urban-jamz" Then
                forum = "forumdisplay.php?f=25"
            ElseIf forum = "alternative-rock" Then
                forum = "forumdisplay.php?f=31"
            ElseIf forum = "datempo-lounge" Then
                forum = "forumdisplay.php?f=30"
            ElseIf forum = "movie-soundtracks" Then
                forum = "forumdisplay.php?f=36"
            ElseIf forum = "salsa" Then
                forum = "forumdisplay.php?f=12"
            ElseIf forum = "contemporary-christian" Then
                forum = "forumdisplay.php?f=35"
            ElseIf forum = "a-beatles-tribute" Then
                forum = "forumdisplay.php?f=38"
            ElseIf forum = "american-songbook" Then
                forum = "forumdisplay.php?f=42"
            ElseIf forum = "classical-piano-trios" Then
                forum = "forumdisplay.php?f=43"
            ElseIf forum = "bossa-nova" Then
                forum = "forumdisplay.php?f=32"
            ElseIf forum = "piano-jazz" Then
                forum = "forumdisplay.php?f=33"
            ElseIf forum = "bebop-jazz" Then
                forum = "forumdisplay.php?f=40"
            ElseIf forum = "dance-hits" Then
                forum = "forumdisplay.php?f=41"
            ElseIf forum = "vocal-smooth-jazz" Then
                forum = "forumdisplay.php?f=53"
            ElseIf forum = "pop-punk" Then
                forum = "forumdisplay.php?f=49"
            ElseIf forum = "romantica" Then
                forum = "forumdisplay.php?f=50"
            ElseIf forum = "dreamscapes" Then
                forum = "forumdisplay.php?f=51"
            ElseIf forum = "mostly-classical" Then
                forum = "forumdisplay.php?f=8"
            ElseIf forum = "jpop" Then
                forum = "forumdisplay.php?f=52"
            ElseIf forum = "smooth-jazz-24'7" Then
                forum = "forumdisplay.php?f=57"
            ElseIf forum = "relaxation" Then
                forum = "forumdisplay.php?f=55"
            ElseIf forum = "nature" Then
                forum = "forumdisplay.php?f=54"
            ElseIf forum = "vocal-new-age" Then
                forum = "forumdisplay.php?f=56"
            End If

        End If


        Process.Start("http://forums." & StationChooser.Tag & "/" & forum)
    End Sub

    Private Sub Mute_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Mute.Click

        If Volume.Value = 0 Then

            If oldvol <= 0 Then
                Volume.Value = 50
            Else
                Volume.Value = oldvol
            End If

        Else

            oldvol = Volume.Value
            Volume.Value = 0

        End If

    End Sub

    Private Sub Volume_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Volume.ValueChanged
        Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, Volume.Value / 100)

        If Volume.Value = 0 Then
            Mute.Image = My.Resources.mutedvol
            Mute.Tag = "Unmute"
        ElseIf Volume.Value > 1 And Volume.Value < 25 Then
            Mute.Image = My.Resources.lowvol
            Mute.Tag = "Mute"
        ElseIf Volume.Value >= 25 And Volume.Value < 75 Then
            Mute.Image = My.Resources.mediumvol
            Mute.Tag = "Mute"
        ElseIf Volume.Value >= 75 And Volume.Value <= 100 Then
            Mute.Image = My.Resources.highvol
            Mute.Tag = "Mute"
        End If

        ToolTip.SetToolTip(Mute, Mute.Tag)
        ToolTip.SetToolTip(Volume, Volume.Value & "%")
    End Sub

    Private Sub EditFavorites_LinkClicked(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles EditFavorites.LinkClicked
        Process.Start("http://www." & StationChooser.Tag & "/member/favorite/channels")
    End Sub

    Private Sub RefreshFavorites_LinkClicked(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles RefreshFavorites.LinkClicked

        OldFav = SelectedServer.Text
        Dim file As String = exeFolder & "\servers\" & StationChooser.Text & "\favorites.db"

        If My.Computer.FileSystem.FileExists(file) Then
            Kill(file)
        End If

        If PlayStop.Tag = "Stop" Then
            RefreshFavorites.Enabled = False
        End If

        If SelectedChannel.Enabled = True Then
            SelectedChannel_SelectedIndexChanged(Me, Nothing)
        End If

    End Sub

    Private Sub StationChooser_ButtonClick(sender As Object, e As System.EventArgs) Handles StationChooser.ButtonClick
        If StationChooser.Tag = "di.fm" Then
            JazzRadio_Click(sender, e)
        ElseIf StationChooser.Tag = "jazzradio.com" Then
            RockRadio_Click(sender, e)
        ElseIf StationChooser.Tag = "sky.fm" Then
            DIFM_Click(sender, e)
        ElseIf StationChooser.Tag = "rockradio.com" Then
            SKYFM_Click(sender, e)
        End If
    End Sub

    Private Sub StationChooser_TextChanged(sender As Object, e As System.EventArgs) Handles StationChooser.TextChanged

        ' Long lists were removed by _Tobias. Now the player doesn't have to be updated in order to add new channels!

        PlayStop.Enabled = False
        StationChooser.Enabled = False
        SelectedChannel.Enabled = False
        SelectedServer.Enabled = False

        SelectedChannel.Items.Clear()

        If ListenKey = Nothing = False And StationChooser.Text = RockRadio.Text = False Then
            SelectedChannel.Items.Add("My Favorites")
        End If

        Calendar.Enabled = False
        History.Enabled = False
        Forums.Enabled = False
        HistoryList.Items.Clear()
        RetryChannels.Hide()
        DownloadingMessage.Show()
        Marquee.Show()

        DownloadDb.RunWorkerAsync()

    End Sub

    Public Sub SelectedChannel_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SelectedChannel.SelectedIndexChanged

        RetryChannels.Hide()
        RetryServers.Hide()

        If PlayStop.Tag = "Play" Then
            PlayStop.Enabled = False
        End If

        StationChooser.Enabled = False

        If ServersDownloader.IsBusy = False Then
            ServersDownloader.RunWorkerAsync()
        End If

        Marquee.Show()
        SelectedChannel.Enabled = False
        SelectedServer.Enabled = False

        If SelectedChannel.Text = "My Favorites" Then
            RefreshFavorites.Show()
            EditFavorites.Show()
            RefreshFavorites.Enabled = False
        Else
            EditFavorites.Hide()
            RefreshFavorites.Hide()
        End If

        If PlayStop.Tag = "Stop" And PlayNewOnChannelChange = True Then
            RestartPlayback = True
            PlayStop_Click(Me, Nothing)
        End If

        If StationChooser.Text = DIFM.Text Then
            DIChannel = SelectedChannel.SelectedIndex
        ElseIf StationChooser.Text = SKYFM.Text Then
            SKYChannel = SelectedChannel.SelectedIndex
        ElseIf StationChooser.Text = JazzRadio.Text Then
            JazzChannel = SelectedChannel.SelectedIndex
        ElseIf StationChooser.Text = RockRadio.Text Then
            RockChannel = SelectedChannel.SelectedIndex
        End If

        If GetHistory.IsBusy = False Then
            GetHistory.RunWorkerAsync()
        End If

    End Sub

    Private Sub SelectedChannel_TextChanged(sender As Object, e As System.EventArgs) Handles SelectedChannel.TextChanged
        ' Check if the Forums button should be enabled or not and exit as soon as there is a match
        ' Or always disable the button if JazzRadio or RockRadio is selected

        Dim Channels As String = NoForumsChannel
        Dim Channel() As String = Split(Channels, ";")
        Dim ChannelNumber As Integer = 0

        Do While ChannelNumber < Channel.Length
            ChannelNumber += 1
            If ChannelNumber <= Channel.Length - 1 Then

                If SelectedChannel.Text = Channel(ChannelNumber) OrElse StationChooser.Text = JazzRadio.Text OrElse StationChooser.Text = RockRadio.Text Then
                    Forums.Enabled = False
                    Exit Do
                Else
                    Forums.Enabled = True
                End If

            Else
                Exit Do
            End If
        Loop

    End Sub

    Private Sub SelectedServer_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles SelectedServer.SelectedIndexChanged

        If SelectedChannel.Text = "My Favorites" And PlayStop.Tag = "Stop" And PlayNewOnChannelChange = True Then
            RestartPlayback = True
            PlayStop_Click(Me, Nothing)
            OldFav = SelectedServer.Text
            SelectedChannel_SelectedIndexChanged(Me, Nothing)
        End If

        If SelectedChannel.Text = "My Favorites" Then
            Dim Channels As String = NoForumsChannel
            Dim Channel() As String = Split(Channels, ";")
            Dim ChannelNumber As Integer = 0

            If StationChooser.Text = JazzRadio.Text = False Then
                Do While ChannelNumber < Channel.Length
                    ChannelNumber += 1
                    If ChannelNumber <= 7 Then

                        If SelectedServer.Text = Channel(ChannelNumber) Then
                            Forums.Enabled = False
                            Exit Do
                        Else
                            Forums.Enabled = True
                        End If

                    Else
                        Exit Do
                    End If
                Loop
            End If
        End If

    End Sub

    Private Sub TimerString_TextChanged(sender As System.Object, e As System.EventArgs) Handles TimerString.TextChanged

        ' BASS reporting that -01 seconds have passed since playback has started means that connection has been droppped
        ' In that case, if the user isn't using the My Favourites playlist, automatically stop playback, select a new
        ' server and reconnect

        If TimerString.Text.EndsWith("-01") Then
            PlayStop_Click(Me, Nothing)
            Bufer.CancelAsync()

            If SelectedChannel.Text = "My Favorites" = False Then

                If SelectedServer.SelectedIndex = SelectedServer.Items.Count - 1 = False Then
                    SelectedServer.SelectedIndex = SelectedServer.SelectedIndex + 1
                Else
                    SelectedServer.SelectedIndex = 0
                End If

                PlayStop_Click(Me, Nothing)
            Else
                RadioString.BackColor = Color.Red
                RadioString.ForeColor = Color.White
                RadioString.Text = "Lost connection to channel."
            End If
        End If

    End Sub

    Private Sub HistoryList_ColumnWidthChanging(sender As Object, e As System.Windows.Forms.ColumnWidthChangingEventArgs) Handles HistoryList.ColumnWidthChanging
        e.Cancel = True
        e.NewWidth = HistoryList.Columns(e.ColumnIndex).Width
    End Sub

    Private Sub RetryChannels_Click(sender As System.Object, e As System.EventArgs) Handles RetryChannels.Click
        StationChooser_TextChanged(Me, Nothing)
        RetryChannels.Hide()
        Marquee.Show()
        StationChooser.Enabled = False
    End Sub

    Private Sub RetryServers_Click(sender As System.Object, e As System.EventArgs) Handles RetryServers.Click
        SelectedChannel_SelectedIndexChanged(Me, Nothing)
        RetryServers.Hide()
        Marquee.Show()
    End Sub

    Private Sub History_Click(sender As System.Object, e As System.EventArgs) Handles History.Click
        If HistoryList.Visible = False Then
            History.ImageAlign = ContentAlignment.BottomCenter
            History.Image = My.Resources.back
            HistoryList.Show()
            HistoryList.Items.Clear()

            If GetHistory.IsBusy = False Then
                GetHistory.RunWorkerAsync()
            End If

            VisTimer.Stop()
            Me.Size = Me.MaximumSize
            ToolTip.SetToolTip(History, "Hide track history")
        Else
            History.ImageAlign = ContentAlignment.BottomRight
            History.Image = My.Resources.history
            HistoryList.Hide()
            If Visualisation = False Then
                Me.Size = Me.MinimumSize
            Else
                VisTimer.Start()
            End If
            ToolTip.SetToolTip(History, "Show track history")
        End If
    End Sub

#End Region

#Region "Background Workers"

    Private Sub ServersDownloader_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles ServersDownloader.DoWork
        ' The code below has contributions by _Tobias from the Digitally Imported forums.

        RefreshFavorites.Enabled = False
        PlayStop.Enabled = False
        StationChooser.Enabled = False
        ServersArray.Items.Clear()
        SelectedServer.Items.Clear()

        Dim channel As String = SelectedChannel.Text
        Dim domain As String = StationChooser.Tag
        Dim serversFolder = exeFolder & "\servers\" & StationChooser.Text

        ' create servers directory if doesn't exist
        My.Computer.FileSystem.CreateDirectory(serversFolder)

        ' determine channel database
        Dim chdb = serversFolder & "\channels.db"

        If channel = "My Favorites" Then
            channel = "favorites"
        End If

        ' open file

        Try
            Dim readerChdb As IO.StreamReader = channelDb(chdb)

            ' read file
            Do While (readerChdb.Peek > -1)
                Dim line = readerChdb.ReadLine()
                Dim splitter = Split(line, "|")
                If splitter(0) = channel Then
                    channel = splitter(1)
                End If
            Loop

            readerChdb.Close()
            readerChdb.Dispose()
        Catch ex As Exception
            Dim word As String

            If SelectedChannel.Text = "My Favourites" Then
                word = "favourites"
            Else
                word = "servers"
            End If

            MessageBox.Show("Couldn't download " & word & " list.", "Error getting " & word & " list", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub
        End Try


  

        Dim file = serversFolder & "\" & channel & ".db"

        Dim EndString As String

        Dim PremiumEnd As String = ""

        If ListenKey = Nothing = False And PremiumFormats = True Then

            PremiumEnd = "?" & ListenKey

            If StationChooser.Text = DIFM.Text Then
                If DIFormat = 0 Then
                    EndString = "premium"
                ElseIf DIFormat = 1 Then
                    EndString = "premium_medium"
                ElseIf DIFormat = 2 Then
                    EndString = "premium_low"
                ElseIf DIFormat = 3 Then
                    EndString = "premium_high"
                ElseIf DIFormat = 4 Then
                    EndString = "premium"
                ElseIf DIFormat = 5 Then
                    EndString = "premium_wma"
                ElseIf DIFormat = 6 Then
                    EndString = "premium_wma_low"
                End If

            ElseIf StationChooser.Text = SKYFM.Text Then

                If SKYFormat = 0 Then
                    EndString = "premium"
                ElseIf SKYFormat = 1 Then
                    EndString = "premium_medium"
                ElseIf SKYFormat = 2 Then
                    EndString = "premium_low"
                ElseIf SKYFormat = 3 Then
                    EndString = "premium_high"
                ElseIf SKYFormat = 4 Then
                    EndString = "premium"
                ElseIf SKYFormat = 5 Then
                    EndString = "premium_wma"
                ElseIf SKYFormat = 6 Then
                    EndString = "premium_wma_low"
                End If

            ElseIf StationChooser.Text = JazzRadio.Text Then

                If JazzFormat = 0 Then
                    EndString = "premium"
                ElseIf JazzFormat = 1 Then
                    EndString = "premium_medium"
                ElseIf JazzFormat = 2 Then
                    EndString = "premium_low"
                ElseIf JazzFormat = 3 Then
                    EndString = "premium_high"
                ElseIf JazzFormat = 4 Then
                    EndString = "premium"
                ElseIf JazzFormat = 5 Then
                    EndString = "premium_wma"
                End If

            ElseIf StationChooser.Text = RockRadio.Text Then

                EndString = "public3"
                PremiumEnd = ""

            End If


        Else

            If StationChooser.Text = DIFM.Text Then

                If DIFormat = 0 Then
                    EndString = "public2"
                ElseIf DIFormat = 1 Then
                    EndString = "public3"
                ElseIf DIFormat = 2 Then
                    EndString = "public5"
                End If

            ElseIf StationChooser.Text = SKYFM.Text Then

                If SKYFormat = 0 Then
                    EndString = "public1"
                ElseIf SKYFormat = 1 Then
                    EndString = "public3"
                ElseIf SKYFormat = 2 Then
                    EndString = "public5"
                End If

            ElseIf StationChooser.Text = JazzRadio.Text Then

                If JazzFormat = 0 Then
                    EndString = "public1"
                ElseIf JazzFormat = 1 Then
                    EndString = "public3"
                ElseIf JazzFormat = 2 Then
                    EndString = "public5"
                End If

            ElseIf StationChooser.Text = RockRadio.Text Then

                EndString = "public3"

            End If


        End If

        If SelectedChannel.Text = "My Favorites" Then
            PremiumEnd = "?" & ListenKey
        End If

        If My.Computer.FileSystem.FileExists(file) = False Then
            Dim streams
            If SelectedChannel.Text = "My Favorites" Then
                streams = downloadFavos(serversFolder, ListenKey)
            Else
                streams = downloadStreams(channel, EndString, serversFolder, PremiumEnd)
            End If

            If streams.GetType.ToString.ToLower.Contains("boolean") Then
                Dim Message As New MsgBoxSafe(AddressOf DisplayMessage)

                Me.Invoke(Message, "Couldn't download servers list.", MsgBoxStyle.Exclamation, "Error getting servers list")

                Marquee.Hide()
                SelectedServer.Enabled = False

                If PlayStop.Tag = "Play" Then
                    PlayStop.Enabled = False
                End If

                StationChooser.Enabled = True
                RestartPlayback = False
                Exit Sub
            Else
                Dim writer As New IO.StreamWriter(file, False)
                Dim v
                For Each v In streams
                    writer.WriteLine(v)
                Next
                writer.Close()
                writer.Dispose()
            End If
        End If

        Dim serverno As Integer = 1

        Dim reader As New IO.StreamReader(file)

        If SelectedChannel.Text = "My Favorites" Then
            Do While (reader.Peek > -1)
                Dim name As String = reader.ReadLine
                Dim key As String

                Dim r2 As New IO.StreamReader(chdb)

                Do While (r2.Peek > -1)
                    Dim splitter = Split(r2.ReadLine, "|")
                    If splitter(0) = name Then
                        key = splitter(1)
                    End If
                Loop

                r2.Close()
                r2.Dispose()

                Dim file2 As String = serversFolder & "\" & key & ".db"

                ' get random url by name
                If My.Computer.FileSystem.FileExists(file2) = False Then

                    Dim streams2 = downloadStreams(key, EndString, serversFolder, PremiumEnd)

                    ' open file
                    Dim writer As New IO.StreamWriter(file2, False)
                    Dim entry As String

                    For Each entry In streams2
                        writer.WriteLine(entry)
                    Next

                    writer.Close()
                    writer.Dispose()

                End If

                Dim r3 As New IO.StreamReader(file2)
                Dim streams As ArrayList = New ArrayList

                Do While (r3.Peek > -1)
                    streams.Add(r3.ReadLine())
                Loop

                r3.Close()
                r3.Dispose()

                SelectedServer.Items.Add(name)
                ServersArray.Items.Add(streams(RandomNumber(streams.Count - 1, 1)))
            Loop
        Else
            Do While (reader.Peek > -1)
                Dim line As String = reader.ReadLine

                If line.Contains("mms") Then
                    ServersArray.Items.Add(line.Replace("?" & ListenKey, "?user=h&pass=" & ListenKey))
                Else
                    ServersArray.Items.Add(line)
                End If

                SelectedServer.Items.Add("Server #" & serverno)
                serverno += 1
            Loop
        End If

        reader.Close()
        reader.Dispose()

    End Sub

    Private Sub ServersDownloader_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles ServersDownloader.RunWorkerCompleted
        Marquee.Hide()
        SelectedChannel.Enabled = True

        If SelectedServer.Items.Count > 0 Then
            PlayStop.Enabled = True
            SelectedServer.SelectedIndex = 0
            SelectedServer.Enabled = True
        End If

        StationChooser.Enabled = True

        If SelectedChannel.Text = "My Favorites" And OldFav = Nothing = False Then
            Try
                SelectedServer.SelectedItem = OldFav
            Catch
            End Try
        End If


        If PlayStop.Tag = "Stop" And PlayNewOnChannelChange = True OrElse RestartPlayback = True Then
            PlayStop_Click(Me, Nothing)
            RestartPlayback = False
        End If

        If SelectedServer.Items.Count = 0 And SelectedChannel.Text = "My Favorites" Then
            SelectedServer.Items.Add("Click the link to edit list")
            SelectedServer.SelectedIndex = 0
            SelectedServer.Enabled = False
            Forums.Enabled = False
            History.Enabled = False
            HistoryList.Hide()
            History.ImageAlign = ContentAlignment.BottomRight
            History.Image = My.Resources.back
            Calendar.Enabled = False
        ElseIf StationChooser.Text = JazzRadio.Text = False And StationChooser.Text = RockRadio.Text = False Then
            History.Enabled = True

            If StationChooser.Text = SKYFM.Text = False Then
                Calendar.Enabled = True
            End If
        End If

        If Bufer.IsBusy = False Then
            RefreshFavorites.Enabled = True
        End If

        DownloadingMessage.Hide()

        If SelectedServer.Items.Count < 1 Then
            RetryServers.Show()
        End If

    End Sub

    Private Sub Bufer_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles Bufer.DoWork
        RefreshFavorites.Enabled = False
again:
        If Bufer.CancellationPending = False Then
            stream = Bass.BASS_StreamCreateURL(ServersArray.Items.Item(SelectedServer.SelectedIndex).Text, 2, BASSFlag.BASS_STREAM_AUTOFREE Or BASSFlag.BASS_STREAM_PRESCAN, Nothing, Nothing)

            Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, Volume.Value / 100)

            If Bass.BASS_ChannelPlay(stream, False) = False Then

                If SelectedServer.SelectedIndex + 1 > SelectedServer.Items.Count - 1 Then
                    RadioString.BackColor = Color.Red
                    RadioString.ForeColor = Color.White
                    RadioString.Text = "Couldn't connect to channel " & SelectedChannel.Text & "."
                    SelectedServer.Enabled = True
                    PlayStop.Image = My.Resources.StartPlayback
                    PlayStop.Tag = "Play"
                    ToolTip.SetToolTip(PlayStop, "Play")
                Else

                    If Bufer.CancellationPending = False And SelectedServer.Text = "My Favorites" = False Then
                        RadioString.BackColor = Color.Yellow
                        RadioString.ForeColor = Color.Black
                        RadioString.Text = "Connection is taking some time, please wait..."
                        SelectedServer.SelectedIndex = SelectedServer.SelectedIndex + 1
                        PlayStop.Image = My.Resources.StopPlayback
                        PlayStop.Tag = "Stop"
                        ToolTip.SetToolTip(PlayStop, "Stop")
                        PlayStop.Enabled = True
                        StationChooser.Enabled = True
                        GoTo again
                    Else
                        PlayStop.Image = My.Resources.StartPlayback
                        PlayStop.Tag = "Play"
                        ToolTip.SetToolTip(PlayStop, "Play")
                        RadioString.BackColor = Color.Red
                        RadioString.ForeColor = Color.White
                        RadioString.Text = "Couldn't connect to channel " & SelectedChannel.Text & "."
                        e.Cancel = True
                    End If

                End If

            Else

                Try
                    RadioString.Text = Split(String.Concat(Bass.BASS_ChannelGetTagsMETA(stream)), "='")(1).Replace("';", Nothing).Replace("StreamUrl", Nothing)
                Catch ex As Exception
                    RadioString.Text = "Now playing"
                End Try


                If ChangeWholeBackground = True Then
                    RadioString.BackColor = Color.FromArgb(BackgroundColour)
                Else
                    RadioString.BackColor = SystemColors.Control
                End If

                If BackgroundColour < -8323328 Then
                    RadioString.ForeColor = Color.White
                    TimerString.ForeColor = Color.White
                Else
                    RadioString.ForeColor = Color.Black
                    TimerString.ForeColor = Color.Black
                End If

                _mySync = New SYNCPROC(AddressOf MetaSync)
                Bass.BASS_ChannelSetSync(stream, BASSSync.BASS_SYNC_META, 0, _mySync, IntPtr.Zero)

                SetUpEq()
                UpdateEQ(0, Band0)
                UpdateEQ(1, Band1)
                UpdateEQ(2, Band2)
                UpdateEQ(3, Band3)
                UpdateEQ(4, Band4)
                UpdateEQ(5, Band5)

                PlayStop.Image = My.Resources.StopPlayback
                PlayStop.Tag = "Stop"
                ToolTip.SetToolTip(PlayStop, "Stop")
            End If
        Else

            PlayStop.Enabled = True
            StationChooser.Enabled = True
            If SelectedServer.Items.Count > 0 Then
                SelectedServer.Enabled = True
            End If
            e.Cancel = True

        End If

        PlayStop.Enabled = True
        StationChooser.Enabled = True

        If SelectedChannel.Text = "My Favorites" = False Then
            SelectedChannel.Enabled = True
        Else
            SelectedServer.Enabled = True
            SelectedChannel.Enabled = False
        End If

        TrayMenu_Opening(Me, Nothing)
        RefreshFavorites.Enabled = True
    End Sub

    Private Sub GetUpdates_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles GetUpdates.DoWork

        Dim WebClient As Net.WebClient = New Net.WebClient()
        Dim file As String = exeFolder & "\Info.txt"
        Dim DownloadedString As String

        Try
            DownloadedString = WebClient.DownloadString("http://www.tobiass.eu/api/update")
        Catch
            Exit Sub
        End Try

        Dim writer As New IO.StreamWriter(file, False)
        writer.Write(DownloadedString)
        writer.Close()
        writer.Dispose()
        WebClient.Dispose()

        Dim reader As New IO.StreamReader(file)

        Do While (reader.Peek > -1)
            Dim whole As String = reader.ReadLine
            Dim splitter() As String = Split(whole, "=")

            If splitter(0) = "TotalVersion" Then

                TotalVersionString = splitter(1)

            ElseIf splitter(0) = "DI Radio.exe" Then

                LatestVersionString = splitter(1)

            End If

        Loop

        reader.Close()
        reader.Dispose()

    End Sub

    Private Sub GetUpdates_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles GetUpdates.RunWorkerCompleted
        Dim updating As Boolean = False
        If My.Computer.FileSystem.FileExists(exeFolder & "\Info.txt") Then
            Dim FullLine As String = LatestVersionString
            Dim Splitter As String() = Split(FullLine, ".")

            If TotalVersionString > TotalVersionFixed Then

                Options.LookNow.Text = "Download update"
                Options.LatestVersion.ForeColor = Color.Green

                If AtStartup = True Then

                    If Splitter(2) > 0 And BetaVersions = True Then
                        If MsgBox("There's a new beta version available!" & vbNewLine & "Download now?", MsgBoxStyle.YesNo, "Update available") = MsgBoxResult.Yes Then
                            Options.LookNow_Click(Me, Nothing)
                            OptionsButton_Click(Me, Nothing)
                            updating = True
                        End If
                    ElseIf Splitter(2) = 0 Then
                        If MsgBox("There's a new version available!" & vbNewLine & "Download now?", MsgBoxStyle.YesNo, "Update available") = MsgBoxResult.Yes Then
                            Options.LookNow_Click(Me, Nothing)
                            OptionsButton_Click(Me, Nothing)
                            updating = True
                        End If
                    End If


                End If
            End If

            If Splitter(2) > 0 Then

                If Splitter(1) = 9 Then
                    Options.LatestVersion.Text = "Latest version: " & Splitter(0) + 1 & ".0" & " Beta " & Splitter(2)
                Else
                    Options.LatestVersion.Text = "Latest version: " & Splitter(0) & "." & Splitter(1) + 1 & " Beta " & Splitter(2)
                End If

            Else

                Options.LatestVersion.Text = "Latest version: " & Splitter(0) & "." & Splitter(1)

            End If



            AtStartup = False

            Kill(exeFolder & "\Info.txt")
        End If



        Options.UndefinedProgress.Hide()
        Options.Status.Text = "Status: Idle"
        If updating = False Then
            Options.LookNow.Enabled = True
        End If

    End Sub

    Private Sub GetHistory_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles GetHistory.DoWork
        HistoryList.Enabled = False
        Dim WebClient As Net.WebClient = New Net.WebClient()
        Dim HistoryLog As String
        Dim file As String = exeFolder & "\servers\historytemp"


        Try
            Dim writer As New IO.StreamWriter(file)
            HistoryLog = WebClient.DownloadString("http://tobiass.eu/api/history/text/" & KeysArray.Items.Item(SelectedChannel.Text).Tag)

            writer.Write(HistoryLog)
            writer.Close()
            writer.Dispose()

            Dim reader As New IO.StreamReader(file)

            HistoryList.Items.Clear()

            Do While (reader.Peek > -1)

                Dim line As String = reader.ReadLine
                Dim splitter() As String = Split(line, "|")

                HistoryList.Items.Add(splitter(0))

                Dim span As TimeSpan = TimeSpan.FromSeconds(splitter(1))

                If span.Hours < 1 Then
                    HistoryList.Items.Item(HistoryList.Items.Count - 1).SubItems.Add(String.Format("{0:00}:{1:00}", span.Minutes, span.Seconds))

                Else
                    HistoryList.Items.Item(HistoryList.Items.Count - 1).SubItems.Add(String.Format("{0:00}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds))
                End If

            Loop

            reader.Close()
            reader.Dispose()

            Kill(exeFolder & "\servers\historytemp")
            HistoryList.Enabled = True
        Catch
            HistoryList.Items.Clear()
            HistoryList.Items.Add("Couldn't download history information.")
            HistoryList.Items.Add("Please go back and try again.")
        End Try
    End Sub

    ' The following code thanks to _Tobias from the Digitally Imported forums.

    Private Sub DownloadDb_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles DownloadDb.DoWork

         Dim chdb = exeFolder & "\servers\" & StationChooser.Text & "\channels.db"

        Try

            Dim readerChdb As IO.StreamReader = channelDb(chdb)

            Do While (readerChdb.Peek > -1)
                Dim line = readerChdb.ReadLine()
                Dim splitter = Split(line, "|")
                SelectedChannel.Items.Add(splitter(0))
                KeysArray.Items.Add(splitter(0))
                KeysArray.Items.Item(KeysArray.Items.Count - 1).Tag = splitter(2)
                KeysArray.Items.Item(KeysArray.Items.Count - 1).Name = splitter(0)
            Loop

            readerChdb.Close()
            readerChdb.Dispose()

        Catch ex As Exception
            MessageBox.Show("Couldn't download channels list", "Error downloading channels list", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Marquee.Hide()
        End Try

    End Sub

    Private Sub DownloadDb_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles DownloadDb.RunWorkerCompleted
        If My.Computer.FileSystem.FileExists(exeFolder & "\servers\" & StationChooser.Text & "\channels.db") And SelectedChannel.Items.Count > 1 Then
            If StationChooser.Text = DIFM.Text Then

                SelectedChannel.SelectedIndex = DIChannel
                Calendar.Enabled = True
                History.Enabled = True

            ElseIf StationChooser.Text = SKYFM.Text Then

                SelectedChannel.SelectedIndex = SKYChannel
                Calendar.Enabled = False
                History.Enabled = True

            ElseIf StationChooser.Text = JazzRadio.Text Then

                SelectedChannel.SelectedIndex = JazzChannel
                Calendar.Enabled = False
                History.Enabled = False
                HistoryList.Visible = False
                History.ImageAlign = ContentAlignment.BottomRight
                History.Image = My.Resources.history
                Forums.Enabled = False

            ElseIf StationChooser.Text = RockRadio.Text Then

                SelectedChannel.SelectedIndex = RockChannel
                Calendar.Enabled = False
                History.Enabled = False
                HistoryList.Visible = False
                History.ImageAlign = ContentAlignment.BottomRight
                History.Image = My.Resources.history
                Forums.Enabled = False

            End If
        Else
            RetryChannels.Show()
        End If
        

    End Sub

#End Region

#Region "Timers"

    Private Sub FadeOut_Tick(sender As System.Object, e As System.EventArgs) Handles FadeOut.Tick

        If Volume.Value - 2 > 0 Then
            Volume.Value -= 2
        Else
            Volume.Value = 0
            Me.Close()
        End If

    End Sub

    Private Sub VisTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles VisTimer.Tick
        VisTimer.Interval = Smoothness

        If Visualisation = True And Me.WindowState = FormWindowState.Normal Then

            Dim SpectrumImage As Image
            SpectrumImage = New Bitmap(VisualisationBox.Width, VisualisationBox.Height)
            Dim SpectrumRectangle As New Rectangle(VisualisationBox.Location.X, VisualisationBox.Location.Y, VisualisationBox.Width, VisualisationBox.Height)
            Using g As Graphics = Graphics.FromImage(SpectrumImage)

                Try
                    VisualisationBox.Image.Dispose()
                Catch
                End Try


                If VisualisationType = 0 Then
                    drawing.CreateSpectrumBean(stream, g, SpectrumRectangle, Color.FromArgb(MainColour), Color.FromArgb(SecondaryColour), Color.FromArgb(BackgroundColour), 5, LinealRepresentation, FullSoundRange, HighQualityVis)
                    VisualisationBox.Image = SpectrumImage
                ElseIf VisualisationType = 1 Then
                    drawing.CreateSpectrumDot(stream, g, SpectrumRectangle, Color.FromArgb(MainColour), Color.FromArgb(SecondaryColour), Color.FromArgb(BackgroundColour), 5, 1, LinealRepresentation, FullSoundRange, False)
                    VisualisationBox.Image = SpectrumImage
                ElseIf VisualisationType = 2 Then
                    drawing.CreateSpectrum(stream, g, SpectrumRectangle, Color.FromArgb(MainColour), Color.FromArgb(SecondaryColour), Color.FromArgb(BackgroundColour), LinealRepresentation, FullSoundRange, False)
                    VisualisationBox.Image = SpectrumImage
                ElseIf VisualisationType = 3 Then
                    drawing.CreateSpectrumEllipse(stream, g, SpectrumRectangle, Color.FromArgb(MainColour), Color.FromArgb(SecondaryColour), Color.FromArgb(BackgroundColour), 3, 1, LinealRepresentation, FullSoundRange, HighQualityVis)
                    VisualisationBox.Image = SpectrumImage
                ElseIf VisualisationType = 4 Then
                    drawing.CreateSpectrumLine(stream, g, SpectrumRectangle, Color.FromArgb(MainColour), Color.FromArgb(SecondaryColour), Color.FromArgb(BackgroundColour), 5, 1, LinealRepresentation, FullSoundRange, False)
                    VisualisationBox.Image = SpectrumImage
                ElseIf VisualisationType = 5 Then
                    drawing.CreateSpectrumLinePeak(stream, g, SpectrumRectangle, Color.FromArgb(MainColour), Color.FromArgb(SecondaryColour), Color.FromArgb(PeakColour), Color.FromArgb(BackgroundColour), 5, 5, 1, 100, LinealRepresentation, FullSoundRange, False)
                    VisualisationBox.Image = SpectrumImage
                ElseIf VisualisationType = 6 Then
                    drawing.CreateSpectrumWave(stream, g, SpectrumRectangle, Color.FromArgb(MainColour), Color.FromArgb(SecondaryColour), Color.FromArgb(BackgroundColour), 5, LinealRepresentation, FullSoundRange, HighQualityVis)
                    VisualisationBox.Image = SpectrumImage
                ElseIf VisualisationType = 7 Then
                    drawing.CreateWaveForm(stream, g, SpectrumRectangle, Color.FromArgb(MainColour), Color.FromArgb(SecondaryColour), Color.FromArgb(BackgroundColour), Color.FromArgb(BackgroundColour), 5, FullSoundRange, False, HighQualityVis)
                    VisualisationBox.Image = SpectrumImage
                End If
            End Using

        Else
            VisTimer.Stop()
        End If

    End Sub

    Private Sub TimePassed_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimePassed.Tick

        If RadioString.Text.ToLower = "connecting, please wait..." = False And RadioString.Text.ToLower = "connection is taking some time, please wait..." = False And RadioString.Text.ToLower.StartsWith("couldn't connect to channel ") = False Then
            Dim pos As Long = Bass.BASS_ChannelGetPosition(stream)
            Dim elapsedtime As Double = Bass.BASS_ChannelBytes2Seconds(stream, pos)
            Dim span As TimeSpan = TimeSpan.FromSeconds(elapsedtime)

            If span.Hours < 1 Then
                RadioString.Size = New Size(300, 14)
                TimerString.Size = New Size(34, 13)
                TimerString.Location = New Point(296, 0)
                TimerString.Text = String.Format("{0:00}:{1:00}", span.Minutes, span.Seconds)
            Else
                RadioString.Size = New Size(285, 14)
                TimerString.Size = New Size(49, 13)
                TimerString.Location = New Point(282, 0)
                TimerString.Text = String.Format("{0:00}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds)
            End If

            UpdateWait += 1

            If HistoryList.Items.Count > 0 Then
                If UpdateWait = 3 And GetHistory.IsBusy = False And HistoryList.Items.Item(0).Text = RadioString.Text = False And HistoryList.Visible = True Then
                    GetHistory.RunWorkerAsync()
                End If
            End If

        End If
    End Sub

#End Region

#Region "Context menus and other events"

    Private Sub TrayMenu_Opening(sender As System.Object, e As System.ComponentModel.CancelEventArgs) Handles TrayMenu.Opening
        PlayStopTray.Enabled = PlayStop.Enabled
        PlayStopTray.Text = PlayStop.Tag
        PlayStopTray.Image = PlayStop.Image
        CalendarTray.Enabled = Calendar.Enabled
        TrackHistoryTray.Enabled = History.Enabled
        ForumsTray.Enabled = Forums.Enabled

        MuteTray.Image = Mute.Image
        MuteTray.Text = Mute.Tag

        If RadioString.Text = "" OrElse RadioString.Text.Contains("Connection is taking some time") = True OrElse RadioString.Text.Contains("Lost connection to") = True OrElse RadioString.Text.Contains("Connecting, please wait...") = True OrElse RadioString.Text.Contains("Couldn't connect to") = True Then
            CopyTitleTray.Enabled = False
            GoogleSearchTray.Enabled = False
        Else
            CopyTitleTray.Enabled = True
            GoogleSearchTray.Enabled = True
        End If
    End Sub

    Private Sub TrayIcon_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles TrayIcon.MouseClick

        If e.Button.ToString = "Left" Then
            If NoTaskbarButton = True And Me.WindowState = FormWindowState.Normal Then
                Me.TopMost = True
                Me.TopMost = False
            End If
        End If

    End Sub

    Public Sub TrayIcon_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles TrayIcon.MouseDoubleClick

        If NoTaskbarButton = False Then
            Me.Show()
            Me.WindowState = FormWindowState.Normal
            TrayIcon.Visible = False
        Else
            If Me.WindowState = FormWindowState.Minimized Then
                Me.Show()
                Me.WindowState = FormWindowState.Normal
            Else
                Me.WindowState = FormWindowState.Minimized
                Form1_Resize(Me, Nothing)
            End If
        End If

    End Sub

    Private Sub CopyTitleMenu_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles CopyTitleMenu.Opening
        If RadioString.Text = "" OrElse RadioString.Text.Contains("Connection is taking some time") = True OrElse RadioString.Text.Contains("Lost connection to") = True OrElse RadioString.Text.Contains("Connecting, please wait...") = True OrElse RadioString.Text.Contains("Couldn't connect to") = True Then
            CopyToolStripMenuItem.Enabled = False
            GoogleSearchToolStripMenuItem.Enabled = False
        Else
            CopyToolStripMenuItem.Enabled = True
            GoogleSearchToolStripMenuItem.Enabled = True
        End If
    End Sub

    Private Sub ExitTray_Click(sender As System.Object, e As System.EventArgs) Handles ExitTray.Click
        Me.Close()
    End Sub

    Private Sub CopyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyToolStripMenuItem.Click
        Clipboard.Clear()
        Clipboard.SetDataObject(RadioString.Text)
    End Sub

    Private Sub TrackHistoryTray_Click(sender As System.Object, e As System.EventArgs) Handles TrackHistoryTray.Click
        ' Get the current channel, remove spaces and convert names to their URL counterparts if necessary. Then open the URL
        ' of the currently-chosen radio station


        Dim channel As String

        If SelectedChannel.Text = "My Favorites" Then
            channel = SelectedServer.Text.ToLower.Replace(" ", Nothing)
        Else
            channel = SelectedChannel.Text.ToLower.Replace(" ", Nothing)
        End If

        If StationChooser.Text = DIFM.Text Then

            If channel = "classicelectronica" Then
                channel = "classictechno"
            ElseIf channel = "clubsounds" Then
                channel = "club"
            ElseIf channel = "deepnu-disco" Then
                channel = "deepnudisco"
            ElseIf channel = "drum'nbass" Then
                channel = "drumandbass"
            ElseIf channel = "electrohouse" Then
                channel = "electro"
            ElseIf channel = "goa-psytrance" Then
                channel = "goapsy"
            ElseIf channel = "spacedreams" Then
                channel = "spacemusic"
            End If

        ElseIf StationChooser.Text = SKYFM.Text Then

            If channel = "bestofthe80's" Then
                channel = "the80s"
            ElseIf channel = "80'srockhits" Then
                channel = "80srock"
            ElseIf channel = "smoothjazz24'7" Then
                channel = "smoothjazz247"
            ElseIf channel = "moviesoundtracks" Then
                channel = "soundtracks"
            ElseIf channel = "hit70's" Then
                channel = "hit70s"
            ElseIf channel = "mostlyclassical" Then
                channel = "classical"
            ElseIf channel = "classicalguitar" Then
                channel = "guitar"
            ElseIf channel = "alternativerock" Then
                channel = "altrock"
            ElseIf channel = "bebopjazz" Then
                channel = "bebop"
            ElseIf channel = "abeatlestribute" Then
                channel = "beatles"
            ElseIf channel = "contemporarychristian" Then
                channel = "christian"
            End If

        End If


        Process.Start("http://www." & StationChooser.Tag & "/" & channel)
    End Sub

    Private Sub CopyHistoryMenu_Opening(sender As System.Object, e As System.ComponentModel.CancelEventArgs) Handles CopyHistoryMenu.Opening
        If HistoryList.SelectedItems.Count > 0 Then
            CopyHistory.Enabled = True
            GoogleHistory.Enabled = True
        Else
            CopyHistory.Enabled = False
            GoogleHistory.Enabled = False
        End If
    End Sub

    Private Sub CopyHistory_Click(sender As System.Object, e As System.EventArgs) Handles CopyHistory.Click
        Clipboard.Clear()
        Clipboard.SetDataObject(HistoryList.SelectedItems.Item(0).Text)
    End Sub

    Private Sub GoogleHistory_Click(sender As System.Object, e As System.EventArgs) Handles GoogleHistory.Click
        Process.Start("https://www.google.com/search?q=" & HistoryList.SelectedItems.Item(0).Text.Replace("&", "%26"))
    End Sub

    ' These only call to the event function of their respective main form button counterparts

    Private Sub PlayStopTray_Click(sender As System.Object, e As System.EventArgs) Handles PlayStopTray.Click
        PlayStop_Click(sender, e)
    End Sub

    Private Sub MuteTray_Click(sender As System.Object, e As System.EventArgs) Handles MuteTray.Click
        Mute_Click(sender, e)
    End Sub

    Private Sub CopyTitleTray_Click(sender As System.Object, e As System.EventArgs) Handles CopyTitleTray.Click
        CopyToolStripMenuItem_Click(sender, e)
    End Sub

    Private Sub CalendarTray_Click(sender As System.Object, e As System.EventArgs) Handles CalendarTray.Click
        Calendar_Click(sender, e)
    End Sub

    Private Sub ForumsTray_Click(sender As System.Object, e As System.EventArgs) Handles ForumsTray.Click
        Forums_Click(sender, e)
    End Sub

    Private Sub OptionsTray_Click(sender As System.Object, e As System.EventArgs) Handles OptionsTray.Click
        OptionsButton_Click(sender, e)
    End Sub

    Private Sub GoogleSearchTray_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GoogleSearchTray.Click
        GoogleSearchToolStripMenuItem_Click(Me, Nothing)
    End Sub

    ' ----------------------------------------------------------------------------------------

    Private Sub GoogleSearchToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GoogleSearchToolStripMenuItem.Click
        Process.Start("https://www.google.com/search?q=" & RadioString.Text.Replace("&", "%26"))
    End Sub

    Private Sub DIFM_Click(sender As System.Object, e As System.EventArgs) Handles DIFM.Click
        StationChooser.Image = DIFM.Image
        StationChooser.Tag = "di.fm"
        StationChooser.Text = DIFM.Text
        StationChooser.ToolTipText = DIFM.Text
        Me.Text = Me.Text.Replace("JazzRadio", "DI")
        Me.Text = Me.Text.Replace("SKY.FM", "DI")
        Me.Text = Me.Text.Replace("RockRadio", "DI")
    End Sub

    Private Sub JazzRadio_Click(sender As System.Object, e As System.EventArgs) Handles JazzRadio.Click
        StationChooser.Image = JazzRadio.Image
        StationChooser.Tag = "jazzradio.com"
        StationChooser.Text = JazzRadio.Text
        StationChooser.ToolTipText = JazzRadio.Text
        Me.Text = Me.Text.Replace("DI", "JazzRadio")
        Me.Text = Me.Text.Replace("SKY.FM", "JazzRadio")
        Me.Text = Me.Text.Replace("RockRadio", "JazzRadio")
    End Sub

    Private Sub SKYFM_Click(sender As System.Object, e As System.EventArgs) Handles SKYFM.Click
        StationChooser.Image = SKYFM.Image
        StationChooser.Tag = "sky.fm"
        StationChooser.Text = SKYFM.Text
        StationChooser.ToolTipText = SKYFM.Text
        Me.Text = Me.Text.Replace("DI", "SKY.FM")
        Me.Text = Me.Text.Replace("JazzRadio", "SKY.FM")
        Me.Text = Me.Text.Replace("RockRadio", "SKY.FM")
    End Sub

    Private Sub RockRadio_Click(sender As System.Object, e As System.EventArgs) Handles RockRadio.Click
        StationChooser.Image = RockRadio.Image
        StationChooser.Tag = "rockradio.com"
        StationChooser.Text = RockRadio.Text
        StationChooser.ToolTipText = RockRadio.Text
        Me.Text = Me.Text.Replace("JazzRadio", "RockRadio")
        Me.Text = Me.Text.Replace("SKY.FM", "RockRadio")
        Me.Text = Me.Text.Replace("DI", "RockRadio")
    End Sub

#End Region

#Region "Other functions"

    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        If m.Msg = WM_HOTKEY Then
            Dim id As IntPtr = m.WParam

            If id = 1 OrElse id = 5 Then

                If PlayStop.Enabled = True Then
                    PlayStop_Click(Me, Nothing)
                End If

                TrayMenu_Opening(Me, Nothing)
            ElseIf id = 2 Then
                If Volume.Value < Volume.Maximum And Volume.Value + 5 < Volume.Maximum Then
                    Volume.Value += 5
                Else
                    Volume.Value = Volume.Maximum
                End If
            ElseIf id = 3 Then
                If Volume.Value > Volume.Minimum And Volume.Value - 5 > Volume.Minimum Then
                    Volume.Value -= 5
                Else
                    Volume.Value = Volume.Minimum
                End If
                TrayMenu_Opening(Me, Nothing)
            ElseIf id = 4 Then
                Mute_Click(Me, Nothing)
                TrayMenu_Opening(Me, Nothing)
            ElseIf id = 6 Then
                TrayIcon_MouseDoubleClick(Me, Nothing)
            End If

        End If

        MyBase.WndProc(m)
    End Sub

    Private Sub MetaSync(ByVal handle As Integer, ByVal channel As Integer, ByVal data As Integer, ByVal user As IntPtr)

        Dim tags() As String = Bass.BASS_ChannelGetTagsMETA(channel)
        Dim tag As String

        For Each tag In tags
            RadioString.Text = Split(String.Concat(Bass.BASS_ChannelGetTagsMETA(stream)), "='")(1).Replace("';", Nothing).Replace("StreamUrl", Nothing)

            If ChangeWholeBackground = True Then
                RadioString.BackColor = Color.FromArgb(BackgroundColour)
            Else
                RadioString.BackColor = SystemColors.Control
            End If

            If BackgroundColour < -8323328 Then
                RadioString.ForeColor = Color.White
                TimerString.ForeColor = Color.White
            Else
                RadioString.ForeColor = Color.Black
                TimerString.ForeColor = Color.Black
            End If
        Next

    End Sub

    Public Sub LoadOptions()

        Dim file As String = exeFolder & "\options.ini"

        ' If the options file doesn't exist, create it with some default values

        If My.Computer.FileSystem.FileExists(file) = False Then
            Dim writer As New IO.StreamWriter(file)
            writer.WriteLine(Options.NotificationTitle.Name & "=True")
            writer.WriteLine(Options.PlayNewOnChannelChange.Name & "=False")
            writer.WriteLine(Options.NotificationIcon.Name & "=True")
            writer.WriteLine(Options.NoTaskbarButton.Name & "=False")
            writer.WriteLine(Options.GoogleSearch.Name & "=True")
            writer.WriteLine(Options.ListenKey.Name & "=")
            writer.WriteLine(Options.PremiumFormats.Name & "=False")
            writer.WriteLine("DIFormat=1")
            writer.WriteLine("SKYFormat=1")
            writer.WriteLine("JazzFormat=1")
            writer.WriteLine(Options.BetaVersions.Name & "=False")
            writer.WriteLine(Options.UpdatesAtStart.Name & "=True")
            writer.WriteLine(Options.Visualisation.Name & "=True")
            writer.WriteLine(Options.VisualisationType.Name & "=5")
            writer.WriteLine(Options.HighQualityVis.Name & "=False")
            writer.WriteLine(Options.LinealRepresentation.Name & "=False")
            writer.WriteLine(Options.FullSoundRange.Name & "=False")
            writer.WriteLine(Options.Smoothness.Name & "=27")
            writer.WriteLine(Options.MainColour.Name & "=" & SystemColors.Control.ToArgb())
            writer.WriteLine(Options.SecondaryColour.Name & "=" & Color.Black.ToArgb())
            writer.WriteLine(Options.PeakColour.Name & "=" & Color.Silver.ToArgb())
            writer.WriteLine(Options.BackgroundColour.Name & "=" & SystemColors.Control.ToArgb())
            writer.WriteLine(Options.ChangeWholeBackground.Name & "=False")
            writer.WriteLine(Options.MultimediaKeys.Name & "=True")
            writer.WriteLine(Options.HotkeyPlayStop.Name & "=0=0=" & Keys.MediaPlayPause)
            writer.WriteLine(Options.HotkeyVolumeUp.Name & "=0=0=" & Keys.VolumeUp)
            writer.WriteLine(Options.HotkeyVolumeDown.Name & "=0=0=" & Keys.VolumeDown)
            writer.WriteLine(Options.HotkeyMuteUnmute.Name & "=0=0=" & Keys.VolumeMute)
            writer.WriteLine(Options.HotkeyShowHide.Name & "=" & Keys.Control + Keys.Shift & "=6=" & Keys.Home)
            writer.WriteLine(Options.Band0.Name & "=0")
            writer.WriteLine(Options.Band1.Name & "=0")
            writer.WriteLine(Options.Band2.Name & "=0")
            writer.WriteLine(Options.Band3.Name & "=0")
            writer.WriteLine(Options.Band4.Name & "=0")
            writer.WriteLine(Options.Band5.Name & "=0")
            writer.WriteLine("DIChannel=0")
            writer.WriteLine("SkyChannel=0")
            writer.WriteLine("JazzChannel=0")
            writer.WriteLine("RockChannel=0")
            writer.WriteLine(Volume.Name & "=50")
            writer.WriteLine(SelectedServer.Name & "=0")
            writer.Close()
            writer.Dispose()
        End If

        Try
            Dim reader As New IO.StreamReader(file)

            Do While (reader.Peek > -1)
                Dim whole As String = reader.ReadLine
                Dim splitter() As String = Split(whole, "=")

                If splitter(0) = Options.NotificationTitle.Name Then
                    NotificationTitle = splitter(1)
                ElseIf splitter(0) = Options.PlayNewOnChannelChange.Name Then
                    PlayNewOnChannelChange = splitter(1)
                ElseIf splitter(0) = Options.NotificationIcon.Name Then
                    NotificationIcon = splitter(1)
                ElseIf splitter(0) = Options.NoTaskbarButton.Name Then
                    NoTaskbarButton = splitter(1)

                    If NoTaskbarButton = False Then
                        Me.ShowInTaskbar = True

                        If Me.WindowState = FormWindowState.Normal Then
                            TrayIcon.Visible = False
                        End If
                    Else
                        Me.ShowInTaskbar = False
                        TrayIcon.Visible = True
                    End If
                ElseIf splitter(0) = Options.GoogleSearch.Name Then
                    GoogleSearch = splitter(1)
                ElseIf splitter(0) = Options.PremiumFormats.Name Then
                    PremiumFormats = splitter(1)
                ElseIf splitter(0) = "DIFormat" Then

                    If splitter(1) > 2 And PremiumFormats = True Then
                        DIFormat = splitter(1)
                    ElseIf splitter(1) <= 2 And PremiumFormats = False Then
                        DIFormat = splitter(1)
                    Else
                        DIFormat = 0
                    End If


                ElseIf splitter(0) = "SKYFormat" Then

                    If splitter(1) > 2 And PremiumFormats = True Then
                        SKYFormat = splitter(1)
                    ElseIf splitter(1) <= 2 And PremiumFormats = False Then
                        SKYFormat = splitter(1)
                    Else
                        SKYFormat = 0
                    End If


                ElseIf splitter(0) = "JazzFormat" Then

                    If splitter(1) > 2 And PremiumFormats = True Then
                        JazzFormat = splitter(1)
                    ElseIf splitter(1) <= 2 And PremiumFormats = False Then
                        JazzFormat = splitter(1)
                    Else
                        JazzFormat = 0
                    End If


                ElseIf splitter(0) = Options.ListenKey.Name OrElse splitter(0) = "PremiumKey" Then

                    ListenKey = splitter(1)

                    If ListenKey = Nothing = False Then
                        SelectedChannel.Items.Add("My Favorites")
                    End If
                ElseIf splitter(0) = Options.BetaVersions.Name Then
                    BetaVersions = splitter(1)
                ElseIf splitter(0) = Options.UpdatesAtStart.Name Then

                    UpdatesAtStart = splitter(1)

                    If UpdatesAtStart = True Then
                        AtStartup = True
                        GetUpdates.RunWorkerAsync()
                    End If

                ElseIf splitter(0) = Options.Visualisation.Name Then
                    Visualisation = splitter(1)

                    If Visualisation = True Then

                        VisualisationBox.Show()
                        Me.Size = New Size(Me.MaximumSize)

                    Else

                        VisualisationBox.Hide()
                        Me.Size = New Size(Me.MinimumSize)

                    End If

                ElseIf splitter(0) = Options.VisualisationType.Name Then
                    VisualisationType = splitter(1)
                ElseIf splitter(0) = Options.HighQualityVis.Name Then
                    HighQualityVis = splitter(1)
                ElseIf splitter(0) = Options.LinealRepresentation.Name Then
                    LinealRepresentation = splitter(1)
                ElseIf splitter(0) = Options.FullSoundRange.Name Then
                    FullSoundRange = splitter(1)
                ElseIf splitter(0) = Options.Smoothness.Name Then
                    Smoothness = splitter(1)
                ElseIf splitter(0) = Options.MainColour.Name Then
                    MainColour = splitter(1)
                ElseIf splitter(0) = Options.SecondaryColour.Name Then
                    SecondaryColour = splitter(1)
                ElseIf splitter(0) = Options.PeakColour.Name Then
                    PeakColour = splitter(1)
                ElseIf splitter(0) = Options.BackgroundColour.Name Then
                    BackgroundColour = splitter(1)
                ElseIf splitter(0) = Options.ChangeWholeBackground.Name Then
                    ChangeWholeBackground = splitter(1)

                    If ChangeWholeBackground = True Then

                        Me.BackColor = Color.FromArgb(BackgroundColour)
                        ToolStrip1.BackColor = Color.FromArgb(BackgroundColour)
                        StationChooser.BackColor = Color.FromArgb(BackgroundColour)
                        Label1.BackColor = Color.FromArgb(BackgroundColour)
                        Label2.BackColor = Color.FromArgb(BackgroundColour)

                        If BackgroundColour < -8323328 Then
                            RadioString.ForeColor = Color.White
                            TimerString.ForeColor = Color.White
                        Else
                            RadioString.ForeColor = Color.Black
                            TimerString.ForeColor = Color.Black
                        End If

                        If BackgroundColour < -7105537 Then
                            EditFavorites.LinkColor = Color.White
                            RefreshFavorites.LinkColor = Color.White
                        Else
                            EditFavorites.LinkColor = Color.Blue
                            RefreshFavorites.LinkColor = Color.Blue
                        End If

                    End If
                ElseIf splitter(0) = Options.MultimediaKeys.Name Then
                    MultimediaKeys = splitter(1)
                ElseIf splitter(0) = Options.HotkeyPlayStop.Name Then
                    HumanModifiersPlayStop = splitter(1)
                    ModifiersPlayStop = splitter(2)
                    KeyPlayStop = splitter(3)
                ElseIf splitter(0) = Options.HotkeyVolumeUp.Name Then
                    HumanModifiersVolumeUp = splitter(1)
                    ModifiersVolumeUp = splitter(2)
                    KeyVolumeUp = splitter(3)
                ElseIf splitter(0) = Options.HotkeyVolumeDown.Name Then
                    HumanModifiersVolumeDown = splitter(1)
                    ModifiersVolumeDown = splitter(2)
                    KeyVolumeDown = splitter(3)
                ElseIf splitter(0) = Options.HotkeyMuteUnmute.Name Then
                    HumanModifiersMuteUnmute = splitter(1)
                    ModifiersMuteUnmute = splitter(2)
                    KeyMuteUnmute = splitter(3)
                ElseIf splitter(0) = Options.HotkeyShowHide.Name Then
                    HumanModifiersShowHide = splitter(1)
                    ModifiersShowHide = splitter(2)
                    KeyShowHide = splitter(3)
                ElseIf splitter(0) = Options.Band0.Name Then
                    Band0 = splitter(1)
                ElseIf splitter(0) = Options.Band1.Name Then
                    Band1 = splitter(1)
                ElseIf splitter(0) = Options.Band2.Name Then
                    Band2 = splitter(1)
                ElseIf splitter(0) = Options.Band3.Name Then
                    Band3 = splitter(1)
                ElseIf splitter(0) = Options.Band4.Name Then
                    Band4 = splitter(1)
                ElseIf splitter(0) = Options.Band5.Name Then
                    Band5 = splitter(1)
                ElseIf splitter(0) = StationChooser.Name Then

                    RadioStation = splitter(1)

                ElseIf splitter(0) = "DIChannel" Then

                    DIChannel = splitter(1)

                ElseIf splitter(0) = "SkyChannel" Then

                    SKYChannel = splitter(1)

                ElseIf splitter(0) = "JazzChannel" Then

                    JazzChannel = splitter(1)

                ElseIf splitter(0) = "RockChannel" Then

                    RockChannel = splitter(1)

                ElseIf splitter(0) = Volume.Name Then
                    Volume.Value = splitter(1)
                ElseIf splitter(0) = SelectedServer.Name And SelectedChannel.Text = "My Favorites" Then
                    OldFav = splitter(1)
                End If


            Loop

            reader.Close()
            reader.Dispose()

            If RadioStation = DIFM.Text Then
                DIFM_Click(Me, Nothing)
            ElseIf RadioStation = SKYFM.Text Then
                SKYFM_Click(Me, Nothing)
            ElseIf RadioStation = JazzRadio.Text Then
                JazzRadio_Click(Me, Nothing)
            ElseIf RadioStation = RockRadio.Text Then
                RockRadio_Click(Me, Nothing)
            End If

            If ModifiersShowHide = 0 And KeyShowHide = 0 Then
                HumanModifiersShowHide = Keys.Control + Keys.Shift
                ModifiersShowHide = 6
                KeyShowHide = Keys.Home
            End If

            If FullSoundRange = True Then

                If VisualisationType = 0 Then
                    drawing.ScaleFactorLinear = 1
                    drawing.ScaleFactorLinearBoost = 0
                    drawing.ScaleFactorSqr = 1
                    drawing.ScaleFactorSqrBoost = 0
                ElseIf VisualisationType = 1 Then
                    drawing.ScaleFactorLinear = 3
                    drawing.ScaleFactorLinearBoost = 0.34
                    drawing.ScaleFactorSqr = 1
                    drawing.ScaleFactorSqrBoost = 0.04
                ElseIf VisualisationType = 2 Then
                    drawing.ScaleFactorLinear = 0.6
                    drawing.ScaleFactorLinearBoost = 0.1
                    drawing.ScaleFactorSqr = 1
                    drawing.ScaleFactorSqrBoost = 0
                ElseIf VisualisationType = 3 Then
                    drawing.ScaleFactorLinear = 1.5
                    drawing.ScaleFactorLinearBoost = 0.07
                    drawing.ScaleFactorSqr = 1
                    drawing.ScaleFactorSqrBoost = 0
                ElseIf VisualisationType = 4 Then
                    drawing.ScaleFactorLinear = 3
                    drawing.ScaleFactorLinearBoost = 0.7
                    drawing.ScaleFactorSqr = 1
                    drawing.ScaleFactorSqrBoost = 0.25
                ElseIf VisualisationType = 5 Then
                    drawing.ScaleFactorLinear = 3
                    drawing.ScaleFactorLinearBoost = 0.7
                    drawing.ScaleFactorSqr = 1
                    drawing.ScaleFactorSqrBoost = 0.25
                ElseIf VisualisationType = 6 Then
                    drawing.ScaleFactorLinear = 1
                    drawing.ScaleFactorLinearBoost = 0.14
                    drawing.ScaleFactorSqr = 0.6
                    drawing.ScaleFactorSqrBoost = 0.01
                ElseIf VisualisationType = 7 Then
                    drawing.ScaleFactorLinear = 0
                    drawing.ScaleFactorLinearBoost = 0
                    drawing.ScaleFactorSqr = 0
                    drawing.ScaleFactorSqrBoost = 0
                End If

            Else

                If VisualisationType = 0 Then
                    drawing.ScaleFactorLinear = 1
                    drawing.ScaleFactorLinearBoost = 0.1
                    drawing.ScaleFactorSqr = 0.6
                    drawing.ScaleFactorSqrBoost = 0
                ElseIf VisualisationType = 1 Then
                    drawing.ScaleFactorLinear = 9
                    drawing.ScaleFactorLinearBoost = 0.18
                    drawing.ScaleFactorSqr = 7
                    drawing.ScaleFactorSqrBoost = 0
                ElseIf VisualisationType = 2 Then
                    drawing.ScaleFactorLinear = 3
                    drawing.ScaleFactorLinearBoost = 0.06
                    drawing.ScaleFactorSqr = 2
                    drawing.ScaleFactorSqrBoost = 0.02
                ElseIf VisualisationType = 3 Then
                    drawing.ScaleFactorLinear = 4
                    drawing.ScaleFactorLinearBoost = 0.07
                    drawing.ScaleFactorSqr = 1.5
                    drawing.ScaleFactorSqrBoost = 0.2
                ElseIf VisualisationType = 4 Then
                    drawing.ScaleFactorLinear = 5.5
                    drawing.ScaleFactorLinearBoost = 0.23
                    drawing.ScaleFactorSqr = 2.5
                    drawing.ScaleFactorSqrBoost = 0.15
                ElseIf VisualisationType = 5 Then
                    drawing.ScaleFactorLinear = 5.5
                    drawing.ScaleFactorLinearBoost = 0.23
                    drawing.ScaleFactorSqr = 2.5
                    drawing.ScaleFactorSqrBoost = 0.15
                ElseIf VisualisationType = 6 Then
                    drawing.ScaleFactorLinear = 3
                    drawing.ScaleFactorLinearBoost = 0.025
                    drawing.ScaleFactorSqr = 2.3
                    drawing.ScaleFactorSqrBoost = 0.014
                ElseIf VisualisationType = 7 Then
                    drawing.ScaleFactorLinear = 0
                    drawing.ScaleFactorLinearBoost = 0
                    drawing.ScaleFactorSqr = 0
                    drawing.ScaleFactorSqrBoost = 0
                End If

            End If

        Catch ex As Exception

            MsgBox("There was an error loading your options.ini file:" & vbNewLine & ex.Message & vbNewLine & vbNewLine & "Please close the application and open it again.", MsgBoxStyle.Exclamation, "Invalid options.ini file")

        End Try

        Me.CenterToScreen()
    End Sub

    Sub DisplayMessage(ByVal text As String, ByVal style As MsgBoxStyle, ByVal title As String)
        MsgBox(text, style, title)
        Me.BringToFront()
    End Sub

    Private Sub SetUpEq()
        EqBands(0) = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0)
        EqBands(1) = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0)
        EqBands(2) = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0)
        EqBands(3) = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0)
        EqBands(4) = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0)
        EqBands(5) = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0)

        Eq.fBandwidth = 18

        Eq.fCenter = 80
        Bass.BASS_FXSetParameters(EqBands(0), Eq)
        Eq.fCenter = 220
        Bass.BASS_FXSetParameters(EqBands(1), Eq)
        Eq.fCenter = 622
        Bass.BASS_FXSetParameters(EqBands(2), Eq)
        Eq.fCenter = 1800
        Bass.BASS_FXSetParameters(EqBands(3), Eq)
        Eq.fCenter = 5000
        Bass.BASS_FXSetParameters(EqBands(4), Eq)
        Eq.fCenter = 140000
        Bass.BASS_FXSetParameters(EqBands(5), Eq)
    End Sub

    Public Sub UpdateEq(band As Integer, gain As Single)
        Dim Eq As New BASS_DX8_PARAMEQ()

        If Bass.BASS_FXGetParameters(EqBands(band), Eq) Then
            Eq.fGain = gain
            Bass.BASS_FXSetParameters(EqBands(band), Eq)
        End If
    End Sub

    ' The following code thanks to _Tobias from the Digitally Imported forums.

    Public Function RandomNumber(ByVal MaxNumber As Integer, Optional ByVal MinNumber As Integer = 0) As Integer

        'initialize random number generator
        Dim r As New Random(System.DateTime.Now.Millisecond)

        'if passed incorrect arguments, swap them
        'can also throw exception or return 0

        If MinNumber > MaxNumber Then
            Dim t As Integer = MinNumber
            MinNumber = MaxNumber
            MaxNumber = t
        End If

        Return r.Next(MinNumber, MaxNumber)

    End Function

    Public Function downloadStreams(ByVal key As String, ByVal quality As String, ByVal serversFolder As String, Optional ByVal premiumadd As String = "")
        Dim pls
        If My.Computer.FileSystem.FileExists(serversFolder & "\" & key & ".db") = False Then
            Dim wc As Net.WebClient = New Net.WebClient

            Try
                pls = wc.DownloadString("http://listen." & StationChooser.Tag & "/" & quality & "/" & key & ".pls" & premiumadd)
            Catch ex As Exception
                Return False
            End Try
        Else
            If My.Computer.FileSystem.FileExists(serversFolder & "/" & key & ".db") Then
                Dim reader As IO.StreamReader = New IO.StreamReader(serversFolder & "/" & key & ".db")
                pls = reader.ReadToEnd()
            Else
                Return False
            End If
            

        End If
        Return Audioaddict.ParsePlaylistAudioaddict(pls)

    End Function

    Public Function downloadFavos(ByVal serversFolder As String, ByVal listenkey As String)
        Dim data
        If My.Computer.FileSystem.FileExists(serversFolder & "\favorites.db") = False Then
            Dim wc As Net.WebClient = New Net.WebClient
            Try
                data = wc.DownloadString("http://tobiass.eu/api/favorites/" & StationChooser.Tag & "/" & listenkey)
            Catch ex As Exception
                Return False
            End Try
        Else
            If My.Computer.FileSystem.FileExists(serversFolder & "\favorites.db") Then
                Dim reader As IO.StreamReader = New IO.StreamReader(serversFolder & "\favorites.db")
                data = reader.ReadToEnd()
                reader.Close()
            Else
                Return False
            End If
            

        End If
        Return Split(data, vbNewLine)

    End Function

    Public Function channelDb(ByVal loc As String)

        Dim fileinfo As New IO.FileInfo(loc)

        If My.Computer.FileSystem.FileExists(loc) = False Or fileinfo.LastWriteTime.Date = DateTime.UtcNow.Date = False Then
            Dim wc As Net.WebClient = New Net.WebClient
            Dim data

            Try
                data = wc.DownloadString("http://tobiass.eu/api/channels/" & StationChooser.Tag)
            Catch ex As Exception
                Return "Didn't download"
            End Try

            Dim writer As New IO.StreamWriter(loc, False)

            writer.Write(data)
            writer.Close()
        End If

        Return New IO.StreamReader(loc)
    End Function

#End Region

End Class
