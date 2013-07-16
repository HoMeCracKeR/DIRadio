' DI Radio Player by ViRUS
'
'
' I've done my best to document the most relevant parts of this source code, but if you still find yourself having problems
' with it, feel free to drop me an e-mail at newvirus@live.com.ar
'
' This source code is protected by the BSD license and you should have received a "BSD License.txt" file with it.
' If you haven't, please drop me an e-mail at newvirus@live.com.ar with information on where you downloaded the source code
'
' One last note. This source code will compile as-is, but Bass.Net will display a popup when you start the player since I've erased my
' e-mail and registration key (you can't publicly share it). To get a registration key head over to http://bass.radio42.com/bass_register.html
' Registering is free and shouldn't take you more than 5 minutes. Once you have your registration key, go into the Application Events and into
' the Startup event. You'll find where to put your registration info there.
'
' Have fun!

Imports Un4seen.Bass
Imports Un4seen.Bass.Misc
Imports System.Runtime.InteropServices

Public Class Player

#Region "Global Declarations"

#Region "Options"

    ' Most of these get a value once the LoadOptions function is called.
    ' They all have default values in case something goes wrong loading the options.ini file.
    ' They are ordered in the same way as they appear in the Options dialog.

    Public NotificationTitle As Boolean = True
    Public PlayNewOnChannelChange As Boolean = False
    Public NotificationIcon As Boolean = True
    Public NoTaskbarButton As Boolean = False
    Public GoogleSearch As Boolean = True
    Public ShowSongStart As Boolean = False

    Public DIFormat As SByte = 0
    Public SKYFormat As SByte = 0
    Public JazzFormat As SByte = 0
    Public ListenKey As String

    Public UpdatesAtStart As Boolean = True
    Public BetaVersions As Boolean = False

    Public Visualisation As Boolean = True
    Public VisualisationType As Byte = 5
    Public HighQualityVis As Boolean = False
    Public LinealRepresentation As Boolean = False
    Public FullSoundRange As Boolean = False
    Public Smoothness As Byte = 27
    Public MainColour As Integer = -986896
    Public SecondaryColour As Integer = -16777216
    Public PeakColour As Integer = -4144960
    Public BackgroundColour As Integer = -986896
    Public ChangeWholeBackground As Boolean = False

    Public MultimediaKeys As Boolean = True

    Public Band0 As SByte = 0
    Public Band1 As SByte = 0
    Public Band2 As SByte = 0
    Public Band3 As SByte = 0
    Public Band4 As SByte = 0
    Public Band5 As SByte = 0

    Public username As String
    Public password As String
    Public userInfo As String
    Public isLogged As Boolean = False
    Public isPremium As Boolean = False
    Public canTrial As Boolean = False
    Public apiKey As String
    Public userId As String

    Public removeKey As Boolean = True

    Public cacheList As String = "Every day"

    ' These are not on the Options dialog but are saved anyway.

    Public DIChannel As String = "Ambient"                  ' -> Last used Digitally Imported channel
    Public SKYChannel As String = "80's Rock Hits"          ' -> Last used SKY.FM channel
    Public JazzChannel As String = "Avant-Garde"            ' -> Last used JazzRadio channel
    Public RockChannel As String = "80's Alternative"       ' -> Last used RockRadio channel

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

    <DllImport("User32.dll")> Public Shared Function RegisterHotKey(ByVal hwnd As IntPtr, ByVal id As Integer, ByVal fsModifiers As Integer, ByVal vk As Integer) As Integer
    End Function

    <DllImport("User32.dll")> _
    Public Shared Function UnregisterHotKey(ByVal hwnd As IntPtr, ByVal id As Integer) As Integer
    End Function

    Public Const WM_HOTKEY As Integer = &H312

#End Region

#Region "Update stuff"

    Public AtStartup As String = False                              ' -> Used to tell the GetUpdates background worker that it's looking for updates at startup. Only becomes True if UpdatesAtStart is true
    Public TotalVersionString As String = "Didn't download"         ' -> Used to store the TotalVersion returned by the server
    Public LatestVersionString As String = "Didn't download"        ' -> Used to store the actual version number returned by the server
    Public TotalVersionFixed As Byte = 62                           ' -> For commodity, I don't use the actual version number of the application to know when there's an update. Instead I check if this number is higher.
    Public UpdaterDownloaded As Boolean = False                     ' -> Used when the updater file has been downloaded in this run, to avoid having to download it again

#End Region

#Region "Other"

    Public drawing As New Un4seen.Bass.Misc.Visuals ' -> Used to draw the vis
    Public RestartPlayback As Boolean               ' -> Used to know if playback should be restarted after an operation has completed (changing channels, for example)
    Public stream As Integer                        ' -> The stream that is passed to BASS so it plays it
    Public oldvol As Byte                        ' -> This stores the volume when the user clicks the Mute button; to know which volume level should be used when the user clicks the Unmute button
    Dim ServersArray As New ListView                ' -> Used to store a list of available servers for a particular channel
    Dim EventsArray As New ListView                 ' -> Used to store some info when obtaining events
    ' v  This list of channels may be outdated. It's only used as a fallback in case CheckForums fails to download the (maybe updated?) list of channels that don't have a forum link
    Dim NoForumsChannel As String = "Cosmic Downtempo;Deep Nu-Disco;Vocal Chillout;Deep House;Epic Trance;Hands Up;Club Dubstep;Progressive Psy;80's Rock Hits;Club Bollywood;Compact Discoveries;Hard Rock;Metal;Modern Blues;Modern Rock;Pop Rock;Relaxing Excursions;Ska;Smooth Lounge;Soft Rock;Glitch Hop;Deep Tech;Liquid Dubstep;Classic EuroDisco;Dark DnB;90's Hits;Mellow Jazz;Café de Paris;Christmas Channel;UMF Radio;UMF Stage 1;UMF Stage 2;Big Room House;EcLectronica;Russian Club Hits;Mainstage;Best of the 60s; Classic Motown;Russian Pop;Russian Dance Hits;Israeli Hits;ChillHop;Downtempo Lounge;Trap;Sankeys Radio;80's Dance;90's R&B;60's Rock"
    Private _mySync As SYNCPROC                     ' -> Sync so BASS says when the stream title has changed
    Dim nochange As Boolean

    Dim channelKey As Byte
    Dim KeysArray As New ListView
    Dim UpdateWait As Integer = 0

    Delegate Sub MsgBoxSafe(ByVal text As String, ByVal style As MsgBoxStyle, ByVal title As String)

    Private EqBands As Integer() = {0, 0, 0, 0, 0, 0}
    Dim Eq As New BASS_DX8_PARAMEQ()

    Public WasPlaying As String

    Public dataFolder As String

    Public HotkeysSet As Boolean = False
    Delegate Sub RestartPlaybackSafe()

#End Region

#End Region

