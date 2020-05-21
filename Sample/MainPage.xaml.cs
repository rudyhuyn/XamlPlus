using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace XamlPlusSample
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void ModifyTheme_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var nightStyle = Resources["FbButtonStyle"] as Style;
            var instaStyle = Resources["InstaButtonStyle"] as Style;
            var accentStyle = Resources["AccentColorButtonStyle"] as Style;
            if (button.Style == nightStyle)
            {
                button.Style = instaStyle;
            }
            else
            {
                if (button.Style == instaStyle)
                {
                    button.Style = accentStyle;
                }
                else
                {
                    button.Style = nightStyle;
                }
            }
        }

        private void AddStyle_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).Style = Resources["FbButtonStyle"] as Style;
        }

        private void ResetStyle_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).Style = null;
        }
    }
}