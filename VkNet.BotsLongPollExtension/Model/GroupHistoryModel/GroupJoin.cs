using System;
using Newtonsoft.Json;
using VkNet.BotsLongPollExtension.Enums;
using VkNet.BotsLongPollExtension.Utils.JsonConverter;
using VkNet.Utils;

namespace VkNet.BotsLongPollExtension.Model.GroupHistoryModel
{
	/// <summary>
	/// Добавление участника или заявки на вступление в сообщество
	/// </summary>
	[Serializable]
	public class GroupJoin
	{
		/// <summary>
		/// Идентификатор пользователя
		/// </summary>
		public long? UserId { get; set; }

		/// <summary>
		/// Указывает, как именно был добавлен участник.
		/// </summary>
		[JsonProperty("join_type")]
		[JsonConverter(typeof(EnumJsonConverter))]
		public GroupJoinType? JoinType { get; set; }

		/// <summary>
		/// Разобрать из json.
		/// </summary>
		/// <param name="response"> Ответ сервера. </param>
		public static GroupJoin FromJson(VkResponse response)
		{
			var groupJoin = JsonConvert.DeserializeObject<GroupJoin>(response.ToString());
			groupJoin.UserId = response["user_id"];
			return groupJoin;
		}
	}
}