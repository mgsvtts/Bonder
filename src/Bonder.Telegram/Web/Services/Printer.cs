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
        return $"Привет, <b>{message.From.FirstName}</b>, добро пожаловать в <b>bonder-bot</b>{Environment.NewLine}{Environment.NewLine}" +
               $"Доступные команды:{Environment.NewLine}" +
               $"/start - начало работы{Environment.NewLine}" +
               $"/top_bonds - показать топ облигаций";
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
}
