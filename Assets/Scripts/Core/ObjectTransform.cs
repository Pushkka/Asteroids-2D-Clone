using UnityEngine;

public class ObjectTransform
{
    private Vector3 Pos;
    private float Rot;
    public Vector3 Move;

    public ObjectTransform(Vector3 pos, float rot)
    {
        Pos = pos;
        Rot = rot;
    }
    public ObjectTransform(Vector3 pos, float rot, float speed)
    {
        Pos = pos;
        Rot = rot;
        AddSpeed(speed);
    }


    //Change Object speed
    public void AddSpeed(float Speed)
    {
        Move += Quaternion.Euler(0,0,Rot) * Vector3.right * Speed;
    }

    //Change Object rotation
    public void AddRotation(float AddRotation)
    {
        Rot += AddRotation;
    }

    //Change Object position
    public void UpdatePos()
    {
        Pos += Move * 0.05f;

        //Teleport Object 
        if (Pos.x > 1020)
            Pos.x = -20;
        if (Pos.x < -20)
            Pos.x = 1020;

        if (Pos.y > 1020)
            Pos.y = -20;
        if (Pos.y < -20)
            Pos.y = 1020;
    }


    public Vector3 GetPosition()
    {
        return Pos;
    }
    public Vector3 GetDirection()
    {
        return Quaternion.Euler(0, 0, Rot) * Vector3.right;
    }
    public float GetRotation()
    {
        return Rot;
    }
}
