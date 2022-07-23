using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : ObjectTransform
{
    public Player(Vector2 pos, float rot) : base(pos, rot)
    {

    }

    public int Health = 3;

    public float ShotDelay;
    public float LaserDelay;

    public int LaserCount = 3;
    public float LaserReload = 20;

    public void DecreaceDelay(float DeltaTime)
    {
        if(ShotDelay > 0)
            ShotDelay -= DeltaTime;

        if (LaserDelay > 0)
            LaserDelay -= DeltaTime;

        if (LaserCount < 3)
            LaserReload -= DeltaTime;

        if (LaserReload <= 0)
        {
            LaserCount += 1;
            LaserReload = 20;
        }
    }
    public void Shot()
    {
        ShotDelay = 0.25f;
    }
    public void ShotLaser()
    {
        LaserCount -= 1;
        LaserDelay = 3;
    }
    public bool DeadCrash()
    {
        Health--;
        if (Health <= 0)
            return true;
        else
            return false;
    }
}
