using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum UiScreenName
{
    IDOL,
    MAIN,
    UPGRADES,
    OBJECTIVES,
    LEADERBOARDS,
    POSTRUN,
    WORLDOFOZ,
    RANDINGBOARDS
}

public enum NotificationType
{
    Powerup,
    Modifier,
    Consumable,
    MoreCoins,
    Challenge,
    Character,
    Land,
    ArtifactSale,
    ConsumableSale,
    PowerupSale,
    CharacterSale,
    None
}

// wxj, add activity notification type
public enum OneShotNotificationType
{
    NewWeeklyChallenge,
    WeeklyChallengeCompleted,
    TopScorePositionWentUp,
    TopScorePositionWentDown,
    TopDistancePositionWentUp,
    TopDistancePositionWentDown,
    NewTeamChallenge,
    TeamChallengeCompleted,
    NewActivityChallenge,
    LegendaryChallengeCompleted, //已完成成就
    ActivityChallengeCompleted //已完成任务
}

public class WrappedDict // necessary for JSON serialization & deserialization of Notification Clears
{
    public Dictionary<int, Int64> dict = new Dictionary<int, Int64>();

    public WrappedDict()
    {
    }

    public WrappedDict(Dictionary<int, Int64> sourceDict)
    {
        dict = sourceDict;
    }
}

public class NotificationSystem : MonoBehaviour
{
    protected static Notify notify;
    private readonly int characterClearTime = 540; //9min;
    //	间隔几秒后，再重新提醒								// seconds before the clear is lifted

    private readonly int consumableClearTime = 180; //3min;
    private readonly int landClearTime = 180; //3min;
    private AppCounters appCounters;
    private Dictionary<int, Int64> charactersCleared = new Dictionary<int, Int64>();
    private int charactersNewFeature; // for notification on first run of app, that new character feature exists
    private List<int> charactersPurchasable = new List<int>(); //可购买角色
    // notification clears that expire after certain amount of time

    private Dictionary<int, Int64> consumablesCleared = new Dictionary<int, Int64>();
    private List<int> consumablesPurchasable = new List<int>(); //可购买消耗品
    private Dictionary<int, Int64> landsCleared = new Dictionary<int, Int64>();
    private List<int> landsDownloadable = new List<int>(); //可下载新场景
    public NotificationIcons notificationIcons;
    // notification clears that don't expire (that stay cleared forever)
    private Dictionary<int, Int64> oneShotNotifications = new Dictionary<int, Int64>();
        // Key = ID of whatever it is, Value = (int)OneShotNotificationType

    private int totalAppTime;

    private void Awake()
    {
        notify = new Notify(GetType().Name);
    }

    private void Start()
    {
//		ResetClearedNotificationsInPlayerPrefs();
        appCounters = Services.Get<AppCounters>();
        charactersNewFeature = PlayerPrefs.GetInt("charactersNewFeature", 1); // '0' = cleared, '1' = new
        RestoreAllClearedNotificationsFromPlayerPrefs();
        RefreshAllNotifications();
    }

    //重置已清除的提示
    public static void ResetClearedNotificationsInPlayerPrefs()
    {
        PlayerPrefs.SetString("consumablesCleared", "{}");
        PlayerPrefs.SetString("landsCleared", "{}");
        PlayerPrefs.SetString("charactersCleared", "{}");
        PlayerPrefs.SetString("oneShotNotifications", "{}");
        PlayerPrefs.Save();
    }

    //添加一次性提示
    public void SendOneShotNotificationEvent(int id, int type) //OneShotNotificationType type,
    {
        if (!oneShotNotifications.ContainsKey(id))
            //(int)type))	// skip this if there is already an old uncleared one-shot notification
            oneShotNotifications.Add(id, type); // log the one-shot notification

        notify.Debug("oneShotNotification count = " + oneShotNotifications.Count);

        foreach (var kvp in oneShotNotifications)
            notify.Debug("oneShotNotification key = " + kvp.Key + ", value = " + kvp.Value);
    }

    //清除新角色提示
    public void ClearCharactersNewFeatureNotification()
    {
        charactersNewFeature = 0;
        PlayerPrefs.SetInt("charactersNewFeature", 0);
    }

