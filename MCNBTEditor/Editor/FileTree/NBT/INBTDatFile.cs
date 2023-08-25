using System.Collections.Generic;
using MCNBTEditor.NBT;

namespace MCNBTEditor.Editor.FileTree.NBT {
    public interface INBTDatFile {
        void SetContents(IEnumerable<BaseNBTTreeItem> items);
    }
}