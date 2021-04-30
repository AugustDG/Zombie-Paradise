using UnityEngine;
using Utility;

public class BuildingBehaviour : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private float healthBit;
    [SerializeField] private int turnOffLightsPos;

    [Space(5f)]
    [Header("References")]
    [SerializeField] private HealthLight[] healthLights = new HealthLight[10];

    private void Start()
    {
        healthBit = MapData.CurrentTreeHealth / 10;
    }

    public void SufferDamage(float damage)
    {
        MapData.CurrentTreeHealth -= damage;

        //bigger than base health, no lights turn off
        if (MapData.CurrentTreeHealth >= 500) return;
        
        print(MapData.CurrentTreeHealth);

        if (MapData.CurrentTreeHealth < 0)
        {
            MapData.GameManagerRef.GameFinished(GameEndType.LossByTree);
        }
        else
        {
            turnOffLightsPos = 9 - (int)MapData.CurrentTreeHealth / (int)healthBit;

            for (var i = 0; i < turnOffLightsPos; i++) healthLights[turnOffLightsPos].FadeOut();
        }
    }
}