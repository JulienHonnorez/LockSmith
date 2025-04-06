using UnityEngine;

public class Vis : MonoBehaviour
{
    [SerializeField] private Animator Animator;

    private void OnMouseDown()
    {
        Animator.SetTrigger("Go");
    }

    public void DestroyMySelf()
    {
        Destroy(gameObject);
    }
}
