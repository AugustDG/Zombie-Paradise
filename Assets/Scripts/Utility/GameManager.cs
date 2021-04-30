using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

public class GameManager : MonoBehaviour
{
    public int initialBrains;
    public int initialFingers;

    [SerializeField] private TMP_Text timerText;
    
    private void Awake()
    {
        MapData.GameManagerRef = this;
        
        MapData.BrainAmount = initialBrains;
        MapData.FingerAmount = initialFingers;
        
        MapEvents.HumanKilledEvent += HumanKilledHandler;
    }

    public void Start()
    {
        StartCoroutine(LevelTimer());
    }

    public void GameFinished(GameEndType endType)
    {
        Pathfinder.Cleanup();
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
    }

    private IEnumerator LevelTimer()
    {
        yield return new WaitForSeconds(1f);
        MapData.CurrentTime--;

        timerText.text = TimeSpan.FromSeconds(MapData.CurrentTime).ToString(@"mm\:ss");
        
        if (MapData.CurrentTime == 0)
            GameFinished(GameEndType.LossByTime);

        StartCoroutine(LevelTimer());
    }
    
    private void HumanKilledHandler(object sender, EventArgs e)
    {
        if (MapData.HumanList.Count == 0) GameFinished(GameEndType.Win);
    }
}
