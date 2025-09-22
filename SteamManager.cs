using System;
using System.IO;
using System.Linq;
using System.Text;
using Steamworks;
using UnityEngine;

[DisallowMultipleComponent]
public class SteamManager : MonoBehaviour
{
	public static uint[] ACCEPTED_APPIDs = new uint[2] { 1223920u, 892970u };

	public static uint APP_ID = 0u;

	private static int m_serverPort = 2456;

	private static SteamManager s_instance;

	private static bool s_EverInialized;

	private bool m_bInitialized;

	private SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;

	public static SteamManager instance => s_instance;

	public static bool Initialized
	{
		get
		{
			if (s_instance != null)
			{
				return s_instance.m_bInitialized;
			}
			return false;
		}
	}

	public static bool Initialize()
	{
		if (s_instance == null)
		{
			new GameObject("SteamManager").AddComponent<SteamManager>();
		}
		return Initialized;
	}

	private static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
	{
		Debug.LogWarning(pchDebugText);
	}

	public static void SetServerPort(int port)
	{
		m_serverPort = port;
	}

	private uint LoadAPPID()
	{
		string environmentVariable = Environment.GetEnvironmentVariable("SteamAppId");
		if (environmentVariable != null)
		{
			ZLog.Log("Using environment steamid " + environmentVariable);
			return uint.Parse(environmentVariable);
		}
		try
		{
			string s = File.ReadAllText("steam_appid.txt");
			ZLog.Log("Using steam_appid.txt");
			return uint.Parse(s);
		}
		catch
		{
		}
		ZLog.LogWarning("Failed to find APPID");
		return 0u;
	}

	private void Awake()
	{
		if (s_instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		s_instance = this;
		APP_ID = LoadAPPID();
		ZLog.Log("Using steam APPID:" + APP_ID);
		if (!ACCEPTED_APPIDs.Contains(APP_ID))
		{
			ZLog.Log("Invalid APPID");
			Application.Quit();
			return;
		}
		if (s_EverInialized)
		{
			throw new Exception("Tried to Initialize the SteamAPI twice in one session!");
		}
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (!Packsize.Test())
		{
			Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
		}
		if (!DllCheck.Test())
		{
			Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
		}
		try
		{
			if (SteamAPI.RestartAppIfNecessary((AppId_t)APP_ID))
			{
				Application.Quit();
				return;
			}
		}
		catch (DllNotFoundException ex)
		{
			Debug.LogError("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + ex, this);
			Application.Quit();
			return;
		}
		m_bInitialized = GameServer.Init(0u, (ushort)m_serverPort, (ushort)(m_serverPort + 1), EServerMode.eServerModeNoAuthentication, "1.0.0.0");
		if (!m_bInitialized)
		{
			Debug.LogError("[Steamworks.NET] GameServer.Init() failed.", this);
			return;
		}
		SteamGameServer.SetProduct("valheim");
		SteamGameServer.SetModDir("valheim");
		SteamGameServer.SetDedicatedServer(bDedicated: true);
		SteamGameServer.SetMaxPlayerCount(64);
		SteamGameServer.LogOnAnonymous();
		ZLog.Log("Server ID " + SteamGameServer.GetSteamID().ToString());
		ZSteamMatchmaking.Initialize();
		ZLog.Log("Authentication:" + SteamGameServerNetworkingSockets.InitAuthentication());
		ZLog.Log("Steam game server initialized");
		s_EverInialized = true;
	}

	private void OnEnable()
	{
		if (s_instance == null)
		{
			s_instance = this;
		}
		_ = m_bInitialized;
	}

	private void OnDestroy()
	{
		ZLog.Log("Steam manager on destroy");
		if (!(s_instance != this))
		{
			s_instance = null;
			if (m_bInitialized)
			{
				GameServer.Shutdown();
			}
		}
	}

	private void Update()
	{
		if (m_bInitialized)
		{
			GameServer.RunCallbacks();
		}
	}
}
