using Windows.UI.Xaml;

namespace StyleExtension
{
    internal static class ResourceDictionaryCloner
    {
        internal static ResourceDictionary Clone(ResourceDictionary resource, ResourceDictionary destination)
        {
            if (resource == null) return null;

            if (resource.Source != null)
            {
                destination.Source = resource.Source;
            }
            else
            {
                // Clone theme dictionaries
                if (resource.ThemeDictionaries != null)
                {
                    foreach (var theme in resource.ThemeDictionaries)
                    {
                        if (theme.Value is ResourceDictionary themedResource)
                        {
                            var themeDictionary = new ResourceDictionary();
                            Clone(themedResource, themeDictionary);
                            destination.ThemeDictionaries[theme.Key] = themeDictionary;
                        }
                        else
                        {
                            destination.ThemeDictionaries[theme.Key] = theme.Value;
                        }
                    }
                }

                // Clone merged dictionaries
                if (resource.MergedDictionaries != null)
                {
                    foreach (var mergedResource in resource.MergedDictionaries)
                    {
                        var themeDictionary = new ResourceDictionary();
                        Clone(mergedResource, themeDictionary);
                        destination.MergedDictionaries.Add(themeDictionary);
                    }
                }

                // Clone all contents
                foreach (var item in resource)
                {
                    destination[item.Key] = item.Value;
                }
            }

            return destination;
        }
    }
}
