using UnityEngine;
using System.Collections;

public class OzDisappear : MonoBehaviour
{
	
	public void disappear()
	{
		GamePlayer.SharedInstance.playerFx.DoOzDisappear();
		AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_Gatcha_Deal);
	}
	
}
