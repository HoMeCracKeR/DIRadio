<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Changelog
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Changelog))
        Me.ChangelogText = New System.Windows.Forms.RichTextBox()
        Me.GetChangelog = New System.ComponentModel.BackgroundWorker()
        Me.SuspendLayout()
        '
        'ChangelogText
        '
        Me.ChangelogText.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.ChangelogText.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ChangelogText.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ChangelogText.Location = New System.Drawing.Point(0, 0)
        Me.ChangelogText.Name = "ChangelogText"
        Me.ChangelogText.ReadOnly = True
        Me.ChangelogText.Size = New System.Drawing.Size(294, 390)
        Me.ChangelogText.TabIndex = 0
        Me.ChangelogText.Text = "Please wait, downloading changelog..."
        '
        'GetChangelog
        '
        '
        'Changelog
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 14.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(294, 390)
        Me.Controls.Add(Me.ChangelogText)
        Me.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimumSize = New System.Drawing.Size(310, 305)
        Me.Name = "Changelog"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "Changelog"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ChangelogText As System.Windows.Forms.RichTextBox
    Friend WithEvents GetChangelog As System.ComponentModel.BackgroundWorker
End Class
