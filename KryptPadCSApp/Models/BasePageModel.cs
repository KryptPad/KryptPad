using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace KryptPadCSApp.Models
{
    class BasePageModel : BaseModel
    {

        #region Properties

        #endregion

        #region Helper Methods

        protected  override void Navigate(Type pageType, object parameter)
        {
            var mainPage = Window.Current.Content as MainPage;

            if (mainPage != null)
            {
                var frame = mainPage.RootFrame;
                if (frame != null)
                {
                    frame.Navigate(pageType, parameter);
                }
            }
            else
            {
                //try to get the frame from the window content itself
                var frame = Window.Current.Content as Frame;

                if (frame != null)
                {
                    frame.Navigate(pageType, parameter);
                }
            }

        }

        protected void PageNavigate(Type pageType)
        {
            
        }
        #endregion
    }
}
