using UnityEngine;

public class UIViewControllerOz : MonoBehaviour
{
    private const float fadeTime = 1.0f;
    [HideInInspector] public Camera myCamera;
    protected Notify notify;
//	private UIPanelAlpha fader;

    protected virtual void Awake()
    {
        SetupNotify();
        if (UIManagerOz.SharedInstance != null)
            myCamera = UIManagerOz.SharedInstance.UICamera;
    }

    protected virtual void Start()
    {
        RegisterEvent();
    }

    protected virtual void RegisterEvent()
    {
    }

    public void SetupNotify()
    {
        if (notify == null)
        {
            notify = new Notify(GetType().Name);
        }
    }

    public void Hide(GameObject go) // used only during initialization and tab switching
    {
        NGUITools.SetActive(go, false);
    }

    public void Show(GameObject go) // used only during tab switching
    {
        NGUITools.SetActive(go, true);
    }

    public virtual void appear()
    {
        SetupNotify();

        NGUITools.SetActive(gameObject, true);
    }

    public virtual void disappear()
    {
        NGUITools.SetActive(gameObject, false);
    }
}