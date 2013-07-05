Public Class Options

#Region "Global declarations"

    ' HumanModifiers    -> It's used so that, for example, 393216 becomes CTRL+ALT+Space in the options dialog
    ' Modifiers         -> Used to register the combination of modifier keys. For example, 3 registers CTRL+ALT as modifiers
    ' Key               -> The keycode of the non-modifier key. For example, 32 is the space bar

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

    Dim versionnumber As String = Application.ProductVersion
    Dim KeyConverter As New KeysConverter

    Public DISetting As SByte = -1
    Public SKYSetting As SByte = -1
    Public JazzSetting As SByte = -1
    Public DISettingPremium As SByte = -1
    Public SKYSettingPremium As SByte = -1
    Public JazzSettingPremium As SByte = -1

    Public EnableButtons As Boolean = True

    Delegate Sub MsgBoxSafe(ByVal text As String, ByVal style As MsgBoxStyle, ByVal title As String)

    Public username As String
    Public password As String
    Public userInfo As String
    Public isLogged As Boolean
    Public isPremium As Boolean
    Public canTrial As Boolean
    Public apiKey As String
    Public userId As String
    Public radioStation As String

    Dim success As Boolean
    Dim loginAttempts As Byte = 0

    Dim returnedData As Byte()

    Public dataFolder As String

#End Region

