using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Asteroid : ObjectRigidbody
{
    public Asteroid(Vector2 pos, float rot, float speed, bool Big) : base(pos, rot, speed)
    {
        BigAsteroid = Big;
    }

    public bool BigAsteroid;
}
