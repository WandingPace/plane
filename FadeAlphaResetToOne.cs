/*
using UnityEngine;

public class FadeAlphaResetToOne : MonoBehaviour
{
	private float _alpha = 1f;	
    private UIWidget[] _mWidgets;
 
    void OnEnable()
    {
        _mWidgets = GetComponentsInChildren<UIWidget>();
 
        foreach (UIWidget w in _mWidgets)
        {
            Color c = w.color;
            c.a = _alpha;
            w.color = c;
        }
		
		Destroy(this);	// end myself
	}
}
*/
