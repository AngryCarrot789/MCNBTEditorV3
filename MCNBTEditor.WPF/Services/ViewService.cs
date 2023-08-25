using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using MCNBTEditor.Services;
using MCNBTEditor.Views.Windows;

namespace MCNBTEditor.WPF.Services {
    [ServiceImplementation(typeof(IViewService))]
    public class ViewService : IViewService {
        private readonly Dictionary<Type, Func<Window>> vmToView;
        private readonly Dictionary<Type, Func<Window>> vmToDialog;

        public ViewService() {
            this.vmToView = new Dictionary<Type, Func<Window>>();
            this.vmToDialog = new Dictionary<Type, Func<Window>>();
        }

        public void RegisterWindow<T, TWindow>(Func<TWindow> window) where TWindow : Window, IWindow {
            if (window == null)
                throw new ArgumentNullException(nameof(window));
            this.vmToView[typeof(T)] = window;
        }

        public void RegisterDialog<T>(Func<Window> window) {
            if (window == null)
                throw new ArgumentNullException(nameof(window));
            this.vmToDialog[typeof(T)] = window;
        }

        public async Task<IWindow> ShowAsync<T>(T viewModel) {
            for (Type type = typeof(T); type != null; type = type.BaseType) {
                if (this.vmToView.TryGetValue(type, out Func<Window> func)) {
                    if (IoC.Dispatcher.IsOnOwnerThread) {
                        return ShowWindow(func, viewModel);
                    }
                    else {
                        return await IoC.Dispatcher.InvokeAsync(() => ShowWindow(func, viewModel));
                    }
                }
            }

            throw new Exception("Could not find a suitable window for " + typeof(T));
        }

        public async Task<bool> ShowDialogAsync<T>(T viewModel) {
            for (Type type = typeof(T); type != null; type = type.BaseType) {
                if (this.vmToDialog.TryGetValue(type, out Func<Window> func)) {
                    if (IoC.Dispatcher.IsOnOwnerThread) {
                        return ShowDialog(func, viewModel);
                    }
                    else {
                        return await IoC.Dispatcher.InvokeAsync(() => ShowDialog(func, viewModel));
                    }
                }
            }

            throw new Exception("Could not find a suitable dialog for " + typeof(T));
        }

        private static IWindow ShowWindow(Func<Window> func, object dataContext) {
            Window window = func();
            window.DataContext = dataContext;
            window.Show();
            return (IWindow) window;
        }

        private static bool ShowDialog(Func<Window> func, object dataContext) {
            Window window = func();
            window.DataContext = dataContext;
            return window.ShowDialog() == true;
        }
    }
}