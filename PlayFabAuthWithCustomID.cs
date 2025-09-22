using PlayFab;
using PlayFab.ClientModels;
using Splatform;

public static class PlayFabAuthWithCustomID
{
	public static void Login(PlatformUserID customId)
	{
		if (!customId.IsValid)
		{
			PlayFabManager.instance.OnLoginFailure(null);
			return;
		}
		PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
		{
			CreateAccount = true,
			CustomId = customId.ToString()
		}, OnLoginSuccess, OnLoginFailed);
	}

	private static void OnLoginSuccess(LoginResult result)
	{
		ZLog.Log("Logged in PlayFab user via custom ID");
		PlayFabManager.instance.OnLoginSuccess(result);
	}

	private static void OnLoginFailed(PlayFabError error)
	{
		ZLog.LogError("Failed to logged in PlayFab user via custom ID: " + error.GenerateErrorReport());
		PlayFabManager.instance.OnLoginFailure(error);
	}
}
