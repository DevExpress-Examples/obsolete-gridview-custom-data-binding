using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data;
using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using System.Reflection;
using Example.Models;

public class DataObject : IListServer, ITypedList {
    DataClassesDataContext context = new DataClassesDataContext();

    readonly IQueryable<Contact> origQuery;

    IQueryable<Contact> query;
    String keyExpression;

    const Int32 pageSize = 23; // approximate page size

    // cache accessibility
    Boolean isCountDirty = true;
    Boolean isResultDirty = true;
    Boolean isKeysDirty = true;

    // cached values
    Int32 count = -1; // row count
    Array keys;  // array of keys
    Array rows;  // array of records
    Int32 startKeyIndex = -1; // relative key index
    Int32 startRowIndex = -1; // relative row index

    public DataObject() {
        this.origQuery = from contact in context.Contacts
                         select contact;

        this.query = this.origQuery;
        this.keyExpression = "ContactID";
    }

    void IListServer.Apply(DevExpress.Data.Filtering.CriteriaOperator filterCriteria, ICollection<ServerModeOrderDescriptor> sortInfo, int groupCount, ICollection<ServerModeSummaryDescriptor> groupSummaryInfo, ICollection<ServerModeSummaryDescriptor> totalSummaryInfo) {
        isCountDirty = true;
        isResultDirty = true;
        isKeysDirty = true;

        count = -1;
        startKeyIndex = -1;
        startRowIndex = -1;

        this.query = this.origQuery;

        if (sortInfo != null) {
            Boolean first = true;
            IOrderedQueryable<Contact> oq = null;

            foreach (ServerModeOrderDescriptor info in sortInfo) {
                String propertyName = (info.SortExpression as OperandProperty).PropertyName;

                if (first) {
                    if (info.IsDesc)
                        oq = LinqHelper.OrderByDescending<Contact>(this.query, propertyName);
                    else
                        oq = LinqHelper.OrderBy<Contact>(this.query, propertyName);

                    first = false;
                }
                else {
                    if (info.IsDesc)
                        oq = LinqHelper.ThenByDescending<Contact>(oq, propertyName);
                    else
                        oq = LinqHelper.ThenBy<Contact>(oq, propertyName);
                }
            }

            this.query = oq.AsQueryable<Contact>();
        }
    }

    event EventHandler<ServerModeExceptionThrownEventArgs> IListServer.ExceptionThrown {
        add { throw new NotImplementedException(); }
        remove { throw new NotImplementedException(); }
    }

    int IListServer.FindIncremental(DevExpress.Data.Filtering.CriteriaOperator expression, string value, int startIndex, bool searchUp, bool ignoreStartRow, bool allowLoop) {
        throw new NotImplementedException();
    }

    IList IListServer.GetAllFilteredAndSortedRows() {
        throw new NotImplementedException();
    }

    List<ListSourceGroupInfo> IListServer.GetGroupInfo(ListSourceGroupInfo parentGroup) {
        throw new NotImplementedException();
    }

    int IListServer.GetRowIndexByKey(object key) {
        throw new NotImplementedException();
    }

    object IListServer.GetRowKey(int index) {
        if (isKeysDirty ||
            (index < startKeyIndex) ||
            (index >= startKeyIndex + pageSize)) {
            isKeysDirty = false;
            startKeyIndex = index;

            var keysQuery = from obj in query.Skip(index).Take(pageSize)
                            select obj.GetType().GetProperty(this.keyExpression).GetValue(obj, null);
            var keysArray = keysQuery.ToArray();

            this.keys = Array.CreateInstance(keysQuery.ElementType, keysArray.Count());

            Array.Copy(keysArray, this.keys, keysArray.Count());
        }

        return this.keys.GetValue(index - startKeyIndex);
    }

    List<object> IListServer.GetTotalSummary() {
        throw new NotImplementedException();
    }

