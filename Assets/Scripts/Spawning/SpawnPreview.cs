using UnityEngine;
public class SpawnPreview : MonoBehaviour
{
    public bool IsError
    {
        get => _isError;
        set
        {
            _isError = value;
            SetMaterialColor();
        }
    }
    private bool _isError;

    public bool hasJustSpawned = true;
    public Vector2Int gridPosition;

    private Renderer _rend; 
    
    public void Awake()
    {
        _rend = GetComponentInChildren<MeshRenderer>();
    }

    private void SetMaterialColor()
    {
        _rend.material.SetColor("_EmissionColor", IsError ? Color.red : Color.green);
    }
}