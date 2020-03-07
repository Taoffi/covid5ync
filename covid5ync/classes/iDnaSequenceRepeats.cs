using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;

namespace iDna
{
	public partial class iDnaSequence
	{
		protected iDnaSequenceList				_repeatsBasket			= new iDnaSequenceList(),
												_repeatSearch			= new iDnaSequenceList();
		protected int							_reaptSearchPosition	= 0;
		protected bool							_isRepeatProcessRunning	= false;


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

			iDnaRepeatSettings		settings			= iDnaRepeatSettings.Instance;
			iDnaMinMaxValues		minMax				= settings.MinMaxValues;
			int						startIndex			= 0,
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

			if (_cancelSource != null)
			{
				_cancelSource.Cancel();
				_cancelSource.Dispose();
			}
			
			_cancelSource	= new CancellationTokenSource();
			
			await Task.Run( () =>
			{
				while( (startIndex + minMax.MinNodes) < this.Count )
				{
					if(_cancelSource.IsCancellationRequested)
						break;

					RepeatSearchPosition	= startIndex;
					endIndexMin				= startIndex	+ minMax.MinNodes;

					if(startIndex + minMax.MaxNodes >= this.Count)
						endIndexMax		= this.Count - 1;
					else
						endIndexMax		= startIndex	+ minMax.MaxNodes;

					foundRepeats	= false;

					if(showPosition)
					{
						this[startIndex].IsSelected = true;
						Thread.Sleep(40);
					}

					string		strAtStart	= this.StringAtIndex(startIndex, minMax.MaxNodes);
					int			lenStrMax	= strAtStart.Length;

					for(int len = lenStrMax; len > minMax.MinNodes && len > 0; len --)
					{
						string			str		= strAtStart.Substring(0, len);
						if (_repeatsBasket.Any(s => s.SequenceString == str))
						{
							lenSearch		= len;
							foundRepeats	= true;
							goto next_location;
						}
					}

					//seqMax = new iDnaSequence("repeatMax", this.Where( 
					//									i => i.Index > startIndex && i.Index <= endIndexMax
					//									&& ! _repeatsBasket.Exists(s => s.Count > 0 && s[0].Index == startIndex + 1)		// is it already in our basket?
					//									), refOnly: true);

					//if(seqMax.Count <= 0)	// || seqMax._stats.SequenceMeltingTm > TmMax || seqMax._stats.SequenceMeltingTm < TmMin)
					//	goto next_location;

					sequenceString	= strAtStart;			// seqMax.SequenceString;
					lenSeqString	= lenStrMax;			// seqMax.SequenceString.Length;

					int				trimEnd		= 0;
					lenSearch	= lenSeqString;

					while ( lenSearch - trimEnd >= minMax.MinNodes)	// endIndexMax >= endIndexMin)
					{
						lenSearch		= lenSeqString - trimEnd;
						searchString	= sequenceString.Substring(0, lenSearch);

						// string alreay in repeat library? : skip
						if(_repeatsBasket.Any(s => s.SequenceString == searchString))
							goto next_index;

						var		allStartOccurrences	= this.AllStartoccurrencesOfString(searchString);

						if(allStartOccurrences != null && allStartOccurrences.Count() > 1)
						{
							foundRepeats	= true;

							foreach(var item in allStartOccurrences)
							{
								//Console.WriteLine(item.Index);
								var		repeatSeq	= this.SkipWhile( n => n.Index < item.Index).Take(lenSearch);
								_repeatsBasket.Add( new iDnaSequence("repeat " + (_repeatsBasket.Count + 1).ToString(), repeatSeq, refOnly: true));
							}
							break;
						}

						//if (settings.ShowSearchPosition)
						//{
						//	for(int ndx = startIndex; ndx < lenSearch && ndx < this.Count; ndx++)
						//	{
						//		this[ndx].IsSelected = true;
						//		Thread.Sleep(50);
						//	}
						//}

						//// find the longest sequence matches / add any existing
						//_repeatSearch.Clear();
						//await this.FindString( searchString, SequenceSearchType.SearchRepeats, dispatcher, updateSeletions: false);

						//if(_repeatSearch.Count > 1)
						//{
						//	foundRepeats	= true;

						//	foreach (var occur in _repeatSearch)
						//	{
						//		occur.Name	= "repeat " + (_repeatsBasket.Count +1).ToString();
						//		occur.ResetSelection(true, dispatcher);
						//		_repeatsBasket.Add(occur);
						//	}

						//	break;
						//}

next_index:
						endIndexMax--;
						trimEnd++;

						if (_cancelSource.IsCancellationRequested)
							break;

						//if (settings.ShowSearchPosition)
						//{
						//	for (int ndx = startIndex; ndx < lenSearch && ndx < this.Count; ndx++)
						//	{
						//		this[ndx].IsSelected = false;
						//		Thread.Sleep(50);
						//	}
						//}
					}

next_location:

					if (settings.ShowSearchPosition)
					{
						this[startIndex].IsSelected	= false;

						for (int ndx = startIndex; ndx < lenSearch && ndx < this.Count; ndx++)
						{
							this[ndx].IsSelected	= false;
							Thread.Sleep(50);
						}
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
						NotifyPropertyChanged(() => RepeatsBasket);

					basketLastCount	= _repeatsBasket.Count;
				}
			}, _cancelSource.Token);

			IsRepeatProcessRunning	= false;
			NotifyPropertyChanged(() => RepeatsBasket);
			return _repeatsBasket.Count;
		}

	}
}
