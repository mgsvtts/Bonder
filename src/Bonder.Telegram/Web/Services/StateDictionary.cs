﻿using System.Collections.Concurrent;
using Telegram.Bot.Types;
using Web.Services.Dto;

namespace Web.Services;

public sealed class StateDictionary
{
    private readonly ConcurrentDictionary<string, UserState> _states = new();

    public UserState GetState(Message message)
    {
        if (!_states.TryGetValue(message.Chat.Username, out var state))
        {
            state = new UserState();
        }

        return state;
    }

    public void Add(string userName, UserState state)
    {
        _states.TryAdd(userName, state);
    }
}
