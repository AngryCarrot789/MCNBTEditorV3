using System;

namespace MCNBTEditor.ThreadSafety {
    [Flags]
    public enum LockType {
        None = 0,
        Automation = 1,
        Property = 2
    }
}