    //
    public List<int> GetLandsDownloadable()
    {
        return landsDownloadable;
    }

    //按类型 获取一次性提示的个数
    public int GetOneShotCount(OneShotNotificationType type)
    {
        var count = 0;

        foreach (var kvp in oneShotNotifications)
        {
            if (kvp.Value == (int) type)
                count++;
        }

        return count;
    }

    //清除指定Id的一次性提示
    public void ClearOneShotNotification(int id)
    {
        oneShotNotifications.Remove(id);
    }

    //清除指定类型的一次性提示
    public void ClearOneShotNotification(OneShotNotificationType type)
    {
        var keysToRemove = new List<int>();

        foreach (var kvp in oneShotNotifications)
        {
            if (kvp.Value == (int) type)
                keysToRemove.Add(kvp.Key);
        }

        foreach (var key in keysToRemove)
            oneShotNotifications.Remove(key);

        SaveClearedNotificationsToPlayerPrefs();
    }

    //清除永久提示
    public void ClearNotification(NotificationType type, int id)
    {
        switch (type)
        {
            case NotificationType.Consumable:
                ClearSingleNotification(consumablesPurchasable, ref consumablesCleared, id);
                break;
            case NotificationType.Character:
                ClearSingleNotification(charactersPurchasable, ref charactersCleared, id);
                break;
            case NotificationType.MoreCoins:
                break;
            case NotificationType.Land:
                foreach (var setID in landsDownloadable)
                {
                    if (!landsCleared.ContainsKey(setID))
                    {
                        landsCleared.Add(setID, appCounters.GetSecondsSpentInApp());
                    }
                }
                break;
        }
    }

    //清除提示（添加到cleared列表）
    private void ClearSingleNotification(List<int> purchasableList, ref Dictionary<int, Int64> clearedDict, int id)
    {
        if (purchasableList.Contains(id))
        {
            if (!clearedDict.ContainsKey(id))
            {
                clearedDict.Add(id, appCounters.GetSecondsSpentInApp());
                notify.Debug("add in cleared id:" + id);
            }
        }
    }

    /// <summary>
    ///     -1 = don't show icon at all, 0 = show exclamation point, 1-9 show number, >9 show exclamation point
    ///     -1不提示，0提示感叹号，1-9提示数字，大于9提示感叹号
    /// </summary>
    public void SetNotificationIconsForThisPage(UiScreenName screenName)
    {
        var idolMenuVC = UIManagerOz.SharedInstance.idolMenuVC; //主界面
        var worldOfOzVC = UIManagerOz.SharedInstance.worldOfOzVC; //关卡
        //var inventoryVC = UIManagerOz.SharedInstance.inventoryVC; //挑战
        //var characterVC = UIManagerOz.SharedInstance.chaSelVC; //角色
        //var ObjectivesVC = UIManagerOz.SharedInstance.ObjectivesVC; //任务
        //var statVc = UIManagerOz.SharedInstance.statsVC; //成就
        var leaderboardVC = UIManagerOz.SharedInstance.leaderboardVC; //排行榜


        RefreshAllNotifications(); // refresh everything every time for now, to make sure it works right first

        var legendaryCompleted = GetOneShotCount(OneShotNotificationType.LegendaryChallengeCompleted);


        var distances = GetOneShotCount(OneShotNotificationType.TopDistancePositionWentUp) +
                        GetOneShotCount(OneShotNotificationType.TopDistancePositionWentDown);
        var scores = GetOneShotCount(OneShotNotificationType.TopScorePositionWentUp) +
                     GetOneShotCount(OneShotNotificationType.TopScorePositionWentDown);
        //var totalLeaderboards = distances + scores; //(distances ? 1 : 0) + (scores ? 1 : 0);

        var lands = GetActualNotifications(landsDownloadable, landsCleared);
        var characters = GetActualNotifications(charactersPurchasable, charactersCleared);
        //var totalWorldOfOz = ((lands.Count > 0) ? 1 : 0) + characters.Count + charactersNewFeature;


        // for sale banners, -1 = don't show banner, 0 = show banner


        switch (screenName)
        {
            case UiScreenName.IDOL:
                idolMenuVC.SetNotificationIcon(0, (legendaryCompleted == 0) ? -1 : legendaryCompleted);
                break;

            case UiScreenName.OBJECTIVES:

                break;

            case UiScreenName.LEADERBOARDS:
                leaderboardVC.SetNotificationIcon(0, (distances == 0) ? -1 : 0);
                    //distances); //0 : -1);// top_distances
                leaderboardVC.SetNotificationIcon(1, (scores == 0) ? -1 : 0);
                    //scores);	//0 : -1);	// tab_top_scores				
                break;

            case UiScreenName.WORLDOFOZ:
                worldOfOzVC.SetNotificationIcon(0, (lands.Count == 0) ? -1 : 0); // tab_lands
                worldOfOzVC.SetNotificationIcon(1, (charactersNewFeature == 1)
                    ? 0
                    : // tab_characters
                    (characters.Count == 0) ? -1 : characters.Count);
                break;
        }
    }

