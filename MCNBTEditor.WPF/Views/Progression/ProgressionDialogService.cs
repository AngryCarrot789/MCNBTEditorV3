using System.Threading.Tasks;
using MCNBTEditor.Views.Dialogs.Progression;

namespace MCNBTEditor.WPF.Views.Progression {
    [ServiceImplementation(typeof(IProgressionDialogService))]
    public class ProgressionDialogService : IProgressionDialogService {
        public Task ShowIndeterminateAsync(IndeterminateProgressViewModel viewModel) {
            IndeterminateProgressionWindow window = new IndeterminateProgressionWindow();
            viewModel.Window = window;
            window.DataContext = viewModel;
            window.Show();
            return Task.CompletedTask;
        }
    }
}