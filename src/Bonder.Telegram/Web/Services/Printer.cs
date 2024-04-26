using Bonder.Calculation.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Web.Services.Dto;

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
               $"Пишите нам если:\n" +
               $"    - У вас есть идея или предложение\n" +
               $"    - Вы нашли баг\n" +
               $"    - Вы мощный и уверенный <b>C#, DOTNET, .NET, ASP NET, ASP NET CORE, DOTNET CORE</b> разработчик и желаете поработать за еду (мы студенты у нас нет денег)\n\n" +
               $"<b>Спасибо вам, что пользуетесь :3</b>";
    }

    public static string GetBondsFinish(BondFilters filters)
    {
        return $"Итоговые фильтры:\n" +
               $"    - <b>Цена от: </b> {(filters.PriceFrom > 0 ? filters.PriceFrom.ToString() : "не указана")}\n" +
               $"    - <b>Цена до: </b> {(filters.PriceTo > 0 ? filters.PriceTo.ToString() : "не указана")}\n" +
               $"    - <b>Рейтинг от: </b> {(filters.RatingFrom > 0 ? filters.RatingFrom.ToString() : "не указан")}\n" +
               $"    - <b>Рейтинг до: </b> {(filters.RatingTo > 0 ? filters.RatingTo.ToString() : "не указан")}\n" +
               $"    - <b>Дата от: </b> {(filters.DateFrom is not null ? filters.DateFrom.ToString() : "не указана")}\n" +
               $"    - <b>Дата до: </b> {(filters.DateTo != DateOnly.MaxValue ? filters.DateTo.ToString() : "не указана")}\n" +
               $"    - <b>Неизвестные эмитенты: </b> {(filters.IncludeUnknownRatings == true ? "да" : "нет")}\n";
    }
}
