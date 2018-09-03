using System;

namespace VkNet.BotsLongPollExtension.Exception
{
	/// <summary>
	/// История событий устарела или была частично утеряна, приложение может получать события далее, используя новое значение ts из ответа.
	/// </summary>
	[Serializable]
	public class BotsLongPollHistoryOutdateException : BotsLongPollHistoryException
	{
		/// <summary>
		/// Значение для кода ошибки - 1
		/// </summary>
		public ulong Ts { get; set; }

		/// <inheritdoc />
		public BotsLongPollHistoryOutdateException(ulong ts) : base(1, "История событий устарела или была частично утеряна, приложение может получать события далее, используя новое значение ts из ответа.")
		{
			Ts = ts;
		}
	}
}