using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using BaseSpace;


interface IExecuteable
{
    public void Execute(string id);
}

interface IExecutrable<T1>
{
    public void Execute(string id, T1 t1);

}
interface IExecutrable<T1, T2>
{
    public void Execute(string id, T1 t1, T2 t2);

}
interface IExecutrable<T1, T2, T3>
{
    public void Execute(string id, T1 t1, T2 t2, T3 t3);
}

interface IExecutrable<T1, T2, T3,T4>
{
    public void Execute(string id, T1 t1, T2 t2, T3 t3,T4 t4);
}


public class SimpleSendBox<T1, T2, T3,T4> : BaseSpace.SimpleSendBox<SimpleSendBox<T1, T2, T3,T4>>
{

    public void Push(SimpleLetter<T1, T2, T3,T4> letter)
    {
        SimpleSender<T1, T2, T3,T4>.Instance.mail.Enqueue(letter);
    }

}


public class SimpleSendBox<T1, T2, T3> : BaseSpace.SimpleSendBox<SimpleSendBox<T1, T2, T3>>
{

    public void Push(SimpleLetter<T1, T2, T3> letter)
    {
        SimpleSender<T1, T2, T3>.Instance.mail.Enqueue(letter);
    }

}
public class SimpleSendBox<T1> : BaseSpace.SimpleSendBox<SimpleSendBox<T1>>
{
    public void Push(SimpleLetter<T1> letter)
    {
        SimpleSender<T1>.Instance.mail.Enqueue(letter);
    }


}

public class SimpleSendBox<T1, T2> : BaseSpace.SimpleSendBox<SimpleSendBox<T1, T2>>
{
    public void Push(SimpleLetter<T1, T2> letter)
    {
        SimpleSender<T1, T2>.Instance.mail.Enqueue(letter);
    }
}




public class SimpleSendBox : BaseSpace.SimpleSendBox<SimpleSendBox>
{

    public void Push(SimpleLetter letter)
    {
        SimpleSender.Instance.mail.Enqueue(letter);
    }

}


public class SimpleSender : BaseSpace.SimpleSender<SimpleSender>
{

    public ConcurrentQueue<SimpleLetter> mail = new ConcurrentQueue<SimpleLetter>();
    public override void Update()
    {

        while (mail.Count > 0)
        {
            SimpleLetter tmp;
            if (mail.TryDequeue(out tmp))
                SimpleMail.Instance.Execute(tmp.id);
        }
    }
}

public class SimpleSender<T1> : BaseSpace.SimpleSender<SimpleSender<T1>>
{
    public ConcurrentQueue<SimpleLetter<T1>> mail = new ConcurrentQueue<SimpleLetter<T1>>();
    public override void Update()
    {

        while (mail.Count > 0)
        {
            SimpleLetter<T1> tmp;
            if (mail.TryDequeue(out tmp))
            {

                SimpleMail<T1>.Instance.Execute(tmp.id, tmp.t1);
            }
        }
    }
}

public class SimpleSender<T1, T2> : BaseSpace.SimpleSender<SimpleSender<T1, T2>>
{
    public ConcurrentQueue<SimpleLetter<T1, T2>> mail = new ConcurrentQueue<SimpleLetter<T1, T2>>();
    public override void Update()
    {

        while (mail.Count > 0)
        {
            SimpleLetter<T1, T2> tmp;
            if (mail.TryDequeue(out tmp))
                SimpleMail<T1, T2>.Instance.Execute(tmp.id, tmp.t1, tmp.t2);
        }
    }
}

public class SimpleSender<T1, T2, T3> : BaseSpace.SimpleSender<SimpleSender<T1, T2, T3>>
{

    public ConcurrentQueue<SimpleLetter<T1, T2, T3>> mail = new ConcurrentQueue<SimpleLetter<T1, T2, T3>>();


    public override void Update()
    {

        while (mail.Count > 0)
        {

            SimpleLetter<T1, T2, T3> tmp;

            if (mail.TryDequeue(out tmp))
            {
                SimpleMail<T1, T2, T3>.Instance.Execute(tmp.id, tmp.t1, tmp.t2, tmp.t3);
            }
        }
    }
}

