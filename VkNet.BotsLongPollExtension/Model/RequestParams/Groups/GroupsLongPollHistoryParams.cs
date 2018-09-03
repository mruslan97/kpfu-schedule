using System;
using VkNet.Model.RequestParams;
using VkNet.Utils;

namespace VkNet.BotsLongPollExtension.Model.RequestParams.Groups
{
	/// <summary>
	/// Параметры bots long poll api
	/// </summary>
	[Serializable]
	public class GroupsLongPollHistoryParams
	{
		/// <summary>
		/// Сервер для подключения Long Poll группы
		/// </summary>
		public string Server { get; set; }

		/// <summary>
		/// Последние полученое событие
		/// </summary>
		public ulong Ts { get; set; }

		/// <summary>
		/// Ключ сессии
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// Время ожидание события
		/// </summary>
		public int Wait { get; set; }

		/// <summary>
		/// Привести к типу VkParameters.
		/// </summary>
		/// <param name="p"> Параметры. </param>
		/// <returns> </returns>
		public static VkParameters ToVkParameters(GroupsLongPollHistoryParams p)
		{
			var parameters = new VkParameters
			{
				{ "ts", p.Ts }
				, { "key", p.Key }
				, { "wait", p.Wait }
				,{"act","a_check"}
			};

			return parameters;
		}
	}
}