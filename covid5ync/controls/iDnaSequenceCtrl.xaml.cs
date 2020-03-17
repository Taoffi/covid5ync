using isosoft.root;
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
        ICommand        _commandNextPage,
                        _commandPrevPage;

		public iDnaSequenceCtrl()
		{
			InitializeComponent();
			//this.Loaded += new RoutedEventHandler(Control_Loaded);
		}

		//private void Control_Loaded(object sender, RoutedEventArgs e)
		//{
		//	var presenter		= UiHelpers.FindChild<ScrollContentPresenter>(listItems, null);
		//	var mouseWheelZoom  = new MouseWheelZoom(presenter);
		//	PreviewMouseWheel   += mouseWheelZoom.Zoom;
		//}


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

            await iDnaSequence.Instance.FindString( str, SequenceSearchType.SearchNormal, this.Dispatcher, resetSelections: true, gotoFirstNodePage: true );
        }

        private async void buttonSearchPair_Click(object sender, RoutedEventArgs e)
        {
            string str  = textBoxSearchPair.Text;

            if (string.IsNullOrEmpty(str))
                return;

            str     = iDnaBaseNucleotides.Instance.GetPairString(str);
            await iDnaSequence.Instance.FindString(str, SequenceSearchType.SearchPairs, this.Dispatcher, resetSelections: true, gotoFirstNodePage: true);
        }

 
        public ICommand NextPageCommand
        {
            get
            {
                if (_commandNextPage == null)
                {
                    _commandNextPage = new CommandExecuter(async () =>
                    {
                        iDnaSequence            seq     = this.DataContext as iDnaSequence;
                        iDnaSequencePaging      paging  = seq == null ? null : seq.SequencePaging;

                        if (paging == null || ! paging.HasNext)
                            return;

                        await Task.Run(() => paging.CurrentPage++);
                    });
                }
                return _commandNextPage;
            }
        }

        public ICommand PrevPageCommand
        {
            get
            {
                if (_commandPrevPage == null)
                {
                    _commandPrevPage = new CommandExecuter(async () =>
                    {
                        iDnaSequence        seq     = this.DataContext as iDnaSequence;
                        iDnaSequencePaging  paging  = seq == null ? null : seq.SequencePaging;

                        if (paging == null || !paging.HasPrevious)
                            return;

                        await Task.Run(() => paging.CurrentPage--);
                    });
                }
                return _commandPrevPage;
            }
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
            e.Handled   = false;
            if(_element == null)
                return;

            Dispatcher  dispatcher = _element.Dispatcher;

            await dispatcher.InvokeAsync(() =>
            {
                var handle = (Keyboard.Modifiers & ModifierKeys.Control) > 0;
                if (!handle)
                {
                    return;
                }

                ApplyZoom(e.Delta);
                e.Handled  = true;
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
