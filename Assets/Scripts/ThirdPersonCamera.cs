using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Library;
using Units;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThirdPersonCamera : MonoBehaviour
{
#if !UNITY_WEBGL
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
    private Vector2 m_MouseAnchor;

    private Vector2 m_PrevMousePosition;
    private Vector2 m_DeltaMousePosition;

    private Vector3 m_PrevOffset;

    [SerializeField]
    private GameObject m_Following;
    [SerializeField]
    private GameObject m_Target;

    [SerializeField]
    private Vector3 m_Offset;

    [System.Serializable]
    private struct Box
    {
        public Vector3 m_Min;
        public Vector3 m_Max;
    }

    [SerializeField]
    private Box m_ScreenBorders;

    private Camera m_Camera;

    public GameObject following
    {
        get { return m_Following; }
        set { m_Following = value; }
    }

    public GameObject target
    {
        get { return m_Target; }
        private set
        {
            m_Target = value;
            Publisher.self.DelayedBroadcast(Define.Event.PlayerTargetChanged, this, value);
        }
    }

    public bool isTargeting
    {
        get { return m_Target != null; }
    }

    public Vector3 offset
    {
        get { return m_Offset; }
        set { m_Offset = value; }
    }


    private void Awake()
    {
        Publisher.self.Subscribe(Define.Event.UnitDied, OnUnitDied);
        Publisher.self.Subscribe(Define.Event.TargetTogglePressed, OnTargetTogglePressed);
        Publisher.self.Subscribe(Define.Event.TargetChangePressed, OnTargetChangePressed);
    }

    private void Start()
    {
#if UNITY_WEBGL
        m_EventSystem = FindObjectOfType(typeof(EventSystem)) as EventSystem;
#endif

        if (m_Camera == null && GetComponentInChildren<Camera>() != null)
            m_Camera = GetComponentInChildren<Camera>();
        else
        {
            Debug.LogWarning(name + " needs a camera to be parented to this object!");
            gameObject.SetActive(false);
        }
    }

    private void OnGUI()
    {
        if (Event.current.type == EventType.ScrollWheel)
            m_Offset += new Vector3(0, 0, -Event.current.delta.y);
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_Camera == null || m_Following == null)
            return;

        CheckMouseInput();

        if (m_Target != null)
        {
            Vector3 newPosition = m_Following.transform.position + m_Target.transform.position;
            newPosition /= 2f;

            transform.position = newPosition;

            Vector3 distanceVector = transform.position - m_Following.transform.position;

            float distance = Mathf.Sqrt(
                Mathf.Pow(distanceVector.x, 2)
                + Mathf.Pow(distanceVector.y, 2)
                + Mathf.Pow(distanceVector.z, 2));
            m_Offset = new Vector3(m_Offset.x, m_Offset.y, -10 - (distance * 1.1f));

            m_Camera.transform.localPosition = m_Offset;

            float angle = Mathf.Atan(distanceVector.x / distanceVector.z);
            if ((distanceVector.x < 0.0f && distanceVector.z < 0.0f) ||
                (distanceVector.x > 0.0f && distanceVector.z < 0.0f) ||
                (distanceVector.x == 0.0f && distanceVector.z < 0.0f))
                angle += Mathf.PI;

            transform.eulerAngles = new Vector3(
                35f,
                angle * (180f / Mathf.PI),
                transform.eulerAngles.z);
        }
        else
        {
            transform.position = m_Following.transform.position;
            m_Camera.transform.localPosition = m_Offset;
        }

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, m_ScreenBorders.m_Min.x, m_ScreenBorders.m_Max.x),
            Mathf.Clamp(transform.position.y, m_ScreenBorders.m_Min.y, m_ScreenBorders.m_Max.y),
            Mathf.Clamp(transform.position.z, m_ScreenBorders.m_Min.z, m_ScreenBorders.m_Max.z));
    }

    private void CheckMouseInput()
    {
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
            m_Offset = m_PrevOffset;
            return;
        }

        m_PrevOffset = m_Offset;

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
            m_Offset = m_PrevOffset;
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
}
