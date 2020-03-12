using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace iDna
{
	public partial class iDnaSequence
	{

		internal bool AddRegion(iDnaRegionIndex newRegion)
		{
			if(newRegion == null)
				return false;

			int		count		= _namedRegionsList.Count;

			_namedRegionsList.Add(newRegion);

			if(count == _namedRegionsList.Count)
				return false;

			NotifyPropertyChanged(() => SequenceNamedRegionList);
			return true;
		}

		internal void Select(int minNdx, int maxNdx, bool resetCurrentSelections)
		{
			var		nodes	= this.Where(i => i.Index >= minNdx && i.Index <= maxNdx);
			if(nodes == null || nodes.Count() <= 0)
				return;

			if(resetCurrentSelections)
				this.ResetSelection(false, null);

			Dispatcher.CurrentDispatcher.InvokeAsync(() =>
			{
				foreach(var n in nodes)
					n.IsSelected	= true;
			});
		}

		internal void RefreshNamedRegions()
		{
			NotifyPropertyChanged(() => SequenceNamedRegionList);
		}
	}
}
