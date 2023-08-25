using System.IO;

namespace MCNBTEditor.Editor.FileTree.Physical {
    /// <summary>
    /// The base class that represents all physical virtual files
    /// </summary>
    public abstract class BasePhysicalVirtualFile : TreeEntry {
        public string FilePath {
            get => this.GetData<string>(Win32FileSystem.FilePathKey);
            internal set => this.SetData(Win32FileSystem.FilePathKey, value);
        }

        public string FileName {
            get {
                if (string.IsNullOrWhiteSpace(this.FilePath))
                    return null;
                string name = Path.GetFileName(this.FilePath);
                return string.IsNullOrEmpty(name) ? this.FilePath : name;
            }
        }

        protected BasePhysicalVirtualFile() {

        }

        protected override void OnDataChanged(string key, object value) {
            base.OnDataChanged(key, value);
            switch (key) {
                case Win32FileSystem.FilePathKey:
                    this.RaisePropertyChanged(nameof(this.FilePath));
                    this.RaisePropertyChanged(nameof(this.FileName));
                    break;
            }
        }
    }
}