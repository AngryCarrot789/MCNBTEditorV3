using System.Threading.Tasks;
using MCNBTEditor.Views.Windows;

namespace MCNBTEditor.Services {
    public interface IViewService {
        Task<IWindow> ShowAsync<T>(T viewModel);

        Task<bool> ShowDialogAsync<T>(T viewModel);
    }
}