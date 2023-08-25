using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MCNBTEditor.Editor.FileTree;
using MCNBTEditor.Editor.FileTree.Physical;

namespace MCNBTEditor.Editor {
    public class MainViewModel : BaseViewModel {
        public AsyncRelayCommand OpenFolderCommand { get; }

        public Tree FileTree { get; }

        public MainViewModel() {
            this.OpenFolderCommand = new AsyncRelayCommand(this.OpenFolderAction);
            this.FileTree = new Tree();
            this.FileTree.NavigateToItem += this.FileTree_NavigateToItem;

            string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "level.dat");
            if (File.Exists(file)) {
                // this.FileTree.Root.AddFile();
            }
        }

        private Task FileTree_NavigateToItem(TreeEntry file) {
            // if (file is PhysicalTreeFolder folder && !string.IsNullOrEmpty(folder.FilePath) && Directory.Exists(folder.FilePath)) {
            //     return this.FileManager.NavigateToFolder(folder.FilePath);
            // }

            return Task.CompletedTask;
        }

        private async Task OpenFolderAction() {
            string path = await IoC.FilePicker.OpenFolder(null, "Select a folder to open");
            if (string.IsNullOrEmpty(path)) {
                return;
            }

            TreeEntry first = this.FileTree.Root.Items.FirstOrDefault(x => x.TryGetData(Win32FileSystem.FilePathKey, out string fpath) && fpath == path);
            if (first == null) {
                this.FileTree.Root.AddItemCore(new PhysicalVirtualFolder() {
                    FilePath = path, FileSystem = Win32FileSystem.Instance
                });
            }
            else {
                await first.RefreshAsync();
            }
        }
    }
}