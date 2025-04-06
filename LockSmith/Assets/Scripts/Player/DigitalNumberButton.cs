using UnityEngine;

public class DigitalNumberButton : MonoBehaviour
{
    [SerializeField] private DigitalNumberLocker DigitalNumberLocker;
    [SerializeField] private SpriteRenderer Renderer;
    [SerializeField] private int Value;

    [SerializeField] private bool IsLeftButton;
    [SerializeField] private bool IsRightButton;
    [SerializeField] private bool IsDecodeButton;
    [SerializeField] private bool IsValidateButton;

    [SerializeField] private AudioSource AudioSource;
    [SerializeField] private AudioSource AudioSourceOK;
    [SerializeField] private AudioSource AudioSourceNOK;

    private Color OnColor;
    private Color OffColor;

    void Start()
    {
        OnColor = new Color(Renderer.color.r, Renderer.color.g, Renderer.color.b, 0.5f);
        OffColor = new Color(Renderer.color.r, Renderer.color.g, Renderer.color.b, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
    {
        if (IsLeftButton)
            DigitalNumberLocker.SwitchIndex(-1);
        else if (IsRightButton)
            DigitalNumberLocker.SwitchIndex(1);
        else if (IsDecodeButton)
            DigitalNumberLocker.Decode();
        else if (IsValidateButton)
            DigitalNumberLocker.OnValidate();
        else
            DigitalNumberLocker.SetCurrentValue(Value);

        AudioSource.Play();

        if (IsValidateButton)
        {
            if (DigitalNumberLocker.IsUnlock)
                AudioSourceOK.Play();
            else
                AudioSourceNOK.Play();
        }
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
