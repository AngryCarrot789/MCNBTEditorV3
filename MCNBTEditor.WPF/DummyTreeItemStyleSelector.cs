using System.Windows;
using System.Windows.Controls;
using MCNBTEditor.Editor.FileTree;

namespace MCNBTEditor.WPF {
    public class DummyTreeItemStyleSelector : StyleSelector {
        public Style WithDummyStyle { get; set; }

        public Style DefaultStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container) {
            if (item is TreeEntry file && file.CanHoldItems)
                return this.WithDummyStyle;
            return this.DefaultStyle;
        }
    }
}