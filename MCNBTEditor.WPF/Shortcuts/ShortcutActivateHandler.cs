using System.Threading.Tasks;
using MCNBTEditor.Shortcuts.Managing;

namespace MCNBTEditor.WPF.Shortcuts {
    public delegate Task<bool> ShortcutActivateHandler(ShortcutProcessor processor, GroupedShortcut shortcut);
}