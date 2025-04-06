using TMPro;
using UnityEngine;

public class NumberSwitcherButton : MonoBehaviour
{
    [SerializeField] private NumberSwitcher NumberSwitcher;
    [SerializeField] private SpriteRenderer Renderer;
    [SerializeField] private int Value;

    private Color OnColor;
    private Color OffColor;

    private void Start()
    {
        OnColor = new Color(Renderer.color.r, Renderer.color.g, Renderer.color.b, 0.5f);
        OffColor = new Color(Renderer.color.r, Renderer.color.g, Renderer.color.b, 0);
    }

    private void OnMouseDown()
    {
        NumberSwitcher.AddValue(Value);
    }

    private void OnMouseOver()
    {
        Renderer.color = OnColor;
    }

    private void OnMouseExit()
    {
        Renderer.color = OffColor;
    }
}
