using System.Threading.Tasks;

namespace MCNBTEditor.Views.Dialogs.Progression {
    public interface IProgressionDialogService {
        Task ShowIndeterminateAsync(IndeterminateProgressViewModel viewModel);
    }
}