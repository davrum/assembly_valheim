using System;

public struct SteamworksUserHostedSearchTimeout
{
	public ServerJoinDataSteamUser m_server;

	public DateTime m_refreshStartTimeUtc;

	public SteamworksUserHostedSearchTimeout(ServerJoinDataSteamUser server, DateTime refreshStartTimeUtc)
	{
		m_server = server;
		m_refreshStartTimeUtc = refreshStartTimeUtc;
	}
}
