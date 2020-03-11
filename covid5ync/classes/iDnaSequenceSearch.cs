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
		protected List<string>						_searchStringList			= new List<string>(),	// "",
													_searchPairStringList		= new List<string>();	// "";
		protected string							_searchString			= "",
													_searchPairString		= "";
		protected int								_searchCount			= 0,
													_searchPairCount		= 0,
													_minSearchLenght		= 3;
		protected bool								_searchInProgress		= false;
		protected List<iDnaSequence>				_selectionBasket		= new List<iDnaSequence>(),		// iDnaSequenceList(),
													_pairSelectionBasket	= new List<iDnaSequence>();		// iDnaSequenceList();
		protected SequenceSearchType				_currentSearchType		= SequenceSearchType.SearchNormal;
		protected vm.iDnaSequenceSortOptionList		_searchSortOptions		= new vm.iDnaSequenceSortOptionList();


		public vm.iDnaSequenceSortOptionList SearchSortOptionList
		{
			get { return _repeatSortOptions; }
		}

		iDnaBasketSortOption SearchSortOption
		{
			get { return _searchSortOptions.SelectedOption; }
			set
			{
				if(value == _searchSortOptions.SelectedOption)
					return;

				_searchSortOptions.SelectedOption	= value;
				NotifyPropertyChanged(()=> SearchSortOption);
				NotifyPropertyChanged(() => CurrentSearchBasketSorted);
			}
		}


		private void _searchSortOptions_SelectedOptionChanged(vm.iDnaSequenceSortOption selectedItem)
		{
			NotifyPropertyChanged(() => SearchSortOption);
			NotifyPropertyChanged(() => CurrentSearchBasketSorted);
		}


		public IEnumerable<iDnaSequence> CurrentSearchBasketSorted
		{
			get
			{
				switch(RepeatSortOption)
				{
					case iDnaBasketSortOption.NoSort:
					default:
						return CurrentSearchBasket;

					case iDnaBasketSortOption.SortByNumberOfOccurrences:
						return CurrentSearchBasket.OrderBy(i => i._nOccurrences).ToList();

					case iDnaBasketSortOption.SortByPosition:
						return CurrentSearchBasket.OrderBy(s => (s == null || s.Count<=0) ? 0 : s[0].Index).ToList();
				}
			}
		}



		public SequenceSearchType CurrentSearchType
		{
			get { return _currentSearchType; }
			protected set
			{
				if(value == _currentSearchType)
					return;

				_currentSearchType	= value;
				NotifyPropertyChanged(() => CurrentSearchType);
				NotifyPropertyChanged(() => CurrentSearchBasket);
				NotifyPropertyChanged(() => CurrentSearchBasketSorted);
			}
		}


		public List<iDnaSequence> CurrentSearchBasket
		{
			get {  return _currentSearchType == SequenceSearchType.SearchNormal ? _selectionBasket : _pairSelectionBasket; }
		}


		public List<iDnaSequence> SelectionBasket
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
				NotifyPropertyChanged(() => CurrentSearchBasket);
				NotifyPropertyChanged(() => CurrentSearchBasketSorted);
			}
		}


		public List<iDnaSequence> PairSelectionBasket
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
				NotifyPropertyChanged(() => CurrentSearchBasket);
				NotifyPropertyChanged(() => CurrentSearchBasketSorted);
			}
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
				//if(value == _searchString)
				//	return;

				_searchString = value;
				NotifyPropertyChanged(() => SearchString);
			}
		}


		public string SearchPairString
		{
			get { return _searchPairString; }
			set
			{
				//if (value == _searchPairString)
				//	return;

				_searchPairString = value;
				NotifyPropertyChanged(() => SearchPairString);
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

		public List<string> SearchStringList
		{
			get {  return _searchStringList; }
			set
			{
				if(value == _searchStringList)
					return;

				if(value == null)
					_searchStringList.Clear();
				else
					_searchStringList	= value;

				NotifyPropertyChanged(() => SearchStringList);
			}
		}


		public List<string> SearchPairStringList
		{
			get {  return _searchPairStringList; }
			set
			{
				if(value == _searchPairStringList)
					return;

				if(value == null)
					_searchPairStringList.Clear();
				else
					_searchPairStringList = value;

				NotifyPropertyChanged(() => SearchPairStringList);
			}
		}

		public async Task<int> SearchCurrentStringList( Dispatcher dispatcher, bool updateSelections, bool clearSelectionBasket = true)
		{
			if(_searchStringList == null || _searchStringList.Count <= 0)
				return 0;

			InitializeCancellationSource();
			int occurrences = await FindStringList(_searchStringList, SequenceSearchType.SearchNormal,
													dispatcher, 
													updateSelections, 
													clearSelectionBasket: false);
			return occurrences;
		}


		public async Task<int> SearchCurrentPairStringList(Dispatcher dispatcher, bool updateSelections, bool clearSelectionBasket = true)
		{
			if (_searchPairStringList == null || _searchPairStringList.Count <= 0)
				return 0;

			if(clearSelectionBasket)
				_pairSelectionBasket.Clear();

			int occurrences = await FindStringList(_searchPairStringList, SequenceSearchType.SearchPairs, dispatcher, updateSelections, clearSelectionBasket: false);
			return occurrences;
		}


		public async Task<int> FindStringList(List<string> stringList, 
												SequenceSearchType searchType, 
												Dispatcher dispatcher, 
												bool updateSeletions = true, 
												bool clearSelectionBasket = true)
		{
			if(stringList == null || stringList.Count <= 0)
				return 0;

			int			occurrences		= 0;
			InitializeCancellationSource();

			await Task.Run( async() =>
			{
				foreach(var str in stringList)
				{
					var foundCollection = await FindString(str, searchType, 
															dispatcher, 
															resetSelections: false,
															gotoFirstNodePage: false,
															updateSeletions: updateSeletions, 
															clearSelectionBasket,
															initCancellation: false);

					if(_cancelSource.IsCancellationRequested)
						break;

					int itemsFound		= foundCollection == null ? 0 : foundCollection.Count();

					//if(itemsFound > 0)
					{
						occurrences += itemsFound;
					}
				}
			}, _cancelSource.Token);

			Dispatcher.CurrentDispatcher.Invoke(() =>
			{
				NotifyPropertyChanged(() => CurrentSearchBasket);
				NotifyPropertyChanged(() => CurrentSearchBasketSorted);
			});
			return occurrences;
		}


		public async Task<IEnumerable<iDnaNode>> FindString(string str, SequenceSearchType searchType, 
												Dispatcher dispatcher, 
												bool resetSelections,
												bool gotoFirstNodePage,
												bool updateSeletions = true,
												bool clearSelectionBasket = true,
												bool initCancellation = false)
		{
			if (string.IsNullOrWhiteSpace(str))
				return null;

			List<iDnaSequence>	basket			= null;
			bool				isPairSearch	= false;

			switch (searchType)
			{
				case SequenceSearchType.SearchNormal:
					basket			= _selectionBasket;
					break;

				case SequenceSearchType.SearchPairs:
					basket			= _pairSelectionBasket;
					isPairSearch	= true;
					break;

				case SequenceSearchType.SearchRepeats:
					basket	= _repeatSearch;
					break;

				default:
					return null;
			}

			if(clearSelectionBasket)
				basket.Clear();

			if(initCancellation)
				InitializeCancellationSource();

			CurrentSearchType	 = searchType;

			if (isPairSearch)
			{
				SearchPairOccurrences	= 0;
				NotifyPropertyChanged(() => PairSelectionBasket);
				NotifyPropertyChanged(() => CurrentSearchBasket);
			}
			else
			{
				SearchOccurrences = 0;
				if(searchType != SequenceSearchType.SearchRepeats)
					NotifyPropertyChanged(() => CurrentSearchBasket);
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

			if (searchType != SequenceSearchType.SearchRepeats)
				NotifyPropertyChanged(() => CanSearch);

			if( dispatcher == null)
				dispatcher	= Dispatcher.CurrentDispatcher;

			List<iDnaNode>	list		= new List<iDnaNode>();
			int				lenStr			= str.Length,
							nOccurrences	= 0;
			var				allStarts		= this.AllStartoccurrencesOfString(str).ToList();

			if(allStarts == null || allStarts.Count <= 0)
				goto end_of_process;

			int				startsCount		= allStarts.Count();

			if (resetSelections)
			{
				ResetSelection(false, dispatcher: dispatcher);
			}

			await Task.Run(() =>
			{
				foreach( var item in allStarts)
				{
					if(_cancelSource != null && _cancelSource.IsCancellationRequested)
						break;

					var			subSeq	= this.SkipWhile(n => n.Index < item.Index).Take(lenStr);

					if(subSeq != null && subSeq.Count() > 0)
					{
						nOccurrences++;
						list.AddRange(subSeq);
						basket.Add(new iDnaSequence("search match " + nOccurrences.ToString(), subSeq));

						if(updateSeletions)
						{
							foreach(var node in subSeq)
							{
								node.IsSelected = true;
							}
						}
					}
				}

				if (searchType != SequenceSearchType.SearchRepeats)
				{
					NotifyPropertyChanged(() => SelectionBasket);
					NotifyPropertyChanged(() => PairSelectionBasket);
					NotifyPropertyChanged(() => CurrentSearchBasket);
				}
			});
			
end_of_process:
			int		distinctCount	= nOccurrences;

			if (isPairSearch)
				SearchPairOccurrences = distinctCount;
			else
				SearchOccurrences = distinctCount;
			
			_searchInProgress	= false;

			if(resetBusy)
				IsBusy				= false;

			if (searchType != SequenceSearchType.SearchRepeats)
			{
				NotifyPropertyChanged(() => CanSearch);
				NotifyPropertyChanged(() => CurrentSearchBasket);
			}

			if(list.Count > 0 && gotoFirstNodePage)
				GoToNodePage(list[0]);

			return list;
		}


	}
}
