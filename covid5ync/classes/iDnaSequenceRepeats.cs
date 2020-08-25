using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;
using isosoft.root;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace iDna
{
	public enum iDnaBasketSortOption
	{
		[Description("No sort")]
		NoSort,

		[Description("By position")]
		SortByPosition,

		[Description("By number of occurrences")]
		SortByNumberOfOccurrences,
	};

	public enum RepeatSerachType
	{
		[Description("Search for repeats")]
		SearchRepeats,

		[Description("Search for hairpins")]
		SerachHiarpins,
	};


	public partial class iDnaSequence
	{
		protected vm.iDnaSequenceSortOptionList	_repeatSortOptions		= new vm.iDnaSequenceSortOptionList();
		protected vm.iDnaSequenceSortOptionList	_hairpinSortOptions		= new vm.iDnaSequenceSortOptionList();
		protected List<iDnaSequence>			_repeatsBasket			= new List<iDnaSequence>(),			// iDnaSequenceList(),
												_hairPinBasket			= new List<iDnaSequence>(),			// iDnaSequenceList(),
												_repeatSearch			= new List<iDnaSequence>();			// iDnaSequenceList();
		protected int							_repeatSearchPosition	= 0;
		protected bool							_isRepeatProcessRunning	= false;
		protected CancellationTokenSource		_repeatCancelSource		= new CancellationTokenSource(5);
		protected RepeatSerachType				_repeatSerachType		= RepeatSerachType.SearchRepeats;



		public CancellationTokenSource RepeatCancellation
		{
			get { return _repeatCancelSource; }
		}


		public RepeatSerachType CurrentRepeatSearchType
		{
			get { return _repeatSerachType; }
			protected set
			{
				_repeatSerachType	= value;
				NotifyPropertyChanged(() => CurrentRepeatsBasket);
				NotifyPropertyChanged(() => CurrentRepeatsBasketSorted);
			}
		}


		public vm.iDnaSequenceSortOptionList RepeatSortOptionList
		{
			get {  return _repeatSortOptions; }
		}


		public vm.iDnaSequenceSortOptionList HairpinSortOptionList
		{
			get { return _hairpinSortOptions; }
		}



#region xxxxxxxxxxxxxxxxxxxxxx occurrence classes xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

		public class StringOccurrence
		{
			public string	ItemString	{ get; set; }
			public int		Occurrs		{ get; set; }	= 0;

			public StringOccurrence(string str, int occurrs)
			{
				ItemString		= str;
				Occurrs			= occurrs;
			}
		}

		public class StringOccurrenceList : List<StringOccurrence>
		{
			public StringOccurrenceList()
			{

			}

			public StringOccurrence this[string str]
			{
				get { return this.FirstOrDefault(i => i.ItemString == str); }
			}

			public void AddUnique(StringOccurrence item)
			{
				if(item == null || string.IsNullOrEmpty(item.ItemString))
					return;

				var	existent	= this[item.ItemString];
				if(existent == null)
					base.Add(item);
			}
		}

#endregion // xxxxxxxxxxxxxxxxxxxxxx occurrence classes xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

		protected StringOccurrenceList		_stringOccurList		= new StringOccurrenceList(),
											_pairStringOccurList	= new StringOccurrenceList();

		public int RepeatsCount
		{
			get
			{
				if(_repeatsBasket == null || _repeatsBasket.Count <= 0)
					return 0;

				return _repeatsBasket.Select(i => i.SequenceFlatString).Distinct().Count();
			}
		}

		public int HairpinCount
		{
			get
			{
				if (_hairPinBasket == null || _hairPinBasket.Count <= 0)
					return 0;

				return _hairPinBasket.Select(i => i.SequenceFlatString).Distinct().Count();
			}
		}

		public bool IsRepeatProcessRunning
		{
			get {  return _isRepeatProcessRunning; }
			set
			{
				if(value == _isRepeatProcessRunning)
					return;

				_isRepeatProcessRunning		= value;
				Dispatcher.CurrentDispatcher.Invoke(() =>
				{
					NotifyPropertyChanged(() => IsRepeatProcessRunning);
				});
			}
		}


		public int RepeatSearchPosition
		{
			get { return _repeatSearchPosition; }
			protected set
			{
				if(value == _repeatSearchPosition)
					return;

				_repeatSearchPosition = value;
				NotifyPropertyChanged(() => RepeatSearchPosition);
			}
		}


		iDnaBasketSortOption HairpinSortOption
		{
			get { return _hairpinSortOptions.SelectedOption; }
			set
			{
				if(value == _hairpinSortOptions.SelectedOption)
					return;

				_hairpinSortOptions.SelectedOption	= value;
				NotifyPropertyChanged(()=> HairpinSortOption);
				NotifyPropertyChanged(() => HairpinBasketSorted);
			}
		}

		iDnaBasketSortOption RepeatSortOption
		{
			get { return _repeatSortOptions.SelectedOption; }
			set
			{
				if(value == _repeatSortOptions.SelectedOption)
					return;

				_repeatSortOptions.SelectedOption	= value;
				NotifyPropertyChanged(()=> RepeatSortOption);
				NotifyPropertyChanged(() => RepeatsBasketSorted);
			}
		}


		public IEnumerable<iDnaSequence> RepeatsBasketSorted
		{
			get
			{
				switch(RepeatSortOption)
				{
					case iDnaBasketSortOption.NoSort:
					default:
						return RepeatsBasket;

					case iDnaBasketSortOption.SortByNumberOfOccurrences:
						return _repeatsBasket.OrderBy(i => i._nOccurrences).ToList();

					case iDnaBasketSortOption.SortByPosition:
						return _repeatsBasket.OrderBy(s => (s == null || s.Count<=0) ? 0 : s[0].Index).ToList();
				}
			}
		}

		public List<iDnaSequence> RepeatsBasket
		{
			get {  return _repeatsBasket; }
			protected set
			{
				if(value == _repeatsBasket)
					return;

				if(value == null)
				{
					_repeatsBasket.Clear();
					_stringOccurList.Clear();
				}
				else
					_repeatsBasket = value;

				NotifyPropertyChanged(() => RepeatsBasket);
				NotifyPropertyChanged(() => CurrentRepeatsBasket);
				NotifyPropertyChanged(() => CurrentRepeatsBasketSorted);
				NotifyPropertyChanged(() => RepeatsCount);
			}
		}

		public List<iDnaSequence> CurrentRepeatsBasket
		{
			get {  return _repeatSerachType == RepeatSerachType.SearchRepeats ? _repeatsBasket : _hairPinBasket; }
		}

		public IEnumerable<iDnaSequence> CurrentRepeatsBasketSorted
		{
			get { return _repeatSerachType == RepeatSerachType.SearchRepeats ? RepeatsBasketSorted : HairpinBasketSorted; }
		}


		public IEnumerable<iDnaSequence> HairpinBasketSorted
		{
			get
			{
				switch (HairpinSortOption)
				{
					case iDnaBasketSortOption.NoSort:
					default:
						return HairpinBasket;

					case iDnaBasketSortOption.SortByNumberOfOccurrences:
						return _hairPinBasket.OrderBy(i => i._nOccurrences).ToList();

					case iDnaBasketSortOption.SortByPosition:
						return _hairPinBasket.OrderBy(s => (s == null || s.Count <= 0) ? 0 : s[0].Index).ToList();
				}
			}
		}


		public List<iDnaSequence> HairpinBasket
		{
			get {  return _hairPinBasket; }
			protected set
			{
				if(value == _hairPinBasket)
					return;

				if(value == null)
				{
					_hairPinBasket.Clear();
					_pairStringOccurList.Clear();
				}
				else
					_hairPinBasket = value;

				NotifyPropertyChanged(() => HairpinBasket);
				NotifyPropertyChanged(() => HairpinBasketSorted);
				NotifyPropertyChanged(() => HairpinCount);
			}
		}



		IEnumerable<iDnaNode> AllStartoccurrencesOfString(string str)
		{
			if(string.IsNullOrEmpty(str))
				return null;

			int		len	= str.Length;

			return this.Where(i => StringAtIndex(i.Index, len) == str);
		}


		public string StringAtIndex(int index, int len)
		{
			if(index < 0 || len <= 0)
				return "";

			string	str			= "";
			var		nodes		= this.SkipWhile(i => i.Index < index).Take(len);

			if(nodes == null || nodes.Count() <= 0)
				return str;

			foreach(var n in nodes)
				str	+= n.Code.ToString();

			return str;
		}


		public async Task<int> GetRepeatsOrHairpins(Dispatcher dispatcher, bool searchHairpins)
		{
			if(dispatcher == null)
				dispatcher	= Dispatcher.CurrentDispatcher;

			List<iDnaSequence>	targetBasket	= searchHairpins ? _hairPinBasket : _repeatsBasket;

			CurrentRepeatSearchType	= searchHairpins ? RepeatSerachType.SerachHiarpins : RepeatSerachType.SearchRepeats;

			/// 15-03-2020: do not clear the basket: the user will do this when he or she needs to
			// targetBasket.Clear();
			/// idem for search occurrences
			// _stringOccurList.Clear();

			RepeatSortOption		= HairpinSortOption	= iDnaBasketSortOption.NoSort;

			iDnaRepeatSettings		repeatSettings		= iDnaRepeatSettings.Instance;
			iDnaHairpinSettings		hairSettings		= iDnaHairpinSettings.Instance;
			iDnaMinMaxValues		minMax				= (searchHairpins) ? hairSettings.MinMaxValues : repeatSettings.MinMaxValues;
			int						startIndex			= minMax.StartSearchRegionIndex,
									endRegionIndex		= minMax.EndSearchRegionIndex,
									endIndexMin			= minMax.MinNodes,
									endIndexMax			= minMax.MaxNodes;
			double					TmMin				= (double)minMax.MinMeltingTm,
									TmMax				= (double) minMax.MaxMeltingTm;
			bool					foundRepeats		= false,
									findOverlapping		= repeatSettings.SearchOverlapping,
									showPosition		= repeatSettings.ShowSearchPosition;
			int						basketLastCount		= 0;
			int						lenSeqString,
									lenSearch			= 0;
			string					sequenceString,
									searchString;
			StringOccurrenceList	occurList			= searchHairpins ? _pairStringOccurList : _stringOccurList;

			IsRepeatProcessRunning	= true;
			RepeatSearchPosition	= 0;

			if(searchHairpins)
			{
				NotifyPropertyChanged(() => HairpinBasket);
				NotifyPropertyChanged(() => HairpinBasketSorted);
				NotifyPropertyChanged(() => HairpinCount);
			}
			else
			{
				NotifyPropertyChanged(() => RepeatsBasket);
				NotifyPropertyChanged(() => RepeatsBasketSorted);
				NotifyPropertyChanged(() => RepeatsCount);
			}

			if (_repeatCancelSource != null)
			{
				_repeatCancelSource.Cancel();
				_repeatCancelSource.Dispose();
			}

			_repeatCancelSource = new CancellationTokenSource();
			
			await Task.Run( () =>
			{
				int		maxEndRegion			= Math.Min(endRegionIndex, this.Count);
				bool	nodeCurrentSelection	= false;

				while ( (startIndex + minMax.MinNodes) < maxEndRegion)
				{
					if(_repeatCancelSource.IsCancellationRequested)
						break;

					RepeatSearchPosition	= startIndex;
					endIndexMin				= startIndex	+ minMax.MinNodes;

					if(startIndex + minMax.MaxNodes >= maxEndRegion)
						endIndexMax		= maxEndRegion - 1;
					else
						endIndexMax		= startIndex	+ minMax.MaxNodes;

					foundRepeats	= false;

					// keep track of current node selection
					nodeCurrentSelection	= this[startIndex].IsSelected;

					if (showPosition)
					{
						this[startIndex].IsSelected = !nodeCurrentSelection;	/// invert selection
						Thread.Sleep(15);
					}

					string		strAtStart	= this.StringAtIndex(startIndex, minMax.MaxNodes);
					int			lenStrMax	= strAtStart.Length;

					if(lenStrMax < minMax.MinNodes)
						goto next_location;

					sequenceString				= strAtStart;			// seqMax.SequenceString;
					lenSeqString				= lenStrMax;			// seqMax.SequenceString.Length;
					lenSearch					= lenSeqString;
					string		shortestString	= strAtStart.Substring(0, minMax.MinNodes);

					/// we already searched for this as shortest string: skip this location
					if (occurList.Count > 0 && occurList.Any(s => s.ItemString == shortestString))
					{
						lenSearch		= minMax.MinNodes;
						foundRepeats	= true;
						goto next_location;
					}

					for(int len = lenStrMax; len > minMax.MinNodes && len > 0; len --)
					{
						string			str		= strAtStart.Substring(0, len);

						/// did we already searched fro this string?: skip
						if (occurList.Count > 0 && occurList.Any(s => s.ItemString == str))
						{
							lenSearch		= len;
							foundRepeats	= true;
							goto next_location;
						}
					}

					int				trimEnd		= 0;

					while ( lenSearch - trimEnd >= minMax.MinNodes)
					{
						lenSearch		= lenSeqString - trimEnd;
						searchString	= sequenceString.Substring(0, lenSearch);

						/// we already searched for this: skip
						if (occurList.Count >0 && occurList.Any(s => s.ItemString == searchString))
							goto next_index;

						/// add this to the search list
						occurList.AddUnique(new StringOccurrence(searchString, 0));

						/// string alreay in repeat library? : skip
						if (targetBasket.Any(s => s.SequenceFlatString == searchString))
							goto next_index;

						/// for hairpin: get the pair string
						
						var		allStartOccurrences	= this.AllStartoccurrencesOfString( searchHairpins 
																						? iDnaBaseNucleotides.Instance.GetPairString(searchString) 
																						: searchString);
						int		occurrences			= allStartOccurrences == null ? 0 : allStartOccurrences.Count();

						/// keep track of occurrences
						occurList[searchString].Occurrs	= occurrences;

						if (allStartOccurrences != null && occurrences > (searchHairpins ? 0 : 1))
						{
							foundRepeats	= true;
							int				occurrenceIndex		= 1;
							string			repeatName			= (searchHairpins ? "H" : "R") 
																+ ((searchHairpins ? HairpinCount : RepeatsCount) +1).ToString(),
											sequenceName		= "";
							
							/// for hairpins: explicitly add the current search string. otherwise teh string is in the occurrences
							if(searchHairpins)
							{
								var		repeatSeq	= this.SkipWhile( n => n.Index < startIndex).Take(lenSearch);
								sequenceName		= repeatName + " Origin: oc=" + occurrences.ToString();
								targetBasket.Add( new iDnaSequence(sequenceName, repeatSeq, refOnly: true, nOccurrences: occurrences));
							}

							foreach (var item in allStartOccurrences)
							{
								/// Console.WriteLine(item.Index);
								var		repeatSeq	= this.SkipWhile( n => n.Index < item.Index).Take(lenSearch);
								sequenceName		= repeatName + " oc:" + occurrenceIndex.ToString() + "/" + occurrences.ToString();
								targetBasket.Add( new iDnaSequence(sequenceName, repeatSeq, refOnly: true, nOccurrences: occurrences));

								if(searchHairpins)
								{
									NotifyPropertyChanged(() => HairpinCount);
								}
								else
								{
									NotifyPropertyChanged(() => RepeatsCount);
								}

								occurrenceIndex++;
							}
							break;
						}

next_index:
						endIndexMax--;
						trimEnd++;

						if (_repeatCancelSource.IsCancellationRequested)
							break;
					}

next_location:

					if (showPosition || repeatSettings.ShowSearchPosition)
					{
						this[startIndex].IsSelected	= nodeCurrentSelection;
					}

					if(findOverlapping || ! foundRepeats)
						startIndex++;
					else
					{
						startIndex	+= Math.Max(lenSearch, 1);
					}

					if(repeatSettings.ShowSearchPosition)
						GoToNodePage(this[startIndex]);

					if(targetBasket.Count > 0 && targetBasket.Count != basketLastCount)
					{
						//NotifyPropertyChanged(() => CurrentRepeatsBasket);
						//NotifyPropertyChanged(() => CurrentRepeatsBasketSorted);

						if (searchHairpins)
						{
							NotifyPropertyChanged(() => HairpinBasket);
							NotifyPropertyChanged(() => HairpinBasketSorted);
							NotifyPropertyChanged(() => HairpinCount);
						}
						else
						{
							NotifyPropertyChanged(() => RepeatsBasket);
							NotifyPropertyChanged(() => RepeatsBasketSorted);
							NotifyPropertyChanged(() => RepeatsCount);
						}
					}

					basketLastCount	= targetBasket.Count;
				}
			}, _repeatCancelSource.Token);

			IsRepeatProcessRunning	= false;
			if(searchHairpins)
			{
				NotifyPropertyChanged(() => HairpinBasket);
				NotifyPropertyChanged(() => HairpinBasketSorted);
				NotifyPropertyChanged(() => HairpinCount);
			}
			else
			{
				NotifyPropertyChanged(() => RepeatsBasket);
				NotifyPropertyChanged(() => RepeatsBasketSorted);
				NotifyPropertyChanged(() => RepeatsCount);
			}

			return targetBasket.Count;
		}


		private void _repeatSortOptions_SelectedOptionChanged(vm.iDnaSequenceSortOption selectedItem)
		{
			NotifyPropertyChanged(() => RepeatSortOption);
			Dispatcher.CurrentDispatcher.InvokeAsync(() => NotifyPropertyChanged(() => RepeatsBasketSorted));
		}

		private void _hairpinSortOptions_SelectedOptionChanged(vm.iDnaSequenceSortOption selectedItem)
		{
			NotifyPropertyChanged(() => HairpinSortOption);
			Dispatcher.CurrentDispatcher.InvokeAsync(() => NotifyPropertyChanged(() => HairpinBasketSorted));
		}

	}
}
