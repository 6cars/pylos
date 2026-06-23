using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pylos.Backend.Models;
public interface IAIAlgorithm
{
    PylosCoordinate DecideMove(BoardModel board);
}

