using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class CheckShowShop : MonoBehaviour
{
    [SerializeField] private GameObject shopBtn;

    void OnEnable()
    {
        IAPManager.OnProductFetched += ShowShop;
    }

    void OnDisable()
    {
        IAPManager.OnProductFetched -= ShowShop;
    }

    void Start()
    {
        ShowShop(IAPManager.Instance.GetProductById(IAPManager.Instance.GetProductId(0)));
    }

    public void ShowShop(Product product)
    {
        if (product.availableToPurchase)
        {
            shopBtn.SetActive(true);
        }
    }
}
