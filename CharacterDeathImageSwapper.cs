using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterDeathImageSwapper : MonoBehaviour 
{
	public UISprite deathPortraitSprite;
	public UITexture deathPortraitTexture;

	public void SetImage(string deathType)
	{
		int charID = GameProfile.SharedInstance.GetActiveCharacter().characterId;			// get active character's ID
		
		SelectSpriteOrTextureToBeActive(charID);			// turn on sprite or texture, depending on if regular Oz or not
		
		if (charID == 0)
		{
			deathPortraitSprite.spriteName = "Oz_DS_" + deathType + "_color_r01.jpg";		// set Oz sprite for death type
		}
		else
		{
			deathPortraitTexture.mainTexture = GetTextureForActiveCharacter(deathType);		// set image appropriate for death type
		}
	}
	
	private Texture2D GetTextureForActiveCharacter(string deathType)
	{
//		string prefix = GameProfile.SharedInstance.GetActiveCharacter().deathImagePrefix;	// character name prefix
		int postfix = UIManagerOz.SharedInstance.GetUISizeVal();							// get atlas size identifier (1, 2 or 3)
		string path = "Oz/interface/DeathScreens/";
//		
//		if (postfix == 1)
//		{
//			path = "hiresresources/";		// high res images are only loaded from high res asset bundle
//		}
//
//        // 数据源来自_DesignData上挂载的GameProfile脚本内Character列表,暂时在此处重新进行赋值
//	    if (prefix.Equals("Oz"))
//            prefix = "OzTopHat";
        string prefix = "";
	    var fullpath = path + prefix + "_DS_" + deathType + "_color_r0" + postfix;
	    var texture = (Texture2D)ResourceManager.Load(fullpath, typeof(Texture2D));
		return texture;
	}			

	private void SelectSpriteOrTextureToBeActive(int charID)
	{
		if (charID == 0)
		{
			deathPortraitSprite.enabled = true;	
			deathPortraitTexture.enabled = false;
		}
		else
		{
			deathPortraitSprite.enabled = false;	
			deathPortraitTexture.enabled = true;			
		}
	}	
}






	
	//private Dictionary<string,Texture2D> deathImageTextures = new Dictionary<string,Texture2D>();
	
//	void Start()
//	{
		//deathImageTextures.Add("baboon", new Texture2D(4, 4));
		//deathImageTextures.Add("falling", new Texture2D(4, 4));
		//deathImageTextures.Add("stunned", new Texture2D(4, 4));
		
		//GetTextureForActiveCharacter("");
		//LoadTexturesForCharacter	
//	}
	


//	private void LoadTexturesForCharacter(string characterPrefix)
//	{
//		int postfix = UIManagerOz.SharedInstance.GetUISizeVal();	// get atlas size identifier (1, 2 or 3)
//		
//		//deathImageTextures["baboon"] = 
//		//deathImageTextures["falling"] = 
//		//deathImageTextures["stunned"] = 
//	}


	
//	private Texture2D LoadSingleTexture(string characterPrefix, string deathType, int sizeID)
//	{
//		//string fileName = 
//		return new Texture2D(4,4);	//LoadTextureFromFile(NavMeshPath + fileName);
//	}
	



//	public static Texture2D LoadTextureFromFile(string filePath)
//	{
//		byte[] byteArray = System.IO.File.ReadAllBytes(filePath);
//		Texture2D tex = new Texture2D(4,4);
//		tex.LoadImage(byteArray);
//		tex.Apply();
//		return tex;
//	}	



		//GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		//go.name = "TextureLoadingTest";
        //go.renderer.material.mainTexture = (Texture2D)Resources.Load("ChinaGirl_DS_baboon_color_r01", typeof(Texture2D));
		//return go.renderer.material.mainTexture as Texture2D;
		
