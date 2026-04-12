using TMPro;
using UnityEngine;

namespace Project.UI._CollectionLedger
{
    public class CollectionPopupUI : MonoBehaviour
    {

        // ──────────────────────────────
        // Serialized & Private Fields
        // ──────────────────────────────

        [SerializeField]
        private TextMeshProUGUI _titleText;
        [SerializeField]
        private TextMeshProUGUI _infoText;
        [SerializeField]
        private CanvasGroup _canvasGroup;

        // ──────────────────────────────
        // Public Methods
        // ──────────────────────────────

        public void SetData(string title, string info)
        {
            _titleText.text = title;
            _infoText.text = info;
            _canvasGroup.alpha = 0;
        }

        public void SetCanvasGroupAlpha(float alpha)
        {
            _canvasGroup.alpha = alpha;
        }
    }
}
