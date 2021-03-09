using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
public class Cursor3D : MonoBehaviour
{
    private Camera _mainCam;

    private void Awake()
    {
        _mainCam = Camera.main;
    }

    // Update is called once per frame
    private void Update()
    {
        transform.DOMove(_mainCam.ScreenToWorldPoint( new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, _mainCam.nearClipPlane + 5f)), Time.deltaTime);
    }
}
