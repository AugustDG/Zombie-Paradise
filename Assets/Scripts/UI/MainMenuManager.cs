using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private AudioClip _clip;
    [SerializeField] private CinemachineVirtualCamera _vCam;
    [SerializeField] private CinemachineBrain _brain;

    private CanvasGroup _group;
    private AudioSource _source;
    
    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _group = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (!_source.isPlaying)
        {
            _source.clip = _clip;
            _source.loop = true;
            _source.Play();
        }
    }

    public void TransitionToGame()
    {
        _group.DOFade(0f, 0.75f).OnComplete(() =>
        {
            _vCam.Priority = 50;
            StartCoroutine(ResumeGame());
        }).Play();
    }

    private IEnumerator ResumeGame()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        yield return new WaitUntil(() => !_brain.IsBlending);
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit(0);
    }
}
