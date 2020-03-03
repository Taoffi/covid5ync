using doc5Words.vm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace iDna
{
	public class iDnaSequencePaging : iObjectPaging<iDnaNode>
	{
		public iDnaSequencePaging() : base()
		{

		}

		public void ResetSelection(bool select)
		{
			var		data	= this.SourceCollection;

			if(data == null || data.Count() <= 0)
				return;

			Task.Run(() =>
			{
				foreach(var item in data)
					item.IsSelected	= select;
			});
		}


		public void Refresh()
		{
			var data = this.CurrentPageData;	//.SourceCollection;

			if (data == null || data.Count() <= 0)
				return;

			Dispatcher.CurrentDispatcher.Invoke(() =>
			{
				foreach(var item in data)
					NotifyPropertyChanged(() => item.Color);
			});
		}

		public iDnaSequencePaging(int pageSize) : base(pageSize)
		{
		}
	}

}
