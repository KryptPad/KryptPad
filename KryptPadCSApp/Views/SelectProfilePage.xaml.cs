using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace KryptPadCSApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectProfilePage : Page
    {
        public SelectProfilePage()
        {
            this.InitializeComponent();
        }

        public static Rect GetElementRect(FrameworkElement element)
        {
            GeneralTransform buttonTransform = element.TransformToVisual(null);
            Point point = buttonTransform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }


        private async void Grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            // Create a menu and add commands specifying a callback delegate for each. 
            // Since command delegates are unique, no need to specify command Ids. 
            var menu = new PopupMenu(); 
            menu.Commands.Add(new UICommand("Open with", (command) => 
            { 
                //OutputTextBlock.Text = "'" + command.Label + "' selected"; 
            })); 
            menu.Commands.Add(new UICommand("Save attachment", (command) => 
            { 
                //OutputTextBlock.Text = "'" + command.Label + "' selected"; 
            }));

            var chosenCommand = await menu.ShowForSelectionAsync(Rect.Empty, Placement.Default);


        }
    }
}
