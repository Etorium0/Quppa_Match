using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class LocalData : MonoBehaviour
{
    public static LocalData Instance;
    public const string DATA = "Data";

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

    public void SaveUserData(UserData userData)
    {
        string value = JsonConvert.SerializeObject(userData);
        PlayerPrefs.SetString(DATA,value);
    }  
    

    public UserData DefaulUserData()
    {
        UserData newUserData = new UserData();
        newUserData.id =GameDataManager.Instance.userData.id;
        newUserData.username = GameDataManager.Instance.userData.username;
        newUserData.avatar = "no";
        newUserData.balance = 10;
        newUserData.data.Data = new();
        newUserData.data.Data.Add(DataType.HIGH_SCORE, 0);
        newUserData.data.Data.Add(DataType.HINT, 10);
        newUserData.data.Data.Add(DataType.SWAP, 10);
        newUserData.data.Data.Add(DataType.TOTAL_SCORE, 0);
        newUserData.data.Data.Add(DataType.PIKACHU, 0);

        return newUserData;
    }

    public UserData GetUserData()
    {
        string defaulData = JsonConvert.SerializeObject(DefaulUserData());
        string data = PlayerPrefs.GetString(DATA, defaulData);
        return JsonConvert.DeserializeObject<UserData>(data);
    }

}
