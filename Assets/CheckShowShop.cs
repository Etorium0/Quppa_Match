using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class CheckShowShop : MonoBehaviour
{
    public GameObject shopBtn;

    public void ShowShop(Product product)
    {
        if (product.availableToPurchase)
        {
            shopBtn.SetActive(true);
        }

    }
}
