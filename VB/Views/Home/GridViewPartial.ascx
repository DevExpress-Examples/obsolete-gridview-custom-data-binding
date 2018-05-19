<%@ Control Language="VB" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    Html.DevExpress().GridView(
        Function(settings)
            settings.Name = "grid"
            settings.CallbackRouteValues = New With {.Controller = "Home", .Action = "GridViewPartial"}
            settings.Width = Unit.Percentage(100)
        End Function
   ).Bind(Model).Render()
%>