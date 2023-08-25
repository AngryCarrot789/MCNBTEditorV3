using System;
using System.Collections.Generic;
using MCNBTEditor.Shortcuts.Inputs;

namespace MCNBTEditor.Shortcuts.Managing {
    public readonly struct ShortcutCollectorArgs {
        public readonly IInputStroke stroke;
        public readonly List<GroupedShortcut> list;
        public readonly Predicate<GroupedShortcut> filter;

        public ShortcutCollectorArgs(IInputStroke stroke, List<GroupedShortcut> list, Predicate<GroupedShortcut> filter) {
            this.stroke = stroke;
            this.list = list;
            this.filter = filter;
        }
    }
}