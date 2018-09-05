using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VkNet.BotsLongPollExtension.Enums;
using VkNet.Utils;

namespace VkNet.BotsLongPollExtension.Model.GroupHistoryModel
{
	/// <summary>
	/// Добавление пользователя в чёрный список
	/// </summary>
	[Serializable]
	public class UserBlock
	{
		/// <summary>
		/// Идентификатор пользователя
		/// </summary>
		public long? UserId { get; set; }

		/// <summary>
		/// Идентификатор администратора, который внёс пользователя в чёрный список
		/// </summary>
		public long? AdminId { get; set; }

		/// <summary>
		/// Дата разблокировки
		/// </summary>
		[JsonConverter(typeof(UnixDateTimeConverter))]
		public DateTime? UnblockDate { get; set; }

		/// <summary>
		/// Причины блокировки пользователя
		/// </summary>
		public GroupUserBlockReason? Reason { get; set; }

		/// <summary>
		/// Комментарий администратора к блокировке
		/// </summary>
		public string Comment { get; set; }

		/// <summary>
		/// Разобрать из json.
		/// </summary>
		/// <param name="response"> Ответ сервера. </param>
		public static UserBlock FromJson(VkResponse response)
		{
			return new UserBlock
			{
				UserId = response["user_id"],
				AdminId = response["admin_id"],
				Comment = response["comment"],
				Reason = (GroupUserBlockReason) ((int) response["reason"]),
				UnblockDate = response["unblock_date"]
			};
		}
	}
}