using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : ObjectTransform
{
    public Asteroid(Vector2 pos, float rot, float speed, bool Big) : base(pos, rot, speed)
    {
        BigAsteroid = Big;
    }

    public bool BigAsteroid;
}
