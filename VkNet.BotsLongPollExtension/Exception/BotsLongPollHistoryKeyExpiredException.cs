using System;

namespace VkNet.BotsLongPollExtension.Exception
{
	/// <summary>
	/// Истекло время действия ключа, нужно заново получить key методом groups.getLongPollServer.
	/// </summary>
	[Serializable]
	public class BotsLongPollHistoryKeyExpiredException : BotsLongPollHistoryException
	{
		/// <inheritdoc />
		public BotsLongPollHistoryKeyExpiredException() : base(2, "Истекло время действия ключа, нужно заново получить key методом groups.getLongPollServer.") { }
	}
}