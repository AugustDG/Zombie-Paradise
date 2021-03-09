using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Utilities.Extensions;
using Utility;
public class CameraController : MonoBehaviour
{
    public float speed = 1f;
    public float zoomSpeed = 10f;

    [SerializeField] private CinemachineVirtualCamera vPersCam, vTopCam, vGraveyardCam, vCreationCam, vResearchCam;
    [SerializeField] private GameObject creationCollection;
    [SerializeField] private VolumeProfile volumeProfile;
    [SerializeField] private RawImage fgImg;

    private Vector3 _translateVec;
    private Vector3 _targetRotation;
    private float _targetZoomDirection, _minZoom = 1f, _maxZoom = 75f;
    private bool _isZoomedOut, _onGraveyard, _onCreation;

    private CinemachineOrbitalTransposer _body;
    private Button3D _lastButt;
    private DepthOfField _dofComponent;

    public void Awake()
    {
        _body = (CinemachineOrbitalTransposer)vPersCam.GetCinemachineComponent(CinemachineCore.Stage.Body);

        volumeProfile.TryGet(out _dofComponent);
    }

    private void Start()
    {
        _dofComponent.focalLength.value = 1f;
    }

    private void Update()
    {
        transform.Translate(_translateVec * Time.deltaTime);

        if (_onGraveyard)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out var hit, LayerMask.GetMask("Button3D")))
            {
                if (hit.transform.TryGetComponent(out _lastButt))
                {
                    if (Mouse.current.leftButton.isPressed)
                    {
                        _lastButt.SwitchButtonMesh(ButtonState.Pressed);
                        return;
                    }
                    _lastButt.SwitchButtonMesh(ButtonState.Hovered);
                    return;
                }
            }

            if (_lastButt != null) _lastButt.SwitchButtonMesh(ButtonState.None);
        }
    }

    public void OnTranslateInput(InputAction.CallbackContext context)
    {
        _translateVec = context.ReadValue<Vector2>().TransformTo2DVector3() * speed;
    }

    public void OnRotateInput(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        var rotateDirection = -context.ReadValue<float>();

        _targetRotation = new Vector3(0, 90f, 0) * rotateDirection + transform.rotation.eulerAngles;

        transform.DORotate(_targetRotation, 0.2f).Play();
    }

    public void OnZoomInput(InputAction.CallbackContext context)
    {
        _targetZoomDirection = -context.ReadValue<float>().Clamp(-1, 1);

        _body.m_FollowOffset.y = (_body.m_FollowOffset.y + _targetZoomDirection * zoomSpeed * Time.fixedDeltaTime).Clamp(_minZoom, _maxZoom);
    }

    public void OnChangeCamMapInput(InputAction.CallbackContext context)
    {
        if (_onGraveyard) return;
        if (!context.started) return;
        if (_isZoomedOut)
        {
            vPersCam.Priority = 50;
            vTopCam.Priority = 0;

            DOTween.To(() => RenderSettings.fogDensity, res => RenderSettings.fogDensity = res, MapData.FogLoDensity, 1f);
        }
        else
        {
            vPersCam.Priority = 0;
            vTopCam.Priority = 50;

            DOTween.To(() => RenderSettings.fogDensity, res => RenderSettings.fogDensity = res, 0, 1f);
        }

        _isZoomedOut = !_isZoomedOut;
    }

    public void OnChangeCamGraveyardInput(InputAction.CallbackContext context)
    {
        if (_isZoomedOut || _onCreation) return;
        if (!context.started) return;
        if (_onGraveyard)
        {
            RenderSettings.fogDensity = MapData.FogLoDensity;
            vGraveyardCam.Priority = 0;
        }
        else
        {
            RenderSettings.fogDensity = MapData.FogHiDensity;
            vGraveyardCam.Priority = 100;
        }

        fgImg.DOFade(1f, 0.5f).OnComplete(() => fgImg.DOFade(0f, 0.1f));
        _onGraveyard = !_onGraveyard;
    }

    public void OnChangeCamCreationClick()
    {
        if (!_onGraveyard) return;

        if (_onCreation)
        {
            vCreationCam.Priority = 0;
            vGraveyardCam.Priority = 100;

            UnityExtensions.DelayAction(this, () =>
            {
                DOTween.To(() => _dofComponent.focalLength.value, res => _dofComponent.focalLength.value = res, 1f, 0.1f);
                creationCollection.SetActive(false);
            }, 0.1f);
        }
        else
        {
            creationCollection.SetActive(true);
            
            vCreationCam.Priority = 100;
            vGraveyardCam.Priority = 0;

            UnityExtensions.DelayAction(this, () =>
            {
                DOTween.To(() => _dofComponent.focalLength.value, res => _dofComponent.focalLength.value = res, 32f, 0.1f);
            }, 0.8f);
        }

        _onCreation = !_onCreation;
    }
}