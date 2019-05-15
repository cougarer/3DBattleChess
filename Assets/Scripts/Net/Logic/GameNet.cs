using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameNet
{
    //同步位置
    //UnitMove 部队，目的地
    public void SendMove(Point unit, Point dest)
    {
        ProtocolBytes info = new ProtocolBytes();
        info.AddString("UnitMove");
        info.AddPoint(unit);
        info.AddPoint(dest);

        NetMgr.srvConn.Send(info);
    }
    public enum GameStatus
    {
        Ready,
        Win,
        Lose,
    };
    //同步游戏状态
    //GameStatus 玩家0/1，状态
    public void SendStatus(bool isHost, GameStatus status)
    {
        ProtocolBytes pb = new ProtocolBytes();
        pb.AddString("GameStatus");
        pb.AddInt(isHost ? 0 : 1);
        pb.AddInt((int)status);
        NetMgr.srvConn.Send(pb);
    }

    //同步单位移动结束
    //MoveDone 部队坐标
    public void SendMoveDone(Point p)
    {
        ProtocolBytes pb = new ProtocolBytes();
        pb.AddString("MoveDone");
        pb.AddPoint(p);
        NetMgr.srvConn.Send(pb);
    }

    //同步创建部队
    //CreateUnit 部队类型 坐标
    public void SendCreateUnit(int type, Point pos)
    {
        ProtocolBytes pb = new ProtocolBytes();
        pb.AddString("CreateUnit");
        pb.AddInt(type);
        pb.AddPoint(pos);
        NetMgr.srvConn.Send(pb);
    }

    //主动攻击
    //AttackUnit 部队坐标 攻击目标
    public void SendAttackInitiative(Point s, Point e)
    {
        ProtocolBytes pb = new ProtocolBytes();
        pb.AddString("AttackInitiative");
        pb.AddPoint(s);
        pb.AddPoint(e);
        NetMgr.srvConn.Send(pb);
    }

    //被动攻击
    //AttackPassive 部队坐标 攻击目标
    public void SendAttackPassive(Point s, Point e)
    {
        ProtocolBytes pb = new ProtocolBytes();
        pb.AddString("AttackPassive");
        pb.AddPoint(s);
        pb.AddPoint(e);
        NetMgr.srvConn.Send(pb);
    }

    //部队被摧毁
    //UnitDestroy 坐标
    public void SendUnitDestroy(Point p)
    {
        ProtocolBytes pb = new ProtocolBytes();
        pb.AddString("UnitDestroy");
        pb.AddPoint(p);
        NetMgr.srvConn.Send(pb);
    }

    //占领建筑
    //BuildingCapture 坐标
    public void SendBuildingCapture(Point p)
    {
        ProtocolBytes pb = new ProtocolBytes();
        pb.AddString("BuildingCapture");
        pb.AddPoint(p);
        NetMgr.srvConn.Send(pb);
    }

    //装载单位
    //LoadUnit 单位 载具
    public void SendLoadUnit(Point unit, Point loader)
    {
        ProtocolBytes pb = new ProtocolBytes();
        pb.AddString("LoadUnit");
        pb.AddPoint(unit);
        pb.AddPoint(loader);
        NetMgr.srvConn.Send(pb);
    }

    //卸载单位
    //UnloadUnit 单位 载具
    public void SendUnloadUnit(Point unit, Point loader)
    {
        ProtocolBytes pb = new ProtocolBytes();
        pb.AddString("UnloadUnit");
        pb.AddPoint(unit);
        pb.AddPoint(loader);
        NetMgr.srvConn.Send(pb);
    }
}
