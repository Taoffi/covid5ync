using isosoft.root;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace iDna
{
	public delegate void SequenceParseCompleteHandler(iDnaSequence sender, int nodeCount);

	public class iDnaSequence : RootListTemplate<iDnaNode>
	{
		static iDnaSequence							_instance			= null;

		public event SequenceParseCompleteHandler	SequenceParseCompleted;

		protected string							_id						= Guid.NewGuid().ToString();
		protected iDnaBaseStats						_stats					= new iDnaBaseStats();
		protected bool								_isBusy					= false;
		protected CancellationTokenSource			_cancelSource			= new CancellationTokenSource(5);
		protected string							_sequenceFileInfo		= "";
		protected iDnaSequencePaging				_paging					= new iDnaSequencePaging(1000);
		protected string							_searchString			= "",
													_searchPairString		= "";
		protected int								_searchCount			= 0,
													_searchPairCount		= 0,
													_minSearchLenght		= 3;
		protected bool								_searchInProgress		= false;
		protected iDnaSequenceList					_selectionBasket		= new iDnaSequenceList(),
													_pairSelectionBasket	= new iDnaSequenceList();

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
		}

		protected iDnaSequence(IEnumerable<iDnaNode> nodeList, bool refOnly = true) : base()
		{
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


		public string Id
		{
			get {  return _id; }
		}

		public iDnaSequenceList SelectionBasket
		{
			get {  return _selectionBasket; }
			protected set
			{
				if(value == _selectionBasket)
					return;

				if(value == null)
					_selectionBasket.Clear();
				else
					_selectionBasket	= value;
				NotifyPropertyChanged(() => SelectionBasket);
			}
		}


		public iDnaSequenceList PairSelectionBasket
		{
			get {  return _pairSelectionBasket; }
			protected set
			{
				if(value == _pairSelectionBasket)
					return;

				if(value == null)
					_pairSelectionBasket.Clear();
				else
					_pairSelectionBasket = value;
				NotifyPropertyChanged(() => PairSelectionBasket);
			}
		}

		public IEnumerable<iDnaNode> SelectedItems
		{
			get { return this.Where(i => i.IsSelected); }
		}


		public string SequenceFileInfo
		{
			get { return _sequenceFileInfo; }
			set
			{
				if(value == _sequenceFileInfo)
					return;

				_sequenceFileInfo	= value;
				NotifyPropertyChanged(() => SequenceFileInfo);
			}
		}



		public string SequenceString
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
			foreach(var baseIem in iDnaBaseNucleotides.Instance)
			{
				var item	= _stats[baseIem];

				if(item != null)
					item.Count	= this.Count( i => i.RootBaseItem == baseIem);

				NotifyPropertyChanged(() => Statistics);
			}
		}


		public iDnaBaseStats Statistics
		{
			get { return _stats; }
		}


		public async Task<bool> ParseString(string str)
		{
			if (string.IsNullOrEmpty(str))
				return false;

			this.Clear(false);
			IsBusy	= true;
			_cancelSource.Dispose();
			_cancelSource			= new CancellationTokenSource();
			int				index	= 0;

			await Task.Run(() =>
			{
				foreach (var chr in str)
				{
					if(_cancelSource.Token.IsCancellationRequested)
						break;

					if(! iDnaBaseNucleotides.IsValidChar(chr))
						continue;

					this.AddNode(this, chr, ++index);

					//// refersh if we areon first non-full page
					//if(_paging.CurrentPage <= 1 && this.Count <= _paging.PageSize)
					//	Dispatcher.CurrentDispatcher.Invoke(()=> NotifyPropertyChanged(() => SequencePaging));
				}
			}, _cancelSource.Token);

			IsBusy	= false;
			
			await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
			{
				if(SequenceParseCompleted != null)
					SequenceParseCompleted.Invoke(this, this.Count);

				//NotifyCollectionChanged(this);
				NotifyPropertyChanged(() => Count);
				NotifyPropertyChanged(() => Statistics);
				NotifyPropertyChanged(() => SequencePaging);
			}
			);
			return this.Count > 0;
		}


		public int SearchOccurrences
		{
			get { return _searchCount; }
			protected set
			{
				if(value == _searchCount)
					return;

				_searchCount	= value;
				NotifyPropertyChanged(() => SearchOccurrences);
			}
		}


		public int SearchPairOccurrences
		{
			get { return _searchPairCount; }
			protected set
			{
				if(value == _searchPairCount)
					return;

				_searchPairCount = value;
				NotifyPropertyChanged(() => SearchPairOccurrences);
			}
		}


		public string SearchString
		{
			get { return _searchString; }
			set
			{
				if(value == _searchString)
					return;

				_searchString = value;
				NotifyPropertyChanged(() => SearchString);

				//ResetSelection(false);

				//if (!_searchInProgress && !string.IsNullOrEmpty(value) && value.Length > _minSearchLenght)
				//{
				//	Task.Run(() => FindString( value, isPairSearch: false, null));
				//}
			}
		}


		public string SearchPairString
		{
			get { return _searchPairString; }
			set
			{
				if (value == _searchPairString)
					return;

				_searchPairString = value;
				NotifyPropertyChanged(() => SearchPairString);

				//ResetSelection(false);

				//if (! _searchInProgress && !string.IsNullOrEmpty(value) && value.Length > _minSearchLenght)
				//	Task.Run(() => FindString( iDnaBaseNucleotides.Instance.GetPairString(_searchPairString), isPairSearch: true, null));
			}
		}

		public bool CanSearch
		{
			get
			{
				if(IsBusy)
				{
					if(_searchInProgress)
						return true;

					return false;
				}

				return true;
			}
		}


		void ResetSelection(bool selectValue, Dispatcher dispatcher)
		{
			if (dispatcher == null)
				dispatcher = Dispatcher.CurrentDispatcher;

			dispatcher.Invoke(() =>
			{
				foreach(var item in this)
					item.IsSelected	= selectValue;
			}
			);
		}


		public async Task<IEnumerable<iDnaNode>> FindString(string str, bool isPairSearch, Dispatcher dispatcher)
		{
			if (string.IsNullOrWhiteSpace(str))
				return null;

			iDnaSequenceList		basket	= isPairSearch ? _pairSelectionBasket : _selectionBasket;

			basket.Clear();

			if (isPairSearch)
			{
				SearchPairOccurrences = 0;
				NotifyPropertyChanged(() => PairSelectionBasket);
			}
			else
			{
				SearchOccurrences = 0;
				NotifyPropertyChanged(() => SelectionBasket);
			}

			str		= iDnaBaseNucleotides.TrimInvalidChars(str);

			if (! iDnaBaseNucleotides.Instance.IsValidSearchString(str))
				return null;

			bool		resetBusy	= false;

			_searchInProgress	= true;
			if(! _isBusy)
			{
				resetBusy		= true;
				IsBusy		= true;
			}
			
			NotifyPropertyChanged(() => CanSearch);

			if( dispatcher == null)
				dispatcher	= Dispatcher.CurrentDispatcher;

			List<iDnaNode>	list		= new List<iDnaNode>();
			var				allStarts	= this.Where( i => str[0] == i.Code).ToList();
			int				nodeNdx,
							ndxEnd,
							ndxStr,
							lenStr			= str.Length;

			if(allStarts == null)
				return null;

			int				startsCount	= allStarts.Count(),
							ndxStarts	= 0;

			await Task.Run(() =>
			{

				//foreach(var item in allStarts)
				while ((ndxStarts /*+ lenStr*/) < startsCount)
				{
					iDnaNode item	= allStarts[ndxStarts];

					nodeNdx			= item.Index;
					ndxEnd			= nodeNdx + lenStr;
					var subSeq		= this.Where(i => i.Index >= nodeNdx && i.Index < ndxEnd);

					if (subSeq == null)
						continue;

					ndxStr			= 0;
					bool mismatch	= false;

					foreach (var node in subSeq)
					{
						if (node.Code != str[ndxStr] && ndxStr < lenStr)
						{
							mismatch	= true;
							break;
						}
						ndxStr++;
					}

					if (!mismatch && ndxStr == lenStr)
					{
						list.AddRange(subSeq);
						//ndxStarts		+= lenStr;

						basket.Add( new iDnaSequence(subSeq));
					}

					ndxStarts++;
				}
			});
			

			// select found nodes
			dispatcher.Invoke(() =>
			{
				ResetSelection(false, dispatcher: dispatcher);

				foreach (var item in list)
					item.IsSelected = true;
			}
			);

			var		distinctIndexes	= list.Select(i => i.Index).Distinct();
			int		distinctCount	= distinctIndexes.Count() / lenStr;

			if (isPairSearch)
				SearchPairOccurrences = distinctCount;
			else
				SearchOccurrences = distinctCount;
			
			_searchInProgress	= false;

			if(resetBusy)
				IsBusy				= false;

			NotifyPropertyChanged(() => CanSearch);
			//_paging.RaisePagingEvents();
			//NotifyPropertyChanged(() => _paging.CurrentPageData);
			//NotifyPropertyChanged(() => _paging.CurrentPage);

			return list;
		}


		public static async Task<iDnaSequence> FromString(string str)
		{
			iDnaSequence		sequence	= new iDnaSequence();

			await sequence.ParseString(str);
			return sequence;
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
		}
	}

}
