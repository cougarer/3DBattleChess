using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterServer
{
    class Player
    {
        //id,连接，玩家数据，玩家临时数据
        public string id;
        public Conn Conn;
        public PlayerData Data;
        public PlayerTempData TempData;

        public Player(string id,Conn conn)
        {
            this.id = id;
            Conn = conn;
            TempData = new PlayerTempData();
        }

        //发送消息
        public void Send(ProtocolBase proto)
        {
            if (Conn == null)
                return;
            ConnMananger.Instance.Send(Conn, proto);
        }

        public static bool KickOff(string id,ProtocolBase proto)
        {
            Conn[] conns = ConnMananger.Instance.Conns;
            for (int i = 0; i < conns.Length; i++)
            {
                if (conns[i] == null) continue;
                if (!conns[i].IsUse) continue;
                if (conns[i].player == null) continue;
                if (conns[i].player.id == id)// 查找该id
                {
                    lock (conns[i].player)
                    {
                        if (proto != null)
                        {
                            conns[i].player.Send(proto);
                            return conns[i].player.Logout();
                        }
                    }
                }
            }
            return true;
        }

        public bool Logout()
        {
            //下线事件处理
            ConnMananger.Instance.handlerPlayerEvent.OnLogout(this);

            //保存


            //下线
            Conn.player = null;
            Conn.Close();
            return true;
        }
    }
}