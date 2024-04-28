using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.BackgroundServices;

public sealed class BotConfiguration
{
    public static readonly string Configuration = "BotConfiguration";

    public string BotToken { get; init; } = default!;
    public string HostAddress { get; init; } = default!;
    public string Route { get; init; } = default!;
    public string SecretToken { get; init; } = default!;
}