using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.UI;

using Pylos.Backend.Logic;

public class PhaseView : MonoBehaviour
{
    [SerializeField] private Text phaseText;

    public void UpdatePhase(PhaseState phase)
    {
        phaseText.text =
            phase == PhaseState.Placement ? "配置フェーズ" : "回収フェーズ";
    }
}

