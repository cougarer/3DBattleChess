using System;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class DataMgr
{
    MySqlConnection sqlConn;

    //单例模式
    public static DataMgr Instance;
    public DataMgr()
    {
        Instance = this;
        Connect();
    }

    //连接数据库
    public void Connect()
    {
        //数据库
        string connStr = "Database=game;Data Source = 127.0.0.1;";
        connStr += "User Id=root;Password=123456;port=3306;charset=utf8";

        sqlConn = new MySqlConnection(connStr);
        try
        {
            sqlConn.Open();
        }
        catch (Exception ex)
        {
            Console.WriteLine("[DataMgr]Connect " + ex.Message);
            return;
        }
    }

    //判定安全字符串
    public bool IsSafeStr(string str)
    {
        return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
    }

    //判断是否存在该用户
    private bool CanRegister(string id)
    {
        //防sql注入
        if (!IsSafeStr(id))
            return false;

        //查询id是否存在
        string cmdStr = string.Format("select * from user where id ='{0}';", id);
        MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);

        try
        {
            using (MySqlDataReader dataReader = cmd.ExecuteReader())
            {
                bool hasRows = dataReader.HasRows;
                return !hasRows;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("[DataMgr]CanRegister fail " + ex.Message);
            return false;
        }
    }

    //注册
    public bool Register(string id, string pw)
    {
        //防sql注入
        if (!IsSafeStr(id) || !IsSafeStr(pw))
        {
            Console.WriteLine("[DataMgr]Register 使用非法字符");
            return false;
        }

        //判断能否注册
        if (!CanRegister(id))
        {
            Console.WriteLine("[DataMgr]Register 使用非法字符");
            return false;
        }

        //将数据写入数据库User表
        string cmdStr = string.Format("insert into user set id='{0}',pw='{1}';", id, pw);
        MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
        try
        {
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("[DataMgr]Register " + ex.Message);
            return false;
        }
    }

    //创建角色
    public bool CreatePlayer(string id)
    {
        //防sql注入
        if (!IsSafeStr(id))
            return false;

        //序列化
        IFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();
        PlayerData playerData = new PlayerData();
        try
        {
            formatter.Serialize(stream, playerData);
        }
        catch (Exception ex)
        {
            Console.WriteLine("[DataMgr]CreatePlayer 序列化" + ex.Message);
            return false;
        }

        byte[] byteArr = stream.ToArray();

        //写入数据库
        string cmdStr = string.Format("insert into player set id='{0}',data=@data;", id);
        MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
        #region 看不懂
        cmd.Parameters.Add("@data", MySqlDbType.Blob);
        cmd.Parameters[0].Value = byteArr;
        #endregion
        try
        {
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("[DataMgr]CreatePlayer 写入" + ex.Message);
            return false;
        }
    }

    //检测用户名和密码
    public bool CheckPassWord(string id, string pw)
    {
        //防sql注入
        if (!IsSafeStr(id) || !IsSafeStr(pw))
            return false;

        //查询
        string cmdStr = string.Format("select * from user where id='{0}' and pw='{1}';", id, pw);
        MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
        try
        {
            using (MySqlDataReader dataReader = cmd.ExecuteReader())
            {
                bool hasRows = dataReader.HasRows;
                return hasRows;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("[DataMgr]CheckPassWord " + ex.Message);
            return false;
        }
    }

    public PlayerData GetPlayerData(string id)
    {
        PlayerData playerData = null;

        //防sql注入
        if (!IsSafeStr(id))
            return playerData;

        //查询
        string cmdStr = string.Format("select * from player where id = '{0}';", id);
        MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
        byte[] buffer = new byte[1];
        try
        {
            using (MySqlDataReader dataReader = cmd.ExecuteReader())
            {
                if (!dataReader.HasRows)
                {
                    return playerData;
                }

                dataReader.Read();

                long len = dataReader.GetBytes(1, 0, null, 0, 0);   //1是data
                buffer = new byte[len];
                dataReader.GetBytes(1, 0, buffer, 0, (int)len);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("[DataMgr]GetPlayerData 查询 " + ex.Message);
            return playerData;
        }

        //反序列化
        MemoryStream stream = new MemoryStream(buffer);
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            playerData = (PlayerData)formatter.Deserialize(stream);
            return playerData;
        }
        catch (SerializationException ex)
        {
            Console.WriteLine("[DataMgr]GetPlayerData 反序列化 " + ex.Message);
            return playerData;
        }
    }

    public bool SavePlayer(Player player)
    {
        string id = player.id;
        PlayerData playerData = player.data;

        //序列化
        IFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();
        try
        {
            formatter.Serialize(stream, playerData);
        }
        catch (Exception ex)
        {
            Console.WriteLine("[DataMgr]SavePlayer 序列化 " + ex.Message);
            return false;
        }
        byte[] byteArr = stream.ToArray();

        //写入数据库
        string formaterStr = "update player set data = @data where id ='{0}';";
        string cmdStr = string.Format(formaterStr, player.id);
        MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
        cmd.Parameters.Add("@data", MySqlDbType.Blob);
        cmd.Parameters[0].Value = byteArr;
        try
        {
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("[DataMgr]SavePlayer 写入 " + ex.Message);
            return false;
        }
    }
}
