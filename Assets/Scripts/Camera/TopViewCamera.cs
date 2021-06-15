using InfenixTools;
using UnityEngine;

[AddComponentMenu("Camera/Top View Follower"), DisallowMultipleComponent, ExecuteAlways]
public class TopViewCamera : MonoBehaviour
{
    private const int completeCircleDegrees = 360;
    public const string wallTag = "Wall";
    [Header("Object to follow")]
    [SerializeField]
    private Transform m_Target;

    [Header("Camera height")]
    [SerializeField]
    private float m_Height;
    [SerializeField]
    private float m_MinHeight = 3f;
    [SerializeField]
    private float m_MaxHeight = 20f;

    [Header("Camera distance")]
    [SerializeField]
    private float m_Distance;
    [SerializeField]
    private float m_MinDistance = 3f;
    [SerializeField]
    private float m_MaxDistance = 20f;
    [SerializeField]
    private float m_distanceScrollSpeed = 4f;

    [Header("Other settings")]
    [SerializeField]
    private float m_CurrentAngle;

    [SerializeField]
    private float m_smoothDampSpeed = .1f;
    private Vector3 m_smoothDampVelocity;

    [SerializeField]
    private float cameraLookOffset = 1f;

    public bool automaticallyRotate;
    [SerializeField]
    private float m_rotationSpeed = 1f;

    private void Start()
    {
        HandleCamera();
    }

    private void Update()
    {
        HandleMouseInputs();
        HandleCamera();
    }

    private void HandleCamera()
    {
        if (!m_Target)
            return;

        Vector3 worldPosition = (Vector3.forward * -m_Distance) + (Vector3.up * m_Height);
        Vector3 rotatedVector = Quaternion.AngleAxis(m_CurrentAngle, Vector3.up) * worldPosition;

        Vector3 flatTargetPosition = m_Target.position;
        flatTargetPosition.y = 0;
        Vector3 finalPosition = flatTargetPosition + rotatedVector;

        transform.position = Vector3.SmoothDamp(transform.position, finalPosition, ref m_smoothDampVelocity, m_smoothDampSpeed);
        transform.LookAt(flatTargetPosition + Vector3.up * cameraLookOffset);
    }

    private void HandleMouseInputs()
    {
        if (Input.GetMouseButton(2) && !automaticallyRotate)
        {
            float newHeight = m_Height - Input.GetAxis("Mouse Y");
            m_Height = Mathf.Clamp(newHeight, m_MinHeight, m_MaxHeight);
        }

        if (Input.GetMouseButton(1) || automaticallyRotate)
        {
            m_CurrentAngle += automaticallyRotate ? 1 * Time.deltaTime * m_rotationSpeed : Input.GetAxis("Mouse X");
            m_CurrentAngle %= completeCircleDegrees;
        }

        float newDistance = m_Distance - Input.mouseScrollDelta.y * m_distanceScrollSpeed;
        m_Distance = Mathf.Clamp(newDistance, m_MinDistance, m_MaxDistance);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = CustomColors.transparentDarkGreen;
        if (m_Target)
        {
            Gizmos.DrawLine(transform.position, m_Target.position);
            Gizmos.DrawSphere(m_Target.position, 1.5f);
        }
        Gizmos.DrawSphere(transform.position, 1.5f);
    }

    private void OnValidate()
    {
        if (m_Height < m_MinHeight)
            m_Height = m_MinHeight;
        else if (m_Height > m_MaxHeight)
            m_Height = m_MaxHeight;

        if (m_Distance < m_MinDistance)
            m_Distance = m_MinDistance;
        else if (m_Distance > m_MaxDistance)
            m_Distance = m_MaxDistance;
    }
}
