Imports CefSharp.WinForms

Public Class Form1
    Dim cef As ChromiumWebBrowser
    Dim xTor As Tor

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Panel1.Visible = False
        Panel2.Visible = False
        Panel3.Visible = False

        Dim s As New CefSettings

        xTor = New Tor("de")
        s.CefCommandLineArgs.Add("proxy-server", "socks5://127.0.0.1:1338")
        s.UserAgent = ""
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

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        CefSharp.Cef.GetGlobalCookieManager.VisitAllCookies(New CookieVisitor)

        For Each c In CookieVisitor.Cookies

        Next

        CookieVisitor.Cookies.Clear()
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If CefSharp.Cef.IsInitialized Then
            CefSharp.Cef.Shutdown()
        End If
    End Sub
End Class