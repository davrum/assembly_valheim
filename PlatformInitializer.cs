using System;
using System.Threading;
using Splatform;
using UnityEngine;

public static class PlatformInitializer
{
	private static bool s_platformInitialized = false;

	private static bool s_loginFinished = false;

	private static bool s_startedStorageInitialization = false;

	private static bool s_allowStorageInitialization = true;

	private static bool s_inputDeviceRequired = false;

	public static bool PlatformInitialized => s_platformInitialized;

	public static bool StartedSaveDataInitialization => s_startedStorageInitialization;

	public static bool SaveDataInitialized => true;

	public static bool AllowSaveDataInitialization
	{
		get
		{
			return s_allowStorageInitialization;
		}
		set
		{
			s_allowStorageInitialization = value;
		}
	}

	public static bool InputDeviceRequired
	{
		get
		{
			return s_inputDeviceRequired;
		}
		set
		{
			s_inputDeviceRequired = value;
			if (PlatformManager.DistributionPlatform != null && PlatformManager.DistributionPlatform.InputDeviceManager != null)
			{
				PlatformManager.DistributionPlatform.InputDeviceManager.SetInputDeviceRequiredForLocalUser(requireGamepad: false, ZInput.CheckKeyboardMouseConnected);
			}
		}
	}

	public static bool WaitingForInputDevice
	{
		get
		{
			if (PlatformManager.DistributionPlatform == null)
			{
				return false;
			}
			if (PlatformManager.DistributionPlatform.InputDeviceManager == null)
			{
				return false;
			}
			return !PlatformManager.DistributionPlatform.InputDeviceManager.HasInputDeviceAssociation(PlatformManager.DistributionPlatform.LocalUser.PlatformUserID);
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	private static void EarlyInitialize()
	{
		if (!Application.isEditor)
		{
			GameObject.Find("");
		}
	}

	private static void InitializePlatform()
	{
		SetMainThreadName();
		ParseArguments();
		SteamManager.Initialize();
		Splatform.Logger.SetLogHandler(OnSplatformLog);
		PlatformManager.InitializeAsync(default(PlatformConfiguration), OnInitializeCompleted);
	}

	private static void SetMainThreadName()
	{
		if (string.IsNullOrEmpty(Thread.CurrentThread.Name))
		{
			Thread.CurrentThread.Name = "MainValheimThread";
		}
	}

	private static void ParseArguments()
	{
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			_ = commandLineArgs[i];
		}
	}

	private static void OnInitializeCompleted(bool succeeded)
	{
		if (!succeeded)
		{
			ZLog.LogError("Failed to initialize platform!");
			Application.Quit();
			return;
		}
		SuspendManager.Initialize();
		s_platformInitialized = true;
		ZLog.Log("Initialized platform!");
		PlatformManager.DistributionPlatform.LocalUser.SignedIn += OnLoginCompleted;
	}

	private static void OnLoginCompleted()
	{
		PlatformManager.DistributionPlatform.LocalUser.SignedIn -= OnLoginCompleted;
		MatchmakingManager.Initialize();
		if (s_allowStorageInitialization)
		{
			InitializeSaveDataStorage();
		}
	}

	private static void InitializeSaveDataStorage()
	{
		s_startedStorageInitialization = true;
		if (PlatformManager.DistributionPlatform.SaveDataProvider == null)
		{
			return;
		}
		PlatformManager.DistributionPlatform.SaveDataProvider.InitializeAsync(delegate(bool succeeded)
		{
			if (succeeded)
			{
				if (FileHelpers.LocalStorageSupported)
				{
					string[] files = FileHelpers.GetFiles(FileHelpers.FileSource.Local, Utils.GetSaveDataPath(FileHelpers.FileSource.Local));
					string text = "All files in local storage save data:";
					for (int i = 0; i < files.Length; i++)
					{
						text += $"\n{files[i]} ({FileHelpers.GetFileSize(files[i], FileHelpers.FileSource.Local)})";
					}
					ZLog.Log(text);
				}
				else
				{
					ZLog.Log("Local storage is not supported");
				}
				if (FileHelpers.CloudStorageSupported && FileHelpers.CloudStorageEnabled)
				{
					string[] files = FileHelpers.GetFiles(FileHelpers.FileSource.Cloud, Utils.GetSaveDataPath(FileHelpers.FileSource.Cloud));
					string text = "All files in platform save data:";
					for (int j = 0; j < files.Length; j++)
					{
						text += $"\n{files[j]} ({FileHelpers.GetFileSize(files[j], FileHelpers.FileSource.Cloud)})";
					}
					ZLog.Log(text);
				}
				else
				{
					ZLog.Log("Cloud storage is not supported or enabled");
				}
			}
		});
	}

	private static void OnSplatformLog(LogType logType, object message)
	{
		switch (logType)
		{
		case LogType.Error:
			ZLog.LogError(message);
			break;
		case LogType.Warning:
			ZLog.LogWarning(message);
			break;
		case LogType.Log:
			ZLog.Log(message);
			break;
		default:
			ZLog.LogError($"Log type {logType} not implemented! Log message:\n{message}");
			break;
		}
	}
}
