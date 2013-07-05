<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Gallery
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Gallery))
        Me.ThemeImage = New System.Windows.Forms.PictureBox()
        Me.DownloadTheme = New System.Windows.Forms.Button()
        Me.GetThemes = New System.ComponentModel.BackgroundWorker()
        Me.NextTheme = New System.Windows.Forms.Button()
        Me.PreviousTheme = New System.Windows.Forms.Button()
        Me.ThemeInfo = New System.Windows.Forms.Label()
        Me.Marquee = New System.Windows.Forms.ProgressBar()
        Me.GetImage = New System.ComponentModel.BackgroundWorker()
        Me.GetTheme = New System.ComponentModel.BackgroundWorker()
        Me.RetryImage = New System.Windows.Forms.Button()
        CType(Me.ThemeImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ThemeImage
        '
        Me.ThemeImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.ThemeImage.ImageLocation = ""
        Me.ThemeImage.Location = New System.Drawing.Point(12, 26)
        Me.ThemeImage.Name = "ThemeImage"
        Me.ThemeImage.Size = New System.Drawing.Size(332, 382)
        Me.ThemeImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.ThemeImage.TabIndex = 3
        Me.ThemeImage.TabStop = False
        '
        'DownloadTheme
        '
        Me.DownloadTheme.Enabled = False
        Me.DownloadTheme.Location = New System.Drawing.Point(117, 414)
        Me.DownloadTheme.Name = "DownloadTheme"
        Me.DownloadTheme.Size = New System.Drawing.Size(123, 23)
        Me.DownloadTheme.TabIndex = 4
        Me.DownloadTheme.Text = "Download and apply"
        Me.DownloadTheme.UseVisualStyleBackColor = True
        '
        'GetThemes
        '
        '
        'NextTheme
        '
        Me.NextTheme.Enabled = False
        Me.NextTheme.Location = New System.Drawing.Point(269, 414)
        Me.NextTheme.Name = "NextTheme"
        Me.NextTheme.Size = New System.Drawing.Size(75, 23)
        Me.NextTheme.TabIndex = 6
        Me.NextTheme.Text = "Next >"
        Me.NextTheme.UseVisualStyleBackColor = True
        '
        'PreviousTheme
        '
        Me.PreviousTheme.Enabled = False
        Me.PreviousTheme.Location = New System.Drawing.Point(12, 414)
        Me.PreviousTheme.Name = "PreviousTheme"
        Me.PreviousTheme.Size = New System.Drawing.Size(75, 23)
        Me.PreviousTheme.TabIndex = 7
        Me.PreviousTheme.Text = "< Previous"
        Me.PreviousTheme.UseVisualStyleBackColor = True
        '
        'ThemeInfo
        '
        Me.ThemeInfo.Location = New System.Drawing.Point(12, 9)
        Me.ThemeInfo.Name = "ThemeInfo"
        Me.ThemeInfo.Size = New System.Drawing.Size(332, 13)
        Me.ThemeInfo.TabIndex = 8
        Me.ThemeInfo.Text = "Please wait, downloading themes info..."
        Me.ThemeInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Marquee
        '
        Me.Marquee.Location = New System.Drawing.Point(126, 200)
        Me.Marquee.Name = "Marquee"
        Me.Marquee.Size = New System.Drawing.Size(100, 23)
        Me.Marquee.Style = System.Windows.Forms.ProgressBarStyle.Marquee
        Me.Marquee.TabIndex = 9
        '
        'GetImage
        '
        '
        'GetTheme
        '
        '
        'RetryImage
        '
        Me.RetryImage.Location = New System.Drawing.Point(138, 200)
        Me.RetryImage.Name = "RetryImage"
        Me.RetryImage.Size = New System.Drawing.Size(75, 23)
        Me.RetryImage.TabIndex = 10
        Me.RetryImage.Text = "Retry"
        Me.RetryImage.UseVisualStyleBackColor = True
        Me.RetryImage.Visible = False
        '
        'Gallery
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 14.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(356, 449)
        Me.Controls.Add(Me.RetryImage)
        Me.Controls.Add(Me.Marquee)
        Me.Controls.Add(Me.ThemeInfo)
        Me.Controls.Add(Me.PreviousTheme)
        Me.Controls.Add(Me.NextTheme)
        Me.Controls.Add(Me.DownloadTheme)
        Me.Controls.Add(Me.ThemeImage)
        Me.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(362, 478)
        Me.MinimumSize = New System.Drawing.Size(362, 478)
        Me.Name = "Gallery"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "Themes viewer"
        CType(Me.ThemeImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ThemeImage As System.Windows.Forms.PictureBox
    Friend WithEvents DownloadTheme As System.Windows.Forms.Button
    Friend WithEvents GetThemes As System.ComponentModel.BackgroundWorker
    Friend WithEvents NextTheme As System.Windows.Forms.Button
    Friend WithEvents PreviousTheme As System.Windows.Forms.Button
    Friend WithEvents ThemeInfo As System.Windows.Forms.Label
    Friend WithEvents Marquee As System.Windows.Forms.ProgressBar
    Friend WithEvents GetImage As System.ComponentModel.BackgroundWorker
    Friend WithEvents GetTheme As System.ComponentModel.BackgroundWorker
    Friend WithEvents RetryImage As System.Windows.Forms.Button
End Class
