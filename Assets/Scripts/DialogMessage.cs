using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogMessage : SingletonMonoBehaviour<DialogMessage>
{
	[SerializeField]
	private Text tileText;

	[SerializeField]
	private Text messageText;

	[SerializeField]
	private Text noText;

	[SerializeField]
	private Text yesText;

	[SerializeField]
	private GameObject dialogPanel;

	[SerializeField]
	private GameObject okObj;

	[SerializeField]
	private GameObject yesObj;

	[SerializeField]
	private GameObject noObj;

	private Action okAction;

	private Action cancelAction;

	public void OpenDialog(string title, string message, Action okAction)
	{
		dialogPanel.SetActive(value: true);
		tileText.text = title;
		messageText.text = message;
		this.okAction = okAction;
		yesObj.SetActive(value: false);
		noObj.SetActive(value: false);
		okObj.SetActive(value: true);
	}

	public void OpenDialog(string title, string message, string no, string yes, Action okAction, Action cancelAction = null)
	{
		dialogPanel.SetActive(value: true);
		tileText.text = title;
		messageText.text = message;
		noText.text = no;
		yesText.text = yes;
		this.okAction = okAction;
		this.cancelAction = cancelAction;
		yesObj.SetActive(value: true);
		noObj.SetActive(value: true);
		okObj.SetActive(value: false);
	}
    public GameMgr gamemgr;
	public void OnPressOkButton()
	{
        //AdsManager.Instance.ShowRewardedAd();
    }
    private void CompleteMethod(bool completed, string advertiser)
    {
        Debug.Log("Closed rewarded from: " + advertiser + " -> Completed " + completed);
        if (completed == true)
        {
            //give the reward
            gamemgr.Revive(true);
            //if (okAction != null)
            //{
            //    okAction();
            //}
            Time.timeScale = 1.0f;
            StartCoroutine(WaitAndPrint());
        }
        else
        {
            //no reward
        }
    }

    private IEnumerator WaitAndPrint()
    {
        yield return new WaitForSeconds(0.1f);
        dialogPanel.SetActive(false);
    }

    public void OnPressCancelButton()
	{
		dialogPanel.SetActive(value: false);
		if (cancelAction != null)
		{
			cancelAction();
		}
	}

	public void OnPressCloseButton()
	{
		dialogPanel.SetActive(value: false);
		if (cancelAction != null)
		{
			cancelAction();
		}
	}
}
