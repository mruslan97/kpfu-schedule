using System;
using Newtonsoft.Json;

namespace VkNet.BotsLongPollExtension.Utils.JsonConverter
{
	/// <summary>
	/// Конвертер для перечислений контакта(убирает "_" и не учитывает регистр)
	/// </summary>
	public class EnumJsonConverter : Newtonsoft.Json.JsonConverter
	{
		/// <summary>
		/// Object to json
		/// </summary>
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteValue(value.ToString());
		}

		/// <summary>
		/// Json to object
		/// </summary>
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}

			var enumValue = reader.Value.ToString().Replace("_", "");

			if (objectType.IsGenericType &&
			    objectType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				objectType = objectType.GetGenericArguments()[0];
			}
			var result = Activator.CreateInstance(objectType);

			foreach (var variable in Enum.GetValues(objectType))
			{
				if (string.Equals(variable.ToString(), enumValue, StringComparison.CurrentCultureIgnoreCase))
				{
					result = variable;
					break;
				}
			}
			return result;
		}

		/// <summary>
		/// TODO: Description
		/// </summary>
		public override bool CanConvert(Type objectType)
		{
			throw new NotImplementedException();
		}
	}
}