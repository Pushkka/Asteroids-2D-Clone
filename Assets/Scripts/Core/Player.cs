using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player : ObjectRigidbody
{
    public Player(Vector2 pos, float rot) : base(pos, rot) { }

    public int Health = 3;

    public float ShotDelay;
    public float LaserDelay;

    public int LaserCount = 3;
    public float LaserReload = 20;

    public delegate void ShotEvent();
    public event ShotEvent OnShot;
    public event ShotEvent OnLaserShot;
    public void Update()
    {
        Vector2 Input = PlayerInput.GetMovementInput();
        AddSpeed(Input.x);
        AddRotation(Input.y * 3);
        UpdatePosition();

        //Decreace Ship Reload Timers
        DecreaceDelay(0.02f);

        if (PlayerInput.IsShoot() && ShotDelay <= 0)
        {
            Shot();
        }
        if (PlayerInput.IsLaserShoot() && LaserDelay <= 0)
        {
            ShotLaser();
        }
    }
    public bool DeadCrash()
    {
        Health--;
        if (Health <= 0)
            return true;
        else
            return false;
    }

    private void DecreaceDelay(float DeltaTime)
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
    private void Shot()
    {
        ShotDelay = 0.25f;
        OnShot?.Invoke();
    }
    private void ShotLaser()
    {
        LaserCount -= 1;
        LaserDelay = 3;

        OnLaserShot?.Invoke();
    }
}
