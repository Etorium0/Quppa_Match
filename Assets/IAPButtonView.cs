using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
public class IAPButtonView : MonoBehaviour
{
  
    [SerializeField]Text price;
    [SerializeField]Text title;
    private int coin;

    public void OnProductFetched(Product product)
    {
        if (product.availableToPurchase)
        {
            string tile = product.metadata.localizedTitle;
            coin = int.Parse(tile.Split(' ')[0]); 
            title.text = tile.Split('(')[0];
            price.text = product.metadata.localizedPriceString;
        }
    }


    public void BuySuccess(){
        GameDataManager.Instance.AddCoinTimer(coin);
    }
}
