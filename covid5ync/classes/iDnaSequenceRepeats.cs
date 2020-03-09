using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;
using isosoft.root;
using System.ComponentModel;

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


	public partial class iDnaSequence
	{
		protected vm.iDnaSequenceSortOptionList	_repeatSortOptions		= new vm.iDnaSequenceSortOptionList();
		protected iDnaSequenceList				_repeatsBasket			= new iDnaSequenceList(),
												_repeatSearch			= new iDnaSequenceList();
		protected int							_reaptSearchPosition	= 0;
		protected bool							_isRepeatProcessRunning	= false;
		protected CancellationTokenSource		_repeatCancelSource		= new CancellationTokenSource(5);

		public CancellationTokenSource RepeatCancellation
		{
			get { return _repeatCancelSource; }
		}



		public vm.iDnaSequenceSortOptionList RepeatSortOptionList
		{
			get {  return _repeatSortOptions; }
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

		protected StringOccurrenceList		_stringOccurList		= new StringOccurrenceList();

		public int RepeatsCount
		{
			get
			{
				if(_repeatsBasket == null || _repeatsBasket.Count <= 0)
					return 0;

				return _repeatsBasket.Select(i => i.SequenceString).Distinct().Count();
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
			get { return _reaptSearchPosition; }
			protected set
			{
				if(value == _reaptSearchPosition)
					return;

				_reaptSearchPosition = value;
				NotifyPropertyChanged(() => RepeatSearchPosition);
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

		public iDnaSequenceList RepeatsBasket
		{
			get {  return _repeatsBasket; }
			protected set
			{
				if(value == _repeatsBasket)
					return;

				if(value == null)
					_repeatsBasket.Clear();
				else
					_repeatsBasket = value;

				NotifyPropertyChanged(() => RepeatsBasket);
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


		public async Task<int> GetRepeats(Dispatcher dispatcher)
		{
			if(dispatcher == null)
				dispatcher	= Dispatcher.CurrentDispatcher;

			_repeatsBasket.Clear();
			_stringOccurList.Clear();
			RepeatSortOption		= iDnaBasketSortOption.NoSort;

			iDnaRepeatSettings		settings			= iDnaRepeatSettings.Instance;
			iDnaMinMaxValues		minMax				= settings.MinMaxValues;
			int						startIndex			= minMax.StartRegionIndex,
									endRegionIndex		= minMax.EndRegionIndex,
									endIndexMin			= minMax.MinNodes,
									endIndexMax			= minMax.MaxNodes;
			//iDnaSequence			seqMax				= null;
			double					TmMin				= (double)minMax.MinMeltingTm,
									TmMax				= (double) minMax.MaxMeltingTm;
			bool					foundRepeats		= false,
									//nodeSeleted			= false,
									findOverlapping		= settings.SearchOverlapping,
									showPosition		= settings.ShowSearchPosition;
			int						basketLastCount		= 0;
			int						lenSeqString,
									lenSearch			= 0;
			string					sequenceString,
									searchString;

			IsRepeatProcessRunning	= true;
			RepeatSearchPosition	= 0;
			_repeatsBasket.Clear();
			NotifyPropertyChanged(() => RepeatsBasket);
			NotifyPropertyChanged(() => RepeatsBasketSorted);

			if (_repeatCancelSource != null)
			{
				_repeatCancelSource.Cancel();
				_repeatCancelSource.Dispose();
			}

			_repeatCancelSource = new CancellationTokenSource();
			
			await Task.Run( () =>
			{
				int		maxEndRegion		= Math.Min(endRegionIndex, this.Count);

				while ( (startIndex + minMax.MinNodes) < maxEndRegion)
				{
					if(_repeatCancelSource.IsCancellationRequested)
						break;

					RepeatSearchPosition	= startIndex;
					endIndexMin				= startIndex	+ minMax.MinNodes;

					if(startIndex + minMax.MaxNodes >= maxEndRegion)	//this.Count)
						endIndexMax		= maxEndRegion - 1;				// this.Count - 1;
					else
						endIndexMax		= startIndex	+ minMax.MaxNodes;

					foundRepeats	= false;

					if(showPosition)
					{
						this[startIndex].IsSelected = true;
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

					// we already searched for this as shortest string: skip this location
					if (_stringOccurList.Count > 0 && _stringOccurList.Any(s => s.ItemString == shortestString))
					{
						lenSearch		= minMax.MinNodes;
						foundRepeats	= true;
						goto next_location;
					}

					for(int len = lenStrMax; len > minMax.MinNodes && len > 0; len --)
					{
						string			str		= strAtStart.Substring(0, len);

						if (_stringOccurList.Count > 0 && _stringOccurList.Any(s => s.ItemString == str))		//if (_repeatsBasket.Any(s => s.SequenceString == str))
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

						// we already searched for this: skip
						if (_stringOccurList.Count >0 && _stringOccurList.Any(s => s.ItemString == searchString))
							goto next_index;

						// add this to the search list
						_stringOccurList.AddUnique(new StringOccurrence(searchString, 0));

						// string alreay in repeat library? : skip
						if (_repeatsBasket.Any(s => s.SequenceString == searchString))
							goto next_index;

						var		allStartOccurrences	= this.AllStartoccurrencesOfString(searchString);
						int		occurrences			= allStartOccurrences == null ? 0 : allStartOccurrences.Count();

						// keep track of occurrences
						_stringOccurList[searchString].Occurrs	= occurrences;

						if (allStartOccurrences != null && occurrences > 1)
						{
							foundRepeats	= true;
							int				occurrenceIndex		= 1;
							string			repeatName			= "R" + (RepeatsCount +1).ToString(),
											sequenceName		= "";

							foreach (var item in allStartOccurrences)
							{
								//Console.WriteLine(item.Index);
								var		repeatSeq	= this.SkipWhile( n => n.Index < item.Index).Take(lenSearch);
								sequenceName		= repeatName + " oc:" + occurrenceIndex.ToString() + "/" + occurrences.ToString();
								_repeatsBasket.Add( new iDnaSequence(sequenceName, repeatSeq, refOnly: true, nOccurrences: occurrences));
								NotifyPropertyChanged(() => RepeatsCount);
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

					if (showPosition || settings.ShowSearchPosition)
					{
						this[startIndex].IsSelected	= false;
					}

					if(findOverlapping || ! foundRepeats)
						startIndex++;
					else
					{
						startIndex	+= Math.Max(lenSearch, 1);
					}

					if(settings.ShowSearchPosition)
						GoToNodePage(this[startIndex]);

					if(_repeatsBasket.Count > 0 && _repeatsBasket.Count != basketLastCount)
					{
						NotifyPropertyChanged(() => RepeatsBasket);
						NotifyPropertyChanged(() => RepeatsBasketSorted);
					}

					basketLastCount	= _repeatsBasket.Count;
				}
			}, _repeatCancelSource.Token);

			IsRepeatProcessRunning	= false;
			NotifyPropertyChanged(() => RepeatsBasket);
			NotifyPropertyChanged(() => RepeatsBasketSorted);
			return _repeatsBasket.Count;
		}

		private void _repeatSortOptions_SelectedOptionChanged(vm.iDnaSequenceSortOption selectedItem)
		{
			NotifyPropertyChanged(() => RepeatSortOption);
			NotifyPropertyChanged(() => RepeatsBasketSorted);

		}
	}
}
