﻿Public Class Options

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

    Public DISetting As Integer = -1
    Public SKYSetting As Integer = -1
    Public JazzSetting As Integer = -1
    Public DISettingPremium As Integer = -1
    Public SKYSettingPremium As Integer = -1
    Public JazzSettingPremium As Integer = -1

    Delegate Sub MsgBoxSafe(ByVal text As String, ByVal style As MsgBoxStyle, ByVal title As String)

#End Region

#Region "Main Form events"

    Private Sub Form2_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        ' Load values from the global variables of the main form and accomodate the interface

        If Player.PremiumFormats = False Then
            DISetting = Player.DIFormat
            JazzSetting = Player.JazzFormat
            SKYSetting = Player.SKYFormat
        Else
            DISettingPremium = Player.DIFormat
            JazzSettingPremium = Player.JazzFormat
            SKYSettingPremium = Player.SKYFormat
        End If
        
        NotificationTitle.Checked = Player.NotificationTitle
        Visualisation.Checked = Player.Visualisation
        NotificationIcon.Checked = Player.NotificationIcon
        NoTaskbarButton.Checked = Player.NoTaskbarButton
        MultimediaKeys.Checked = Player.MultimediaKeys
        GoogleSearch.Checked = Player.GoogleSearch
        ListenKey.Text = Player.ListenKey

        If ListenKey.Text = Nothing = False Then
            PremiumFormats.Checked = Player.PremiumFormats
            ValidateKey.Enabled = True
        Else
            PremiumFormats.Checked = False
            PremiumFormats.Enabled = False
            ValidateKey.Enabled = False
        End If

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

        If Player.TotalVersionString > Player.TotalVersionFixed Then
            LookNow.Text = "Download update"
            LatestVersion.ForeColor = Color.Green
        End If

        CurrentVersion.Text = "Current version:" & Player.Text.Replace("DI Radio Player", Nothing)
        CurrentVersion.Text = CurrentVersion.Text.Replace("SKY.FM Radio Player", Nothing)
        CurrentVersion.Text = CurrentVersion.Text.Replace("JazzRadio Radio Player", Nothing)
        CurrentVersion.Text = CurrentVersion.Text.Replace("RockRadio Radio Player", Nothing)

        If Player.LatestVersionString = Nothing = False Then
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

        OK.Enabled = False
        Cancel.Text = "Close"
        Apply.Enabled = False

    End Sub

    Private Sub Form2_Move(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Move
        OnlyModifiers.Hide(Me)
    End Sub

    Private Sub Form2_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Changelog.Close()
    End Sub

#End Region

#Region "Events caused by the controls in the form"

    ' These only enable the OK and Apply buttons and change the Close button's text to Cancel

    Private Sub NotificationTitle_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles NotificationTitle.Click
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub PlayNewOnChannelChange_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles PlayNewOnChannelChange.CheckedChanged
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

    Private Sub FileFormat_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles FileFormat.Click
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub ListenKey_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles ListenKey.KeyUp
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub UpdatesAtStart_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles UpdatesAtStart.Click
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub BetaVersions_Click(sender As Object, e As System.EventArgs) Handles BetaVersions.Click
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub Visualisation_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Visualisation.Click
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub VisualisationType_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles VisualisationType.Click
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

    Private Sub ChangeWholeBackground_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles ChangeWholeBackground.CheckedChanged
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub MultimediaKeys_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles MultimediaKeys.CheckedChanged
        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    ' ---------------------------------------------------------------------------------------------

    ' These only change the buttons if a valid colour was selected

    Private Sub MainButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MainButton.Click
        ColourPicker.Color = Color.FromArgb(MainColour.BackColor.ToArgb())

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

        If ColourPicker.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
            SecondaryColour.BackColor = ColourPicker.Color
            OK.Enabled = True
            Cancel.Text = "Cancel"
            Apply.Enabled = True
        End If
    End Sub

    Private Sub PeakButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PeakButton.Click
        ColourPicker.Color = Color.FromArgb(PeakColour.BackColor.ToArgb())

        If ColourPicker.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
            PeakColour.BackColor = ColourPicker.Color
            OK.Enabled = True
            Cancel.Text = "Cancel"
            Apply.Enabled = True
        End If
    End Sub

    Private Sub BackgroundButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BackgroundButton.Click
        ColourPicker.Color = Color.FromArgb(BackgroundColour.BackColor.ToArgb())

        If ColourPicker.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
            BackgroundColour.BackColor = ColourPicker.Color
            OK.Enabled = True
            Cancel.Text = "Cancel"
            Apply.Enabled = True
        End If
    End Sub

    ' -------------------------------------------------------------

    Private Sub PremiumFormats_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PremiumFormats.CheckedChanged
        If PremiumFormats.Checked = True And StationSelector.Text = "RockRadio" = False Then
            FileFormat.Items.Clear()
            FileFormat.Items.Add("AAC-HE 128k")
            FileFormat.Items.Add("AAC-HE 64k")
            FileFormat.Items.Add("AAC-HE 40k")
            FileFormat.Items.Add("MP3 256k")
            FileFormat.Items.Add("MP3 128k")
            FileFormat.Items.Add("Windows Media 128k")

            If StationSelector.Text = "JazzRadio" = False Then
                FileFormat.Items.Add("Windows Media 64k")
            End If

            If StationSelector.Text = "Digitally Imported" Then
                If DISettingPremium > -1 Then
                    FileFormat.SelectedIndex = DISettingPremium
                Else
                    If DISetting = 0 Then
                        FileFormat.SelectedIndex = 0
                    ElseIf DISetting = 1 Then
                        FileFormat.SelectedIndex = 3
                    ElseIf DISetting = 2 Then
                        FileFormat.SelectedIndex = 5
                    End If
                End If

            ElseIf StationSelector.Text = "JazzRadio" Then
                If JazzSettingPremium > -1 Then
                    FileFormat.SelectedIndex = JazzSettingPremium
                Else
                    If JazzSetting = 0 Then
                        FileFormat.SelectedIndex = 0
                    ElseIf JazzSetting = 1 Then
                        FileFormat.SelectedIndex = 3
                    ElseIf JazzSetting = 2 Then
                        FileFormat.SelectedIndex = 5
                    End If
                End If

            ElseIf StationSelector.Text = "SKY.FM" Then
                If SKYSettingPremium > -1 Then
                    FileFormat.SelectedIndex = SKYSettingPremium
                Else
                    If SKYSetting = 0 Then
                        FileFormat.SelectedIndex = 0
                    ElseIf SKYSetting = 1 Then
                        FileFormat.SelectedIndex = 3
                    ElseIf SKYSetting = 2 Then
                        FileFormat.SelectedIndex = 5
                    End If
                End If

            End If
        ElseIf PremiumFormats.Checked = False And StationSelector.Text = "RockRadio" = False Then
            FileFormat.Items.Clear()
            FileFormat.Items.Add("AAC-HE")
            FileFormat.Items.Add("MP3")
            FileFormat.Items.Add("Windows Media")

            If StationSelector.Text = "Digitally Imported" Then
                If DISetting > -1 Then
                    FileFormat.SelectedIndex = DISetting
                Else
                    If DISettingPremium <= 2 Then
                        FileFormat.SelectedIndex = 0
                    ElseIf DISettingPremium >= 3 And DISettingPremium <= 4 Then
                        FileFormat.SelectedIndex = 1
                    ElseIf DISettingPremium >= 5 Then
                        FileFormat.SelectedIndex = 2
                    End If
                End If

            ElseIf StationSelector.Text = "JazzRadio" Then
                If JazzSetting > -1 Then
                    FileFormat.SelectedIndex = JazzSetting
                Else
                    If JazzSettingPremium <= 2 Then
                        FileFormat.SelectedIndex = 0
                    ElseIf JazzSettingPremium >= 3 And JazzSettingPremium <= 4 Then
                        FileFormat.SelectedIndex = 1
                    ElseIf JazzSettingPremium >= 5 Then
                        FileFormat.SelectedIndex = 2
                    End If
                End If

            ElseIf StationSelector.Text = "SKY.FM" Then
                If SKYSetting > -1 Then
                    FileFormat.SelectedIndex = SKYSetting
                Else
                    If SKYSettingPremium <= 2 Then
                        FileFormat.SelectedIndex = 0
                    ElseIf SKYSettingPremium >= 3 And SKYSettingPremium <= 4 Then
                        FileFormat.SelectedIndex = 1
                    ElseIf SKYSettingPremium >= 5 Then
                        FileFormat.SelectedIndex = 2
                    End If
                End If

            End If
        End If

        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub StationSelector_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles StationSelector.SelectedIndexChanged
        If StationSelector.Text = "Digitally Imported" Then
            FileFormat.Enabled = True

            If PremiumFormats.Checked = True Then
                FileFormat.Items.Clear()
                FileFormat.Items.Add("AAC-HE 128k")
                FileFormat.Items.Add("AAC-HE 64k")
                FileFormat.Items.Add("AAC-HE 40k")
                FileFormat.Items.Add("MP3 256k")
                FileFormat.Items.Add("MP3 128k")
                FileFormat.Items.Add("Windows Media 128k")
                FileFormat.Items.Add("Windows Media 64k")

                If DISettingPremium > -1 Then
                    FileFormat.SelectedIndex = DISettingPremium
                Else
                    FileFormat.SelectedIndex = 3
                End If

            Else
                FileFormat.Items.Clear()
                FileFormat.Items.Add("AAC-HE")
                FileFormat.Items.Add("MP3")
                FileFormat.Items.Add("Windows Media")

                If DISetting > -1 Then
                    FileFormat.SelectedIndex = DISetting
                Else
                    FileFormat.SelectedIndex = 1
                End If

            End If
        ElseIf StationSelector.Text = "SKY.FM" Then
            FileFormat.Enabled = True

            If PremiumFormats.Checked = True Then
                FileFormat.Items.Clear()
                FileFormat.Items.Add("AAC-HE 128k")
                FileFormat.Items.Add("AAC-HE 64k")
                FileFormat.Items.Add("AAC-HE 40k")
                FileFormat.Items.Add("MP3 256k")
                FileFormat.Items.Add("MP3 128k")
                FileFormat.Items.Add("Windows Media 128k")
                FileFormat.Items.Add("Windows Media 64k")

                If SKYSettingPremium > -1 Then
                    FileFormat.SelectedIndex = SKYSettingPremium
                Else
                    FileFormat.SelectedIndex = 3
                End If

            Else
                FileFormat.Items.Clear()
                FileFormat.Items.Add("AAC-HE")
                FileFormat.Items.Add("MP3")
                FileFormat.Items.Add("Windows Media")

                If SKYSetting > -1 Then
                    FileFormat.SelectedIndex = SKYSetting
                Else
                    FileFormat.SelectedIndex = 1
                End If

            End If
        ElseIf StationSelector.Text = "JazzRadio" Then
            FileFormat.Enabled = True

            If PremiumFormats.Checked = True Then
                FileFormat.Items.Clear()
                FileFormat.Items.Add("AAC-HE 128k")
                FileFormat.Items.Add("AAC-HE 64k")
                FileFormat.Items.Add("AAC-HE 40k")
                FileFormat.Items.Add("MP3 256k")
                FileFormat.Items.Add("MP3 128k")
                FileFormat.Items.Add("Windows Media 128k")

                If JazzSettingPremium > -1 Then
                    FileFormat.SelectedIndex = JazzSettingPremium
                Else
                    FileFormat.SelectedIndex = 3
                End If

            Else
                FileFormat.Items.Clear()
                FileFormat.Items.Add("AAC-HE")
                FileFormat.Items.Add("MP3")
                FileFormat.Items.Add("Windows Media")

                If JazzSetting > -1 Then
                    FileFormat.SelectedIndex = JazzSetting
                Else
                    FileFormat.SelectedIndex = 1
                End If

            End If
        ElseIf StationSelector.Text = "RockRadio" Then
            FileFormat.Items.Clear()
            FileFormat.Items.Add("MP3 96k")
            FileFormat.SelectedIndex = 0
            FileFormat.Enabled = False
        End If
    End Sub

    Private Sub FileFormat_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles FileFormat.SelectedIndexChanged

        If PremiumFormats.Checked = True Then
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

    End Sub

    Private Sub ListenLink_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles ListenLink.LinkClicked

        If StationSelector.Text = "Digitally Imported" OrElse StationSelector.Text = "RockRadio" Then
            Process.Start("http://www.di.fm/member/listen_key")
        ElseIf StationSelector.Text = "JazzRadio" Then
            Process.Start("http://www.jazzradio.com/member/listen_key")
        ElseIf StationSelector.Text = "SKY.FM" Then
            Process.Start("http://www.sky.fm/member/listen_key")
        End If

    End Sub

    Private Sub ListenKey_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListenKey.TextChanged
        If ListenKey.TextLength = Nothing = False Then
            ValidateKey.Enabled = True
            PremiumFormats.Enabled = True
        Else
            ValidateKey.Enabled = False
            PremiumFormats.Checked = False
            PremiumFormats.Enabled = False
        End If

        OK.Enabled = True
        Cancel.Text = "Cancel"
        Apply.Enabled = True
    End Sub

    Private Sub ValidateKey_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ValidateKey.Click
        ListenKey.Enabled = False
        ValidateKey.Text = "Wait..."
        ValidateKey.Enabled = False
        ValidateWorker.RunWorkerAsync()
    End Sub

    Public Sub LookNow_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LookNow.Click
        If LookNow.Text = "Look for updates now" Then
            LookNow.Enabled = False
            UndefinedProgress.Show()
            Status.Text = "Status: Looking for updates, please wait..."

            Player.GetUpdates.RunWorkerAsync()
        Else
            DownloadUpdater.RunWorkerAsync()
        End If
    End Sub

    Private Sub ViewChangelog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ViewChangelog.Click
        Changelog.Location = New Point(Me.Location.X + 5, Me.Location.Y + 28)
        Changelog.Show()
        Changelog.BringToFront()
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

    Private Sub Band0_ValueChanged(sender As Object, e As System.EventArgs) Handles Band0.ValueChanged
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

    Private Sub Band1_ValueChanged(sender As Object, e As System.EventArgs) Handles Band1.ValueChanged
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

    Private Sub Band2_ValueChanged(sender As Object, e As System.EventArgs) Handles Band2.ValueChanged
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

    Private Sub Band3_ValueChanged(sender As Object, e As System.EventArgs) Handles Band3.ValueChanged
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

    Private Sub Band4_ValueChanged(sender As Object, e As System.EventArgs) Handles Band4.ValueChanged
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

    Private Sub Band5_ValueChanged(sender As Object, e As System.EventArgs) Handles Band5.ValueChanged
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

    Private Sub Zero_Click(sender As System.Object, e As System.EventArgs) Handles Zero.Click
        Band0.Value = 0
        Band1.Value = 0
        Band2.Value = 0
        Band3.Value = 0
        Band4.Value = 0
        Band5.Value = 0
    End Sub

    Private Sub RestoreEq_Click(sender As System.Object, e As System.EventArgs) Handles RestoreEq.Click
        Band0.Value = Player.Band0
        Band1.Value = Player.Band1
        Band2.Value = Player.Band2
        Band3.Value = Player.Band3
        Band4.Value = Player.Band4
        Band5.Value = Player.Band5
    End Sub

    Private Sub AutoEq_Click(sender As System.Object, e As System.EventArgs) Handles AutoEq.Click
        Dim Highest As Integer

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

        Try
            Band0.Value -= Highest
        Catch
            Band0.Value = Band0.Minimum
        End Try

        Try
            Band1.Value -= Highest
        Catch
            Band1.Value = Band1.Minimum
        End Try

        Try
            Band2.Value -= Highest
        Catch
            Band2.Value = Band2.Minimum
        End Try

        Try
            Band3.Value -= Highest
        Catch
            Band3.Value = Band3.Minimum
        End Try

        Try
            Band4.Value -= Highest
        Catch
            Band4.Value = Band4.Minimum
        End Try

        Try
            Band5.Value -= Highest
        Catch
            Band5.Value = Band5.Minimum
        End Try
    End Sub

    Private Sub DevFacebook_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DevFacebook.Click
        Process.Start("www.facebook.com/rodbernard")
    End Sub

    Private Sub DevTwitter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DevTwitter.Click
        Process.Start("www.twitter.com/rodbernard")
    End Sub

    Private Sub DevMail_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DevMail.Click

        Try
            Process.Start("mailto:newvirus@live.com.ar?subject=Digitally Imported Windows Player")
        Catch
            MsgBox("newvirus@live.com.ar", MsgBoxStyle.Information, DevMail.Tag)
        End Try

    End Sub

    Private Sub DevForums_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DevForums.Click
        Process.Start("http://forums.di.fm/general-help-and-support/digitally-imported-windows-player-unofficial-269504/")
    End Sub

    Private Sub AboutText_LinkClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.LinkClickedEventArgs) Handles AboutText.LinkClicked
        Process.Start(e.LinkText)
    End Sub

    Private Sub OK_Click(sender As System.Object, e As System.EventArgs) Handles OK.Click
        ApplyOptions()
        Me.Close()
        Player.BringToFront()
    End Sub

    Private Sub Cancel_Click(sender As System.Object, e As System.EventArgs) Handles Cancel.Click
        Me.Close()

        Player.UpdateEq(0, Player.Band0)
        Player.UpdateEq(1, Player.Band1)
        Player.UpdateEq(2, Player.Band2)
        Player.UpdateEq(3, Player.Band3)
        Player.UpdateEq(4, Player.Band4)
        Player.UpdateEq(5, Player.Band5)
    End Sub

    Private Sub Apply_Click(sender As System.Object, e As System.EventArgs) Handles Apply.Click

        ApplyOptions()
        OK.Enabled = False
        Cancel.Text = "Close"
        Apply.Enabled = False
        Me.BringToFront()

    End Sub

    Private Sub TabControl1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabControl1.SelectedIndexChanged
        OnlyModifiers.Hide(Me)
    End Sub