public class SimpleSender<T1, T2, T3,T4> : BaseSpace.SimpleSender<SimpleSender<T1, T2, T3,T4>>
{

    public ConcurrentQueue<SimpleLetter<T1, T2, T3,T4>> mail = new ConcurrentQueue<SimpleLetter<T1, T2, T3,T4>>();


    public override void Update()
    {

        while (mail.Count > 0)
        {

            SimpleLetter<T1, T2, T3,T4> tmp;

            if (mail.TryDequeue(out tmp))
            {
                SimpleMail<T1, T2, T3,T4>.Instance.Execute(tmp.id, tmp.t1, tmp.t2, tmp.t3,tmp.t4);
            }
        }
    }
}

public class SimpleLetter<T1, T2, T3,T4> : SimpleLetter
{

    public T1 t1;
    public T2 t2;
    public T3 t3;
    public T4 t4;
}

public class SimpleLetter<T1, T2, T3> : SimpleLetter
{

    public T1 t1;
    public T2 t2;
    public T3 t3;
}

public class SimpleLetter<T1, T2> : SimpleLetter
{

    public T1 t1;
    public T2 t2;
}


public class SimpleLetter<T1> : SimpleLetter
{

    public T1 t1;
}


public class SimpleLetter
{
    public string id;
}


public partial class SimpleMailManager : Singleton<SimpleMailManager>
{
    private List<Senderable> senders = new List<Senderable>();
    public void Register(Senderable sender)
    {
        if (!senders.Contains(sender))
            senders.Add(sender);
    }

    public void UnRegister(Senderable sender)
    {
        senders.Remove(sender);
    }

    public void Update()
    {
        foreach (Senderable item in senders)
        {
            item.Update();
        }
    }


    public void SendLetter<T>(T t1,string id)
    {
        SimpleLetter<T> simpleLetter = new SimpleLetter<T>();
        simpleLetter.id = id;
        simpleLetter.t1 = t1;
        SimpleSendBox<T>.Instance.Push(simpleLetter);
    }

    public void SendLetter<T,T2>(T t1,T2 t2, string id)
    {
        SimpleLetter<T,T2> simpleLetter = new SimpleLetter<T,T2>();
        simpleLetter.id = id;
        simpleLetter.t1 = t1;
        simpleLetter.t2 = t2;
        SimpleSendBox<T,T2>.Instance.Push(simpleLetter);
    }

    public void SendLetter<T,T2,T3>(T t1,T2 t2,T3 t3, string id)
    {
        SimpleLetter<T,T2,T3> simpleLetter = new SimpleLetter<T,T2,T3>();
        simpleLetter.id = id;
        simpleLetter.t1 = t1;
        simpleLetter.t2 = t2;
        simpleLetter.t3 = t3;
        SimpleSendBox<T,T2, T3>.Instance.Push(simpleLetter);
    }

    public void SendLetter<T,T2,T3,T4>(T t1,T2 t2,T3 t3 ,T4  t4, string id)
    {
        SimpleLetter<T,T2,T3,T4> simpleLetter = new SimpleLetter<T,T2,T3,T4>();
        simpleLetter.id = id;
        simpleLetter.t1 = t1;
        SimpleSendBox<T, T2, T3, T4>.Instance.Push(simpleLetter);
    }
}



public class SimpleMail<T1, T2, T3,T4> : BaseSpace.SimpleMail<SimpleMail<T1, T2, T3,T4>>, IExecutrable<T1, T2, T3,T4>
{

    Dictionary<string, Action<T1, T2, T3, T4>> registered = new Dictionary<string, Action<T1, T2, T3, T4>>();
    public override void Init()
    {
        base.Init();
    }

    public void Execute(string id, T1 t1, T2 t2, T3 t3,T4 t4)
    {
        if (registered[id] == null)
            Debug.Log("Registered is Null   " + id);
        registered[id]?.Invoke(t1, t2, t3,t4);
    }

    //public void Execute(string id, params object[] objects)
    //{
    //    Execute(id, objects[0], objects[1], objects[2]);
    //}

    public void Register(string id, Action<T1, T2, T3,T4> events)
    {
        Debug.Log("register ID: " + id + "     register action:" + events);
        Action<T1, T2, T3,T4> tmp;
        if (!registered.TryGetValue(id, out tmp))
        {
            registered.Add(id, events);
        }
        else
        {
            CustomLog.LogError("Regiseter Failed:" + id);
        }

    }

