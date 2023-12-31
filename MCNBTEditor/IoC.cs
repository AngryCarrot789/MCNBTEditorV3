using System;
using MCNBTEditor.Services;
using MCNBTEditor.Shortcuts.Dialogs;
using MCNBTEditor.Views.Dialogs.FilePicking;
using MCNBTEditor.Views.Dialogs.Message;
using MCNBTEditor.Views.Dialogs.Progression;
using MCNBTEditor.Views.Dialogs.UserInputs;

namespace MCNBTEditor {
    public static class IoC {
        private static volatile bool isAppRunning = true;

        public static SimpleIoC Instance { get; } = new SimpleIoC();

        public static bool IsAppRunning {
            get => isAppRunning;
            set => isAppRunning = value;
        }

        public static IShortcutManagerDialogService ShortcutManagerDialog => Provide<IShortcutManagerDialogService>();

        public static Action<string> OnShortcutModified { get; set; }
        public static Action<string> BroadcastShortcutActivity { get; set; }

        /// <summary>
        /// The application dispatcher, used to execute actions on the main thread
        /// </summary>
        public static IApplicationDispatcher ApplicationDispatcher { get; set; }

        public static IClipboardService Clipboard => Provide<IClipboardService>();
        public static IMessageDialogService MessageDialogs => Provide<IMessageDialogService>();
        public static IProgressionDialogService ProgressionDialogs => Provide<IProgressionDialogService>();
        public static IFilePickDialogService FilePicker => Provide<IFilePickDialogService>();
        public static IUserInputDialogService UserInput => Provide<IUserInputDialogService>();
        public static IExplorerService ExplorerService => Provide<IExplorerService>();
        public static IKeyboardDialogService KeyboardDialogs => Provide<IKeyboardDialogService>();
        public static IMouseDialogService MouseDialogs => Provide<IMouseDialogService>();

        public static ITranslator Translator => Provide<ITranslator>();

        public static Action<string> BroadcastShortcutChanged { get; set; }

        public static T Provide<T>() => Instance.GetService<T>();
    }
}