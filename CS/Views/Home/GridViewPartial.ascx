<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<%
    Html.DevExpress().GridView(settings => {
        settings.Name = "grid";
        settings.CallbackRouteValues = new { Controller = "Home", Action = "GridViewPartial" };

        settings.Width = Unit.Percentage(100);
    })
    .Bind(Model)
    .Render();
 %>