    public void Remove(string id)
    {
        Action<T1, T2, T3,T4> tmp;
        if (registered.TryGetValue(id, out tmp))
            registered.Remove(id);
        else
        {

        }
    }


}


public class SimpleMail<T1, T2, T3, T4, T5, T6, T7, T8> : Singleton<SimpleMail<T1, T2, T3, T4, T5, T6, T7, T8>>
{

}


public class SimpleMail : BaseSpace.SimpleMail<SimpleMail>, IExecuteable
{
    Dictionary<string, Action> registered = new Dictionary<string, Action>();
    public override void Init()
    {
        base.Init();
    }

    public void Execute(string id)
    {
        Debug.Log("id:"+id);
        registered[id]?.Invoke();
    }

    public void Register(string id, Action events)
    {

        Action tmp;
        if (!registered.TryGetValue(id, out tmp))
        {
            registered.Add(id, events);
        }

    }

    public void Remove(string id)
    {
        Action tmp;
        if (registered.TryGetValue(id, out tmp))
            registered.Remove(id);
        else
        {

        }
    }
}
public class SimpleMail<T1> : BaseSpace.SimpleMail<SimpleMail<T1>>, IExecutrable<T1>
{
    Dictionary<string, Action<T1>> registered = new Dictionary<string, Action<T1>>();
    public void Execute(string id, T1 t1)
    {
        if (registered.ContainsKey(id))
        {
            registered[id]?.Invoke(t1);
        }
        else
            CustomLog.LogError("not have this key:" + id);
    }

    public void Register(string id, Action<T1> events)
    {
        Action<T1> tmp;
        if (!registered.TryGetValue(id, out tmp))
        {
            registered.Add(id, events);
        }

    }

    public void Remove(string id)
    {
        Action<T1> tmp;
        if (registered.TryGetValue(id, out tmp))
            registered.Remove(id);
        else
        {

        }
    }
}

public class SimpleMail<T1, T2> : BaseSpace.SimpleMail<SimpleMail<T1, T2>>, IExecutrable<T1, T2>
{
    Dictionary<string, Action<T1, T2>> registered = new Dictionary<string, Action<T1, T2>>();


    public void Execute(string id, T1 t1, T2 t2)
    {
        registered[id]?.Invoke(t1, t2);
    }

    public void Register(string id, Action<T1, T2> events)
    {

        Action<T1, T2> tmp;
        if (!registered.TryGetValue(id, out tmp))
        {
            registered.Add(id, events);
            CustomLog.LogError("Regiseter succes:" + id);
        }
        else
        {
            CustomLog.LogError("Regiseter Failed:" + id);
        }

    }

    public void Remove(string id)
    {
        Action<T1, T2> tmp;
        if (registered.TryGetValue(id, out tmp))
            registered.Remove(id);
        else
        {

        }
    }
}




public class SimpleMail<T1, T2, T3> : BaseSpace.SimpleMail<SimpleMail<T1, T2, T3>>, IExecutrable<T1, T2, T3>
{

    Dictionary<string, Action<T1, T2, T3>> registered = new Dictionary<string, Action<T1, T2, T3>>();
    public override void Init()
    {
        base.Init();
    }

    public void Execute(string id, T1 t1, T2 t2, T3 t3)
    {
        if (registered[id] == null)
            Debug.Log("Registered is Null   " + id);
        registered[id]?.Invoke(t1, t2, t3);
    }

    //public void Execute(string id, params object[] objects)
    //{
    //    Execute(id, objects[0], objects[1], objects[2]);
    //}

    public void Register(string id, Action<T1, T2, T3> events)
    {
        Debug.Log("register ID: " + id + "     register action:" + events);
        Action<T1, T2, T3> tmp;
        if (!registered.TryGetValue(id, out tmp))
        {
            registered.Add(id, events);
        }
        else
        {
            CustomLog.LogError("Regiseter Failed:" + id);
        }

    }

    public void Remove(string id)
    {
        Action<T1, T2, T3> tmp;
        if (registered.TryGetValue(id, out tmp))
            registered.Remove(id);
        else
        {

        }
    }


}

