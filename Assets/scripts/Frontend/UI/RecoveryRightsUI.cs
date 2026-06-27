using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace Pylos.Frontend.UI
{
    public class RecoveryRightsUI : MonoBehaviour
    {
        [SerializeField]
        private Text recoveryRightsText;

        /// <summary>
        /// 残り回収権数を表示する
        /// （Presenter から呼ばれる）
        /// </summary>
        public void UpdateRecoveryRights(int remainingCount)
        {
            recoveryRightsText.text = $"回収権：{remainingCount}";
        }

        /// <summary>
        /// 回収フェーズでないときは非表示にする等にも使える
        /// </summary>
        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}
