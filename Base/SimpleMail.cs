using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BaseSpace
{

    public class SimpleSendBox<T1> : Singleton<T1> where T1 : new()
    {
        protected SimpleSendBox() { }
    }

    public class SimpleMail<T1> : Singleton<T1> where T1 : new()
    {
            protected SimpleMail() { }
    }


    public interface Senderable
    {
        public void SenderRegiseter();

        public void SenderUnRegiseter();
        public void Update();
    }

    public class SimpleSender<T1> : Singleton<T1>, Senderable where T1 : new()
    {
        protected SimpleSender() : base()
        {
            Debug.Log("SimpleSender Register");
            this.SenderRegiseter();
        }
        public void SenderRegiseter()
        {
            SimpleMailManager.Instance.Register(this);
        }

        public void SenderUnRegiseter()
        {
            SimpleMailManager.Instance.UnRegister(this);
        }

        public virtual void Update()
        {

        }

    
    }

}