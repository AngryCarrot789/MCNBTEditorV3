using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MCNBTEditor.Editor.FileTree;

namespace MCNBTEditor.WPF.Explorer {
    internal class FileTreeItem : TreeViewItem {
        private bool isProcessingNavigation;

        public FileTreeItem() {
        }

        protected override bool IsItemItsOwnContainerOverride(object item) => item is FileTreeItem;

        protected override DependencyObject GetContainerForItemOverride() => new FileTreeItem();

        protected override async void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            if (this.isProcessingNavigation) {
                e.Handled = true;
                return;
            }

            if (!e.Handled && this.DataContext is TreeEntry file && file.Explorer != null) {
                this.isProcessingNavigation = true;
                try {
                    await file.Explorer.OnNavigate(file);
                }
                finally {
                    this.isProcessingNavigation = false;
                }
            }

            base.OnMouseLeftButtonDown(e);
        }
    }
}
