using System;

public struct ServerNameAtTimePoint
{
	public static readonly ServerNameAtTimePoint None;

	public readonly string m_name;

	public readonly DateTime m_timestampUtc;

	public ServerNameAtTimePoint(string serverName, DateTime timestampUtc)
	{
		m_name = serverName;
		m_timestampUtc = timestampUtc;
	}
}
