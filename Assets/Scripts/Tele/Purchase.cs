using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;

#if UNITY_WEBGL
public class Purchase : MonoBehaviour
{
    string botToken = "";

    public string invoiceLink;
    int coinPack;



    public static string GenerateSecureToken(int size)
    {
        byte[] tokenData = new byte[size];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(tokenData);
        }
        return Convert.ToBase64String(tokenData);
    }

    public void BuyIapp(int coinPack)
    {
        switch (coinPack)
        {
            case 1:
                BuyInApp(500, coinPack, 50);              
                break;
            case 2:
                BuyInApp(1000, coinPack, 100);
               
                break;
            case 3:
                BuyInApp(1500, coinPack, 150);
               
                break;
            case 4:
                BuyInApp(3000, coinPack, 250);
              
                break;
            case 5:
                BuyInApp(5000, coinPack, 400);
              
                break;
            case 6:
                BuyInApp(6500, coinPack, 500);
                break;
            case 7:
                BuyInApp(13000, coinPack, 1000);
                break;
            case 8:
                BuyInApp(34000, coinPack, 2500);
                break;
        }
    }

    void BuyInApp(int coin, int coinPack, int price)
    {
        string _inAppName = coin + " Coins";
        string _priceLabel = "Coins Pack " + coinPack;
        CreateLink(_inAppName, CreateLabelPrice(_priceLabel, price));
        GameDataManager.Instance.AddCoinTimer(coin);      
    }

    public string CreateLabelPrice(string label, int price)
    {
        var a = new PriceLabel { label = "Coins " + label, amount = price };
        List<PriceLabel> items = new List<PriceLabel>(1);
        items.Add(a);

        string labelPrice = JsonConvert.SerializeObject(items); 

        Debug.Log(labelPrice);
        return labelPrice;
    }

    public void CreateLink(string inappName, string labelPrice)
    {       
        StartCoroutine(CreateInvoiceLink1(inappName, labelPrice));
    }

    IEnumerator CreateInvoiceLink1(string inappName, string labelPrice)
    {
        
        WWWForm form = new WWWForm();
        form.AddField("title", inappName);
        form.AddField("description", "Buy " + inappName);
        form.AddField("payload", GenerateSecureToken(32));
        form.AddField("provider_token", "");
        form.AddField("currency", "XTR");
        form.AddField("prices", labelPrice);

        string createInvoiceUrl = "https://api.telegram.org/bot" + botToken + "/createInvoiceLink";

        using (UnityWebRequest request = UnityWebRequest.Post(createInvoiceUrl, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("POST Success: " + request.downloadHandler.text);
                //message.text = request.downloadHandler.text;
                JObject jsonObject = JObject.Parse(request.downloadHandler.text);
                invoiceLink = (string)jsonObject["result"];
                ClickOpenInvoice();
            }
            else
            {
                Debug.Log("POST Error: " + request.error);
                //message.text = request.error;
            }
        }
    }

    [DllImport("__Internal")]
    private static extern void OpenInvoice(string mess);
    public void ClickOpenInvoice()
    {
        OpenInvoice(invoiceLink);
    }


    public void ShowLink(string mess)
    {
        Debug.Log(mess);
    }

    public void ShowPurchaseLog(string mess)
    {   
        //CoinManager.Instance.AddCoin(5000);
      
    }
}

class Test
{
    public bool ok;
    public string result;
}

class PriceLabel
{
    public string label { get; set; }
    public int amount { get; set; }
}
#endif