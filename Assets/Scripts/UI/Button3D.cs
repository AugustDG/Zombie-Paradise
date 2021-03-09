using UnityEngine;
using UnityEngine.Events;
public class Button3D : MonoBehaviour
{

    [SerializeField] private float yPressed, yNormal;
    [SerializeField] private Mesh baseMesh;
    [SerializeField] private Mesh hoverMesh;
    [SerializeField] private Mesh pressedMesh;
    
    public UnityEvent onClick = new UnityEvent();

    private MeshFilter _meshFilter;
    private bool _justClicked;
    
    // Start is called before the first frame update
    void Awake()
    {
        _meshFilter = GetComponentInChildren<MeshFilter>();
    }

    public void SwitchButtonMesh(ButtonState state)
    {
        var tempMeshPos = _meshFilter.transform.localPosition;
        
        switch (state)
        {
            case ButtonState.None:
                _meshFilter.sharedMesh = baseMesh;
                tempMeshPos.y = yNormal;
                _justClicked = false;
                break;
            case ButtonState.Hovered:
                _meshFilter.sharedMesh = hoverMesh;
                tempMeshPos.y = yNormal;
                _justClicked = false;
                break;
            case ButtonState.Pressed:
                _meshFilter.sharedMesh = pressedMesh;
                tempMeshPos.y = yPressed;
                if (!_justClicked) onClick.Invoke();
                _justClicked = true;
                break;
        }

        _meshFilter.transform.localPosition = tempMeshPos;
    }
}

public enum ButtonState
{
    None,
    Hovered,
    Pressed
}
