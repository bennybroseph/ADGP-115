using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Library;
using Units;
using UnityEngine;

#if UNITY_WEBGL
using UnityEngine.EventSystems;
#endif

public class ThirdPersonCamera : MonoBehaviour
{
    #region -- VARIABLES --
#if !UNITY_WEBGL
    /// <summary> Replica of windows' 'Point' struct </summary>
    public struct Point
    {
        public int X;
        public int Y;

        public Point(int a_X, int a_Y)
        {
            X = a_X;
            Y = a_Y;
        }
    }
    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out Point a_Point);
    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int X, int Y);
#else
    [SerializeField]
    private EventSystem m_EventSystem;
#endif
    /// <summary> Where the mouse is anchored to when holding the left or right button </summary>
    private Vector2 m_MouseAnchor;

    /// <summary> The last frame's mouse position </summary>
    private Vector2 m_PrevMousePosition;
    /// <summary> The change in mouse position since the last frame </summary>
    private Vector2 m_DeltaMousePosition;

    [SerializeField, Tooltip("The camera object this script uses. Should be a child of the current.")]
    private Camera m_Camera;

    [Space]
    [SerializeField, Tooltip("The GameObject that the camera is following")]
    private GameObject m_Following;
    [SerializeField, Tooltip("The GameObject that the Following object is targeting")]
    private GameObject m_Target;

    [Space]
    [SerializeField, Tooltip("How far away in each axis the camera should offset from the anchor")]
    private Vector3 m_Offset;

    /// <summary> The value of the offset before a target was selected </summary>
    private Vector3 m_CacheOffset;

    /// <summary> 
    /// The difference between the position before a target was selected and where it needs to currently be.
    /// Used to smooth the transition instead of moving immediately.
    /// </summary>
    private Vector3 m_OffsetAnchorPosition;
    /// <summary>
    /// The difference between the rotation before a target was selected and where it needs to currently be.
    /// Used to smooth the transition instead of rotating immediately.
    /// </summary>
    private Vector3 m_OffsetAnchorRotation;
    /// <summary>
    /// The difference between the camera's position before a target was selected and 
    /// where it needs to currently be.
    /// Used to smooth the transition instead of moving immediately.
    /// </summary>
    private Vector3 m_OffsetCameraPosition;

    /// <summary> Whether or not smoothing needs to start or not </summary>
    private bool m_ShouldSmoothTransition;

    [System.Serializable]
    private struct Box
    {
        public Vector3 m_Min;
        public Vector3 m_Max;
    }

    [SerializeField, Tooltip("A clamp to the anchor position of the camera")]
    private Box m_ScreenBorders;
    #endregion

    #region -- PROPERTIES --
    public GameObject following
    {
        get { return m_Following; }
        set { m_Following = value; }
    }

    /// <summary> 
    /// Always Broadcasts that the target was changed when 'set' is called and 
    /// forces a smooth transition to the new position 
    /// </summary>
    public GameObject target
    {
        get { return m_Target; }
        private set
        {
            m_Target = value;
            m_ShouldSmoothTransition = true;
            Publisher.self.DelayedBroadcast(Define.Event.PlayerTargetChanged, this, value);
        }
    }

    /// <summary> Actually returns whether 'm_Target' is null or not </summary>
    public bool isTargeting
    {
        get { return m_Target != null; }
    }

    public Vector3 offset
    {
        get { return m_Offset; }
        set { m_Offset = value; }
    }
    #endregion

    #region -- UNITY FUNCTIONS --
    /// <summary> Updates the camera's position when changed in editor if one exists </summary>
    private void OnValidate()
    {
        if (m_Camera == null && GetComponentInChildren<Camera>() != null)
            m_Camera = GetComponentInChildren<Camera>();

        if (m_Camera != null)
            m_Camera.transform.localPosition = m_Offset;
    }

    /// <summary> Subscribes to the proper events </summary>
    private void Awake()
    {
        Publisher.self.Subscribe(Define.Event.UnitDied, OnUnitDied);
        Publisher.self.Subscribe(Define.Event.TargetTogglePressed, OnTargetTogglePressed);
        Publisher.self.Subscribe(Define.Event.TargetChangePressed, OnTargetChangePressed);
    }

    /// <summary> Grabs componets and objects in the scene </summary>
    private void Start()
    {
#if UNITY_WEBGL
        // Grabs the event system in the scene to be used later to determine if a UI element is selected
        m_EventSystem = FindObjectOfType(typeof(EventSystem)) as EventSystem;
#endif
        // If the user hasn't set a camera object, try to find one
        if (m_Camera == null && GetComponentInChildren<Camera>() != null)
            m_Camera = GetComponentInChildren<Camera>();

        // If one still couldn't be found
        if (m_Camera == null)
        {
            Debug.LogWarning(name + " needs a camera to be parented to this object!");
            gameObject.SetActive(false);
        }
    }

    /// <summary> Grabs the mouse scrollwheel data </summary>
    private void OnGUI()
    {
        if (Event.current.type == EventType.ScrollWheel)
            m_Offset += new Vector3(0, 0, -Event.current.delta.y);
    }

    /// <summary> Updates the positions of the anchor and camera </summary>
    private void Update()
    {
        // If no camera object exists or there is no following, don't try to update
        if (m_Camera == null || m_Following == null)
            return;

        CheckMouseInput();

        Vector3 newAnchorPosition = Vector3.zero;   // The new position the anchor needs to go to
        Vector3 newAnchorRotation = Vector3.zero;   // The new rotation the anchor needs to go to
        Vector3 newCameraPosition = Vector3.zero;   // The new position the camera needs to go to

        if (m_Target != null)
        {
            newAnchorPosition = m_Following.transform.position + m_Target.transform.position;
            newAnchorPosition /= 2f;

            // The difference between the target's position and the following's position
            Vector3 distanceVector = m_Target.transform.position - m_Following.transform.position;

            // The distance between the target's position and the following's position
            float distance = Mathf.Sqrt(
                Mathf.Pow(distanceVector.x, 2)
                + Mathf.Pow(distanceVector.y, 2)
                + Mathf.Pow(distanceVector.z, 2));
            // Makes the camera position scale with distance to the target
            newCameraPosition = new Vector3(
                m_Camera.transform.localPosition.x,
                m_Camera.transform.localPosition.x,
                -5 - distance);

            float angle = Mathf.Atan(distanceVector.x / distanceVector.z);

            // Adjust the angle when it is in the 3rd, 4th or 180 degrees
            if ((distanceVector.x < 0.0f && distanceVector.z < 0.0f) ||
                (distanceVector.x > 0.0f && distanceVector.z < 0.0f) ||
                (distanceVector.x == 0.0f && distanceVector.z < 0.0f))
                angle += Mathf.PI;

            // Make sure the angle stays positive
            if (angle >= 2 * Mathf.PI)
                angle -= 2 * Mathf.PI;
            if (angle < 0)
                angle += 2 * Mathf.PI;

            // Set the anchor rotaion in degrees with an offset on the x axis
            newAnchorRotation = new Vector3(
                35f,
                angle * (180f / Mathf.PI),
                transform.eulerAngles.z);
        }
        else
        {
            newAnchorPosition = m_Following.transform.position;
            newAnchorRotation = transform.eulerAngles;
            newCameraPosition = m_Offset;
        }

        if (m_ShouldSmoothTransition)
        {
            m_ShouldSmoothTransition = false;

            m_OffsetAnchorPosition = newAnchorPosition - transform.position;
            m_OffsetAnchorRotation = newAnchorRotation - transform.eulerAngles;
            m_OffsetCameraPosition = newCameraPosition - m_Camera.transform.localPosition;
        }

        m_OffsetAnchorPosition = Vector3.Lerp(m_OffsetAnchorPosition, Vector3.zero, Time.deltaTime * 5);
        m_OffsetAnchorRotation = Vector3.Lerp(m_OffsetAnchorRotation, Vector3.zero, Time.deltaTime * 5);
        m_OffsetCameraPosition = Vector3.Lerp(m_OffsetCameraPosition, Vector3.zero, Time.deltaTime * 5);

        if (m_OffsetAnchorPosition.magnitude <= 0.01f)
            m_OffsetAnchorPosition = Vector3.zero;
        if (m_OffsetAnchorRotation.magnitude <= 0.01f)
            m_OffsetAnchorRotation = Vector3.zero;
        if (m_OffsetCameraPosition.magnitude <= 0.01f)
            m_OffsetCameraPosition = Vector3.zero;

        transform.position = newAnchorPosition - m_OffsetAnchorPosition;
        transform.eulerAngles = newAnchorRotation - m_OffsetAnchorRotation;
        m_Camera.transform.localPosition = newCameraPosition - m_OffsetCameraPosition;

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, m_ScreenBorders.m_Min.x, m_ScreenBorders.m_Max.x),
            Mathf.Clamp(transform.position.y, m_ScreenBorders.m_Min.y, m_ScreenBorders.m_Max.y),
            Mathf.Clamp(transform.position.z, m_ScreenBorders.m_Min.z, m_ScreenBorders.m_Max.z));
    }
    #endregion

    #region -- PRIVATE FUNCTIONS --
    private void CheckMouseInput()
    {
        if (m_Target != null)
            return;

#if !UNITY_WEBGL
        Point currentMousePosition;
        GetCursorPos(out currentMousePosition);

        m_DeltaMousePosition =
            new Vector2(currentMousePosition.X, currentMousePosition.Y) - m_PrevMousePosition;

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            m_MouseAnchor = new Vector2(currentMousePosition.X, currentMousePosition.Y);

        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            Cursor.visible = false;

            SetCursorPos((int)m_MouseAnchor.x, (int)m_MouseAnchor.y);
            GetCursorPos(out currentMousePosition);

            Vector3 newAngle = transform.eulerAngles;
            newAngle +=
                new Vector3(
                    m_DeltaMousePosition.y / 12f,
                    m_DeltaMousePosition.x / 10f,
                    0);

            newAngle =
                    new Vector3(
                        Mathf.Clamp(newAngle.x, 10, 90),
                        newAngle.y,
                        newAngle.z);

            transform.eulerAngles = newAngle;
        }
        else
            Cursor.visible = true;

        m_PrevMousePosition = new Vector2(currentMousePosition.X, currentMousePosition.Y);
