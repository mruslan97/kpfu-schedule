using System;
using VkNet.Utils;

namespace VkNet.BotsLongPollExtension.Exception
{
	/// <summary>
	/// Ошибка при получении истории обновлений
	/// </summary>
	[Serializable]
	public class BotsLongPollHistoryException : System.Exception
	{
		/// <summary>
		/// Код ошибки
		/// </summary>
		public int Code { get; set; }

		/// <inheritdoc />
		public BotsLongPollHistoryException() { }

		/// <inheritdoc />
		public BotsLongPollHistoryException(int code, string message) : base(message)
		{
			Code = code;
		}
	}
}