using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utility;

public class GameManager : MonoBehaviour
{
    public int initialBrains;
    public int initialFingers;

    [SerializeField] private TMP_Text timerText;
    [SerializeField] private EndMenuManager endMenu;

    private void Awake()
    {
        MapData.GameManagerRef = this;

        MapData.BrainAmount = initialBrains;
        MapData.FingerAmount = initialFingers;

        MapEvents.HumanKilledEvent += HumanKilledHandler;
        
        StartCoroutine(IncreaseFingers());
    }

    public void Start()
    {
        //StartCoroutine(LevelTimer());
    }

    private void OnDestroy()
    {
        Pathfinder.Cleanup();
    }

    public void GameFinished(GameEndType endType)
    {
        Pathfinder.Cleanup();
        MapData.CanSpawnZombies = false;

        endMenu.StartSequence(endType);
    }

    private IEnumerator IncreaseFingers()
    {
        yield return new WaitForSeconds(10f);

        MapData.FingerAmount += 10;

        StartCoroutine(IncreaseFingers());
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