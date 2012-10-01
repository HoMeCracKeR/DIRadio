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
            If Computer.Network.IsAvailable = False Then
                MsgBox("This application requires an internet connection to run.", MsgBoxStyle.Critical, "Network connection unavailable")
                Form1.TrayIcon.Visible = False
                End
            End If
        End Sub

        Private Sub MyApplication_Startup(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs) Handles Me.Startup
            ' You need to have a registered key so BASS.NET doesn't display its popup at start.
            ' You can register at http://bass.radio42.com/bass_register.html

            'Un4seen.Bass.BassNet.Registration("example@example.com", "RegKey here")

            If Computer.Network.IsAvailable = False Then
                MsgBox("This application requires an internet connection to run.", MsgBoxStyle.Critical, "Network connection unavailable")
                End
            End If
        End Sub

        Private Sub MyApplication_StartupNextInstance(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupNextInstanceEventArgs) Handles Me.StartupNextInstance
            If Form1.WindowState = FormWindowState.Minimized And Form1.NotificationIcon = True Then
                Form1.TrayIcon_MouseDoubleClick(Me, Nothing)
            Else
                Form1.WindowState = FormWindowState.Normal
                Form1.BringToFront()
            End If

        End Sub

    End Class


End Namespace

