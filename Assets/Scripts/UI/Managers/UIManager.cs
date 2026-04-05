using Project.UI.Items;
using Project.UI.Systems;
using UnityEngine;
namespace Project.UI.Managers
{
    /// <summary>
    /// Manager that will contain global references to UI objects used in global systems
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private TooltipUI _tooltipUI;

        private void Awake()
        {
            TooltipSystem.Initialize(_tooltipUI);
        }
    }
}