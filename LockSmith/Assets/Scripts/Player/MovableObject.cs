using UnityEngine;

public class MovableObject : MonoBehaviour
{
    private Vector3 mouseOffset;
    private float zDistance;
    private Transform parentTransform;
    private PolygonCollider2D parentCollider;

    void Start()
    {
        parentTransform = transform.parent;
        parentCollider = parentTransform.GetComponent<PolygonCollider2D>();
    }

    void OnMouseDown()
    {
        zDistance = Camera.main.WorldToScreenPoint(transform.position).z;

        mouseOffset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDistance));
    }

    void OnMouseDrag()
    {
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDistance)) + mouseOffset;

        Vector3 parentMin = parentCollider.bounds.min;
        Vector3 parentMax = parentCollider.bounds.max;

        newPosition.x = Mathf.Clamp(newPosition.x, parentMin.x, parentMax.x);
        newPosition.y = Mathf.Clamp(newPosition.y, parentMin.y, parentMax.y);
        newPosition.z = Mathf.Clamp(newPosition.z, parentMin.z, parentMax.z);

        transform.position = newPosition;
    }
}