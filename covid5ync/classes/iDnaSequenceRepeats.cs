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
<<<<<<< HEAD
=======

		protected iDnaRegionIndexList			_repeatsRegionBasket	= new iDnaRegionIndexList(),
												_hairPinRegionBasket	= new iDnaRegionIndexList(),
												_repeatRegionSearch		= new iDnaRegionIndexList();

>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15
		protected int							_repeatSearchPosition	= 0;
		protected bool							_isRepeatProcessRunning	= false;
		protected CancellationTokenSource		_repeatCancelSource		= new CancellationTokenSource(5);
		protected RepeatSerachType				_repeatSerachType		= RepeatSerachType.SearchRepeats;

<<<<<<< HEAD

=======
		protected string						_flatStringCache		="";
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15

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

<<<<<<< HEAD
#endregion // xxxxxxxxxxxxxxxxxxxxxx occurrence classes xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

		protected StringOccurrenceList		_stringOccurList		= new StringOccurrenceList(),
											_pairStringOccurList	= new StringOccurrenceList();
=======
		#endregion // xxxxxxxxxxxxxxxxxxxxxx occurrence classes xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

		//protected StringOccurrenceList		_stringOccurList		= new StringOccurrenceList(),
		//									_pairStringOccurList	= new StringOccurrenceList();

		protected string			_stringOccurList		= "",
									_pairStringOccurList	= "";

>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15

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
<<<<<<< HEAD
				if(value == _isRepeatProcessRunning)
					return;

				_isRepeatProcessRunning		= value;
=======
				//if(value == _isRepeatProcessRunning)
				//	return;

				_isRepeatProcessRunning		= value;

>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15
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
<<<<<<< HEAD
					_stringOccurList.Clear();
=======
					//_stringOccurList.Clear();
					_stringOccurList	= "";
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15
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
<<<<<<< HEAD
					_pairStringOccurList.Clear();
=======
					//_pairStringOccurList.Clear();
					_pairStringOccurList	= "";
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15
				}
				else
					_hairPinBasket = value;

				NotifyPropertyChanged(() => HairpinBasket);
				NotifyPropertyChanged(() => HairpinBasketSorted);
				NotifyPropertyChanged(() => HairpinCount);
			}
		}

<<<<<<< HEAD
=======
		protected void CheckStringCache()
		{
			// 18-03-2020
			if (string.IsNullOrEmpty(_flatStringCache) || _flatStringCache.Length != this.Count)
				_flatStringCache = this.SequenceFlatString;
		}


		IEnumerable<iDnaNode> AllCacheStartoccurrencesOfString(string str)
		{
			if(string.IsNullOrEmpty(str) || this.Count <= 0)
				return null;

			CheckStringCache();

			List<int>		indexes			= new List<int>();
			int				index1stNode	= this[0].Index,
							index			= 0,
							len				= str.Length;
			int				foundIndex		= _flatStringCache.IndexOf(str, index, StringComparison.InvariantCulture);
			
			while(foundIndex >= 0)
			{
				indexes.Add(foundIndex + index1stNode);	// 1);
				index		+= len - 1;
				foundIndex	= _flatStringCache.IndexOf(str, index, StringComparison.InvariantCulture);
			}

			if(indexes.Count<= 0)
				return null;

			return this.Where(i =>indexes.IndexOf( i.Index) >= 0);
		}
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15


		IEnumerable<iDnaNode> AllStartoccurrencesOfString(string str)
		{
<<<<<<< HEAD
			if(string.IsNullOrEmpty(str))
=======
			if(string.IsNullOrEmpty(str) || this.Count <= 0)
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15
				return null;

			int		len	= str.Length;

			return this.Where(i => StringAtIndex(i.Index, len) == str);
		}


