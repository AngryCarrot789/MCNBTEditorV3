using System.Threading.Tasks;

namespace MCNBTEditor.Editor.FileTree {
    /// <summary>
    /// Used to query a tree entry's contents
    /// </summary>
    public abstract class TreeFileSystem {
        protected TreeFileSystem() {
        }

        public abstract Task<bool> QueryContent(TreeEntry target);
    }
}