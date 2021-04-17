using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using Utilities.Extensions;
using Utility;
public class CameraController : MonoBehaviour
{
    public float speed = 1f;
    public float zoomSpeed = 10f;

    [SerializeField] private CinemachineVirtualCamera vPersCam, vTopCam, vGraveyardCam, vCreationCam, vResearchCam;
    [SerializeField] private RectTransform currencyColection;
    [SerializeField] private RectTransform creationCollection, researchCollection;
    [SerializeField] private VolumeProfile volumeProfile;

    private Vector3 _translateVec;
    private Vector3 _targetRotation;
    private float _targetZoomDirection, _minZoom = 1f, _maxZoom = 75f;
    private bool _isZoomedOut, _onGraveyard, _onCreation, _onResearch;

    private CinemachineOrbitalTransposer _body;
    private Button3D _lastButt;
    //private DepthOfField _dofComponent;

    public void Awake()
    {
        _body = (CinemachineOrbitalTransposer)vPersCam.GetCinemachineComponent(CinemachineCore.Stage.Body);

        //volumeProfile.TryGet(out _dofComponent);
    }

    private void Start()
    {
        //_dofComponent.gaussianStart.value = 300f;
        
        
    }

    private void Update()
    {
        transform.Translate(_translateVec * Time.deltaTime);

        //transform.position = Vector3.Slerp(transform.position, transform.position + _translateVec, Time.deltaTime);
        
        //transform.GetComponent<Rigidbody>().AddForce(_translateVec * 0.5f, ForceMode.VelocityChange);

        if (_onGraveyard)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out var hit, LayerMask.GetMask("Button3D")))
            {
                if (hit.transform.TryGetComponent(out _lastButt))
                {
                    if (Mouse.current.leftButton.isPressed)
                    {
                        _lastButt.SwitchButtonMesh(Button3DState.Pressed);
                        return;
                    }
                    _lastButt.SwitchButtonMesh(Button3DState.Hovered);
                    return;
                }
            }

            if (_lastButt != null) _lastButt.SwitchButtonMesh(Button3DState.None);
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
        
        MapEvents.ChangedToTopViewEvent.Invoke(this, !_isZoomedOut);
        
        _isZoomedOut = !_isZoomedOut;
    }

    public void OnChangeCamGraveyardInput(InputAction.CallbackContext context)
    {
        if (_isZoomedOut || _onCreation) return;
        if (!context.started) return;
        if (_onGraveyard)
        {
            Time.timeScale = 1f;
            currencyColection.DOAnchorPosY(0f, 1.5f, true);

            RenderSettings.fogDensity = MapData.FogLoDensity;
            vGraveyardCam.Priority = 0;
        }
        else
        {
            Time.timeScale = 0f;
            currencyColection.DOAnchorPosY(-400f, 1.5f, true);
            
            RenderSettings.fogDensity = MapData.FogHiDensity;
            vGraveyardCam.Priority = 100;
        }
        
        MapData.CanSpawnZombies = _onGraveyard;
        MapEvents.ChangedToGraveyardEvent.Invoke(this, !_onGraveyard);
        
        _onGraveyard = !_onGraveyard;
    }

    public void OnChangeCamCreationClick()
    {
        if (!_onGraveyard) return;

        if (_onCreation)
        {
            vCreationCam.Priority = 0;
            vGraveyardCam.Priority = 100;

            creationCollection.DOAnchorPosY(2500, 1.5f, true);
            
            /*UnityExtensions.DelayAction(this, () =>
            {
                //DOTween.To(() => _dofComponent.gaussianStart.value, res => _dofComponent.gaussianStart.value = res, 300f, 0.1f);
            }, 0.1f);*/
        }
        else
        {
            vCreationCam.Priority = 100;
            vGraveyardCam.Priority = 0;

            creationCollection.DOAnchorPosY(0f, 1.5f, true);
            
            /*UnityExtensions.DelayAction(this, () =>
            {
                //DOTween.To(() => _dofComponent.gaussianStart.value, res => _dofComponent.gaussianStart.value = res, 2f, 0.1f);
            }, 0.8f);*/
        }

        _onCreation = !_onCreation;
    }
    
    public void OnChangeCamResearchClick()
    {
        if (!_onGraveyard) return;

        if (_onResearch)
        {
            vResearchCam.Priority = 0;
            vGraveyardCam.Priority = 100;

            researchCollection.DOAnchorPosY(-2500, 1.5f, true);
            
            /*UnityExtensions.DelayAction(this, () =>
            {
                //DOTween.To(() => _dofComponent.gaussianStart.value, res => _dofComponent.gaussianStart.value = res, 300f, 0.1f);
            }, 0.1f);*/
        }
        else
        {
            vResearchCam.Priority = 100;
            vGraveyardCam.Priority = 0;

            researchCollection.DOAnchorPosY(0f, 1.5f, true);
            
            /*UnityExtensions.DelayAction(this, () =>
            {
                //DOTween.To(() => _dofComponent.gaussianStart.value, res => _dofComponent.gaussianStart.value = res, 2f, 0.1f);
            }, 0.8f);*/
        }

        _onResearch = !_onResearch;
    }
}