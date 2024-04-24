using Bonder.Calculation.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Web.Services;
public static class Printer
{
    public static string GetStartText(Message message)
    {
        return $"Привет, <b>{message.From.FirstName}</b>, добро пожаловать в <b>Bonder-bot</b>\n\n" +
               $"Доступные команды:\n" +
               $"/start - начало работы\n" +
               $"/top_bonds - показать топ облигаций\n" +
               $"/devs - кто же эти гении, создавшие бота";
    }

    public static string GetTopBondsText(IEnumerable<GetCurrentBondsItem> bonds)
    {
        var builder = new StringBuilder($"<b>Топ доходных облигаций</b>");
        builder.AppendLine();
        builder.AppendLine();

        var count = 1;
        foreach (var bond in bonds)
        {
            var income = (decimal)bond.Income;

            var incomePercent = income.ToString("P");

            var test = income * 100;
            var test2 = test.ToString("#.##");

            builder.AppendLine($"{count++}) " +
                               $"<b>{bond.Name}: {bond.Ticker}{Environment.NewLine}</b>" +
                               $"    - Цена: {(decimal)bond.Price} руб.{Environment.NewLine}" +
                               $"    - Прибыль: {incomePercent}");
            builder.AppendLine();
        }

        return builder.ToString();
    }

    public static string GetDevsText()
    {
        return $"Этот бот (как и весь проект <b>Bonder</b>) был создан двумя людьми:\n\n" +
               $"https://t.me/mgsvtts - backend разработчик\n" +
               $"https://t.me/EvpatiyKaloed - создатель идеи и духовный наставник разработчика\n\n" +
               $"Пишите нам если у вас есть:\n" +
               $"    - Идеи и предложения\n" +
               $"    - Отчеты о багах\n" +
               $"    - Если вы мощный и уверенный <b>C#, DOTNET, .NET, ASP NET, ASP NET CORE, DOTNET CORE</b> разработчик и желаете поработать за еду (мы студенты у нас нет денег)\n\n" +
               $"<b>Спасибо вам, что пользуетесь :3</b>";
    }
}