    private int GetChallengesIconValue(int completed, int newChallenges)
    {
        var iconValue = -1;

        if (completed > 0)
            iconValue = completed; // if any completed challenges, show how many in notification icon
        else
            iconValue = (newChallenges > 0) ? 0 : -1; // otherwise, if we have new challenges, show exclamation point

        return iconValue;
    }

    public bool GetNotificationStatusForThisCell(NotificationType type, int id)
    {
        var returnVal = false;

        switch (type)
        {
            case NotificationType.Consumable:
                returnVal = (consumablesPurchasable.Contains(id) && !consumablesCleared.ContainsKey(id));
                break;

            case NotificationType.Land:
                returnVal = (landsDownloadable.Count != 0); // start with 'true' only if we have lands to download

                foreach (var setID in landsDownloadable) // check clears only if we have lands to download
                {
                    if (landsCleared.ContainsKey(setID))
                    {
                        returnVal = false; // turn notification off for downloadable lands only if cleared
                    }
                }
                break;

            case NotificationType.Character:
                returnVal = (charactersPurchasable.Contains(id) && !charactersCleared.ContainsKey(id));
                break;

            case NotificationType.Challenge:
                // only true for completed challenges (weekly or legendary), not for new weekly challenges
                returnVal = false;

                if (oneShotNotifications.ContainsKey(id))
                {
                    if (oneShotNotifications[id] != (int) OneShotNotificationType.NewWeeklyChallenge &&
                        oneShotNotifications[id] != (int) OneShotNotificationType.NewTeamChallenge)
                    {
                        return true;
                    }
                }
                break;
        }

        return returnVal; //false;
    }

    //保存提示数据到用户偏好中
    public void SaveClearedNotificationsToPlayerPrefs()
    {
        // serialize 'notifications cleared' dicts to JSON and compress string, then store in player prefs	
        SaveSingleClearedNotificationDict("consumablesCleared", consumablesCleared);
        SaveSingleClearedNotificationDict("landsCleared", landsCleared);
        SaveSingleClearedNotificationDict("charactersCleared", charactersCleared);
        SaveSingleClearedNotificationDict("oneShotNotifications", oneShotNotifications);
        PlayerPrefs.Save();
    }

    private void SaveSingleClearedNotificationDict(string prefString, Dictionary<int, Int64> dict)
    {
        //string compressed = StringCompressor.CompressString(MiniJSON.Json.Serialize(dict));
//		string compressed = MiniJSON.Json.Serialize(dict);
        var compressed = SerializationUtils.ToJson(new WrappedDict(dict));
        PlayerPrefs.SetString(prefString, compressed);
        notify.Debug("Saving cleared notification list (" + prefString + "): " + compressed + " / Dict.Count = " +
                     dict.Count);
    }

    //恢复所有已清除提示数据
    private void RestoreAllClearedNotificationsFromPlayerPrefs()
    {
        RestoreSingleClearedNotificationFromPlayerprefs("consumablesCleared", ref consumablesCleared);
        RestoreSingleClearedNotificationFromPlayerprefs("landsCleared", ref landsCleared);
        RestoreSingleClearedNotificationFromPlayerprefs("charactersCleared", ref charactersCleared);
        RestoreSingleClearedNotificationFromPlayerprefs("oneShotNotifications", ref oneShotNotifications);
    }

