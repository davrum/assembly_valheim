using Splatform;

internal struct ServerJoinDataAndHostUser
{
	public ServerJoinData m_joinData;

	public PlatformUserID m_hostUser;

	public ServerJoinDataAndHostUser(ServerJoinData joinData, PlatformUserID hostUser)
	{
		m_joinData = joinData;
		m_hostUser = hostUser;
	}
}
