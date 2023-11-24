using System;
using UnityEngine;

public class Query<TReturn>
{
    public readonly string queryName;
    public Func<TReturn> QueryHandler = default;

    private readonly TReturn _notHandledDefault;

    public Query(string queryName, TReturn notHandledDefault = default)
    {
        this.queryName = queryName;
        this._notHandledDefault = notHandledDefault;
    }

    public TReturn RaiseQuery()
    {
        if (QueryHandler != null)
        {
            return QueryHandler.Invoke();
        }
        else
        {
            Debug.LogWarning($"A {queryName} query was raised, but no handlers are configured.");
            return _notHandledDefault;
        }
    }
}

public class Query<T1, TReturn>
{
    public readonly string queryName;
    public Func<T1, TReturn> QueryHandler = default;

    private readonly TReturn _notHandledDefault;

    public Query(string queryName, TReturn notHandledDefault = default)
    {
        this.queryName = queryName;
        this._notHandledDefault = notHandledDefault;
    }

    public TReturn RaiseQuery(T1 arg0)
    {
        if (QueryHandler != null)
        {
            return QueryHandler.Invoke(arg0);
        }
        else
        {
            Debug.LogWarning($"A {queryName} query was raised, but no handlers are configured.");
            return _notHandledDefault;
        }
    }
}

public class Query<T1, T2, TReturn>
{
    public readonly string queryName;
    public Func<T1, T2, TReturn> QueryHandler = default;

    private readonly TReturn _notHandledDefault;

    public Query(string queryName, TReturn notHandledDefault = default)
    {
        this.queryName = queryName;
        this._notHandledDefault = notHandledDefault;
    }

    public TReturn RaiseQuery(T1 arg0, T2 arg1)
    {
        if (QueryHandler != null)
        {
            return QueryHandler.Invoke(arg0, arg1);
        }
        else
        {
            Debug.LogWarning($"A {queryName} query was raised, but no handlers are configured.");
            return _notHandledDefault;
        }
    }
}

public class Query<T1, T2, T3, TReturn>
{
    public readonly string queryName;
    public Func<T1, T2, T3, TReturn> QueryHandler = default;

    private readonly TReturn _notHandledDefault;

    public Query(string queryName, TReturn notHandledDefault = default)
    {
        this.queryName = queryName;
        this._notHandledDefault = notHandledDefault;
    }

    public TReturn RaiseQuery(T1 arg0, T2 arg1, T3 arg2)
    {
        if (QueryHandler != null)
        {
            return QueryHandler.Invoke(arg0, arg1, arg2);
        }
        else
        {
            Debug.LogWarning($"A {queryName} query was raised, but no handlers are configured.");
            return _notHandledDefault;
        }
    }
}

public class Query<T1, T2, T3, T4, TReturn>
{
    public readonly string queryName;
    public Func<T1, T2, T3, T4, TReturn> QueryHandler = default;

    private readonly TReturn _notHandledDefault;

    public Query(string queryName, TReturn notHandledDefault = default)
    {
        this.queryName = queryName;
        this._notHandledDefault = notHandledDefault;
    }

    public TReturn RaiseQuery(T1 arg0, T2 arg1, T3 arg2, T4 arg3)
    {
        if (QueryHandler != null)
        {
            return QueryHandler.Invoke(arg0, arg1, arg2, arg3);
        }
        else
        {
            Debug.LogWarning($"A {queryName} query was raised, but no handlers are configured.");
            return _notHandledDefault;
        }
    }
}