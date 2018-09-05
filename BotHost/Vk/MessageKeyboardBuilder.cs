using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Keyboard;
using VkNet.Utils;

namespace BotHost.Vk
{
    /// <summary>
    /// Генератор клавиатур
    /// </summary>
    public class MessageKeyboardBuilder
    {
        private bool _isOneTime;

        private readonly List<List<MessageKeyboardButton>> _fullKeyboard = new List<List<MessageKeyboardButton>>();
        private List<MessageKeyboardButton> _currentLine = new List<MessageKeyboardButton>();
        private readonly string _type;

        /// <summary>
        /// Конструктор без типа кнопки, надо задавать везде вручную
        /// </summary>
        public MessageKeyboardBuilder()
        {
        }

        /// <summary>
        /// Конструктор с типом кнопок
        /// </summary>
        /// <param name="type">Тип кнопки</param>
        public MessageKeyboardBuilder(string type)
        {
            _type = type;
        }

        /// <summary>
        /// Сделать клавиатуру одноразовой
        /// </summary>
        public MessageKeyboardBuilder SetOneTime()
        {
            _isOneTime = true;
            return this;
        }

        /// <summary>
        /// Добавить кнопку
        /// </summary>
        /// <param name="label">Надписть на кнопке</param>
        /// <param name="extra">Дополнительная информация о кнопке</param>
        /// <param name="type">Основная информация о кнопке</param>
        /// <param name="color">Цвет кнопки</param>
        public MessageKeyboardBuilder AddButton(string label, string extra,
            KeyboardButtonColor color = null, string type = null)
        {
            color = color ?? KeyboardButtonColor.Default;
            type = type ?? _type;
            _currentLine.Add(new MessageKeyboardButton
            {
                Color = color,
                Action = new MessageKeyboardButtonAction
                {
                    Label = label,
                    Payload = $"{{\"{type}\":\"{extra}\"}}",
                    Type = KeyboardButtonActionType.Text
                }
            });
            return this;
        }

        /// <summary>
        /// Перейти на следующую линию
        /// </summary>
        public MessageKeyboardBuilder Line()
        {
            _fullKeyboard.Add(_currentLine);
            _currentLine = new List<MessageKeyboardButton>();
            return this;
        }

        /// <summary>
        /// Получить объект клавиатуры для сообщения
        /// </summary>
        public MessageKeyboard Get()
        {
            _fullKeyboard.Add(_currentLine);
            var messageKeyboard = new MessageKeyboard
            {
                OneTime = _isOneTime,
                Buttons = _fullKeyboard.Select(e => e.ToReadOnlyCollection()).ToReadOnlyCollection()
            };

            return messageKeyboard;
        }
    }
}
