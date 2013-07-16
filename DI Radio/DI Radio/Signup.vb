Public Class Signup

    Delegate Sub MsgBoxSafe(ByVal text As String, ByVal style As MsgBoxStyle, ByVal title As String)
    Dim error422 As Boolean = False
    Dim returnedData As String
    Dim success As Boolean

#Region "Events caused by the controls on the main form"

    Private Sub termsOfService_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles termsOfService.LinkClicked
        Process.Start("http://www." & Player.StationChooser.Tag & "/member/tos")
    End Sub

    Private Sub signUpButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles signUpButton.Click

        If String.IsNullOrEmpty(firstName.Text) Then
            MessageBox.Show("Please write your first name.", "Account creation error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            firstName.Focus()
        ElseIf String.IsNullOrEmpty(lastName.Text) Then
            MessageBox.Show("Please write your last name.", "Account creation error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            lastName.Focus()
        ElseIf CheckEmail(eMail.Text) = False Then
            MessageBox.Show("The Email is invalid.", "Account creation error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            eMail.Focus()
        ElseIf passWord.Text.Length < 6 Then
            MessageBox.Show("Your password must have at least 6 characters.", "Account creation error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            passWord.Focus()
        ElseIf passWord.Text = retypePassword.Text = False Then
            MessageBox.Show("The passwords don't match.", "Account creation error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            retypePassword.Focus()
        Else
            firstName.ReadOnly = True
            lastName.ReadOnly = True
            eMail.ReadOnly = True
            passWord.ReadOnly = True
            retypePassword.ReadOnly = True
            signUpButton.Enabled = False
            pleaseWait.Show()
            createAccountWorker.RunWorkerAsync()
        End If

    End Sub

    Private Sub firstName_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles firstName.KeyDown
        If e.KeyCode = Keys.Enter And signUpButton.Enabled = True Then
            signUpButton_Click(Me, Nothing)
        ElseIf e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    Private Sub lastName_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles lastName.KeyDown
        If e.KeyCode = Keys.Enter And signUpButton.Enabled = True Then
            signUpButton_Click(Me, Nothing)
        ElseIf e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    Private Sub eMail_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles eMail.KeyDown
        If e.KeyCode = Keys.Enter And signUpButton.Enabled = True Then
            signUpButton_Click(Me, Nothing)
        ElseIf e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    Private Sub passWord_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles passWord.KeyDown
        If e.KeyCode = Keys.Enter And signUpButton.Enabled = True Then
            signUpButton_Click(Me, Nothing)
        ElseIf e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    Private Sub retypePassword_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles retypePassword.KeyDown
        If e.KeyCode = Keys.Enter And signUpButton.Enabled = True Then
            signUpButton_Click(Me, Nothing)
        ElseIf e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    Private Sub signUpButton_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles signUpButton.KeyDown
        If e.KeyCode = Keys.Enter And signUpButton.Enabled = True Then
            signUpButton_Click(Me, Nothing)
        ElseIf e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

#End Region

#Region "Background Worker"

    Private Sub createAccountWorker_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles createAccountWorker.DoWork
        Dim sendstring As String = "member[email]=" & eMail.Text & "&member[first_name]=" & firstName.Text & "&member[last_name]=" & lastName.Text & "&member[password]=" & passWord.Text.Replace("%", "%25").Replace("&", "%26") & "&member[password_confirmation]=" & retypePassword.Text.Replace("%", "%25").Replace("&", "%26")
        Dim WebClient As Net.WebClient = New Net.WebClient()
        WebClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded")
        WebClient.Credentials = New Net.NetworkCredential("ephemeron", "dayeiph0ne@pp")
        Dim request As Byte() = System.Text.Encoding.ASCII.GetBytes(sendstring)
        Dim returnedData As Byte()

        Try

            returnedData = WebClient.UploadData("http://api.audioaddict.com/v1/di/members", "POST", request)
            success = True

        Catch ex As Exception
            Dim Message As New MsgBoxSafe(AddressOf DisplayMessage)

            If ex.Message.Contains("422") Then
                Me.Invoke(Message, "That Email address is already in use.", MsgBoxStyle.Exclamation, "Account creation failed")
                error422 = True
                success = False
            Else
                Me.Invoke(Message, "Couldn't create your account." & vbNewLine & ex.Message & vbNewLine & vbNewLine & "Please try again.", MsgBoxStyle.Exclamation, "Account creation failed")
                success = False
            End If

        End Try

    End Sub

    Private Sub createAccountWorker_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles createAccountWorker.RunWorkerCompleted

        If success Then

            MessageBox.Show("Account created.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            If Options.emailBox.ReadOnly = False Then
                Options.emailBox.Text = eMail.Text
                Options.passwordBox.Text = passWord.Text
                Options.logIn_Click(Me, Nothing)
            End If
            Me.Close()

        Else

            firstName.ReadOnly = False
            lastName.ReadOnly = False
            eMail.ReadOnly = False
            passWord.ReadOnly = False
            retypePassword.ReadOnly = False
            signUpButton.Enabled = True
            pleaseWait.Hide()

            If error422 Then
                eMail.SelectAll()
                eMail.Focus()
                error422 = False
            End If

        End If
    End Sub

#End Region

#Region "Other functions"

    Function CheckEmail(ByVal emailAddress As String) As Boolean

        Dim pattern As String = "^.+@[^\.].*\.[a-z]{2,}$"
        Dim emailAddressMatch As System.Text.RegularExpressions.Match = System.Text.RegularExpressions.Regex.Match(emailAddress, pattern)

        If emailAddressMatch.Success Then
            Return True
        Else
            Return False
        End If

    End Function

    Sub DisplayMessage(ByVal text As String, ByVal style As MsgBoxStyle, ByVal title As String)

        MsgBox(text, style, title)
        Me.BringToFront()

    End Sub

#End Region

End Class