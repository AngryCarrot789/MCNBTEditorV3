using System.Threading.Tasks;

namespace MCNBTEditor.Views.Dialogs.UserInputs {
    public interface IUserInputDialogService {
        Task<string> ShowSingleInputDialogAsync(string title = "Input a value", string message = "Input a new valid", string def = null, InputValidator validator = null);
        Task<bool> ShowSingleInputDialogAsync(SingleInputViewModel viewModel);
        Task<bool> ShowDoubleInputDialogAsync(DoubleInputViewModel viewModel);
    }
}