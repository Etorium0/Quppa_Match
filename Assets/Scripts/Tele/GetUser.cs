using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

#if UNITY_WEBGL
public class GetUser : MonoBehaviour
{
    long userID;
    string userName;

    [DllImport("__Internal")]
    private static extern void GetUserInfo();

    [DllImport("__Internal")]
    private static extern void InitTeleWebApp();


    private void Start()
    {
        InitTeleWebApp();
        GetUserInfo();
        //GameDataManager.Instance.Login();
    }

    public void CallBackUserData(string userData)
    {            
        JObject jsonData = JObject.Parse(userData);
        long id = (long)jsonData["id"];
        userName = (string)jsonData["username"];      
        userID = id;
        GameDataManager.Instance.userData.id = id;
        GameDataManager.Instance.userData.username = userName;
        GameDataManager.Instance.Login();
    }
}
#endif
