using System;
using System.Globalization;
using System.IO;
using MCNBTEditor.Utils;

namespace MCNBTEditor.WPF.Converters {
    public class FileExistsToBoolConverter : SingletonValueConverter<FileExistsToBoolConverter> {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return File.Exists((string) value).Box();
        }
    }
}