#Region "Main Form events"

    Private Sub Player_DragEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles Me.DragEnter
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

    Private Sub Player_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles Me.DragDrop
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then

            Dim filepath() As String = e.Data.GetData(DataFormats.FileDrop)
            ReadThemeFile(filepath(0))
            ApplyTheme()

        End If
    End Sub

    Private Sub Player_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        ' I know how to check my threads. Don't need VS babysitting me
        Control.CheckForIllegalCrossThreadCalls = False

        If String.IsNullOrEmpty(My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\DI Radio", "installDir", Nothing)) = True Then
            Dim executable As String = Application.ExecutablePath
            Dim tabla() As String = Split(executable, "\")
            dataFolder = Application.ExecutablePath.Replace(tabla(tabla.Length - 1), Nothing)
        Else
            dataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) & "\DI Radio\"
        End If

        DownloadingMessage.SelectedIndex = 0
        DownloadingMessage.Show()

        ' Try to open a device for BASS
        If Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, Me.Handle, Nothing) = False Then

            If Bass.BASS_ErrorGetCode = 23 Then
                MessageBox.Show("No audio devices available." & vbNewLine & "The application will close now.", "Critical error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            Else
                MessageBox.Show("The application can't start due to the error number " & Bass.BASS_ErrorGetCode, "Critical error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            End If

            End

        End If

        CheckForums.RunWorkerAsync()

        ' Create the servers folder and a folder for each station.
        My.Computer.FileSystem.CreateDirectory(dataFolder & "servers")
        My.Computer.FileSystem.CreateDirectory(dataFolder & "servers\" & DIFM.Text)
        My.Computer.FileSystem.CreateDirectory(dataFolder & "servers\" & JazzRadio.Text)
        My.Computer.FileSystem.CreateDirectory(dataFolder & "servers\" & SKYFM.Text)
        My.Computer.FileSystem.CreateDirectory(dataFolder & "servers\" & RockRadio.Text)
        My.Computer.FileSystem.CreateDirectory(dataFolder & "equalizer\")
        My.Computer.FileSystem.WriteAllText(dataFolder & "equalizer\readme.txt", "Use this folder to load equalizer settings per-channel." & vbNewLine & "Simply place your .feq files with the name of the channel you want to use (trance.feq or smooth jazz 24'7.feq, for example) and the player will load it automatically when you select that channel." & vbNewLine & "You can also have a default.feq file for channels that don't have a custom setting, or no files at all to use what's in your options.ini file.", False)

        ' Load plugin for AAC support
        Bass.BASS_PluginLoad("bassaac.dll")
        Bass.BASS_PluginLoad("basswma.dll")

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

    End Sub

    Private Sub Player_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
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

    Private Sub Player_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.TextChanged

        If TrayIcon.Text.StartsWith("DI Radio") OrElse TrayIcon.Text.StartsWith("JazzRadio Radio") OrElse TrayIcon.Text.StartsWith("SKY.FM Radio") OrElse TrayIcon.Text.StartsWith("RockRadio Radio") Then
            TrayIcon.Text = Me.Text
        End If

    End Sub

    Private Sub Player_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If FadeOut.Enabled = False Then
            If SaveSettings(True) = False Then
                If MessageBox.Show("Your options couldn't be saved." & vbNewLine & "Would you like to close the player anyway (your options won't be saved)?", "Error saving options", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) = MsgBoxResult.No Then
                    e.Cancel = True
                    Exit Sub
                End If
            End If
        End If

        Options.Close()
        Me.Hide()

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

        If GoogleSearch Then

            If RadioString.Text = Nothing = False And RadioString.Text.Contains("Connection is taking some time") = False And RadioString.Text.Contains("Lost connection to") = False And RadioString.Text.Contains("Connecting, please wait...") = False And RadioString.Text.Contains("Couldn't connect to") = False Then
                GoogleSearchToolStripMenuItem_Click(Me, Nothing)
            End If

        End If

    End Sub

    Private Sub RadioString_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles RadioString.TextChanged

        If String.IsNullOrEmpty(RadioString.Text) = False Then

            Dim raw As String = RadioString.Text.Replace("&", "&&&")

            If raw.Length >= 64 Then
                TrayIcon.Text = raw.Remove(60, raw.Length - 60) & "..."
            Else
                TrayIcon.Text = raw
            End If

            If Me.WindowState = FormWindowState.Minimized And NotificationTitle And _
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
             RadioString.Text.ToLower = "digitally imported radio (captivating)" = False And _
             RadioString.Text.ToLower = "get ready for di radio's imported set of the day" = False And _
             RadioString.Text.ToLower = "love thy neighbour as thyself - turn up the volume (ht)" = False Then

                TrayIcon.BalloonTipText = RadioString.Text

                If RadioString.Text.Contains("Connecting, please wait...") Then
                    TrayIcon.BalloonTipTitle = "Info:"
                    TrayIcon.BalloonTipIcon = ToolTipIcon.Info
                ElseIf RadioString.Text.Contains("Connection is taking some time") Then
                    TrayIcon.BalloonTipTitle = "Warning:"
                    TrayIcon.BalloonTipIcon = ToolTipIcon.Warning
                ElseIf RadioString.Text.Contains("Lost connection to") OrElse RadioString.Text.Contains("Couldn't connect to") OrElse RadioString.Text.Contains("Internet connection") Then
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

                If BackgroundColour < -8323328 Then
                    RadioString.ForeColor = Color.White
                    TimerString.ForeColor = Color.White
                Else
                    RadioString.ForeColor = Color.Black
                    TimerString.ForeColor = Color.Black
                End If
            Else
                RadioString.BackColor = SystemColors.Control

                RadioString.ForeColor = SystemColors.ControlText
                TimerString.ForeColor = SystemColors.ControlText
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
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, 0)
        End If
    End Sub

    Private Sub OptionsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OptionsButton.Click
        Dim X As Integer
        Dim Y As Integer

        ' If main form is visible, get its boundaries, make sure the Options panel won't appear out of screen
        ' and then open it and bring it to front just in case. Otherwise simply center it on screen and
        ' bring it.

        If Me.Visible = True Then

            If Me.Location.X + 8 > SystemInformation.VirtualScreen.Width Then
                X = Me.Location.X - Options.Size.Width
            ElseIf Me.Location.X + 8 < Screen.GetBounds(Me).X Then
                X = Screen.GetBounds(Me).X
            Else
                X = Me.Location.X + 8
            End If

            If Visualisation = True Then
                If (Me.Location.Y - 9) + Options.Size.Height > SystemInformation.VirtualScreen.Height Then
                    Y = SystemInformation.VirtualScreen.Height - Options.Size.Height
                ElseIf Me.Location.Y - 9 < Screen.GetBounds(Me).Y Then
                    Y = Screen.GetBounds(Me).Y
                Else
                    Y = Me.Location.Y - 9
                End If
            Else
                If (Me.Location.Y - 9) + Options.Size.Height > SystemInformation.VirtualScreen.Height Then
                    Y = SystemInformation.VirtualScreen.Height - Options.Size.Height
                ElseIf Me.Location.Y - 190 < Screen.GetBounds(Me).Y Then
                    Y = Screen.GetBounds(Me).Y
                Else
                    Y = Me.Location.Y - 190
                End If
            End If

            Options.Location = New Point(X, Y)

        Else

            Options.StartPosition = FormStartPosition.CenterScreen

        End If

        Options.Show()
        Options.BringToFront()

    End Sub

    Private Sub showEvents_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles showEvents.Click
        If EventsPanel.Visible = False Then

            If HistoryList.Visible = True Then
                History_Click(Me, Nothing)
            End If

            showEvents.ImageAlign = ContentAlignment.BottomCenter
            showEvents.Image = My.Resources.back
            SelectedEvent.Enabled = False

            If GetEvents.IsBusy = False Then
                GetEvents.RunWorkerAsync()
            End If

            EventsPanel.Show()

            VisTimer.Stop()
            Me.Size = Me.MaximumSize

            ToolTip.SetToolTip(showEvents, "Hide events list")
        Else

            showEvents.ImageAlign = ContentAlignment.MiddleCenter
            showEvents.Image = My.Resources.events
            EventsPanel.Hide()

            If Visualisation = True Then
                VisTimer.Start()
            Else
                Me.Size = Me.MinimumSize
            End If

            ToolTip.SetToolTip(showEvents, "Show events list")
        End If
    End Sub

    Private Sub Forums_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Forums.Click
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
        ElseIf Volume.Value > 0 And Volume.Value < 25 Then
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

    Private Sub EditFavorites_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles EditFavorites.LinkClicked
        Process.Start("http://www." & StationChooser.Tag & "/member/favorite/channels")
    End Sub

    Private Sub RefreshFavorites_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles RefreshFavorites.LinkClicked

        OldFav = SelectedServer.Text
        Dim file As String = dataFolder & "servers\" & StationChooser.Text & "\favorites.db"

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

    Private Sub StationChooser_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles StationChooser.ButtonClick
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

    Public Sub StationChooser_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles StationChooser.TextChanged

        ' Long lists were removed by _Tobias. Now the player doesn't have to be updated in order to add new channels!

        PlayStop.Enabled = False
        StationChooser.Enabled = False
        SelectedChannel.Enabled = False
        SelectedServer.Enabled = False

        SelectedChannel.Items.Clear()

        showEvents.Enabled = False
        History.Enabled = False
        Forums.Enabled = False
        HistoryList.Items.Clear()
        RetryChannels.Hide()
        DownloadingMessage.Show()
        Marquee.Show()
        Options.radioStation = StationChooser.Text

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
            nochange = False
        Else
            nochange = True
        End If

        If StationChooser.Text = DIFM.Text Then
            DIChannel = SelectedChannel.Text
        ElseIf StationChooser.Text = SKYFM.Text Then
            SKYChannel = SelectedChannel.Text
        ElseIf StationChooser.Text = JazzRadio.Text Then
            JazzChannel = SelectedChannel.Text
        ElseIf StationChooser.Text = RockRadio.Text Then
            RockChannel = SelectedChannel.Text
        End If

        If HistoryList.Visible = True And GetHistory.IsBusy = False Then
            GetHistory.RunWorkerAsync()
        End If

        If EventsPanel.Visible = True And GetEvents.IsBusy = False Then
            GetEvents.RunWorkerAsync()
        End If
    End Sub

    Private Sub SelectedChannel_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles SelectedChannel.TextChanged
        ' Check if the Forums button should be enabled or not and exit as soon as there is a match
        ' Or always disable the button if JazzRadio or RockRadio is selected

        Dim Channel() As String = Split(NoForumsChannel, ";")
        Dim ChannelNumber As Byte = 0

        Do While ChannelNumber < Channel.Length
            ChannelNumber += 1
            If ChannelNumber <= Channel.Length - 1 Then

                If SelectedChannel.Text = Channel(ChannelNumber) OrElse StationChooser.Text = JazzRadio.Text OrElse StationChooser.Text = RockRadio.Text OrElse SelectedChannel.Text = Nothing Then
                    Forums.Enabled = False
                    Exit Do
                Else
                    Forums.Enabled = True
                End If

            Else
                Exit Do
            End If
        Loop

        Dim file As String = dataFolder & "equalizer\" & SelectedChannel.Text & ".feq"

        If My.Computer.FileSystem.FileExists(file) And SelectedChannel.Text = "My Favorites" = False Then
            LoadEqFile(file)
        ElseIf My.Computer.FileSystem.FileExists(dataFolder & "equalizer\default.feq") And SelectedChannel.Text = "My Favorites" = False Then
            LoadEqFile(dataFolder & "equalizer\default.feq")
        End If

        Options.lookForChannels.Enabled = False

    End Sub

    Private Sub SelectedServer_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles SelectedServer.SelectedIndexChanged

        If SelectedChannel.Text = "My Favorites" And PlayStop.Tag = "Stop" And PlayNewOnChannelChange = True Then
            RestartPlayback = True
            PlayStop_Click(Me, Nothing)
            OldFav = SelectedServer.Text
            SelectedChannel_SelectedIndexChanged(Me, Nothing)
        ElseIf SelectedChannel.Text = "My Favorites" = False And PlayStop.Tag = "Stop" And nochange = False Then
            PlayStop_Click(Me, Nothing)
            PlayStop_Click(Me, Nothing)
        End If

        If SelectedChannel.Text = "My Favorites" Then
            Dim Channels As String = NoForumsChannel
            Dim Channel() As String = Split(Channels, ";")
            Dim ChannelNumber As Byte = 0

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

            If HistoryList.Visible = True And GetHistory.IsBusy = False Then
                GetHistory.RunWorkerAsync()
            End If

            If EventsPanel.Visible = True And GetEvents.IsBusy = False Then
                GetEvents.RunWorkerAsync()
            End If
        End If

        Dim file As String = dataFolder & "equalizer\" & SelectedServer.Text & ".feq"

        If My.Computer.FileSystem.FileExists(file) And SelectedChannel.Text = "My Favorites" Then
            LoadEqFile(file)
        ElseIf My.Computer.FileSystem.FileExists(dataFolder & "equalizer\default.feq") And SelectedChannel.Text = "My Favorites" Then
            LoadEqFile(dataFolder & "equalizer\default.feq")
        End If

    End Sub

    Private Sub TimerString_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerString.TextChanged

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

    Private Sub RetryChannels_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RetryChannels.Click
        StationChooser_TextChanged(Me, Nothing)
        RetryChannels.Hide()
        Marquee.Show()
        StationChooser.Enabled = False
    End Sub

    Private Sub RetryServers_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RetryServers.Click
        SelectedChannel_SelectedIndexChanged(Me, Nothing)
        RetryServers.Hide()
        Marquee.Show()
    End Sub

    Private Sub History_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles History.Click
        If HistoryList.Visible = False Then

            If EventsPanel.Visible = True Then
                showEvents_Click(Me, Nothing)
            End If

            History.ImageAlign = ContentAlignment.BottomCenter
            History.Image = My.Resources.back
            HistoryList.Enabled = False
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

    Private Sub SelectedEvent_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SelectedEvent.SelectedIndexChanged

        If SelectedEvent.Text.ToLower.StartsWith("please wait,") = False And SelectedEvent.Text.ToLower.StartsWith("couldn't download") = False And SelectedEvent.Text.ToLower.StartsWith("there are no") = False Then
            EventName.Text = EventsArray.Items.Item(SelectedEvent.SelectedIndex).Text
            EventTagline.Text = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.Default.GetBytes(EventsArray.Items.Item(SelectedEvent.SelectedIndex).SubItems(1).Text))

            EventTimes.Text = ReturnDate(EventsArray.Items.Item(SelectedEvent.SelectedIndex).SubItems(2).Text, "hourmin") & " - "

            EventTimes.Text += ReturnDate(EventsArray.Items.Item(SelectedEvent.SelectedIndex).SubItems(3).Text, "hourmin")

            Dim thistime As Integer
            thistime = (DateTime.Now - New DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds

            If thistime > EventsArray.Items.Item(SelectedEvent.SelectedIndex).SubItems(2).Text And thistime < EventsArray.Items.Item(SelectedEvent.SelectedIndex).SubItems(3).Text Then
                EventTimes.Text += " ♫ Now playing ♫"
            End If

            EventDescription.Text = "Please wait, downloading event details..."

            If GetEventDetails.IsBusy = False Then
                GetEventDetails.RunWorkerAsync()
            End If

        End If

    End Sub

    Private Sub EventDescription_LinkClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.LinkClickedEventArgs) Handles EventDescription.LinkClicked
        Process.Start(e.LinkText)
    End Sub

    Private Sub Export_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExportButton.Click
        If ExportButton.Text = "Options" Then
            eventOptionsMenu.Show(Me, ExportButton.Location.X + 13, ExportButton.Location.Y + 35)
        Else
            ExportButton.Text = "Options"
            GetEvents.RunWorkerAsync()
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
        Dim serversFolder = dataFolder & "servers\" & StationChooser.Text

        ' create servers directory if doesn't exist
        My.Computer.FileSystem.CreateDirectory(serversFolder)

        ' determine channel database
        Dim chdb = serversFolder & "\channels.db"

        If channel = "My Favorites" Then
            channel = "favorites"
        End If

        ' open file

        Try

            If My.Computer.FileSystem.FileExists(chdb) Then
                Dim readerChdb As IO.StreamReader = channelDb(chdb)

                ' read file
                Do While (readerChdb.Peek > -1)
                    Dim line = readerChdb.ReadLine()
                    Dim splitter = Split(line, "|")

                    If splitter(2) = channel Then
                        channel = splitter(1)
                    End If
                Loop

                readerChdb.Close()
                readerChdb.Dispose()
            Else
                Marquee.Show()
                DownloadDb.RunWorkerAsync()
                Exit Sub
            End If

        Catch ex As Exception
            Dim word As String

            If SelectedChannel.Text = "My Favourites" Then
                word = "favourites"
            Else
                word = "servers"
            End If

            Dim Message As New MsgBoxSafe(AddressOf DisplayMessage)

            Me.Invoke(Message, "Couldn't download " & word & " list.", MsgBoxStyle.Exclamation, "Error getting " & word & " list")

            Exit Sub
        End Try

        Dim file = serversFolder & "\" & channel & ".db"

        Dim EndString As String = ""

        Dim PremiumEnd As String = ""

        If isPremium Then

            PremiumEnd = "?" & ListenKey

            If StationChooser.Text = DIFM.Text Then
                If DIFormat = 0 Then
                    EndString = "premium"
                ElseIf DIFormat = 1 Then
                    EndString = "premium_high"
                ElseIf DIFormat = 2 Then
                    EndString = "premium_medium"
                ElseIf DIFormat = 3 Then
                    EndString = "premium_low"
                End If

            ElseIf StationChooser.Text = SKYFM.Text Then

                If SKYFormat = 0 Then
                    EndString = "premium"
                ElseIf SKYFormat = 1 Then
                    EndString = "premium_high"
                ElseIf SKYFormat = 2 Then
                    EndString = "premium_wma"
                ElseIf SKYFormat = 3 Then
                    EndString = "premium_medium"
                ElseIf SKYFormat = 4 Then
                    EndString = "premium_wma_low"
                ElseIf SKYFormat = 5 Then
                    EndString = "premium_low"
                End If

            ElseIf StationChooser.Text = JazzRadio.Text Then

                If JazzFormat = 0 Then
                    EndString = "premium"
                ElseIf JazzFormat = 1 Then
                    EndString = "premium_high"
                ElseIf JazzFormat = 2 Then
                    EndString = "premium_wma"
                ElseIf JazzFormat = 3 Then
                    EndString = "premium_low"
                End If

            ElseIf StationChooser.Text = RockRadio.Text Then

                EndString = "public3"
                PremiumEnd = ""

            End If


        Else

            If StationChooser.Text = DIFM.Text Then

                If DIFormat = 0 Then
                    EndString = "public1"
                ElseIf DIFormat = 1 Then
                    EndString = "public3"
                ElseIf DIFormat = 2 Then
                    EndString = "public2"
                End If

            ElseIf StationChooser.Text = SKYFM.Text Then

                If SKYFormat = 0 Then
                    EndString = "public3"
                ElseIf SKYFormat = 1 Then
                    EndString = "public1"
                ElseIf SKYFormat = 2 Then
                    EndString = "public5"
                End If

            ElseIf StationChooser.Text = JazzRadio.Text Then

                If JazzFormat = 0 Then
                    EndString = "public3"
                ElseIf JazzFormat = 1 Then
                    EndString = "public1"
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

        Dim serverno As Byte = 1

        Dim reader As New IO.StreamReader(file)

        If SelectedChannel.Text = "My Favorites" Then
            Do While (reader.Peek > -1)
                Dim name As String = reader.ReadLine
                Dim key As String = ""

                Dim r2 As New IO.StreamReader(chdb)

                Do While (r2.Peek > -1)
                    Dim splitter = Split(r2.ReadLine, "|")
                    If splitter(2) = name Then
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
                ServersArray.Items.Add(streams(0))
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

    Private Sub ServersDownloader_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles ServersDownloader.RunWorkerCompleted
        If My.Computer.FileSystem.FileExists(dataFolder & "servers\" & StationChooser.Text & "\channels.db") Then
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

            Dim Restart As New RestartPlaybackSafe(AddressOf ResumePlaying)

            If PlayStop.Tag = "Stop" And PlayNewOnChannelChange = True OrElse RestartPlayback = True Then
                Me.Invoke(Restart)
            End If

            If SelectedServer.Items.Count = 0 And SelectedChannel.Text = "My Favorites" Then
                SelectedServer.Items.Add("Click the link to edit list")
                SelectedServer.SelectedIndex = 0
                SelectedServer.Enabled = False
                Forums.Enabled = False
                History.Enabled = False
                HistoryList.Hide()
                History.ImageAlign = ContentAlignment.BottomRight
                History.Image = My.Resources.history
                showEvents.Enabled = False
                EventsPanel.Hide()
                showEvents.ImageAlign = ContentAlignment.MiddleCenter
                showEvents.Image = My.Resources.events
            ElseIf StationChooser.Text = JazzRadio.Text = False And StationChooser.Text = RockRadio.Text = False Then
                History.Enabled = True

                If StationChooser.Text = SKYFM.Text = False Then
                    showEvents.Enabled = True
                End If
            End If

            If Bufer.IsBusy = False Then
                RefreshFavorites.Enabled = True
            End If

            DownloadingMessage.Hide()

            If SelectedServer.Items.Count < 1 Then
                RetryServers.Show()
            End If
        End If

        Options.lookForChannels.Enabled = True
    End Sub

    Private Sub Bufer_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles Bufer.DoWork
        RefreshFavorites.Enabled = False
again:
        If Bufer.CancellationPending = False Then
            stream = Bass.BASS_StreamCreateURL(ServersArray.Items.Item(SelectedServer.SelectedIndex).Text, 2, BASSFlag.BASS_STREAM_AUTOFREE Or BASSFlag.BASS_STREAM_PRESCAN, Nothing, Nothing)

            Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, Volume.Value / 100)
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, 24)

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

                Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, 0)
            Else

                Try
                    RadioString.Text = Split(String.Concat(Bass.BASS_ChannelGetTagsMETA(stream)), "='")(1).Replace("';", Nothing).Replace("StreamUrl", Nothing)
                Catch ex As Exception
                    RadioString.Text = "Now playing"
                End Try


                If ChangeWholeBackground = True Then
                    RadioString.BackColor = Color.FromArgb(BackgroundColour)

                    If BackgroundColour < -8323328 And ChangeWholeBackground = True Then
                        RadioString.ForeColor = Color.White
                        TimerString.ForeColor = Color.White
                    Else
                        RadioString.ForeColor = Color.Black
                        TimerString.ForeColor = Color.Black
                    End If
                Else
                    RadioString.BackColor = SystemColors.Control
                    RadioString.ForeColor = SystemColors.ControlText
                    TimerString.ForeColor = SystemColors.ControlText
                End If



                _mySync = New SYNCPROC(AddressOf MetaSync)
                Bass.BASS_ChannelSetSync(stream, BASSSync.BASS_SYNC_META, 0, _mySync, IntPtr.Zero)

                SetUpEq()
                UpdateEq(0, Band0)
                UpdateEq(1, Band1)
                UpdateEq(2, Band2)
                UpdateEq(3, Band3)
                UpdateEq(4, Band4)
                UpdateEq(5, Band5)

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

        SelectedChannel.Enabled = True
        SelectedServer.Enabled = True
        nochange = False
        TrayMenu_Opening(Me, Nothing)
        RefreshFavorites.Enabled = True
    End Sub

    Private Sub GetUpdates_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles GetUpdates.DoWork

        Dim WebClient As Net.WebClient = New Net.WebClient()
        Dim DownloadedString As String

        Try
            DownloadedString = WebClient.DownloadString("http://www.tobiass.eu/api/update")
        Catch
            Exit Sub
        End Try

        WebClient.Dispose()

        Dim reader As New IO.StringReader(DownloadedString)

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

    Private Sub GetUpdates_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles GetUpdates.RunWorkerCompleted
        Dim updating As Boolean = False
        If TotalVersionString = "Didn't download" = False And LatestVersionString = "Didn't download" = False Then
            Dim FullLine As String = LatestVersionString
            Dim Splitter As String() = Split(FullLine, ".")

            If TotalVersionString > TotalVersionFixed Then

                Options.LookNow.Text = "Download update"
                Options.LatestVersion.ForeColor = Color.Green

                If AtStartup = True Then

                    If Splitter(2) > 0 And BetaVersions = True Then
                        If MessageBox.Show("There's a new beta version available!" & vbNewLine & "Download now?", "Update available", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = MsgBoxResult.Yes Then
                            Options.LookNow_Click(Me, Nothing)
                            OptionsButton_Click(Me, Nothing)
                            updating = True
                        End If
                    ElseIf Splitter(2) = 0 Then
                        If MessageBox.Show("There's a new version available!" & vbNewLine & "Download now?", "Update available", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = MsgBoxResult.Yes Then
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

        End If

        Options.UndefinedProgress.Hide()
        Options.Status.Text = "Status: Idle"
        If updating = False Then
            Options.LookNow.Enabled = True
        End If

    End Sub

    Private Sub GetHistory_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles GetHistory.DoWork
        Dim channel As String

        HistoryList.ForeColor = SystemColors.ControlText

startover:

        If SelectedChannel.Text = "My Favorites" Then
            channel = SelectedServer.Text
        Else
            channel = SelectedChannel.Text
        End If

        HistoryList.Enabled = False
        HistoryList.Items.Clear()
        HistoryList.Items.Add("")
        HistoryList.Items.Item(0).SubItems.Add("Please wait, downloading history...")
        Dim WebClient As Net.WebClient = New Net.WebClient()
        Dim HistoryLog As String

        Try

            If SelectedChannel.Text = "My Favorites" Then
                HistoryLog = WebClient.DownloadString("http://api.audioaddict.com/v1/di/track_history/channel/" & KeysArray.Items.Item(SelectedServer.Text).Tag)
            Else
                HistoryLog = WebClient.DownloadString("http://api.audioaddict.com/v1/di/track_history/channel/" & KeysArray.Items.Item(SelectedChannel.Text).Tag)
            End If
      

            Dim reader As New IO.StringReader(HistoryLog.Replace("{", Nothing).Replace("[", Nothing).Replace("}", Nothing).Replace("]", Nothing).Replace(""",""", vbNewLine).Replace(",""", vbNewLine))

            HistoryList.Items.Clear()

            Do While (reader.Peek > -1)

                If channel = SelectedChannel.Text = False And channel = SelectedServer.Text = False Then
                    GoTo startover
                End If

                Dim line As String = reader.ReadLine

                Dim duration As String
                Dim started As String

                If line.StartsWith("duration"":") Then

                    If Integer.TryParse(Split(line, ":")(1), Nothing) Then
                        duration = Split(line, ":")(1)
                    Else
                        duration = Nothing
                    End If

                ElseIf line.StartsWith("started"":") Then

                    If Integer.TryParse(Split(line, ":")(1), Nothing) Then
                        started = Split(line, ":")(1)
                    Else
                        started = Nothing
                    End If

                ElseIf line.StartsWith("track"":""") And String.IsNullOrEmpty(started) = False And String.IsNullOrEmpty(duration) = False Then

                    HistoryList.Items.Add(ReturnDate(started, "hourmin", True))
                    HistoryList.Items.Item(HistoryList.Items.Count - 1).SubItems.Add(Split(line, "track"":""")(1))
                    Dim span As TimeSpan
                    span = TimeSpan.FromSeconds(duration)

                    If span.Hours < 1 Then
                        HistoryList.Items.Item(HistoryList.Items.Count - 1).SubItems.Add(String.Format("{0:00}:{1:00}", span.Minutes, span.Seconds))
                    Else
                        HistoryList.Items.Item(HistoryList.Items.Count - 1).SubItems.Add(String.Format("{0:00}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds))
                    End If

                End If

            Loop

            If HistoryList.Items.Count < 20 Then
                If ShowSongStart = False Then
                    Time.Width = 0
                    Title.Width = 276
                ElseIf ShowSongStart = True And New DateTime(2000, 1, 1, 13, 0, 0).ToString.Contains("13") Then
                    Title.Width = 236
                    Time.Width = 40
                Else
                    Title.Width = 226
                    Time.Width = 50
                End If
            Else
                If ShowSongStart = False Then
                    Time.Width = 0
                    Title.Width = 259
                ElseIf ShowSongStart = True And New DateTime(2000, 1, 1, 13, 0, 0).ToString.Contains("13") Then
                    Title.Width = 219
                    Time.Width = 40
                Else
                    Title.Width = 209
                    Time.Width = 50
                End If
            End If

            reader.Close()
            reader.Dispose()

            HistoryList.Enabled = True

            If ChangeWholeBackground = True And BackgroundColour < -8323328 Then
                HistoryList.ForeColor = Color.White
            End If

        Catch
            HistoryList.Items.Clear()
            HistoryList.Items.Add("")
            HistoryList.Items.Item(0).SubItems.Add("Couldn't download history information.")
            HistoryList.Items.Add("")
            HistoryList.Items.Item(1).SubItems.Add("Please go back and try again.")
        End Try
    End Sub

    Private Sub GetEvents_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles GetEvents.DoWork
        Dim channel As String
        ExportButton.Enabled = False
        ExportButton.Text = "Options"

startover:

        If SelectedChannel.Text = "My Favorites" Then
            channel = SelectedServer.Text
        Else
            channel = SelectedChannel.Text
        End If

        SelectedEvent.Enabled = False
        SelectedEvent.Items.Clear()

        SelectedEvent.Items.Add("Please wait, downloading events...")
        SelectedEvent.SelectedIndex = 0


        EventsArray.Items.Clear()
        EventName.Text = Nothing
        EventTagline.Text = Nothing
        EventTimes.Text = Nothing
        EventDescription.Text = Nothing
        Dim WebClient As Net.WebClient = New Net.WebClient()
        Dim EventsLog As String

        Try

            If SelectedChannel.Text = "My Favorites" Then
                EventsLog = WebClient.DownloadString("http://api.audioaddict.com/v1/di/events/channel/" & KeysArray.Items.Item(SelectedServer.Text).Tag)
            Else
                EventsLog = WebClient.DownloadString("http://api.audioaddict.com/v1/di/events/channel/" & KeysArray.Items.Item(SelectedChannel.Text).Tag)
            End If

        Catch
            SelectedEvent.Items.Clear()
            SelectedEvent.Items.Add("Couldn't download events. Please retry.")
            SelectedEvent.SelectedIndex = 0
            ExportButton.Text = "Retry"
            ExportButton.Enabled = True
            Exit Sub
        End Try

        SelectedEvent.Items.RemoveAt(0)

        Dim reader As New IO.StringReader(EventsLog.Replace("{", Nothing).Replace("[", Nothing).Replace("}", Nothing).Replace("]", Nothing).Replace(",""", vbNewLine))

        Dim thistime As Integer
        thistime = (DateTime.Now - New DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds

        Dim id As String = ""
        Dim start_at As Date
        Dim end_at As Date
        Dim name As String = ""
        Dim artists_tagline As String

        Do While (reader.Peek > -1)

            If channel = SelectedChannel.Text = False And channel = SelectedServer.Text = False Then
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

                SelectedEvent.Items.Add(ReturnDate((start_at - New DateTime(1970, 1, 1)).TotalSeconds, "fulldate") & ": " & name)
                EventsArray.Items.Add(name)
                EventsArray.Items.Item(EventsArray.Items.Count - 1).SubItems.Add(artists_tagline)
                EventsArray.Items.Item(EventsArray.Items.Count - 1).SubItems.Add((start_at - New DateTime(1970, 1, 1)).TotalSeconds)
                EventsArray.Items.Item(EventsArray.Items.Count - 1).SubItems.Add((end_at - New DateTime(1970, 1, 1)).TotalSeconds)
                EventsArray.Items.Item(EventsArray.Items.Count - 1).SubItems.Add(id)

            End If

            If thistime > (start_at - New DateTime(1970, 1, 1)).TotalSeconds And thistime < (end_at - New DateTime(1970, 1, 1)).TotalSeconds Then
                SelectedEvent.SelectedIndex = SelectedEvent.Items.Count - 1
            End If

        Loop

        reader.Close()
        reader.Dispose()

        If SelectedEvent.Items.Count > 0 Then
            SelectedEvent.Enabled = True

            If SelectedEvent.Text = "" Then
                SelectedEvent.SelectedIndex = 0
            End If
        Else
            SelectedEvent.Items.Add("There are no future events for this channel.")
            SelectedEvent.SelectedIndex = 0
        End If

        ExportButton.Enabled = True

    End Sub

    Private Sub GetEventDetails_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles GetEventDetails.DoWork
        Dim WebClient As Net.WebClient = New Net.WebClient()
        Dim EventDetails As String
        Dim channel As String
startover:

        channel = SelectedEvent.Text

        Try

            If SelectedEvent.Text.ToLower.StartsWith("there are no") = False And SelectedEvent.Text.ToLower.StartsWith("please wait") = False And SelectedEvent.Text.ToLower.StartsWith("couldn't download") = False Then

                EventDetails = WebClient.DownloadString("http://api.audioaddict.com/v1/di/events/" & EventsArray.Items.Item(SelectedEvent.SelectedIndex).SubItems(4).Text)
                    Dim reader As New IO.StringReader(EventDetails.Replace("{", Nothing).Replace("[", Nothing).Replace("}", Nothing).Replace("]", Nothing).Replace(",""", vbNewLine))

                Do While (reader.Peek > -1)

                    If channel = SelectedEvent.Text = False Then
                        GoTo startover
                    End If

                    Dim line As String = reader.ReadLine

                    If line.StartsWith("description""") Then
                        EventDescription.Text = Split(line, """:""")(1).Replace("\n", vbNewLine).Replace("**", Nothing).Replace("\""", """").Replace("_", Nothing)
                        EventDescription.Text = EventDescription.Text.Remove(EventDescription.Text.Length - 1, 1)
                        Exit Do
                    End If

                Loop

                End If

        Catch
            EventDescription.Text = "Couldn't download event description. Please check http://www.di.fm/calendar/event/" & EventsArray.Items.Item(SelectedEvent.SelectedIndex).SubItems(4).Text & " for more information about this event."
        End Try
    End Sub

    Private Sub CheckForums_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles CheckForums.DoWork
        Dim WebClient As Net.WebClient = New Net.WebClient()

        Try
            NoForumsChannel = WebClient.DownloadString("http://tobiass.eu/files/noforums.txt")
            SelectedChannel_TextChanged(Me, Nothing)
        Catch
        End Try
    End Sub

    ' The following code thanks to _Tobias from the Digitally Imported forums.

    Private Sub DownloadDb_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles DownloadDb.DoWork

        Dim chdb = dataFolder & "servers\" & StationChooser.Text & "\channels.db"
        My.Computer.FileSystem.CreateDirectory(dataFolder & "servers\" & StationChooser.Text)

        Try

            Dim readerChdb As IO.StreamReader = channelDb(chdb)
            SelectedChannel.Items.Clear()

            Do While (readerChdb.Peek > -1)
                Dim line = readerChdb.ReadLine()
                Dim splitter = Split(line, "|")
                SelectedChannel.Items.Add(splitter(2))
                KeysArray.Items.Add(splitter(2))
                KeysArray.Items.Item(KeysArray.Items.Count - 1).Tag = splitter(0)
                KeysArray.Items.Item(KeysArray.Items.Count - 1).Name = splitter(2)
                KeysArray.Items.Item(KeysArray.Items.Count - 1).SubItems.Add(splitter(1))
            Loop

            readerChdb.Close()
            readerChdb.Dispose()
            DownloadingMessage.Hide()

        Catch ex As Exception
            Dim Message As New MsgBoxSafe(AddressOf DisplayMessage)
            Me.Invoke(Message, "Couldn't download channels list", MsgBoxStyle.Exclamation, "Error getting channels list")

            Marquee.Hide()
        End Try

        If ListenKey = Nothing = False And StationChooser.Text = RockRadio.Text = False Then
            SelectedChannel.Items.Add("My Favorites")

            If SelectedChannel.Text = "My Favorites" And OldFav = Nothing = False Then
                Try
                    SelectedServer.SelectedItem = OldFav
                Catch
                End Try
            End If
        End If

    End Sub

    Private Sub DownloadDb_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles DownloadDb.RunWorkerCompleted
        If My.Computer.FileSystem.FileExists(dataFolder & "servers\" & StationChooser.Text & "\channels.db") And SelectedChannel.Items.Count > 1 Then
            If StationChooser.Text = DIFM.Text Then

                SelectedChannel.SelectedItem = DIChannel
                showEvents.Enabled = True
                History.Enabled = True

            ElseIf StationChooser.Text = SKYFM.Text Then

                SelectedChannel.SelectedItem = SKYChannel
                showEvents.Enabled = False
                showEvents.ImageAlign = ContentAlignment.MiddleCenter
                showEvents.Image = My.Resources.events
                EventsPanel.Visible = False
                History.Enabled = True

            ElseIf StationChooser.Text = JazzRadio.Text Then

                SelectedChannel.SelectedItem = JazzChannel
                showEvents.Enabled = False
                showEvents.ImageAlign = ContentAlignment.MiddleCenter
                showEvents.Image = My.Resources.events
                EventsPanel.Visible = False
                History.Enabled = False
                HistoryList.Visible = False
                History.ImageAlign = ContentAlignment.BottomRight
                History.Image = My.Resources.history
                Forums.Enabled = False

            ElseIf StationChooser.Text = RockRadio.Text Then

                SelectedChannel.SelectedItem = RockChannel
                showEvents.Enabled = False
                showEvents.ImageAlign = ContentAlignment.MiddleCenter
                showEvents.Image = My.Resources.events
                EventsPanel.Visible = False
                History.Enabled = False
                HistoryList.Visible = False
                History.ImageAlign = ContentAlignment.BottomRight
                History.Image = My.Resources.history
                Forums.Enabled = False

            End If
        Else
            RetryChannels.Show()
            StationChooser.Enabled = True
        End If

        PlayStop.Focus()

    End Sub

#End Region

#Region "Timers"

    Private Sub FadeOut_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FadeOut.Tick

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

                If VisualisationBox.Image Is Nothing = False Then
                    VisualisationBox.Image.Dispose()
                End If


                Select Case VisualisationType

                    Case 0
                        Drawing.CreateSpectrumBean(stream, g, SpectrumRectangle, Color.FromArgb(MainColour), Color.FromArgb(SecondaryColour), Color.FromArgb(BackgroundColour), 5, LinealRepresentation, FullSoundRange, HighQualityVis)
                    Case 1
                        Drawing.CreateSpectrumDot(stream, g, SpectrumRectangle, Color.FromArgb(MainColour), Color.FromArgb(SecondaryColour), Color.FromArgb(BackgroundColour), 5, 1, LinealRepresentation, FullSoundRange, False)
                    Case 2
                        Drawing.CreateSpectrum(stream, g, SpectrumRectangle, Color.FromArgb(MainColour), Color.FromArgb(SecondaryColour), Color.FromArgb(BackgroundColour), LinealRepresentation, FullSoundRange, False)
                    Case 3
                        Drawing.CreateSpectrumEllipse(stream, g, SpectrumRectangle, Color.FromArgb(MainColour), Color.FromArgb(SecondaryColour), Color.FromArgb(BackgroundColour), 3, 1, LinealRepresentation, FullSoundRange, HighQualityVis)
                    Case 4
                        Drawing.CreateSpectrumLine(stream, g, SpectrumRectangle, Color.FromArgb(MainColour), Color.FromArgb(SecondaryColour), Color.FromArgb(BackgroundColour), 5, 1, LinealRepresentation, FullSoundRange, False)
                    Case 5
                        Drawing.CreateSpectrumLinePeak(stream, g, SpectrumRectangle, Color.FromArgb(MainColour), Color.FromArgb(SecondaryColour), Color.FromArgb(PeakColour), Color.FromArgb(BackgroundColour), 5, 5, 1, 100, LinealRepresentation, FullSoundRange, False)
                    Case 6
                        Drawing.CreateSpectrumWave(stream, g, SpectrumRectangle, Color.FromArgb(MainColour), Color.FromArgb(SecondaryColour), Color.FromArgb(BackgroundColour), 5, LinealRepresentation, FullSoundRange, HighQualityVis)
                    Case 7
                        Drawing.CreateWaveForm(stream, g, SpectrumRectangle, Color.FromArgb(MainColour), Color.FromArgb(SecondaryColour), Color.FromArgb(BackgroundColour), Color.FromArgb(BackgroundColour), 5, FullSoundRange, False, HighQualityVis)
                End Select

            End Using

            VisualisationBox.Image = SpectrumImage
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

    Private Sub TrayMenu_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles TrayMenu.Opening
        PlayStopTray.Enabled = PlayStop.Enabled
        PlayStopTray.Text = PlayStop.Tag
        PlayStopTray.Image = PlayStop.Image
        CalendarTray.Enabled = showEvents.Enabled
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
                Player_Resize(Me, Nothing)
            End If
        End If

    End Sub

    Private Sub CopyTitleMenu_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles CopyTitleMenu.Opening
        If RadioString.Text = "" OrElse RadioString.Text.Contains("Connection is taking some time") OrElse RadioString.Text.Contains("Lost connection to") OrElse RadioString.Text.Contains("Connecting, please wait...") OrElse RadioString.Text.Contains("Couldn't connect to") OrElse RadioString.Text.Contains("Internet connection") Then
            CopyToolStripMenuItem.Enabled = False
            GoogleSearchToolStripMenuItem.Enabled = False
        Else
            CopyToolStripMenuItem.Enabled = True
            GoogleSearchToolStripMenuItem.Enabled = True
        End If
    End Sub

    Private Sub ExitTray_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitTray.Click
        Me.Close()
    End Sub

    Private Sub CopyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyToolStripMenuItem.Click
        Clipboard.Clear()
        Clipboard.SetDataObject(RadioString.Text)
    End Sub

    Private Sub TrackHistoryTray_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackHistoryTray.Click
        If Me.WindowState = FormWindowState.Minimized And TrayIcon.Visible = True Then
            TrayIcon_MouseDoubleClick(Me, Nothing)
        Else
            Me.WindowState = FormWindowState.Normal
        End If

        If HistoryList.Visible = False Then
            History_Click(Me, Nothing)
        End If
    End Sub

    Private Sub CalendarTray_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CalendarTray.Click

        If Me.WindowState = FormWindowState.Minimized And TrayIcon.Visible = True Then
            TrayIcon_MouseDoubleClick(Me, Nothing)
        Else
            Me.WindowState = FormWindowState.Normal
        End If

        If EventsPanel.Visible = False Then
            showEvents_Click(Me, Nothing)
        End If

    End Sub

    Private Sub CopyHistoryMenu_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles CopyHistoryMenu.Opening
        If HistoryList.SelectedItems.Count > 0 Then
            CopyHistory.Enabled = True
            GoogleHistory.Enabled = True
        Else
            CopyHistory.Enabled = False
            GoogleHistory.Enabled = False
        End If
    End Sub

    Private Sub CopyHistory_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyHistory.Click
        Clipboard.Clear()
        Clipboard.SetDataObject(HistoryList.SelectedItems.Item(0).SubItems(1).Text)
    End Sub

    Private Sub GoogleHistory_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GoogleHistory.Click
        Process.Start("https://www.google.com/search?q=" & HistoryList.SelectedItems.Item(0).SubItems(1).Text.Replace("&", "%26"))
    End Sub

    ' These only call to the event function of their respective main form button counterparts

    Private Sub PlayStopTray_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PlayStopTray.Click
        PlayStop_Click(sender, e)
    End Sub

    Private Sub MuteTray_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MuteTray.Click
        Mute_Click(sender, e)
    End Sub

    Private Sub CopyTitleTray_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyTitleTray.Click
        CopyToolStripMenuItem_Click(sender, e)
    End Sub

    Private Sub ForumsTray_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ForumsTray.Click
        Forums_Click(sender, e)
    End Sub

    Private Sub OptionsTray_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OptionsTray.Click
        OptionsButton_Click(sender, e)
    End Sub

    Private Sub GoogleSearchTray_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GoogleSearchTray.Click
        GoogleSearchToolStripMenuItem_Click(Me, Nothing)
    End Sub

    ' ----------------------------------------------------------------------------------------

    Private Sub CopyServerURLToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyServerURLToolStripMenuItem.Click
        Clipboard.Clear()

        If ListenKey = Nothing = False And removeKey Then
            Clipboard.SetDataObject(ServersArray.Items.Item(SelectedServer.SelectedIndex).Text.Replace(ListenKey, "*listen key removed*"))
        Else
            Clipboard.SetDataObject(ServersArray.Items.Item(SelectedServer.SelectedIndex).Text)
        End If
    End Sub

    Private Sub GoogleSearchToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GoogleSearchToolStripMenuItem.Click
        Process.Start("https://www.google.com/search?q=" & RadioString.Text.Replace("&", "%26").Replace("+", "%2B"))
    End Sub

    Private Sub DIFM_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DIFM.Click
        StationChooser.Image = DIFM.Image
        StationChooser.Tag = "di.fm"
        StationChooser.Text = DIFM.Text
        StationChooser.ToolTipText = DIFM.Text
        Options.radioStation = DIFM.Text
        Me.Text = Me.Text.Replace("JazzRadio", "DI")
        Me.Text = Me.Text.Replace("SKY.FM", "DI")
        Me.Text = Me.Text.Replace("RockRadio", "DI")
    End Sub

    Private Sub JazzRadio_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles JazzRadio.Click
        StationChooser.Image = JazzRadio.Image
        StationChooser.Tag = "jazzradio.com"
        StationChooser.Text = JazzRadio.Text
        StationChooser.ToolTipText = JazzRadio.Text
        Options.radioStation = JazzRadio.Text
        Me.Text = Me.Text.Replace("DI", "JazzRadio")
        Me.Text = Me.Text.Replace("SKY.FM", "JazzRadio")
        Me.Text = Me.Text.Replace("RockRadio", "JazzRadio")
    End Sub

    Private Sub SKYFM_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SKYFM.Click
        StationChooser.Image = SKYFM.Image
        StationChooser.Tag = "sky.fm"
        StationChooser.Text = SKYFM.Text
        StationChooser.ToolTipText = SKYFM.Text
        Options.radioStation = DIFM.Text
        Me.Text = Me.Text.Replace("DI", "SKY.FM")
        Me.Text = Me.Text.Replace("JazzRadio", "SKY.FM")
        Me.Text = Me.Text.Replace("RockRadio", "SKY.FM")
    End Sub

    Private Sub RockRadio_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RockRadio.Click
        StationChooser.Image = RockRadio.Image
        StationChooser.Tag = "rockradio.com"
        StationChooser.Text = RockRadio.Text
        StationChooser.ToolTipText = RockRadio.Text
        Options.radioStation = RockRadio.Text
        Me.Text = Me.Text.Replace("JazzRadio", "RockRadio")
        Me.Text = Me.Text.Replace("SKY.FM", "RockRadio")
        Me.Text = Me.Text.Replace("DI", "RockRadio")
    End Sub

    Private Sub ExportToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExportToolStripMenuItem.Click
        Export.Location = New Point(Me.Location.X, Me.Location.Y)
        Export.Show()
        Export.BringToFront()
    End Sub

    Private Sub FacebookToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FacebookToolStripMenuItem.Click
        Process.Start("https://www.facebook.com/dialog/feed?display=page&show_error=true&link=http%3A%2F%2Fwww.di.fm%2Fcalendar%2Fevent%2F" & EventsArray.Items.Item(SelectedEvent.SelectedIndex).SubItems(4).Text & "&e2e=%7B%7D&app_id=109375636874&locale=en_US&sdk=joey&next=http%3A%2F%2Ffacebook.com")
    End Sub

    Private Sub TwitterToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TwitterToolStripMenuItem.Click
        Dim channel As String
        If SelectedChannel.Text = "My Favorites" Then
            channel = SelectedServer.Text
        Else
            channel = SelectedChannel.Text
        End If

        Process.Start("https://twitter.com/intent/tweet?text=I'll%20be%20tuning%20in%20to%20'" & EventName.Text.Replace(" ", "%20") & "'%20on%20the%20" & channel & "%20channel%20at%20Digitally%20Imported%2C%20check%20it%20out%3A%20&url=http%3A%2F%2Fwww.di.fm%2Fcalendar%2Fevent%2F" & EventsArray.Items.Item(SelectedEvent.SelectedIndex).SubItems(4).Text)
    End Sub

    Private Sub EmailToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EmailToolStripMenuItem.Click

        Dim channel As String
        If SelectedChannel.Text = "My Favorites" Then
            channel = SelectedServer.Text
        Else
            channel = SelectedChannel.Text
        End If

        Try
            Process.Start("mailto:?to=&subject=Check%20out%20'" & EventName.Text.Replace(" ", "%20") & "'%20on%20Digitally%20Imported&body=I%20will%20be%20tuning%20in%20to%20" & channel & "%20on%20Digitally%20Imported%20(http://www.di.fm/" & KeysArray.Items.Item(channel).SubItems(1).Text & ")%20to%20listen%20to%20'" & EventName.Text & "%20',%20and%20I%20thought%20you'd%20enjoy%20it%20too.%0ACheck it out here: http://www.di.fm/calendar/event/" & EventsArray.Items.Item(SelectedEvent.SelectedIndex).SubItems(4).Text)
        Catch ex As Exception
            MessageBox.Show("It appears you don't have a default e-mail program configured." & vbNewLine & "Please set a program as your default e-mail handler and try again.", "Couldn't find e-mail program", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try


    End Sub

    Private Sub eventOptionsMenu_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles eventOptionsMenu.Opening
        ShareToolStripMenuItem.Enabled = SelectedEvent.Enabled
    End Sub

    Private Sub shareChannelFB_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles shareChannelFB.Click
        Dim channel As String

        If SelectedChannel.Text = "My Favorites" Then
            channel = SelectedServer.Text
        Else
            channel = SelectedChannel.Text
        End If

        Process.Start("https://www.facebook.com/dialog/feed?display=page&show_error=true&link=http%3A%2F%2Fwww.di.fm%2F" & KeysArray.Items.Item(channel).SubItems(1).Text & "&e2e=%7B%7D&app_id=109375636874&locale=en_US&sdk=joey&next=http%3A%2F%2Ffacebook.com")
    End Sub

    Private Sub shareChannelTT_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles shareChannelTT.Click
        Dim channel As String

        If SelectedChannel.Text = "My Favorites" Then
            channel = SelectedServer.Text
        Else
            channel = SelectedChannel.Text
        End If

        Process.Start("https://twitter.com/intent/tweet?text=I'm%20enjoying%20the%20" & channel.Replace(" ", "%20") & "%20channel%20at%20%40diradio&url=http%3A%2F%2Fwww.di.fm%2F" & KeysArray.Items.Item(channel).SubItems(1).Text)
    End Sub

    Private Sub shareChannelEM_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles shareChannelEM.Click
        Dim channel As String

        If SelectedChannel.Text = "My Favorites" Then
            channel = SelectedServer.Text
        Else
            channel = SelectedChannel.Text
        End If

        Try
            Process.Start("mailto:?to=&subject=Check%20out%20the%20" & channel.Replace(" ", "%20") & "%20channel%20on%20Digitally%20Imported&body=I%20just%20heard%20an%20amazing%20song%20on%20the%20" & channel.Replace(" ", "%20") & "%20channel%20at%20Digitally%20Imported:%20http://www.di.fm/" & KeysArray.Items.Item(channel).SubItems(1).Text & ".%0AIt%20was%20so%20good%20that%20I%20had%20to%20tell%20you%20about%20it.")
        Catch ex As Exception
            MessageBox.Show("It appears you don't have a default e-mail program configured." & vbNewLine & "Please set a program as your default e-mail handler and try again.", "Couldn't find e-mail program", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try

    End Sub

    Private Sub shareMenu_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles shareMenu.Opening
        If StationChooser.Tag = DIFM.Tag Then
            shareChannelFB.Enabled = True
            shareChannelTT.Enabled = True
            shareChannelEM.Enabled = True
        Else
            shareChannelFB.Enabled = False
            shareChannelTT.Enabled = False
            shareChannelEM.Enabled = False
        End If
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

        Dim file As String = dataFolder & "options.ini"

        ' If the options file doesn't exist, create it with some default values

        If My.Computer.FileSystem.FileExists(file) = False Then
            Dim writer As New IO.StreamWriter(file)
            writer.WriteLine(Options.NotificationTitle.Name & "=True")
            writer.WriteLine(Options.PlayNewOnChannelChange.Name & "=False")
            writer.WriteLine(Options.NotificationIcon.Name & "=True")
            writer.WriteLine(Options.NoTaskbarButton.Name & "=False")
            writer.WriteLine(Options.GoogleSearch.Name & "=True")
            writer.WriteLine(Options.ShowSongStart.Name & "=False")
            writer.WriteLine(Options.removeListenKey.Name & "=True")
            writer.WriteLine(Options.cacheList.Name & "=Every day")
            writer.WriteLine("username=")
            writer.WriteLine("password=")
            writer.WriteLine("userinfo=")
            writer.WriteLine("isLogged=False")
            writer.WriteLine("isPremium=False")
            writer.WriteLine("canTrial=False")
            writer.WriteLine("apiKey=")
            writer.WriteLine("userId=")
            writer.WriteLine(Options.ListenKey.Name & "=")
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
            writer.WriteLine("DIChannel=Ambient")
            writer.WriteLine("SkyChannel=80's Rock Hits")
            writer.WriteLine("JazzChannel=Avant-Garde")
            writer.WriteLine("RockChannel=80's Alternative")
            writer.WriteLine(Volume.Name & "=50")
            writer.WriteLine(SelectedServer.Name & "=0")
            writer.Close()
            writer.Dispose()
        End If

        Dim reader As New IO.StreamReader(file)

        Try


            Do While (reader.Peek > -1)
                Dim whole As String = reader.ReadLine
                Dim splitter() As String = Split(whole, "=")

                If splitter(0) = Options.NotificationTitle.Name And Boolean.TryParse(splitter(1), Nothing) Then
                    NotificationTitle = splitter(1)
                ElseIf splitter(0) = Options.PlayNewOnChannelChange.Name And Boolean.TryParse(splitter(1), Nothing) Then
                    PlayNewOnChannelChange = splitter(1)
                ElseIf splitter(0) = Options.NotificationIcon.Name And Boolean.TryParse(splitter(1), Nothing) Then
                    NotificationIcon = splitter(1)
                ElseIf splitter(0) = Options.NoTaskbarButton.Name Then
                    If Boolean.TryParse(splitter(1), Nothing) Then
                        NoTaskbarButton = splitter(1)
                    End If

                    If NoTaskbarButton = False Then
                        Me.ShowInTaskbar = True

                        If Me.WindowState = FormWindowState.Normal Then
                            TrayIcon.Visible = False
                        End If
                    Else
                        Me.ShowInTaskbar = False
                        TrayIcon.Visible = True
                    End If
                ElseIf splitter(0) = Options.GoogleSearch.Name And Boolean.TryParse(splitter(1), Nothing) Then
                    GoogleSearch = splitter(1)
                ElseIf splitter(0) = Options.ShowSongStart.Name Then
                    If Boolean.TryParse(splitter(1), Nothing) Then
                        ShowSongStart = splitter(1)
                    End If

                    If ShowSongStart = False Then
                        Time.Width = 0
                        Title.Width = 255
                    End If
                ElseIf splitter(0) = Options.removeListenKey.Name And Boolean.TryParse(splitter(1), Nothing) Then
                    removeKey = splitter(1)
                ElseIf splitter(0) = Options.cacheList.Name Then

                    cacheList = splitter(1)

                ElseIf splitter(0) = "isLogged" And Boolean.TryParse(splitter(1), Nothing) Then

                    isLogged = splitter(1)

                ElseIf splitter(0) = "isPremium" And Boolean.TryParse(splitter(1), Nothing) And isLogged Then

                    isPremium = splitter(1)

                ElseIf splitter(0) = Options.ListenKey.Name And isLogged Then

                    ListenKey = SimpleCrypt(splitter(1))

                ElseIf splitter(0) = "username" And isLogged Then

                    username = SimpleCrypt(splitter(1))

                ElseIf splitter(0) = "password" And isLogged Then

                    password = SimpleCrypt(splitter(1))

                ElseIf splitter(0) = "userinfo" And isLogged = True Then

                    userInfo = SimpleCrypt(splitter(1))

                ElseIf splitter(0) = "canTrial" And isLogged Then

                    canTrial = splitter(1)

                ElseIf splitter(0) = "apiKey" And isLogged Then

                    apiKey = SimpleCrypt(splitter(1))

                ElseIf splitter(0) = "userId" And isLogged Then

                    userId = SimpleCrypt(splitter(1))

                ElseIf splitter(0) = "DIFormat" And Integer.TryParse(splitter(1), Nothing) Then

                    If isPremium And splitter(1) <= 3 Then
                        DIFormat = splitter(1)
                    ElseIf splitter(1) <= 2 And isPremium = False Then
                        DIFormat = splitter(1)
                    Else
                        DIFormat = 1

                        If splitter(1) > 3 And My.Computer.FileSystem.DirectoryExists(dataFolder & "servers\Digitally Imported") Then
                            My.Computer.FileSystem.DeleteDirectory(dataFolder & "servers\Digitally Imported", FileIO.DeleteDirectoryOption.DeleteAllContents)
                        End If
                    End If


                ElseIf splitter(0) = "SKYFormat" And Integer.TryParse(splitter(1), Nothing) Then

                    If isPremium And splitter(1) <= 5 Then
                        SKYFormat = splitter(1)
                    ElseIf splitter(1) <= 2 And isPremium = False Then
                        SKYFormat = splitter(1)
                    Else
                        If isPremium Then
                            SKYFormat = 1
                        Else
                            SKYFormat = 0
                        End If

                        If splitter(1) > 5 And My.Computer.FileSystem.DirectoryExists(dataFolder & "servers\SKY.FM") Then
                            My.Computer.FileSystem.DeleteDirectory(dataFolder & "servers\SKY.FM", FileIO.DeleteDirectoryOption.DeleteAllContents)
                        End If
                    End If


                ElseIf splitter(0) = "JazzFormat" And Integer.TryParse(splitter(1), Nothing) Then

                    If isPremium And splitter(1) <= 3 Then
                        JazzFormat = splitter(1)
                    ElseIf splitter(1) <= 2 And isPremium = False Then
                        JazzFormat = splitter(1)
                    Else
                        If isPremium Then
                            JazzFormat = 1
                        Else
                            JazzFormat = 0
                        End If

                        If splitter(1) > 5 And My.Computer.FileSystem.DirectoryExists(dataFolder & "servers\JazzRadio") Then
                            My.Computer.FileSystem.DeleteDirectory(dataFolder & "servers\JazzRadio", FileIO.DeleteDirectoryOption.DeleteAllContents)
                        End If
                    End If


                ElseIf splitter(0) = Options.BetaVersions.Name And Boolean.TryParse(splitter(1), Nothing) Then
                    BetaVersions = splitter(1)
                ElseIf splitter(0) = Options.UpdatesAtStart.Name Then

                    If Boolean.TryParse(splitter(1), Nothing) Then
                        UpdatesAtStart = splitter(1)
                    End If

                    If UpdatesAtStart = True Then
                        AtStartup = True
                        GetUpdates.RunWorkerAsync()
                    End If

                ElseIf splitter(0) = Options.Visualisation.Name Then

                    If Boolean.TryParse(splitter(1), Nothing) Then
                        Visualisation = splitter(1)
                    End If

                    If Visualisation = True Then

                        VisualisationBox.Show()
                        Me.Size = New Size(Me.MaximumSize)

                    Else

                        VisualisationBox.Hide()
                        Me.Size = New Size(Me.MinimumSize)

                    End If

                ElseIf splitter(0) = Options.VisualisationType.Name And Integer.TryParse(splitter(1), Nothing) Then
                    VisualisationType = splitter(1)
                ElseIf splitter(0) = Options.HighQualityVis.Name And Boolean.TryParse(splitter(1), Nothing) Then
                    HighQualityVis = splitter(1)
                ElseIf splitter(0) = Options.LinealRepresentation.Name And Boolean.TryParse(splitter(1), Nothing) Then
                    LinealRepresentation = splitter(1)
                ElseIf splitter(0) = Options.FullSoundRange.Name And Boolean.TryParse(splitter(1), Nothing) Then
                    FullSoundRange = splitter(1)
                ElseIf splitter(0) = Options.Smoothness.Name And Integer.TryParse(splitter(1), Nothing) Then
                    Smoothness = splitter(1)
                ElseIf splitter(0) = Options.MainColour.Name And Integer.TryParse(splitter(1), Nothing) Then
                    MainColour = splitter(1)
                ElseIf splitter(0) = Options.SecondaryColour.Name And Integer.TryParse(splitter(1), Nothing) Then
                    SecondaryColour = splitter(1)
                ElseIf splitter(0) = Options.PeakColour.Name And Integer.TryParse(splitter(1), Nothing) Then
                    PeakColour = splitter(1)
                ElseIf splitter(0) = Options.BackgroundColour.Name And Integer.TryParse(splitter(1), Nothing) Then
                    BackgroundColour = splitter(1)
                ElseIf splitter(0) = Options.ChangeWholeBackground.Name Then

                    If Boolean.TryParse(splitter(1), Nothing) Then
                        ChangeWholeBackground = splitter(1)
                    End If

                    ApplyTheme()

                ElseIf splitter(0) = Options.MultimediaKeys.Name And Boolean.TryParse(splitter(1), Nothing) Then
                    MultimediaKeys = splitter(1)
                ElseIf splitter(0) = Options.HotkeyPlayStop.Name And Integer.TryParse(splitter(1), Nothing) Then
                    HumanModifiersPlayStop = splitter(1)

                    If Integer.TryParse(splitter(2), Nothing) Then
                        ModifiersPlayStop = splitter(2)
                    End If

                    If Integer.TryParse(splitter(2), Nothing) Then
                        KeyPlayStop = splitter(3)
                    End If

                ElseIf splitter(0) = Options.HotkeyVolumeUp.Name And Integer.TryParse(splitter(1), Nothing) Then
                    HumanModifiersVolumeUp = splitter(1)

                    If Integer.TryParse(splitter(2), Nothing) Then
                        ModifiersVolumeUp = splitter(2)
                    End If

                    If Integer.TryParse(splitter(3), Nothing) Then
                        KeyVolumeUp = splitter(3)
                    End If

                ElseIf splitter(0) = Options.HotkeyVolumeDown.Name And Integer.TryParse(splitter(1), Nothing) Then
                    HumanModifiersVolumeDown = splitter(1)
                    If Integer.TryParse(splitter(2), Nothing) Then
                        ModifiersVolumeDown = splitter(2)
                    End If

                    If Integer.TryParse(splitter(3), Nothing) Then
                        KeyVolumeDown = splitter(3)
                    End If

                ElseIf splitter(0) = Options.HotkeyMuteUnmute.Name And Integer.TryParse(splitter(1), Nothing) Then
                    HumanModifiersMuteUnmute = splitter(1)

                    If Integer.TryParse(splitter(2), Nothing) Then
                        ModifiersMuteUnmute = splitter(2)
                    End If

                    If Integer.TryParse(splitter(3), Nothing) Then
                        KeyMuteUnmute = splitter(3)
                    End If

                ElseIf splitter(0) = Options.HotkeyShowHide.Name And Integer.TryParse(splitter(1), Nothing) Then
                    HumanModifiersShowHide = splitter(1)

                    If Integer.TryParse(splitter(2), Nothing) Then
                        ModifiersShowHide = splitter(2)
                    End If

                    If Integer.TryParse(splitter(3), Nothing) Then
                        KeyShowHide = splitter(3)
                    End If

                ElseIf splitter(0) = Options.Band0.Name And Integer.TryParse(splitter(1), Nothing) Then
                    Band0 = splitter(1)
                ElseIf splitter(0) = Options.Band1.Name And Integer.TryParse(splitter(1), Nothing) Then
                    Band1 = splitter(1)
                ElseIf splitter(0) = Options.Band2.Name And Integer.TryParse(splitter(1), Nothing) Then
                    Band2 = splitter(1)
                ElseIf splitter(0) = Options.Band3.Name And Integer.TryParse(splitter(1), Nothing) Then
                    Band3 = splitter(1)
                ElseIf splitter(0) = Options.Band4.Name And Integer.TryParse(splitter(1), Nothing) Then
                    Band4 = splitter(1)
                ElseIf splitter(0) = Options.Band5.Name And Integer.TryParse(splitter(1), Nothing) Then
                    Band5 = splitter(1)
                ElseIf splitter(0) = StationChooser.Name Then

                    If splitter(1) = "Digitally Imported" OrElse splitter(1) = "SKY.FM" OrElse splitter(1) = "JazzRadio" OrElse splitter(1) = "RockRadio" Then
                        RadioStation = splitter(1)
                    End If

                ElseIf splitter(0) = "DIChannel" Then

                    If IsNumeric(splitter(1)) Then
                        DIChannel = "Ambient"
                    Else
                        DIChannel = splitter(1)
                    End If

                ElseIf splitter(0) = "SkyChannel" Then

                    If IsNumeric(splitter(1)) Then
                        SKYChannel = "80's Rock Hits"
                    Else
                        SKYChannel = splitter(1)
                    End If

                ElseIf splitter(0) = "JazzChannel" Then

                    If IsNumeric(splitter(1)) Then
                        JazzChannel = "Avant-Garde"
                    Else
                        JazzChannel = splitter(1)
                    End If

                ElseIf splitter(0) = "RockChannel" Then

                    If IsNumeric(splitter(1)) Then
                        RockChannel = "80's Alternative"
                    Else
                        RockChannel = splitter(1)
                    End If

                ElseIf splitter(0) = Volume.Name And IsNumeric(splitter(1)) Then
                    Volume.Value = splitter(1)
                ElseIf splitter(0) = SelectedServer.Name Then
                    OldFav = splitter(1)
                End If


            Loop

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

            MessageBox.Show("There was an error trying to load your options file. Please check the Options panel.", "Error loading options", MessageBoxButtons.OK, MessageBoxIcon.Information)

        End Try

        reader.Close()
        reader.Dispose()

        Me.CenterToScreen()
        RadioString.Text = Greetings(userInfo)
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

    Public Sub UpdateEq(ByVal band As Integer, ByVal gain As Single)
        Dim Eq As New BASS_DX8_PARAMEQ()

        If Bass.BASS_FXGetParameters(EqBands(band), Eq) Then
            Eq.fGain = gain
            Bass.BASS_FXSetParameters(EqBands(band), Eq)
        End If
    End Sub

    Public Function ReturnDate(ByVal seconds As String, ByVal datetype As String, Optional ByVal toLocal As Boolean = False)
        Dim firstDay As DateTime = #1/1/1970#
        Dim time As DateTime = firstDay.AddSeconds(seconds)

        If toLocal Then
            time = time.ToLocalTime()
        End If

        Dim numeral As String
        Dim datestring As String = ""

        If time.Day.ToString.EndsWith("1") And time.Day < 11 Then
            numeral = "st"
        ElseIf time.Day.ToString.EndsWith("2") And time.Day < 12 Then
            numeral = "nd"
        ElseIf time.Day.ToString.EndsWith("3") And time.Day < 13 Then
            numeral = "rd"
        Else
            numeral = "th"
        End If

        Dim ampm As String = ""
        Dim hour As String

        If New DateTime(2000, 1, 1, 13, 0, 0).ToString.Contains("13") = False Then
            If time.Hour >= 12 Then
                ampm = "pm"
                If time.Hour = 12 Then
                    hour = time.Hour
                Else
                    hour = time.Hour - 12
                End If
            Else
                ampm = "am"
                hour = time.Hour

                If hour = "0" Then
                    hour = "12"
                End If
            End If
        Else
            hour = time.Hour
        End If

        If datetype = "fulldate" Then
            datestring = time.DayOfWeek.ToString.Remove(3) & ". " & time.Day & numeral & " - " & String.Format("{0:00}:{1:00}" & ampm, hour, time.Minute)
        ElseIf datetype = "hourmin" Then
            datestring = String.Format("{0:00}:{1:00}" & ampm, hour, time.Minute)
        End If

        Return datestring
    End Function

    Sub ResumePlaying()
        RestartPlayback = False
        PlayStop_Click(Me, Nothing)
    End Sub

    Public Sub ApplyTheme()

        If ChangeWholeBackground = True Then

            Me.BackColor = Color.FromArgb(BackgroundColour)
            ToolStrip1.BackColor = Color.FromArgb(BackgroundColour)
            StationChooser.BackColor = Color.FromArgb(BackgroundColour)
            Label1.BackColor = Color.FromArgb(BackgroundColour)
            Label2.BackColor = Color.FromArgb(BackgroundColour)
            Label3.BackColor = Color.FromArgb(BackgroundColour)
            Label4.BackColor = Color.FromArgb(BackgroundColour)
            EventDescription.BackColor = Color.FromArgb(BackgroundColour)
            HistoryList.BackColor = Color.FromArgb(BackgroundColour)
            TimerString.BackColor = Color.FromArgb(BackgroundColour)

            If BackgroundColour < -8323328 Then
                RadioString.ForeColor = Color.White
                TimerString.ForeColor = Color.White
                EventName.ForeColor = Color.White
                EventDescription.ForeColor = Color.White
                EventTimes.ForeColor = Color.White
                EventTagline.ForeColor = Color.White
                HistoryList.ForeColor = Color.White
            Else
                RadioString.ForeColor = Color.Black
                TimerString.ForeColor = Color.Black
                EventName.ForeColor = Color.Black
                EventDescription.ForeColor = Color.Black
                EventTimes.ForeColor = Color.Black
                EventTagline.ForeColor = Color.Black
                HistoryList.ForeColor = Color.Black
            End If

            If BackgroundColour < -7105537 Then
                EditFavorites.LinkColor = Color.White
                RefreshFavorites.LinkColor = Color.White
            Else
                EditFavorites.LinkColor = Color.Blue
                RefreshFavorites.LinkColor = Color.Blue
            End If

            If RadioString.Text.ToLower.StartsWith("lost connection to") = False And RadioString.Text.ToLower.StartsWith("couldn't connect to") = False And RadioString.Text.ToLower.StartsWith("connection is taking") = False Then
                RadioString.BackColor = Color.FromArgb(BackgroundColour)

                If BackgroundColour < -8323328 Then
                    RadioString.ForeColor = Color.White
                    TimerString.ForeColor = Color.White
                Else
                    RadioString.ForeColor = Color.Black
                    TimerString.ForeColor = Color.Black
                End If

            End If
        Else

            BackColor = SystemColors.Control
            ToolStrip1.BackColor = SystemColors.Control
            StationChooser.BackColor = SystemColors.Control
            Label1.BackColor = SystemColors.Control
            Label2.BackColor = SystemColors.Control
            Label3.BackColor = SystemColors.Control
            Label4.BackColor = SystemColors.Control
            EventDescription.BackColor = SystemColors.Control
            EventName.ForeColor = SystemColors.ControlText
            EventDescription.ForeColor = SystemColors.ControlText
            EventTimes.ForeColor = SystemColors.ControlText
            EventTagline.ForeColor = SystemColors.ControlText
            HistoryList.BackColor = SystemColors.Window
            HistoryList.ForeColor = SystemColors.ControlText
            EditFavorites.LinkColor = Color.Blue
            RefreshFavorites.LinkColor = Color.Blue
            TimerString.BackColor = SystemColors.Control

            If RadioString.Text.ToLower.StartsWith("internet connection") = False And RadioString.Text.ToLower.StartsWith("lost connection to") = False And RadioString.Text.ToLower.StartsWith("couldn't connect to") = False And RadioString.Text.ToLower.StartsWith("connection is taking") = False Then
                RadioString.BackColor = SystemColors.Control
                RadioString.ForeColor = SystemColors.ControlText
                TimerString.ForeColor = SystemColors.ControlText
            End If

        End If

    End Sub

    Public Sub ReadThemeFile(ByVal path As String)
        Dim reader As New IO.StreamReader(path)
        Dim lineNumber As Byte = 0

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
    End Sub

    Public Function SaveSettings(ByVal closing As Boolean)
        Dim file As String = dataFolder & "options.ini"

        Try

            Dim writer As New IO.StreamWriter(file, False)
            writer.WriteLine(Options.NotificationTitle.Name & "=" & NotificationTitle)
            writer.WriteLine(Options.PlayNewOnChannelChange.Name & "=" & PlayNewOnChannelChange)
            writer.WriteLine(Options.NotificationIcon.Name & "=" & NotificationIcon)
            writer.WriteLine(Options.NoTaskbarButton.Name & "=" & NoTaskbarButton)
            writer.WriteLine(Options.GoogleSearch.Name & "=" & GoogleSearch)
            writer.WriteLine(Options.ShowSongStart.Name & "=" & ShowSongStart)
            writer.WriteLine(Options.removeListenKey.Name & "=" & removeKey)
            writer.WriteLine(Options.cacheList.Name & "=" & cacheList)
            writer.WriteLine("isLogged=" & isLogged)
            writer.WriteLine("username=" & SimpleCrypt(username))
            writer.WriteLine("password=" & SimpleCrypt(password))
            writer.WriteLine("userinfo=" & SimpleCrypt(userInfo))
            writer.WriteLine("isPremium=" & isPremium)
            writer.WriteLine("canTrial=" & canTrial)
            writer.WriteLine("apiKey=" & SimpleCrypt(apiKey))
            writer.WriteLine("userId=" & SimpleCrypt(userId))
            writer.WriteLine(Options.ListenKey.Name & "=" & SimpleCrypt(ListenKey))
            writer.WriteLine("DIFormat=" & DIFormat)
            writer.WriteLine("SKYFormat=" & SKYFormat)
            writer.WriteLine("JazzFormat=" & JazzFormat)
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

            If closing = True Then
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
            End If

            writer.Close()
            writer.Dispose()

            Return True

        Catch ex As Exception

            ' If the options file couldn't be written, return an error message
            Return ex.Message

        End Try

    End Function

    Public Sub LoadEqFile(ByVal path As String)

        Dim reader As New IO.StreamReader(path)
        Dim band As Byte = 0

        Do While (reader.Peek > -1)
            Dim bandValue As String = reader.ReadLine

            If band = 1 Then

                If bandValue > 15 Then
                    Band0 = 15
                ElseIf bandValue < -15 Then
                    Band0 = -15
                Else
                    Band0 = bandValue
                End If

            ElseIf band = 3 Then

                If bandValue > 15 Then
                    Band1 = 15
                ElseIf bandValue < -15 Then
                    Band1 = -15
                Else
                    Band1 = bandValue
                End If

            ElseIf band = 7 Then

                If bandValue > 15 Then
                    Band2 = 15
                ElseIf bandValue < -15 Then
                    Band2 = -15
                Else
                    Band2 = bandValue
                End If

            ElseIf band = 10 Then

                If bandValue > 15 Then
                    Band3 = 15
                ElseIf bandValue < -15 Then
                    Band3 = -15
                Else
                    Band3 = bandValue
                End If

            ElseIf band = 13 Then

                If bandValue > 15 Then
                    Band4 = 15
                ElseIf bandValue < -15 Then
                    Band4 = -15
                Else
                    Band4 = bandValue
                End If

            ElseIf band = 16 Then

                If bandValue > 15 Then
                    Band5 = 15
                ElseIf bandValue < -15 Then
                    Band5 = -15
                Else
                    Band5 = bandValue
                End If

            End If

            band += 1
        Loop

        reader.Close()
        reader.Dispose()

        If Options.Visible = True Then
            Options.Band0.Value = Band0
            Options.Band1.Value = Band1
            Options.Band2.Value = Band2
            Options.Band3.Value = Band3
            Options.Band4.Value = Band4
            Options.Band5.Value = Band5
        End If

        If PlayNewOnChannelChange = True Then
            SetUpEq()
            UpdateEq(0, Band0)
            UpdateEq(1, Band1)
            UpdateEq(2, Band2)
            UpdateEq(3, Band3)
            UpdateEq(4, Band4)
            UpdateEq(5, Band5)
        End If

    End Sub

    Public Function SimpleCrypt(ByVal Text As String) As String
        ' Encrypts/decrypts the passed string using 
        ' a simple ASCII value-swapping algorithm
        ' Code by Karl Moore

        Dim strTempChar As String = "", i As Integer
        For i = 1 To Len(Text)
            If Asc(Mid$(Text, i, 1)) < 128 Then
                strTempChar = _
          CType(Asc(Mid$(Text, i, 1)) + 128, String)
            ElseIf Asc(Mid$(Text, i, 1)) > 128 Then
                strTempChar = _
          CType(Asc(Mid$(Text, i, 1)) - 128, String)
            End If
            Mid$(Text, i, 1) = _
                Chr(CType(strTempChar, Integer))
        Next i
        Return Text
    End Function

    Public Function Greetings(ByVal name As String)
        ' Why should everything be professional and serious?
        ' Let's add some fun to the player :P

        If String.IsNullOrEmpty(name) = False Then
            Dim splitter() As String = Split(name, "|")
            Dim messages As String() = {"Howdy, %s!",
                                        "How are you today, %s?",
                                        "Prepare your ears, %s!",
                                        "Good to see you, %s!",
                                        "Lookin' good today, %s!",
                                        "Let's get crazy, %s!",
                                        "%s! %s! %s!",
                                        "Giving Dubstep a try, %s?",
                                        "Your neighbors love you, %s.",
                                        "Collect all the messages, %s!",
                                        "What's up, %s?",
                                        "Oh, behave! Yeah, %s, yeah! Woooh!",
                                        "Rise and shine, %s.",
                                        "Are you sitting comfortably, %s?",
                                        "Close your eyes and relax, %s.",
                                        "Hi, %s!",
                                        "Ready for some eargasms, %s?"}
            Dim Random As New Random()
            Dim number As Byte = Random.Next(0, messages.Length)
            Return messages(number).Replace("%s", splitter(0))
        Else
            Return Nothing
        End If

    End Function

    ' The following code thanks to _Tobias from the Digitally Imported forums.

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
                Dim list As List(Of String) = Audioaddict.ParsePlaylistFavorites(wc.DownloadString("http://listen." & StationChooser.Tag & "/premium/favorites.pls?" & listenkey))
                data = String.Join(vbNewLine, list.ToArray)
            Catch ex As Exception
                Return False
            End Try
        Else
            If My.Computer.FileSystem.FileExists(serversFolder & "\favorites.db") Then
                Using reader As IO.StreamReader = New IO.StreamReader(serversFolder & "\favorites.db")
                    data = reader.ReadToEnd()
                    reader.Close()
                End Using
            Else
                Return False
            End If
        End If
        Return Split(data, vbNewLine)

    End Function

    Public Function channelDb(ByVal loc As String)

        Dim fileinfo As New IO.FileInfo(loc)
        Dim download As Boolean = True

        Select Case cacheList

            Case "Every day"

                If fileinfo.LastWriteTime.Date = DateTime.Now.Date Then
                    download = False
                End If

            Case "Every week"

                Dim theDate As DateTime = fileinfo.LastWriteTime.Date
                Dim culture As System.Globalization.CultureInfo = System.Globalization.CultureInfo.CurrentCulture
                Dim weekNumberFile As Byte = culture.Calendar.GetWeekOfYear(theDate, Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)
                theDate = DateTime.Now.Date
                Dim weekNumberToday As Byte = culture.Calendar.GetWeekOfYear(theDate, Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)

                If weekNumberFile = weekNumberToday Then
                    download = False
                End If

            Case "Every month"

                If fileinfo.LastWriteTime.Month = DateTime.Now.Month Then
                    download = False
                End If

            Case "Never"

                download = False

        End Select


        If My.Computer.FileSystem.FileExists(loc) = False Or download = True Then
            Dim wc As Net.WebClient = New Net.WebClient
            Dim data As String
            Dim finalstring As String = ""
            Dim premium As String
            Try

                If isPremium Then
                    premium = "premium"
                Else
                    premium = "public"
                End If

                data = wc.DownloadString("http://listen." & StationChooser.Tag & "/" & premium)

                Dim splitter = Split(data.Replace("""", Nothing).Replace("{", Nothing).Replace("[", Nothing).Replace("}", Nothing).Replace("]", Nothing).Replace("id:", Nothing).Replace(",key:", "|").Replace(",name:", "|"), ",")
                Dim item As Integer = 0

                Do While item < splitter.Length

                    If splitter(item).Contains("|") Then
                        finalstring += splitter(item) & vbNewLine
                    End If

                    item += 1
                Loop
            Catch ex As Exception
                Return "Didn't download"
            End Try

            Dim writer As New IO.StreamWriter(loc, False)

            writer.Write(finalstring)
            writer.Close()
        End If

        Return New IO.StreamReader(loc)
    End Function

#End Region

    Private Sub copyDescription_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles copyDescription.Opening
        If String.IsNullOrEmpty(EventDescription.SelectedText) Then
            copyText.Enabled = False
        Else
            copyText.Enabled = True
        End If
    End Sub

    Private Sub copyText_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles copyText.Click
        Clipboard.SetText(EventDescription.SelectedText.Replace(Chr(10), vbNewLine))
    End Sub
End Class