#Region "Main Form events"

    Private Sub Options_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        If String.IsNullOrEmpty(My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\DI Radio", "installDir", Nothing)) = True Then
            Dim executable As String = Application.ExecutablePath
            Dim tabla() As String = Split(executable, "\")
            dataFolder = Application.ExecutablePath.Replace(tabla(tabla.Length - 1), Nothing)
        Else
            dataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) & "\DI Radio"
        End If

        AboutLabel.Text = Player.Text
        Copyright.Text += Now.Year.ToString() & " ViRUS"

        ' Load values from the global variables of the main form and accomodate the interface

        If Player.isLogged Then
            isLogged = Player.isLogged
            isPremium = Player.isPremium

            If Player.isPremium = False Then
                DISetting = Player.DIFormat
                JazzSetting = Player.JazzFormat
                SKYSetting = Player.SKYFormat
            Else
                DISettingPremium = Player.DIFormat
                JazzSettingPremium = Player.JazzFormat
                SKYSettingPremium = Player.SKYFormat
            End If

            emailBox.Text = Player.username
            emailBox.ReadOnly = True
            passwordBox.Text = Player.password
            passwordBox.ReadOnly = True
            logIn.Text = "Logout"
            createAccount.Enabled = False
            forgotPass.Enabled = False
            infoLogIn.Hide()
            userInfo = Player.userInfo
            accountOwner.Text = Player.userInfo.Replace("|", " ")

            If Player.isPremium Then
                premiumAccount.Text = "Yes"
            Else
                premiumAccount.Text = "No"
            End If

            If Player.canTrial Then
                premiumTrial.Text = "Available"
                startTrial.Show()
            Else
                premiumTrial.Text = "Not available"
                startTrial.Hide()
            End If

            ListenKey.Text = Player.ListenKey
            apiKey = Player.apiKey
            userId = Player.userId
        Else
            DISetting = Player.DIFormat
            JazzSetting = Player.JazzFormat
            SKYSetting = Player.SKYFormat
        End If

        StationSelector_SelectedIndexChanged(Me, Nothing)

        NotificationTitle.Checked = Player.NotificationTitle
        Visualisation.Checked = Player.Visualisation
        NotificationIcon.Checked = Player.NotificationIcon
        NoTaskbarButton.Checked = Player.NoTaskbarButton
        MultimediaKeys.Checked = Player.MultimediaKeys
        GoogleSearch.Checked = Player.GoogleSearch
        ShowSongStart.Checked = Player.ShowSongStart
        removeListenKey.Checked = Player.removeKey
        cacheList.SelectedItem = Player.cacheList
        StationSelector.SelectedItem = Player.StationChooser.Text

        If StationSelector.Text = "Digitally Imported" Then
            FileFormat.SelectedIndex = Player.DIFormat
        ElseIf StationSelector.Text = "JazzRadio" Then
            FileFormat.SelectedIndex = Player.JazzFormat
        ElseIf StationSelector.Text = "SKY.FM" Then
            FileFormat.SelectedIndex = Player.SKYFormat
        End If

        UpdatesAtStart.Checked = Player.UpdatesAtStart
        BetaVersions.Checked = Player.BetaVersions
        VisualisationType.SelectedIndex = Player.VisualisationType
        HighQualityVis.Checked = Player.HighQualityVis
        LinealRepresentation.Checked = Player.LinealRepresentation
        FullSoundRange.Checked = Player.FullSoundRange
        PlayNewOnChannelChange.Checked = Player.PlayNewOnChannelChange

        If Player.Smoothness >= 15 And Player.Smoothness <= 40 Then
            Smoothness.Value = Player.Smoothness
        Else
            Smoothness.Value = 27
        End If

        MainColour.BackColor = Color.FromArgb(Player.MainColour)
        SecondaryColour.BackColor = Color.FromArgb(Player.SecondaryColour)
        PeakColour.BackColor = Color.FromArgb(Player.PeakColour)
        BackgroundColour.BackColor = Color.FromArgb(Player.BackgroundColour)
        ChangeWholeBackground.Checked = Player.ChangeWholeBackground

        ModifiersPlayStop = Player.ModifiersPlayStop
        HumanModifiersPlayStop = Player.HumanModifiersPlayStop
        KeyPlayStop = Player.KeyPlayStop

        If ModifiersPlayStop = Nothing = False And KeyPlayStop = Keys.MediaPlayPause = False OrElse ModifiersPlayStop = Nothing And KeyPlayStop = Keys.MediaPlayPause = False And KeyPlayStop = Nothing = False Then
            HotkeyPlayStop.Checked = True
            CustomPlayStop.Text = KeyConverter.ConvertToString(HumanModifiersPlayStop + KeyPlayStop).Replace("None", Nothing)
            CustomPlayStop.Font = New Font(CustomMuteUnmute.Font, FontStyle.Regular)
            CustomPlayStop.ForeColor = SystemColors.WindowText
        End If

        ModifiersVolumeUp = Player.ModifiersVolumeUp
        HumanModifiersVolumeUp = Player.HumanModifiersVolumeUp
        KeyVolumeUp = Player.KeyVolumeUp

        If ModifiersVolumeUp = Nothing = False And KeyVolumeUp = Keys.VolumeUp = False OrElse ModifiersVolumeUp = Nothing And KeyVolumeUp = Keys.VolumeUp = False And KeyVolumeUp = Nothing = False Then
            HotkeyVolumeUp.Checked = True
            CustomVolumeUp.Text = KeyConverter.ConvertToString(HumanModifiersVolumeUp + KeyVolumeUp).Replace("None", Nothing)
            CustomVolumeUp.Font = New Font(CustomMuteUnmute.Font, FontStyle.Regular)
            CustomVolumeUp.ForeColor = SystemColors.WindowText
        End If

        ModifiersVolumeDown = Player.ModifiersVolumeDown
        HumanModifiersVolumeDown = Player.HumanModifiersVolumeDown
        KeyVolumeDown = Player.KeyVolumeDown

        If ModifiersVolumeDown = Nothing = False And KeyVolumeDown = Keys.VolumeDown = False OrElse ModifiersVolumeDown = Nothing And KeyVolumeDown = Keys.VolumeDown = False And KeyVolumeDown = Nothing = False Then
            HotkeyVolumeDown.Checked = True
            CustomVolumeDown.Text = KeyConverter.ConvertToString(HumanModifiersVolumeDown + KeyVolumeDown).Replace("None", Nothing)
            CustomVolumeDown.Font = New Font(CustomMuteUnmute.Font, FontStyle.Regular)
            CustomVolumeDown.ForeColor = SystemColors.WindowText
        End If

        ModifiersMuteUnmute = Player.ModifiersMuteUnmute
        HumanModifiersMuteUnmute = Player.HumanModifiersMuteUnmute
        KeyMuteUnmute = Player.KeyMuteUnmute

        If ModifiersMuteUnmute = Nothing = False And KeyMuteUnmute = Keys.VolumeMute = False OrElse ModifiersMuteUnmute = Nothing And KeyMuteUnmute = Keys.VolumeMute = False And KeyMuteUnmute = Nothing = False Then
            HotkeyMuteUnmute.Checked = True
            CustomMuteUnmute.Text = KeyConverter.ConvertToString(HumanModifiersMuteUnmute + KeyMuteUnmute).Replace("None", Nothing)
            CustomMuteUnmute.Font = New Font(CustomMuteUnmute.Font, FontStyle.Regular)
            CustomMuteUnmute.ForeColor = SystemColors.WindowText
        End If


        ModifiersShowHide = Player.ModifiersShowHide
        HumanModifiersShowHide = Player.HumanModifiersShowHide
        KeyShowHide = Player.KeyShowHide

        If ModifiersShowHide = "196608" = False And KeyShowHide = "36" = False Then
            HotkeyShowHide.Checked = True
            CustomShowHide.Text = KeyConverter.ConvertToString(HumanModifiersShowHide + KeyShowHide).Replace("None", Nothing)
        End If

        Band0.Value = Player.Band0
        Band1.Value = Player.Band1
        Band2.Value = Player.Band2
        Band3.Value = Player.Band3
        Band4.Value = Player.Band4
        Band5.Value = Player.Band5

        If Player.GetUpdates.IsBusy = True Then
            LookNow.Enabled = False
            UndefinedProgress.Show()
            Status.Text = "Status: Looking for updates, please wait..."
        End If

        If Player.TotalVersionString = "Didn't download" = False Then

            If Player.TotalVersionString > Player.TotalVersionFixed Then
                LookNow.Text = "Download update"
                LatestVersion.ForeColor = Color.Green
            End If

        End If

        Dim file As String = dataFolder & "\Updater.exe"

        If Player.UpdaterDownloaded = True And My.Computer.FileSystem.FileExists(file) Then
            LookNow.Text = "Run updater"
            LookNow.Enabled = True
            Status.Text = "Status: Updater downloaded. Click button to launch."
        End If

        CurrentVersion.Text = "Current version:" & Player.Text.Replace("DI Radio Player", Nothing)
        CurrentVersion.Text = CurrentVersion.Text.Replace("SKY.FM Radio Player", Nothing)
        CurrentVersion.Text = CurrentVersion.Text.Replace("JazzRadio Radio Player", Nothing)
        CurrentVersion.Text = CurrentVersion.Text.Replace("RockRadio Radio Player", Nothing)

        If Player.LatestVersionString = "Didn't download" = False Then
            Dim FullLine As String = Player.LatestVersionString
            Dim Splitter As String() = Split(FullLine, ".")

            If Splitter(2) > 0 Then

                If Splitter(1) = 9 Then
                    LatestVersion.Text = "Latest version: " & Splitter(0) + 1 & ".0" & " Beta " & Splitter(2)
                Else
                    LatestVersion.Text = "Latest version: " & Splitter(0) & "." & Splitter(1) + 1 & " Beta " & Splitter(2)
                End If

            Else

                LatestVersion.Text = "Latest version: " & Splitter(0) & "." & Splitter(1)

            End If
        End If

        TabControl1.SelectedIndex = 0
        EnableButtons = True

        OK.Enabled = False
        Cancel.Text = "Close"
        Apply.Enabled = False

        If isLogged = False Then
            qualityInfo.Text = "Only Free quality options are available for anonymous users."
        End If

        lookForChannels.Enabled = Player.StationChooser.Enabled

    End Sub

    Private Sub Options_Move(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Move
        OnlyModifiers.Hide(Me)
    End Sub

    Private Sub Options_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Changelog.Close()
        Gallery.Close()
        Signup.Close()
    End Sub

#End Region

#Region "Events caused by the controls in the form"

    ' These only enable the OK and Apply buttons and change the Close button's text to Cancel

    Private Sub NotificationTitle_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles NotificationTitle.Click
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub PlayNewOnChannelChange_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PlayNewOnChannelChange.CheckedChanged
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub NotificationIcon_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles NotificationIcon.Click
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub NoTaskbarButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NoTaskbarButton.CheckedChanged
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub GoogleSearch_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GoogleSearch.CheckedChanged
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub ShowSongStart_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowSongStart.CheckedChanged
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub UpdatesAtStart_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles UpdatesAtStart.Click
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub BetaVersions_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BetaVersions.Click
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub Visualisation_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Visualisation.Click
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub HighQualityVis_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles HighQualityVis.Click
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub LinealRepresentation_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LinealRepresentation.Click
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub FullSoundRange_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles FullSoundRange.Click
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub Smoothness_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Smoothness.Scroll
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub MultimediaKeys_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MultimediaKeys.CheckedChanged
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub VisualisationType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles VisualisationType.SelectedIndexChanged
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    ' -----------------------------------------------------------

    Private Sub StationSelector_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StationSelector.SelectedIndexChanged
        EnableButtons = False

        If StationSelector.Text = "Digitally Imported" Then
            FileFormat.Enabled = True

            If isPremium Then
                FileFormat.Items.Clear()
                FileFormat.Items.Add("Excellent (128k AAC)")
                FileFormat.Items.Add("Excellent (256k MP3)")
                FileFormat.Items.Add("Good (64k AAC-HE)")
                FileFormat.Items.Add("Low (40k AAC-HE)")

                If DISettingPremium > -1 Then
                    FileFormat.SelectedIndex = DISettingPremium
                Else
                    FileFormat.SelectedIndex = 1
                End If

            Else
                FileFormat.Items.Clear()
                FileFormat.Items.Add("Good (64k AAC-HE)")
                FileFormat.Items.Add("Good (96k MP3)")
                FileFormat.Items.Add("Low (40k AAC-HE)")

                If DISetting > -1 Then
                    FileFormat.SelectedIndex = DISetting
                Else
                    FileFormat.SelectedIndex = 1
                End If

            End If
        ElseIf StationSelector.Text = "SKY.FM" Then
            FileFormat.Enabled = True

            If isPremium Then
                FileFormat.Items.Clear()
                FileFormat.Items.Add("Excellent (128k AAC)")
                FileFormat.Items.Add("Excellent (256k MP3)")
                FileFormat.Items.Add("Excellent (128k WMA)")
                FileFormat.Items.Add("Good (64k AAC-HE)")
                FileFormat.Items.Add("Good (64k WMA)")
                FileFormat.Items.Add("Low (40k AAC-HE)")

                If SKYSettingPremium > -1 Then
                    FileFormat.SelectedIndex = SKYSettingPremium
                Else
                    FileFormat.SelectedIndex = 1
                End If

            Else
                FileFormat.Items.Clear()
                FileFormat.Items.Add("Good (96k MP3)")
                FileFormat.Items.Add("Good (40k AAC-HE)")
                FileFormat.Items.Add("Good (40k WMA)")


                If SKYSetting > -1 Then
                    FileFormat.SelectedIndex = SKYSetting
                Else
                    FileFormat.SelectedIndex = 0
                End If

            End If
        ElseIf StationSelector.Text = "JazzRadio" Then
            FileFormat.Enabled = True

            If isPremium Then
                FileFormat.Items.Clear()
                FileFormat.Items.Add("Excellent (128k AAC)")
                FileFormat.Items.Add("Excellent (256k MP3)")
                FileFormat.Items.Add("Excellent (128k WMA)")
                FileFormat.Items.Add("Good (64k AAC-HE)")

                If JazzSettingPremium > -1 Then
                    FileFormat.SelectedIndex = JazzSettingPremium
                Else
                    FileFormat.SelectedIndex = 1
                End If

            Else
                FileFormat.Items.Clear()
                FileFormat.Items.Add("Good (64k MP3)")
                FileFormat.Items.Add("Good (40k AAC-HE)")
                FileFormat.Items.Add("Good (40k WMA)")

                If JazzSetting > -1 Then
                    FileFormat.SelectedIndex = JazzSetting
                Else
                    FileFormat.SelectedIndex = 0
                End If

            End If
        ElseIf StationSelector.Text = "RockRadio" Then
            FileFormat.Items.Clear()
            FileFormat.Items.Add("Good (96k MP3)")
            FileFormat.SelectedIndex = 0
            FileFormat.Enabled = False
        End If

        If isPremium And StationSelector.Text = "RockRadio" = False Then
            qualityInfo.Text = "Showing Premium quality options."
        ElseIf isPremium = False And StationSelector.Text = "RockRadio" = False And isLogged Then
            qualityInfo.Text = "Showing Free quality options."
        ElseIf StationSelector.Text = "RockRadio" Then
            qualityInfo.Text = "RockRadio only supports MP3 at 96kbits/sec."
        ElseIf isLogged = False Then
            qualityInfo.Text = "Only Free quality options are available for anonymous users."
        End If
    End Sub

    Private Sub FileFormat_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FileFormat.SelectedIndexChanged

        If isPremium Then
            If StationSelector.Text = "Digitally Imported" Then
                DISettingPremium = FileFormat.SelectedIndex
            ElseIf StationSelector.Text = "JazzRadio" Then
                JazzSettingPremium = FileFormat.SelectedIndex
            ElseIf StationSelector.Text = "SKY.FM" Then
                SKYSettingPremium = FileFormat.SelectedIndex
            End If
        Else
            If StationSelector.Text = "Digitally Imported" Then
                DISetting = FileFormat.SelectedIndex
            ElseIf StationSelector.Text = "JazzRadio" Then
                JazzSetting = FileFormat.SelectedIndex
            ElseIf StationSelector.Text = "SKY.FM" Then
                SKYSetting = FileFormat.SelectedIndex
            End If
        End If

        If EnableButtons = False Then
            EnableButtons = True
        Else
            OK.Enabled = True
            Cancel.Text = "Cancel"
            Apply.Enabled = True
        End If

    End Sub

    Public Sub LookNow_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LookNow.Click
        If LookNow.Text = "Look for updates now" Then
            LookNow.Enabled = False
            UndefinedProgress.Show()
            Status.Text = "Status: Looking for updates, please wait..."

            Player.GetUpdates.RunWorkerAsync()
        ElseIf LookNow.Text = "Download update" Then
            DownloadUpdater.RunWorkerAsync()
        Else
            DownloadUpdater_RunWorkerCompleted(Me, Nothing)
        End If
    End Sub

    Private Sub ViewChangelog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ViewChangelog.Click
        Changelog.Location = New Point(Me.Location.X + 5, Me.Location.Y + 28)
        Changelog.Show()
        Changelog.BringToFront()
    End Sub

    Private Sub Band0_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Band0.ValueChanged
        If Band0.Value >= 0 Then
            Band1db.Text = "+" & Band0.Value & "dB"
        Else
            Band1db.Text = Band0.Value & "dB"
        End If

        Player.UpdateEq(0, Band0.Value)

        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub Band1_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Band1.ValueChanged
        If Band1.Value >= 0 Then
            Band2db.Text = "+" & Band1.Value & "dB"
        Else
            Band2db.Text = Band1.Value & "dB"
        End If

        Player.UpdateEq(1, Band1.Value)

        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub Band2_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Band2.ValueChanged
        If Band2.Value >= 0 Then
            Band3db.Text = "+" & Band2.Value & "dB"
        Else
            Band3db.Text = Band2.Value & "dB"
        End If

        Player.UpdateEq(2, Band2.Value)

        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub Band3_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Band3.ValueChanged
        If Band3.Value >= 0 Then
            Band4db.Text = "+" & Band3.Value & "dB"
        Else
            Band4db.Text = Band3.Value & "dB"
        End If

        Player.UpdateEq(3, Band3.Value)

        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub Band4_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Band4.ValueChanged
        If Band4.Value >= 0 Then
            Band5db.Text = "+" & Band4.Value & "dB"
        Else
            Band5db.Text = Band4.Value & "dB"
        End If

        Player.UpdateEq(4, Band4.Value)

        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub Band5_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Band5.ValueChanged
        If Band5.Value >= 0 Then
            Band6db.Text = "+" & Band5.Value & "dB"
        Else
            Band6db.Text = Band5.Value & "dB"
        End If

        Player.UpdateEq(5, Band5.Value)

        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub Zero_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Zero.Click
        Band0.Value = 0
        Band1.Value = 0
        Band2.Value = 0
        Band3.Value = 0
        Band4.Value = 0
        Band5.Value = 0
    End Sub

    Private Sub RestoreEq_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RestoreEq.Click
        Band0.Value = Player.Band0
        Band1.Value = Player.Band1
        Band2.Value = Player.Band2
        Band3.Value = Player.Band3
        Band4.Value = Player.Band4
        Band5.Value = Player.Band5
    End Sub

    Private Sub AutoEq_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AutoEq.Click
        Dim Highest As SByte

        Highest = Band0.Value

        If Highest < Band1.Value Then
            Highest = Band1.Value
        End If

        If Highest < Band2.Value Then
            Highest = Band2.Value
        End If

        If Highest < Band3.Value Then
            Highest = Band3.Value
        End If

        If Highest < Band4.Value Then
            Highest = Band4.Value
        End If

        If Highest < Band5.Value Then
            Highest = Band5.Value
        End If

        If Band0.Value - Highest < Band0.Minimum Then
            Band0.Value = Band0.Minimum
        Else
            Band0.Value = Band0.Value - Highest
        End If

        If Band1.Value - Highest < Band1.Minimum Then
            Band1.Value = Band1.Minimum
        Else
            Band1.Value = Band1.Value - Highest
        End If

        If Band2.Value - Highest < Band2.Minimum Then
            Band2.Value = Band2.Minimum
        Else
            Band2.Value = Band2.Value - Highest
        End If

        If Band3.Value - Highest < Band3.Minimum Then
            Band3.Value = Band3.Minimum
        Else
            Band3.Value = Band3.Value - Highest
        End If

        If Band4.Value - Highest < Band4.Minimum Then
            Band4.Value = Band4.Minimum
        Else
            Band4.Value = Band4.Value - Highest
        End If

        If Band5.Value - Highest < Band5.Minimum Then
            Band5.Value = Band5.Minimum
        Else
            Band5.Value = Band5.Value - Highest
        End If
    End Sub

    Private Sub OK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK.Click
        ApplyOptions()
        Me.Close()
        Player.BringToFront()
    End Sub

    Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click
        Me.Close()

        Player.UpdateEq(0, Player.Band0)
        Player.UpdateEq(1, Player.Band1)
        Player.UpdateEq(2, Player.Band2)
        Player.UpdateEq(3, Player.Band3)
        Player.UpdateEq(4, Player.Band4)
        Player.UpdateEq(5, Player.Band5)
    End Sub

    Private Sub Apply_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Apply.Click

        ApplyOptions()
        OK.Enabled = False
        Cancel.Text = "Close"
        Apply.Enabled = False
        Me.BringToFront()

    End Sub

    Private Sub TabControl1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabControl1.SelectedIndexChanged
        OnlyModifiers.Hide(Me)
    End Sub

    Private Sub createAccount_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles createAccount.Click
        Signup.Location = New Point(Me.Location.X + 87, Me.Location.Y + 83)
        Signup.eMail.Text = emailBox.Text
        Signup.passWord.Text = passwordBox.Text
        Signup.retypePassword.Text = passwordBox.Text
        Signup.Show()
    End Sub

    Public Sub logIn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles logIn.Click

        If logIn.Text = "Login" Then

            If CheckEmail(emailBox.Text) And passwordBox.Text.Length >= 5 Then
                emailBox.ReadOnly = True
                passwordBox.ReadOnly = True
                logIn.Enabled = False
                pleaseWait.Text = "Please wait, logging in..."
                pleaseWait.Show()
                StationSelector.Enabled = False
                FileFormat.Enabled = False
                forgotPass.Enabled = False
                logInWorker.RunWorkerAsync()
            ElseIf CheckEmail(emailBox.Text) = False Then
                MessageBox.Show("The Email is invalid.", "Login error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            ElseIf passwordBox.Text.Length < 5 Then
                MessageBox.Show("The password must have at least 5 characters.", "Login error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

        Else

            emailBox.Clear()
            passwordBox.Clear()
            emailBox.ReadOnly = False
            passwordBox.ReadOnly = False
            createAccount.Enabled = True
            forgotPass.Enabled = True
            logIn.Text = "Login"
            infoLogIn.Show()
            isPremium = False
            isLogged = False
            StationSelector_SelectedIndexChanged(Me, Nothing)
            OK.Enabled = True
            Cancel.Text = "Cancel"
            Apply.Enabled = True

        End If


    End Sub

    Private Sub forgotPass_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles forgotPass.Click
        If CheckEmail(emailBox.Text) Then
            If MessageBox.Show("Are you sure you want to receive an Email to reset your password?", "Password reset", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                emailBox.ReadOnly = True
                passwordBox.ReadOnly = True
                logIn.Enabled = False
                pleaseWait.Text = "Please wait..."
                pleaseWait.Show()
                forgotPass.Enabled = False

                resetPasswordWorker.RunWorkerAsync()
            End If
        Else
            MessageBox.Show("The Email is invalid.", "Password reset error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If

    End Sub

    Private Sub refreshInfo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles refreshInfo.Click
        pleaseWaitRefresh.Text = "Please wait, refreshing account information..."
        pleaseWaitRefresh.Show()
        logIn.Enabled = False
        StationSelector.Enabled = False
        FileFormat.Enabled = False
        refreshInfo.Enabled = False
        startTrial.Enabled = False
        logInWorker.RunWorkerAsync()
    End Sub

    Private Sub startTrial_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles startTrial.Click
        pleaseWaitRefresh.Text = "Please wait, starting trial offer..."
        pleaseWaitRefresh.Show()
        logIn.Enabled = False
        StationSelector.Enabled = False
        FileFormat.Enabled = False
        refreshInfo.Enabled = False
        startTrial.Enabled = False
        startTrialWorker.RunWorkerAsync()
    End Sub

    Private Sub passWord_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles passwordBox.KeyDown
        If e.KeyCode = Keys.Enter And passwordBox.ReadOnly = False And logIn.Enabled = True Then
            logIn_Click(Me, Nothing)
        End If
    End Sub

    Private Sub CopyListenKeyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyListenKeyToolStripMenuItem.Click
        Clipboard.Clear()
        Clipboard.SetDataObject(ListenKey.Text)
    End Sub

    Private Sub lookForChannels_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lookForChannels.Click
        My.Computer.FileSystem.DeleteDirectory("servers", FileIO.DeleteDirectoryOption.DeleteAllContents)
        Player.StationChooser_TextChanged(Me, Nothing)
        lookForChannels.Enabled = False
    End Sub

    Private Sub cacheList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cacheList.SelectedIndexChanged
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub removeListenKey_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles removeListenKey.CheckedChanged
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub payPal_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles payPal.Click
        Process.Start("http://www.paypal.com/cgi-bin/webscr?cmd=_flow&SESSION=CoGnN_73KfMI27PUA1Ecqg1g7GCA781wniihgiJJF9T8XokaV7YMD6IHXf8&dispatch=5885d80a13c0db1f8e263663d3faee8d4e181b3aff599f99e8c17bd6c7fe2f56")
    End Sub

    Private Sub flattr_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles flattr.Click
        Process.Start("http://flattr.com/thing/1352129/DI-Radio")
    End Sub

    Private Sub emailBox_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles emailBox.KeyDown
        If e.KeyCode = Keys.Enter And emailBox.ReadOnly = False And logIn.Enabled = True Then
            logIn_Click(Me, Nothing)
        End If
    End Sub

#End Region

#Region "Hotkeys code"

    Private Sub HotkeyPlayStop_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HotkeyPlayStop.CheckedChanged

        If HotkeyPlayStop.Checked = True Then
            CustomPlayStop.Enabled = True
            CustomPlayStop.Focus()
            CustomPlayStop.Text = "Press keys now"
            CustomPlayStop.Font = New Font(CustomPlayStop.Font, FontStyle.Italic)
            CustomPlayStop.ForeColor = Color.Gray
        Else
            ModifiersPlayStop = Nothing
            HumanModifiersPlayStop = Nothing
            KeyPlayStop = Keys.MediaStop
            CustomPlayStop.Enabled = False
            CustomPlayStop.Text = "Multimedia Play/Pause/Stop"
            CustomPlayStop.Font = New Font(CustomPlayStop.Font, FontStyle.Regular)
        End If

    End Sub

    Private Sub CustomPlayStop_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CustomPlayStop.Click
        CustomPlayStop.Text = "Press keys now"
        CustomPlayStop.Font = New Font(CustomPlayStop.Font, FontStyle.Italic)
        CustomPlayStop.ForeColor = Color.Gray
    End Sub

    Private Sub CustomPlayStop_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles CustomPlayStop.KeyDown
        If e.KeyValue = "16" Then
            ModifiersPlayStop += HotKeyModifiers.MOD_SHIFT
            HumanModifiersPlayStop = 0
            KeyPlayStop = 0
        ElseIf e.KeyValue = "17" Then
            ModifiersPlayStop += HotKeyModifiers.MOD_CONTROL
            HumanModifiersPlayStop = 0
            KeyPlayStop = 0
        ElseIf e.KeyValue = "18" Then
            ModifiersPlayStop += HotKeyModifiers.MOD_ALT
            HumanModifiersPlayStop = 0
            KeyPlayStop = 0
        Else
            HumanModifiersPlayStop = e.Modifiers
            KeyPlayStop = e.KeyValue
        End If
    End Sub

    Private Sub CustomPlayStop_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles CustomPlayStop.KeyUp
        CustomPlayStop.Text = KeyConverter.ConvertToString(HumanModifiersPlayStop + KeyPlayStop).Replace("None", Nothing)

        If KeyConverter.ConvertToString(HumanModifiersPlayStop + KeyPlayStop).Replace("None", Nothing) = Nothing = False Then
            CustomPlayStop.Font = New Font(CustomPlayStop.Font, FontStyle.Regular)
            CustomPlayStop.ForeColor = SystemColors.WindowText
        End If

    End Sub

    Private Sub HotkeyVolumeUp_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HotkeyVolumeUp.CheckedChanged
        If HotkeyVolumeUp.Checked = True Then
            CustomVolumeUp.Enabled = True
            CustomVolumeUp.Focus()
            CustomVolumeUp.Text = "Press keys now"
            CustomVolumeUp.Font = New Font(CustomVolumeUp.Font, FontStyle.Italic)
            CustomVolumeUp.ForeColor = Color.Gray
        Else
            ModifiersVolumeUp = Nothing
            HumanModifiersVolumeUp = Nothing
            KeyVolumeUp = Keys.VolumeUp
            CustomVolumeUp.Enabled = False
            CustomVolumeUp.Text = "Multimedia Volume Up"
            CustomVolumeUp.Font = New Font(CustomPlayStop.Font, FontStyle.Regular)
        End If
    End Sub

    Private Sub CustomVolumeUp_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CustomVolumeUp.Click
        CustomVolumeUp.Text = "Press keys now"
        CustomVolumeUp.Font = New Font(CustomVolumeUp.Font, FontStyle.Italic)
        CustomVolumeUp.ForeColor = Color.Gray
    End Sub

    Private Sub CustomVolumeUp_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles CustomVolumeUp.KeyDown
        If e.KeyValue = "16" Then
            ModifiersVolumeUp += HotKeyModifiers.MOD_SHIFT
            HumanModifiersVolumeUp = 0
            KeyVolumeUp = 0
        ElseIf e.KeyValue = "17" Then
            ModifiersVolumeUp += HotKeyModifiers.MOD_CONTROL
            HumanModifiersVolumeUp = 0
            KeyVolumeUp = 0
        ElseIf e.KeyValue = "18" Then
            ModifiersVolumeUp += HotKeyModifiers.MOD_ALT
            HumanModifiersVolumeUp = 0
            KeyVolumeUp = 0
        Else
            HumanModifiersVolumeUp = e.Modifiers
            KeyVolumeUp = e.KeyValue
        End If
    End Sub

    Private Sub CustomVolumeUp_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles CustomVolumeUp.KeyUp
        CustomVolumeUp.Text = KeyConverter.ConvertToString(HumanModifiersVolumeUp + KeyVolumeUp).Replace("None", Nothing)

        If KeyConverter.ConvertToString(HumanModifiersVolumeUp + KeyVolumeUp).Replace("None", Nothing) = Nothing = False Then
            CustomVolumeUp.Font = New Font(CustomVolumeUp.Font, FontStyle.Regular)
            CustomVolumeUp.ForeColor = SystemColors.WindowText
        End If
    End Sub

    Private Sub HotkeyVolumeDown_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HotkeyVolumeDown.CheckedChanged
        If HotkeyVolumeDown.Checked = True Then
            CustomVolumeDown.Enabled = True
            CustomVolumeDown.Focus()
            CustomVolumeDown.Text = "Press keys now"
            CustomVolumeDown.Font = New Font(CustomVolumeDown.Font, FontStyle.Italic)
            CustomVolumeDown.ForeColor = Color.Gray
        Else
            ModifiersVolumeDown = Nothing
            HumanModifiersVolumeDown = Nothing
            KeyVolumeDown = Keys.VolumeDown
            CustomVolumeDown.Enabled = False
            CustomVolumeDown.Text = "Multimedia Volume Down"
            CustomVolumeDown.Font = New Font(CustomPlayStop.Font, FontStyle.Regular)
        End If
    End Sub

    Private Sub CustomVolumeDown_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CustomVolumeDown.Click
        CustomVolumeDown.Text = "Press keys now"
        CustomVolumeDown.Font = New Font(CustomVolumeDown.Font, FontStyle.Italic)
        CustomVolumeDown.ForeColor = Color.Gray
    End Sub

    Private Sub CustomVolumeDown_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles CustomVolumeDown.KeyDown
        If e.KeyValue = "16" Then
            ModifiersVolumeDown += HotKeyModifiers.MOD_SHIFT
            HumanModifiersVolumeDown = 0
            KeyVolumeDown = 0
        ElseIf e.KeyValue = "17" Then
            ModifiersVolumeDown += HotKeyModifiers.MOD_CONTROL
            HumanModifiersVolumeDown = 0
            KeyVolumeDown = 0
        ElseIf e.KeyValue = "18" Then
            ModifiersVolumeDown += HotKeyModifiers.MOD_ALT
            HumanModifiersVolumeDown = 0
            KeyVolumeDown = 0
        Else
            HumanModifiersVolumeDown = e.Modifiers
            KeyVolumeDown = e.KeyValue
        End If
    End Sub

    Private Sub CustomVolumeDown_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles CustomVolumeDown.KeyUp
        CustomVolumeDown.Text = KeyConverter.ConvertToString(HumanModifiersVolumeDown + KeyVolumeDown).Replace("None", Nothing)
        If KeyConverter.ConvertToString(HumanModifiersVolumeDown + KeyVolumeDown).Replace("None", Nothing) = Nothing = False Then
            CustomVolumeDown.Font = New Font(CustomVolumeDown.Font, FontStyle.Regular)
            CustomVolumeDown.ForeColor = SystemColors.WindowText
        End If
    End Sub

    Private Sub HotkeyMuteUnmute_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HotkeyMuteUnmute.CheckedChanged
        If HotkeyMuteUnmute.Checked = True Then
            CustomMuteUnmute.Enabled = True
            CustomMuteUnmute.Focus()
            CustomMuteUnmute.Text = "Press keys now"
            CustomMuteUnmute.Font = New Font(CustomMuteUnmute.Font, FontStyle.Italic)
            CustomMuteUnmute.ForeColor = Color.Gray
        Else
            ModifiersMuteUnmute = Nothing
            HumanModifiersMuteUnmute = Nothing
            KeyMuteUnmute = Keys.VolumeMute
            CustomMuteUnmute.Enabled = False
            CustomMuteUnmute.Text = "Multimedia Mute/Unmute"
            CustomMuteUnmute.Font = New Font(CustomPlayStop.Font, FontStyle.Regular)
        End If
    End Sub

    Private Sub CustomMuteUnmute_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CustomMuteUnmute.Click
        CustomMuteUnmute.Text = "Press keys now"
        CustomMuteUnmute.Font = New Font(CustomMuteUnmute.Font, FontStyle.Italic)
        CustomMuteUnmute.ForeColor = Color.Gray
    End Sub

    Private Sub CustomMuteUnmute_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles CustomMuteUnmute.KeyDown
        If e.KeyValue = "16" Then
            ModifiersMuteUnmute += HotKeyModifiers.MOD_SHIFT
            HumanModifiersMuteUnmute = 0
            KeyMuteUnmute = 0
        ElseIf e.KeyValue = "17" Then
            ModifiersMuteUnmute += HotKeyModifiers.MOD_CONTROL
            HumanModifiersMuteUnmute = 0
            KeyMuteUnmute = 0
        ElseIf e.KeyValue = "18" Then
            ModifiersMuteUnmute += HotKeyModifiers.MOD_ALT
            HumanModifiersMuteUnmute = 0
            KeyMuteUnmute = 0
        Else
            HumanModifiersMuteUnmute = e.Modifiers
            KeyMuteUnmute = e.KeyValue
        End If
    End Sub

    Private Sub CustomMuteUnmute_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles CustomMuteUnmute.KeyUp
        CustomMuteUnmute.Text = KeyConverter.ConvertToString(HumanModifiersMuteUnmute + KeyMuteUnmute).Replace("None", Nothing)
        If KeyConverter.ConvertToString(HumanModifiersMuteUnmute + KeyMuteUnmute).Replace("None", Nothing) = Nothing = False Then
            CustomMuteUnmute.Font = New Font(CustomMuteUnmute.Font, FontStyle.Regular)
            CustomMuteUnmute.ForeColor = SystemColors.WindowText
        End If
    End Sub

    Private Sub CustomPlayStop_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CustomPlayStop.TextChanged
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
        OnlyModifiers.Hide(CustomPlayStop)

        If CustomPlayStop.Text = Nothing Then
            CustomPlayStop.Text = "Press keys now"
            CustomPlayStop.Font = New Font(CustomPlayStop.Font, FontStyle.Italic)
            CustomPlayStop.ForeColor = Color.Gray
            OnlyModifiers.Show("Please use a combination that includes both" & vbNewLine & "modifier keys and non-modifier keys or only" & vbNewLine & "non-modifier keys.", CustomPlayStop)
            OnlyModifiers.Show("Please use a combination that includes both" & vbNewLine & "modifier keys and non-modifier keys or only" & vbNewLine & "non-modifier keys.", CustomPlayStop)
        End If

    End Sub

    Private Sub CustomVolumeUp_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CustomVolumeUp.TextChanged
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
        OnlyModifiers.Hide(CustomVolumeUp)
        If CustomVolumeUp.Text = Nothing Then
            CustomVolumeUp.Text = "Press keys now"
            CustomVolumeUp.Font = New Font(CustomVolumeUp.Font, FontStyle.Italic)
            CustomVolumeUp.ForeColor = Color.Gray
            OnlyModifiers.Show("Please use a combination that includes both" & vbNewLine & "modifier keys and non-modifier keys or only" & vbNewLine & "non-modifier keys.", CustomVolumeUp)
            OnlyModifiers.Show("Please use a combination that includes both" & vbNewLine & "modifier keys and non-modifier keys or only" & vbNewLine & "non-modifier keys.", CustomVolumeUp)
        End If
    End Sub

    Private Sub CustomVolumeDown_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CustomVolumeDown.TextChanged
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
        OnlyModifiers.Hide(CustomVolumeDown)
        If CustomVolumeDown.Text = Nothing Then
            CustomVolumeDown.Text = "Press keys now"
            CustomVolumeDown.Font = New Font(CustomVolumeDown.Font, FontStyle.Italic)
            CustomVolumeDown.ForeColor = Color.Gray
            OnlyModifiers.Show("Please use a combination that includes both" & vbNewLine & "modifier keys and non-modifier keys or only" & vbNewLine & "non-modifier keys.", CustomVolumeDown)
            OnlyModifiers.Show("Please use a combination that includes both" & vbNewLine & "modifier keys and non-modifier keys or only" & vbNewLine & "non-modifier keys.", CustomVolumeDown)
        End If
    End Sub

    Private Sub CustomMuteUnmute_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CustomMuteUnmute.TextChanged
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
        OnlyModifiers.Hide(CustomMuteUnmute)
        If CustomMuteUnmute.Text = Nothing Then
            CustomMuteUnmute.Text = "Press keys now"
            CustomMuteUnmute.Font = New Font(CustomMuteUnmute.Font, FontStyle.Italic)
            CustomMuteUnmute.ForeColor = Color.Gray
            OnlyModifiers.Show("Please use a combination that includes both" & vbNewLine & "modifier keys and non-modifier keys or only" & vbNewLine & "non-modifier keys.", CustomMuteUnmute)
            OnlyModifiers.Show("Please use a combination that includes both" & vbNewLine & "modifier keys and non-modifier keys or only" & vbNewLine & "non-modifier keys.", CustomMuteUnmute)
        End If
    End Sub

    Private Sub CustomShowHide_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CustomShowHide.Click
        CustomShowHide.Text = "Press keys now"
        CustomShowHide.Font = New Font(CustomShowHide.Font, FontStyle.Italic)
        CustomShowHide.ForeColor = Color.Gray
    End Sub

    Private Sub CustomShowHide_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles CustomShowHide.KeyDown
        If e.KeyValue = "16" Then
            ModifiersShowHide += HotKeyModifiers.MOD_SHIFT
            HumanModifiersShowHide = 0
            KeyShowHide = 0
        ElseIf e.KeyValue = "17" Then
            ModifiersShowHide += HotKeyModifiers.MOD_CONTROL
            HumanModifiersShowHide = 0
            KeyShowHide = 0
        ElseIf e.KeyValue = "18" Then
            ModifiersShowHide += HotKeyModifiers.MOD_ALT
            HumanModifiersShowHide = 0
            KeyShowHide = 0
        Else
            HumanModifiersShowHide = e.Modifiers
            KeyShowHide = e.KeyValue
        End If
    End Sub

    Private Sub CustomShowHide_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles CustomShowHide.KeyUp
        CustomShowHide.Text = KeyConverter.ConvertToString(HumanModifiersShowHide + KeyShowHide).Replace("None", Nothing)

        If KeyConverter.ConvertToString(HumanModifiersShowHide + KeyShowHide).Replace("None", Nothing) = Nothing = False Then
            CustomShowHide.Font = New Font(CustomShowHide.Font, FontStyle.Regular)
            CustomShowHide.ForeColor = SystemColors.WindowText
        End If
    End Sub

    Private Sub CustomShowHide_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CustomShowHide.TextChanged
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
        OnlyModifiers.Hide(CustomShowHide)

        If CustomShowHide.Text = Nothing Then
            CustomShowHide.Text = "Press keys now"
            CustomShowHide.Font = New Font(CustomShowHide.Font, FontStyle.Italic)
            CustomShowHide.ForeColor = Color.Gray
            OnlyModifiers.Show("Please use a combination that includes both" & vbNewLine & "modifier keys and non-modifier keys or only" & vbNewLine & "non-modifier keys.", CustomShowHide)
            OnlyModifiers.Show("Please use a combination that includes both" & vbNewLine & "modifier keys and non-modifier keys or only" & vbNewLine & "non-modifier keys.", CustomShowHide)
        End If
    End Sub

    Private Sub HotkeyShowHide_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HotkeyShowHide.CheckedChanged
        If HotkeyShowHide.Checked = True Then
            CustomShowHide.Enabled = True
            CustomShowHide.Focus()
            CustomShowHide.Text = "Press keys now"
            CustomShowHide.Font = New Font(CustomShowHide.Font, FontStyle.Italic)
            CustomShowHide.ForeColor = Color.Gray
        Else
            ModifiersShowHide = Nothing
            HumanModifiersShowHide = Nothing
            KeyShowHide = Keys.VolumeMute
            CustomShowHide.Enabled = False
            CustomShowHide.Text = "Ctrl+Shift+Home"
            CustomShowHide.Font = New Font(CustomPlayStop.Font, FontStyle.Regular)
        End If
    End Sub

#End Region

#Region "Background Workers"

    Private Sub DownloadUpdater_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles DownloadUpdater.DoWork
        Status.Text = "Status: Connecting to server, please wait..."
        LookNow.Enabled = False
        UndefinedProgress.Hide()

        Dim file As String = dataFolder & "\Updater.exe"
        Dim Message As New MsgBoxSafe(AddressOf DisplayMessage)

        Dim theResponse As Net.HttpWebResponse = Nothing
        Dim theRequest As Net.HttpWebRequest

        Try
            theRequest = Net.WebRequest.Create("http://www.tobiass.eu/files/Updater.exe")
            theResponse = theRequest.GetResponse
        Catch ex As Exception
            Me.Invoke(Message, "Couldn't download the updating utility. Please try again.", MsgBoxStyle.Exclamation, "Error while updating")
            Status.Text = "Status: Idle"
            LookNow.Enabled = True
            UndefinedProgress.Hide()
        End Try

        Dim length As Long = theResponse.ContentLength

        Dim FS As IO.FileStream

        FS = New IO.FileStream(file, IO.FileMode.Create)

        Dim nRead As Integer

        Do
            Dim readByte(1024) As Byte
            Dim inMemory As IO.Stream = theResponse.GetResponseStream
            Dim totalBytes As Integer
            totalBytes = inMemory.Read(readByte, 0, 1024)
            If totalBytes = 0 Then Exit Do
            FS.Write(readByte, 0, totalBytes)
            nRead += totalBytes
            Dim percent As Short = (nRead * 100) / length
            Status.Text = "Status: Downloading, please wait. " & percent & "% complete."
            ProgressBar.Value = percent
        Loop

        FS.Close()
        FS.Dispose()
        theResponse.GetResponseStream.Close()
        theResponse.GetResponseStream.Dispose()
    End Sub

    Private Sub DownloadUpdater_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles DownloadUpdater.RunWorkerCompleted
        Status.Text = "Status: Updater downloaded. Launching and exiting..."

        Dim file As String = dataFolder & "\Updater.exe"

        Try
            Process.Start(file)
            Player.Close()
        Catch
            LookNow.Text = "Run updater"
            LookNow.Enabled = True
            Status.Text = "Status: Updater downloaded. Click button to launch."
            Player.UpdaterDownloaded = True
        End Try
    End Sub

    Private Sub logInWorker_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles logInWorker.DoWork

        Dim WebClient As Net.WebClient = New Net.WebClient()
        WebClient.Credentials = New Net.NetworkCredential("ephemeron", "dayeiph0ne@pp")
        WebClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded")
        Dim request As Byte() = System.Text.Encoding.ASCII.GetBytes("username=" & emailBox.Text & "&password=" & passwordBox.Text.Replace("%", "%25").Replace("&", "%26"))
        Dim Message As New MsgBoxSafe(AddressOf DisplayMessage)

        Try
            returnedData = WebClient.UploadData("https://api.audioaddict.com/v1/di/members/authenticate", "POST", request)
            success = True
        Catch ex As Exception
            If ex.Message.Contains("403") Then
                Me.Invoke(Message, "Invalid username or password.", MsgBoxStyle.Exclamation, "Login failed")
                loginAttempts += 1
            Else
                Me.Invoke(Message, "There was an error while logging in." & vbNewLine & ex.Message & vbNewLine & vbNewLine & "Please try again.", MsgBoxStyle.Exclamation, "Error logging in")
            End If

            success = False
        End Try

    End Sub

    Private Sub logInWorker_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles logInWorker.RunWorkerCompleted
        If success Then
            premiumAccount.Text = "No"
            isPremium = False

            Dim reader As New IO.StringReader(System.Text.Encoding.UTF8.GetString(returnedData).Replace(",", vbNewLine))
            Do While (reader.Peek > -1)
                Dim whole As String = reader.ReadLine

                Dim splitter() As String = Split(whole, ":")

                If splitter(0) = "{""api_key""" Then
                    apiKey = splitter(1).Replace("""", Nothing)
                ElseIf splitter(0) = """listen_key""" Then
                    ListenKey.Text = splitter(1).Replace("""", Nothing)
                ElseIf splitter(0) = """first_name""" Then
                    accountOwner.Text = splitter(1).Replace("""", Nothing) & " "
                    userInfo = splitter(1).Replace("""", Nothing)
                ElseIf splitter(0) = """id""" Then
                    userId = splitter(1).Replace("""", Nothing)
                ElseIf splitter(0) = """last_name""" Then
                    accountOwner.Text += splitter(1).Replace("""", Nothing)
                    userInfo += "|" & splitter(1).Replace("""", Nothing)
                ElseIf splitter(0) = """status""" Then
                    If splitter(1) = """active""" Then
                        premiumAccount.Text = "Yes"
                        isPremium = True
                    End If
                End If
            Loop

            reader.Close()
            reader.Dispose()

            Dim WebClient As Net.WebClient = New Net.WebClient()
            WebClient.Credentials = New Net.NetworkCredential("ephemeron", "dayeiph0ne@pp")

            If WebClient.DownloadString("https://api.audioaddict.com/v1/di/members/" & userId & "/subscriptions/trial_allowed/premium-pass?api_key=" & apiKey).Contains("true") Then
                premiumTrial.Text = "Available"
                startTrial.Enabled = True
                startTrial.Show()
            Else
                premiumTrial.Text = "Not available"
                startTrial.Hide()
            End If

            logIn.Text = "Logout"
            logIn.Enabled = True
            forgotPass.Enabled = False
            createAccount.Enabled = False
            pleaseWait.Hide()
            infoLogIn.Hide()
            StationSelector.Enabled = True
            FileFormat.Enabled = True
            pleaseWaitRefresh.Hide()
            refreshInfo.Enabled = True

            isLogged = True

            If premiumAccount.Text = "Yes" Then
                isPremium = True
            Else
                isPremium = False
            End If

            StationSelector_SelectedIndexChanged(Me, Nothing)

            OK.Enabled = True
            Cancel.Text = "Cancel"
            Apply.Enabled = True

        Else
            If loginAttempts >= 5 Then
                If MessageBox.Show("Login failed after " & loginAttempts & " attempts." & vbNewLine & "Would you like to receive an e-mail to reset your password?", "Password reset", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                    resetPasswordWorker.RunWorkerAsync()
                    loginAttempts = 0
                    pleaseWait.Text = "Please wait..."
                    Exit Sub
                Else
                    loginAttempts = 0
                End If
            End If

            emailBox.ReadOnly = False
            passwordBox.ReadOnly = False
            logIn.Enabled = True
            pleaseWait.Hide()
            StationSelector.Enabled = True
            FileFormat.Enabled = True
            forgotPass.Enabled = True
        End If
    End Sub

    Private Sub resetPasswordWorker_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles resetPasswordWorker.DoWork

        Dim WebClient As Net.WebClient = New Net.WebClient()
        WebClient.Credentials = New Net.NetworkCredential("ephemeron", "dayeiph0ne@pp")
        WebClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded")
        Dim request As Byte() = System.Text.Encoding.ASCII.GetBytes("username=" & emailBox.Text)
        Dim Message As New MsgBoxSafe(AddressOf DisplayMessage)

        Try
            Dim returnedData As Byte() = WebClient.UploadData("https://api.audioaddict.com/v1/di/members/send_reset_password", "POST", request)
            success = True
        Catch ex As Exception


            If ex.Message.Contains("404") Then
                Me.Invoke(Message, "A " & radioStation & " account was not found for that email address." & vbNewLine & "Please try another address or contact " & radioStation & " for support.", MsgBoxStyle.Exclamation, "Reset your password")
            Else
                Me.Invoke(Message, "There was an error while asking for a password reset email." & vbNewLine & ex.Message & vbNewLine & vbNewLine & "Please try again.", MsgBoxStyle.Exclamation, "Reset your password")
            End If

            success = False
        End Try

        If success Then
            Me.Invoke(Message, "An email was sent to the address you specified." & vbNewLine & vbNewLine & "Follow the instructions in the email to retrieve your password." & vbNewLine & "If you don't receive an email within 5 minutes, check your spam filter or contact " & radioStation & " for help.", MsgBoxStyle.Information, "Almost done")
        End If

    End Sub

    Private Sub resetPasswordWorker_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles resetPasswordWorker.RunWorkerCompleted
        emailBox.ReadOnly = False
        passwordBox.ReadOnly = False
        logIn.Enabled = True
        forgotPass.Enabled = True
        pleaseWait.Hide()
    End Sub

    Private Sub startTrialWorker_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles startTrialWorker.DoWork
        Dim WebClient As Net.WebClient = New Net.WebClient()
        WebClient.Credentials = New Net.NetworkCredential("ephemeron", "dayeiph0ne@pp")
        WebClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded")
        Dim request As Byte() = System.Text.Encoding.ASCII.GetBytes("api_key=" & apiKey)
        Dim returnedData As Byte()
        Dim Message As New MsgBoxSafe(AddressOf DisplayMessage)

        Try
            returnedData = WebClient.UploadData("https://api.audioaddict.com/v1/di/members/" & userId & "/subscriptions/trial/premium-pass", "POST", request)
            success = True
        Catch ex As Exception
            Me.Invoke(Message, "Couldn't start your free trial." & vbNewLine & ex.Message & vbNewLine & vbNewLine & "Please refresh your account information and try again.", MsgBoxStyle.Exclamation, "Trial petition failed")
            success = False
        End Try
    End Sub

    Private Sub startTrialWorker_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles startTrialWorker.RunWorkerCompleted
        pleaseWaitRefresh.Hide()
        logIn.Enabled = True
        StationSelector.Enabled = True
        FileFormat.Enabled = True
        refreshInfo.Enabled = True
        startTrial.Enabled = True

        If success = True Then
            premiumAccount.Text = "Yes"
            premiumTrial.Text = "Not available"
            startTrial.Hide()
            isPremium = True
            success = False
        End If

        StationSelector_SelectedIndexChanged(Me, Nothing)
    End Sub

#End Region

#Region "Themes"

    ' ---------------------------------------------------------------------------------------------

    ' These only change the buttons if a valid colour was selected

    Private Sub MainButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MainButton.Click
        ColourPicker.Color = Color.FromArgb(MainColour.BackColor.ToArgb())
        ColourPicker.Tag = "main"

        If ColourPicker.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
            MainColour.Text = ColourPicker.Color.ToArgb.ToString
            MainColour.BackColor = ColourPicker.Color
            OK.Enabled = True
            Cancel.Text = "Cancel"
            Apply.Enabled = True
        End If
    End Sub

    Private Sub SecondaryButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SecondaryButton.Click
        ColourPicker.Color = Color.FromArgb(SecondaryColour.BackColor.ToArgb())
        ColourPicker.Tag = "secondary"

        If ColourPicker.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
            SecondaryColour.BackColor = ColourPicker.Color
            OK.Enabled = True
            Cancel.Text = "Cancel"
            Apply.Enabled = True
        End If
    End Sub

    Private Sub PeakButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PeakButton.Click
        ColourPicker.Color = Color.FromArgb(PeakColour.BackColor.ToArgb())
        ColourPicker.Tag = "peak"

        If ColourPicker.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
            PeakColour.BackColor = ColourPicker.Color
            OK.Enabled = True
            Cancel.Text = "Cancel"
            Apply.Enabled = True
        End If
    End Sub

    Private Sub BackgroundButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BackgroundButton.Click
        ColourPicker.Color = Color.FromArgb(BackgroundColour.BackColor.ToArgb())
        ColourPicker.Tag = "background"

        If ColourPicker.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
            BackgroundColour.BackColor = ColourPicker.Color
            OK.Enabled = True
            Cancel.Text = "Cancel"
            Apply.Enabled = True
        End If
    End Sub

    ' -------------------------------------------------------------

    Private Sub Themes_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Themes.Click
        ThemesMenu.Show(Me, Themes.Location.X + 17, Themes.Location.Y + 245)
    End Sub

    Private Sub RestoreColours_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RestoreColours.Click
        MainColour.BackColor = SystemColors.Control
        SecondaryColour.BackColor = Color.Black
        PeakColour.BackColor = Color.Silver
        BackgroundColour.BackColor = SystemColors.Control

        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub BlackNight_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BlackNight.Click
        MainColour.BackColor = Color.FromArgb(-16777216)
        SecondaryColour.BackColor = Color.FromArgb(-1)
        PeakColour.BackColor = Color.FromArgb(-1)
        BackgroundColour.BackColor = Color.FromArgb(-16777216)
        ChangeWholeBackground.Checked = True

        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub BlueOcean_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BlueOcean.Click
        MainColour.BackColor = Color.FromArgb(-5000193)
        SecondaryColour.BackColor = Color.FromArgb(-16777133)
        PeakColour.BackColor = Color.FromArgb(-9455617)
        BackgroundColour.BackColor = Color.FromArgb(-16760704)
        ChangeWholeBackground.Checked = True

        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub BlueSky_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BlueSky.Click
        MainColour.BackColor = Color.FromArgb(-16725302)
        SecondaryColour.BackColor = Color.FromArgb(-3473409)
        PeakColour.BackColor = Color.FromArgb(-143)
        BackgroundColour.BackColor = Color.FromArgb(-4986881)
        ChangeWholeBackground.Checked = True

        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub BrownWood_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BrownWood.Click
        MainColour.BackColor = Color.FromArgb(-8388608)
        SecondaryColour.BackColor = Color.FromArgb(-4497920)
        PeakColour.BackColor = Color.FromArgb(-12582912)
        BackgroundColour.BackColor = Color.FromArgb(-11327232)
        ChangeWholeBackground.Checked = True

        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub SilverMetal_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SilverMetal.Click
        MainColour.BackColor = Color.FromArgb(-16777216)
        SecondaryColour.BackColor = Color.FromArgb(-8355712)
        PeakColour.BackColor = Color.FromArgb(-10526799)
        BackgroundColour.BackColor = Color.FromArgb(-4144960)
        ChangeWholeBackground.Checked = True

        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub YellowSunset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles YellowSunset.Click
        MainColour.BackColor = Color.FromArgb(-7237376)
        SecondaryColour.BackColor = Color.FromArgb(-243)
        PeakColour.BackColor = Color.FromArgb(-32704)
        BackgroundColour.BackColor = Color.FromArgb(-88)
        ChangeWholeBackground.Checked = True

        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub SaveTheme_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveTheme.Click
        SaveThemeDialog.InitialDirectory = dataFolder & "themes"
        SaveThemeDialog.FileName = Nothing

        If SaveThemeDialog.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim writer As New IO.StreamWriter(SaveThemeDialog.FileName)

            writer.WriteLine(MainColour.BackColor.ToArgb())
            writer.WriteLine(SecondaryColour.BackColor.ToArgb())
            writer.WriteLine(PeakColour.BackColor.ToArgb())
            writer.WriteLine(BackgroundColour.BackColor.ToArgb())
            writer.WriteLine(ChangeWholeBackground.Checked)

            writer.Close()
            writer.Dispose()
        End If
    End Sub

    Private Sub LoadTheme_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoadTheme.Click
        OpenThemeDialog.InitialDirectory = dataFolder & "themes"
        OpenThemeDialog.FileName = Nothing

        If OpenThemeDialog.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim reader As New IO.StreamReader(OpenThemeDialog.FileName)
            Dim lineNumber As Byte = 0

            Do While (reader.Peek > -1)
                Dim line As String = reader.ReadLine

                If lineNumber = 0 Then
                    MainColour.BackColor = Color.FromArgb(line)
                ElseIf lineNumber = 1 Then
                    SecondaryColour.BackColor = Color.FromArgb(line)
                ElseIf lineNumber = 2 Then
                    PeakColour.BackColor = Color.FromArgb(line)
                ElseIf lineNumber = 3 Then
                    BackgroundColour.BackColor = Color.FromArgb(line)
                ElseIf lineNumber = 4 Then
                    ChangeWholeBackground.Checked = line
                End If

                lineNumber += 1
            Loop

            reader.Close()
            reader.Dispose()

            OK.Enabled = True
            Cancel.Text = "Cancel"
            Apply.Enabled = True
        End If
    End Sub

    Private Sub ChangeWholeBackground_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChangeWholeBackground.CheckedChanged
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub SaveThemeDialog_HelpRequest(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveThemeDialog.HelpRequest
        MessageBox.Show("Saving your current theme will allow you to share it with other people by sending the .cth file or by uploading it to the online gallery.", "Saving theme file", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub OpenThemeDialog_HelpRequest(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenThemeDialog.HelpRequest
        MessageBox.Show("Load a .cth file to replace your current colour theme.", "Loading a theme file", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub GetMoreThemesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GetMoreThemesToolStripMenuItem.Click
        Gallery.Location = New Point(Me.Location.X - 8, Me.Location.Y + 9)
        Gallery.Show()
        Gallery.BringToFront()
    End Sub

#End Region

#Region "Equalizer Presets"

    Private Sub Presets_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Presets.Click
        PresetsMenu.Show(Me, Presets.Location.X + 15, Presets.Location.Y - 196)
    End Sub

    Private Sub ClassicalMusic_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ClassicalMusic.Click
        Band0.Value = 6
        Band1.Value = 3
        Band2.Value = 0
        Band3.Value = 0
        Band4.Value = 2
        Band5.Value = 2
    End Sub

    Private Sub DancePreset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DancePreset.Click
        Band0.Value = 4
        Band1.Value = 1
        Band2.Value = -1
        Band3.Value = 0
        Band4.Value = 0
        Band5.Value = 4
    End Sub

    Private Sub JazzPreset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles JazzPreset.Click
        Band0.Value = 0
        Band1.Value = 3
        Band2.Value = 3
        Band3.Value = 0
        Band4.Value = 2
        Band5.Value = 5
    End Sub

    Private Sub MetalPreset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MetalPreset.Click
        Band0.Value = 0
        Band1.Value = 0
        Band2.Value = 0
        Band3.Value = 3
        Band4.Value = 0
        Band5.Value = 1
    End Sub

    Private Sub NewAgePreset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewAgePreset.Click
        Band0.Value = 3
        Band1.Value = 0
        Band2.Value = 0
        Band3.Value = 0
        Band4.Value = 0
        Band5.Value = 1
    End Sub

    Private Sub ReggaePreset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReggaePreset.Click
        Band0.Value = 0
        Band1.Value = -3
        Band2.Value = 0
        Band3.Value = 4
        Band4.Value = 0
        Band5.Value = 4
    End Sub

    Private Sub RockPreset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RockPreset.Click
        Band0.Value = 1
        Band1.Value = 3
        Band2.Value = -1
        Band3.Value = 0
        Band4.Value = 0
        Band5.Value = 4
    End Sub

    Private Sub TechnoPreset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TechnoPreset.Click
        Band0.Value = 1
        Band1.Value = -1
        Band2.Value = -1
        Band3.Value = 0
        Band4.Value = 0
        Band5.Value = 5
    End Sub

    Private Sub SavePreset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SavePreset.Click
        SavePresetDialog.InitialDirectory = dataFolder & "equalizer"
        SavePresetDialog.FileName = Nothing
        If SavePresetDialog.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim writer As New IO.StreamWriter(SavePresetDialog.FileName)

            writer.WriteLine("0")
            writer.WriteLine(Band0.Value)
            writer.WriteLine("0")
            writer.WriteLine(Band1.Value)
            writer.WriteLine("0")
            writer.WriteLine("0")
            writer.WriteLine("0")
            writer.WriteLine(Band2.Value)
            writer.WriteLine("0")
            writer.WriteLine("0")
            writer.WriteLine(Band3.Value)
            writer.WriteLine("0")
            writer.WriteLine("0")
            writer.WriteLine(Band4.Value)
            writer.WriteLine("0")
            writer.WriteLine("0")
            writer.WriteLine(Band5.Value)
            writer.WriteLine("0")

            writer.Close()
            writer.Dispose()
        End If
    End Sub

    Private Sub LoadPreset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoadPreset.Click
        OpenPresetDialog.InitialDirectory = dataFolder & "equalizer"
        OpenPresetDialog.FileName = Nothing

        If OpenPresetDialog.ShowDialog = Windows.Forms.DialogResult.OK Then
            Player.LoadEqFile(OpenPresetDialog.FileName)
        End If
    End Sub

    Private Sub SavePresetDialog_HelpRequest(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SavePresetDialog.HelpRequest
        MessageBox.Show("Use this dialog to save your current equalizer levels to a file that can then be loaded in foobar2000.", "Preset saving", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub OpenPresetDialog_HelpRequest(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenPresetDialog.HelpRequest
        MessageBox.Show("Use this dialog to select a previously saved .feq file. Any .feq file saved by foobar2000 or this application will work.", "Preset loading", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

#End Region

#Region "Others"

    Public Enum HotKeyModifiers As Integer
        MOD_ALT = &H1
        MOD_CONTROL = &H2
        MOD_SHIFT = &H4
        MOD_WIN = &H8
    End Enum

    Sub DisplayMessage(ByVal text As String, ByVal style As MsgBoxStyle, ByVal title As String)
        MsgBox(text, style, title)
        Player.BringToFront()
        Me.BringToFront()
    End Sub

    Private Sub ApplyOptions()

        ' Update main form variables with the values from this form's controls

        Player.NotificationTitle = NotificationTitle.Checked
        Player.NotificationIcon = NotificationIcon.Checked
        Player.MultimediaKeys = MultimediaKeys.Checked

        Player.NoTaskbarButton = NoTaskbarButton.Checked

        If NoTaskbarButton.Checked = False Then
            Player.ShowInTaskbar = True

            If Player.WindowState = FormWindowState.Normal Then
                Player.TrayIcon.Visible = False
            End If

        Else
            Player.ShowInTaskbar = False
            Player.TrayIcon.Visible = True
        End If

        Player.GoogleSearch = GoogleSearch.Checked
        Player.cacheList = cacheList.Text
        Player.ShowSongStart = ShowSongStart.Checked
        Player.removeKey = removeListenKey.Checked

        If Player.HistoryList.Items.Count < 20 Then
            If Player.ShowSongStart = False Then
                Player.Time.Width = 0
                Player.Title.Width = 276
            ElseIf Player.ShowSongStart = True And New DateTime(2000, 1, 1, 13, 0, 0).ToString.Contains("13") Then
                Player.Title.Width = 236
                Player.Time.Width = 40
            Else
                Player.Title.Width = 226
                Player.Time.Width = 50
            End If
        Else
            If Player.ShowSongStart = False Then
                Player.Time.Width = 0
                Player.Title.Width = 259
            ElseIf Player.ShowSongStart = True And New DateTime(2000, 1, 1, 13, 0, 0).ToString.Contains("13") Then
                Player.Title.Width = 219
                Player.Time.Width = 40
            Else
                Player.Title.Width = 209
                Player.Time.Width = 50
            End If
        End If


        If isLogged = False Then

            If Player.SelectedChannel.Text = "My Favorites" Then
                Player.SelectedServer.Items.Clear()
            End If

            Player.SelectedChannel.Items.Remove("My Favorites")
            Player.RefreshFavorites.Hide()
            Player.EditFavorites.Hide()

        Else
            Dim item As String

            If Player.StationChooser.Text = Player.DIFM.Text And Player.SelectedChannel.Items.Count > 1 Then

                For Each item In Player.SelectedChannel.Items
                    If item = "My Favorites" Then
                        GoTo nofavs
                    End If
                Next

                Player.SelectedChannel.Items.Add("My Favorites")

            ElseIf Player.StationChooser.Text = Player.SKYFM.Text And Player.SelectedChannel.Items.Count > 1 Then

                For Each item In Player.SelectedChannel.Items
                    If item = "My Favorites" Then
                        GoTo nofavs
                    End If
                Next

                Player.SelectedChannel.Items.Add("My Favorites")

            ElseIf Player.StationChooser.Text = Player.JazzRadio.Text And Player.SelectedChannel.Items.Count > 1 Then

                For Each item In Player.SelectedChannel.Items
                    If item = "My Favorites" Then
                        GoTo nofavs
                    End If
                Next

                Player.SelectedChannel.Items.Add("My Favorites")

            End If


        End If

nofavs:

        If isPremium = Player.isPremium = False OrElse ListenKey.Text = Player.ListenKey = False Then

            If isPremium = False Then
                If DISetting > -1 Then
                    Player.DIFormat = DISetting
                Else
                    Player.DIFormat = 1
                End If

                If SKYSetting > -1 Then
                    Player.SKYFormat = SKYSetting
                Else
                    Player.SKYFormat = 1
                End If

                If JazzSetting > -1 Then
                    Player.JazzFormat = JazzSetting
                Else
                    Player.JazzFormat = 1
                End If

            Else

                If DISettingPremium > -1 Then
                    Player.DIFormat = DISettingPremium
                Else
                    Player.DIFormat = 3
                End If

                If SKYSettingPremium > -1 Then
                    Player.SKYFormat = SKYSettingPremium
                Else
                    Player.SKYFormat = 3
                End If

                If JazzSettingPremium > -1 Then
                    Player.JazzFormat = JazzSettingPremium
                Else
                    Player.JazzFormat = 3
                End If
            End If

            If My.Computer.FileSystem.DirectoryExists("servers\Digitally Imported") Then
                For Each foundFile As String In My.Computer.FileSystem.GetFiles("servers\Digitally Imported")

                    If foundFile.Contains("channels.db") = False Then
                        Kill(foundFile)
                    End If

                Next
            End If

            If My.Computer.FileSystem.DirectoryExists("servers\JazzRadio") Then
                For Each foundFile As String In My.Computer.FileSystem.GetFiles("servers\JazzRadio")

                    If foundFile.Contains("channels.db") = False Then
                        Kill(foundFile)
                    End If

                Next
            End If

            If My.Computer.FileSystem.DirectoryExists("servers\SKY.FM") Then
                For Each foundFile As String In My.Computer.FileSystem.GetFiles("servers\SKY.FM")

                    If foundFile.Contains("channels.db") = False Then
                        Kill(foundFile)
                    End If

                Next
            End If



            If Player.SelectedChannel.Text = "My Favorites" Then
                Player.OldFav = Player.SelectedServer.Text
            End If

            If Player.PlayStop.Tag = "Stop" And Player.PlayStop.Enabled = True Then
                Player.PlayStop_Click(Me, Nothing)
                Player.RestartPlayback = True
            End If

            If Player.SelectedChannel.Text = Nothing = False Then

                If Player.ServersDownloader.IsBusy = False Then
                    Player.SelectedChannel_SelectedIndexChanged(Me, Nothing)
                End If

            Else
                Player.SelectedServer.Enabled = False
                Player.SelectedServer.Items.Add("Pick a channel")
                Player.SelectedServer.SelectedIndex = 0
            End If

        Else

            If Player.DIFormat = DISetting = False And Player.DIFormat = DISettingPremium = False Then

                If isPremium = False Then
                    Player.DIFormat = DISetting
                Else
                    Player.DIFormat = DISettingPremium
                End If


                If My.Computer.FileSystem.DirectoryExists("servers\Digitally Imported") Then
                    For Each foundFile As String In My.Computer.FileSystem.GetFiles("servers\Digitally Imported")

                        If foundFile.Contains("channels.db") = False Then
                            Kill(foundFile)
                        End If

                    Next
                End If

                If Player.StationChooser.Text = Player.DIFM.Text Then

                    If Player.SelectedChannel.Text = "My Favorites" Then
                        Player.OldFav = Player.SelectedServer.Text
                    End If


                    If Player.PlayStop.Tag = "Stop" And Player.PlayStop.Enabled = True Then
                        Player.PlayStop_Click(Me, Nothing)
                        Player.RestartPlayback = True
                    End If


                    If Player.SelectedChannel.Text = Nothing = False Then
                        If Player.ServersDownloader.IsBusy = False Then
                            Player.SelectedChannel_SelectedIndexChanged(Me, Nothing)
                        End If
                    Else
                        Player.SelectedServer.Enabled = False
                        Player.SelectedServer.Items.Add("Pick a server")
                        Player.SelectedServer.SelectedIndex = 0
                    End If

                End If

            End If

            If Player.SKYFormat = SKYSetting = False And Player.SKYFormat = SKYSettingPremium = False Then

                If isPremium = False Then
                    Player.SKYFormat = SKYSetting
                Else
                    Player.SKYFormat = SKYSettingPremium
                End If


                If My.Computer.FileSystem.DirectoryExists("servers\SKY.FM") Then
                    For Each foundFile As String In My.Computer.FileSystem.GetFiles("servers\SKY.FM")

                        If foundFile.Contains("channels.db") = False Then
                            Kill(foundFile)
                        End If

                    Next
                End If

                If Player.StationChooser.Text = Player.SKYFM.Text Then

                    If Player.SelectedChannel.Text = "My Favorites" Then
                        Player.OldFav = Player.SelectedServer.Text
                    End If

                    If Player.PlayStop.Tag = "Stop" And Player.PlayStop.Enabled = True Then
                        Player.PlayStop_Click(Me, Nothing)
                        Player.RestartPlayback = True
                    End If

                    If Player.SelectedChannel.Text = Nothing = False Then
                        If Player.ServersDownloader.IsBusy = False Then
                            Player.SelectedChannel_SelectedIndexChanged(Me, Nothing)
                        End If
                    Else
                        Player.SelectedServer.Enabled = False
                        Player.SelectedServer.Items.Add("Pick a server")
                        Player.SelectedServer.SelectedIndex = 0
                    End If

                End If

            End If

            If Player.JazzFormat = JazzSetting = False And Player.JazzFormat = JazzSettingPremium = False Then

                If isPremium = False Then
                    Player.JazzFormat = JazzSetting
                Else
                    Player.JazzFormat = JazzSettingPremium
                End If

                If My.Computer.FileSystem.DirectoryExists("servers\JazzRadio") Then
                    For Each foundFile As String In My.Computer.FileSystem.GetFiles("servers\JazzRadio")

                        If foundFile.Contains("channels.db") = False Then
                            Kill(foundFile)
                        End If

                    Next
                End If


                If Player.StationChooser.Text = Player.JazzRadio.Text Then

                    If Player.SelectedChannel.Text = "My Favorites" Then
                        Player.OldFav = Player.SelectedServer.Text
                    End If

                    If Player.PlayStop.Tag = "Stop" And Player.PlayStop.Enabled = True Then
                        Player.PlayStop_Click(Me, Nothing)
                        Player.RestartPlayback = True
                    End If

                    If Player.SelectedChannel.Text = Nothing = False Then
                        If Player.ServersDownloader.IsBusy = False Then
                            Player.SelectedChannel_SelectedIndexChanged(Me, Nothing)
                        End If
                    Else
                        Player.SelectedServer.Enabled = False
                        Player.SelectedServer.Items.Add("Pick a server")
                        Player.SelectedServer.SelectedIndex = 0
                    End If

                End If
            End If

        End If

        Player.ListenKey = ListenKey.Text
        Player.isPremium = isPremium
        Player.userId = userId
        Player.userInfo = userInfo
        Player.isPremium = isPremium
        Player.canTrial = canTrial
        Player.apiKey = apiKey
        Player.isLogged = isLogged
        Player.username = emailBox.Text
        Player.password = passwordBox.Text
        Player.UpdatesAtStart = UpdatesAtStart.Checked
        Player.BetaVersions = BetaVersions.Checked
        Player.Visualisation = Visualisation.Checked

        If Visualisation.Checked = True Then

            Player.VisualisationBox.Show()
            Player.Size = New Size(Player.MaximumSize)

            If Player.PlayStop.Tag = "Stop" And Player.HistoryList.Visible = True Then
                Player.VisTimer.Start()
            End If

        Else

            Player.VisTimer.Stop()
            Player.VisualisationBox.Hide()

            If Player.HistoryList.Visible = False Then
                Player.Size = New Size(Player.MinimumSize)
            End If

        End If

        Player.VisualisationType = VisualisationType.SelectedIndex
        Player.HighQualityVis = HighQualityVis.Checked
        Player.LinealRepresentation = LinealRepresentation.Checked
        Player.FullSoundRange = FullSoundRange.Checked

        If FullSoundRange.Checked = True Then

            If VisualisationType.SelectedIndex = 0 Then
                Player.drawing.ScaleFactorLinear = 1
                Player.drawing.ScaleFactorLinearBoost = 0
                Player.drawing.ScaleFactorSqr = 1
                Player.drawing.ScaleFactorSqrBoost = 0
            ElseIf VisualisationType.SelectedIndex = 1 Then
                Player.drawing.ScaleFactorLinear = 3
                Player.drawing.ScaleFactorLinearBoost = 0.34
                Player.drawing.ScaleFactorSqr = 1
                Player.drawing.ScaleFactorSqrBoost = 0.04
            ElseIf VisualisationType.SelectedIndex = 2 Then
                Player.drawing.ScaleFactorLinear = 0.6
                Player.drawing.ScaleFactorLinearBoost = 0.1
                Player.drawing.ScaleFactorSqr = 1
                Player.drawing.ScaleFactorSqrBoost = 0
            ElseIf VisualisationType.SelectedIndex = 3 Then
                Player.drawing.ScaleFactorLinear = 1.5
                Player.drawing.ScaleFactorLinearBoost = 0.07
                Player.drawing.ScaleFactorSqr = 1
                Player.drawing.ScaleFactorSqrBoost = 0
            ElseIf VisualisationType.SelectedIndex = 4 Then
                Player.drawing.ScaleFactorLinear = 3
                Player.drawing.ScaleFactorLinearBoost = 0.7
                Player.drawing.ScaleFactorSqr = 1
                Player.drawing.ScaleFactorSqrBoost = 0.25
            ElseIf VisualisationType.SelectedIndex = 5 Then
                Player.drawing.ScaleFactorLinear = 3
                Player.drawing.ScaleFactorLinearBoost = 0.7
                Player.drawing.ScaleFactorSqr = 1
                Player.drawing.ScaleFactorSqrBoost = 0.25
            ElseIf VisualisationType.SelectedIndex = 6 Then
                Player.drawing.ScaleFactorLinear = 1
                Player.drawing.ScaleFactorLinearBoost = 0.14
                Player.drawing.ScaleFactorSqr = 0.6
                Player.drawing.ScaleFactorSqrBoost = 0.01
            ElseIf VisualisationType.SelectedIndex = 7 Then
                Player.drawing.ScaleFactorLinear = 0
                Player.drawing.ScaleFactorLinearBoost = 0
                Player.drawing.ScaleFactorSqr = 0
                Player.drawing.ScaleFactorSqrBoost = 0
            End If

        Else

            If VisualisationType.SelectedIndex = 0 Then
                Player.drawing.ScaleFactorLinear = 1
                Player.drawing.ScaleFactorLinearBoost = 0.1
                Player.drawing.ScaleFactorSqr = 0.6
                Player.drawing.ScaleFactorSqrBoost = 0
            ElseIf VisualisationType.SelectedIndex = 1 Then
                Player.drawing.ScaleFactorLinear = 9
                Player.drawing.ScaleFactorLinearBoost = 0.18
                Player.drawing.ScaleFactorSqr = 7
                Player.drawing.ScaleFactorSqrBoost = 0
            ElseIf VisualisationType.SelectedIndex = 2 Then
                Player.drawing.ScaleFactorLinear = 3
                Player.drawing.ScaleFactorLinearBoost = 0.06
                Player.drawing.ScaleFactorSqr = 2
                Player.drawing.ScaleFactorSqrBoost = 0.02
            ElseIf VisualisationType.SelectedIndex = 3 Then
                Player.drawing.ScaleFactorLinear = 4
                Player.drawing.ScaleFactorLinearBoost = 0.07
                Player.drawing.ScaleFactorSqr = 1.5
                Player.drawing.ScaleFactorSqrBoost = 0.2
            ElseIf VisualisationType.SelectedIndex = 4 Then
                Player.drawing.ScaleFactorLinear = 5.5
                Player.drawing.ScaleFactorLinearBoost = 0.23
                Player.drawing.ScaleFactorSqr = 2.5
                Player.drawing.ScaleFactorSqrBoost = 0.15
            ElseIf VisualisationType.SelectedIndex = 5 Then
                Player.drawing.ScaleFactorLinear = 5.5
                Player.drawing.ScaleFactorLinearBoost = 0.23
                Player.drawing.ScaleFactorSqr = 2.5
                Player.drawing.ScaleFactorSqrBoost = 0.15
            ElseIf VisualisationType.SelectedIndex = 6 Then
                Player.drawing.ScaleFactorLinear = 3
                Player.drawing.ScaleFactorLinearBoost = 0.025
                Player.drawing.ScaleFactorSqr = 2.3
                Player.drawing.ScaleFactorSqrBoost = 0.014
            ElseIf VisualisationType.SelectedIndex = 7 Then
                Player.drawing.ScaleFactorLinear = 0
                Player.drawing.ScaleFactorLinearBoost = 0
                Player.drawing.ScaleFactorSqr = 0
                Player.drawing.ScaleFactorSqrBoost = 0
            End If

        End If

        Player.Smoothness = Smoothness.Value
        Player.MainColour = MainColour.BackColor.ToArgb()
        Player.SecondaryColour = SecondaryColour.BackColor.ToArgb()
        Player.PeakColour = PeakColour.BackColor.ToArgb()
        Player.BackgroundColour = BackgroundColour.BackColor.ToArgb()
        Player.ChangeWholeBackground = ChangeWholeBackground.Checked

        Player.ApplyTheme()

        Player.PlayNewOnChannelChange = PlayNewOnChannelChange.Checked

        If CustomPlayStop.Text = Nothing OrElse HotkeyPlayStop.Checked = False Then
            Player.HumanModifiersPlayStop = Nothing
            Player.ModifiersPlayStop = Nothing
            Player.KeyPlayStop = Keys.MediaPlayPause
        Else
            Player.HumanModifiersPlayStop = HumanModifiersPlayStop
            Player.ModifiersPlayStop = ModifiersPlayStop
            Player.KeyPlayStop = KeyPlayStop
        End If

        If CustomVolumeUp.Text = Nothing OrElse HotkeyVolumeUp.Checked = False Then
            Player.HumanModifiersVolumeUp = Nothing
            Player.ModifiersVolumeUp = Nothing
            Player.KeyVolumeUp = Keys.VolumeUp
        Else
            Player.HumanModifiersVolumeUp = HumanModifiersVolumeUp
            Player.ModifiersVolumeUp = ModifiersVolumeUp
            Player.KeyVolumeUp = KeyVolumeUp
        End If

        If CustomVolumeDown.Text = Nothing OrElse HotkeyVolumeDown.Checked = False Then
            Player.HumanModifiersVolumeDown = Nothing
            Player.ModifiersVolumeDown = Nothing
            Player.KeyVolumeDown = Keys.VolumeDown
        Else
            Player.HumanModifiersVolumeDown = HumanModifiersVolumeDown
            Player.ModifiersVolumeDown = ModifiersVolumeDown
            Player.KeyVolumeDown = KeyVolumeDown
        End If

        If CustomMuteUnmute.Text = Nothing OrElse HotkeyMuteUnmute.Checked = False Then
            Player.HumanModifiersMuteUnmute = Nothing
            Player.ModifiersMuteUnmute = Nothing
            Player.KeyMuteUnmute = Keys.VolumeMute
        Else
            Player.HumanModifiersMuteUnmute = HumanModifiersMuteUnmute
            Player.ModifiersMuteUnmute = ModifiersMuteUnmute
            Player.KeyMuteUnmute = KeyMuteUnmute
        End If

        If CustomShowHide.Text = Nothing OrElse HotkeyShowHide.Checked = False Then
            Player.HumanModifiersShowHide = Keys.Control + Keys.Shift
            Player.ModifiersShowHide = 6
            Player.KeyShowHide = Keys.Home
        Else
            Player.HumanModifiersShowHide = HumanModifiersShowHide
            Player.ModifiersShowHide = ModifiersShowHide
            Player.KeyShowHide = KeyShowHide
        End If

        If Player.HotkeysSet = True Then
            Player.UnregisterHotKey(Player.Handle, 1)
            Player.UnregisterHotKey(Player.Handle, 2)
            Player.UnregisterHotKey(Player.Handle, 3)
            Player.UnregisterHotKey(Player.Handle, 4)
            Player.UnregisterHotKey(Player.Handle, 5)
            Player.UnregisterHotKey(Player.Handle, 6)
        End If

        If Player.WindowState = FormWindowState.Minimized And Player.MultimediaKeys = True Then
            Player.RegisterHotKey(Player.Handle, 1, Player.ModifiersPlayStop, Player.KeyPlayStop)
            Player.RegisterHotKey(Player.Handle, 2, Player.ModifiersVolumeUp, Player.KeyVolumeUp)
            Player.RegisterHotKey(Player.Handle, 3, Player.ModifiersVolumeDown, Player.KeyVolumeDown)
            Player.RegisterHotKey(Player.Handle, 4, Player.ModifiersMuteUnmute, Player.KeyMuteUnmute)
            Player.RegisterHotKey(Player.Handle, 5, Player.ModifiersPlayStop, Player.KeyPlayStop)
            Player.RegisterHotKey(Player.Handle, 6, Player.ModifiersShowHide, Player.KeyShowHide)
        End If

        Player.Band0 = Band0.Value
        Player.Band1 = Band1.Value
        Player.Band2 = Band2.Value
        Player.Band3 = Band3.Value
        Player.Band4 = Band4.Value
        Player.Band5 = Band5.Value

        Player.SaveSettings(False)
    End Sub

    Private Sub ColourPicker_HelpRequest(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ColourPicker.HelpRequest
        If ColourPicker.Tag.ToString.Contains("secondary") Then
            MessageBox.Show("You're selecting the Secondary colour of the visualisation." & vbNewLine & "This colour is used at the top of the visualisation as the frequency levels reach higher values." & vbNewLine & "The middle range of the visualisation uses a mixture of the Main and Secondary colours." & vbNewLine & vbNewLine & "You can select a colour from the default ones on the left side of the window or create a custom one by dragging the plus sign inside the color hue and the arrow on the brightness level." & vbNewLine & "Alternatively, you can write your own custom values on the number boxes to define a colour.", "Colour help", MessageBoxButtons.OK, MessageBoxIcon.Information)
        ElseIf ColourPicker.Tag.ToString.Contains("main") Then
            MessageBox.Show("You're selecting the Main colour of the visualisation." & vbNewLine & "This colour is used at the bottom of the visualisation and is almost always displayed." & vbNewLine & "The middle range of the visualisation uses a mixture of the Main and Secondary colours." & vbNewLine & vbNewLine & "You can select a colour from the default ones on the left side of the window or create a custom one by dragging the plus sign inside the color hue and the arrow on the brightness level." & vbNewLine & "Alternatively, you can write your own custom values on the number boxes to define a colour.", "Colour help", MessageBoxButtons.OK, MessageBoxIcon.Information)
        ElseIf ColourPicker.Tag.ToString.Contains("peak") Then
            MessageBox.Show("You're selecting the colour that will be used for the peaks when the Lines with peaks visualisation is selected." & vbNewLine & "The peaks are the squares displayed at the top of the lines." & vbNewLine & vbNewLine & "You can select a colour from the default ones on the left side of the window or create a custom one by dragging the plus sign inside the color hue and the arrow on the brightness level." & vbNewLine & "Alternatively, you can write your own custom values on the number boxes to define a colour.", "Colour help", MessageBoxButtons.OK, MessageBoxIcon.Information)
        ElseIf ColourPicker.Tag.ToString.Contains("background") Then
            MessageBox.Show("You're selecting the colour that will be used as a background on the visualisation area." & vbNewLine & "If you check the 'Change the whole background' box, this colour will be used as a background for the entire player, even when the visualisation is disabled." & vbNewLine & "If you select a colour that's too dark, the title and time displays will use a white font colour; and if you select a colour that's too bright, they will use a black font colour." & vbNewLine & vbNewLine & "You can select a colour from the default ones on the left side of the window or create a custom one by dragging the plus sign inside the color hue and the arrow on the brightness level." & vbNewLine & "Alternatively, you can write your own custom values on the number boxes to define a colour.", "Colour help", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub AboutWebsite_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles AboutWebsite.LinkClicked
        Process.Start("http://www.tobiass.eu")
    End Sub

    Private Sub AboutForums_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles AboutForums.LinkClicked
        Process.Start("http://forums.di.fm")
    End Sub

    Private Sub AboutLicense_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles AboutLicense.LinkClicked
        Process.Start("http://www.tobiass.eu/files/license.htm")
    End Sub

    Function CheckEmail(ByVal emailAddress As String) As Boolean

        Dim pattern As String = "^.+@[^\.].*\.[a-z]{2,}$"
        Dim emailAddressMatch As System.Text.RegularExpressions.Match = System.Text.RegularExpressions.Regex.Match(emailAddress, pattern)

        If emailAddressMatch.Success Then
            Return True
        Else
            Return False
        End If

    End Function

#End Region

End Class