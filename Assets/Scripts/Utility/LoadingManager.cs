using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public float duration = 2f;

    [SerializeField] private RectTransform twirl;
    [SerializeField] private Image fG;
    [SerializeField] private Volume volume;

    private LensDistortion _distortion;

    private void Awake()
    {
        volume.profile.TryGet(out _distortion);
    }

    private IEnumerator Start()
    {
        fG.DOFade(0f, duration / 2f).Play();
        
        yield return new WaitForSecondsRealtime(duration / 2f);

        twirl.DOScale(new Vector3(1, 1, 1), duration / 1.5f).OnComplete(() => twirl.DOScale(Vector3.zero, 0.5f).Play());

        DOTween.To(() => _distortion.intensity.value, res => _distortion.intensity.value = res, -1f, duration);
        DOTween.To(() => _distortion.scale.value, res => _distortion.scale.value = res, 0.01f, duration).OnComplete(() => SceneManager.LoadSceneAsync(3, LoadSceneMode.Single));

        DOTween.PlayAll();

    }
}