#End Region

#Region "Hotkeys code"

    Private Sub HotkeyPlayStop_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles HotkeyPlayStop.CheckedChanged

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

    Private Sub CustomPlayStop_Click(sender As Object, e As System.EventArgs) Handles CustomPlayStop.Click
        CustomPlayStop.Text = "Press keys now"
        CustomPlayStop.Font = New Font(CustomPlayStop.Font, FontStyle.Italic)
        CustomPlayStop.ForeColor = Color.Gray
    End Sub

    Private Sub CustomPlayStop_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles CustomPlayStop.KeyDown
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

    Private Sub CustomPlayStop_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles CustomPlayStop.KeyUp
        CustomPlayStop.Text = KeyConverter.ConvertToString(HumanModifiersPlayStop + KeyPlayStop).Replace("None", Nothing)

        If KeyConverter.ConvertToString(HumanModifiersPlayStop + KeyPlayStop).Replace("None", Nothing) = Nothing = False Then
            CustomPlayStop.Font = New Font(CustomPlayStop.Font, FontStyle.Regular)
            CustomPlayStop.ForeColor = SystemColors.WindowText
        End If

    End Sub

    Private Sub HotkeyVolumeUp_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles HotkeyVolumeUp.CheckedChanged
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

    Private Sub CustomVolumeUp_Click(sender As Object, e As System.EventArgs) Handles CustomVolumeUp.Click
        CustomVolumeUp.Text = "Press keys now"
        CustomVolumeUp.Font = New Font(CustomVolumeUp.Font, FontStyle.Italic)
        CustomVolumeUp.ForeColor = Color.Gray
    End Sub

    Private Sub CustomVolumeUp_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles CustomVolumeUp.KeyDown
        If e.KeyValue = "16" Then
            ModifiersVolumeUp += 4
            HumanModifiersVolumeUp = 0
            KeyVolumeUp = 0
        ElseIf e.KeyValue = "17" Then
            ModifiersVolumeUp += 2
            HumanModifiersVolumeUp = 0
            KeyVolumeUp = 0
        ElseIf e.KeyValue = "18" Then
            ModifiersVolumeUp += 1
            HumanModifiersVolumeUp = 0
            KeyVolumeUp = 0
        Else
            HumanModifiersVolumeUp = e.Modifiers
            KeyVolumeUp = e.KeyValue
        End If
    End Sub

    Private Sub CustomVolumeUp_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles CustomVolumeUp.KeyUp
        CustomVolumeUp.Text = KeyConverter.ConvertToString(HumanModifiersVolumeUp + KeyVolumeUp).Replace("None", Nothing)

        If KeyConverter.ConvertToString(HumanModifiersVolumeUp + KeyVolumeUp).Replace("None", Nothing) = Nothing = False Then
            CustomVolumeUp.Font = New Font(CustomVolumeUp.Font, FontStyle.Regular)
            CustomVolumeUp.ForeColor = SystemColors.WindowText
        End If
    End Sub

    Private Sub HotkeyVolumeDown_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles HotkeyVolumeDown.CheckedChanged
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

    Private Sub CustomVolumeDown_Click(sender As Object, e As System.EventArgs) Handles CustomVolumeDown.Click
        CustomVolumeDown.Text = "Press keys now"
        CustomVolumeDown.Font = New Font(CustomVolumeDown.Font, FontStyle.Italic)
        CustomVolumeDown.ForeColor = Color.Gray
    End Sub

    Private Sub CustomVolumeDown_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles CustomVolumeDown.KeyDown
        If e.KeyValue = "16" Then
            ModifiersVolumeDown += 4
            HumanModifiersVolumeDown = 0
            KeyVolumeDown = 0
        ElseIf e.KeyValue = "17" Then
            ModifiersVolumeDown += 2
            HumanModifiersVolumeDown = 0
            KeyVolumeDown = 0
        ElseIf e.KeyValue = "18" Then
            ModifiersVolumeDown += 1
            HumanModifiersVolumeDown = 0
            KeyVolumeDown = 0
        Else
            HumanModifiersVolumeDown = e.Modifiers
            KeyVolumeDown = e.KeyValue
        End If
    End Sub

    Private Sub CustomVolumeDown_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles CustomVolumeDown.KeyUp
        CustomVolumeDown.Text = KeyConverter.ConvertToString(HumanModifiersVolumeDown + KeyVolumeDown).Replace("None", Nothing)
        If KeyConverter.ConvertToString(HumanModifiersVolumeDown + KeyVolumeDown).Replace("None", Nothing) = Nothing = False Then
            CustomVolumeDown.Font = New Font(CustomVolumeDown.Font, FontStyle.Regular)
            CustomVolumeDown.ForeColor = SystemColors.WindowText
        End If
    End Sub

    Private Sub HotkeyMuteUnmute_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles HotkeyMuteUnmute.CheckedChanged
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

    Private Sub CustomMuteUnmute_Click(sender As Object, e As System.EventArgs) Handles CustomMuteUnmute.Click
        CustomMuteUnmute.Text = "Press keys now"
        CustomMuteUnmute.Font = New Font(CustomMuteUnmute.Font, FontStyle.Italic)
        CustomMuteUnmute.ForeColor = Color.Gray
    End Sub

    Private Sub CustomMuteUnmute_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles CustomMuteUnmute.KeyDown
        If e.KeyValue = "16" Then
            ModifiersMuteUnmute += 4
            HumanModifiersMuteUnmute = 0
            KeyMuteUnmute = 0
        ElseIf e.KeyValue = "17" Then
            ModifiersMuteUnmute += 2
            HumanModifiersMuteUnmute = 0
            KeyMuteUnmute = 0
        ElseIf e.KeyValue = "18" Then
            ModifiersMuteUnmute += 1
            HumanModifiersMuteUnmute = 0
            KeyMuteUnmute = 0
        Else
            HumanModifiersMuteUnmute = e.Modifiers
            KeyMuteUnmute = e.KeyValue
        End If
    End Sub

    Private Sub CustomMuteUnmute_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles CustomMuteUnmute.KeyUp
        CustomMuteUnmute.Text = KeyConverter.ConvertToString(HumanModifiersMuteUnmute + KeyMuteUnmute).Replace("None", Nothing)
        If KeyConverter.ConvertToString(HumanModifiersMuteUnmute + KeyMuteUnmute).Replace("None", Nothing) = Nothing = False Then
            CustomMuteUnmute.Font = New Font(CustomMuteUnmute.Font, FontStyle.Regular)
            CustomMuteUnmute.ForeColor = SystemColors.WindowText
        End If
    End Sub

    Private Sub CustomPlayStop_TextChanged(sender As System.Object, e As System.EventArgs) Handles CustomPlayStop.TextChanged
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
            ModifiersShowHide += 4
            HumanModifiersShowHide = 0
            KeyShowHide = 0
        ElseIf e.KeyValue = "17" Then
            ModifiersShowHide += 2
            HumanModifiersShowHide = 0
            KeyShowHide = 0
        ElseIf e.KeyValue = "18" Then
            ModifiersShowHide += 1
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

