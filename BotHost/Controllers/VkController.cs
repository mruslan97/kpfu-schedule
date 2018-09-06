using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BotHost.Models;
using BotHost.Tools;
using BotHost.Vk;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using VkNet;
using VkNet.Abstractions;
using VkNet.Abstractions.Utils;
using VkNet.BotsLongPollExtension.Enums;
using VkNet.BotsLongPollExtension.Model;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using VkNet.Utils;

namespace BotHost.Controllers
{
    [ApiController]
    public class VkController : Controller
    {
        private readonly ImageGenerator _imageGenerator;
        private PdfGenerator _pdfGenerator;
        private readonly UsersContext _usersContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IVkApi _vkApi;
        private readonly long _groupId = 170386942;
        private readonly ILogger<VkController> _logger;

        public VkController(ImageGenerator imageGenerator, PdfGenerator pdfGenerator, UsersContext usersContext,
            IHttpClientFactory httpClientFactory, ILogger<VkController> logger, IVkApi vkApi)
        {
            _logger = logger;
            _imageGenerator = imageGenerator;
            _pdfGenerator = pdfGenerator;
            _usersContext = usersContext;
            _httpClientFactory = httpClientFactory;
            _vkApi = vkApi;
        }

        [HttpPost("vkapi")]
        public async Task<IActionResult> MessageHandler([FromBody] JToken body)
        {
            var vkResponse = new VkResponse(body);
            var groupUpdate = GroupUpdate.FromJson(vkResponse);
            try
            {
                if (groupUpdate.Type == GroupLongPollUpdateType.MessageNew)
                {
                    _logger.LogInformation($"Incoming message from {groupUpdate.UserId} Text:{groupUpdate.Message.Text}");
                    await SortInputMessage(groupUpdate);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{e.Message}");
            }
            return Ok("ok");
        }

        private async Task SortInputMessage(GroupUpdate groupUpdate)
        {
            switch (groupUpdate.Message.Text.ToLower())
            {
                case "старт":
                    HelloAnswer(groupUpdate);
                    break;
                case "начать":
                    HelloAnswer(groupUpdate);
                    break;
                case "на сегодня":
                    await GetDay(groupUpdate, true);
                    break;
                case "на завтра":
                    await GetDay(groupUpdate, false);
                    break;
                case "на неделю":
                    await GetWeek(groupUpdate);
                    break;
                default:
                    if (groupUpdate.Message.Text.Contains("-")) await VerificationAnswer(groupUpdate);
                    break;
            }
        }

        private async void HelloAnswer(GroupUpdate groupUpdate)
        {
            var user = _vkApi.Users.Get(new List<long> {(long) groupUpdate.UserId}).SingleOrDefault();
            _vkApi.Messages.Send(new MessagesSendParams
            {
                UserId = user.Id,
                Message = $"Привет, {user.FirstName}! Введи номер своей группы в формате **-***",
                PeerId = _groupId
            });
            if (_usersContext.VkUsers.Find(user.Id) != null) return;
            _usersContext.VkUsers.Add(new VkUser
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName
            });
            try
            {
                await _usersContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"dbcontext exception");
                _vkApi.Messages.Send(new MessagesSendParams
                {
                    UserId = user.Id,
                    Message = $"Ошибка сохранения, попробуй отправить заново",
                    PeerId = _groupId
                });
            }
        }

