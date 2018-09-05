using System;
using VkNet.BotsLongPollExtension.Enums;
using VkNet.Utils;

namespace VkNet.BotsLongPollExtension.Model.GroupHistoryModel
{
	/// <summary>
	/// Редактирование списка руководителей
	/// </summary>
	[Serializable]
	public class GroupOfficersEdit
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
		/// Новый уровень полномочий
		/// </summary>
		public GroupOfficerLevel? LevelNew { get; set; }

		/// <summary>
		/// Старый уровень полномочий
		/// </summary>
		public GroupOfficerLevel? LevelOld { get; set; }

		/// <summary>
		/// Разобрать из json.
		/// </summary>
		/// <param name="response"> Ответ сервера. </param>
		public static GroupOfficersEdit FromJson(VkResponse response)
		{
			return new GroupOfficersEdit
			{
				UserId = response["user_id"],
				AdminId = response["admin_id"],
				LevelNew = (GroupOfficerLevel)((int)response["level_new"]),
				LevelOld = (GroupOfficerLevel)((int)response["level_old"])
			};
		}
	}
}