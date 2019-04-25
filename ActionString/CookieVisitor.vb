Imports CefSharp

Class CookieVisitor
    Implements ICookieVisitor

    Public Shared Cookies As New List(Of Cookie)

    Public Sub Dispose() Implements IDisposable.Dispose

    End Sub

    Public Function Visit(cookie As Cookie, count As Integer, total As Integer, ByRef deleteCookie As Boolean) As Boolean Implements ICookieVisitor.Visit
        Cookies.Add(cookie)

        Return True
    End Function
End Class