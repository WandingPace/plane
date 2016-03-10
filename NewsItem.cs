using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Representation of one news item we get from the server,  expect Title or Body to be in different languages
/// </summary> 
public class NewsItem 
{
	protected static Notify notify = new Notify("NewsItem");
	public string Title;
	public string Body;
	public string LanguageShortCode;
	public int NewsIndex;
	
	public NewsItem( string title, string body, string languageShortCode, int newsIndex)
	{
		Title = title;
		Body = body;
		LanguageShortCode = languageShortCode;
		NewsIndex = newsIndex;
	}
	
	/// <summary>
	/// Decodes the json newsItem, may return null if there are errors
	/// </summary>
	/// <returns>
	/// The json object.
	/// </returns>
	/// <param name='newsItem'>
	/// News item.
	/// </param>
	public static NewsItem DecodeJsonObject(object newsItem)
	{
		NewsItem result = null;
		try
		{
			Dictionary<string, object> dict = newsItem as Dictionary<string, object>;
			string title = dict["newsTitle"] as string;
			string body = dict["newsBody"] as string;
			string languageShortCode = dict["languageShortCode"] as string;
			//int newsIndex = (int) ( (double) dict["newsIndex"] );
			result = new NewsItem(title, body, languageShortCode,  0);
		}
		catch (System.Exception theException)
		{
			notify.Warning("NewsItem.DecodeJsonObject " + theException.Message);
		}
		return result;
	}
}
