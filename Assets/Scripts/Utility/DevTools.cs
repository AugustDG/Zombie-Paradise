using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

public class DevTools : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            MapData.FingerAmount += 10;
            MapData.BrainAmount += 1;
        }
    }
}