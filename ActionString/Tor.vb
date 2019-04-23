Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports Knapcode.TorSharp

Public Class Tor
    Dim proxy As TorSharpProxy
    Dim settings As TorSharpSettings
    Dim client As HttpClient
    Dim cookies As New CookieContainer

    Public IP As String = ""

    Public Shared Addresses As New List(Of String)

    Sub New()
        Dim success = False

        Do Until success
            Try
                TryNew()

                success = True
            Catch ex As Exception

            End Try
        Loop
    End Sub

    Private Sub TryNew()
        Dim text = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory, "TorExtracted\privoxy-3.0.26\config.txt"))
        Dim lines = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory, "TorExtracted\tor-win32-0.3.2.10\Data\Tor\torrc"))

        lines(0) = "ControlPort " & 1339
        lines(1) = "DataDirectory " & Path.Combine(Directory.GetCurrentDirectory, "TorExtracted\tor-win32-0.3.2.10\Data\Tor")
        lines(3) = "SocksPort " & 1338
        lines(4) = ""
        lines(5) = ""

        File.WriteAllLines(Path.Combine(Directory.GetCurrentDirectory, "TorExtracted\tor-win32-0.3.2.10\Data\Tor\torrc"), lines)

        settings = New TorSharpSettings With {.ZippedToolsDirectory = Path.Combine(Directory.GetCurrentDirectory, "TorZipped"), .ExtractedToolsDirectory = Path.Combine(Directory.GetCurrentDirectory, "TorExtracted"), .PrivoxyPort = 1337, .TorSocksPort = 1338, .TorControlPort = 1339, .TorControlPassword = "foobar", .TorExitNodes = "{de}", .TorStrictNodes = True}
        proxy = New TorSharpProxy(settings)

        Dim handler = New HttpClientHandler With {.Proxy = New WebProxy(New Uri("http://localhost:" & settings.PrivoxyPort))}

        client = New HttpClient(handler)

        proxy.ConfigureAndStartAsync().Wait()
    End Sub

    Sub AddToken(token As String)
        client.DefaultRequestHeaders.Add("x-access-token", token)
    End Sub

    Sub RemoveToken()
        client.DefaultRequestHeaders.Remove("x-access-token")
    End Sub

    Async Function NewIdentity(c As List(Of String)) As Task(Of String)
        Dim success = False

        cookies.Add(New Cookie("PHPSESSID", c(0)) With {.Domain = "rucaptcha.com"})
        cookies.Add(New Cookie("counter_user_id", c(1)) With {.Domain = "rucaptcha.com"})
        cookies.Add(New Cookie("client", c(2)) With {.Domain = "rucaptcha.com"})

        Dim handler = New HttpClientHandler With {.Proxy = New WebProxy(New Uri("http://localhost:" & settings.PrivoxyPort))}

        handler.CookieContainer = cookies
        client = New HttpClient(handler)
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:50.0) Gecko/20100101 Firefox/50.0")
        client.DefaultRequestHeaders.Remove("x-access-token")

        Dim result = ""

        Await proxy.GetNewIdentityAsync()

        Threading.Thread.Sleep(5000)

        Do Until success
            IP = Await client.GetStringAsync("http://api.ipify.org")

            If Addresses.Contains(IP) Then
                Await proxy.GetNewIdentityAsync

                handler = New HttpClientHandler With {.Proxy = New WebProxy(New Uri("http://localhost:" & settings.PrivoxyPort))}
                handler.CookieContainer = cookies
                client = New HttpClient(handler)
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:50.0) Gecko/20100101 Firefox/50.0")
                Threading.Thread.Sleep(5000)
            Else
                Addresses.Add(IP)
                success = True
            End If
        Loop

        result = IP

        Return result
    End Function

    Private Shared Sub DirectoryCopy(SourcePath As String, DestinationPath As String)
        For Each dirPath As String In Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories)
            Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath))
        Next

        For Each newPath As String In Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories)
            File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), True)
        Next
    End Sub

    Sub FullStop()
        proxy.Stop()
    End Sub

    Async Function SendRequest(URI As String, Request As HttpContent) As Task(Of String)
        Return Await client.PostAsync(URI, Request).Result.Content.ReadAsStringAsync
    End Function

    Async Function SendRequest(URI As String) As Task(Of String)
        Return Await client.GetStringAsync(URI)
    End Function
End Class