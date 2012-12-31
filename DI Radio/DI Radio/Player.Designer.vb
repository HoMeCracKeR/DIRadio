<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Player
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Player))
        Me.CopyTitleMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.CopyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.GoogleSearchToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ServersDownloader = New System.ComponentModel.BackgroundWorker()
        Me.Bufer = New System.ComponentModel.BackgroundWorker()
        Me.VisTimer = New System.Windows.Forms.Timer(Me.components)
        Me.TimePassed = New System.Windows.Forms.Timer(Me.components)
        Me.TrayIcon = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.TrayMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.OptionsTray = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.GoogleSearchTray = New System.Windows.Forms.ToolStripMenuItem()
        Me.ForumsTray = New System.Windows.Forms.ToolStripMenuItem()
        Me.CalendarTray = New System.Windows.Forms.ToolStripMenuItem()
        Me.TrackHistoryTray = New System.Windows.Forms.ToolStripMenuItem()
        Me.CopyTitleTray = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.MuteTray = New System.Windows.Forms.ToolStripMenuItem()
        Me.PlayStopTray = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.ExitTray = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.Forums = New System.Windows.Forms.Button()
        Me.History = New System.Windows.Forms.Button()
        Me.Calendar = New System.Windows.Forms.Button()
        Me.Mute = New System.Windows.Forms.Button()
        Me.OptionsButton = New System.Windows.Forms.Button()
        Me.PlayStop = New System.Windows.Forms.Button()
        Me.Volume = New System.Windows.Forms.TrackBar()
        Me.TimerString = New System.Windows.Forms.Label()
        Me.RadioString = New System.Windows.Forms.Label()
        Me.SelectedChannel = New System.Windows.Forms.ComboBox()
        Me.SelectedServer = New System.Windows.Forms.ComboBox()
        Me.ServerMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.CopyServerURLToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Marquee = New System.Windows.Forms.ProgressBar()
        Me.ControlsPanel = New System.Windows.Forms.Panel()
        Me.DownloadingMessage = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.StationChooser = New System.Windows.Forms.ToolStripSplitButton()
        Me.DIFM = New System.Windows.Forms.ToolStripMenuItem()
        Me.JazzRadio = New System.Windows.Forms.ToolStripMenuItem()
        Me.RockRadio = New System.Windows.Forms.ToolStripMenuItem()
        Me.SKYFM = New System.Windows.Forms.ToolStripMenuItem()
        Me.EditFavorites = New System.Windows.Forms.LinkLabel()
        Me.RefreshFavorites = New System.Windows.Forms.LinkLabel()
        Me.GetUpdates = New System.ComponentModel.BackgroundWorker()
        Me.FadeOut = New System.Windows.Forms.Timer(Me.components)
        Me.DownloadDb = New System.ComponentModel.BackgroundWorker()
        Me.VisualisationBox = New System.Windows.Forms.PictureBox()
        Me.HistoryList = New System.Windows.Forms.ListView()
        Me.Title = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Length = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Label2 = New System.Windows.Forms.Label()
        Me.CopyTitleMenu.SuspendLayout()
        Me.TrayMenu.SuspendLayout()
        CType(Me.Volume, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ServerMenu.SuspendLayout()
        Me.ControlsPanel.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        CType(Me.VisualisationBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'CopyTitleMenu
        '
        Me.CopyTitleMenu.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CopyTitleMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CopyToolStripMenuItem, Me.ToolStripSeparator4, Me.GoogleSearchToolStripMenuItem})
        Me.CopyTitleMenu.Name = "CopyTitleMenu"
        Me.CopyTitleMenu.Size = New System.Drawing.Size(156, 54)
        '
        'CopyToolStripMenuItem
        '
        Me.CopyToolStripMenuItem.Image = CType(resources.GetObject("CopyToolStripMenuItem.Image"), System.Drawing.Image)
        Me.CopyToolStripMenuItem.Name = "CopyToolStripMenuItem"
        Me.CopyToolStripMenuItem.Size = New System.Drawing.Size(155, 22)
        Me.CopyToolStripMenuItem.Text = "Copy"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(152, 6)
        '
        'GoogleSearchToolStripMenuItem
        '
        Me.GoogleSearchToolStripMenuItem.Image = CType(resources.GetObject("GoogleSearchToolStripMenuItem.Image"), System.Drawing.Image)
        Me.GoogleSearchToolStripMenuItem.Name = "GoogleSearchToolStripMenuItem"
        Me.GoogleSearchToolStripMenuItem.Size = New System.Drawing.Size(155, 22)
        Me.GoogleSearchToolStripMenuItem.Text = "Google search"
        '
        'ServersDownloader
        '
        Me.ServersDownloader.WorkerSupportsCancellation = True
        '
        'Bufer
        '
        Me.Bufer.WorkerSupportsCancellation = True
        '
        'VisTimer
        '
        Me.VisTimer.Interval = 25
        '
        'TimePassed
        '
        Me.TimePassed.Interval = 1000
        '
        'TrayIcon
        '
        Me.TrayIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info
        Me.TrayIcon.BalloonTipTitle = "Now playing:"
        Me.TrayIcon.ContextMenuStrip = Me.TrayMenu
        Me.TrayIcon.Icon = CType(resources.GetObject("TrayIcon.Icon"), System.Drawing.Icon)
        Me.TrayIcon.Text = "DI Radio Player"
        '
        'TrayMenu
        '
        Me.TrayMenu.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TrayMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OptionsTray, Me.ToolStripSeparator3, Me.GoogleSearchTray, Me.ForumsTray, Me.CalendarTray, Me.TrackHistoryTray, Me.CopyTitleTray, Me.ToolStripSeparator1, Me.MuteTray, Me.PlayStopTray, Me.ToolStripSeparator2, Me.ExitTray})
        Me.TrayMenu.Name = "TrayIconMenu"
        Me.TrayMenu.Size = New System.Drawing.Size(173, 220)
        '
        'OptionsTray
        '
        Me.OptionsTray.Image = CType(resources.GetObject("OptionsTray.Image"), System.Drawing.Image)
        Me.OptionsTray.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.OptionsTray.Name = "OptionsTray"
        Me.OptionsTray.Size = New System.Drawing.Size(172, 22)
        Me.OptionsTray.Text = "Options"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(169, 6)
        '
        'GoogleSearchTray
        '
        Me.GoogleSearchTray.Image = CType(resources.GetObject("GoogleSearchTray.Image"), System.Drawing.Image)
        Me.GoogleSearchTray.Name = "GoogleSearchTray"
        Me.GoogleSearchTray.Size = New System.Drawing.Size(172, 22)
        Me.GoogleSearchTray.Text = "Google search"
        '
        'ForumsTray
        '
        Me.ForumsTray.Image = CType(resources.GetObject("ForumsTray.Image"), System.Drawing.Image)
        Me.ForumsTray.Name = "ForumsTray"
        Me.ForumsTray.Size = New System.Drawing.Size(172, 22)
        Me.ForumsTray.Text = "Open forums"
        '
        'CalendarTray
        '
        Me.CalendarTray.Image = CType(resources.GetObject("CalendarTray.Image"), System.Drawing.Image)
        Me.CalendarTray.Name = "CalendarTray"
        Me.CalendarTray.Size = New System.Drawing.Size(172, 22)
        Me.CalendarTray.Text = "Open calendar"
        '
        'TrackHistoryTray
        '
        Me.TrackHistoryTray.Image = CType(resources.GetObject("TrackHistoryTray.Image"), System.Drawing.Image)
        Me.TrackHistoryTray.Name = "TrackHistoryTray"
        Me.TrackHistoryTray.Size = New System.Drawing.Size(172, 22)
        Me.TrackHistoryTray.Text = "Open track history"
        '
        'CopyTitleTray
        '
        Me.CopyTitleTray.Image = CType(resources.GetObject("CopyTitleTray.Image"), System.Drawing.Image)
        Me.CopyTitleTray.Name = "CopyTitleTray"
        Me.CopyTitleTray.Size = New System.Drawing.Size(172, 22)
        Me.CopyTitleTray.Text = "Copy song title"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(169, 6)
        '
        'MuteTray
        '
        Me.MuteTray.Image = CType(resources.GetObject("MuteTray.Image"), System.Drawing.Image)
        Me.MuteTray.Name = "MuteTray"
        Me.MuteTray.Size = New System.Drawing.Size(172, 22)
        Me.MuteTray.Text = "Mute"
        '
        'PlayStopTray
        '
        Me.PlayStopTray.Image = CType(resources.GetObject("PlayStopTray.Image"), System.Drawing.Image)
        Me.PlayStopTray.Name = "PlayStopTray"
        Me.PlayStopTray.Size = New System.Drawing.Size(172, 22)
        Me.PlayStopTray.Text = "Play"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(169, 6)
        '
        'ExitTray
        '
        Me.ExitTray.Image = CType(resources.GetObject("ExitTray.Image"), System.Drawing.Image)
        Me.ExitTray.Name = "ExitTray"
        Me.ExitTray.Size = New System.Drawing.Size(172, 22)
        Me.ExitTray.Text = "Exit"
        '
        'Forums
        '
        Me.Forums.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.Forums.Image = CType(resources.GetObject("Forums.Image"), System.Drawing.Image)
        Me.Forums.ImageAlign = System.Drawing.ContentAlignment.BottomRight
        Me.Forums.Location = New System.Drawing.Point(129, 20)
        Me.Forums.Name = "Forums"
        Me.Forums.Size = New System.Drawing.Size(25, 25)
        Me.Forums.TabIndex = 5
        Me.ToolTip.SetToolTip(Me.Forums, "Open channel's forums")
        Me.Forums.UseVisualStyleBackColor = True
        '
        'History
        '
        Me.History.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.History.Image = CType(resources.GetObject("History.Image"), System.Drawing.Image)
        Me.History.ImageAlign = System.Drawing.ContentAlignment.BottomRight
        Me.History.Location = New System.Drawing.Point(98, 20)
        Me.History.Name = "History"
        Me.History.Size = New System.Drawing.Size(25, 25)
        Me.History.TabIndex = 4
        Me.ToolTip.SetToolTip(Me.History, "Open channel's track history")
        Me.History.UseVisualStyleBackColor = True
        '
        'Calendar
        '
        Me.Calendar.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.Calendar.Image = CType(resources.GetObject("Calendar.Image"), System.Drawing.Image)
        Me.Calendar.Location = New System.Drawing.Point(67, 20)
        Me.Calendar.Name = "Calendar"
        Me.Calendar.Size = New System.Drawing.Size(25, 25)
        Me.Calendar.TabIndex = 3
        Me.ToolTip.SetToolTip(Me.Calendar, "Open channel's calendar")
        Me.Calendar.UseVisualStyleBackColor = True
        '
        'Mute
        '
        Me.Mute.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.Mute.Image = CType(resources.GetObject("Mute.Image"), System.Drawing.Image)
        Me.Mute.Location = New System.Drawing.Point(160, 20)
        Me.Mute.Name = "Mute"
        Me.Mute.Size = New System.Drawing.Size(25, 25)
        Me.Mute.TabIndex = 6
        Me.Mute.Tag = "Mute"
        Me.ToolTip.SetToolTip(Me.Mute, "Mute")
        Me.Mute.UseVisualStyleBackColor = True
        '
        'OptionsButton
        '
        Me.OptionsButton.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.OptionsButton.Image = CType(resources.GetObject("OptionsButton.Image"), System.Drawing.Image)
        Me.OptionsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.OptionsButton.Location = New System.Drawing.Point(36, 20)
        Me.OptionsButton.Name = "OptionsButton"
        Me.OptionsButton.Size = New System.Drawing.Size(25, 25)
        Me.OptionsButton.TabIndex = 2
        Me.ToolTip.SetToolTip(Me.OptionsButton, "Options")
        Me.OptionsButton.UseVisualStyleBackColor = True
        '
        'PlayStop
        '
        Me.PlayStop.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.PlayStop.Image = CType(resources.GetObject("PlayStop.Image"), System.Drawing.Image)
        Me.PlayStop.Location = New System.Drawing.Point(0, 17)
        Me.PlayStop.Name = "PlayStop"
        Me.PlayStop.Size = New System.Drawing.Size(30, 30)
        Me.PlayStop.TabIndex = 1
        Me.PlayStop.Tag = "Play"
        Me.ToolTip.SetToolTip(Me.PlayStop, "Play")
        Me.PlayStop.UseVisualStyleBackColor = True
        '
        'Volume
        '
        Me.Volume.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.Volume.AutoSize = False
        Me.Volume.Location = New System.Drawing.Point(191, 20)
        Me.Volume.Maximum = 100
        Me.Volume.Name = "Volume"
        Me.Volume.Size = New System.Drawing.Size(138, 25)
        Me.Volume.TabIndex = 7
        Me.Volume.TickStyle = System.Windows.Forms.TickStyle.None
        Me.Volume.Value = 100
        '
        'TimerString
        '
        Me.TimerString.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.TimerString.AutoEllipsis = True
        Me.TimerString.Location = New System.Drawing.Point(296, 0)
        Me.TimerString.Name = "TimerString"
        Me.TimerString.Size = New System.Drawing.Size(34, 13)
        Me.TimerString.TabIndex = 11
        Me.TimerString.Text = "00:00"
        Me.TimerString.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'RadioString
        '
        Me.RadioString.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.RadioString.AutoEllipsis = True
        Me.RadioString.ContextMenuStrip = Me.CopyTitleMenu
        Me.RadioString.Location = New System.Drawing.Point(-3, 0)
        Me.RadioString.Name = "RadioString"
        Me.RadioString.Size = New System.Drawing.Size(300, 14)
        Me.RadioString.TabIndex = 10
        Me.RadioString.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.RadioString.UseMnemonic = False
        '
        'SelectedChannel
        '
        Me.SelectedChannel.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.SelectedChannel.DropDownHeight = 394
        Me.SelectedChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.SelectedChannel.DropDownWidth = 149
        Me.SelectedChannel.FormattingEnabled = True
        Me.SelectedChannel.IntegralHeight = False
        Me.SelectedChannel.ItemHeight = 14
        Me.SelectedChannel.Items.AddRange(New Object() {"Downloading channels..."})
        Me.SelectedChannel.Location = New System.Drawing.Point(36, 59)
        Me.SelectedChannel.MaxDropDownItems = 7
        Me.SelectedChannel.Name = "SelectedChannel"
        Me.SelectedChannel.Size = New System.Drawing.Size(149, 22)
        Me.SelectedChannel.Sorted = True
        Me.SelectedChannel.TabIndex = 8
        '
        'SelectedServer
        '
        Me.SelectedServer.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.SelectedServer.ContextMenuStrip = Me.ServerMenu
        Me.SelectedServer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.SelectedServer.FormattingEnabled = True
        Me.SelectedServer.Location = New System.Drawing.Point(191, 59)
        Me.SelectedServer.Name = "SelectedServer"
        Me.SelectedServer.Size = New System.Drawing.Size(139, 22)
        Me.SelectedServer.TabIndex = 9
        '
        'ServerMenu
        '
        Me.ServerMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CopyServerURLToolStripMenuItem})
        Me.ServerMenu.Name = "ServerMenu"
        Me.ServerMenu.Size = New System.Drawing.Size(161, 26)
        '
        'CopyServerURLToolStripMenuItem
        '
        Me.CopyServerURLToolStripMenuItem.Image = CType(resources.GetObject("CopyServerURLToolStripMenuItem.Image"), System.Drawing.Image)
        Me.CopyServerURLToolStripMenuItem.Name = "CopyServerURLToolStripMenuItem"
        Me.CopyServerURLToolStripMenuItem.Size = New System.Drawing.Size(160, 22)
        Me.CopyServerURLToolStripMenuItem.Text = "Copy server URL"
        '
        'Marquee
        '
        Me.Marquee.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.Marquee.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.Marquee.Location = New System.Drawing.Point(191, 59)
        Me.Marquee.Name = "Marquee"
        Me.Marquee.Size = New System.Drawing.Size(139, 22)
        Me.Marquee.Style = System.Windows.Forms.ProgressBarStyle.Marquee
        Me.Marquee.TabIndex = 10
        Me.Marquee.Visible = False
        '
        'ControlsPanel
        '
        Me.ControlsPanel.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.ControlsPanel.Controls.Add(Me.Label2)
        Me.ControlsPanel.Controls.Add(Me.DownloadingMessage)
        Me.ControlsPanel.Controls.Add(Me.Label1)
        Me.ControlsPanel.Controls.Add(Me.ToolStrip1)
        Me.ControlsPanel.Controls.Add(Me.EditFavorites)
        Me.ControlsPanel.Controls.Add(Me.RefreshFavorites)
        Me.ControlsPanel.Controls.Add(Me.Marquee)
        Me.ControlsPanel.Controls.Add(Me.SelectedServer)
        Me.ControlsPanel.Controls.Add(Me.Forums)
        Me.ControlsPanel.Controls.Add(Me.History)
        Me.ControlsPanel.Controls.Add(Me.SelectedChannel)
        Me.ControlsPanel.Controls.Add(Me.Calendar)
        Me.ControlsPanel.Controls.Add(Me.RadioString)
        Me.ControlsPanel.Controls.Add(Me.TimerString)
        Me.ControlsPanel.Controls.Add(Me.Volume)
        Me.ControlsPanel.Controls.Add(Me.Mute)
        Me.ControlsPanel.Controls.Add(Me.OptionsButton)
        Me.ControlsPanel.Controls.Add(Me.PlayStop)
        Me.ControlsPanel.Location = New System.Drawing.Point(13, 352)
        Me.ControlsPanel.Name = "ControlsPanel"
        Me.ControlsPanel.Size = New System.Drawing.Size(330, 81)
        Me.ControlsPanel.TabIndex = 2
        '
        'DownloadingMessage
        '
        Me.DownloadingMessage.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.DownloadingMessage.DropDownHeight = 394
        Me.DownloadingMessage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.DownloadingMessage.DropDownWidth = 149
        Me.DownloadingMessage.Enabled = False
        Me.DownloadingMessage.FormattingEnabled = True
        Me.DownloadingMessage.IntegralHeight = False
        Me.DownloadingMessage.ItemHeight = 14
        Me.DownloadingMessage.Items.AddRange(New Object() {"Downloading channels..."})
        Me.DownloadingMessage.Location = New System.Drawing.Point(36, 59)
        Me.DownloadingMessage.MaxDropDownItems = 7
        Me.DownloadingMessage.Name = "DownloadingMessage"
        Me.DownloadingMessage.Size = New System.Drawing.Size(149, 22)
        Me.DownloadingMessage.Sorted = True
        Me.DownloadingMessage.TabIndex = 13
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.SystemColors.Control
        Me.Label1.Location = New System.Drawing.Point(32, 59)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(4, 22)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = " "
        '
        'ToolStrip1
        '
        Me.ToolStrip1.AutoSize = False
        Me.ToolStrip1.BackColor = System.Drawing.SystemColors.Control
        Me.ToolStrip1.Dock = System.Windows.Forms.DockStyle.None
        Me.ToolStrip1.GripMargin = New System.Windows.Forms.Padding(0)
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StationChooser})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 59)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(33, 22)
        Me.ToolStrip1.TabIndex = 4
        Me.ToolStrip1.Text = "Digitally Imported"
        '
        'StationChooser
        '
        Me.StationChooser.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.StationChooser.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DIFM, Me.JazzRadio, Me.RockRadio, Me.SKYFM})
        Me.StationChooser.Image = CType(resources.GetObject("StationChooser.Image"), System.Drawing.Image)
        Me.StationChooser.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.StationChooser.Name = "StationChooser"
        Me.StationChooser.Size = New System.Drawing.Size(32, 19)
        Me.StationChooser.Tag = "di.fm"
        Me.StationChooser.ToolTipText = "Digitally Imported"
        '
        'DIFM
        '
        Me.DIFM.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DIFM.Image = CType(resources.GetObject("DIFM.Image"), System.Drawing.Image)
        Me.DIFM.Name = "DIFM"
        Me.DIFM.Size = New System.Drawing.Size(169, 22)
        Me.DIFM.Tag = "di.fm"
        Me.DIFM.Text = "Digitally Imported"
        '
        'JazzRadio
        '
        Me.JazzRadio.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.JazzRadio.Image = CType(resources.GetObject("JazzRadio.Image"), System.Drawing.Image)
        Me.JazzRadio.Name = "JazzRadio"
        Me.JazzRadio.Size = New System.Drawing.Size(169, 22)
        Me.JazzRadio.Tag = "jazzradio.com"
        Me.JazzRadio.Text = "JazzRadio"
        '
        'RockRadio
        '
        Me.RockRadio.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RockRadio.Image = CType(resources.GetObject("RockRadio.Image"), System.Drawing.Image)
        Me.RockRadio.Name = "RockRadio"
        Me.RockRadio.Size = New System.Drawing.Size(169, 22)
        Me.RockRadio.Tag = "rockradio.com"
        Me.RockRadio.Text = "RockRadio"
        '
        'SKYFM
        '
        Me.SKYFM.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SKYFM.Image = CType(resources.GetObject("SKYFM.Image"), System.Drawing.Image)
        Me.SKYFM.Name = "SKYFM"
        Me.SKYFM.Size = New System.Drawing.Size(169, 22)
        Me.SKYFM.Tag = "sky.fm"
        Me.SKYFM.Text = "SKY.FM"
        '
        'EditFavorites
        '
        Me.EditFavorites.Location = New System.Drawing.Point(188, 46)
        Me.EditFavorites.Name = "EditFavorites"
        Me.EditFavorites.Size = New System.Drawing.Size(43, 11)
        Me.EditFavorites.TabIndex = 12
        Me.EditFavorites.TabStop = True
        Me.EditFavorites.Text = "Edit list"
        Me.EditFavorites.Visible = False
        '
        'RefreshFavorites
        '
        Me.RefreshFavorites.Location = New System.Drawing.Point(268, 46)
        Me.RefreshFavorites.Name = "RefreshFavorites"
        Me.RefreshFavorites.Size = New System.Drawing.Size(62, 11)
        Me.RefreshFavorites.TabIndex = 6
        Me.RefreshFavorites.TabStop = True
        Me.RefreshFavorites.Text = "Refresh list"
        Me.RefreshFavorites.Visible = False
        '
        'GetUpdates
        '
        '
        'FadeOut
        '
        Me.FadeOut.Interval = 1
        '
        'DownloadDb
        '
        '
        'VisualisationBox
        '
        Me.VisualisationBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.VisualisationBox.Location = New System.Drawing.Point(12, 12)
        Me.VisualisationBox.Name = "VisualisationBox"
        Me.VisualisationBox.Size = New System.Drawing.Size(331, 334)
        Me.VisualisationBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.VisualisationBox.TabIndex = 3
        Me.VisualisationBox.TabStop = False
        '
        'HistoryList
        '
        Me.HistoryList.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.Title, Me.Length})
        Me.HistoryList.FullRowSelect = True
        Me.HistoryList.GridLines = True
        Me.HistoryList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.HistoryList.HideSelection = False
        Me.HistoryList.Location = New System.Drawing.Point(12, 12)
        Me.HistoryList.MultiSelect = False
        Me.HistoryList.Name = "HistoryList"
        Me.HistoryList.ShowItemToolTips = True
        Me.HistoryList.Size = New System.Drawing.Size(331, 334)
        Me.HistoryList.TabIndex = 14
        Me.HistoryList.UseCompatibleStateImageBehavior = False
        Me.HistoryList.View = System.Windows.Forms.View.Details
        Me.HistoryList.Visible = False
        '
        'Title
        '
        Me.Title.Text = "Title"
        Me.Title.Width = 255
        '
        'Length
        '
        Me.Length.Text = "Length"
        Me.Length.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.Length.Width = 55
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.SystemColors.Control
        Me.Label2.Location = New System.Drawing.Point(2, 79)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(30, 2)
        Me.Label2.TabIndex = 14
        Me.Label2.Text = " "
        '
        'Player
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(356, 441)
        Me.Controls.Add(Me.HistoryList)
        Me.Controls.Add(Me.VisualisationBox)
        Me.Controls.Add(Me.ControlsPanel)
        Me.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(362, 470)
        Me.MinimumSize = New System.Drawing.Size(362, 125)
        Me.Name = "Player"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "DI Radio Player "
        Me.CopyTitleMenu.ResumeLayout(False)
        Me.TrayMenu.ResumeLayout(False)
        CType(Me.Volume, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ServerMenu.ResumeLayout(False)
        Me.ControlsPanel.ResumeLayout(False)
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        CType(Me.VisualisationBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents VisualisationBox As System.Windows.Forms.PictureBox
    Friend WithEvents ServersDownloader As System.ComponentModel.BackgroundWorker
    Friend WithEvents Bufer As System.ComponentModel.BackgroundWorker
    Friend WithEvents VisTimer As System.Windows.Forms.Timer
    Friend WithEvents TimePassed As System.Windows.Forms.Timer
    Friend WithEvents TrayIcon As System.Windows.Forms.NotifyIcon
    Friend WithEvents CopyTitleMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents CopyToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolTip As System.Windows.Forms.ToolTip
    Friend WithEvents PlayStop As System.Windows.Forms.Button
    Friend WithEvents OptionsButton As System.Windows.Forms.Button
    Friend WithEvents Mute As System.Windows.Forms.Button
    Friend WithEvents Volume As System.Windows.Forms.TrackBar
    Friend WithEvents TimerString As System.Windows.Forms.Label
    Friend WithEvents RadioString As System.Windows.Forms.Label
    Friend WithEvents Calendar As System.Windows.Forms.Button
    Friend WithEvents SelectedChannel As System.Windows.Forms.ComboBox
    Friend WithEvents History As System.Windows.Forms.Button
    Friend WithEvents Forums As System.Windows.Forms.Button
    Friend WithEvents SelectedServer As System.Windows.Forms.ComboBox
    Friend WithEvents Marquee As System.Windows.Forms.ProgressBar
    Friend WithEvents ControlsPanel As System.Windows.Forms.Panel
    Friend WithEvents TrayMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ExitTray As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OptionsTray As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ForumsTray As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CalendarTray As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TrackHistoryTray As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CopyTitleTray As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents MuteTray As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PlayStopTray As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents RefreshFavorites As System.Windows.Forms.LinkLabel
    Friend WithEvents GetUpdates As System.ComponentModel.BackgroundWorker
    Private WithEvents FadeOut As System.Windows.Forms.Timer
    Friend WithEvents EditFavorites As System.Windows.Forms.LinkLabel
    Friend WithEvents GoogleSearchToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents GoogleSearchTray As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
    Friend WithEvents StationChooser As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents DIFM As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents JazzRadio As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SKYFM As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ServerMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents CopyServerURLToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RockRadio As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DownloadDb As System.ComponentModel.BackgroundWorker
    Friend WithEvents DownloadingMessage As System.Windows.Forms.ComboBox
    Friend WithEvents HistoryList As System.Windows.Forms.ListView
    Friend WithEvents Title As System.Windows.Forms.ColumnHeader
    Friend WithEvents Length As System.Windows.Forms.ColumnHeader
    Friend WithEvents Label2 As System.Windows.Forms.Label

End Class
