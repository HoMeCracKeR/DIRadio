Namespace My

    ' Los siguientes eventos están disponibles para MyApplication:
    ' 
    ' Inicio: se desencadena cuando se inicia la aplicación, antes de que se cree el formulario de inicio.
    ' Apagado: generado después de cerrar todos los formularios de la aplicación. Este evento no se genera si la aplicación termina de forma anómala.
    ' UnhandledException: generado si la aplicación detecta una excepción no controlada.
    ' StartupNextInstance: se desencadena cuando se inicia una aplicación de instancia única y la aplicación ya está activa. 
    ' NetworkAvailabilityChanged: se desencadena cuando la conexión de red está conectada o desconectada.
    Partial Friend Class MyApplication

        Private Sub MyApplication_NetworkAvailabilityChanged(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.Devices.NetworkAvailableEventArgs) Handles Me.NetworkAvailabilityChanged
            If e.IsNetworkAvailable = False And Player.PlayStop.Tag = "Stop" Then
                Player.WasPlaying = Player.SelectedChannel.Text

                If Player.PlayStop.Enabled = True Then
                    Player.PlayStop_Click(Me, Nothing)
                    Player.RadioString.Text = "Internet connection lost."
                    Player.RadioString.ForeColor = Color.White
                    Player.RadioString.BackColor = Color.Red
                End If

            ElseIf e.IsNetworkAvailable = True And Player.PlayStop.Tag = "Play" And Player.WasPlaying = Player.SelectedChannel.Text And Player.PlayStop.Enabled = True Then
                Player.PlayStop_Click(Me, Nothing)
            End If
        End Sub

        Private Sub MyApplication_Startup(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs) Handles Me.Startup
            ' You need to have a registered key so BASS.NET doesn't display its popup at start.
            ' You can register at http://bass.radio42.com/bass_register.html

            'Un4seen.Bass.BassNet.Registration("example@example.com", "RegKey here")

            If Computer.Network.IsAvailable = False Then
                MsgBox("This application requires an internet connection to run.", MsgBoxStyle.Critical, "Network connection unavailable")
                Player.Close()
            End If
        End Sub

        Private Sub MyApplication_StartupNextInstance(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupNextInstanceEventArgs) Handles Me.StartupNextInstance
            If Player.WindowState = FormWindowState.Minimized And Player.NotificationIcon = True Then
                Player.TrayIcon_MouseDoubleClick(Me, Nothing)
            Else
                Player.WindowState = FormWindowState.Normal
                Player.BringToFront()
            End If

        End Sub

        Private Sub MyApplication_UnhandledException(sender As Object, e As Microsoft.VisualBasic.ApplicationServices.UnhandledExceptionEventArgs) Handles Me.UnhandledException
            If e.Exception.Message.ToLower.Contains("un4seen.bass.bass") Then
                MsgBox("There was an error with the BASS library. Please make sure that the following DLL files are located in the same folder as the main executable:" & vbNewLine & vbNewLine & "Bass.Net.dll" & vbNewLine & "bass.dll" & vbNewLine & "bassaac.dll" & vbNewLine & "basswma.dll", MsgBoxStyle.Critical)
            ElseIf e.Exception.Message.ToLower.Contains("bass.net") Then
                MsgBox("There was an error loading the BASS.NET library. Please make sure that the Bass.Net.dll file is located in the same folder as the main executable, and that it is version number 2.4.9.1." & vbNewLine & vbNewLine & "You can do this by right-clicking the DLL file, clicking Properties, and finally the Details tab.", MsgBoxStyle.Critical)
            Else
                MsgBox("An unhandled exception has occured in the application." & vbNewLine & "Please contact the developer with the following information, including what you were doing when this exception happened." & vbNewLine & vbNewLine & e.Exception.InnerException.Message, MsgBoxStyle.Critical)
            End If
        End Sub

    End Class


End Namespace

