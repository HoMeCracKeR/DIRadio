<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Export
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Export))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ChannelsList = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.AvailableBox = New System.Windows.Forms.ListView()
        Me.AvailEvents = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.SaveFile = New System.Windows.Forms.Button()
        Me.SaveBox = New System.Windows.Forms.ListView()
        Me.EventsToSave = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Label3 = New System.Windows.Forms.Label()
        Me.OptionsBox = New System.Windows.Forms.GroupBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.ReminderMinutes = New System.Windows.Forms.NumericUpDown()
        Me.SetReminder = New System.Windows.Forms.CheckBox()
        Me.ScheduleAsBusy = New System.Windows.Forms.CheckBox()
        Me.IncludeLink = New System.Windows.Forms.CheckBox()
        Me.IncludeDescription = New System.Windows.Forms.CheckBox()
        Me.ToDown = New System.Windows.Forms.Button()
        Me.ToUp = New System.Windows.Forms.Button()
        Me.GetEvents = New System.ComponentModel.BackgroundWorker()
        Me.ExportICS = New System.Windows.Forms.SaveFileDialog()
        Me.ExportLabel = New System.Windows.Forms.Label()
        Me.Exporter = New System.ComponentModel.BackgroundWorker()
        Me.Retry = New System.Windows.Forms.Button()
        Me.OptionsBox.SuspendLayout()
        CType(Me.ReminderMinutes, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(49, 14)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Channel:"
        '
        'ChannelsList
        '
        Me.ChannelsList.DropDownHeight = 394
        Me.ChannelsList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ChannelsList.FormattingEnabled = True
        Me.ChannelsList.IntegralHeight = False
        Me.ChannelsList.Location = New System.Drawing.Point(12, 26)
        Me.ChannelsList.Name = "ChannelsList"
        Me.ChannelsList.Size = New System.Drawing.Size(160, 22)
        Me.ChannelsList.TabIndex = 1
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 51)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(90, 14)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Available events:"
        '
        'AvailableBox
        '
        Me.AvailableBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.AvailableBox.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.AvailEvents})
        Me.AvailableBox.FullRowSelect = True
        Me.AvailableBox.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.AvailableBox.HideSelection = False
        Me.AvailableBox.Location = New System.Drawing.Point(12, 68)
        Me.AvailableBox.Name = "AvailableBox"
        Me.AvailableBox.Size = New System.Drawing.Size(332, 103)
        Me.AvailableBox.TabIndex = 3
        Me.AvailableBox.UseCompatibleStateImageBehavior = False
        Me.AvailableBox.View = System.Windows.Forms.View.Details
        '
        'AvailEvents
        '
        Me.AvailEvents.Width = 315
        '
        'SaveFile
        '
        Me.SaveFile.Enabled = False
        Me.SaveFile.Location = New System.Drawing.Point(269, 419)
        Me.SaveFile.Name = "SaveFile"
        Me.SaveFile.Size = New System.Drawing.Size(75, 23)
        Me.SaveFile.TabIndex = 4
        Me.SaveFile.Text = "Save to file"
        Me.SaveFile.UseVisualStyleBackColor = True
        '
        'SaveBox
        '
        Me.SaveBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.SaveBox.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.EventsToSave})
        Me.SaveBox.FullRowSelect = True
        Me.SaveBox.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.SaveBox.HideSelection = False
        Me.SaveBox.Location = New System.Drawing.Point(12, 207)
        Me.SaveBox.Name = "SaveBox"
        Me.SaveBox.Size = New System.Drawing.Size(332, 101)
        Me.SaveBox.TabIndex = 5
        Me.SaveBox.UseCompatibleStateImageBehavior = False
        Me.SaveBox.View = System.Windows.Forms.View.Details
        '
        'EventsToSave
        '
        Me.EventsToSave.Width = 315
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(9, 190)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(82, 14)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "Events to save:"
        '
        'OptionsBox
        '
        Me.OptionsBox.Controls.Add(Me.Label4)
        Me.OptionsBox.Controls.Add(Me.ReminderMinutes)
        Me.OptionsBox.Controls.Add(Me.SetReminder)
        Me.OptionsBox.Controls.Add(Me.ScheduleAsBusy)
        Me.OptionsBox.Controls.Add(Me.IncludeLink)
        Me.OptionsBox.Controls.Add(Me.IncludeDescription)
        Me.OptionsBox.Location = New System.Drawing.Point(12, 308)
        Me.OptionsBox.Name = "OptionsBox"
        Me.OptionsBox.Size = New System.Drawing.Size(332, 105)
        Me.OptionsBox.TabIndex = 7
        Me.OptionsBox.TabStop = False
        Me.OptionsBox.Text = "Options"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(164, 81)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(158, 14)
        Me.Label4.TabIndex = 5
        Me.Label4.Text = "minutes before the event starts"
        '
        'ReminderMinutes
        '
        Me.ReminderMinutes.Location = New System.Drawing.Point(108, 78)
        Me.ReminderMinutes.Maximum = New Decimal(New Integer() {120, 0, 0, 0})
        Me.ReminderMinutes.Name = "ReminderMinutes"
        Me.ReminderMinutes.Size = New System.Drawing.Size(50, 20)
        Me.ReminderMinutes.TabIndex = 4
        Me.ReminderMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'SetReminder
        '
        Me.SetReminder.AutoSize = True
        Me.SetReminder.Location = New System.Drawing.Point(6, 80)
        Me.SetReminder.Name = "SetReminder"
        Me.SetReminder.Size = New System.Drawing.Size(96, 18)
        Me.SetReminder.TabIndex = 3
        Me.SetReminder.Text = "Set a reminder"
        Me.SetReminder.UseVisualStyleBackColor = True
        '
        'ScheduleAsBusy
        '
        Me.ScheduleAsBusy.Location = New System.Drawing.Point(6, 59)
        Me.ScheduleAsBusy.Name = "ScheduleAsBusy"
        Me.ScheduleAsBusy.Size = New System.Drawing.Size(316, 18)
        Me.ScheduleAsBusy.TabIndex = 2
        Me.ScheduleAsBusy.Text = "Schedule events as 'busy' time"
        Me.ScheduleAsBusy.UseVisualStyleBackColor = True
        '
        'IncludeLink
        '
        Me.IncludeLink.Location = New System.Drawing.Point(6, 39)
        Me.IncludeLink.Name = "IncludeLink"
        Me.IncludeLink.Size = New System.Drawing.Size(316, 18)
        Me.IncludeLink.TabIndex = 1
        Me.IncludeLink.Text = "Include a link to DI's calendar for each event"
        Me.IncludeLink.UseVisualStyleBackColor = True
        '
        'IncludeDescription
        '
        Me.IncludeDescription.Location = New System.Drawing.Point(6, 19)
        Me.IncludeDescription.Name = "IncludeDescription"
        Me.IncludeDescription.Size = New System.Drawing.Size(316, 18)
        Me.IncludeDescription.TabIndex = 0
        Me.IncludeDescription.Text = "Include events description"
        Me.IncludeDescription.UseVisualStyleBackColor = True
        '
        'ToDown
        '
        Me.ToDown.Image = CType(resources.GetObject("ToDown.Image"), System.Drawing.Image)
        Me.ToDown.Location = New System.Drawing.Point(188, 177)
        Me.ToDown.Name = "ToDown"
        Me.ToDown.Size = New System.Drawing.Size(75, 24)
        Me.ToDown.TabIndex = 8
        Me.ToDown.UseVisualStyleBackColor = True
        '
        'ToUp
        '
        Me.ToUp.Enabled = False
        Me.ToUp.Image = CType(resources.GetObject("ToUp.Image"), System.Drawing.Image)
        Me.ToUp.Location = New System.Drawing.Point(269, 177)
        Me.ToUp.Name = "ToUp"
        Me.ToUp.Size = New System.Drawing.Size(75, 24)
        Me.ToUp.TabIndex = 9
        Me.ToUp.UseVisualStyleBackColor = True
        '
        'GetEvents
        '
        '
        'ExportICS
        '
        Me.ExportICS.Filter = "iCalendar|*.ics"
        Me.ExportICS.ShowHelp = True
        Me.ExportICS.Title = "Export event to .ics file"
        '
        'ExportLabel
        '
        Me.ExportLabel.Location = New System.Drawing.Point(12, 423)
        Me.ExportLabel.Name = "ExportLabel"
        Me.ExportLabel.Size = New System.Drawing.Size(251, 13)
        Me.ExportLabel.TabIndex = 11
        Me.ExportLabel.Text = "Status: Idle"
        Me.ExportLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Exporter
        '
        Me.Exporter.WorkerSupportsCancellation = True
        '
        'Retry
        '
        Me.Retry.Location = New System.Drawing.Point(178, 25)
        Me.Retry.Name = "Retry"
        Me.Retry.Size = New System.Drawing.Size(75, 23)
        Me.Retry.TabIndex = 12
        Me.Retry.Text = "Retry"
        Me.Retry.UseVisualStyleBackColor = True
        Me.Retry.Visible = False
        '
        'Export
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 14.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(356, 449)
        Me.Controls.Add(Me.Retry)
        Me.Controls.Add(Me.ExportLabel)
        Me.Controls.Add(Me.ToUp)
        Me.Controls.Add(Me.ToDown)
        Me.Controls.Add(Me.OptionsBox)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.SaveBox)
        Me.Controls.Add(Me.SaveFile)
        Me.Controls.Add(Me.AvailableBox)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.ChannelsList)
        Me.Controls.Add(Me.Label1)
        Me.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(362, 478)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(362, 478)
        Me.Name = "Export"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "Export events"
        Me.OptionsBox.ResumeLayout(False)
        Me.OptionsBox.PerformLayout()
        CType(Me.ReminderMinutes, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ChannelsList As System.Windows.Forms.ComboBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents AvailableBox As System.Windows.Forms.ListView
    Friend WithEvents AvailEvents As System.Windows.Forms.ColumnHeader
    Friend WithEvents SaveFile As System.Windows.Forms.Button
    Friend WithEvents SaveBox As System.Windows.Forms.ListView
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents OptionsBox As System.Windows.Forms.GroupBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents ReminderMinutes As System.Windows.Forms.NumericUpDown
    Friend WithEvents SetReminder As System.Windows.Forms.CheckBox
    Friend WithEvents ScheduleAsBusy As System.Windows.Forms.CheckBox
    Friend WithEvents IncludeLink As System.Windows.Forms.CheckBox
    Friend WithEvents IncludeDescription As System.Windows.Forms.CheckBox
    Friend WithEvents ToDown As System.Windows.Forms.Button
    Friend WithEvents ToUp As System.Windows.Forms.Button
    Friend WithEvents EventsToSave As System.Windows.Forms.ColumnHeader
    Friend WithEvents GetEvents As System.ComponentModel.BackgroundWorker
    Friend WithEvents ExportICS As System.Windows.Forms.SaveFileDialog
    Friend WithEvents ExportLabel As System.Windows.Forms.Label
    Friend WithEvents Exporter As System.ComponentModel.BackgroundWorker
    Friend WithEvents Retry As System.Windows.Forms.Button
End Class
