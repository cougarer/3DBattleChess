using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

//网络连接
public class Connection
{
    const int BUFFER_SIZE = 1024;

    private Socket socket;

    private byte[] readBuff = new byte[BUFFER_SIZE];
    private int buffCount = 0;

    //粘包分包
    private Int32 msgLength = 0;
    private byte[] lenBytes = new byte[sizeof(int)];

    //协议
    public ProtocolBase proto;

    //心跳事件
    public float LastTickTime = 0;
    public float HeartBeatTime = 30;

    //消息分发
    public MsgDistribution msgDist = new MsgDistribution();

    //状态
    public enum Status
    {
        None,
        Connected,
    };
    public Status status = Status.None;

    //连接服务器
    public bool Connect(string host, int port)
    {
        try
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //connect
            socket.Connect(host, port);

            //BeginReceive
            socket.BeginReceive(readBuff, buffCount, BUFFER_SIZE - buffCount, SocketFlags.None, ReceiveCb, readBuff);
            Debug.Log("连接成功");
            //更新状态
            status = Status.Connected;
            return true;
        }
        catch (Exception ex)
        {
            Debug.Log("连接服务器失败"+ex.Message);
            return false;
        }
    }

    //异步接受回调
    private void ReceiveCb(IAsyncResult ar)
    {
        try
        {
            int count = socket.EndReceive(ar);
            buffCount += count;
            ProcessData();
            socket.BeginReceive(readBuff, buffCount, BUFFER_SIZE - buffCount, SocketFlags.None, ReceiveCb, readBuff);
        }
        catch (Exception ex)
        {
            Debug.Log("异步接受回调失败" + ex.Message);
            status = Status.None;
        }
    }

    private void ProcessData()
    {
        //粘包处理
        if (buffCount < sizeof(Int32))
            return;

        //包体长度
        Array.Copy(readBuff, lenBytes, sizeof(Int32));
        msgLength = BitConverter.ToInt32(lenBytes,0);
        if (buffCount < msgLength + sizeof(Int32))
            return;

        //协议解码
        ProtocolBase protocol = proto.Decode(readBuff, sizeof(Int32), msgLength);
        Debug.Log("收到消息" + protocol.GetDesc());

        Queue<ProtocolBase> msgQueue = msgDist.MsgQueue;
        lock (msgQueue)
            msgQueue.Enqueue(protocol);

        //清除已处理的消息
        int count = buffCount - msgLength - sizeof(Int32);
        Array.Copy(readBuff, msgLength + sizeof(Int32), readBuff, 0, count);
        buffCount = count;
        if (buffCount > 0)
            ProcessData();
    }

    public bool Send(ProtocolBase protocol)
    {
        if (status != Status.Connected)
        {
            Debug.LogError("还没建立间接无法发送数据");
            return false;
        }

        byte[] bytes = protocol.Encode();
        byte[] length = BitConverter.GetBytes(bytes.Length);
        byte[] sendbuff = length.Concat(bytes).ToArray();
        socket.Send(sendbuff);
        Debug.Log("发送消息"+protocol.GetDesc());
        return true;
    }
    public bool Send(ProtocolBase protocol, string cbName, MsgDistribution.Delegate cb)
    {
        if (status != Status.Connected)
            return false;
        msgDist.AddOnceListener(cbName, cb);
        return Send(protocol);
    }
    public bool Send(ProtocolBase protocol, MsgDistribution.Delegate cb)
    {
        string cbName = protocol.GetName();
        return Send(protocol, cbName, cb);
    }

    //关闭连接
    public bool Close()
    {
        try
        {
            socket.Close();
            status = Status.None;
            return true;
        }
        catch (Exception ex)
        {
            Debug.Log("断开连接失败" + ex.Message);
            return false;
        }
    }

    //心跳检测
    public void Update()
    {
        //消息
        msgDist.Update();
        if (status == Status.Connected)
        {
            if (Time.time - LastTickTime > HeartBeatTime)
            {
                ProtocolBase protocol = NetMgr.GetHeartBeatProtocol();
                Send(protocol);
                LastTickTime = Time.time;
            }
        }
    }
}
