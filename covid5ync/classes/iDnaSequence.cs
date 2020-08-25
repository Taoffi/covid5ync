using isosoft.root;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace iDna
{
	public delegate void SequenceParseCompleteHandler(iDnaSequence sender, int nodeCount);
	public enum SequenceSearchType
	{
		SearchNormal,
		SearchPairs,
		SearchRepeats,
	};

	public partial class iDnaSequence : RootListTemplate<iDnaNode>
	{
		static iDnaSequence							_instance			= null;

		public event SequenceParseCompleteHandler	SequenceParseCompleted;

		protected string							_id						= Guid.NewGuid().ToString();
		protected string							_name					= "sequence1";
		protected iDnaBaseStats						_stats					= new iDnaBaseStats();
		protected bool								_isBusy					= false;
		protected CancellationTokenSource			_cancelSource			= new CancellationTokenSource(5);
		protected string							_sequenceFileInfo		= "";
		protected iDnaSequencePaging				_paging					= new iDnaSequencePaging(1000);
		protected int								_nOccurrences			= 1;
		protected iDnaRegionIndexList				_namedRegionsList			= new iDnaRegionIndexList();


		public static iDnaSequence Instance
		{
			get
			{
				if(_instance == null)
					_instance = new iDnaSequence();
				return _instance;
			}
		}


		public static iDnaSequence GetInstance()
		{
			return Instance;
		}
			

		
		
		public iDnaSequence() : base()
		{
			_paging.SourceCollection	= this;
			_namedRegionsList			= new iDnaRegionIndexList(this, "Sequence Regions");

			_repeatSortOptions.SelectedOptionChanged	+= _repeatSortOptions_SelectedOptionChanged;
			_searchSortOptions.SelectedOptionChanged	+= _searchSortOptions_SelectedOptionChanged;
			_hairpinSortOptions.SelectedOptionChanged	+= _hairpinSortOptions_SelectedOptionChanged;
		}

		internal iDnaSequence(string name, IEnumerable<iDnaNode> nodeList, bool refOnly = true, int nOccurrences = 1) : base()
		{
			_paging.SourceCollection	= this;
			_namedRegionsList			= new iDnaRegionIndexList(this, "Sequence Regions");

			_name			= name;
			_nOccurrences	= nOccurrences;
			CopyNodeList(nodeList, refOnly);
		}

		/// <summary>
		/// replace current sequence with the list of node. Embed or keep nodes parent and index.
		/// </summary>
		/// <param name="nodeList">nodes to copy</param>
		/// <param name="refOnly">when true: embed and reindex copied nodes (detatch from the source). 
		/// otherwise: keep parent and index</param>
		protected void CopyNodeList(IEnumerable<iDnaNode> nodeList, bool refOnly = true)
		{
			this.Clear();
			if(nodeList == null)
				return;

			this.AddRange(nodeList);
			_stats.Total		= this.Count;

			// embed nodes: set parent to this and reset nodes index
			if( ! refOnly)
				ReIndexNodes();
		}


		/// <summary>
		/// embed nodes: set parent to this and reset nodes index
		/// </summary>
		protected void ReIndexNodes()
		{
			int			ndx			= 1;

			foreach(var node in this)
			{
				node.ParentSequence	= this;
				node.Index			= ndx;
				ndx++;
			}
		}
		public iDnaSequencePaging SequencePaging
		{
			get { return _paging; }
		}



		public int PaginPageSize
		{
			get { return _paging.PageSize; }
			set
			{
				if(value == _paging.PageSize)
					return;

				Dispatcher.CurrentDispatcher.Invoke(() =>
				{
					IsBusy			= true;
					Thread.Sleep(500);
					_paging.PageSize	= value;
					IsBusy = false;
				}
				);
				
			}
		}

		public IEnumerable<iDnaNode> this[char code]
		{
			get { return this.Where(i => i.Code == code); }
		}

		public IEnumerable<iDnaNode> this[iDnaBase rootBase]
		{
			get { return this.Where(i => i.RootBaseItem == rootBase); }
		}

		public double SequenceLinearBaseValue
		{
			get { return this.Sum(i => i.LinearBaseValue); }
		}



		public IEnumerable<iDnaNode> SelectedItems
		{
			get { return this.Where(i => i.IsSelected).OrderBy(i => i.Index); }
		}


		public string SequenceFlatString
		{
			get
			{
				string		str		= "";

				foreach(var node in this)
					str	+= node.Code.ToString();

				return str;
			}
		}

		public bool IsBusy
		{
			get {  return _isBusy; }
			set
			{
				if(value == _isBusy)
					return;

				_isBusy	= value;
				Dispatcher.CurrentDispatcher.Invoke(() =>
				{
					NotifyPropertyChanged(() => IsBusy);
					NotifyPropertyChanged(() => CanSearch);
				});
			}
		}

		public CancellationTokenSource Cancellation
		{
			get { return _cancelSource; }
		}

		public CancellationTokenSource CurrentCancellationSource
		{
			get { return _cancelSource; }
			set
			{
				_cancelSource	= value;
				NotifyPropertyChanged(()=> CurrentCancellationSource);
				NotifyPropertyChanged(() => Cancellation);
			}
		}

		public bool AddNode(iDnaSequence parentSequence, char code, int index)
		{
			iDnaBase	rootBase	= iDnaBaseNucleotides.Instance[code];

			if(rootBase == null)
				return false;

			this.Add(new iDnaNode(parentSequence, rootBase, index));
			UpdateStats();
			return true;
		}

		protected void UpdateStats()
		{
			if(_stats == null)
				return;

			foreach (var baseIem in iDnaBaseNucleotides.Instance)
			{
				var item	= _stats[baseIem];

				if(item != null)
				{
					item.Count = this.Count(i => i.RootBaseItem == baseIem);
				}
			}
			_stats.SetTotalNoNotify(this.Count);
			_stats.NotifyChanges();
		}


		public iDnaBaseStats Statistics
		{
			get { return _stats; }
		}

		void InitializeCancellationSource()
		{
			if (this._cancelSource != null)
			{
				_cancelSource.Cancel();
			}

			_cancelSource.Dispose();
			CurrentCancellationSource	= new CancellationTokenSource();
		}

		public bool HasPendingChanges
		{
			get
			{
				if (_selectionBasket.Count > 0
						|| _pairSelectionBasket.Count > 0
						|| _repeatsBasket.Count > 0
						|| _hairPinBasket.Count > 0
						|| _namedRegionsList.Count > 0)
					return true;

				return false;
			}
		}


		bool InitializeNewSequence()
		{
			this._selectionBasket.Clear();
			this._pairSelectionBasket.Clear();
			this._repeatsBasket.Clear();
			this._hairPinBasket.Clear();
			this._repeatSearch.Clear();
			this._stringOccurList.Clear();
			this._pairStringOccurList.Clear();

			_namedRegionsList = new iDnaRegionIndexList(this, "Sequence Regions");

			NotifyPropertyChanged(() => SelectionBasket);
			NotifyPropertyChanged(() => CurrentSearchBasket);
			NotifyPropertyChanged(() => CurrentSearchBasketSorted);
			NotifyPropertyChanged(() => CurrentRepeatsBasket);
			NotifyPropertyChanged(() => CurrentRepeatsBasketSorted);
			NotifyPropertyChanged(() => SequenceNamedRegionList);
			NotifyPropertyChanged(() => HairpinCount);
			NotifyPropertyChanged(() => RepeatsCount);
			NotifyPropertyChanged(() => SearchOccurrences);

			return true;
		}


		public async Task<bool> ParseString(string str)
		{
			if (string.IsNullOrEmpty(str))
				return false;

			InitializeNewSequence();
			InitializeCancellationSource();
			
			this.Clear(false);

			this.Id					= Guid.NewGuid().ToString();

			IsBusy					= true;
			int				index	= 0;

			await Task.Run(() =>
			{
				foreach (var chr in str)
				{
					if(_cancelSource.Token.IsCancellationRequested)
						break;

					char	c		= char.ToLower(chr);
					if(! iDnaBaseNucleotides.IsValidChar( c))
						continue;

					this.AddNode(this, c, ++index);

					//// refersh if we are on first non-full page
					//if(_paging.CurrentPage <= 1 && this.Count <= _paging.PageSize)
					//	Dispatcher.CurrentDispatcher.Invoke(()=> NotifyPropertyChanged(() => SequencePaging));
				}
			}, _cancelSource.Token);

			IsBusy	= false;
			
			await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
			{
				if(SequenceParseCompleted != null)
					SequenceParseCompleted.Invoke(this, this.Count);

				NotifyPropertyChanged(() => Count);
				NotifyPropertyChanged(() => Statistics);
				NotifyPropertyChanged(() => SequencePaging);
			}
			);

			_stats.Total	= this.Count;
			return this.Count > 0;
		}

		internal void ClearSearchAndSelectionBaskets()
		{
			_selectionBasket.Clear();
			_pairSelectionBasket.Clear();

			NotifyPropertyChanged(() => SelectionBasket);
			NotifyPropertyChanged(() => PairSelectionBasket);
			NotifyPropertyChanged(() => CurrentSearchBasket);
			NotifyPropertyChanged(() => CurrentSearchBasketSorted);
			NotifyPropertyChanged(() => PairSelectionBasket);
		}


		internal void ClearRepeatsBaskets()
		{
			_repeatsBasket.Clear();
			_repeatSearch.Clear();

			NotifyPropertyChanged(() => RepeatsBasket);
			NotifyPropertyChanged(() => RepeatsBasketSorted);
			NotifyPropertyChanged(() => RepeatsCount);
		}


		internal void ClearHairpinBaskets()
		{
			_hairPinBasket.Clear();
			_repeatSearch.Clear();

			NotifyPropertyChanged(() => HairpinBasket);
			NotifyPropertyChanged(() => HairpinBasketSorted);
			NotifyPropertyChanged(() => HairpinCount);
		}


		internal void ResetSelection(bool selectValue, Dispatcher dispatcher)
		{
			if (dispatcher == null)
				dispatcher = Dispatcher.CurrentDispatcher;

			dispatcher.Invoke(() =>
			{
				foreach(var item in this)
					item.IsSelected	= selectValue;
			}
			);

			_selectionBasket.Clear();
			_pairSelectionBasket.Clear();

			NotifyPropertyChanged(() => SelectionBasket);
			NotifyPropertyChanged(() => PairSelectionBasket);
			NotifyPropertyChanged(() => CurrentSearchBasket);
			NotifyPropertyChanged(() => CurrentSearchBasketSorted);
			NotifyPropertyChanged(() => PairSelectionBasket);
		}


		internal void GoToNodePage(int nodeIndex)
		{
			if(nodeIndex < 0 || nodeIndex >= this.Count)
				return;

			int		curPageNumber	= _paging.CurrentPage,
					nodesPerPage	= _paging.PageSize,
					nodePageNumber	= nodeIndex / nodesPerPage + 1;
			bool	isVisible		= nodePageNumber == curPageNumber;

			if(isVisible)
				return;

			_paging.CurrentPage	= nodePageNumber;
		}


		internal void GoToNodePage(iDnaNode node)
		{
			if(node == null)
				return;

			GoToNodePage(node.Index);
		}

	}


	public class iDnaSequenceDesignTime : iDnaSequence
	{

		public iDnaSequenceDesignTime()
		{
			init_instance();
		}

		private async void init_instance()
		{
			this.Clear();

			await this.ParseString(
@"tttcccaggtaacaaaccaaccaactttcgatctcttgtagatctgttctctaaacgaac
tttaaaatctgtgtggctgtcactcggctgcatgcttagtgcactcacgcagtataatta
ataactaattactgtcgttgacaggacacgagtaactcgtctatcttctgcaggctgctt");

			// add some selection for designtime
			int[]	selectedNodes	= { 3, 4, 5, 6,7,8,9,10, 28, 29, 30};

			foreach(int ndx in selectedNodes)
				this[ndx].IsSelected	= true;

			string[]			str_repeats	= { "caggtaacaaa", "caggtaacaaa" },
								str_hp		= { "caggtaacaaa", iDnaBaseNucleotides.Instance.GetPairString("caggtaacaaa") };
			List<iDnaNode>		repeats		= new List<iDnaNode>(),
								hairpins	= new List<iDnaNode>();
			foreach(var s in str_repeats)
			{
				int ndx = 1;
				foreach(var c in s)
					repeats.Add(new iDnaNode(this, iDnaBaseNucleotides.Instance[c], ndx++));
			}

			foreach (var s in str_hp)
			{
				int ndx = 1;
				foreach (var c in s)
					hairpins.Add(new iDnaNode(this, iDnaBaseNucleotides.Instance[c], ndx++));
			}

			this._repeatsBasket.Add(new iDnaSequence($"repeat 1", repeats, true, 1));
			this._hairPinBasket.Add(new iDnaSequence($"hairpin 1", hairpins, true, 1));
		}
	}

}