    object[] IListServer.GetUniqueColumnValues(DevExpress.Data.Filtering.CriteriaOperator expression, int maxCount, bool includeFilteredOut) {
        throw new NotImplementedException();
    }

    event EventHandler<ServerModeInconsistencyDetectedEventArgs> IListServer.InconsistencyDetected {
        add { throw new NotImplementedException(); }
        remove { throw new NotImplementedException(); }
    }

    int IListServer.LocateByValue(DevExpress.Data.Filtering.CriteriaOperator expression, object value, int startIndex, bool searchUp) {
        throw new NotImplementedException();
    }

    void IListServer.Refresh() {
        throw new NotImplementedException();
    }

    int IList.Add(object value) {
        throw new NotImplementedException();
    }

    void IList.Clear() {
        throw new NotImplementedException();
    }

    bool IList.Contains(object value) {
        throw new NotImplementedException();
    }

    int IList.IndexOf(object value) {
        throw new NotImplementedException();
    }

    void IList.Insert(int index, object value) {
        throw new NotImplementedException();
    }

    bool IList.IsFixedSize {
        get { throw new NotImplementedException(); }
    }

    bool IList.IsReadOnly {
        get { throw new NotImplementedException(); }
    }

    void IList.Remove(object value) {
        throw new NotImplementedException();
    }

    void IList.RemoveAt(int index) {
        throw new NotImplementedException();
    }

    object IList.this[int index] {
        get {
            if (isResultDirty ||
                (index < startRowIndex) ||
                (index >= startRowIndex + pageSize)) {
                isResultDirty = false;
                startRowIndex = index;

                var rowsQuery = from obj in query.Skip(index).Take(pageSize)
                                select obj;
                var rowsArray = rowsQuery.ToArray();

                this.rows = Array.CreateInstance(rowsQuery.ElementType, rowsArray.Count());

                Array.Copy(rowsArray, this.rows, rowsArray.Count());
            }

            return this.rows.GetValue(index - startRowIndex);
        }
        set {
            throw new NotImplementedException();
        }
    }

    void ICollection.CopyTo(Array array, int index) {
        throw new NotImplementedException();
    }

    int ICollection.Count {
        get {
            if (isCountDirty) {
                isCountDirty = false;
                count = query.Count();
            }

            return count;
        }
    }

    bool ICollection.IsSynchronized {
        get { throw new NotImplementedException(); }
    }

    object ICollection.SyncRoot {
        get { throw new NotImplementedException(); }
    }

    IEnumerator IEnumerable.GetEnumerator() {
        throw new NotImplementedException();
    }

    PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
        return TypeDescriptor.GetProperties(query.ElementType);
    }

    string ITypedList.GetListName(PropertyDescriptor[] listAccessors) {
        return query.ElementType.Name;
    }



}

/* http://stackoverflow.com/questions/41244/dynamic-linq-orderby */

public static class LinqHelper {
    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property) {
        return ApplyOrder<T>(source, property, "OrderBy");
    }

    public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string property) {
        return ApplyOrder<T>(source, property, "OrderByDescending");
    }

    public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string property) {
        return ApplyOrder<T>(source, property, "ThenBy");
    }

    public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string property) {
        return ApplyOrder<T>(source, property, "ThenByDescending");
    }

    static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName) {
        string[] props = property.Split('.');
        Type type = typeof(T);
        ParameterExpression arg = Expression.Parameter(type, "x");
        Expression expr = arg;
        foreach (string prop in props) {
            // use reflection (not ComponentModel) to mirror LINQ
            PropertyInfo pi = type.GetProperty(prop);
            expr = Expression.Property(expr, pi);
            type = pi.PropertyType;
        }
        Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
        LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

        object result = typeof(Queryable).GetMethods().Single(
                method => method.Name == methodName
                        && method.IsGenericMethodDefinition
                        && method.GetGenericArguments().Length == 2
                        && method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), type)
                .Invoke(null, new object[] { source, lambda });
        return (IOrderedQueryable<T>)result;
    }
}