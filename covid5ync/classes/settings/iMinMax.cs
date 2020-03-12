using isosoft.root;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iDna
{
	public class iMinMaxInt : RootObject
	{
		protected int					_limitMin					= 0,
										_limitMax					= 255;

		protected int					_minValue					= 0,
										_maxValue					= int.MaxValue;

		public iMinMaxInt(int limitMin, int limitMax, int min, int max) : base()
		{
			_limitMin	= limitMin;
			_limitMax	= limitMax;
			init_instance();

			MinValue	= min;
			MaxValue	= max;
		}

		public iMinMaxInt()
		{
			init_instance();
		}

		void init_instance()
		{
		}

		public bool IsInError
		{
			get
			{
				return _minValue > _maxValue 
					
					|| _maxValue > _limitMax 
					|| _minValue > _limitMax

					|| _maxValue < _limitMin
					|| _minValue < _limitMin;
			}
		}

		public int MinValue
		{
			get { return _minValue; }
			set
			{
				if(value == _minValue || value <= 0)
					return;
				_minValue	= value;
				AdjustMinMaxValues();
				RaisePropertyChanged();
				NotifyPropertyChanged(() => IsInError);
			}
		}

		public int MaxValue
		{
			get { return _maxValue; }
			set
			{
				if(value == _maxValue || value <= 0)
					return;

				_maxValue	= value;
				AdjustMinMaxValues();
				RaisePropertyChanged();
				NotifyPropertyChanged(() => IsInError);
			}
		}

		public int LimitMin
		{
			get { return _limitMin; }
			set
			{
				if(value == _limitMin)
					return;

				_limitMin = value;
				AdjustMinMaxLimitValues();
				RaisePropertyChanged();
				NotifyPropertyChanged(() => IsInError);
			}
		}

		public int LimitMax
		{
			get { return _limitMax; }
			set
			{
				if(value == _limitMax)
					return;

				_limitMax = value;
				AdjustMinMaxLimitValues();
				RaisePropertyChanged();
				NotifyPropertyChanged(() => IsInError);
			}
		}


		public void AdjustMinMaxLimitValues()
		{
			if(_limitMin < _limitMax)
			{
				int		tmp	= _limitMin;

				_limitMin	= _limitMax;
				_limitMax	= tmp;
			}
		}

	
		void AdjustMinMaxValues()
		{
			int		tmp		= _minValue;

			if(_minValue > _maxValue)
			{
				_minValue	= _maxValue;
				_minValue	= tmp;
			}

			bool	limitExceeded	= (_minValue < _limitMin) || (_maxValue > _limitMax);

			if (_minValue < _limitMin)
				_minValue		= LimitMin;

			if(_maxValue > _limitMax)
				_maxValue	= LimitMax;
		}

	}

	public class iDnaRegionIndex : iMinMaxInt
	{
		protected string				_name		= "new region",
										_description;


		public iDnaRegionIndex() : base()
		{
		}

		public iDnaRegionIndex(string name, int startIndex, int endIndex) : base(0, int.MaxValue, startIndex, endIndex)
		{
			_name		= name;
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

		public iDnaRegionIndexList() : base()
		{

		}

		public iDnaRegionIndexList(iDnaSequence sequence, string regionName)
		{
			_parentSequence		= sequence;
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
