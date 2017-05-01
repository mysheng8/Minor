using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State<T> {
    public virtual void Enter(T ent){}
    public virtual void Execute(T ent){}
    public virtual void Exit(T ent){}
    public virtual bool OnMessage(T ent, Telegram msg) { return false; }
}

