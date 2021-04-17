/// Credit Alastair Aitchison
/// Sourced from - https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/issues/123/uilinerenderer-issues-with-specifying

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/UI Line Rect Connector")]
    [RequireComponent(typeof(UILineRenderer))]
    [ExecuteInEditMode]
    public class UILineRectConnector : MonoBehaviour
    {

        // The elements between which line segments should be drawn
        public RectTransform[] transforms;
        private Vector2[] _previousPositions;
        private RectTransform _canvas;
        private RectTransform _rt;
        private UILineRenderer _lr;

        private void Awake()
        {
            _canvas = GetComponentInParent<RectTransform>().GetParentCanvas().GetComponent<RectTransform>();
            _rt = GetComponent<RectTransform>();
            _lr = GetComponent<UILineRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            if (transforms == null || transforms.Length < 1)
            {
                return;
            }
            //Performance check to only redraw when the child transforms move
            if (_previousPositions != null && _previousPositions.Length == transforms.Length)
            {
                var updateLine = false;
                for (var i = 0; i < transforms.Length; i++)
                {
                    if (!updateLine && _previousPositions[i] != transforms[i].anchoredPosition)
                    {
                        updateLine = true;
                    }
                }
                if (!updateLine) return;
            }

            // Get the pivot points
            var thisPivot = _rt.pivot;
            var canvasPivot = _canvas.pivot;

            // Set up some arrays of coordinates in various reference systems
            var worldSpaces = new Vector3[transforms.Length];
            var canvasSpaces = new Vector3[transforms.Length];
            var points = new Vector2[transforms.Length];
            

            // Calculate delta from the canvas pivot point
            for (var i = 0; i < transforms.Length; i++)
            {
                points[i] = new Vector2(transforms[i].anchoredPosition.x, transforms[i].anchoredPosition.y);
            }

            // And assign the converted points to the line renderer
            _lr.Points = points;
            _lr.RelativeSize = false;
            _lr.drivenExternally = true;

            _previousPositions = new Vector2[transforms.Length];
            for (var i = 0; i < transforms.Length; i++)
            {
                _previousPositions[i] = transforms[i].anchoredPosition;
            }
        }
    }
}