using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utility;

public class EndMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Text textOver;
    [SerializeField] private TMP_Text textWin;
    [SerializeField] private TMP_Text textStats;
    [SerializeField] private CanvasGroup optionsGroup;
    
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void StartSequence(GameEndType endType)
    {
        textWin.text = endType == GameEndType.Win ? "For humans >:D" : "For you... :(";
        textStats.text = $"Brains: {MapData.BrainAmount} | Fingers: {MapData.FingerAmount}";
        
        DOTween.To(() => Time.timeScale, scale => Time.timeScale = scale, 0f, 2f);
        GetComponentInParent<Image>().DOFade(1f, 2f).SetEase(Ease.OutCirc).OnComplete(() =>
        {
            _canvasGroup.interactable = true;
            _canvasGroup.DOFade(1f, 2f).OnComplete(() =>
            {
                textOver.DOFade(0f, 1f);
                textWin.DOFade(1f, 1f).OnComplete(() =>
                {
                    textStats.DOFade(1f, 1f);
                    optionsGroup.gameObject.SetActive(true);
                    optionsGroup.interactable = true;
                    optionsGroup.DOFade(1f, 2f);
                });
            }); 
        });

        DOTween.PlayAll();
    }

    public void Again()
    {
        Time.timeScale = 1f;
        MapData.CanSpawnZombies = true;
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
    }
    
    public void Quit()
    {
        Application.Quit();
    }
}
