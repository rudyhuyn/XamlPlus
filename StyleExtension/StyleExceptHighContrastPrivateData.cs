using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace XamlPlus
{
    public sealed class ExceptHighContrastPrivateData : IDisposable
    {
        private readonly AccessibilitySettings _accessibilitySettings;
        private readonly FrameworkElement _element;
        private Windows.UI.Xaml.Style _style;

        public ExceptHighContrastPrivateData(FrameworkElement element, Windows.UI.Xaml.Style style)
        {
            _element = element;
            _style = style;
            _accessibilitySettings = new AccessibilitySettings();
            _accessibilitySettings.HighContrastChanged += AccessibilitySettings_HighContrastChanged;
            ComputeStyle();
        }

        public void Dispose()
        {
            _accessibilitySettings.HighContrastChanged -= AccessibilitySettings_HighContrastChanged;
        }

        public void UpdateStyle(Windows.UI.Xaml.Style style)
        {
            _style = style;
            if (!_accessibilitySettings.HighContrast)
            {
                _element.Style = style;
            }
        }

        private void AccessibilitySettings_HighContrastChanged(AccessibilitySettings sender, object args)
        {
            ComputeStyle();
        }

        private void ComputeStyle()
        {
            _element.Style = _accessibilitySettings.HighContrast ? null : _style;
        }
    }
}