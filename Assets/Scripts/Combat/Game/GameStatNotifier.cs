using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatNotifier
{
    private static GameStatNotifier instance = new GameStatNotifier();
    public static GameStatNotifier Instance
    {
        get { return instance; }
    }

    public int[] Create = new int[2];
    public int[] Elimination = new int[2];
    public int[] Destroyed = new int[2];
    public int[] Capture = new int[2];
    public int[] Earn = new int[2];
    public int[] Cost = new int[2];
    public float[] Attack = new float[2];
    public float[] BeAttack = new float[2];

    public int Round;
    private long time;
    public void StartTimer()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 10, 0, 0, 0, 0);
        time = Convert.ToInt64(ts.TotalSeconds);
    }
    public long StopTimer()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 10, 0, 0, 0, 0);
        time = Convert.ToInt64(ts.TotalSeconds) - time-5;
        return time;
    }

    public void Init()
    {
        for (int i = 0; i < 2; i++)
        {
            Create[i] = 0;
            Elimination[i] = 0;
            Capture[i] = 0;
            Earn[i] = 0;
            Cost[i] = 0;
            Attack[i] = 0;
            BeAttack[i] = 0;
        }
        Round = 1;
        time = 0;
    }
}
