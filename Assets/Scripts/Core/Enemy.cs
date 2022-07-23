using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : ObjectRigidbody
{
    public Enemy(Vector2 pos) : base(pos, 90) { }

    public void MoveTo(Vector3 TargetPos)
    {
        //Move Emeny ship in to Player ship
        SetVelosity( Vector3.Lerp(GetVelosity(), (TargetPos - GetPosition()).normalized * 80, 0.02f)  );
        UpdatePosition();
    }
}
