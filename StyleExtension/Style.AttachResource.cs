using System.Linq;
using Windows.UI.Xaml;

namespace StyleExtension
{
    public static partial class Style
    {
        // Used to distinct normal ResourceDictionary and the one we add.
        private sealed class StyleExtensionResourceDictionary : ResourceDictionary
        {
        }

        public static DependencyProperty ResourcesProperty { get; } = DependencyProperty.RegisterAttached("Resources",
            typeof(ResourceDictionary), typeof(Style), new PropertyMetadata(null, ResourcesChanged));

        public static ResourceDictionary GetResources(DependencyObject sender)
        {
            return (ResourceDictionary)sender?.GetValue(ResourcesProperty);
        }

        public static void SetResources(DependencyObject sender, ResourceDictionary value)
        {
            sender?.SetValue(ResourcesProperty, value);
        }

        private static void ResourcesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is FrameworkElement frameworkElement))
            {return;}

            var mergedDictionaries = frameworkElement.Resources?.MergedDictionaries;
            if (mergedDictionaries == null)
            {
                return;
            }

            var existingResourceDictionary =
                mergedDictionaries.FirstOrDefault(c => c is StyleExtensionResourceDictionary);
            if (existingResourceDictionary != null)
            {
                // Remove the existing resource dictionary
                mergedDictionaries.Remove(existingResourceDictionary);
            }

            if (e.NewValue is ResourceDictionary resource)
            {
                var clonedResources = new StyleExtensionResourceDictionary();
                ResourceDictionaryCloner.Clone(resource, clonedResources);
                mergedDictionaries.Add(clonedResources);
            }

            if (frameworkElement.IsLoaded)
            {
                // Only force if the style was applied after the control was loaded
                ForceControlToReloadThemeResources(frameworkElement);
            }
        }

        private static void ForceControlToReloadThemeResources(FrameworkElement frameworkElement)
        {
            // To force the refresh of all resource references.
            // Note: Don't work when in high-contrast.
            var currentRequestedTheme = frameworkElement.RequestedTheme;
            frameworkElement.RequestedTheme = currentRequestedTheme == ElementTheme.Dark
                ? ElementTheme.Light
                : ElementTheme.Dark;
            frameworkElement.RequestedTheme = currentRequestedTheme;
        }
    }
}