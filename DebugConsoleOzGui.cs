using UnityEngine;
using System.Collections;

public class DebugConsoleOzGui : MonoBehaviour {
	
	private bool initialized = false;
	void OnGUI()
	{		
		if ( Settings.GetBool("console-enabled", false))
		{
			// unfortunately Unity says it must be initialized inside OnGUI
			if (!initialized)
			{
				DebugConsoleOz.Init();
				initialized = true;
			}	
				
			DebugConsoleOz.OnGUI();	
		}
	}
}
