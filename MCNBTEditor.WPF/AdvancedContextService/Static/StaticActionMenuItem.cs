using System.ComponentModel;
using System.Windows.Markup;

namespace MCNBTEditor.WPF.AdvancedContextService.Static {
    [DefaultProperty("Items")]
    [ContentProperty("Items")]
    public class StaticActionMenuItem : StaticBaseMenuItem {
        public string ActionID { get; set; }
    }
}