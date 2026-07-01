using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pylos.Backend.Models
{
    public struct PylosMove
    {
        // 移動元の座標（手元から置く場合は null、または Level = -1 にする）
        public PylosCoordinate? From;
        
        // 移動先（配置先）の座標
        public PylosCoordinate To;
    }
}
