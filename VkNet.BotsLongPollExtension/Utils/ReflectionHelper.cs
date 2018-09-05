using System.Reflection;

namespace VkNet.BotsLongPollExtension.Utils
{
	/// <summary>
	/// Помощник в работе с рефлексией
	/// </summary>
	public static class ReflectionHelper
	{
		/// <summary>
		/// Получение приватного поля
		/// </summary>
		/// <param name="instance">Экземпляр класса содержащего поле</param>
		/// <param name="fieldName">Имя поля</param>
		/// <returns></returns>
		public static object GetPrivateField(object instance, string fieldName)
		{
			var fieldInfo = instance.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
			return fieldInfo?.GetValue(instance);
		}

		/// <summary>
		/// Получение приватного поля
		/// </summary>
		/// <param name="instance">Экземпляр класса содержащего поле</param>
		/// <param name="fieldName">Имя поля</param>
		/// <returns></returns>
		public static T GetPrivateField<T>(object instance, string fieldName) where T : class
		{
			var fieldInfo = instance.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
			return fieldInfo?.GetValue(instance) as T;
		}
	}
}