using Project.Items;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Project.UI.Items
{
    public class TooltipUI : MonoBehaviour
    {

        // ──────────────────────────────
        // Serialized & Private Fields
        // ──────────────────────────────

        [Header("References")]
        [SerializeField]
        private TextMeshProUGUI _nameText;
        [SerializeField]
        private TextMeshProUGUI _descriptionText;
        [SerializeField]
        private RectTransform _rectTransform;
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [Header("Settings")]
        [SerializeField]
        private Vector2 _offset = new Vector2(15, -15);

        // ──────────────────────────────
        // Properties
        // ──────────────────────────────

        // ──────────────────────────────
        // Unity Callbacks
        // ──────────────────────────────
        private void Awake()
        {
            Hide(); // Start hidden
        }

        // ──────────────────────────────
        // Public Methods
        // ──────────────────────────────
        public void Show(string title, string content)
        {
            _nameText.text = title;
            _descriptionText.text = content;

            _canvasGroup.alpha = 1;
            UpdatePosition();
        }

        public void Hide()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0;
            }
        }

        // ──────────────────────────────
        // Protected & Private Methods
        // ──────────────────────────────


        private void Update()
        {
            if (_canvasGroup.alpha > 0)
            {
                UpdatePosition();
            }
        }

        private void UpdatePosition()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            // Basic screen boundary check
            float pivotX = mousePos.x / Screen.width;
            float pivotY = mousePos.y / Screen.height;

            // Adjust pivot so the tooltip doesn't go off-screen
            _rectTransform.pivot = new Vector2(pivotX > 0.5f ? 1 : 0, pivotY > 0.5f ? 1 : 0);
            _rectTransform.position = mousePos + _offset;
        }
    }
}