    private void RestoreSingleClearedNotificationFromPlayerprefs(string prefName, ref Dictionary<int, Int64> clearedDict)
    {
        var pref = PlayerPrefs.GetString(prefName, "{}"); // get string from player prefs
//		pref = (pref == "{}") ? pref : StringCompressor.DecompressString(pref);	// decompress, if applicable
        //		clearedDict =MiniJSON.Json.Deserialize(pref) as Dictionary<int,Int64>;
        //		if(clearedDict==null)
        //			clearedDict=new Dictionary<int, long>();
        var wrappedDict = new WrappedDict();
        SerializationUtils.FromJson(wrappedDict, pref);
        clearedDict = wrappedDict.dict;
        notify.Debug("Loading cleared notification list (" + prefName + "): " + pref);
    }

    private void RefreshAllNotifications() // for manually triggering update of notification status
    {
        //更新已清除提示
        UpdateAllClearedNotification(); // remove 'notification clears' that have expired

        //获取可购买可视 提示数据

        consumablesPurchasable = GetConsumablesPurchasable();
        landsDownloadable = GetLandsDownloadableFromSource();
        charactersPurchasable = GetCharactersPurchasable();
    }

    //更新已清除提示
    private void UpdateAllClearedNotification()
    {
        UpdateAllNotificationCleared(consumablesCleared, consumableClearTime);
        UpdateAllNotificationCleared(landsCleared, landClearTime);
        UpdateAllNotificationCleared(charactersCleared, characterClearTime);
    }

    private void UpdateAllNotificationCleared(Dictionary<int, Int64> clearedList, int seconds)
    {
        var listToClear = new List<int>();

        foreach (var clear in clearedList)
        {
            // remove clear from list when current time becomes greater than time cleared + time passed value
            if (clear.Value + seconds < appCounters.GetSecondsSpentInApp())
            {
                listToClear.Add(clear.Key);
                notify.Debug(" remove from cleared key:" + clear.Key);
            }
        }

        foreach (var idToClear in listToClear)
            clearedList.Remove(idToClear); // clearedList.Remove(clear.Key);
    }

    //获得实际提示
    private List<int> GetActualNotifications(List<int> actionableList, Dictionary<int, Int64> clearedList)
        // ones that aren't cleared, so will be shown
    {
        var notificationList = new List<int>();

        foreach (var itemID in actionableList)
        {
            if (!clearedList.ContainsKey(itemID))
                notificationList.Add(itemID);
        }

        return notificationList;
    }

    private bool IsConsumablePurchasable(int consumableId)
    {
        return (GameProfile.SharedInstance.Player.CanAffordConsumable(consumableId) && // check if can purchase
                !GameProfile.SharedInstance.Player.IsConsumableMaxedOut(consumableId)); // check if maxed out
    }

    private List<int> GetConsumablesPurchasable()
    {
        var purchaseableList = new List<int>();

        foreach (var consumableData in ConsumableStore.consumablesList)
        {
            if (IsConsumablePurchasable(consumableData.PID))
            {
                purchaseableList.Add(consumableData.PID);
            }
        }

        return purchaseableList;
    }

    private List<int> GetLandsDownloadableFromSource()
    {
        var downloadableList = new List<int>();

        foreach (var bootData in EnvironmentSetBootstrap.BootstrapList)
        {
            var envSetData = EnvironmentSetManager.SharedInstance.AllCode2Dict[bootData.SetCode];

            if (!bootData.Embedded &&
                !EnvironmentSetManager.SharedInstance.IsLocallyAvailableAndLatestVersion(envSetData.SetId))
            {
                downloadableList.Add(envSetData.SetId); // if all true, add it to 'downloadable' list
            }
        }

        return downloadableList;
    }

    private bool IsCharacterPurchasable(int characterId)
    {
        return (!GameProfile.SharedInstance.Player.IsHeroPurchased(characterId) && // check if already purchased
                GameProfile.SharedInstance.Player.CanAffordHero(characterId)); // check if can purchase
    }

    private List<int> GetCharactersPurchasable()
    {
        var purchaseableList = new List<int>();

        foreach (var characterStats in GameProfile.SharedInstance.Characters)
        {
            if (IsCharacterPurchasable(characterStats.characterId))
            {
                purchaseableList.Add(characterStats.characterId);
            }
        }

        return purchaseableList;
    }

