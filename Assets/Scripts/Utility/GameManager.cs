using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        MapData.BrainAmount = 0;
        MapData.FingerAmount = 0;
    }
}
