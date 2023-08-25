using System.Threading.Tasks;

namespace MCNBTEditor.Views.Windows {
    public interface IWindow : IViewBase {
        bool IsOpen { get; }

        void CloseWindow();

        Task CloseWindowAsync();
    }
}