#Region "ValidateWorker"

    Private Sub ValidateWorker_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles ValidateWorker.DoWork

        Dim WebClient As Net.WebClient = New Net.WebClient()
        Dim PLS As String
        Dim URL As String
        Dim Message As New MsgBoxSafe(AddressOf DisplayMessage)

        If StationSelector.Text = "Digitally Imported" OrElse StationSelector.Text = "RockRadio" Then
            URL = Player.DIFM.Tag
        ElseIf StationSelector.Text = "JazzRadio" Then
            URL = Player.JazzRadio.Tag
        ElseIf StationSelector.Text = "SKY.FM" Then
            URL = Player.SKYFM.Tag
        End If

        Try
            PLS = WebClient.DownloadString("http://listen." & URL & "/premium/favorites.pls?" & ListenKey.Text)
            Me.Invoke(Message, "That listen key is valid.", MsgBoxStyle.Information, "Success")
        Catch ex As Exception
            If ex.Message.Contains("403") Then
                Me.Invoke(Message, "That Listen Key is invalid.", MsgBoxStyle.Exclamation, "Invalid key")
            Else
                Me.Invoke(Message, "Couldn't validate the Listen Key due to the following error:" & vbNewLine & ex.Message & vbNewLine & vbNewLine & "Please try again.", MsgBoxStyle.Exclamation, "Validation failed")
            End If
        End Try

    End Sub

    Private Sub ValidateWorker_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles ValidateWorker.RunWorkerCompleted
        ListenKey.Enabled = True
        ValidateKey.Text = "Validate"
        ValidateKey.Enabled = True
    End Sub

