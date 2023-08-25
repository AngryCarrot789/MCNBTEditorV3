using System.Windows;
using System.Windows.Controls;
using MCNBTEditor.WPF.Controls.TreeViews.Controls;

namespace MCNBTEditor.WPF.Explorer {
    internal class FileTreeControl : TreeView {
        public FileTreeControl() {

        }

        protected override bool IsItemItsOwnContainerOverride(object item) => item is FileTreeItem;

        protected override DependencyObject GetContainerForItemOverride() => new FileTreeItem();
    }
}