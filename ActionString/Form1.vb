Imports CefSharp.WinForms

Public Class Form1
    Dim cef As ChromiumWebBrowser
    Dim xTor As Tor

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Panel1.Visible = False
        Panel2.Visible = False

        Dim s As New CefSettings

        xTor = New Tor
        s.CefCommandLineArgs.Add("proxy-server", "socks5://127.0.0.1:1338")
        CefSharp.Cef.Initialize(s)
        cef = New ChromiumWebBrowser("https://google.com")
        Controls.Add(cef)

        Dim th0 As New Threading.Thread(AddressOf Active)

        th0.IsBackground = True
        th0.Start()
    End Sub

    Private Sub Active()
        Do Until cef.IsBrowserInitialized
            Threading.Thread.Sleep(100)
        Loop

        Dim rnd As New Random(TimeOfDay.Ticks / 10000)
    End Sub
End Class