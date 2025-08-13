using UnityEngine;
using UnityEngine.InputSystem;

public class MouseActions : MonoBehaviour
{
    [Header("クリック移動")]
    public LayerMask raycastLayers = ~0;
    public bool keepHeight = true;
    public float landingOffsetY = 1.7f;
    public float moveSmoothTime = 0.15f;

    [Header("ドラッグ操作")]
    public float rotateSensitivity = 0.2f;  // deg per pixel
    public float pitchMin = -80f;
    public float pitchMax = 80f;
    public float panSpeed = 0.01f;          // world units per pixel
    public float panDamp = 10f;

    [Header("ズーム")]
    public float zoomSpeed = 1f;            // m per wheel step
    public float zoomMinDistance = 0.5f;
    public float zoomMaxDistance = 200f;

    [SerializeField] protected Camera cam;
    protected Vector3 targetPos;
    protected Vector3 moveVelocity;
    protected float yaw, pitch;
    Vector3 panVelocity;


    protected virtual void Awake()
    {
        cam = Camera.main;
    }

    void Start()
    {
        targetPos = transform.position;
        var e = transform.eulerAngles;
        yaw = e.y;
        pitch = e.x > 180 ? e.x - 360f : e.x;
    }


    public void Move(Vector2 pos)
    {

        Ray ray = cam.ScreenPointToRay(pos);

        if (Physics.Raycast(ray, out var hit, 10000f, raycastLayers, QueryTriggerInteraction.Ignore))
        {
            if (keepHeight)
            {
                targetPos = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            }
            else
            {
                targetPos = hit.point + Vector3.up * landingOffsetY;
            }
        }
    }


    float DistanceFactor()
    {
        float d = Vector3.Distance(transform.position, targetPos);
        return Mathf.Clamp(d * 0.05f + 1f, 1f, 20f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.7f, 1f, 0.5f);
        Gizmos.DrawSphere(targetPos, 0.2f);
    }

    protected void Zoomby(float scrollY)
    {

        if (Mathf.Abs(scrollY) > 0.0001f)
        {
            Vector3 dir = transform.forward;
            float distance = Mathf.Clamp(zoomSpeed * Mathf.Sign(scrollY), -10f, 10f);

            Vector3 candidate = targetPos + dir * distance;

            float nextDist = Vector3.Distance(candidate, transform.position);
            if (nextDist >= zoomMinDistance && nextDist <= zoomMaxDistance)
            {
                targetPos = candidate;
            }
        }
    }
    public void Direction(Vector2 delta)
    {
        yaw += delta.x * rotateSensitivity;
        pitch -= delta.y * rotateSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

    }

    public void Up() => targetPos += Vector3.up;
    public void Down() => targetPos -= Vector3.up;
}
