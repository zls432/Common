using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class GlobalTable<T> : Singleton<GlobalTable<T>>
{
    class TableDate<T1>
    {
        public T1 value;
        public object owner;

        public TableDate(T1 value, object ob)
        {
            this.value = value;
            this.owner = ob;
        }


    }
    Dictionary<string, TableDate<T>> table = new Dictionary<string, TableDate<T>>();
    Dictionary<String, List<Action<T>>> actionTable = new Dictionary<string, List<Action<T>>>();
    public void BindDate(String id, Action<T> action)
    {
        List<Action<T>> cahceActions;
        if (!actionTable.TryGetValue(id, out cahceActions))
            actionTable[id] = new List<Action<T>>();
        actionTable[id].Add(action);
    }
    public void UnBindDate(string id, Action<T> action)
    {
        actionTable[id]?.Remove(action);
    }

    public void SetValue(string id, T value, object owner = null)
    {
        TableDate<T> cahce;
        if (table.TryGetValue(id, out cahce))
        {
            if (cahce.owner == owner)
            {
                cahce.value = value;

            }
            else
            {
                throw new Exception("No access rights or duplicate names are used.");
            }
        }
        else
        {
            table[id] = new TableDate<T>(value, owner);
        }

        List<Action<T>> actionCahce;
        actionTable.TryGetValue(id, out actionCahce);
        if (actionCahce != null&& actionCahce.Count>0)
            foreach (var item in actionCahce)
            {
                item?.Invoke(value);
            }
    }

    public T GetValue(string id)
    {
        TableDate<T> cahce;
        if (table.TryGetValue(id, out cahce))
        {
            return cahce.value;
        }
        throw new Exception($"GlobalTable does not have  key:{id}.");
    }
    public bool TryGetValue(string id ,out T t)
    {
        TableDate<T> cahce;
        if (table.TryGetValue(id, out cahce))
        {
            t = cahce.value;
            return true;
        }
        else
        {
            t = default(T);
            return false;
        }
        throw new Exception($"GlobalTable does not have  key:{id}.");
    }
}
