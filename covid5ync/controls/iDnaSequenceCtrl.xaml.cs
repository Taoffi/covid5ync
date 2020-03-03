using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace iDna.controls
{
	/// <summary>
	/// Interaction logic for iDnaSequenceCtrl.xaml
	/// </summary>
	public partial class iDnaSequenceCtrl : UserControl
	{
		public iDnaSequenceCtrl()
		{
			InitializeComponent();
			this.Loaded += new RoutedEventHandler(Control_Loaded);
		}

		private void Control_Loaded(object sender, RoutedEventArgs e)
		{
			var presenter		= FindChild<ScrollContentPresenter>(listItems, null);
			var mouseWheelZoom  = new MouseWheelZoom(presenter);
			PreviewMouseWheel   += mouseWheelZoom.Zoom;
		}

        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, 
        /// a null parent is being returned.</returns>
        public static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        private void buttonPrevNext_Click(object sender, RoutedEventArgs e)
        {
            iDnaSequencePaging paging = iDnaSequence.Instance.SequencePaging;

            paging.CurrentPage += 1;
        }

        private void buttonPrevPage_Click(object sender, RoutedEventArgs e)
        {
            iDnaSequencePaging  paging  = iDnaSequence.Instance.SequencePaging;

            paging.CurrentPage  -=1;
        }

        private async void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            string      str     = textBoxSearch.Text;

            if(string.IsNullOrEmpty(str))
                return;

            await iDnaSequence.Instance.FindString( str, isPairSearch: false, this.Dispatcher );
            //listItems.ApplyTemplate();
        }

        private async void buttonSearchPair_Click(object sender, RoutedEventArgs e)
        {
            string str  = textBoxSearchPair.Text;

            if (string.IsNullOrEmpty(str))
                return;

            str     = iDnaBaseNucleotides.Instance.GetPairString(str);
            await iDnaSequence.Instance.FindString(str, isPairSearch: true, this.Dispatcher);
            //listItems.InvalidateVisual();
        }
    }

    public class MouseWheelZoom
    {
        private readonly FrameworkElement _element;
        private double _currentZoomFactor;

        public MouseWheelZoom(FrameworkElement element)
        {
            _element = element;
            _currentZoomFactor = 1.0;
        }

        public async void Zoom(object sender, MouseWheelEventArgs e)
        {
            if(_element == null)
                return;

            Dispatcher  dispatcher = _element.Dispatcher;

            await dispatcher.InvokeAsync(() =>
            {
                var handle = (Keyboard.Modifiers & ModifierKeys.Control) > 0;
                if (!handle)
                    return;

                ApplyZoom(e.Delta);
            });
        }

        private void ApplyZoom(int delta)
        {
            var zoomScale = delta / 500.0;
            var newZoomFactor = _currentZoomFactor += zoomScale;
            _element.LayoutTransform = new ScaleTransform(newZoomFactor, newZoomFactor);
            _currentZoomFactor = newZoomFactor;
        }
    }
}
