using System;

namespace VkNet.BotsLongPollExtension.Exception
{
	/// <summary>
	/// Информация утрачена, нужно запросить новые key и ts методом groups.getLongPollServer.
	/// </summary>
	[Serializable]
	public class BotsLongPollHistoryInfoLostException : BotsLongPollHistoryException
	{
		/// <inheritdoc />
		public BotsLongPollHistoryInfoLostException():base(3, "Информация утрачена, нужно запросить новые key и ts методом groups.getLongPollServer.") { }
	}
}