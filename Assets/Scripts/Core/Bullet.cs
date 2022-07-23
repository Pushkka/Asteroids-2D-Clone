using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Bullet : ObjectRigidbody
{
    public Bullet(Vector2 pos, float rot, float speed, bool myBullet) : base(pos, rot, speed)
    {
        FriendlyBullet = myBullet;
    }

    public bool FriendlyBullet;

    public Collider2D Col;
}
