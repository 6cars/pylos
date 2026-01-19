using UnityEngine;
using UnityEngine.UI; // パネルの表示切り替えに必要
using TMPro;          // ★重要：TextMeshProを使うためのおまじない

public class PhaseView : MonoBehaviour
{
    [Header("UI References")]
    // ★ここを変更：型を Text から TextMeshProUGUI に変えました
    public TextMeshProUGUI phaseText;
    public TextMeshProUGUI playerText;
    public TextMeshProUGUI messageText;

    public GameObject messagePanel;

    [Header("System")]
    public PylosGamePresenter presenter;

    // 画面を更新する機能
    public void UpdatePhaseText(string text)
    {
        if (phaseText != null) phaseText.text = text;
    }

    public void UpdatePlayerText(string text, Color color)
    {
        if (playerText != null)
        {
            playerText.text = text;
            playerText.color = color;
        }
    }

    public void ShowMessage(string message)
    {
        if (messagePanel != null) messagePanel.SetActive(true);
        if (messageText != null) messageText.text = message;
    }

    public void HideMessage()
    {
        if (messagePanel != null) messagePanel.SetActive(false);
    }
}