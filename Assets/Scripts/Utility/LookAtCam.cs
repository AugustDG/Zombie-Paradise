using UnityEngine;

namespace Utility
{
    public class LookAtCam : MonoBehaviour
    {
        public Vector3 upDirection;
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void FixedUpdate()
        {
            transform.LookAt(_mainCamera.transform,upDirection);
        }
    }
}