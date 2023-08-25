using System.Collections.Generic;

namespace MCNBTEditor.AdvancedContextService {
    public interface IContextProvider {
        void GetContext(List<IContextEntry> list);
    }
}