Imports Example

Public Class HomeController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult
        ViewData("Message") = "Welcome to DevExpress Extensions for ASP.NET MVC!"

        Dim obj As New DataObject()

        Return View(obj)
    End Function

    Public Function GridViewPartial() As ActionResult
        Dim obj As New DataObject()

        Return PartialView(obj)
    End Function
End Class
