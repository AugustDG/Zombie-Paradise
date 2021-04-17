using DG.Tweening;
using UnityEngine;
using Utilities.Extensions;

public class AudioObject : MonoBehaviour
{
    [HideInInspector] public AudioSource source;

    private float _defaultVolume;
    
    // Start is called before the first frame update
    void Awake()
    {
        source = GetComponent<AudioSource>();

        _defaultVolume = source.volume;
    }

    //only happens if has particle system attached
    private void OnParticleSystemStopped()
    {
        source.DOKill();
        gameObject.Destroy();
    }

    public void FadeIn(float duration)
    {
        if (!source.isPlaying)
        {
            source.volume = 0;
            source.Play(); //plays audio
        }
        source.DOFade(_defaultVolume, duration).Play(); //plays fade in
    }
    
    public void FadeTo(float volume, float duration)
    {
        source.DOFade(volume, duration).Play(); //plays fade to
    }
    
    public void FadeOut(float duration)
    {
        source.DOFade(0, duration).Play();
    }
}
