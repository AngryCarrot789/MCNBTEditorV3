using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using MCNBTEditor.WPF.Utils;

namespace MCNBTEditor.WPF.Converters {
    public class FileExtensionNameConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is string fileName) {
                string extension = Path.GetExtension(fileName);
                return FileExtensionsHelper.GetReadable(extension);
            }

            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return "(.extension)";
        }
    }
}