using UnityEngine;

public class Dirty : MonoBehaviour
{
    private void OnMouseDown()
    {
        Destroy(gameObject);
    }
}
