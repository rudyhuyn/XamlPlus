using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace XamlPlus
{
    public sealed class ThemeAwareBitmapSource : BitmapSource
    {
        private readonly static UISettings _uiSettings = new UISettings();
        private readonly ResourceContext _resourceContext;
        private string _resourceCandidateSelected;
        private string _formattedSource;
        private bool _isLightTheme;

        public ThemeAwareBitmapSource()
        {
            _resourceContext = ResourceContext.GetForCurrentView();
            this.Attach();
        }

        ~ThemeAwareBitmapSource()
        {
            this.UnAttach();
        }

        private void Attach()
        {
            _isLightTheme = Application.Current.RequestedTheme == ApplicationTheme.Light;
            _resourceContext.QualifierValues.MapChanged += new MapChangedEventHandler<string, string>(QualifierValues_MapChanged);
            _uiSettings.ColorValuesChanged += UISettings_ColorValuesChanged;
        }

        private void UnAttach()
        {
            _resourceContext.QualifierValues.MapChanged -= new MapChangedEventHandler<string, string>(QualifierValues_MapChanged);
            _uiSettings.ColorValuesChanged -= UISettings_ColorValuesChanged;
        }

        #region UriSource

        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        private static DependencyProperty SourceProperty { get; } =
            DependencyProperty.Register("Source", typeof(Uri), typeof(ThemeAwareBitmapSource), new PropertyMetadata(null, SourcePropertyChanged));

        private static void SourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var that = (ThemeAwareBitmapSource)d;
            if (e.NewValue == null)
            {
                that._formattedSource = null;
            }
            else
            {
                var uriSource = (Uri)e.NewValue;
                if (!uriSource.IsAbsoluteUri || string.Equals(uriSource.Scheme, "ms-appx", StringComparison.OrdinalIgnoreCase))
                {
                    that._formattedSource = $"Files/{uriSource.LocalPath.TrimStart('/')}";
                }
                else if (string.Equals(uriSource.Scheme, "ms-resource", StringComparison.OrdinalIgnoreCase))
                {
                    that._formattedSource = uriSource.LocalPath.TrimStart('/');
                }
                else
                {
                    throw new NotSupportedException("ThemeAwareBitmapSource doesn't support this URI");
                }
            }
            _ = that.UpdateSource();
        }

        #endregion


        private void QualifierValues_MapChanged(IObservableMap<string, string> sender, IMapChangedEventArgs<string> @event)
        {
            _ = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                _ = UpdateSource();
            });
        }

        private void UISettings_ColorValuesChanged(UISettings sender, object args)
        {
            // We should not have to do that, but ResourceContext isn't automatically updated when the theme changed and keep the theme used at launch time.
            _ = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                bool isLightTheme = Application.Current.RequestedTheme == ApplicationTheme.Light;
                if (!(isLightTheme ^ _isLightTheme))
                {
                    return;
                }

                _isLightTheme = isLightTheme;
                _ = UpdateSource();
            });
        }

        private async Task UpdateSource()
        {
            if (this.Source == null)
            {
                return;
            }

            var namedResource = ResourceManager.Current.MainResourceMap[_formattedSource];
            var currentTheme = Application.Current.RequestedTheme == ApplicationTheme.Dark ? "dark" : "light";

            ResourceContext resourceContext;
            if (_resourceContext.QualifierValues["theme"] == currentTheme)
            {
                resourceContext = _resourceContext;
            }
            else
            {
                // We need to create a new resourceContext with the correct theme
                resourceContext = new ResourceContext
                {
                    Languages = _resourceContext.Languages
                };
                foreach (var v in _resourceContext.QualifierValues)
                {
                    resourceContext.QualifierValues[v.Key] = v.Value;
                }
                resourceContext.QualifierValues["Theme"] = currentTheme;
            }

            var resourceCandidate = namedResource.Resolve(resourceContext);
            if (_resourceCandidateSelected == resourceCandidate.ValueAsString)
            {
                return;
            }

            _resourceCandidateSelected = resourceCandidate.ValueAsString;
            var stream = await resourceCandidate.GetValueAsStreamAsync();
            _ = this.SetSourceAsync(stream);
        }
    }
}
