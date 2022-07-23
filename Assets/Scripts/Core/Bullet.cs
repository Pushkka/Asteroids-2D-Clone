using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : ObjectTransform
{
    public Bullet(Vector2 pos, float rot, float speed, bool myBullet) : base(pos, rot, speed)
    {
        MyBullet = myBullet;
    }

    public bool MyBullet;
}
