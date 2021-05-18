/// Credit drHogan 
/// Sourced from - http://forum.unity3d.com/threads/screenspace-camera-tooltip-controller-sweat-and-tears.293991/#post-1938929
/// updated simonDarksideJ - refactored code to be more performant.
/// updated lucasvinbr - mixed with BoundTooltip, should work with Screenspace Camera (non-rotated) and Overlay
/// *Note - only works for non-rotated Screenspace Camera and Screenspace Overlay canvases at present, needs updating to include rotated Screenspace Camera and Worldspace!

//ToolTip is written by Emiliano Pastorelli, H&R Tallinn (Estonia), http://www.hammerandravens.com
//Copyright (c) 2015 Emiliano Pastorelli, H&R - Hammer&Ravens, Tallinn, Estonia.
//All rights reserved.

//Redistribution and use in source and binary forms are permitted
//provided that the above copyright notice and this paragraph are
//duplicated in all such forms and that any documentation,
//advertising materials, and other materials related to such
//distribution and use acknowledge that the software was developed
//by H&R, Hammer&Ravens. The name of the
//H&R, Hammer&Ravens may not be used to endorse or promote products derived
//from this software without specific prior written permission.
//THIS SOFTWARE IS PROVIDED ``AS IS'' AND WITHOUT ANY EXPRESS OR
//IMPLIED WARRANTIES, INCLUDING, WITHOUT LIMITATION, THE IMPLIED
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.

