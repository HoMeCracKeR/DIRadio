' DI Radio Player by ViRUS
' Source code for version 1.7 Beta 3
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

Public Class Form1

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
    Public TotalVersionFixed As Integer = 22    ' -> For commodity, I don't use the actual version number of the application to know when there's an update. Instead I check if this number is higher.

#End Region

#Region "Other"

    Public WithEvents Seconds As New Label          ' -> Used to know when the connection dropped
    Public drawing As New Un4seen.Bass.Misc.Visuals ' -> Used to draw the vis
    Public RestartPlayback As Boolean               ' -> Used to know if playback should be restarted after an operation has completed (changing channels, for example)
    Public stream As Integer                        ' -> The stream that is passed to BASS so it plays it
    Public oldvol As Integer                        ' -> This stores the volume when the user clicks the Mute button; to know which volume level should be used when the user clicks the Unmute button
    Dim ServersArray As New ListView                ' -> Used to store a list of available servers for a particular channel
    ' v  The following list of channels don't have a forum board and should disable the Forums button when selected
    Dim NoForumsChannel As String = "Cosmic Downtempo;Deep Nu-Disco;Vocal Chillout;Deep House;Epic Trance;Hands Up;Club Dubstep;Progressive Psy;80's Rock Hits;Club Bollywood;Compact Discoveries;Hard Rock;Metal;Modern Blues;Modern Rock;Pop Rock;Relaxing Excursions;Ska;Smooth Lounge;Soft Rock"
    Private _mySync As SYNCPROC                     ' -> Sync so BASS says when the stream title has changed
    ' v Used to get command line arguments
    Dim CommandLineArgs As System.Collections.ObjectModel.ReadOnlyCollection(Of String) = My.Application.CommandLineArgs
    Dim TotalCommandLine As String = ""

    Delegate Sub MsgBoxSafe(ByVal text As String, ByVal style As MsgBoxStyle, ByVal title As String)

    Private EqBands As Integer() = {0, 0, 0, 0, 0, 0}
    Dim Eq As New BASS_DX8_PARAMEQ()

#End Region

#End Region

