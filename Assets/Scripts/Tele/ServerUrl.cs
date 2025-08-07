using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_WEBGL
public static class ServerUrl
{
    const string SERVER_URL = "";
    public const string LOGIN = SERVER_URL + "/login";
    public const string SIGN_UP = SERVER_URL + "/signup";
    public const string DEPOSIT = SERVER_URL + "/deposit";
    public const string SPEND = SERVER_URL + "/spend";
    public const string BALANCE = SERVER_URL + "/balance";
    public const string UPDATE_DATA = SERVER_URL + "/data";
}
#endif
   