<<<<<<< HEAD
		public string StringAtIndex(int index, int len)
		{
			if(index < 0 || len <= 0)
=======
		public string CacheStringAtIndex(int index, int len)
		{
			if(index < 0 || len <= 0|| index >=_flatStringCache.Length)
				return "";

			int		_strLen		= _flatStringCache.Length;

			if ((index + len) >= _strLen)
				len = _strLen - index - 1;

			return _flatStringCache.Substring(index, len);
		}


		public string StringAtIndex(int index, int len)
		{
			if (index < 0 || len <= 0 || index >= this.Count)	// _flatStringCache.Length)
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15
				return "";

			string	str			= "";
			var		nodes		= this.SkipWhile(i => i.Index < index).Take(len);

			if(nodes == null || nodes.Count() <= 0)
				return str;

			foreach(var n in nodes)
				str	+= n.Code.ToString();

			return str;
		}


<<<<<<< HEAD
		public async Task<int> GetRepeatsOrHairpins(Dispatcher dispatcher, bool searchHairpins)
=======
		public async Task<int> FindRepeatsOrHairpins(Dispatcher dispatcher, bool searchHairpins)
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15
		{
			if(dispatcher == null)
				dispatcher	= Dispatcher.CurrentDispatcher;

<<<<<<< HEAD
			List<iDnaSequence>	targetBasket	= searchHairpins ? _hairPinBasket : _repeatsBasket;
=======
			List<iDnaSequence>	targetBasket		= searchHairpins ? _hairPinBasket		: _repeatsBasket;
			iDnaRegionIndexList	targetRegionList	= searchHairpins ? _hairPinRegionBasket	: _repeatsRegionBasket;
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15

			CurrentRepeatSearchType	= searchHairpins ? RepeatSerachType.SerachHiarpins : RepeatSerachType.SearchRepeats;

			/// 15-03-2020: do not clear the basket: the user will do this when he or she needs to
			// targetBasket.Clear();
			/// idem for search occurrences
			// _stringOccurList.Clear();

<<<<<<< HEAD
=======
			_flatStringCache		= SequenceFlatString;		// 200316

>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15
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
<<<<<<< HEAD
			StringOccurrenceList	occurList			= searchHairpins ? _pairStringOccurList : _stringOccurList;
=======
			//StringOccurrenceList	occurList			= searchHairpins ? _pairStringOccurList : _stringOccurList;
			string					occurList			= searchHairpins ? _pairStringOccurList : _stringOccurList;
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15

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

<<<<<<< HEAD
					string		strAtStart	= this.StringAtIndex(startIndex, minMax.MaxNodes);
=======
					string		strAtStart	= this.CacheStringAtIndex(startIndex, minMax.MaxNodes);
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15
					int			lenStrMax	= strAtStart.Length;

					if(lenStrMax < minMax.MinNodes)
						goto next_location;

					sequenceString				= strAtStart;			// seqMax.SequenceString;
					lenSeqString				= lenStrMax;			// seqMax.SequenceString.Length;
					lenSearch					= lenSeqString;
					string		shortestString	= strAtStart.Substring(0, minMax.MinNodes);

					/// we already searched for this as shortest string: skip this location
<<<<<<< HEAD
					if (occurList.Count > 0 && occurList.Any(s => s.ItemString == shortestString))
=======
					//if (occurList.Count > 0 && occurList.Any(s => s.ItemString == shortestString))
					if (occurList.Length > 0 && occurList.IndexOf("|" + shortestString) >= 0)
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15
					{
						lenSearch		= minMax.MinNodes;
						foundRepeats	= true;
						goto next_location;
					}

					for(int len = lenStrMax; len > minMax.MinNodes && len > 0; len --)
					{
						string			str		= strAtStart.Substring(0, len);

						/// did we already searched fro this string?: skip
<<<<<<< HEAD
						if (occurList.Count > 0 && occurList.Any(s => s.ItemString == str))
=======
						//if (occurList.Count > 0 && occurList.Any(s => s.ItemString == str))
						if (occurList.Length > 0 && occurList.IndexOf("|" + str) >= 0)
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15
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
<<<<<<< HEAD
						if (occurList.Count >0 && occurList.Any(s => s.ItemString == searchString))
							goto next_index;

						/// add this to the search list
						occurList.AddUnique(new StringOccurrence(searchString, 0));
=======
						//if (occurList.Count >0 && occurList.Any(s => s.ItemString == searchString))
						if (occurList.Length > 0 && occurList.IndexOf("|" + searchString)>= 0)
							goto next_index;

						/// add this to the search list
						//occurList.AddUnique(new StringOccurrence(searchString, 0));
						occurList	+= "|" + searchString;
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15

						/// string alreay in repeat library? : skip
						if (targetBasket.Any(s => s.SequenceFlatString == searchString))
							goto next_index;

						/// for hairpin: get the pair string
						
<<<<<<< HEAD
						var		allStartOccurrences	= this.AllStartoccurrencesOfString( searchHairpins 
=======
						var		allStartOccurrences	= this.AllCacheStartoccurrencesOfString( searchHairpins 
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15
																						? iDnaBaseNucleotides.Instance.GetPairString(searchString) 
																						: searchString);
						int		occurrences			= allStartOccurrences == null ? 0 : allStartOccurrences.Count();

						/// keep track of occurrences
<<<<<<< HEAD
						occurList[searchString].Occurrs	= occurrences;
=======
						///occurList[searchString].Occurrs	= occurrences;
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15

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
<<<<<<< HEAD
								var		repeatSeq	= this.SkipWhile( n => n.Index < startIndex).Take(lenSearch);
								sequenceName		= repeatName + " Origin: oc=" + occurrences.ToString();
=======
								var		repeatSeq	= this.SkipWhile( n => n.Index <= startIndex).Take(lenSearch);
								sequenceName		= repeatName + " Origin:[" + (startIndex +1).ToString() + " - " + (startIndex + lenSearch).ToString() + "] oc=" + occurrences.ToString();
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15
								targetBasket.Add( new iDnaSequence(sequenceName, repeatSeq, refOnly: true, nOccurrences: occurrences));
							}

							foreach (var item in allStartOccurrences)
							{
								/// Console.WriteLine(item.Index);
<<<<<<< HEAD
								var		repeatSeq	= this.SkipWhile( n => n.Index < item.Index).Take(lenSearch);
								sequenceName		= repeatName + " oc:" + occurrenceIndex.ToString() + "/" + occurrences.ToString();
=======
								sequenceName			= repeatName + " [" + item.Index.ToString() + " - " + (item.Index + lenSearch - 1).ToString() + "] [" + lenSearch.ToString() + "] oc:" + occurrenceIndex.ToString() + "/" + occurrences.ToString();
								
								// try using region instead of sequence
								iDnaRegionIndex	region		= new iDnaRegionIndex(sequenceName, item.Index, item.Index + lenSearch );
								var				repeatSeq	= this.SkipWhile( n => n.Index < item.Index).Take(lenSearch);
								
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15
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
<<<<<<< HEAD
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
=======

			}, _repeatCancelSource.Token);


			IsRepeatProcessRunning = false;


			await dispatcher.InvokeAsync(() =>
			{
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
			});
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15

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
