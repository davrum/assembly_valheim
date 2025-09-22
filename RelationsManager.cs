using Splatform;
using UnityEngine;

public static class RelationsManager
{
	public const string c_AuthorHostPlaceholder = "host";

	public static bool PlatformRequiresTextFiltering()
	{
		if (PlatformManager.DistributionPlatform.Platform == "Xbox")
		{
			return true;
		}
		if (PlatformManager.DistributionPlatform.HardwareInfoProvider != null && PlatformManager.DistributionPlatform.HardwareInfoProvider.HardwareInfo.m_category == HardwareCategory.Console)
		{
			return true;
		}
		return false;
	}

	public static bool FilterTextCommunicationSentToUser(PlatformUserID recipient)
	{
		if (!PlatformRequiresTextFiltering())
		{
			return false;
		}
		if (recipient == PlatformManager.DistributionPlatform.LocalUser.PlatformUserID)
		{
			return false;
		}
		IRelationsProvider relationsProvider = PlatformManager.DistributionPlatform.RelationsProvider;
		if (relationsProvider == null)
		{
			return true;
		}
		if (relationsProvider.IsFriend(recipient))
		{
			return false;
		}
		return true;
	}

	public static void CheckPermissionAsync(PlatformUserID user, Permission permission, bool isSender, CheckPermissionCompletedHandler completedHandler)
	{
		completedHandler(RelationsManagerPermissionResult.Granted);
	}

	private static bool TryCheckEquivalentPrivilege(Permission permission, out bool result)
	{
		Privilege privilege;
		switch (permission)
		{
		case Permission.PlayMultiplayerWith:
			privilege = Privilege.OnlineMultiplayer;
			break;
		case Permission.CommunicateWithUsingText:
			privilege = Privilege.TextCommunication;
			break;
		case Permission.ViewUserGeneratedContent:
			privilege = Privilege.ViewUserGeneratedContent;
			break;
		default:
			ZLog.LogError($"Failed to check equivalent privilege for permission {permission}: There is no equivalent privilege");
			result = false;
			return false;
		}
		PrivilegeResult privilegeResult = PlatformManager.DistributionPlatform.PrivilegeProvider.CheckPrivilege(privilege);
		if (privilegeResult.IsError())
		{
			ZLog.LogError($"Failed to check privilege {privilege}: {privilegeResult}");
			result = false;
			return false;
		}
		result = privilegeResult.IsGranted();
		return true;
	}

	public static bool UpdateAuthorIfHost(string authorString, ref string resolvedAuthor)
	{
		if (authorString != "host")
		{
			return false;
		}
		if (!ZNet.instance.IsCurrentServerDedicated())
		{
			return false;
		}
		if (ZNet.instance.GetPlayerList().Count <= 0)
		{
			return false;
		}
		PlatformUserID id = ZNet.instance.GetPlayerList()[0].m_userInfo.m_id;
		if (!id.IsValid)
		{
			Debug.LogWarning("Server host lacked valid ID while trying to resolve unclaimed object authorship.");
			return false;
		}
		Debug.Log("There was an update from a placeholder PlatformUserID to the following:" + id.ToString());
		resolvedAuthor = id.ToString();
		return true;
	}

	public static bool IsBlocked(PlatformUserID user)
	{
		return false;
	}
}
