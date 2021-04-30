using System;
using DG.Tweening;
using UnityEngine;
using Utility;

public class HealthLight : MonoBehaviour
{
    private Light _light;
    private bool _hasFaded;
    
    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponentInChildren<Light>();
    }

    public void FadeOut()
    {
        if (!_hasFaded)
        {
            _light.DOIntensity(0f, 2.5f).Play().OnComplete(() => _light.enabled = false);
            _hasFaded = true;
            MapEvents.TreeLifeLostEvent.Invoke(this, EventArgs.Empty);
        }
    }
}