using TMPro;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("UI/Extensions/Tooltip/Tooltip")]
    public class ToolTip : MonoBehaviour
    {
        //text of the tooltip
        private TMP_Text _text;
        private RectTransform _rectTransform, _canvasRectTransform;

        [Tooltip("The canvas used by the tooltip as positioning and scaling reference. Should usually be the root Canvas of the hierarchy this component is in")]
        public Canvas canvas;

        [Tooltip("Sets if tooltip triggers will run ForceUpdateCanvases and refresh the tooltip's layout group " +
            "(if any) when hovered, in order to prevent momentousness misplacement sometimes caused by ContentSizeFitters")]
        public bool tooltipTriggersCanForceCanvasUpdate = false;

        /// <summary>
        /// the tooltip's Layout Group, if any
        /// </summary>
        private LayoutGroup _layoutGroup;

        //if the tooltip is inside a UI element
        private bool _inside;

        private float _width, _height;//, canvasWidth, canvasHeight;

        public float yShift,xShift;

        [HideInInspector]
        public RenderMode guiMode;

        private Camera _guiCamera;

        public Camera GuiCamera
        {
            get
            {
                if (!_guiCamera) {
                    _guiCamera = Camera.main;
                }

                return _guiCamera;
            }
        }

        private Vector3 _screenLowerLeft, _screenUpperRight, _shiftingVector;

        /// <summary>
        /// a screen-space point where the tooltip would be placed before applying X and Y shifts and border checks
        /// </summary>
        private Vector3 _baseTooltipPos;

        private Vector3 _newTtPos;
        private Vector3 _adjustedNewTtPos;
        private Vector3 _adjustedTtLocalPos;
        private Vector3 _shifterForBorders;

        private float _borderTest;

        // Standard Singleton Access
        private static ToolTip _instance;
        
        public static ToolTip Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<ToolTip>();
                return _instance;
            }
        }

        
        void Reset() {
            canvas = GetComponentInParent<Canvas>();
            canvas = canvas.rootCanvas;
        }

        // Use this for initialization
        public void Awake()
        {
            _instance = this;
            if (!canvas) {
                canvas = GetComponentInParent<Canvas>();
                canvas = canvas.rootCanvas;
            }

            _guiCamera = canvas.worldCamera;
            guiMode = canvas.renderMode;
            _rectTransform = GetComponent<RectTransform>();
            _canvasRectTransform = canvas.GetComponent<RectTransform>();
            _layoutGroup = GetComponentInChildren<LayoutGroup>();

            _text = GetComponentInChildren<TMP_Text>();

            _inside = false;

            gameObject.SetActive(false);
        }

        //Call this function externally to set the text of the template and activate the tooltip
        public void SetTooltip(string ttext, Vector3 basePos, bool refreshCanvasesBeforeGetSize = false)
        {
            _baseTooltipPos = basePos;

            //set the text
            if (_text) {
                _text.text = ttext;
            }
            else {
                Debug.LogWarning("[ToolTip] Couldn't set tooltip text, tooltip has no child Text component");
            }

            ContextualTooltipUpdate(refreshCanvasesBeforeGetSize);
        }

        //call this function on mouse exit to deactivate the template
        public void HideTooltip()
        {
            gameObject.SetActive(false);
            _inside = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (_inside)
            {
                ContextualTooltipUpdate();
            }
        }

        /// <summary>
        /// forces rebuilding of Canvases in order to update the tooltip's content size fitting.
        /// Can prevent the tooltip from being visibly misplaced for one frame when being resized.
        /// Only runs if tooltipTriggersCanForceCanvasUpdate is true
        /// </summary>
        public void RefreshTooltipSize() {
            if (tooltipTriggersCanForceCanvasUpdate) {
                Canvas.ForceUpdateCanvases();

                if (_layoutGroup) {
                    _layoutGroup.enabled = false;
                    _layoutGroup.enabled = true;
                }
                
            }
            
        }

        /// <summary>
        /// Runs the appropriate tooltip placement method, according to the parent canvas's render mode
        /// </summary>
        /// <param name="refreshCanvasesBeforeGettingSize"></param>
        public void ContextualTooltipUpdate(bool refreshCanvasesBeforeGettingSize = false) {
            switch (guiMode) {
                case RenderMode.ScreenSpaceCamera:
                    OnScreenSpaceCamera(refreshCanvasesBeforeGettingSize);
                    break;
                case RenderMode.ScreenSpaceOverlay:
                    OnScreenSpaceOverlay(refreshCanvasesBeforeGettingSize);
                    break;
            }
        }

        //main tooltip edge of screen guard and movement - camera
        public void OnScreenSpaceCamera(bool refreshCanvasesBeforeGettingSize = false)
        {
            _shiftingVector.x = xShift;
            _shiftingVector.y = yShift;

            _baseTooltipPos.z = canvas.planeDistance;

            _newTtPos = GuiCamera.ScreenToViewportPoint(_baseTooltipPos - _shiftingVector);
            _adjustedNewTtPos = GuiCamera.ViewportToWorldPoint(_newTtPos);

            gameObject.SetActive(true);

            if (refreshCanvasesBeforeGettingSize) RefreshTooltipSize();

            //consider scaled dimensions when comparing against the edges
            _width = transform.lossyScale.x * _rectTransform.sizeDelta[0];
            _height = transform.lossyScale.y * _rectTransform.sizeDelta[1];

            // check and solve problems for the tooltip that goes out of the screen on the horizontal axis

            RectTransformUtility.ScreenPointToWorldPointInRectangle(_canvasRectTransform, Vector2.zero, GuiCamera, out _screenLowerLeft);
            RectTransformUtility.ScreenPointToWorldPointInRectangle(_canvasRectTransform, new Vector2(Screen.width, Screen.height), GuiCamera, out _screenUpperRight);


            //check for right edge of screen
            _borderTest = _adjustedNewTtPos.x + _width / 2;
            if (_borderTest > _screenUpperRight.x)
            {
                _shifterForBorders.x = _borderTest - _screenUpperRight.x;
                _adjustedNewTtPos.x -= _shifterForBorders.x;
            }
            //check for left edge of screen
            _borderTest = _adjustedNewTtPos.x - _width / 2;
            if (_borderTest < _screenLowerLeft.x)
            {
                _shifterForBorders.x = _screenLowerLeft.x - _borderTest;
                _adjustedNewTtPos.x += _shifterForBorders.x;
            }

            // check and solve problems for the tooltip that goes out of the screen on the vertical axis

            //check for lower edge of the screen
            _borderTest = _adjustedNewTtPos.y - _height / 2;
            if (_borderTest < _screenLowerLeft.y) {
                _shifterForBorders.y = _screenLowerLeft.y - _borderTest;
                _adjustedNewTtPos.y += _shifterForBorders.y;
            }

            //check for upper edge of the screen
            _borderTest = _adjustedNewTtPos.y + _height / 2;
            if (_borderTest > _screenUpperRight.y)
            {
                _shifterForBorders.y = _borderTest - _screenUpperRight.y;
                _adjustedNewTtPos.y -= _shifterForBorders.y;
            }

            //failed attempt to circumvent issues caused when rotating the camera
            _adjustedNewTtPos = transform.rotation * _adjustedNewTtPos;

            transform.position = _adjustedNewTtPos;
            _adjustedTtLocalPos = transform.localPosition;
            _adjustedTtLocalPos.z = 0;
            transform.localPosition = _adjustedTtLocalPos;

            _inside = true;
        }


        //main tooltip edge of screen guard and movement - overlay
        public void OnScreenSpaceOverlay(bool refreshCanvasesBeforeGettingSize = false) {
            _shiftingVector.x = xShift;
            _shiftingVector.y = yShift;
            _newTtPos = (_baseTooltipPos - _shiftingVector) / canvas.scaleFactor;
            _adjustedNewTtPos = _newTtPos;

            gameObject.SetActive(true);

            if (refreshCanvasesBeforeGettingSize) RefreshTooltipSize();

            _width = _rectTransform.sizeDelta[0];
            _height = _rectTransform.sizeDelta[1];

            // check and solve problems for the tooltip that goes out of the screen on the horizontal axis
            //screen's 0 = overlay canvas's 0 (always?)
            _screenLowerLeft = Vector3.zero;
            _screenUpperRight = _canvasRectTransform.sizeDelta;

            //check for right edge of screen
            _borderTest = _newTtPos.x + _width / 2;
            if (_borderTest > _screenUpperRight.x) {
                _shifterForBorders.x = _borderTest - _screenUpperRight.x;
                _adjustedNewTtPos.x -= _shifterForBorders.x;
            }
            //check for left edge of screen
            _borderTest = _adjustedNewTtPos.x - _width / 2;
            if (_borderTest < _screenLowerLeft.x) {
                _shifterForBorders.x = _screenLowerLeft.x - _borderTest;
                _adjustedNewTtPos.x += _shifterForBorders.x;
            }

            // check and solve problems for the tooltip that goes out of the screen on the vertical axis

            //check for lower edge of the screen
            _borderTest = _adjustedNewTtPos.y - _height / 2;
            if (_borderTest < _screenLowerLeft.y) {
                _shifterForBorders.y = _screenLowerLeft.y - _borderTest;
                _adjustedNewTtPos.y += _shifterForBorders.y;
            }

            //check for upper edge of the screen
            _borderTest = _adjustedNewTtPos.y + _height / 2;
            if (_borderTest > _screenUpperRight.y) {
                _shifterForBorders.y = _borderTest - _screenUpperRight.y;
                _adjustedNewTtPos.y -= _shifterForBorders.y;
            }

            //remove scale factor for the actual positioning of the TT
            _adjustedNewTtPos *= canvas.scaleFactor;
            transform.position = _adjustedNewTtPos;

            _inside = true;
        }
    }
}