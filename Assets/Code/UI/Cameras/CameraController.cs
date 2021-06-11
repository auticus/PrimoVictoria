using UnityEngine;
using System.Collections.Generic;

namespace PrimoVictoria.UI.Cameras
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        /*
         * Movement defaults to the WASD keys
         * If mouse movement enabled, moving mouse to edge of screen will move the screen as well
         * Q & E will rotate the camera as will holding middle mouse button and moving the mouse
         * mouse wheel will zoom in and out
         * Holding mouse scrollwheel down will pan left and right or up and down, though left and right takes precedence unless you hold the left SHIFT key down
         * TODO: the ability to bring the mouse to center on a unit
         */
        [SerializeField] private float CameraMovementSpeed = 25.0f;
        [SerializeField] private float CameraRotationSpeed = 50.0f;
        [SerializeField] private float CameraZoomSpeed = 100.0f;
        [SerializeField] private float MinCameraTilt = 0.0f;
        [SerializeField] private float MaxCameraTilt = 90.0f;
        [SerializeField] private float MinCameraZoom = 4.0f;
        [SerializeField] private float MaxCameraZoom = 50.0f;
        [SerializeField] private float StartingCameraTilt = 45.0f;
        [SerializeField] private float FieldOfView = 60.0f;
        [SerializeField] [Tooltip("How many units from edge of screen will trigger mouse movement")] private float MouseMoveBorderEdgeScreen = 10.0f;
        [SerializeField] private bool MouseMovementEnabled = true;
        [SerializeField] [Tooltip("Enable to have the speed of camera movement speed up the closer you get to the screen edge")] private bool MouseMoveAccelerationEnabled = true;
        [SerializeField] [Tooltip("How far the camera can go in X or Y coordinates")] private Vector2 MapLimit = new Vector2(60, 60);
        [SerializeField] [Tooltip("The LayerMasks that represent the ground or anything else that can affect camera height")] private List<LayerMask> GroundMasks = new List<LayerMask>() { -1 };

        private Vector2 MouseAxis
        {
            get { return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); }
        }

        private float _currentCameraRotation = 0.0f;
        private float _currentCameraTilt = 0.0f;

        #region Unity Methods
        void Start()
        {
            transform.eulerAngles = new Vector3(StartingCameraTilt, 0, 0);
            _currentCameraTilt = StartingCameraTilt;
            var camera = FindObjectOfType<Camera>();
            camera.fieldOfView = FieldOfView;
        }

        void LateUpdate()
        {
            HandleAxisMovement();

            //if you let the user adjust both the tilt and the rotation at the same time, the camera can get really jacked up on its side potentially
            //only let them tilt if they did not rotate
            if (HandleRotation() == false)
                HandleTilt();
        }
        #endregion Unity Methods

        /// <summary>
        /// Handles the movement of the camera along the x and z axis
        /// </summary>
        private void HandleAxisMovement()
        {
            var xAxis = Input.GetAxis("Horizontal");
            var zAxis = Input.GetAxis("Vertical");
            var pos = transform.position;

            //the mouse axis will override the keyboard
            if (MouseMovementEnabled)
                HandleMouseMovement(ref xAxis, ref zAxis);

            var x = xAxis * CameraMovementSpeed * Time.deltaTime;
            var z = zAxis * CameraMovementSpeed * Time.deltaTime;
            var y = CalculateHeight(pos.y);

            pos.x += x;
            pos.z += z;
            pos.y -= y; //inverted because zooming in is a positive value (decrementing Y) but zooming out is negative (incrementing Y)

            pos.x = Mathf.Clamp(pos.x, -MapLimit.x, MapLimit.x);
            pos.z = Mathf.Clamp(pos.z, -MapLimit.y, MapLimit.y);
            pos.y = Mathf.Clamp(pos.y, MinCameraZoom, MaxCameraZoom);

            transform.position = pos;
        }

        private float _groundDistance;
        private float CalculateHeight(float currentY)
        {
            var scroll = Input.GetAxis("Mouse ScrollWheel");
            var standardHeightCalculation = scroll * CameraZoomSpeed * Time.deltaTime;

            //it could be that terrain is now raised enough where the camera is into the terrain.  We need to adjust for that and bring the camera up to avoid eating dirt
            var distanceToGround = DistanceToGround();

            if (Mathf.Abs(distanceToGround - _groundDistance) >= float.Epsilon)
            {
                _groundDistance = distanceToGround;
            }

            //if current ground distance is less than zoom (we climbed elevation with the camera by moving on the axis and hit a hill or something) then bring it up
            if (_groundDistance < MinCameraZoom)
            {
                standardHeightCalculation = -(MinCameraZoom - _groundDistance);
            }

            //now if the adjustment will take the ground distance below the min camera zoom, it needs adjusted
            if (_groundDistance - standardHeightCalculation < MinCameraZoom)
            {
                standardHeightCalculation = 0.0f;  //we will not perform this action
            }

            return standardHeightCalculation;
        }

        /// <summary>
        /// Calculate the distance to the ground or highest game object in the GroundMask list from the game object this is attached to
        /// </summary>
        /// <returns></returns>
        private float DistanceToGround()
        {
            Ray ray = new Ray(transform.position, Vector3.down);
            RaycastHit hit;
            var shortestMagnitude = 0.0f; //we want the shortest raycast since that represents the tallest item (the distance would be the shortest of them all from here to there)
            foreach (var groundMask in GroundMasks)
            {
                if (Physics.Raycast(ray, out hit, groundMask.value))
                {
                    var magnitude = (hit.point - transform.position).magnitude;
                    if (magnitude < shortestMagnitude || shortestMagnitude <= float.Epsilon)
                    {
                        shortestMagnitude = magnitude;
                    }
                }
            }

            return shortestMagnitude;
        }

        /// <summary>
        /// Handles all movement related to the mouse
        /// </summary>
        /// <param name="xAxis"></param>
        /// <param name="zAxis"></param>
        private void HandleMouseMovement(ref float xAxis, ref float zAxis)
        {
            if (xAxis > float.Epsilon && zAxis > float.Epsilon)
                return;
            var x = Mathf.Clamp(Input.mousePosition.x, 0, Screen.width);
            var y = Mathf.Clamp(Input.mousePosition.y, 0, Screen.height);

            //the speed of the move should accelerate the closer you are to the edge
            if (x >= Screen.width - MouseMoveBorderEdgeScreen)
            {
                if (MouseMoveAccelerationEnabled)
                {
                    xAxis = (Screen.width - x) / MouseMoveBorderEdgeScreen;  //get me a value that is a percentage of how far in the x is to the edge (ie:  0.5, 0.62, 1.0) 
                }
                else
                    xAxis = 1;
            }
            if (x <= MouseMoveBorderEdgeScreen)
            {
                if (MouseMoveAccelerationEnabled)
                {
                    x = Mathf.Clamp(MouseMoveBorderEdgeScreen - x, 1.0f, MouseMoveBorderEdgeScreen);
                    xAxis = -(x / MouseMoveBorderEdgeScreen);
                }
                else
                    xAxis = -1;
            }
            if (y >= Screen.height - MouseMoveBorderEdgeScreen)
            {
                if (MouseMoveAccelerationEnabled)
                {
                    zAxis = (Screen.height - y) / MouseMoveBorderEdgeScreen;
                }
                else
                    zAxis = 1;
            }
            if (y <= MouseMoveBorderEdgeScreen)
            {
                if (MouseMoveAccelerationEnabled)
                {
                    //coming in from the top, we need to reverse the value so a value of 10 on a border of 10 should be the same as being a "0" (just starting to move) and the value of 0 is a 10 as its on the bottom
                    y = Mathf.Clamp(MouseMoveBorderEdgeScreen - y, 1.0f, MouseMoveBorderEdgeScreen);
                    zAxis = -(y / MouseMoveBorderEdgeScreen);
                }
                else
                    zAxis = -1;
            }
        }

        /// <summary>
        /// Handles the rotation, will return TRUE if a rotation has occurred.  Will not rotate at all if the left SHIFT button is being held down (will always return FALSE)
        /// </summary>
        /// <returns>TRUE if a rotation has occurred</returns>
        private bool HandleRotation()
        {
            if (Input.GetKey(KeyCode.LeftShift))
                return false;

            float rotation = 0.0f;
            if (MouseMovementEnabled && Input.GetKey(KeyCode.Mouse2))
            {
                rotation = -MouseAxis.x;
            }
            else
            {
                rotation = Input.GetAxis("Rotation");
            }

            _currentCameraRotation += rotation * CameraRotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(_currentCameraTilt, _currentCameraRotation, 0);

            return Mathf.Abs(rotation) > float.Epsilon;
        }

        /// <summary>
        /// Handles the tilting of the camera.  
        /// </summary>
        private void HandleTilt()
        {
            float tilt = 0.0f;
            if (MouseMovementEnabled && Input.GetKey(KeyCode.Mouse2))
            {
                tilt = -MouseAxis.y;
            }
            else
            {
                tilt = Input.GetAxis("Camera Tilt");
            }

            _currentCameraTilt += tilt * CameraRotationSpeed * Time.deltaTime;
            _currentCameraTilt = Mathf.Clamp(_currentCameraTilt, MinCameraTilt, MaxCameraTilt);

            transform.rotation = Quaternion.Euler(_currentCameraTilt, _currentCameraRotation, 0);
        }
    }
}