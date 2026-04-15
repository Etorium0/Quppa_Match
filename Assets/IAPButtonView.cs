using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using System;
public class IAPButtonView : MonoBehaviour
{
    [SerializeField] int inappIndex = 0;
    [SerializeField] Text price;
    [SerializeField] Text title;
    private int coin = 50;
    string productId = "coin_inapp_1";
    Button buyBtn;

    private void Awake()
    {
        buyBtn = GetComponent<Button>();
        buyBtn.onClick.AddListener(OnBuyButtonClicked);
        productId = IAPManager.Instance.GetProductId(inappIndex);
/*#if !UNITY_EDITOR
        OnProductFetched(IAPManager.Instance.GetProductById(productId));
#endif*/
    }
    
    void OnEnable()
    {
        StartCoroutine(LoadProduct());
        
        //IAPManager.SetBuyButtonState += SetActiveBuyBtn;
        //ShopWindowBehavior.OnShopWindowOpened += FetchedInapp;
    }
    /*void OnDisable()
    {
        IAPManager.SetBuyButtonState -= SetActiveBuyBtn;
        //ShopWindowBehavior.OnShopWindowOpened -= FetchedInapp;
    }*/

    IEnumerator LoadProduct()
    {
        while (!IAPManager.Instance.canShowShop)
        {
            yield return null;
        }
        OnProductFetched(IAPManager.Instance.GetProductById(productId));
    }

    private void SetActiveBuyBtn(bool isActive)
    {
        Debug.Log("[IAP] Setting buy button active state to: " + isActive);
        buyBtn.gameObject.SetActive(isActive);
    }

    private void OnBuyButtonClicked()
    {
        IAPManager.Instance.BuyProduct(this.productId, () =>
        {
            OnPurchaseSuccess();
        });
    }

    public void OnProductFetched(Product product)
    {
        if (product == null || !product.availableToPurchase)
        {
            Debug.LogError($"[IAP] Product with ID {productId} not found or not available for purchase.");
            gameObject.SetActive(false);
            return;
        }
        if (product.availableToPurchase)
        {
            string tile = product.metadata.localizedTitle;
            coin = int.Parse(tile.Split(' ')[0]);
            title.text = tile.Split('(')[0];
            price.text = product.metadata.localizedPriceString;
        }
    }
    
    public void OnPurchaseSuccess()
    {
        GameDataManager.Instance.AddCoinTimer(coin);
    }
}
