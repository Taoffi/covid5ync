using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace iDna.controls
{
	/// <summary>
	/// Interaction logic for BusySpinCtrl.xaml
	/// </summary>
	public partial class BusySpinCtrl : UserControl
	{
		public BusySpinCtrl()
		{
			InitializeComponent();
			Loaded += BusySpinCtrl_Loaded;
		}

		private void BusySpinCtrl_Loaded(object sender, RoutedEventArgs e)
		{
			StartAnimation();
			this.IsVisibleChanged	+= UserControl_IsVisibleChanged;
		}


		private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			StartAnimation();
		}

		async void StartAnimation()
		{
			RotateImage.ToolTip = "click to cancel";
			try
			{
				//if(System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
				//	return;

				Storyboard story = this.FindResource("StoryboardSpin") as Storyboard;

				if (story == null)
					return;

				if (this.Visibility == Visibility.Visible)
					await Dispatcher.InvokeAsync(() => story.Begin());
				else
					await Dispatcher.InvokeAsync(() => story.Stop());
			}
			catch (Exception)
			{
			}
		}


		private void RotateImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			CancellationTokenSource	cancellation	= iDnaSequence.Instance.CurrentCancellationSource;
			if(cancellation != null)
			{
				RotateImage.ToolTip = "cancelling...";
				iDnaSequence.Instance.CurrentCancellationSource.Cancel(true);
				return;
			}

			RotateImage.ToolTip = "could not cancel!...";
		}
	}
}
