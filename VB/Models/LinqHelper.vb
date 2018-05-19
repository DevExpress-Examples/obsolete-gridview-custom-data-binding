
Imports Microsoft.VisualBasic
Imports System.Linq.Expressions
Imports System.Reflection
Imports System.Linq
Imports System

' http://stackoverflow.com/questions/41244/dynamic-linq-orderby 
Module LinqHelper
    <System.Runtime.CompilerServices.Extension()> _
    Public Function OrderBy(Of T)(ByVal source As IQueryable(Of T), ByVal [property] As String) As IOrderedQueryable(Of T)
        Return ApplyOrder(Of T)(source, [property], "OrderBy")
    End Function

    <System.Runtime.CompilerServices.Extension()> _
    Public Function OrderByDescending(Of T)(ByVal source As IQueryable(Of T), ByVal [property] As String) As IOrderedQueryable(Of T)
        Return ApplyOrder(Of T)(source, [property], "OrderByDescending")
    End Function

    <System.Runtime.CompilerServices.Extension()> _
    Public Function ThenBy(Of T)(ByVal source As IOrderedQueryable(Of T), ByVal [property] As String) As IOrderedQueryable(Of T)
        Return ApplyOrder(Of T)(source, [property], "ThenBy")
    End Function

    <System.Runtime.CompilerServices.Extension()> _
    Public Function ThenByDescending(Of T)(ByVal source As IOrderedQueryable(Of T), ByVal [property] As String) As IOrderedQueryable(Of T)
        Return ApplyOrder(Of T)(source, [property], "ThenByDescending")
    End Function

    Private Function ApplyOrder(Of T)(ByVal source As IQueryable(Of T), ByVal [property] As String, ByVal methodName As String) As IOrderedQueryable(Of T)
        Dim props() As String = [property].Split("."c)
        Dim type As System.Type = GetType(T)
        Dim arg As ParameterExpression = Expression.Parameter(type, "x")
        Dim expr As Expression = arg
        For Each prop As String In props
            ' use reflection (not ComponentModel) to mirror LINQ
            Dim pi As PropertyInfo = type.GetProperty(prop)
            expr = Expression.Property(expr, pi)
            type = pi.PropertyType
        Next prop

        Dim delegateType As System.Type = GetType(Func(Of ,)).MakeGenericType(GetType(T), type)
        Dim lambda As LambdaExpression = Expression.Lambda(delegateType, expr, arg)

        Dim result As Object = GetType(Queryable).GetMethods().Single(Function(method) method.Name = methodName AndAlso method.IsGenericMethodDefinition AndAlso method.GetGenericArguments().Length = 2 AndAlso method.GetParameters().Length = 2).MakeGenericMethod(GetType(T), type).Invoke(Nothing, New Object() {source, lambda})
        Return CType(result, IOrderedQueryable(Of T))
    End Function
End Module
