using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        MapData.BrainAmount = 0;
        MapData.FingerAmount = 0;
    }

    public void GameFinished()
    {
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
    }
}
