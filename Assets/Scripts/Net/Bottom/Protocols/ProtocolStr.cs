using System;
using System.Collections;

//字符串协议天生的漏洞： 客户端只要发送一段含有逗号的字符串便会引起混淆

    //字符串协议模型
    //形式： 名称，参数1，参数2，参数3
class ProtocolStr:ProtocolBase
{
    //传输字符串
    public string str;

    //解码器：
    public override ProtocolBase Decode(byte[] readbuff, int start, int length)
    {
        ProtocolStr protocol = new ProtocolStr();
        protocol.str = System.Text.Encoding.UTF8.GetString(readbuff, start, length);
        return (ProtocolBase)protocol;
    }
    //编码器：
    public override byte[] Encode()
    {
        byte[] b = System.Text.Encoding.UTF8.GetBytes(str);
        return b;
    }
    //协议名称
    public override string GetName()
    {
        if (str.Length == 0) return "";
        return str.Split(',')[0];
       
    }

    //协议名称
    public override string GetDesc()
    {
        return str;
    }
}

