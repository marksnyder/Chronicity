﻿using Chronicity.Core.Agent;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chronicity.Core
{
    public interface IStateChangeReaction
    {
        Reaction OnChange(string entity, string key, string priorValue, string newValue, string on);
    }
}