#End Region

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

        If ListenKey.Text.Length < 14 Then

            If Player.SelectedChannel.Text = "My Favorites" Then
                Player.SelectedServer.Items.Clear()
            End If

            Player.SelectedChannel.Items.Remove("My Favorites")
            Player.RefreshFavorites.Hide()
            Player.EditFavorites.Hide()

        Else

            If Player.StationChooser.Text = Player.DIFM.Text Then

                If Player.SelectedChannel.Items.Item(38).ToString() = "My Favorites" = False Then
                    Player.SelectedChannel.Items.Add("My Favorites")
                End If

            ElseIf Player.StationChooser.Text = Player.SKYFM.Text Then

                If Player.SelectedChannel.Items.Item(33).ToString() = "My Favorites" = False Then
                    Player.SelectedChannel.Items.Add("My Favorites")
                End If

            ElseIf Player.StationChooser.Text = Player.JazzRadio.Text Then

                If Player.SelectedChannel.Items.Item(16).ToString() = "My Favorites" = False Then
                    Player.SelectedChannel.Items.Add("My Favorites")
                End If

            End If


        End If

        If PremiumFormats.Checked = Player.PremiumFormats = False OrElse ListenKey.Text = Player.ListenKey = False Then

            If PremiumFormats.Checked = False Then
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
                    Player.JazzFormat = JazzSetting
                Else
                    Player.JazzFormat = 3
                End If
            End If

            Try
                Kill(Player.exeFolder & "servers\Digitally Imported\*.*")
            Catch
            End Try
            Try
                Kill(Player.exeFolder & "servers\JazzRadio\*.*")
            Catch
            End Try
            Try
                Kill(Player.exeFolder & "servers\SKY.FM\*.*")
            Catch
            End Try


            If Player.SelectedChannel.Text = "My Favorites" Then
                Player.OldFav = Player.SelectedServer.Text
            End If

            If Player.PlayStop.Tag = "Stop" And Player.PlayStop.Enabled = True Then
                Player.PlayStop_Click(Me, Nothing)
                Player.RestartPlayback = True
            End If

            If Player.SelectedChannel.Text = Nothing = False Then

                If Player.PLSDownloader.IsBusy = False Then
                    Player.SelectedChannel_SelectedIndexChanged(Me, Nothing)
                End If

            Else
                Player.SelectedServer.Enabled = False
                Player.SelectedServer.Items.Add("Pick a server")
                Player.SelectedServer.SelectedIndex = 0
            End If

        End If

        Player.ListenKey = ListenKey.Text

        If Player.DIFormat = DISetting = False And Player.DIFormat = DISettingPremium = False Then

            If PremiumFormats.Checked = False Then
                Player.DIFormat = DISetting
            Else
                Player.DIFormat = DISettingPremium
            End If


            Try
                Kill(Player.exeFolder & "servers\Digitally Imported\*.*")
            Catch
            End Try

            If Player.StationChooser.Text = Player.DIFM.Text Then

                If Player.SelectedChannel.Text = "My Favorites" Then
                    Player.OldFav = Player.SelectedServer.Text
                End If


                If Player.PlayStop.Tag = "Stop" And Player.PlayStop.Enabled = True Then
                    Player.PlayStop_Click(Me, Nothing)
                    Player.RestartPlayback = True
                End If


                If Player.SelectedChannel.Text = Nothing = False Then
                    If Player.PLSDownloader.IsBusy = False Then
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

            If PremiumFormats.Checked = False Then
                Player.SKYFormat = SKYSetting
            Else
                Player.SKYFormat = SKYSettingPremium
            End If


            Try
                Kill(Player.exeFolder & "servers\SKY.FM\*.*")
            Catch
            End Try

            If Player.StationChooser.Text = Player.SKYFM.Text Then

                If Player.SelectedChannel.Text = "My Favorites" Then
                    Player.OldFav = Player.SelectedServer.Text
                End If

                If Player.PlayStop.Tag = "Stop" And Player.PlayStop.Enabled = True Then
                    Player.PlayStop_Click(Me, Nothing)
                    Player.RestartPlayback = True
                End If

                If Player.SelectedChannel.Text = Nothing = False Then
                    If Player.PLSDownloader.IsBusy = False Then
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

            If PremiumFormats.Checked = False Then
                Player.JazzFormat = JazzSetting
            Else
                Player.JazzFormat = JazzSettingPremium
            End If


            Try
                Kill(Player.exeFolder & "servers\JazzRadio\*.*")
            Catch
            End Try

            If Player.StationChooser.Text = Player.JazzRadio.Text Then

                If Player.SelectedChannel.Text = "My Favorites" Then
                    Player.OldFav = Player.SelectedServer.Text
                End If

                If Player.PlayStop.Tag = "Stop" And Player.PlayStop.Enabled = True Then
                    Player.PlayStop_Click(Me, Nothing)
                    Player.RestartPlayback = True
                End If

                If Player.SelectedChannel.Text = Nothing = False Then
                    If Player.PLSDownloader.IsBusy = False Then
                        Player.SelectedChannel_SelectedIndexChanged(Me, Nothing)
                    End If
                Else
                    Player.SelectedServer.Enabled = False
                    Player.SelectedServer.Items.Add("Pick a server")
                    Player.SelectedServer.SelectedIndex = 0
                End If

            End If
        End If

        Player.PremiumFormats = PremiumFormats.Checked
        Player.UpdatesAtStart = UpdatesAtStart.Checked
        Player.BetaVersions = BetaVersions.Checked
        Player.Visualisation = Visualisation.Checked

        If Visualisation.Checked = True Then

            Player.VisualisationBox.Show()
            Player.Size = New Size(Player.MaximumSize)

            If Player.PlayStop.Tag = "Stop" Then
                Player.VisTimer.Start()
            End If

        Else

            Player.VisTimer.Stop()
            Player.VisualisationBox.Hide()
            Player.Size = New Size(Player.MinimumSize)

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


        If ChangeWholeBackground.Checked = True Then

            Player.BackColor = BackgroundColour.BackColor
            Player.ToolStrip1.BackColor = BackgroundColour.BackColor
            Player.StationChooser.BackColor = BackgroundColour.BackColor
            Player.Label1.BackColor = BackgroundColour.BackColor
            Player.Label2.BackColor = BackgroundColour.BackColor

            If Player.RadioString.Text.ToLower.StartsWith("lost connection to") = False And Player.RadioString.Text.ToLower.StartsWith("couldn't connect to") = False And Player.RadioString.Text.ToLower.StartsWith("connection is taking") = False Then
                Player.RadioString.BackColor = BackgroundColour.BackColor

                If BackgroundColour.BackColor.ToArgb() < -8323328 Then
                    Player.RadioString.ForeColor = Color.White
                    Player.TimerString.ForeColor = Color.White
                Else
                    Player.RadioString.ForeColor = Color.Black
                    Player.TimerString.ForeColor = Color.Black
                End If

                If BackgroundColour.BackColor.ToArgb() < -7105537 Then
                    Player.EditFavorites.LinkColor = Color.White
                    Player.RefreshFavorites.LinkColor = Color.White
                Else
                    Player.EditFavorites.LinkColor = Color.Blue
                    Player.RefreshFavorites.LinkColor = Color.Blue
                End If
            End If

        Else

            Player.BackColor = SystemColors.Control
            Player.ToolStrip1.BackColor = SystemColors.Control
            Player.StationChooser.BackColor = SystemColors.Control
            Player.Label1.BackColor = SystemColors.Control

            If Player.RadioString.Text.ToLower.StartsWith("internet connection") = False And Player.RadioString.Text.ToLower.StartsWith("lost connection to") = False And Player.RadioString.Text.ToLower.StartsWith("couldn't connect to") = False And Player.RadioString.Text.ToLower.StartsWith("connection is taking") = False Then
                Player.RadioString.BackColor = SystemColors.Control
                Player.RadioString.ForeColor = SystemColors.WindowText
                Player.TimerString.ForeColor = SystemColors.WindowText
            End If

        End If

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

        If Player.WindowState = FormWindowState.Minimized And Player.MultimediaKeys = True Then
            Player.UnregisterHotKey(Player.Handle, 1)
            Player.UnregisterHotKey(Player.Handle, 2)
            Player.UnregisterHotKey(Player.Handle, 3)
            Player.UnregisterHotKey(Player.Handle, 4)
            Player.UnregisterHotKey(Player.Handle, 5)
            Player.UnregisterHotKey(Player.Handle, 6)

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
    End Sub

    Private Sub ColourPicker_HelpRequest(sender As System.Object, e As System.EventArgs) Handles ColourPicker.HelpRequest
        If ColourPicker.Tag.ToString.Contains("secondary") Then
            MessageBox.Show("You're selecting the Secondary colour of the visualisation." & vbNewLine & "This colour is used at the top of the visualisation as the frequency levels reach higher values." & vbNewLine & "The middle range of the visualisation uses a mixture of the Main and Secondary colours." & vbNewLine & vbNewLine & "You can select a colour from the default ones on the left side of the window or create a custom one by dragging the plus sign inside the color hue and the arrow on the brightness level." & vbNewLine & "Alternatively, you can write your own custom values on the number boxes to define a colour.", "Colour help", MessageBoxButtons.OK, MessageBoxIcon.Information)
        ElseIf ColourPicker.Tag.ToString.Contains("main") Then
            MessageBox.Show("You're selecting the Main colour of the visualisation." & vbNewLine & "This colour is used at the bottom of the visualisation and is almost always displayed." & vbNewLine & "The middle range of the visualisation uses a mixture of the Main and Secondary colours." & vbNewLine & vbNewLine & "You can select a colour from the default ones on the left side of the window or create a custom one by dragging the plus sign inside the color hue and the arrow on the brightness level." & vbNewLine & "Alternatively, you can write your own custom values on the number boxes to define a colour.", "Colour help", MessageBoxButtons.OK, MessageBoxIcon.Information)
        ElseIf ColourPicker.Tag.ToString.Contains("peaks") Then
            MessageBox.Show("You're selecting the colour that will be used for the peaks when the Lines with peaks visualisation is selected." & vbNewLine & "The peaks are the squares displayed at the top of the lines." & vbNewLine & vbNewLine & "You can select a colour from the default ones on the left side of the window or create a custom one by dragging the plus sign inside the color hue and the arrow on the brightness level." & vbNewLine & "Alternatively, you can write your own custom values on the number boxes to define a colour.", "Colour help", MessageBoxButtons.OK, MessageBoxIcon.Information)
        ElseIf ColourPicker.Tag.ToString.Contains("background") Then
            MessageBox.Show("You're selecting the colour that will be used as a background on the visualisation area." & vbNewLine & "If you check the 'Change the whole background' box, this colour will be used as a background for the entire player, even when the visualisation is disabled." & vbNewLine & "If you select a colour that's too dark, the title and time displays will use a white font colour; and if you select a colour that's too bright, they will use a black font colour." & vbNewLine & vbNewLine & "You can select a colour from the default ones on the left side of the window or create a custom one by dragging the plus sign inside the color hue and the arrow on the brightness level." & vbNewLine & "Alternatively, you can write your own custom values on the number boxes to define a colour.", "Colour help", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub DILogo_Click(sender As System.Object, e As System.EventArgs) Handles DILogo.Click
        Process.Start("http://www.di.fm")
    End Sub

    Private Sub JazzLogo_Click(sender As System.Object, e As System.EventArgs) Handles JazzLogo.Click
        Process.Start("http://www.jazzradio.com")
    End Sub

    Private Sub RockLogo_Click(sender As System.Object, e As System.EventArgs) Handles RockLogo.Click
        Process.Start("http://www.rockradio.com")
    End Sub

    Private Sub SkyLogo_Click(sender As System.Object, e As System.EventArgs) Handles SkyLogo.Click
        Process.Start("http://www.sky.fm")
    End Sub

    Private Sub DownloadUpdater_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles DownloadUpdater.DoWork
        Status.Text = "Status: Connecting to server, please wait..."
        LookNow.Enabled = False
        UndefinedProgress.Hide()

        Dim file As String = Player.exeFolder & "\Updater.exe"
        Dim Message As New MsgBoxSafe(AddressOf DisplayMessage)

        Dim theResponse As Net.HttpWebResponse
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
        theResponse.GetResponseStream.Close()
    End Sub

    Private Sub DownloadUpdater_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles DownloadUpdater.RunWorkerCompleted
        Status.Text = "Status: Updater downloaded. Launching and exiting..."

        Dim executable As String = Application.ExecutablePath
        Dim tabla() As String = Split(executable, "\")
        Dim file As String = Application.ExecutablePath.Replace(tabla(tabla.Length - 1), Nothing) & "\Updater.exe"
        Process.Start(file)
        Player.Close()
    End Sub
End Class