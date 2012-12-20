' Class Audioaddict
' Author _Tobias
' Class for fetching data from the DI API

Public Class Audioaddict

    ' Function for getting a list of a property's values from a .pls playlist
    Public Shared Function GetFromPls(ByVal attr As String, ByVal plsraw As String) As ArrayList
        ' Reader for reading through all lines
        Dim reader As New IO.StringReader(plsraw)

        ' empty array list, server urls are gonna be in here
        Dim servers As New ArrayList()

        ' read through all lines
        Do While (reader.Peek > -1)
            Dim v = reader.ReadLine
            ' if the current line starts with file, add the value (server url) to the arraylist
            If v.ToLower.StartsWith(attr.ToLower) Then
                servers.Add(v.Split("=")(1))
            End If
        Loop

        ' return
        Return servers
    End Function

    ' Function for parsing the DI/audioaddict playlists
    Public Shared Function ParsePlaylistAudioaddict(ByVal rawpls As String) As ArrayList
        ' get the DI playlist streams
        Dim first = GetFromPls("file", rawpls)

        ' empty arraylist, servers gonna be in here
        Dim servers As New ArrayList()

        ' When one pls is added, you already got all urls needed
        Dim firstPlsAdded As Boolean = False

        ' for each through json playlist servers
        Dim v
        For Each v In first
            If v.Contains(".pls") And firstPlsAdded = False Then
                ' when the url has .pls in it and there aren't any .pls urls parsed yet, parse it

                servers.AddRange(GetFromPls("file", Download(v)))
                firstPlsAdded = True
            ElseIf Not v.Contains(".pls") Then
                ' when it's a normal url (pubX.di.fm/y)

                servers.Add(v)
            End If
        Next

        ' return the result
        Return servers

    End Function

    Public Shared Function ParsePlaylistFavorites(ByVal rawpls As String) As ArrayList
        Dim names = GetFromPls("title", rawpls)
        Dim out As New ArrayList()
        Dim v As String
        For Each v In names
            out.Add(Split(v, " - ")(1))
        Next
        Return out
    End Function

    ' Easy download function
    Public Shared Function Download(ByVal url As String)
        Dim wc As Net.WebClient = New Net.WebClient
        Dim out
        Try
            out = wc.DownloadString(url)
        Catch ex As Exception
            out = False
        End Try

        Return out
    End Function
End Class