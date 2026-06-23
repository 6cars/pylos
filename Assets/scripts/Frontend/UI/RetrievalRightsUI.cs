using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.UI;


public class RetrievalRightsUI : MonoBehaviour
{
    [SerializeField] private Text rightsText;

    public void UpdateRights(int remaining)
    {
        rightsText.text = $"‰ñû‰Â”\”F{remaining}";
    }
}

