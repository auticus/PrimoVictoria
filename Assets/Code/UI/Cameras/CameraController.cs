using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    /*
     * Movement defaults to the WASD keys as well as the arrow keys
     * If mouse movement enabled, moving mouse to edge of screen will move the screen as well
     * Q & E will rotate the camera as will holding middle mouse button and moving the mouse
     * mouse wheel will zoom in and out
     * TODO: manual tilt
     * TODO: smart height adjustment so that the camera raises in height if it would collide with a ground object like a tower or a guy on a hill
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
    [SerializeField] [Tooltip("How far the camera can go in X or Y coordinates")] private Vector2 MapLimit = new Vector2(40, 50);


    private Vector2 MouseAxis
    {
        get { return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); }
    }

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        transform.eulerAngles = new Vector3(StartingCameraTilt, 0, 0);

        var camera = FindObjectOfType<Camera>();
        camera.fieldOfView = FieldOfView;
    }

    
    void LateUpdate()
    {
        HandleAxisMovement();
        HandleRotation();
    }
    #endregion Unity Methods

    private void HandleAxisMovement()
    {
        var xAxis = Input.GetAxis("Horizontal");
        var zAxis = Input.GetAxis("Vertical");
        var pos = transform.position;

        if (MouseMovementEnabled)
            HandleMouseMovement(ref xAxis, ref zAxis);

        var x = xAxis * CameraMovementSpeed * Time.deltaTime;
        var z = zAxis * CameraMovementSpeed * Time.deltaTime;

        pos.x += x;
        pos.z += z;

        var scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * CameraZoomSpeed * Time.deltaTime;

        pos.x = Mathf.Clamp(pos.x, -MapLimit.x, MapLimit.x);
        pos.z = Mathf.Clamp(pos.z, -MapLimit.y, MapLimit.y);
        pos.y = Mathf.Clamp(pos.y, MinCameraZoom, MaxCameraZoom);
        transform.position = pos;
    }

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

    private void HandleRotation()
    {
        var rotation = Input.GetAxis("Rotation");
        transform.Rotate(Vector3.up, rotation * Time.deltaTime * CameraRotationSpeed, Space.World);

        if (MouseMovementEnabled)
            HandleMouseRotation();
    }

    private void HandleMouseRotation()
    {
        //holding down the scroll wheel button and moving the mouse also rotates the camera
        //unity is a 0-based system so l. button = 0, r. button = 1, and middle = 2
        if (Input.GetKey(KeyCode.Mouse2))
            transform.Rotate(Vector3.up, -MouseAxis.x * Time.deltaTime * CameraRotationSpeed, Space.World);
    }
}
