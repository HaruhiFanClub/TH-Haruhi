// android callback unity

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AndroidCallBack : MonoBehaviour
{
	/*
	private void OnPurchaseSuccCallback(string jsonParam)
	{
		//TopUpMgr.GetInstance().RpcTopUp(jsonParam);
	}

	private void OnGetSkusCallback(string jsonParam)
	{
		//TopUpMgr.GetInstance().OnAllSkuGeted(jsonParam);
	}
	
	private void LoginCallBack(string jsonParam)
	{
		TTSDK.LoginCallback(jsonParam);
	}

	private void IOSLoginOK(string code)
	{
		TTSDK.LoginCallbackByCode(code);
	}
	
	private void IOSLoginFailed(string msg)
	{
		TTSDK.LoginCallbackFailed(msg);
	}

	private void Log(string log)
	{
		Debug.LogWarning("[IOS]" + log);
	}

	private void OnQuestionFresh(string json)
	{
		Debug.Log("OnQuestionFresh " + json);
		GameEventCenter.Send(GameEvent.OnQuestionFresh);
	}

	private void OnQuestionFinish(string json)
	{
		Debug.Log("OnQuestionFinish " + json);
		GameEventCenter.Send(GameEvent.OnQuestionFinish);
	}

	private void ShowPhoto(string img)
	{
		UploadSDK.OnGetPhotoDate(img);
	}

	private void CancelShowPhoto(string s)
	{
		UploadSDK.OnCancel();
	} 
	
	//支付宝支付回调
	private void AliPayFinish(string result)
	{
		//9000代表成功
		Debug.Log("AliPayFinish " + result);
		PaySDK.OnPayResult(result == "9000");
	}

	//微信支付回调
	private void WXPayFinish(string errCode)
	{
		Debug.Log("WXPayResult " + errCode);
		PaySDK.OnPayResult(errCode == "0");
	}
	*/
}