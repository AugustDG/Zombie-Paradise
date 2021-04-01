// GENERATED AUTOMATICALLY FROM 'Assets/Input/MainControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @MainControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @MainControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""MainControls"",
    ""maps"": [
        {
            ""name"": ""Map"",
            ""id"": ""def93e29-dfe2-4165-b868-65fbfa44eac7"",
            ""actions"": [
                {
                    ""name"": ""MoveMap"",
                    ""type"": ""Value"",
                    ""id"": ""60db56e9-cd12-44d8-bde9-b05db3fb96a6"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RotateMap"",
                    ""type"": ""Value"",
                    ""id"": ""b5400eb9-751a-4532-b2d6-9b5435c5b122"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ZoomMap"",
                    ""type"": ""Value"",
                    ""id"": ""ce0fe5ca-25be-4fc1-8b1a-0e2b1f5664a4"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""GraveyardCamChange"",
                    ""type"": ""Button"",
                    ""id"": ""56d2d7fe-ccd1-47a4-864f-1f3e80e046fd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ZoomOutCamChange"",
                    ""type"": ""Button"",
                    ""id"": ""399df751-7b0c-4995-9a1d-45dfebffd8ed"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Click"",
                    ""type"": ""Button"",
                    ""id"": ""0baeb567-5c2a-47f9-a987-647e29d0db0c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""529f00bb-9fd9-4496-af04-76bd6f36ba9d"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveMap"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""e3196ece-ab0a-496b-bce3-abb16e32246e"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""MoveMap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""bf86d7f7-c763-4619-a7e4-0a2125dd311c"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""MoveMap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""b4aadf29-c637-4a2c-86b1-7d3c6b27f3e0"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""MoveMap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""ec09738f-99a5-43ad-b8df-fab653051e74"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""MoveMap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""ARROWS"",
                    ""id"": ""7f4436a5-2bec-4d68-a275-438b3cca8ecb"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveMap"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""8d9f709d-5bc5-423b-b14b-38267adbb716"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""MoveMap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""305af880-5f17-42d2-8fe4-78c916e013ed"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveMap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""34da5d6f-4ced-4474-95e2-cd37ea03b01d"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""MoveMap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""b71cb1fb-75ac-4023-ae07-78e13090a6e7"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""MoveMap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""QE"",
                    ""id"": ""e72c5434-f1d2-4079-bc5e-362b96d10997"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RotateMap"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""42e958cd-b969-47c4-9556-023f1fbaa172"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""RotateMap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""a59a1a7c-cf16-4936-ab90-ec6f84c41407"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""RotateMap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""3991892f-0cd4-491e-ae27-dc84ad733521"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""ZoomMap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f5b67a46-edfd-4346-aaf5-e73fc44624cd"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""GraveyardCamChange"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b7592517-db2d-4202-9838-8b64a1c4fab1"",
                    ""path"": ""<Keyboard>/alt"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ZoomOutCamChange"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""be4c7b26-ccfe-4429-ab8f-eea4ed8a9861"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""KeyboardMouse"",
            ""bindingGroup"": ""KeyboardMouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Map
        m_Map = asset.FindActionMap("Map", throwIfNotFound: true);
        m_Map_MoveMap = m_Map.FindAction("MoveMap", throwIfNotFound: true);
        m_Map_RotateMap = m_Map.FindAction("RotateMap", throwIfNotFound: true);
        m_Map_ZoomMap = m_Map.FindAction("ZoomMap", throwIfNotFound: true);
        m_Map_GraveyardCamChange = m_Map.FindAction("GraveyardCamChange", throwIfNotFound: true);
        m_Map_ZoomOutCamChange = m_Map.FindAction("ZoomOutCamChange", throwIfNotFound: true);
        m_Map_Click = m_Map.FindAction("Click", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Map
    private readonly InputActionMap m_Map;
    private IMapActions m_MapActionsCallbackInterface;
    private readonly InputAction m_Map_MoveMap;
    private readonly InputAction m_Map_RotateMap;
    private readonly InputAction m_Map_ZoomMap;
    private readonly InputAction m_Map_GraveyardCamChange;
    private readonly InputAction m_Map_ZoomOutCamChange;
    private readonly InputAction m_Map_Click;
    public struct MapActions
    {
        private @MainControls m_Wrapper;
        public MapActions(@MainControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @MoveMap => m_Wrapper.m_Map_MoveMap;
        public InputAction @RotateMap => m_Wrapper.m_Map_RotateMap;
        public InputAction @ZoomMap => m_Wrapper.m_Map_ZoomMap;
        public InputAction @GraveyardCamChange => m_Wrapper.m_Map_GraveyardCamChange;
        public InputAction @ZoomOutCamChange => m_Wrapper.m_Map_ZoomOutCamChange;
        public InputAction @Click => m_Wrapper.m_Map_Click;
        public InputActionMap Get() { return m_Wrapper.m_Map; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MapActions set) { return set.Get(); }
        public void SetCallbacks(IMapActions instance)
        {
            if (m_Wrapper.m_MapActionsCallbackInterface != null)
            {
                @MoveMap.started -= m_Wrapper.m_MapActionsCallbackInterface.OnMoveMap;
                @MoveMap.performed -= m_Wrapper.m_MapActionsCallbackInterface.OnMoveMap;
                @MoveMap.canceled -= m_Wrapper.m_MapActionsCallbackInterface.OnMoveMap;
                @RotateMap.started -= m_Wrapper.m_MapActionsCallbackInterface.OnRotateMap;
                @RotateMap.performed -= m_Wrapper.m_MapActionsCallbackInterface.OnRotateMap;
                @RotateMap.canceled -= m_Wrapper.m_MapActionsCallbackInterface.OnRotateMap;
                @ZoomMap.started -= m_Wrapper.m_MapActionsCallbackInterface.OnZoomMap;
                @ZoomMap.performed -= m_Wrapper.m_MapActionsCallbackInterface.OnZoomMap;
                @ZoomMap.canceled -= m_Wrapper.m_MapActionsCallbackInterface.OnZoomMap;
                @GraveyardCamChange.started -= m_Wrapper.m_MapActionsCallbackInterface.OnGraveyardCamChange;
                @GraveyardCamChange.performed -= m_Wrapper.m_MapActionsCallbackInterface.OnGraveyardCamChange;
                @GraveyardCamChange.canceled -= m_Wrapper.m_MapActionsCallbackInterface.OnGraveyardCamChange;
                @ZoomOutCamChange.started -= m_Wrapper.m_MapActionsCallbackInterface.OnZoomOutCamChange;
                @ZoomOutCamChange.performed -= m_Wrapper.m_MapActionsCallbackInterface.OnZoomOutCamChange;
                @ZoomOutCamChange.canceled -= m_Wrapper.m_MapActionsCallbackInterface.OnZoomOutCamChange;
                @Click.started -= m_Wrapper.m_MapActionsCallbackInterface.OnClick;
                @Click.performed -= m_Wrapper.m_MapActionsCallbackInterface.OnClick;
                @Click.canceled -= m_Wrapper.m_MapActionsCallbackInterface.OnClick;
            }
            m_Wrapper.m_MapActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MoveMap.started += instance.OnMoveMap;
                @MoveMap.performed += instance.OnMoveMap;
                @MoveMap.canceled += instance.OnMoveMap;
                @RotateMap.started += instance.OnRotateMap;
                @RotateMap.performed += instance.OnRotateMap;
                @RotateMap.canceled += instance.OnRotateMap;
                @ZoomMap.started += instance.OnZoomMap;
                @ZoomMap.performed += instance.OnZoomMap;
                @ZoomMap.canceled += instance.OnZoomMap;
                @GraveyardCamChange.started += instance.OnGraveyardCamChange;
                @GraveyardCamChange.performed += instance.OnGraveyardCamChange;
                @GraveyardCamChange.canceled += instance.OnGraveyardCamChange;
                @ZoomOutCamChange.started += instance.OnZoomOutCamChange;
                @ZoomOutCamChange.performed += instance.OnZoomOutCamChange;
                @ZoomOutCamChange.canceled += instance.OnZoomOutCamChange;
                @Click.started += instance.OnClick;
                @Click.performed += instance.OnClick;
                @Click.canceled += instance.OnClick;
            }
        }
    }
    public MapActions @Map => new MapActions(this);
    private int m_KeyboardMouseSchemeIndex = -1;
    public InputControlScheme KeyboardMouseScheme
    {
        get
        {
            if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("KeyboardMouse");
            return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
        }
    }
    public interface IMapActions
    {
        void OnMoveMap(InputAction.CallbackContext context);
        void OnRotateMap(InputAction.CallbackContext context);
        void OnZoomMap(InputAction.CallbackContext context);
        void OnGraveyardCamChange(InputAction.CallbackContext context);
        void OnZoomOutCamChange(InputAction.CallbackContext context);
        void OnClick(InputAction.CallbackContext context);
    }
}
