using UnityEngine;
using System.Collections;

public class UIMoreGamesViewControllerOz : UIViewControllerOz
{
	public override void appear()
	{		
		UIManagerOz.SharedInstance.PaperVC.SetPageName("More Games", "More Disney");
		//UIManagerOz.SharedInstance.PaperVC.SetCurrentPage(UIManagerOz.SharedInstance.moreGamesVC);	
		base.appear();
	}	
}
