using TelegramBotEnums = Telegram.Bot.Types.Enums;

namespace Ollio.Common.Enums
{
    public enum MessageType {
        Audio = TelegramBotEnums.MessageType.Audio,
        Document = TelegramBotEnums.MessageType.Document,
        Photo = TelegramBotEnums.MessageType.Photo,
        Sticker = TelegramBotEnums.MessageType.Sticker,
        Text = TelegramBotEnums.MessageType.Text
    }
}