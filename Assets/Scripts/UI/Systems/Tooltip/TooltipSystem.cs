using Project.Items;
using Project.UI.Items;

namespace Project.UI.Systems
{
    /// <summary>
    /// System required for showing global tooltips
    /// </summary>
    public static class TooltipSystem
    {
        private static TooltipUI _instance;

        public static void Initialize(TooltipUI tooltip)
        {
            _instance = tooltip;
        }

        public static void Show(string title, string content) => _instance?.Show(title, content);
        public static void Hide() => _instance?.Hide();
    }
}