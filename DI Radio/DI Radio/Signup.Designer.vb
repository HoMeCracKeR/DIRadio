<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Signup
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Signup))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.firstName = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.lastName = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.eMail = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.passWord = New System.Windows.Forms.TextBox()
        Me.retypePassword = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.signUpButton = New System.Windows.Forms.Button()
        Me.termsOfService = New System.Windows.Forms.LinkLabel()
        Me.createAccountWorker = New System.ComponentModel.BackgroundWorker()
        Me.pleaseWait = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(60, 14)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "First name:"
        '
        'firstName
        '
        Me.firstName.Location = New System.Drawing.Point(15, 26)
        Me.firstName.Name = "firstName"
        Me.firstName.Size = New System.Drawing.Size(146, 20)
        Me.firstName.TabIndex = 1
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 49)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(60, 14)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Last name:"
        '
        'lastName
        '
        Me.lastName.Location = New System.Drawing.Point(15, 66)
        Me.lastName.Name = "lastName"
        Me.lastName.Size = New System.Drawing.Size(146, 20)
        Me.lastName.TabIndex = 3
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 89)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(34, 14)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Email:"
        '
        'eMail
        '
        Me.eMail.Location = New System.Drawing.Point(15, 106)
        Me.eMail.Name = "eMail"
        Me.eMail.Size = New System.Drawing.Size(146, 20)
        Me.eMail.TabIndex = 5
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(12, 129)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(60, 14)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "Password:"
        '
        'passWord
        '
        Me.passWord.Location = New System.Drawing.Point(15, 146)
        Me.passWord.Name = "passWord"
        Me.passWord.PasswordChar = Global.Microsoft.VisualBasic.ChrW(9679)
        Me.passWord.Size = New System.Drawing.Size(146, 20)
        Me.passWord.TabIndex = 7
        '
        'retypePassword
        '
        Me.retypePassword.Location = New System.Drawing.Point(15, 186)
        Me.retypePassword.Name = "retypePassword"
        Me.retypePassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(9679)
        Me.retypePassword.Size = New System.Drawing.Size(146, 20)
        Me.retypePassword.TabIndex = 9
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(12, 169)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(97, 14)
        Me.Label5.TabIndex = 8
        Me.Label5.Text = "Retype password:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(12, 209)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(113, 14)
        Me.Label6.TabIndex = 11
        Me.Label6.Text = "All fields are required."
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(12, 223)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(149, 28)
        Me.Label7.TabIndex = 12
        Me.Label7.Text = "By clicking Sign up you agree" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "to the"
        '
        'signUpButton
        '
        Me.signUpButton.Location = New System.Drawing.Point(15, 254)
        Me.signUpButton.Name = "signUpButton"
        Me.signUpButton.Size = New System.Drawing.Size(75, 23)
        Me.signUpButton.TabIndex = 14
        Me.signUpButton.Text = "Sign up"
        Me.signUpButton.UseVisualStyleBackColor = True
        '
        'termsOfService
        '
        Me.termsOfService.ActiveLinkColor = System.Drawing.SystemColors.MenuHighlight
        Me.termsOfService.AutoSize = True
        Me.termsOfService.Location = New System.Drawing.Point(43, 237)
        Me.termsOfService.Name = "termsOfService"
        Me.termsOfService.Size = New System.Drawing.Size(92, 14)
        Me.termsOfService.TabIndex = 13
        Me.termsOfService.TabStop = True
        Me.termsOfService.Text = "Terms of Service."
        '
        'createAccountWorker
        '
        Me.createAccountWorker.WorkerSupportsCancellation = True
        '
        'pleaseWait
        '
        Me.pleaseWait.AutoSize = True
        Me.pleaseWait.Location = New System.Drawing.Point(96, 258)
        Me.pleaseWait.Name = "pleaseWait"
        Me.pleaseWait.Size = New System.Drawing.Size(72, 14)
        Me.pleaseWait.TabIndex = 15
        Me.pleaseWait.Text = "Please wait..."
        Me.pleaseWait.Visible = False
        '
        'Signup
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 14.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(179, 288)
        Me.Controls.Add(Me.pleaseWait)
        Me.Controls.Add(Me.termsOfService)
        Me.Controls.Add(Me.signUpButton)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.retypePassword)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.passWord)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.eMail)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.lastName)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.firstName)
        Me.Controls.Add(Me.Label1)
        Me.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(185, 317)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(185, 317)
        Me.Name = "Signup"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "Sign up"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents firstName As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lastName As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents eMail As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents passWord As System.Windows.Forms.TextBox
    Friend WithEvents retypePassword As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents signUpButton As System.Windows.Forms.Button
    Friend WithEvents termsOfService As System.Windows.Forms.LinkLabel
    Friend WithEvents createAccountWorker As System.ComponentModel.BackgroundWorker
    Friend WithEvents pleaseWait As System.Windows.Forms.Label
End Class
