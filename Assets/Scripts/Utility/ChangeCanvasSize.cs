using UnityEngine;
using Utility;
[ExecuteInEditMode]
public class ChangeCanvasSize : MonoBehaviour
{
    void Update()
    {
        gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, MapData.MapSize.x);
        gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, MapData.MapSize.x);
    }
}
