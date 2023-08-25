using System.Threading.Tasks;
using MCNBTEditor.Views.Windows;

namespace MCNBTEditor.WPF.Views {
    public class BaseWindow : WindowViewBase, IWindow {
        public bool IsOpen => base.IsLoaded;

        public BaseWindow() {
            this.SetToCenterOfScreen();
        }

        public void CloseWindow() {
            this.Close();
        }

        public Task CloseWindowAsync() {
            return base.CloseAsync();
        }
    }
}