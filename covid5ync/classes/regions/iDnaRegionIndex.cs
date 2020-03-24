using isosoft.root;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iDna
{
	[DataContract(Namespace ="")]
	public class iDnaRegionIndex : iMinMaxInt
	{
		protected string				_name			= "new region",
										_description;
		protected int					_occurrences	= 1;

		public iDnaRegionIndex() : base()
		{
		}

		public iDnaRegionIndex(string name, int startIndex, int endIndex) : base(0, int.MaxValue, startIndex, endIndex)
		{
			_name		= name;
		}

		public iDnaRegionIndex(string name, int startIndex, int endIndex, string description) : base(0, int.MaxValue, startIndex, endIndex)
		{
			_name			= name;
			_description	= description;
		}

		public iDnaRegionIndex(iDnaSequence sequence, string regionName) : base()
		{
			_name		= regionName;

			if(sequence == null)
				return;

			int		min		= sequence.Min(i => i.Index),
					max		= sequence.Max(i => i.Index);

			_limitMin	= min;
			_limitMax	= max;

			_minValue	= min;
			_maxValue	= max;
		}

		[DataMember]
		public int Occurrences
		{
			get { return _occurrences; }
			set
			{
				if(value == _occurrences)
					return;

				_occurrences = value;
				RaisePropertyChanged();
			}
		}


		[DataMember]
		public string Name
		{
			get { return _name; }
			set
			{
				if(value == _name)
					return;

				_name = value;
				RaisePropertyChanged();
			}
		}

		[DataMember]
		public string Description
		{
			get { return _description; }
			set
			{
				if(value == _description)
					return;

				_description		= value;
				RaisePropertyChanged();
			}
		}

	}


	public class iDnaRegionIndexList: RootListTemplate<iDnaRegionIndex>
	{
		protected iDnaSequence		_parentSequence;
		protected bool				_allowOverlaps			= true;

		public iDnaRegionIndexList() : base()
		{

		}

		public iDnaRegionIndexList(iDnaSequence sequence, string regionName)
		{
			_parentSequence		= sequence;
		}


		/// <summary>
		/// for deserialzation !
		/// </summary>
		/// <param name="seq"></param>
		internal void SetParentSequenceNoCheck(iDnaSequence seq)
		{
			_parentSequence	= seq;
		}

		public iDnaSequence ParentSequence
		{
			get { return _parentSequence; }
			set
			{
				if(value == _parentSequence)
					return;

				_parentSequence = value;
				CheckCurrentRegions();
				NotifyPropertyChanged(() => ParentSequence);
			}
		}

		public bool AllowOverlaps
		{
			get { return _allowOverlaps; }
			set
			{
				if(value == _allowOverlaps)
					return;
				_allowOverlaps = value;
				NotifyPropertyChanged(() => AllowOverlaps);
			}
		}


		/// <summary>
		/// check if regions can be located on the parent sequence
		/// </summary>
		private void CheckCurrentRegions()
		{
			if(_parentSequence == null)
				return;

			foreach(var item in this)
			{
				item.MaxValue	= Math.Min(_parentSequence.Max(i => i.Index), item.MaxValue);
				item.MinValue	= Math.Max(_parentSequence.Min(i => i.Index), item.MinValue);
			}
		}


		public iDnaRegionIndex this[int min, int max]
		{
			get { return this.FirstOrDefault(i => i.MinValue == min && i.MaxValue == max); }
		}

		public bool Overlaps(int min, int max)
		{
			if(_allowOverlaps)
				return false;

			if(this.FirstOrDefault(i => i.MinValue >= min && i.MaxValue <= max) != null)
				return true;

			return false;
		}

		/// <summary>
		/// add only unique min/max values
		/// </summary>
		/// <param name="item"></param>
		public new void Add(iDnaRegionIndex item)
		{
			if(item == null)
				return;

			var		existent	= this[item.MinValue, item.MaxValue];
			if(existent == null && ! Overlaps(item.MinValue, item.MaxValue))
				base.Add(item);
		}

	}

	public class iDnaRegionIndexDesignTime : iDnaRegionIndex
	{
		public iDnaRegionIndexDesignTime()
		{
			_limitMax	= 500;
			_limitMin	= 0;
			
			MaxValue	= 200;
			MinValue	= 50;
			
		}
	}
}
