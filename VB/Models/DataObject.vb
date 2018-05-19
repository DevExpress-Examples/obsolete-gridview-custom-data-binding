Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports DevExpress.Data
Imports System.Collections
Imports System.ComponentModel
Imports System.Linq.Expressions
Imports DevExpress.Data.Filtering
Imports System.Reflection

Public Class DataObject
    Implements IListServer
    Implements ITypedList
    Private context As New DataClassesDataContext()

    ReadOnly origQuery As IQueryable(Of Contact)

    Private query As IQueryable(Of Contact)
    Private keyExpression As [String]

    Const pageSize As Int32 = 23
    ' approximate page size
    ' cache accessibility
    Private isCountDirty As [Boolean] = True
    Private isResultDirty As [Boolean] = True
    Private isKeysDirty As [Boolean] = True

    ' cached values
    Private count As Int32 = -1
    ' row count
    Private keys As Array
    ' array of keys
    Private rows As Array
    ' array of records
    Private startKeyIndex As Int32 = -1
    ' relative key index
    Private startRowIndex As Int32 = -1
    ' relative row index
    Public Sub New()
        Me.origQuery = From contact In context.Contacts Select contact

        Me.query = Me.origQuery
        Me.keyExpression = "ContactID"
    End Sub

    Private Sub IListServer_Apply(ByVal filterCriteria As DevExpress.Data.Filtering.CriteriaOperator, ByVal sortInfo As ICollection(Of ServerModeOrderDescriptor), ByVal groupCount As Integer, ByVal groupSummaryInfo As ICollection(Of ServerModeSummaryDescriptor), ByVal totalSummaryInfo As ICollection(Of ServerModeSummaryDescriptor)) Implements IListServer.Apply
        isCountDirty = True
        isResultDirty = True
        isKeysDirty = True

        count = -1
        startKeyIndex = -1
        startRowIndex = -1

        Me.query = Me.origQuery

        If sortInfo IsNot Nothing Then
            Dim first As [Boolean] = True
            Dim oq As IOrderedQueryable(Of Contact) = Nothing

            For Each info As ServerModeOrderDescriptor In sortInfo
                Dim propertyName As [String] = TryCast(info.SortExpression, OperandProperty).PropertyName

                If first Then
                    If info.IsDesc Then
                        oq = LinqHelper.OrderByDescending(Of Contact)(Me.query, propertyName)
                    Else
                        oq = LinqHelper.OrderBy(Of Contact)(Me.query, propertyName)
                    End If

                    first = False
                Else
                    If info.IsDesc Then
                        oq = LinqHelper.ThenByDescending(Of Contact)(oq, propertyName)
                    Else
                        oq = LinqHelper.ThenBy(Of Contact)(oq, propertyName)
                    End If
                End If
            Next

            Me.query = oq.AsQueryable()
        End If
    End Sub

    Private Custom Event ExceptionThrown As EventHandler(Of ServerModeExceptionThrownEventArgs) Implements IListServer.ExceptionThrown
        AddHandler(ByVal value As EventHandler(Of ServerModeExceptionThrownEventArgs))
            Throw New NotImplementedException()
        End AddHandler
        RemoveHandler(ByVal value As EventHandler(Of ServerModeExceptionThrownEventArgs))
            Throw New NotImplementedException()
        End RemoveHandler
        RaiseEvent(ByVal sender As System.Object, ByVal e As ServerModeExceptionThrownEventArgs)
        End RaiseEvent
    End Event

    Private Function IListServer_FindIncremental(ByVal expression As DevExpress.Data.Filtering.CriteriaOperator, ByVal value As String, ByVal startIndex As Integer, ByVal searchUp As Boolean, ByVal ignoreStartRow As Boolean, ByVal allowLoop As Boolean) As Integer Implements IListServer.FindIncremental
        Throw New NotImplementedException()
    End Function

    Private Function IListServer_GetAllFilteredAndSortedRows() As IList Implements IListServer.GetAllFilteredAndSortedRows
        Throw New NotImplementedException()
    End Function

    Private Function IListServer_GetGroupInfo(ByVal parentGroup As ListSourceGroupInfo) As List(Of ListSourceGroupInfo) Implements IListServer.GetGroupInfo
        Throw New NotImplementedException()
    End Function

    Private Function IListServer_GetRowIndexByKey(ByVal key As Object) As Integer Implements IListServer.GetRowIndexByKey
        Throw New NotImplementedException()
    End Function

    Private Function IListServer_GetRowKey(ByVal index As Integer) As Object Implements IListServer.GetRowKey
        If isKeysDirty OrElse (index < startKeyIndex) OrElse (index >= startKeyIndex + pageSize) Then
            isKeysDirty = False
            startKeyIndex = index

            Dim keysQuery = From obj In query.Skip(index).Take(pageSize) _
                           Select obj.GetType().GetProperty(Me.keyExpression).GetValue(obj, Nothing)
            Dim keysArray = keysQuery.ToArray()

            Me.keys = Array.CreateInstance(keysQuery.ElementType, keysArray.Count())

            Array.Copy(keysArray, Me.keys, keysArray.Count())
        End If

        Return Me.keys.GetValue(index - startKeyIndex)
    End Function

    Private Function IListServer_GetTotalSummary() As Dictionary(Of Object, Object) Implements IListServer.GetTotalSummary
        Throw New NotImplementedException()
    End Function

    Private Function IListServer_GetUniqueColumnValues(ByVal expression As DevExpress.Data.Filtering.CriteriaOperator, ByVal maxCount As Integer, ByVal includeFilteredOut As Boolean) As Object() Implements IListServer.GetUniqueColumnValues
        Throw New NotImplementedException()
    End Function

    Private Custom Event InconsistencyDetected As EventHandler(Of ServerModeInconsistencyDetectedEventArgs) Implements IListServer.InconsistencyDetected
        AddHandler(ByVal value As EventHandler(Of ServerModeInconsistencyDetectedEventArgs))
            Throw New NotImplementedException()
        End AddHandler
        RemoveHandler(ByVal value As EventHandler(Of ServerModeInconsistencyDetectedEventArgs))
            Throw New NotImplementedException()
        End RemoveHandler
        RaiseEvent(ByVal sender As System.Object, ByVal e As ServerModeInconsistencyDetectedEventArgs)
        End RaiseEvent
    End Event

    Private Function IListServer_LocateByValue(ByVal expression As DevExpress.Data.Filtering.CriteriaOperator, ByVal value As Object, ByVal startIndex As Integer, ByVal searchUp As Boolean) As Integer Implements IListServer.LocateByValue
        Throw New NotImplementedException()
    End Function

    Private Sub IListServer_Refresh() Implements IListServer.Refresh
        Throw New NotImplementedException()
    End Sub

    Private Function IList_Add(ByVal value As Object) As Integer Implements IList.Add
        Throw New NotImplementedException()
    End Function

    Private Sub IList_Clear() Implements IList.Clear
        Throw New NotImplementedException()
    End Sub

    Private Function IList_Contains(ByVal value As Object) As Boolean Implements IList.Contains
        Throw New NotImplementedException()
    End Function

    Private Function IList_IndexOf(ByVal value As Object) As Integer Implements IList.IndexOf
        Throw New NotImplementedException()
    End Function

    Private Sub IList_Insert(ByVal index As Integer, ByVal value As Object) Implements IList.Insert
        Throw New NotImplementedException()
    End Sub

    Private ReadOnly Property IList_IsFixedSize() As Boolean Implements IList.IsFixedSize
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    Private ReadOnly Property IList_IsReadOnly() As Boolean Implements IList.IsReadOnly
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    Private Sub IList_Remove(ByVal value As Object) Implements IList.Remove
        Throw New NotImplementedException()
    End Sub

    Private Sub IList_RemoveAt(ByVal index As Integer) Implements IList.RemoveAt
        Throw New NotImplementedException()
    End Sub

    Public Property IList_Item(ByVal index As Integer) As Object Implements IList.Item
        Get
            If isResultDirty OrElse (index < startRowIndex) OrElse (index >= startRowIndex + pageSize) Then
                isResultDirty = False
                startRowIndex = index

                Dim rowsQuery = From obj In query.Skip(index).Take(pageSize) _
                                 Select obj
                Dim rowsArray = rowsQuery.ToArray()

                Me.rows = Array.CreateInstance(rowsQuery.ElementType, rowsArray.Count())

                Array.Copy(rowsArray, Me.rows, rowsArray.Count())
            End If

            Return Me.rows.GetValue(index - startRowIndex)
        End Get
        Set(ByVal value As Object)
            Throw New NotImplementedException()
        End Set
    End Property

    Private Sub ICollection_CopyTo(ByVal array As Array, ByVal index As Integer) Implements ICollection.CopyTo
        Throw New NotImplementedException()
    End Sub

    Private ReadOnly Property ICollection_Count() As Integer Implements ICollection.Count
        Get
            If isCountDirty Then
                isCountDirty = False
                count = query.Count()
            End If

            Return count
        End Get
    End Property

    Private ReadOnly Property ICollection_IsSynchronized() As Boolean Implements ICollection.IsSynchronized
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    Private ReadOnly Property ICollection_SyncRoot() As Object Implements ICollection.SyncRoot
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Throw New NotImplementedException()
    End Function

    Private Function ITypedList_GetItemProperties(ByVal listAccessors As PropertyDescriptor()) As PropertyDescriptorCollection Implements ITypedList.GetItemProperties
        Return TypeDescriptor.GetProperties(query.ElementType)
    End Function

    Private Function ITypedList_GetListName(ByVal listAccessors As PropertyDescriptor()) As String Implements ITypedList.GetListName
        Return query.ElementType.Name
    End Function



End Class