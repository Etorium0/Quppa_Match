using DG.Tweening.Core.Easing;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using EventManager;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;
    public UserData userData;

    public static Action<string> OnGetUserID;
    public static UnityEvent OnSaveUserData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
#if !UNITY_WEBGL
        userData = LocalData.Instance.GetUserData();
        SceneManager.LoadScene(1);
#endif
    }

#if UNITY_WEBGL
    IEnumerator SendRequest(string url, string data, Action<string> completeAction, Action onFailAction)
    {
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(url, ""))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(data);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                onFailAction?.Invoke();
            }
            else
            {
                Debug.Log(www.downloadHandler.text);

                completeAction?.Invoke(www.downloadHandler.text);
            }
        }
    }
#endif

    public void UpdateData()
    {
        LocalData.Instance.SaveUserData(userData);
        string data = JsonConvert.SerializeObject(userData);
#if UNITY_WEBGL
        StartCoroutine(SendRequest(ServerUrl.UPDATE_DATA, data, null, null));
#endif
    }

    public void Login()
    {
        Debug.Log("Login");

        var _userID = new { id = userData.id };

        string data = JsonConvert.SerializeObject(_userID);
        Debug.Log(data);
#if UNITY_WEBGL
        StartCoroutine(SendRequest(ServerUrl.LOGIN, data, OnLoginSucess, OnLoginFail));
#endif
    }
    public void SignUp()
    {
        Debug.Log("SignUp");

        userData = LocalData.Instance.DefaulUserData();

        string data = JsonConvert.SerializeObject(userData);
#if UNITY_WEBGL
        StartCoroutine(SendRequest(ServerUrl.SIGN_UP, data, OnSignUpSucess, OnSignUpFail));
#endif

    }

    public void AddCoin(int value)
    {
        userData.balance += value;
        var addCoin = new { id = userData.id, amount = value };
        string data = JsonConvert.SerializeObject(addCoin);
#if UNITY_WEBGL
        StartCoroutine(SendRequest(ServerUrl.DEPOSIT, data, null, null));
#endif

    }

    public void SpendCoin(int value)
    {
        userData.balance += value; // value < 0
        var spendCoin = new { id = userData.id, amount = value };
        string data = JsonConvert.SerializeObject(spendCoin);

#if UNITY_WEBGL
        StartCoroutine(SendRequest(ServerUrl.SPEND, data, null, null));
#endif

    }

    void OnLoginSucess(string data)
    {
        Debug.Log("LoginSucess");
        JObject jsonData = JObject.Parse(data);
        string userDataJson = jsonData["data"].ToString();
        userData = JsonConvert.DeserializeObject<UserData>(userDataJson);
        userData.data ??= LocalData.Instance.DefaulUserData().data;
        UpdateData();
        LocalData.Instance.SaveUserData(userData);
        SceneManager.LoadScene(1);
    }

    void OnLoginFail()
    {
        Debug.Log("LoginFail");
        SignUp();
    }

    void OnSignUpSucess(string data)
    {
        LocalData.Instance.SaveUserData(userData);
        Debug.Log("SignUp Sucess");
        SceneManager.LoadScene(1);
    }

    void OnSignUpFail()
    {
        Debug.Log("SignUp Fail");

        userData = LocalData.Instance.GetUserData();
        Debug.Log(JsonConvert.SerializeObject(userData));
        SceneManager.LoadScene(1);
        //Load Local Data
    }


    public void AddCoinTimer(int coins)
    {
        StartCoroutine(GetCoin(coins));
    }

    IEnumerator GetCoin(int coin)
    {
#if UNITY_WEBGL
        yield return new WaitForSeconds(3);
#endif
        yield return null;
        //PlayfabManager.Instance.SetUserData();
        //CoinManager.Instance.AddCoin(coin);
        GameDataMgr.Coin += coin;
        this.PostEvent(EventID.UpdateCoin);
        SingletonMonoBehaviour<DialogMessage>.Instance.OpenDialog("Purchase Success", "+" +coin+" coins !!", null);
        LocalData.Instance.SaveUserData(userData);
    }


    public void UpdateData(DataType dataType,int value)
    {
        if (userData.data.Data.ContainsKey(dataType))
        {
            userData.data.Data[dataType] =value;
            UpdateData();
        }
    }
    
}

[System.Serializable]
public class GameDataTele
{
    public Dictionary<DataType, int> Data = new Dictionary<DataType, int>();
}

public enum DataType
{
    HIGH_SCORE,
    HINT,
    SWAP,
    TOTAL_SCORE,
    PIKACHU
}

[System.Serializable]
public class UserData
{
    public long id;
    public string username;
    public string avatar;
    public int balance;
    public GameDataTele data = new();
}
