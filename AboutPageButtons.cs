using UnityEngine;
using System.Collections;

public class AboutPageButtons : MonoBehaviour 
{
	void OnClick()
	{
		if (gameObject.name == "ButtonCredits") { Application.OpenURL("http://disney.go.com/disneyinteractivestudios/"); }
		if (gameObject.name == "ButtonCustomer") { Application.OpenURL("http://disneyinteractivestudios.custhelp.com");	}	
		if (gameObject.name == "ButtonPrivacy") { Application.OpenURL("http://corporate.disney.go.com/corporate/pp.html"); }	
		if (gameObject.name == "ButtonTerms") { Application.OpenURL("http://corporate.disney.go.com/corporate/terms-appapp.html"); }	
	}
}
