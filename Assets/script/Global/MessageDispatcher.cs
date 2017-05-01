using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum MessageType
{ 
    Msg_Attack,
    Msg_Flee,
    Msg_Damage,
    Msg_Push,
}

public class Telegram
{
    public BaseEntity Sender{get;set;}
    public BaseEntity Receiver{get;set;}
    public MessageType Msg{get;set;}
    public float DispatchTime{get;set;}
    public System.Object ExtraInfo{get;set;}
    public override bool Equals(object o)
    {
        Telegram e = o as Telegram;
        return e.DispatchTime - DispatchTime < Config.TelegramEqualDelteTime && Sender == e.Sender && Receiver == e.Receiver && Msg == e.Msg;
    }
    public override int GetHashCode()
    {
        return (int)(DispatchTime * 100);
    }
    public Telegram(BaseEntity s, BaseEntity r, MessageType m, float t, System.Object i)
    {
        Sender = s;
        Receiver = r;
        Msg = m;
        DispatchTime = t;
        ExtraInfo = i;
    }
}

public class MessageDispatcher{

    static MessageDispatcher m_Instance;
    public static MessageDispatcher Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new MessageDispatcher();
            }
            return m_Instance;
        }
    }

    HashSet<Telegram> PriorityQueue;

    MessageDispatcher()
    {
        PriorityQueue = new HashSet<Telegram>();
    }
    

    void Discharge(BaseEntity Receiver, Telegram msg)
    {
        Receiver.HandleMessage(msg);
    }

    public void DispatchMessage(float delay, BaseEntity sender, BaseEntity receiver, MessageType msg, System.Object ExtraInfo)
    { 
        Telegram telegram = new Telegram(sender,receiver,msg,delay,ExtraInfo);
        if (delay <= 0.0)
        {
            Discharge(receiver, telegram);
        }
        else
        {
            float CurrentTime = Time.time;
            telegram.DispatchTime = CurrentTime + delay;
            PriorityQueue.Add(telegram);
        }
    }

    public void DispatchDelayedMessages()
    {
        float CurrentTime = Time.time;
        List<Telegram> sortList = PriorityQueue.ToList();
        sortList.Sort((x, y) => x.DispatchTime.CompareTo(y.DispatchTime));
        int i = 0;
        while (sortList[i].DispatchTime < CurrentTime && sortList[i].DispatchTime > 0)
        { 
            Discharge(sortList[i].Receiver,sortList[i]);
            PriorityQueue.Remove(sortList[i]);
            ++i;
        }
    }

}
