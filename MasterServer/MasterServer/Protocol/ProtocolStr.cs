using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ProtocolStr:ProtocolBase
{
    //传输的字符串
    public string str;

    //解码器,将字节流转换为字符串
    public override ProtocolBase Decode(byte[] readbuff, int start, int length)
    {
        ProtocolStr protocol = new ProtocolStr();
        protocol.str = Encoding.UTF8.GetString(readbuff, start, length);

        return (ProtocolBase)protocol;
    }

    //编码器
    public override byte[] Encode()
    {
        byte[] b = Encoding.UTF8.GetBytes(str);
        return b;
    }

    //协议名称   协议名，信息
    public override string GetName()
    {
        if (str.Length == 0) return "";
        return str.Split('.')[0];
    }

    //协议描述,就是str
    public override string GetDesc()
    {
        return str;
    }
}
