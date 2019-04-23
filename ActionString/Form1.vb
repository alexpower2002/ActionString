Imports CefSharp.WinForms

Public Class Form1
    Dim cef As ChromiumWebBrowser
    Dim xTor As Tor

    Private Sub Form1_Load_1(sender As Object, e As EventArgs) Handles Button2.Click
        Dim s As New CefSettings

        Tor0 = New Tor
        s.CefCommandLineArgs.Add("proxy-server", "socks5://127.0.0.1:1338")
        CefSharp.Cef.Initialize(s)
        cef = New WinForms.ChromiumWebBrowser("https://rucaptcha.com/auth/register")
        Controls.Add(cef)

        Dim th0 As New Threading.Thread(AddressOf Active)

        th0.IsBackground = True
        th0.Start()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Panel1.Visible = False
        Panel2.Visible = False


    End Sub
End Class