using System;
using Windows.UI.Xaml;

namespace XamlPlus
{
    public static partial class Style
    {
        #region ExceptHighContrastPrivate

        private static DependencyProperty ExceptHighContrastPrivateProperty { get; } =
            DependencyProperty.RegisterAttached("ExceptHighContrastPrivate", typeof(Style),
                typeof(ExceptHighContrastPrivateData), new PropertyMetadata(null));

        #endregion

        #region ExceptHighContrast

        public static DependencyProperty ExceptHighContrastProperty { get; } = DependencyProperty.RegisterAttached(
            "ExceptHighContrast", typeof(Style), typeof(Windows.UI.Xaml.Style),
            new PropertyMetadata(null, ExceptHighContrastChanged));

        public static Windows.UI.Xaml.Style GetExceptHighContrast(DependencyObject sender)
        {
            return (Windows.UI.Xaml.Style) sender?.GetValue(ExceptHighContrastProperty);
        }

        public static void SetExceptHighContrast(DependencyObject sender, Windows.UI.Xaml.Style value)
        {
            sender?.SetValue(ExceptHighContrastProperty, value);
        }

        private static void ExceptHighContrastChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is FrameworkElement frameworkElement))
            {
                return;
            }

            var newStyle = e.NewValue as Windows.UI.Xaml.Style;
            if (frameworkElement.GetValue(ExceptHighContrastPrivateProperty) is ExceptHighContrastPrivateData currentPrivateObj)
            {
                if (newStyle != null)
                    currentPrivateObj.UpdateStyle(newStyle);
                else
                    currentPrivateObj.Dispose();
            }

            frameworkElement.SetValue(ExceptHighContrastPrivateProperty,
                new ExceptHighContrastPrivateData(frameworkElement, newStyle));
        }

        #endregion
    }
}