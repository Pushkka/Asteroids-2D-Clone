using UnityEngine;

[System.Serializable]
public abstract class ObjectRigidbody
{
    private Vector3 position;
    private float rotation;
    private Vector3 Velosity;

    public ObjectRigidbody(Vector3 pos, float rot)
    {
        position = pos;
        rotation = rot;
    }
    public ObjectRigidbody(Vector3 pos, float rot, float speed)
    {
        position = pos;
        rotation = rot;
        AddSpeed(speed);
    }


    public void UpdatePosition()
    {
        position += Velosity * 0.05f;

        //Teleport Object 
        if (position.x > 1020)
            position.x = -20;
        if (position.x < -20)
            position.x = 1020;

        if (position.y > 1020)
            position.y = -20;
        if (position.y < -20)
            position.y = 1020;
    }


    public void AddSpeed(float Speed)
    {
        Velosity += Quaternion.Euler(0,0,rotation) * Vector3.right * Speed;
    }
    public void AddRotation(float AddRotation)
    {
        rotation += AddRotation;

        if (rotation < 0)
            rotation += 360;
        if (rotation > 360)
            rotation -= 360;
    }
    public void SetVelosity(Vector3 newVel)
    {
        Velosity = newVel;
    }


    public Vector3 GetVelosity()
    {
        return Velosity;
    }
    public Vector3 GetPosition()
    {
        return position;
    }
    public Vector3 GetDirection()
    {
        return Quaternion.Euler(0, 0, rotation) * Vector3.right;
    }
    public float GetRotation()
    {
        return rotation;
    }
}
