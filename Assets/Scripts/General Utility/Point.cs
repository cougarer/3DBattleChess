using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: MaxLykoS
//UpdateTime: 2017/10/21

[System.Serializable]
public class Point
{
    //坐标
    public int X;
    public int Z;

    public Point(int parax,int paraz)
    {
        X = parax;
        Z = paraz;
    }

    public static bool operator ==(Point p1, Point p2)
    {
        if ((p1 as object) == null) return ((p2 as object) == null);   //判空

        if (p1.X == p2.X 
            && p1.Z == p2.Z)
            return true;
        else return false;
    }
    public static bool operator !=(Point p1, Point p2)
    {
        if ((p1 as object) == null) return ((p2 as object) != null);

        if (p1.X != p2.X || p1.Z != p2.Z) return true;
        else return false;
    }

    public Point Left()
    {
        return new Point(X, Z + 1);
    }
    public Point Up()
    {
        return new Point(X + 1, Z);
    }
    public Point Right()
    {
        return new Point(X, Z - 1);
    }
    public Point Down()
    {
        return new Point(X - 1, Z);
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        if ((obj.GetType().Equals(GetType())) == false)
        {
            return false;
        }
        Point temp = null;
        temp = (Point)obj;

        return X.Equals(temp.X) && Z.Equals(temp.Z);
    }

    public override int GetHashCode()
    {
        return X.GetHashCode() + Z.GetHashCode();
    }

    public override string ToString()
    {
        return X.ToString()+","+Z.ToString();
    }

    public static Point StringToPoint(string st)
    {
        string[] str= st.Split(',');
        return new Point(int.Parse(str[0]), int.Parse(str[1]));
    }
}
