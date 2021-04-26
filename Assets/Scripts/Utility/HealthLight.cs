using DG.Tweening;
using UnityEngine;

public class HealthLight : MonoBehaviour
{
    private Light _light;
    
    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponentInChildren<Light>();
    }

    public void FadeOut()
    {
        _light.DOIntensity(0f, 2.5f).Play().OnComplete(() => _light.enabled = false);
    }
}
