using System.Collections.Generic;
using EventManager;
using UnityEngine;

public class StorePanel : MonoBehaviour
{
    public GameObject panelInAppTele, panelInAppGG;

    public void ShowShopPanel()
    {
#if UNITY_WEBGL
        panelInAppTele.SetActive(true);
        panelInAppGG.SetActive(false);

#else
        panelInAppTele.SetActive(false);
        panelInAppGG.SetActive(true);
#endif

    }
    public void ClickBuyItem(int itemID)
    {
        switch (itemID)
        {
            case 1:
                if (GameDataMgr.Coin < 20)
                {
                    SingletonMonoBehaviour<DialogMessage>.Instance.OpenDialog("Purchase Failed", "You don't have enough coins", null);
                    break;
                }
                GameDataMgr.Coin -= 20;
                GameDataMgr.Hint += 2;
                this.PostEvent(EventID.UpdateCoin);
                this.PostEvent(EventID.UpdateHint);
                SingletonMonoBehaviour<DialogMessage>.Instance.OpenDialog("Notice", "Purchase Success", null);
                break;
            case 2:
                if (GameDataMgr.Coin < 20)
                {
                    SingletonMonoBehaviour<DialogMessage>.Instance.OpenDialog("Purchase Failed", "You don't have enough coins", null);
                    break;
                }
                GameDataMgr.Coin -= 20;           
                GameDataMgr.Swap += 2;
                this.PostEvent(EventID.UpdateCoin);
                this.PostEvent(EventID.UpdateSwap);
                SingletonMonoBehaviour<DialogMessage>.Instance.OpenDialog("Notice", "Purchase Success", null);
                break;
            case 3:
                if (GameDataMgr.Coin < 500)
                {
                    SingletonMonoBehaviour<DialogMessage>.Instance.OpenDialog("Purchase Failed", "You don't have enough coins", null);
                    break;
                }
                GameDataMgr.Coin -= 500;
                GameDataMgr.Hint += 25;
                GameDataMgr.Swap += 25;
                this.PostEvent(EventID.UpdateCoin);
                this.PostEvent(EventID.UpdateSwap);
                SingletonMonoBehaviour<DialogMessage>.Instance.OpenDialog("Notice", "Purchase Success", null);
                break;
        }
    }

    public void WatchAds()
    {

        //AdsManager.Instance.ShowRewardedAd();

    }

    private void CompleteMethod(bool completed, string advertiser)
    {
        Debug.Log("Closed rewarded from: " + advertiser + " -> Completed " + completed);
        if (completed == true)
        {
            //give the reward
            //SingletonMonoBehaviour<DialogMessage>.Instance.OpenDialog("Success", "Get 5 coin", delegate
            //{
            GameDataMgr.Coin += 5;
            this.PostEvent(EventID.UpdateCoin);
            //});
        }
        else
        {
            //no reward
        }
    }

    public void BuyInApp(int inapp)
    {
        switch (inapp)
        {
            case 1:
                GameDataManager.Instance.AddCoinTimer(500);
                break;
            case 2:
                GameDataManager.Instance.AddCoinTimer(1000);
                break;
            case 3:
                GameDataManager.Instance.AddCoinTimer(1500);
                break;
            case 4:
                GameDataManager.Instance.AddCoinTimer(3000);
                break;
            case 5:
                GameDataManager.Instance.AddCoinTimer(5000);
                break;
            case 6:
                GameDataManager.Instance.AddCoinTimer(6500);
                break;
            case 7:
                GameDataManager.Instance.AddCoinTimer(13000);
                break;
            case 8:
                GameDataManager.Instance.AddCoinTimer(34000);
                break;
        }
    }
  
}
