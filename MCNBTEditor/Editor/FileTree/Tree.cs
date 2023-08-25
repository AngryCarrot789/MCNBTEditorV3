using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using MCNBTEditor.Drop;
using MCNBTEditor.Editor.FileTree.Events;
using MCNBTEditor.Editor.FileTree.Physical;

namespace MCNBTEditor.Editor.FileTree {
    public class Tree : BaseViewModel, IDropHandler {
        public AsyncRelayCommand<TreeEntry> OpenItemCommand { get; }

        public TreeEntry Root { get; }

        public ObservableCollection<TreeEntry> SelectedItems { get; }

        public event NavigateToFileEventHandler NavigateToItem;
        public event OpenFileEventHandler OpenFile;

        public Tree() {
            this.Root = new RootTreeEntry();
            this.SelectedItems = new ObservableCollection<TreeEntry>();
            this.OpenItemCommand = new AsyncRelayCommand<TreeEntry>(this.OpenFileAction);
        }

        private class RootTreeEntry : TreeEntry {
            public override bool CanHoldItems => true;
        }

        public async Task OpenFileAction(TreeEntry item) {
            OpenFileEventHandler handler = this.OpenFile;
            if (handler != null)
                await handler(item);
        }

        public DropType OnDropEnter(string[] paths) {
            Debug.WriteLine("Drop Enter! " + string.Join(", ", paths));
            return DropType.Link;
        }

        public void OnDropLeave(bool isCancelled) {
            Debug.WriteLine("Drop Leave! " + (isCancelled ? "Cancelled" : "Not cancelled"));
        }

        public Task OnFilesDropped(string[] paths) {
            foreach (string path in paths) {
                if (Directory.Exists(path)) {
                    this.Root.AddItemCore(Win32FileSystem.Instance.ForEntry(new DirectoryInfo(path)));
                }
                else if (File.Exists(path)) {
                    this.Root.AddItemCore(Win32FileSystem.Instance.ForEntry(new FileInfo(path)));
                }
            }

            Debug.WriteLine("Dropped! " + string.Join(", ", paths));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called when the user tries to navigate to the given file. May be a file or folder
        /// </summary>
        public Task OnNavigate(TreeEntry file) {
            NavigateToFileEventHandler x = this.NavigateToItem;
            if (x != null)
                return x(file);
            return Task.CompletedTask;
        }
    }
}