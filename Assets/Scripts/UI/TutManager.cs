using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutManager : MonoBehaviour
{
    [SerializeField] private TMP_Text storyText;
    [SerializeField] private TMP_Text tutText;
    [SerializeField] private Image fgImg;

    public bool isShowingStory = true;
    
    // Start is called before the first frame update
    void Start()
    {
        storyText.gameObject.SetActive(true);
        tutText.gameObject.SetActive(false);

        fgImg.DOFade(0f, 0.5f).Play().OnComplete(()=>fgImg.gameObject.SetActive(false));
    }

    public void NextScreen()
    {
        if (isShowingStory)
        {
            isShowingStory = false;

            storyText.DOFade(0f, 1f).Play().OnComplete(() =>
            {
                storyText.gameObject.SetActive(false);
                tutText.gameObject.SetActive(true);
                tutText.DOFade(1f, 1f).Play();
            });
        }
        else
        {
            fgImg.gameObject.SetActive(true);
            
            fgImg.DOFade(1f, 0.5f).Play().OnComplete(() =>
            {
                SceneManager.LoadSceneAsync(2, LoadSceneMode.Single); 
            });
        }
    }
}
