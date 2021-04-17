using UnityEngine;
using UnityEngine.Events;
using Utility;
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

    public void SwitchButtonMesh(Button3DState state)
    {
        var tempMeshPos = _meshFilter.transform.localPosition;
        
        switch (state)
        {
            case Button3DState.None:
                _meshFilter.sharedMesh = baseMesh;
                tempMeshPos.y = yNormal;
                _justClicked = false;
                break;
            case Button3DState.Hovered:
                _meshFilter.sharedMesh = hoverMesh;
                tempMeshPos.y = yNormal;
                _justClicked = false;
                break;
            case Button3DState.Pressed:
                _meshFilter.sharedMesh = pressedMesh;
                tempMeshPos.y = yPressed;
                if (!_justClicked) onClick.Invoke();
                _justClicked = true;
                break;
        }

        _meshFilter.transform.localPosition = tempMeshPos;
    }
}