#Region "Main Form events"


    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        ' I know how to check my threads. Don't need VS babysitting me
        Control.CheckForIllegalCrossThreadCalls = False

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
        ' prior to 1.4 Beta 1 - move all existing files in the servers folder to the Digitally Imported folder

        Dim executable As String = Application.ExecutablePath
        Dim tabla() As String = Split(executable, "\")

        My.Computer.FileSystem.CreateDirectory(Application.ExecutablePath.Replace(tabla(tabla.Length - 1), Nothing) & "\servers")
        My.Computer.FileSystem.CreateDirectory(Application.ExecutablePath.Replace(tabla(tabla.Length - 1), Nothing) & "\servers\" & DIFM.Text)
        My.Computer.FileSystem.CreateDirectory(Application.ExecutablePath.Replace(tabla(tabla.Length - 1), Nothing) & "\servers\" & JazzRadio.Text)
        My.Computer.FileSystem.CreateDirectory(Application.ExecutablePath.Replace(tabla(tabla.Length - 1), Nothing) & "\servers\" & SKYFM.Text)

        If IO.Directory.GetFiles(Application.ExecutablePath.Replace(tabla(tabla.Length - 1), Nothing) & "\servers").Length > 0 Then
            Dim str As String

            For Each str In IO.Directory.GetFiles(Application.ExecutablePath.Replace(tabla(tabla.Length - 1), Nothing) & "\servers")
                Try
                    Dim fullfile As String = str
                    Dim choppedfile() As String = Split(fullfile, "\")

                    My.Computer.FileSystem.MoveFile(str, Application.ExecutablePath.Replace(tabla(tabla.Length - 1), Nothing) & "\servers\" & DIFM.Text & "\" & choppedfile(choppedfile.Length - 1))
                Catch ex As Exception

                    MsgBox("Error reorganizing radio files.")

                End Try


            Next
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

                End If
            End If


        Else

            If Visualisation = True And PlayStop.Tag = "Stop" Then
                VisTimer.Start()
            End If

            UnregisterHotKey(Me.Handle, 1)
            UnregisterHotKey(Me.Handle, 2)
            UnregisterHotKey(Me.Handle, 3)
            UnregisterHotKey(Me.Handle, 4)
            UnregisterHotKey(Me.Handle, 5)
            UnregisterHotKey(Me.Handle, 6)

        End If
    End Sub

    Private Sub Form1_TextChanged(sender As Object, e As System.EventArgs) Handles Me.TextChanged

        If PlayStop.Tag = "Play" Then
            TrayIcon.Text = Me.Text
        End If

    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        Dim executable As String = Application.ExecutablePath
        Dim tabla() As String = Split(executable, "\")
        Dim file As String = Application.ExecutablePath.Replace(tabla(tabla.Length - 1), Nothing) & "\options.ini"

        If FadeOut.Enabled = False Then
            Try

                Dim writer As New IO.StreamWriter(file, False)
                writer.WriteLine(Form2.NotificationTitle.Name & "=" & NotificationTitle)
                writer.WriteLine(Form2.PlayNewOnChannelChange.Name & "=" & PlayNewOnChannelChange)
                writer.WriteLine(Form2.NotificationIcon.Name & "=" & NotificationIcon)
                writer.WriteLine(Form2.NoTaskbarButton.Name & "=" & NoTaskbarButton)
                writer.WriteLine(Form2.GoogleSearch.Name & "=" & GoogleSearch)
                writer.WriteLine(Form2.PremiumFormats.Name & "=" & PremiumFormats)
                writer.WriteLine("DIFormat=" & DIFormat)
                writer.WriteLine("SKYFormat=" & SKYFormat)
                writer.WriteLine("JazzFormat=" & JazzFormat)
                writer.WriteLine(Form2.ListenKey.Name & "=" & ListenKey)
                writer.WriteLine(Form2.BetaVersions.Name & "=" & BetaVersions)
                writer.WriteLine(Form2.UpdatesAtStart.Name & "=" & UpdatesAtStart)
                writer.WriteLine(Form2.Visualisation.Name & "=" & Visualisation)
                writer.WriteLine(Form2.VisualisationType.Name & "=" & VisualisationType)
                writer.WriteLine(Form2.HighQualityVis.Name & "=" & HighQualityVis)
                writer.WriteLine(Form2.LinealRepresentation.Name & "=" & LinealRepresentation)
                writer.WriteLine(Form2.FullSoundRange.Name & "=" & FullSoundRange)
                writer.WriteLine(Form2.Smoothness.Name & "=" & Smoothness)
                writer.WriteLine(Form2.MainColour.Name & "=" & MainColour)
                writer.WriteLine(Form2.SecondaryColour.Name & "=" & SecondaryColour)
                writer.WriteLine(Form2.PeakColour.Name & "=" & PeakColour)
                writer.WriteLine(Form2.BackgroundColour.Name & "=" & BackgroundColour)
                writer.WriteLine(Form2.ChangeWholeBackground.Name & "=" & ChangeWholeBackground)
                writer.WriteLine(Form2.MultimediaKeys.Name & "=" & MultimediaKeys)
                writer.WriteLine(Form2.HotkeyPlayStop.Name & "=" & HumanModifiersPlayStop & "=" & ModifiersPlayStop & "=" & KeyPlayStop)
                writer.WriteLine(Form2.HotkeyVolumeUp.Name & "=" & HumanModifiersVolumeUp & "=" & ModifiersVolumeUp & "=" & KeyVolumeUp)
                writer.WriteLine(Form2.HotkeyVolumeDown.Name & "=" & HumanModifiersVolumeDown & "=" & ModifiersVolumeDown & "=" & KeyVolumeDown)
                writer.WriteLine(Form2.HotkeyMuteUnmute.Name & "=" & HumanModifiersMuteUnmute & "=" & ModifiersMuteUnmute & "=" & KeyMuteUnmute)
                writer.WriteLine(Form2.HotkeyShowHide.Name & "=" & HumanModifiersShowHide & "=" & ModifiersShowHide & "=" & KeyShowHide)
                writer.WriteLine(Form2.Band0.Name & "=" & Band0)
                writer.WriteLine(Form2.Band1.Name & "=" & Band1)
                writer.WriteLine(Form2.Band2.Name & "=" & Band2)
                writer.WriteLine(Form2.Band3.Name & "=" & Band3)
                writer.WriteLine(Form2.Band4.Name & "=" & Band4)
                writer.WriteLine(Form2.Band5.Name & "=" & Band5)
                writer.WriteLine(StationChooser.Name & "=" & StationChooser.Text)
                writer.WriteLine("DIChannel=" & DIChannel)
                writer.WriteLine("SkyChannel=" & SKYChannel)
                writer.WriteLine("JazzChannel=" & JazzChannel)

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

            Catch ex As Exception

                ' If the options file couldn't be written, display an error message and cancel closing so the user tries again
                MsgBox("Your options couldn't be saved due to the following error:" & vbNewLine & ex.Message & vbNewLine & vbNewLine & "Please try closing the application again.", MsgBoxStyle.Exclamation)
                e.Cancel = True

            End Try
        End If

        ' Unregister hotkeys
        UnregisterHotKey(Me.Handle, 1)
        UnregisterHotKey(Me.Handle, 2)
        UnregisterHotKey(Me.Handle, 3)
        UnregisterHotKey(Me.Handle, 4)
        UnregisterHotKey(Me.Handle, 5)
        UnregisterHotKey(Me.Handle, 6)

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

            If Me.WindowState = FormWindowState.Minimized And NotificationTitle = True And RadioString.Text.ToLower = "get digitally imported premium" = False And RadioString.Text.ToLower.Contains("adwtag") = False And RadioString.Text.ToLower = "unleash the full potential of sky.fm. get sky.fm premium now!" = False And RadioString.Text.ToLower = "it gets even better! jazzradio premium - www.jazzradio.com/join" = False And RadioString.Text.ToLower = "more of the show after these messages" = False And RadioString.Text.ToLower.Contains("photonvps.com") = False And RadioString.Text.ToLower = "blood pumping mind twisting di radio" = False And RadioString.Text.ToLower = "love thy neighbour as thyself - turn up the volume (t)" = False Then
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

            ToolTip.SetToolTip(PlayStop, "Stop")
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

                If SelectedServer.Items.Count < 1 Then
                    PlayStop.Enabled = False
                    SelectedServer.Enabled = False
                Else
                    SelectedServer.Enabled = True
                End If
            End If

            If PLSDownloader.IsBusy = False Then
                SelectedChannel.Enabled = True
            End If

            Bass.BASS_ChannelStop(stream)
            VisTimer.Stop()
            TimePassed.Stop()
            VisualisationBox.Image = Nothing
            TimerString.Text = "00:00"
            TrayIcon.Text = Me.Text
            ToolTip.SetToolTip(PlayStop, "Play")
        End If
    End Sub

    Private Sub OptionsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Options.Click
        Dim X As Integer
        Dim Y As Integer

        ' If main form is visible, get its boundaries, make sure the Options panel won't appear out of screen
        ' and then open it and bring it to front just in case. Otherwise simply center it on screen and
        ' bring it.

        If Me.Visible = True Then

            If Me.Location.X + 21 > Screen.PrimaryScreen.WorkingArea.Size.Width Then
                X = Screen.PrimaryScreen.WorkingArea.Size.Width - Form2.Size.Width
            ElseIf Me.Location.X + 21 < 0 Then
                X = 0
            Else
                X = Me.Location.X + 21
            End If


            If Visualisation = True Then
                If Me.Location.Y - 12 > Screen.PrimaryScreen.WorkingArea.Size.Height Then
                    Y = Screen.PrimaryScreen.WorkingArea.Size.Height - Form2.Size.Height
                ElseIf Me.Location.Y - 12 < 0 Then
                    Y = 0
                Else
                    Y = Me.Location.Y - 12
                End If
            Else
                If Me.Location.Y - 180 > Screen.PrimaryScreen.WorkingArea.Size.Height Then
                    Y = Screen.PrimaryScreen.WorkingArea.Size.Height - Form2.Size.Height
                ElseIf Me.Location.Y - 180 < 0 Then
                    Y = 0
                Else
                    Y = Me.Location.Y - 180
                End If
            End If

            Form2.Location = New Point(X, Y)

        Else

            Form2.StartPosition = FormStartPosition.CenterScreen

        End If

        Form2.Show()
        Form2.BringToFront()

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

    Private Sub History_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles History.Click
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
            Mute.Tag = "Mute"

        Else

            oldvol = Volume.Value
            Volume.Value = 0
            Mute.Tag = "Unmute"

        End If

        ToolTip.SetToolTip(Mute, Mute.Tag)
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
    End Sub

    Private Sub EditFavorites_LinkClicked(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles EditFavorites.LinkClicked
        Process.Start("http://www." & StationChooser.Tag & "/member/favorite/channels")
    End Sub

    Private Sub RefreshFavorites_LinkClicked(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles RefreshFavorites.LinkClicked
        Try
            OldFav = SelectedServer.Text
            Dim executable As String = Application.ExecutablePath
            Dim tabla() As String = Split(executable, "\")

            Dim file As String = Application.ExecutablePath.Replace(tabla(tabla.Length - 1), Nothing) & "\servers\" & StationChooser.Text & "\myfavorites."

            If My.Computer.FileSystem.FileExists(file & "pls") Then
                Kill(file & "pls")
            ElseIf My.Computer.FileSystem.FileExists(file & "asx") Then
                Kill(file & "asx")
            End If

            If PlayStop.Tag = "Stop" Then
                RefreshFavorites.Enabled = False
            End If

            SelectedChannel_SelectedIndexChanged(Me, Nothing)
        Catch
        End Try
    End Sub

    Private Sub StationChooser_ButtonClick(sender As Object, e As System.EventArgs) Handles StationChooser.ButtonClick
        If StationChooser.Tag = "di.fm" Then
            JazzRadio_Click(sender, e)
        ElseIf StationChooser.Tag = "jazzradio.com" Then
            SKYFM_Click(sender, e)
        ElseIf StationChooser.Tag = "sky.fm" Then
            DIFM_Click(sender, e)
        End If
    End Sub

    Private Sub StationChooser_TextChanged(sender As Object, e As System.EventArgs) Handles StationChooser.TextChanged

        ' Long lists ahead. Enjoy!

        SelectedChannel.Items.Clear()

        If ListenKey = Nothing = False Then
            SelectedChannel.Items.Add("My Favorites")
        End If

        If StationChooser.Text = DIFM.Text Then

            SelectedChannel.Items.Add("Ambient")
            SelectedChannel.Items.Add("Breaks")
            SelectedChannel.Items.Add("Chillout")
            SelectedChannel.Items.Add("Chillout Dreams")
            SelectedChannel.Items.Add("Chiptunes")
            SelectedChannel.Items.Add("Classic Electronica")
            SelectedChannel.Items.Add("Classic EuroDance")
            SelectedChannel.Items.Add("Classic Trance")
            SelectedChannel.Items.Add("Classic Vocal Trance")
            SelectedChannel.Items.Add("Club Dubstep")
            SelectedChannel.Items.Add("Club Sounds")
            SelectedChannel.Items.Add("Cosmic Downtempo")
            SelectedChannel.Items.Add("Deep House")
            SelectedChannel.Items.Add("Disco House")
            SelectedChannel.Items.Add("DJ Mixes")
            SelectedChannel.Items.Add("Drum 'n Bass")
            SelectedChannel.Items.Add("Dubstep")
            SelectedChannel.Items.Add("Electro House")
            SelectedChannel.Items.Add("Epic Trance")
            SelectedChannel.Items.Add("EuroDance")
            SelectedChannel.Items.Add("Funky House")
            SelectedChannel.Items.Add("Future Synthpop")
            SelectedChannel.Items.Add("Goa-Psy Trance")
            SelectedChannel.Items.Add("Hands Up")
            SelectedChannel.Items.Add("Hard Dance")
            SelectedChannel.Items.Add("Hardcore")
            SelectedChannel.Items.Add("Hardstyle")
            SelectedChannel.Items.Add("House")
            SelectedChannel.Items.Add("Latin House")
            SelectedChannel.Items.Add("Liquid DnB")
            SelectedChannel.Items.Add("Lounge")
            SelectedChannel.Items.Add("Minimal")
            SelectedChannel.Items.Add("Oldschool Acid")
            SelectedChannel.Items.Add("Progressive")
            SelectedChannel.Items.Add("Progressive Psy")
            SelectedChannel.Items.Add("PsyChill")
            SelectedChannel.Items.Add("Soulful House")
            SelectedChannel.Items.Add("Space Dreams")
            SelectedChannel.Items.Add("Tech House")
            SelectedChannel.Items.Add("Techno")
            SelectedChannel.Items.Add("Trance")
            SelectedChannel.Items.Add("Tribal House")
            SelectedChannel.Items.Add("UK Garage")
            SelectedChannel.Items.Add("Vocal Chillout")
            SelectedChannel.Items.Add("Vocal Trance")

            SelectedChannel.SelectedIndex = DIChannel

            Calendar.Enabled = True
            History.Enabled = True

        ElseIf StationChooser.Text = SKYFM.Text Then

            SelectedChannel.Items.Add("80's Rock Hits")
            SelectedChannel.Items.Add("A Beatles Tribute")
            SelectedChannel.Items.Add("Alternative Rock")
            SelectedChannel.Items.Add("American Songbook")
            SelectedChannel.Items.Add("Bebop Jazz")
            SelectedChannel.Items.Add("Bossa Nova")
            SelectedChannel.Items.Add("Best of the 80's")
            SelectedChannel.Items.Add("Classic Rap")
            SelectedChannel.Items.Add("Classic Rock")
            SelectedChannel.Items.Add("Classical Guitar")
            SelectedChannel.Items.Add("Classical Piano Trios")
            SelectedChannel.Items.Add("Club Bollywood")
            SelectedChannel.Items.Add("Compact Discoveries")
            SelectedChannel.Items.Add("Contemporary Christian")
            SelectedChannel.Items.Add("Country")
            SelectedChannel.Items.Add("Dance Hits")
            SelectedChannel.Items.Add("DaTempo Lounge")
            SelectedChannel.Items.Add("Dreamscapes")
            SelectedChannel.Items.Add("Hard Rock")
            SelectedChannel.Items.Add("Hit 70's")
            SelectedChannel.Items.Add("Indie Rock")
            SelectedChannel.Items.Add("Jazz Classics")
            SelectedChannel.Items.Add("Jpop")
            SelectedChannel.Items.Add("Love Music")
            SelectedChannel.Items.Add("Metal")
            SelectedChannel.Items.Add("Modern Blues")
            SelectedChannel.Items.Add("Modern Rock")
            SelectedChannel.Items.Add("Mostly Classical")
            SelectedChannel.Items.Add("Movie Soundtracks")
            SelectedChannel.Items.Add("Nature")
            SelectedChannel.Items.Add("New Age")
            SelectedChannel.Items.Add("Oldies")
            SelectedChannel.Items.Add("Piano Jazz")
            SelectedChannel.Items.Add("Pop Punk")
            SelectedChannel.Items.Add("Pop Rock")
            SelectedChannel.Items.Add("Relaxation")
            SelectedChannel.Items.Add("Relaxing Excursions")
            SelectedChannel.Items.Add("Romantica")
            SelectedChannel.Items.Add("Roots Reggae")
            SelectedChannel.Items.Add("Salsa")
            SelectedChannel.Items.Add("Ska")
            SelectedChannel.Items.Add("Smooth Jazz")
            SelectedChannel.Items.Add("Smooth Jazz 24'7")
            SelectedChannel.Items.Add("Smooth Lounge")
            SelectedChannel.Items.Add("Soft Rock")
            SelectedChannel.Items.Add("Solo Piano")
            SelectedChannel.Items.Add("Top Hits")
            SelectedChannel.Items.Add("Uptempo Smooth Jazz")
            SelectedChannel.Items.Add("Vocal New Age")
            SelectedChannel.Items.Add("Vocal Smooth Jazz")
            SelectedChannel.Items.Add("Urban Jamz")
            SelectedChannel.Items.Add("World")

            SelectedChannel.SelectedIndex = SKYChannel

            Calendar.Enabled = False
            History.Enabled = True

        ElseIf StationChooser.Text = JazzRadio.Text Then

            SelectedChannel.Items.Add("Avant-Garde")
            SelectedChannel.Items.Add("Bass Jazz")
            SelectedChannel.Items.Add("Bebop")
            SelectedChannel.Items.Add("Blues")
            SelectedChannel.Items.Add("Bossa Nova")
            SelectedChannel.Items.Add("Classic Jazz")
            SelectedChannel.Items.Add("Contemporary Vocals")
            SelectedChannel.Items.Add("Cool Jazz")
            SelectedChannel.Items.Add("Current Jazz")
            SelectedChannel.Items.Add("Fusion Lounge")
            SelectedChannel.Items.Add("Guitar Jazz")
            SelectedChannel.Items.Add("Gypsy Jazz")
            SelectedChannel.Items.Add("Hard Bop")
            SelectedChannel.Items.Add("Latin Jazz")
            SelectedChannel.Items.Add("Mellow Jazz")
            SelectedChannel.Items.Add("Paris Café")
            SelectedChannel.Items.Add("Piano Jazz")
            SelectedChannel.Items.Add("Piano Trios")
            SelectedChannel.Items.Add("Saxophone Jazz")
            SelectedChannel.Items.Add("Sinatra Style")
            SelectedChannel.Items.Add("Smooth Jazz 24'7")
            SelectedChannel.Items.Add("Smooth Jazz")
            SelectedChannel.Items.Add("Smooth Lounge")
            SelectedChannel.Items.Add("Smooth Uptempo")
            SelectedChannel.Items.Add("Smooth Vocals")
            SelectedChannel.Items.Add("Straight-Ahead")
            SelectedChannel.Items.Add("Swing & Big Band")
            SelectedChannel.Items.Add("Timeless Classics")
            SelectedChannel.Items.Add("Trumpet Jazz")
            SelectedChannel.Items.Add("Vibraphone Jazz")
            SelectedChannel.Items.Add("Vocal Legends")

            SelectedChannel.SelectedIndex = JazzChannel

            Calendar.Enabled = False
            History.Enabled = False
            Forums.Enabled = False

        End If

    End Sub

    Public Sub SelectedChannel_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SelectedChannel.SelectedIndexChanged

        If PlayStop.Tag = "Play" Then
            PlayStop.Enabled = False
        End If

        StationChooser.Enabled = False
        PLSDownloader.RunWorkerAsync()
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
        End If

    End Sub

    Private Sub SelectedChannel_TextChanged(sender As Object, e As System.EventArgs) Handles SelectedChannel.TextChanged
        ' Check if the Forums button should be enabled or not and exit as soon as there is a match
        ' Or always disable the button if JazzRadio is selected

        Dim Channels As String = NoForumsChannel
        Dim Channel() As String = Split(Channels, ";")
        Dim ChannelNumber As Integer = 0

        Do While ChannelNumber < Channel.Length
            ChannelNumber += 1
            If ChannelNumber <= Channel.Length - 1 Then

                If SelectedChannel.Text = Channel(ChannelNumber) OrElse StationChooser.Text = JazzRadio.Text Then
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

#End Region

#Region "Background Workers"


    Private Sub PLSDownloader_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles PLSDownloader.DoWork
        RefreshFavorites.Enabled = False
        PlayStop.Enabled = False
        StationChooser.Enabled = False
        ServersArray.Items.Clear()
        SelectedServer.Items.Clear()

        Dim channel As String
        channel = SelectedChannel.Text.ToLower.Replace(" ", Nothing)

        ' Get the current channel, remove spaces and convert names to their URL counterparts if necessary

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

        ElseIf StationChooser.Text = JazzRadio.Text Then

            If channel = "smoothjazz24'7" Then
                channel = "smoothjazz247"
            ElseIf channel = "pariscafé" Then
                channel = "pariscafe"
            ElseIf channel = "contemporaryvocals" Then
                channel = "vocaljazz"
            ElseIf channel = "avant-garde" Then
                channel = "avantgarde"
            ElseIf channel = "straight-ahead" Then
                channel = "straightahead"
            ElseIf channel = "swing&bigband" Then
                channel = "swingnbigband"
            End If

        End If


        Dim executable As String = Application.ExecutablePath
        Dim tabla() As String = Split(executable, "\")

        My.Computer.FileSystem.CreateDirectory(Application.ExecutablePath.Replace(tabla(tabla.Length - 1), Nothing) & "\servers")

        Dim file As String = Application.ExecutablePath.Replace(tabla(tabla.Length - 1), Nothing) & "\servers\" & StationChooser.Text & "\" & channel & "."

        Dim EndString As String
        Dim IsWMA As Boolean
        Dim fileExtension As String


        If ListenKey = Nothing = False And PremiumFormats = True Then

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
                    IsWMA = True
                ElseIf DIFormat = 6 Then
                    EndString = "premium_wma_low"
                    IsWMA = True
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
                    IsWMA = True
                ElseIf SKYFormat = 6 Then
                    EndString = "premium_wma_low"
                    IsWMA = True
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
                    IsWMA = True
                End If

            End If


        Else

            If StationChooser.Text = DIFM.Text Then

                If DIFormat = 0 Then
                    EndString = "public2"
                ElseIf DIFormat = 1 Then
                    EndString = "public3"
                ElseIf DIFormat = 2 Then
                    EndString = "public5"
                    IsWMA = True
                End If

            ElseIf StationChooser.Text = SKYFM.Text Then

                If SKYFormat = 0 Then
                    EndString = "public1"
                ElseIf SKYFormat = 1 Then
                    EndString = "public3"
                ElseIf SKYFormat = 2 Then
                    EndString = "public5"
                    IsWMA = True
                End If

            ElseIf StationChooser.Text = JazzRadio.Text Then

                If JazzFormat = 0 Then
                    EndString = "public1"
                ElseIf JazzFormat = 1 Then
                    EndString = "public3"
                ElseIf JazzFormat = 2 Then
                    EndString = "public5"
                    IsWMA = True
                End If

            End If


        End If



        If IsWMA = True Then
            fileExtension = "asx"
        Else
            fileExtension = "pls"
        End If

        If My.Computer.FileSystem.FileExists(file & fileExtension) = False Then

            Dim WebClient As Net.WebClient = New Net.WebClient()
            Dim PLS As String

            Try

                PLS = WebClient.DownloadString("http://listen." & StationChooser.Tag & "/" & EndString & "/" & channel.Replace("my", Nothing) & "." & fileExtension & "?" & ListenKey)

            Catch ex As Exception

                Dim Message As New MsgBoxSafe(AddressOf DisplayMessage)

                If ex.Message.Contains("403") Then
                    Me.Invoke(Message, "Couldn't download servers list." & vbNewLine & ex.Message & vbNewLine & vbNewLine & "Wrong or expired premium key?", MsgBoxStyle.Exclamation, "Error getting servers list")
                Else
                    Me.Invoke(Message, "Couldn't download servers list." & vbNewLine & ex.Message, MsgBoxStyle.Exclamation, "Error getting servers list")
                End If

                Marquee.Hide()
                SelectedServer.Enabled = False

                If PlayStop.Tag = "Play" Then
                    PlayStop.Enabled = False
                End If

                StationChooser.Enabled = True
                RestartPlayback = False
                Exit Sub

            End Try

            Dim writer As New IO.StreamWriter(file & fileExtension, False)
            writer.Write(PLS)
            writer.Close()

        End If

        Dim serverno As Integer = 1

        Dim reader As New IO.StreamReader(file & fileExtension)
        Dim Favourite As String

        Do While (reader.Peek > -1)
            Dim check As String = reader.ReadLine

            If check.StartsWith("<ref href") Then

                Dim first As String
                Dim second As String

                first = check.Replace("<ref href=""", Nothing)
                second = first.Replace("""/>", Nothing)
                Dim item As New ListViewItem("Server #" & serverno)
                item.Tag = second
                ServersArray.Items.Add(item.Tag)

                If Favourite = Nothing = False Then
                    SelectedServer.Items.Add(Favourite)
                    Favourite = Nothing
                Else
                    SelectedServer.Items.Add(item.Text)
                End If

                serverno += 1

            ElseIf check.ToLower.StartsWith("file") Then

                Dim item As New ListViewItem("Server #" & serverno)
                item.Tag = check.Replace("File" & serverno & "=", Nothing)
                ServersArray.Items.Add(item.Tag)
                SelectedServer.Items.Add(item.Text)
                serverno += 1

            ElseIf check.ToLower.StartsWith("title") And SelectedChannel.Text = "My Favorites" OrElse check.ToLower.StartsWith("<title>") And SelectedChannel.Text = "My Favorites" Then

                Dim FullLine As String = check
                Dim Splitter As String() = Split(FullLine, " - ")

                If check.ToLower.StartsWith("<title>") = False Then
                    SelectedServer.Items.Item(SelectedServer.Items.Count - 1) = Splitter(1)
                Else
                    Favourite = Splitter(1).Replace("</title>", Nothing)
                End If

            End If

        Loop

        reader.Close()

    End Sub

    Private Sub PLSDownloader_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles PLSDownloader.RunWorkerCompleted
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
            Calendar.Enabled = False
        ElseIf StationChooser.Text = JazzRadio.Text = False Then
            History.Enabled = True

            If StationChooser.Text = SKYFM.Text = False Then
                Calendar.Enabled = True
            End If
        End If

        If Bufer.IsBusy = False Then
            RefreshFavorites.Enabled = True
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
                Else

                    If Bufer.CancellationPending = False And SelectedServer.Text = "My Favorites" = False Then
                        RadioString.BackColor = Color.Yellow
                        RadioString.ForeColor = Color.Black
                        RadioString.Text = "Connection is taking some time, please wait..."
                        SelectedServer.SelectedIndex = SelectedServer.SelectedIndex + 1
                        PlayStop.Image = My.Resources.StopPlayback
                        PlayStop.Tag = "Stop"
                        PlayStop.Enabled = True
                        StationChooser.Enabled = True
                        GoTo again
                    Else
                        PlayStop.Image = My.Resources.StartPlayback
                        PlayStop.Tag = "Play"
                        RadioString.BackColor = Color.Red
                        RadioString.ForeColor = Color.White
                        RadioString.Text = "Couldn't connect to channel " & SelectedServer.Text & "."
                        e.Cancel = True
                    End If

                End If

            Else

                Try
                    Dim raw As String = String.Concat(Bass.BASS_ChannelGetTagsMETA(stream))
                    Dim tabla() As String = Split(raw, "='")
                    RadioString.Text = tabla(1).Replace("';", Nothing)
                    RadioString.Text = RadioString.Text.Replace("StreamUrl", Nothing)
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
        Dim executable As String = Application.ExecutablePath
        Dim tabla() As String = Split(executable, "\")
        Dim file As String = Application.ExecutablePath.Replace(tabla(tabla.Length - 1), Nothing) & "\Info.txt"
        Dim DownloadedString As String

        Try
            DownloadedString = WebClient.DownloadString("http://www.tobiass.eu/files/Info.txt")
        Catch
        End Try

        Dim writer As New IO.StreamWriter(file, False)
        writer.Write(DownloadedString)
        writer.Close()

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

        Kill(file)
    End Sub

    Private Sub GetUpdates_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles GetUpdates.RunWorkerCompleted
        Dim FullLine As String = LatestVersionString
        Dim Splitter As String() = Split(FullLine, ".")

        If TotalVersionString > TotalVersionFixed Then

            Form2.LookNow.Text = "Download update"
            Form2.LatestVersion.ForeColor = Color.Green

            If AtStartup = True Then

                If Splitter(2) > 0 And BetaVersions = True Then
                    If MsgBox("There's a new beta version available!" & vbNewLine & "Download now?", MsgBoxStyle.YesNo, "Update available") = MsgBoxResult.Yes Then
                        Form2.LookNow_Click(Me, Nothing)
                    End If
                ElseIf Splitter(2) = 0 Then
                    If MsgBox("There's a new version available!" & vbNewLine & "Download now?", MsgBoxStyle.YesNo, "Update available") = MsgBoxResult.Yes Then
                        Form2.LookNow_Click(Me, Nothing)
                    End If
                End If


            End If
        End If

        If Splitter(2) > 0 Then

            If Splitter(1) = 9 Then
                Form2.LatestVersion.Text = "Latest version: " & Splitter(0) + 1 & ".0" & " Beta " & Splitter(2)
            Else
                Form2.LatestVersion.Text = "Latest version: " & Splitter(0) & "." & Splitter(1) + 1 & " Beta " & Splitter(2)
            End If

        Else

            Form2.LatestVersion.Text = "Latest version: " & Splitter(0) & "." & Splitter(1)

        End If

        Form2.UndefinedProgress.Hide()
        Form2.Status.Text = "Status: Idle"
        Form2.LookNow.Enabled = True

        AtStartup = False
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

            Seconds.Text = String.Format("{0:00}", span.Seconds)
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
        Try
            Clipboard.Clear()
            Clipboard.SetDataObject(RadioString.Text)
        Catch
        End Try
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

    Private Sub TrackHistoryTray_Click(sender As System.Object, e As System.EventArgs) Handles TrackHistoryTray.Click
        History_Click(sender, e)
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
    End Sub

    Private Sub JazzRadio_Click(sender As System.Object, e As System.EventArgs) Handles JazzRadio.Click
        StationChooser.Image = JazzRadio.Image
        StationChooser.Tag = "jazzradio.com"
        StationChooser.Text = JazzRadio.Text
        StationChooser.ToolTipText = JazzRadio.Text
        Me.Text = Me.Text.Replace("DI", "JazzRadio")
        Me.Text = Me.Text.Replace("SKY.FM", "JazzRadio")
    End Sub

    Private Sub SKYFM_Click(sender As System.Object, e As System.EventArgs) Handles SKYFM.Click
        StationChooser.Image = SKYFM.Image
        StationChooser.Tag = "sky.fm"
        StationChooser.Text = SKYFM.Text
        StationChooser.ToolTipText = SKYFM.Text
        Me.Text = Me.Text.Replace("DI", "SKY.FM")
        Me.Text = Me.Text.Replace("JazzRadio", "SKY.FM")
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
            Dim raw As String = String.Concat(Bass.BASS_ChannelGetTagsMETA(stream))
            Dim tabla() As String = Split(raw, "='")
            RadioString.Text = tabla(1).Replace("StreamUrl", Nothing).Replace("';", Nothing)

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

        Dim executable As String = Application.ExecutablePath
        Dim tabla() As String = Split(executable, "\")
        Dim file As String = Application.ExecutablePath.Replace(tabla(tabla.Length - 1), Nothing) & "\options.ini"

        ' If the options file doesn't exist, create it with some default values

        If My.Computer.FileSystem.FileExists(file) = False Then
            Dim writer As New IO.StreamWriter(file)
            writer.WriteLine(Form2.NotificationTitle.Name & "=True")
            writer.WriteLine(Form2.PlayNewOnChannelChange.Name & "=False")
            writer.WriteLine(Form2.NotificationIcon.Name & "=True")
            writer.WriteLine(Form2.NoTaskbarButton.Name & "=False")
            writer.WriteLine(Form2.GoogleSearch.Name & "=True")
            writer.WriteLine(Form2.ListenKey.Name & "=")
            writer.WriteLine(Form2.PremiumFormats.Name & "=False")
            writer.WriteLine("DIFormat=1")
            writer.WriteLine("SKYFormat=1")
            writer.WriteLine("JazzFormat=1")
            writer.WriteLine(Form2.BetaVersions.Name & "=False")
            writer.WriteLine(Form2.UpdatesAtStart.Name & "=True")
            writer.WriteLine(Form2.Visualisation.Name & "=True")
            writer.WriteLine(Form2.VisualisationType.Name & "=5")
            writer.WriteLine(Form2.HighQualityVis.Name & "=False")
            writer.WriteLine(Form2.LinealRepresentation.Name & "=False")
            writer.WriteLine(Form2.FullSoundRange.Name & "=False")
            writer.WriteLine(Form2.Smoothness.Name & "=27")
            writer.WriteLine(Form2.MainColour.Name & "=" & SystemColors.Control.ToArgb())
            writer.WriteLine(Form2.SecondaryColour.Name & "=" & Color.Black.ToArgb())
            writer.WriteLine(Form2.PeakColour.Name & "=" & Color.Silver.ToArgb())
            writer.WriteLine(Form2.BackgroundColour.Name & "=" & SystemColors.Control.ToArgb())
            writer.WriteLine(Form2.ChangeWholeBackground.Name & "=False")
            writer.WriteLine(Form2.MultimediaKeys.Name & "=True")
            writer.WriteLine(Form2.HotkeyPlayStop.Name & "=0=0=" & Keys.MediaPlayPause)
            writer.WriteLine(Form2.HotkeyVolumeUp.Name & "=0=0=" & Keys.VolumeUp)
            writer.WriteLine(Form2.HotkeyVolumeDown.Name & "=0=0=" & Keys.VolumeDown)
            writer.WriteLine(Form2.HotkeyMuteUnmute.Name & "=0=0=" & Keys.VolumeMute)
            writer.WriteLine(Form2.HotkeyShowHide.Name & "=" & Keys.Control + Keys.Shift & "=6=" & Keys.Home)
            writer.WriteLine(Form2.Band0.Name & "=0")
            writer.WriteLine(Form2.Band1.Name & "=0")
            writer.WriteLine(Form2.Band2.Name & "=0")
            writer.WriteLine(Form2.Band3.Name & "=0")
            writer.WriteLine(Form2.Band4.Name & "=0")
            writer.WriteLine(Form2.Band5.Name & "=0")
            writer.WriteLine("DIChannel=0")
            writer.WriteLine("SkyChannel=0")
            writer.WriteLine("JazzChannel=0")
            writer.WriteLine(Volume.Name & "=50")
            writer.WriteLine(SelectedServer.Name & "=0")
            writer.Close()
        End If

        Try
            Dim reader As New IO.StreamReader(file)

            Do While (reader.Peek > -1)
                Dim whole As String = reader.ReadLine
                Dim splitter() As String = Split(whole, "=")

                If splitter(0) = Form2.NotificationTitle.Name Then
                    NotificationTitle = splitter(1)
                ElseIf splitter(0) = Form2.PlayNewOnChannelChange.Name Then
                    PlayNewOnChannelChange = splitter(1)
                ElseIf splitter(0) = Form2.NotificationIcon.Name Then
                    NotificationIcon = splitter(1)
                ElseIf splitter(0) = Form2.NoTaskbarButton.Name Then
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
                ElseIf splitter(0) = Form2.GoogleSearch.Name Then
                    GoogleSearch = splitter(1)
                ElseIf splitter(0) = Form2.PremiumFormats.Name Then
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


                ElseIf splitter(0) = Form2.ListenKey.Name OrElse splitter(0) = "PremiumKey" Then

                    ListenKey = splitter(1)

                    If ListenKey = Nothing = False Then
                        SelectedChannel.Items.Add("My Favorites")
                    End If
                ElseIf splitter(0) = Form2.BetaVersions.Name Then
                    BetaVersions = splitter(1)
                ElseIf splitter(0) = Form2.UpdatesAtStart.Name Then

                    UpdatesAtStart = splitter(1)

                    If UpdatesAtStart = True Then
                        AtStartup = True
                        GetUpdates.RunWorkerAsync()
                    End If

                ElseIf splitter(0) = Form2.Visualisation.Name Then
                    Visualisation = splitter(1)

                    If Visualisation = True Then

                        VisualisationBox.Show()
                        Me.Size = New Size(Me.MaximumSize)

                    Else

                        VisualisationBox.Hide()
                        Me.Size = New Size(Me.MinimumSize)

                    End If

                ElseIf splitter(0) = Form2.VisualisationType.Name Then
                    VisualisationType = splitter(1)
                ElseIf splitter(0) = Form2.HighQualityVis.Name Then
                    HighQualityVis = splitter(1)
                ElseIf splitter(0) = Form2.LinealRepresentation.Name Then
                    LinealRepresentation = splitter(1)
                ElseIf splitter(0) = Form2.FullSoundRange.Name Then
                    FullSoundRange = splitter(1)
                ElseIf splitter(0) = Form2.Smoothness.Name Then
                    Smoothness = splitter(1)
                ElseIf splitter(0) = Form2.MainColour.Name Then
                    MainColour = splitter(1)
                ElseIf splitter(0) = Form2.SecondaryColour.Name Then
                    SecondaryColour = splitter(1)
                ElseIf splitter(0) = Form2.PeakColour.Name Then
                    PeakColour = splitter(1)
                ElseIf splitter(0) = Form2.BackgroundColour.Name Then
                    BackgroundColour = splitter(1)
                ElseIf splitter(0) = Form2.ChangeWholeBackground.Name Then
                    ChangeWholeBackground = splitter(1)

                    If ChangeWholeBackground = True Then

                        Me.BackColor = Color.FromArgb(BackgroundColour)
                        ToolStrip1.BackColor = Color.FromArgb(BackgroundColour)
                        StationChooser.BackColor = Color.FromArgb(BackgroundColour)
                        Label1.BackColor = Color.FromArgb(BackgroundColour)

                        If BackgroundColour < -8323328 Then
                            RadioString.ForeColor = Color.White
                            TimerString.ForeColor = Color.White
                        Else
                            RadioString.ForeColor = Color.Black
                            TimerString.ForeColor = Color.Black
                        End If

                    End If
                ElseIf splitter(0) = Form2.MultimediaKeys.Name Then
                    MultimediaKeys = splitter(1)
                ElseIf splitter(0) = Form2.HotkeyPlayStop.Name Then
                    HumanModifiersPlayStop = splitter(1)
                    ModifiersPlayStop = splitter(2)
                    KeyPlayStop = splitter(3)
                ElseIf splitter(0) = Form2.HotkeyVolumeUp.Name Then
                    HumanModifiersVolumeUp = splitter(1)
                    ModifiersVolumeUp = splitter(2)
                    KeyVolumeUp = splitter(3)
                ElseIf splitter(0) = Form2.HotkeyVolumeDown.Name Then
                    HumanModifiersVolumeDown = splitter(1)
                    ModifiersVolumeDown = splitter(2)
                    KeyVolumeDown = splitter(3)
                ElseIf splitter(0) = Form2.HotkeyMuteUnmute.Name Then
                    HumanModifiersMuteUnmute = splitter(1)
                    ModifiersMuteUnmute = splitter(2)
                    KeyMuteUnmute = splitter(3)
                ElseIf splitter(0) = Form2.HotkeyShowHide.Name Then
                    HumanModifiersShowHide = splitter(1)
                    ModifiersShowHide = splitter(2)
                    KeyShowHide = splitter(3)
                ElseIf splitter(0) = Form2.Band0.Name Then
                    Band0 = splitter(1)
                ElseIf splitter(0) = Form2.Band1.Name Then
                    Band1 = splitter(1)
                ElseIf splitter(0) = Form2.Band2.Name Then
                    Band2 = splitter(1)
                ElseIf splitter(0) = Form2.Band3.Name Then
                    Band3 = splitter(1)
                ElseIf splitter(0) = Form2.Band4.Name Then
                    Band4 = splitter(1)
                ElseIf splitter(0) = Form2.Band5.Name Then
                    Band5 = splitter(1)
                ElseIf splitter(0) = StationChooser.Name Then

                    RadioStation = splitter(1)

                ElseIf splitter(0) = "DIChannel" Then

                    DIChannel = splitter(1)

                ElseIf splitter(0) = "SkyChannel" Then

                    SKYChannel = splitter(1)

                ElseIf splitter(0) = "JazzChannel" Then

                    JazzChannel = splitter(1)

                ElseIf splitter(0) = Volume.Name Then
                    Volume.Value = splitter(1)
                ElseIf splitter(0) = SelectedServer.Name And SelectedChannel.Text = "My Favorites" Then
                    OldFav = splitter(1)
                End If


            Loop

            reader.Close()

            If RadioStation = DIFM.Text Then
                DIFM_Click(Me, Nothing)
            ElseIf RadioStation = SKYFM.Text Then
                SKYFM_Click(Me, Nothing)
            ElseIf RadioStation = JazzRadio.Text Then
                JazzRadio_Click(Me, Nothing)
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

    Private Sub Seconds_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Seconds.TextChanged

        ' BASS reporting that -01 seconds have passed since playback has started means that connection has been droppped
        ' In that case, if the user isn't using the My Favourites playlist, automatically stop playback, select a new
        ' server and reconnect

        If Seconds.Text = "-01" Then
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

#End Region

End Class
