using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgDistribution
{
    //每帧处理消息的数量
    public const int NUM_MSG = 15;

    //消息队列
    public Queue<ProtocolBase> MsgQueue = new Queue<ProtocolBase>();

    //委托类型
    public delegate void Delegate(ProtocolBase proto);

    //反复执行的监听表
    private Dictionary<string, Delegate> eventDic = new Dictionary<string, Delegate>();
    //只执行一次的监听表
    private Dictionary<string, Delegate> onceDic = new Dictionary<string, Delegate>();

    //Update
    public void Update()
    {
        for (int i = 0; i < NUM_MSG; i++)
        {
            if (MsgQueue.Count > 0)
            {
                DispathMsgEvent(MsgQueue.Peek());
                lock (MsgQueue)
                {
                    MsgQueue.Dequeue();
                }
            }
            else
                break;
        }
    }

    public void DispathMsgEvent(ProtocolBase protocol)
    {
        string name = protocol.GetName();
        Debug.Log("分发处理消息"+ name);

        Delegate method;
        if (eventDic.TryGetValue(name,out method))
        {
            method(protocol);
        }
        if (onceDic.TryGetValue(name, out method))
        {
            method(protocol);
            onceDic[name] = null;
            onceDic.Remove(name);
        }
    }

    //添加监听事件
    public void AddListener(string name, Delegate cb)
    {
        Delegate method;
        if (eventDic.TryGetValue(name, out method))
            method += cb;
        else
            eventDic[name] = cb;       
    }

    //添加单词监听事件
    public void AddOnceListener(string name, Delegate cb)
    {
        Delegate method;
        if (onceDic.TryGetValue(name, out method))
            method += cb;
        else
            onceDic[name] = cb;
    }

    //删除监听事件
    public void DelListener(string name, Delegate cb)
    {
        Delegate method;
        if (eventDic.TryGetValue(name, out method))
        {
            method -= cb;
            if (eventDic[name] == null)
                eventDic.Remove(name);
        }
    }

    //删除单次监听事件
    public void DelOnceListener(string name, Delegate cb)
    {
        Delegate method;
        if (onceDic.TryGetValue(name, out method))
        {
            method -= cb;
            if (onceDic[name] == null)
                onceDic.Remove(name);
        }
    }
}
