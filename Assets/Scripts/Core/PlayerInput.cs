using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class PlayerInput
{
    public static bool IsShoot()
    {
        return Keyboard.current.spaceKey.isPressed;
    }
    public static bool IsLaserShoot()
    {
        return Keyboard.current.leftCtrlKey.isPressed;
    }

    public static Vector2 GetMovementInput()
    {
        Vector2 Input = new Vector2();
        if (Keyboard.current.wKey.isPressed)
            Input.x += 1;
        if (Keyboard.current.sKey.isPressed)
            Input.x -= 1;
        if (Keyboard.current.aKey.isPressed)
            Input.y += 1;
        if (Keyboard.current.dKey.isPressed)
            Input.y -= 1;

        return Input;
    }
}
