using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnselectSelf : MonoBehaviour
{
    private void Awake() => GetComponent<Button>().onClick.AddListener(() => EventSystem.current.SetSelectedGameObject(null));
}
