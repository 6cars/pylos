using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using Pylos.Backend.Models;

public enum PhaseState
{
    Placement,
    Retrieval
}

namespace Pylos.Backend.Logic
{
    public class PhaseManager
    {
        public PhaseState CurrentPhase { get; private set; }
        public BallColor CurrentPlayerColor { get; private set; }

        public event Action<PhaseState> OnPhaseChanged;
    }
}


