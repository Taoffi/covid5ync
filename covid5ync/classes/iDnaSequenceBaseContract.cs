using isosoft.root;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace iDna
{

	[DataContract(Namespace ="")]
	public class iDnaSequenceContract : RootObject, IDisposable
	{
		protected iDnaSequence			_sequence		= new iDnaSequence();
		protected iDnaRegionIndexList	_deserializedHairPinIndexes,
										_deserializedRepeatIndexes;

		public iDnaSequenceContract() : base()
		{
			_sequence	= iDnaSequence.Instance;
		}

		public iDnaSequenceContract(iDnaSequence sequence) : base()
		{
			_sequence	= sequence;
		}

		void InitialzeRootSequence()
		{
			_sequence		= iDnaSequence.Instance;
			_sequence.SequenceParseCompleted += _sequence_SequenceParseCompleted;
		}

		private void _sequence_SequenceParseCompleted(iDnaSequence sender, int nodeCount)
		{
			if(_sequence == null)
				return;

			// assign repeats and hairpins only after parse string process is complete
			_sequence.RepeatsRegionIndex	= _deserializedRepeatIndexes;
			_sequence.HairpinsRegionIndex	= _deserializedRepeatIndexes;
			_sequence.NotifySequenceLoaded();
		}

		public void Dispose()
		{
			if(_sequence != null)
				_sequence.SequenceParseCompleted -= _sequence_SequenceParseCompleted;
		}

		[DataMember(Order =1)]
		public string Id
		{
			get {  return _sequence == null ? "" : _sequence.Id; }
			protected set
			{
				if(_sequence == null)
					InitialzeRootSequence();

				if (value == _sequence.Id)
					return;

				_sequence.Id = value;
				RaisePropertyChanged();
			}
		}


		[DataMember(Order = 2)]
		public string Name
		{
			get { return _sequence == null ? "" : _sequence.Name; }
			set
			{
				if (_sequence == null)
					InitialzeRootSequence();

				if (value == _sequence.Name)
					return;

				_sequence.Name		= value;
				RaisePropertyChanged();
			}
		}

		[DataMember(Order = 3)]
		public int Occurrences
		{
			get { return _sequence == null ? 0 : _sequence.Occurrences; }
			set
			{
				if (_sequence == null)
					InitialzeRootSequence();

				if (value == _sequence.Occurrences)
					return;

				_sequence.Occurrences	= value;
				RaisePropertyChanged();
			}
		}


		[DataMember(Order = 4)]
		public string SequenceFileInfo
		{
			get { return _sequence == null ? "" : _sequence.SequenceFileInfo; }
			set
			{
				if (_sequence == null)
					InitialzeRootSequence();

				if (value == _sequence.SequenceFileInfo)
					return;

				_sequence.SequenceFileInfo	= value;
				RaisePropertyChanged();
			}
		}

		[DataMember(Order = 5)]
		public string SequenceFormattedString
		{
			get { return _sequence == null ? null : _sequence.SequenceFormattedString; }
			set
			{
				if (_sequence == null)
					InitialzeRootSequence();

				_sequence.SequenceFormattedString = value;
				RaisePropertyChanged();
			}
		}

		[DataMember(Order = 6)]
		public iDnaRegionIndexList SequenceNamedRegionList
		{
			get { return _sequence == null ? null : _sequence.SequenceNamedRegionList; }
			set
			{
				if (_sequence == null)
					InitialzeRootSequence();

				_sequence.SequenceNamedRegionList = value;
				RaisePropertyChanged();
			}
		}


		/// <summary>
		/// serialization: transform the basket into region indexes
		/// </summary>
		[DataMember(Order = 7)]
		public iDnaRegionIndexList RepeatsRegionIndex
		{
			get { return _sequence == null ? null : _sequence.RepeatsRegionIndex;  }
			set
			{
				_deserializedRepeatIndexes = value;		// keep this here until parse string completes
				RaisePropertyChanged();
			}
		}

		/// <summary>
		/// serialization: transform the basket into region indexes
		/// </summary>
		[DataMember(Order = 8)]
		public iDnaRegionIndexList HairpinsRegionIndex
		{
			get { return _sequence == null ? null : _sequence.HairpinsRegionIndex; }
			set
			{
				_deserializedHairPinIndexes	= value;    // keep this here until parse string completes
				RaisePropertyChanged();
			}
		}

	}



	//[CollectionDataContract(Namespace ="", IsReference = true)]
	public partial class iDnaSequence
	{

		public string Id
		{
			get {  return _id; }
			internal set
			{
				if(value == _id)
					return;

				_id = value;
				NotifyPropertyChanged(() => Id);
			}
		}


		public string Name
		{
			get { return _name; }
			set
			{
				if(value == _name)
					return;

				_name		= value;
				NotifyPropertyChanged(() => Name);
			}
		}

		public int Occurrences
		{
			get { return _nOccurrences; }
			set
			{
				if(value == _nOccurrences)
					return;

				_nOccurrences	= value;
				NotifyPropertyChanged(() => Occurrences);
			}
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


		public iDnaRegionIndexList SequenceNamedRegionList
		{
			get {  return _namedRegionsList; }
			set
			{
				if(value == null)
					_namedRegionsList.Clear();
				else
				{
					value.SetParentSequenceNoCheck(this);
					_namedRegionsList		= value;
				}
				NotifyPropertyChanged(() => SequenceNamedRegionList);
			}
		}


		/// <summary>
		/// serialization: transform the basket into region indexes
		/// </summary>
		public iDnaRegionIndexList RepeatsRegionIndex
		{
			get { return SequenceListToRegionList(_repeatsBasket);  }
			set
			{
				if(value == null)
					RepeatsBasket.Clear();
				else
				{
					value.SetParentSequenceNoCheck(this);
					RepeatsBasket	= RegionListToSequenceList(this, value);
				}

				NotifyPropertyChanged(() => RepeatsBasket);
				NotifyPropertyChanged(() => RepeatsBasketSorted);
			}
		}

		/// <summary>
		/// serialization: transform the basket into region indexes
		/// </summary>
		public iDnaRegionIndexList HairpinsRegionIndex
		{
			get { return SequenceListToRegionList(_hairPinBasket); }
			set
			{
				if(value == null)
					_hairPinBasket.Clear();
				else
				{
					value.SetParentSequenceNoCheck(this);
					HairpinBasket		= RegionListToSequenceList(this, value);
				}

				NotifyPropertyChanged(() => HairpinBasket);
				NotifyPropertyChanged(() => HairpinBasketSorted);
			}
		}

		public string SequenceFormattedString
		{
			get { return FormattedNodeSttring(this); }
			set
			{
				if(string.IsNullOrEmpty(value))
				{
					this.Clear();
					return;
				}
				
				Dispatcher.CurrentDispatcher.InvokeAsync((Action)(async() =>
				{
					await ParseString(value);
				})).Wait();
			}
		}

		internal void NotifySequenceLoaded()
		{
			Dispatcher.CurrentDispatcher.Invoke(() =>
			{
				NotifyPropertyChanged(() => SequenceFileInfo);
				NotifyPropertyChanged(() => SequencePaging);
				NotifyPropertyChanged(() => SequenceNamedRegionList);
				NotifyPropertyChanged(() => RepeatsBasket);
				NotifyPropertyChanged(() => HairpinBasket);
				NotifyPropertyChanged(() => RepeatsCount);
				NotifyPropertyChanged(() => HairpinCount);
			});
		}
	}
}