#else
        if (m_EventSystem.currentSelectedGameObject == null)
        {
            Vector2 currentMousePosition = Input.mousePosition;
            m_DeltaMousePosition =
                new Vector2(
                    Input.GetAxis("Mouse X"),
                    -Input.GetAxis("Mouse Y"));

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                m_MouseAnchor = currentMousePosition;
            }
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                Vector3 newAngle = transform.eulerAngles;
                newAngle +=
                    new Vector3(
                        m_DeltaMousePosition.y / 3500f,
                        m_DeltaMousePosition.x / 3000f,
                        0);

                newAngle =
                        new Vector3(
                            Mathf.Clamp(newAngle.x, 10, 90),
                            newAngle.y,
                            newAngle.z);

                transform.eulerAngles = newAngle;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            if (currentMousePosition != m_MouseAnchor)
                m_PrevMousePosition = currentMousePosition;
        }
#endif
    }

    private void OnTargetTogglePressed(Define.Event a_Event, params object[] a_Params)
    {
        if (m_Target != null)
        {
            target = null;
            m_Offset = m_CacheOffset;
            return;
        }

        m_CacheOffset = m_Offset;

        Vector3 forward = new Vector3(
            Mathf.Cos((-transform.eulerAngles.y + 90) * (Mathf.PI / 180f)), 0f,
            Mathf.Sin((-transform.eulerAngles.y + 90) * (Mathf.PI / 180f)));

        List<Collider> objectsHit = Physics.OverlapSphere(m_Following.transform.position, 8f).ToList();
        objectsHit.AddRange(
            Physics.OverlapSphere(
                m_Following.transform.position + new Vector3(forward.x * 10f, 0.5f, forward.z * 10f), 10f).
                    Where(x => !objectsHit.Contains(x)));

        List<Collider> parsedUnits = objectsHit.Where(
            x => x.gameObject.GetComponent<Unit>() != null &&
                 x.gameObject.GetComponent<Unit>() != m_Following.GetComponent<Unit>()).ToList();

        parsedUnits.Sort(SortByDistance);

        if (parsedUnits.Count > 0)
            target = parsedUnits[0].gameObject;
    }
    private void OnTargetChangePressed(Define.Event a_Event, params object[] a_Params)
    {
        List<Collider> objectsHit = Physics.OverlapSphere(m_Target.transform.position, 15f).ToList();

        List<Collider> parsedUnits = objectsHit.Where(
            x => x.gameObject.GetComponent<Unit>() != null &&
                 x.gameObject.GetComponent<Unit>() != m_Following.GetComponent<Unit>() &&
                 x.gameObject != m_Target).ToList();

        parsedUnits.Sort(SortByDistance);

        if (parsedUnits.Count > 0)
            target = parsedUnits[0].gameObject;
    }

    private void OnUnitDied(Define.Event a_Event, params object[] a_Params)
    {
        Unit unit = a_Params[0] as Unit;

        if (unit == null || unit.gameObject != m_Target || m_Target == null)
            return;

        List<Collider> objectsFound = Physics.OverlapSphere(m_Target.transform.position, 15f).Where(
            x =>
                x.gameObject.GetComponent<Unit>() != null &&
                x.gameObject != m_Following).ToList();

        target = null;

        if (objectsFound.Count != 0)
            target = objectsFound[0].gameObject;

        if (m_Target == null)
            m_Offset = m_CacheOffset;
    }

    private int SortByDistance(Collider a_FirstCollider, Collider a_SecondCollider)
    {
        float distanceA = Vector3.Distance(a_FirstCollider.transform.position, m_Following.transform.position);
        float distanceB = Vector3.Distance(a_SecondCollider.transform.position, m_Following.transform.position);

        if (distanceA > distanceB)
            return 1;
        if (distanceA < distanceB)
            return -1;

        return 0;
    }
    #endregion
}
