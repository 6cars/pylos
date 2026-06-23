using System.Collections;
using System.Collections.Generic;
using Pylos.Backend.Models;

namespace Pylos.Backend.Logic
{
    public static class Rules
    {
        public static bool CanPlaceAt(BoardModel board, PylosCoordinate coord)
        {
            return false;
        }

        public static bool CanRemoveAt(BoardModel board, PylosCoordinate coord)
        {
            return false;
        }

        public static bool CheckSquareFormation(BoardModel board, PylosCoordinate coord)
        {
            return false;
        }

        public static bool CheckLineFormation(BoardModel board, PylosCoordinate coord)
        {
            return false;
        }

        public static bool IsGameOver(BoardModel board)
        {
            return false;
        }
    }
}