        private async Task VerificationAnswer(GroupUpdate groupUpdate)
        {
            var group = groupUpdate.Message.Text;
            var isValidGroup = await CheckGroup(group);
            if (!isValidGroup)
            {
                _vkApi.Messages.Send(new MessagesSendParams
                {
                    UserId = groupUpdate.UserId,
                    Message = "Нет данных для этой группы",
                    PeerId = _groupId
                });
                return;
            }

            using (var db = new UsersContext())
            {
                var user = await _usersContext.VkUsers.SingleOrDefaultAsync(u => u.UserId == groupUpdate.UserId);
                user.Group = group;
                await db.SaveChangesAsync();
                var keyboardBuilder = new MessageKeyboardBuilder();
                keyboardBuilder.AddButton("На сегодня", "", KeyboardButtonColor.Primary, "");
                keyboardBuilder.Line();
                keyboardBuilder.AddButton("На завтра", "", KeyboardButtonColor.Primary, "");
                keyboardBuilder.Line();
                keyboardBuilder.AddButton("На неделю", "", KeyboardButtonColor.Primary, "");
                //keyboardBuilder.Line();
                //keyboardBuilder.AddButton("На неделю(pdf)", "extra", KeyboardButtonColor.Default, "def type");
                _vkApi.Messages.Send(new MessagesSendParams
                {
                    UserId = groupUpdate.UserId,
                    Message =
                        "Группа успешно сохранена. Используй кнопки или вводи команды вручную. Доступно: на сегодня, на завтра, на неделю",
                    PeerId = 170386942,
                    Keyboard = keyboardBuilder.Get()
                });
            }
            
        }

        private async Task GetDay(GroupUpdate groupUpdate, bool isToday)
        {
            using (var db = new UsersContext())
            {
                var user = await db.VkUsers.SingleOrDefaultAsync(u => u.UserId == groupUpdate.UserId);
                var group = user.Group;
                var image = await _imageGenerator.GetDay(group, isToday);

                var uploadServer = _vkApi.Photo.GetMessagesUploadServer(255959243);
                var response = await UploadImage(uploadServer.UploadUrl, image.ToArray());
                var photo = _vkApi.Photo.SaveMessagesPhoto(response).SingleOrDefault();
                _vkApi.Messages.Send(new MessagesSendParams
                {
                    UserId = groupUpdate.UserId,
                    PeerId = _groupId,
                    Attachments = new List<MediaAttachment>(new List<MediaAttachment>
                    {
                        photo
                    })
                });
            }
        }

        private async Task GetWeek(GroupUpdate groupUpdate)
        {
            using (var db = new UsersContext())
            {
                var user = await db.VkUsers.SingleOrDefaultAsync(u => u.UserId == groupUpdate.UserId);
                var group = user.Group;
                var image = await _imageGenerator.GetWeek(group);
                var uploadServer = _vkApi.Photo.GetMessagesUploadServer(255959243);
                var response = await UploadImage(uploadServer.UploadUrl, image.ToArray());
                var photo = _vkApi.Photo.SaveMessagesPhoto(response).SingleOrDefault();
                var weekNumber = WeeksCalculator.GetCurrentWeek();
                var messageText = weekNumber % 2 == 0
                    ? $"Это {weekNumber}-я неделя - четная."
                    : $"Это {weekNumber}-я неделя - нечетная.";
                _vkApi.Messages.Send(new MessagesSendParams
                {
                    UserId = groupUpdate.UserId,
                    Message = messageText,
                    PeerId = _groupId,
                    Attachments = new List<MediaAttachment>(new List<MediaAttachment>
                    {
                        photo
                    })
                });
            }
        }

        private async Task<bool> CheckGroup(string group)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var bytes = await httpClient.GetByteArrayAsync(
                $"https://kpfu.ru/week_sheadule_print?p_group_name={group}");
            var encoding = CodePagesEncodingProvider.Instance.GetEncoding(1251);
            var htmlPage = encoding.GetString(bytes, 0, bytes.Length);
            return !htmlPage.Contains("Расписание не найдено");
        }

        private async Task<string> UploadImage(string url, byte[] data)
        {
            using (var client = _httpClientFactory.CreateClient())
            {
                var requestContent = new MultipartFormDataContent();
                var imageContent = new ByteArrayContent(data);
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                requestContent.Add(imageContent, "photo", "image.png");
                var encoding = CodePagesEncodingProvider.Instance.GetEncoding(1251);
                var response = await client.PostAsync(url, requestContent);
                var bytes = await response.Content.ReadAsByteArrayAsync();
                return encoding.GetString(bytes, 0, bytes.Length);
            }
        }
    }
}