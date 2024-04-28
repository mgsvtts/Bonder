using Bonder.Calculation.Grpc;
using System.Text;
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
        foreach (var bond in bonds.Take(10))
        {
            var income = bond.Income is not null ? (decimal)bond.Income : 0;
            var priceIncome = bond.PricePercent is not null ? (decimal)bond.PricePercent : 0;

            builder.AppendLine($"{count++}) " +
                               $"<b>{bond.Item.Name}</b>:\n" +
                               $"    <b>- Тикер:</b> {bond.Item.Ticker}\n" +
                               GetRatingText(bond) +
                               $"    <b>- Цена:</b> {(decimal)bond.Item.Price} руб.\n" +
                               $"    <b>- Номинал:</b> {(decimal)bond.Item.Nominal}\n" +
                               GetMaturityDateText(bond) +
                               GetOfferDateText(bond) +
                               $"    <b>- Прибыль:</b> {income:P} из них:\n" +
                               $"        <b>- Прибыль по закрытию:</b> {priceIncome:P}" +
                               GetCouponText(bond) +
                               GetAmortizationText(bond));
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
               $"    - <b>Цена от: </b> {(filters.PriceFrom > 0 ? filters.PriceFrom.ToString() : "0")}\n" +
               $"    - <b>Цена до: </b> {(filters.PriceTo > 0 ? filters.PriceTo.ToString() : "не указана")}\n" +
               $"    - <b>Рейтинг от: </b> {(filters.RatingFrom > 0 ? filters.RatingFrom.ToString() : "1")}\n" +
               $"    - <b>Рейтинг до: </b> {(filters.RatingTo > 0 ? filters.RatingTo.ToString() : "10")}\n" +
               $"    - <b>Дата от: </b> {(filters.DateFrom is not null ? filters.DateFrom.ToString() : "не указана")}\n" +
               $"    - <b>Дата до: </b> {GetDateToString(filters)}\n" +
               $"    - <b>Неизвестные эмитенты: </b> {(filters.IncludeUnknownRatings == true ? "да" : "нет")}\n";
    }

    public static string EnterDateFrom()
    {
        return $"Введите \"Дата до\" (в формате \"{DateTime.Now.AddDays(1):dd/MM/yyyy}\")\n\n" +
                "Или:\n\n" +
                "<b>До погашения:</b> считать каждую облигацию до ее погашения\n\n" +
                "<b>До оферты:</b> считать кажду облигацию до даты ее оферты (или погашения, если у облигации нет даты оферты)\n\n" +
               $"<b>На 1 год вперед:</b> считать до {DateTime.Now.Date.AddYears(1):dd/MM/yyyy}\n\n" +
               $"<b>На 3 года вперед:</b> считать до {DateTime.Now.Date.AddYears(3):dd/MM/yyyy}\n\n" +
               $"<b>На 5 лет вперед:</b> считать до {DateTime.Now.Date.AddYears(5):dd/MM/yyyy}\n\n" +
               $"<b>На 10 лет вперед:</b> считать до {DateTime.Now.Date.AddYears(10):dd/MM/yyyy}\n\n" +
               $"<i>Подробнее про погашение и оферту можно прочитать на нашем сайте:</i> (ванек не забудь добавить сюда сайт)";
    }

    public static string EnterRatingFrom()
    {
        return "Введите \"Рейтинг от\" (от 1 до 10 включительно):\n\n" +
               "<b>Кредитный рейтинг эмитента</b> - оценка финансовой устойчивости компании, выпустившей бумаги.\n" +
               "Чем он больше, тем надеждее компания, однако доход по ее бумагам обычно (но не всегда) ниже, чем у компаний с более низким рейтингом";
    }

    private static string GetDateToString(BondFilters filters)
    {
        string? dateTo;
        if (filters.DateToType == DateToType.Maturity)
        {
            dateTo = "погашения";
        }
        else if (filters.DateToType == DateToType.Offer)
        {
            dateTo = "оферты";
        }
        else
        {
            dateTo = filters.DateTo != DateOnly.MaxValue ? filters.DateTo.ToString() : "не указана";
        }

        return dateTo;
    }

    private static string GetRatingText(GetCurrentBondsItem bond)
    {
        var ratingText = $"    <b>- Рейтинг:</b> ";
        if (bond.Item.Rating > 0)
        {
            ratingText += $"{bond.Item.Rating}\n";
        }
        else
        {
            ratingText += "неизвестен\n";
        }

        return ratingText;
    }

    private static string GetAmortizationText(GetCurrentBondsItem bond)
    {
        var amortizationIncome = bond.AmortizationIncome is not null ? (decimal)bond.AmortizationIncome : 0;

        if (amortizationIncome > 0)
        {
            return $"\n        <b>- Прибыль по амортизациям:</b> {amortizationIncome:P}\n";
        }
        else
        {
            return "\n";
        }
    }

    private static string GetCouponText(GetCurrentBondsItem bond)
    {
        var couponIncome = bond.CouponIncome is not null ? (decimal)bond.CouponIncome : 0;

        if (couponIncome > 0)
        {
            return $"\n        <b>- Прибыль по купонам:</b> {couponIncome:P}";
        }
        else
        {
            return "";
        }
    }

    private static string GetMaturityDateText(GetCurrentBondsItem bond)
    {
        var offerDate = bond.Item.OfferDate.ToDateTime();
        var maturityDate = bond.Item.MaturityDate.ToDateTime();

        if (maturityDate == offerDate)
        {
            return "";
        }

        if (maturityDate > DateTime.MinValue)
        {
            return $"    - <b>Дата закрытия:</b> {maturityDate:dd/MM/yyyy}\n";
        }
        else
        {
            return "";
        }
    }

    private static string GetOfferDateText(GetCurrentBondsItem bond)
    {
        var offerDate = bond.Item.OfferDate.ToDateTime();

        if (offerDate > DateTime.MinValue)
        {
            return $"    - <b>Дата оферты:</b> {offerDate:dd/MM/yyyy}\n";
        }
        else
        {
            return "";
        }
    }
}