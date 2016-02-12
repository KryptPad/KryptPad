using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace KryptPadCSApp.UserControls
{
    public sealed partial class BusyIndicator : UserControl
    {

        #region Dependency Properties
        /// <summary>
        /// Dependency property for IsActive
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty =
                DependencyProperty.Register("IsActive", typeof(bool), typeof(BusyIndicator), new PropertyMetadata(false, (dp, e)=> {
                    var c = dp as BusyIndicator;

                    c.BusyProgressRing.IsActive = (bool)e.NewValue;
                }));

        public static readonly DependencyProperty LabelProperty =
                DependencyProperty.Register("Label", typeof(string), typeof(BusyIndicator), new PropertyMetadata(false, (dp, e) => {
                    var c = dp as BusyIndicator;

                    c.IndicatorLabel.Text = (string)e.NewValue;
                }));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the active status of the progress ring
        /// </summary>
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        /// <summary>
        /// Gets or sets the active status of the progress ring
        /// </summary>
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }
        #endregion


        public BusyIndicator()
        {
            this.InitializeComponent();
        }
    }
}