    /// <summary>
    ///     Check if the discount index in range for the pass Discount Item Type
    /// </summary>
    /// <returns>
    ///     true if the index is within range
    /// </returns>
    /// <param name='discountItemType'>
    ///     If set to <c>true</c> discount item type.
    /// </param>
    /// <param name='index'>
    ///     If set to <c>true</c> index.
    /// </param>
    private bool _isDiscountIndexInRange(DiscountItemType discountItemType, int index)
    {
        var result = false;

        switch (discountItemType)
        {
            case DiscountItemType.Artifact:
                if (ArtifactStore.GetArtifactProtoData(index) != null)
                {
                    result = true;
                }
                break;

            case DiscountItemType.Consumable:
                if (ConsumableStore.ConsumableFromID(index) != null)
                {
                    result = true;
                }
                break;

            case DiscountItemType.Character:
                if (GameProfile.SharedInstance.CharacterOrder.Contains(index))
                {
                    result = true;
                }
                break;

            case DiscountItemType.Powerup:
                if (PowerStore.PowerFromID(index) != null)
                {
                    result = true;
                }
                break;
        }

        return result;
    }

    public List<WeeklyDiscountProtoData> GetSalesViewable(DiscountItemType discountItemType)
    {
        var discounts =
            Services.Get<Store>().GetComponent<WeeklyDiscountManager>().GetDiscountsByItemType(discountItemType);

        var itemsToRemove = new List<DiscountItemProtoData>();

        if (discounts.Count > 0)
        {
            foreach (var item in discounts.First()._itemList)
            {
                notify.Debug("[NotificationSystem] - Item Type: " + discountItemType + "  ID " + item._id);

                // To prevent the IndexOutOfRangeException that occurred 05-13-2013, ensure that the
                // current item's type matches the sale's type, and that the item's id is within the 
                // particular store's
                if (item.ItemType == discountItemType && _isDiscountIndexInRange(discountItemType, item._id))
                {
                    switch (discountItemType)
                    {
                        case DiscountItemType.Artifact:
                            if (GameProfile.SharedInstance.Player.IsArtifactMaxedOut(item._id))
                            {
                                itemsToRemove.Add(item);
                            }
                            break;
                        case DiscountItemType.Consumable:
                            if (GameProfile.SharedInstance.Player.IsConsumableMaxedOut(item._id))
                            {
                                itemsToRemove.Add(item);
                            }
                            break;
                        case DiscountItemType.Powerup:
                            if (GameProfile.SharedInstance.Player.IsPowerPurchased(item._id))
                            {
                                itemsToRemove.Add(item);
                            }
                            break;
                        case DiscountItemType.Character:
                            if (GameProfile.SharedInstance.Player.IsHeroPurchased(item._id))
                            {
                                itemsToRemove.Add(item);
                            }
                            break;
                    }
                }
                // Index is either out of range, or the item type doesn't match the sales
                else
                {
                    if (!_isDiscountIndexInRange(discountItemType, item._id))
                    {
                        notify.Error(
                            string.Format(
                                "[NotificationSystem] GetSalesViewable -- Index out of range.  Id: {0}, ShortCode: {1}, ItemType{2}.  Get Bryan A. or Redmond.",
                                item._id, item.ShortCode, item.ItemType));
                    }

                    if (item.ItemType != discountItemType)
                    {
                        notify.Error(
                            string.Format(
                                "[NotificationSystem] GetSalesViewable -- Item type mismatch.  Id: {0}, ShortCode: {1}, ItemType: {2}, Sale's Type: {3}.  Get Bryan A. or Redmond",
                                item._id, item.ShortCode, item.ItemType, discountItemType));
                    }
                }
            }

            foreach (var item in itemsToRemove)
            {
                discounts.First()._itemList.Remove(item);
            }
        }

        return discounts;
    }

    public List<int> GetIDsFromDiscountList(List<WeeklyDiscountProtoData> discountList)
    {
        var idList = new List<int>();

        if (discountList.Count > 0)
        {
            foreach (var item in discountList.First()._itemList)
            {
                idList.Add(item._id);
            }
        }

        return idList;
    }
}