using isosoft.root;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iDna
{
	public class iDnaMinMaxValues : RootObject
	{
		const int						_constMinIntNodes		= 6,
										_constMaxIntNodes		= 32;
		protected iMinMaxInt			_nodesMinMax			= new iMinMaxInt(limitMin: _constMinIntNodes,limitMax: _constMaxIntNodes, min: _constMinIntNodes, max: 16);
		protected iMinMaxInt			_searchRegionMinMax		= new iMinMaxInt(limitMin: 0, limitMax: int.MaxValue, min: 0, max: int.MaxValue);

		protected decimal				_minTm			= new decimal(0.0),
										_maxTm			= new decimal(59.80);

		protected List<int>				_minMaxNodesSelectionList	= null;
		protected static List<decimal>	_minMaxTmSelectionList;

		protected bool					_allowRegionOverlap			= true;

		protected iDnaMinMaxValues() : base()
		{
			init_instance();
		}

		public iDnaMinMaxValues(int minNodes, int maxNodes, decimal minTm, decimal maxTm, bool allowSearchOverlap) : base()
		{
			_nodesMinMax.MinValue	= minNodes;
			_nodesMinMax.MaxValue	= maxNodes;

			_minTm				= minTm;
			_maxTm				= maxTm;
			_allowRegionOverlap	= allowSearchOverlap;

			init_instance();
		}


		void init_instance()
		{
			if(_minMaxNodesSelectionList == null)
				_minMaxNodesSelectionList	= initMinMaxNodeSelectionList;

			if(_minMaxTmSelectionList == null)
				_minMaxTmSelectionList		= initMinMaxMeltingTmSelectionList;
		}

		public int MinNodes
		{
			get { return _nodesMinMax.MinValue; }
			set
			{
				if (value == _nodesMinMax.MinValue || value <= 0)
					return;

				_nodesMinMax.MinValue	= value;
				RaisePropertyChanged();
			}
		}

		public int MaxNodes
		{
			get { return _nodesMinMax.MaxValue; }
			set
			{
				if (value == _nodesMinMax.MaxValue || value <= 0)
					return;

				_nodesMinMax.MaxValue	= value;
				RaisePropertyChanged();
			}
		}


		public decimal MinMeltingTm
		{
			get { return _minTm; }
			set
			{
				if(value == _minTm || value < (decimal) 0.0)
					return;

				_minTm		= value;
				AdjustMinMaxTm();
				RaisePropertyChanged();
			}
		}

		public decimal MaxMeltingTm
		{
			get { return _maxTm; }
			set
			{
				if(value == _maxTm || value <= (decimal) 0.0)
					return;
				_maxTm	= value;
				AdjustMinMaxTm();
				RaisePropertyChanged();
			}
		}


		void AdjustMinMaxTm()
		{
			decimal		tmp		= _minTm;

			if(_minTm > _maxTm)
			{
				_minTm		= _maxTm;
				_minTm		= tmp;
			}
		}

		public int MaxTmIndex
		{
			get {  return _minMaxTmSelectionList.IndexOf(_maxTm); }
			set
			{
				if(value < 0 || value >= _minMaxTmSelectionList.Count)
					return;

				MaxMeltingTm	= _minMaxTmSelectionList[value];
			}
		}

		public bool AllowSearchRegionsOverlap
		{
			get { return _allowRegionOverlap; }
			set
			{
				_allowRegionOverlap = value;
				RaisePropertyChanged();
			}
		}

		public int StartSearchRegionIndex
		{
			get { return _searchRegionMinMax.MinValue; }
			set
			{
				if(value == _searchRegionMinMax.MinValue || value < 0)
					return;
				_searchRegionMinMax.MinValue	= value;
				RaisePropertyChanged();
			}
		}


		public int EndSearchRegionIndex
		{
			get { return _searchRegionMinMax.MaxValue; }
			set
			{
				if(value == _searchRegionMinMax.MaxValue || value < 0)
					return;
				_searchRegionMinMax.MaxValue = value;
				RaisePropertyChanged();
			}
		}


		public List<int> MinMaxNodeSelectionList
		{
			get {  return _minMaxNodesSelectionList; }
		}

		public List<decimal> MinMaxMeltingTmSelectionList
		{
			get { return _minMaxTmSelectionList; }
		}

		static List<decimal> initMinMaxMeltingTmSelectionList
		{
			get
			{
				List<decimal>		list	= new List<decimal>();
				decimal				min		= new decimal(0.0),
									max		= new decimal(70.0),
									incr	= new decimal(0.10);

				for(decimal i = min; i <= max; i += incr)
					list.Add(i);

				return list;
			}
		}

		static List<int> initMinMaxNodeSelectionList
		{
			get
			{
				List<int>			list	= new List<int>();

				for(int i = _constMinIntNodes; i <= _constMaxIntNodes; i ++)
					list.Add(i);

				return list;
			}
		}

	}

	public class iDnaRepeatSettings : RootObject
	{
		static iDnaRepeatSettings		_instance			= null;

		protected iDnaMinMaxValues		_minMaxValues		= new iDnaMinMaxValues(minNodes: 6, maxNodes: 12, minTm: (decimal) 0.0,maxTm: (decimal) 55.50, allowSearchOverlap: false);
		protected bool					_searchOverlapping	= false,
										_showSearchPosition	= true;

		public static iDnaRepeatSettings Instance
		{
			get
			{
				if(_instance == null)
					_instance = new iDnaRepeatSettings();
				return _instance;
			}
		}

		protected iDnaRepeatSettings() : base()
		{
			init_instance();
		}

		void init_instance()
		{

		}

		public iDnaMinMaxValues MinMaxValues
		{
			get { return _minMaxValues; }
		}

		public bool SearchOverlapping
		{
			get { return _searchOverlapping; }
			set
			{
				if(value == _searchOverlapping)
					return;

				_searchOverlapping = value;
				RaisePropertyChanged();
			}
		}

		public bool ShowSearchPosition
		{
			get { return _showSearchPosition; }
			set
			{
				if(value == _showSearchPosition)
					return;

				_showSearchPosition = value;
				RaisePropertyChanged();
			}
		}

	}



	public class iDnaHairpinSettings : RootObject
	{
		static iDnaHairpinSettings		_instance		= null;

		protected iDnaMinMaxValues		_minMaxValues		= new iDnaMinMaxValues(minNodes: 12, maxNodes: 16, minTm: (decimal) 0.0, maxTm: (decimal) 55.50, allowSearchOverlap: false);

		public static iDnaHairpinSettings Instance
		{
			get
			{
				if(_instance == null)
					_instance = new iDnaHairpinSettings();
				return _instance;
			}
		}

		protected iDnaHairpinSettings() : base()
		{
			init_instance();
		}

		void init_instance()
		{

		}

		public iDnaMinMaxValues MinMaxValues
		{
			get { return _minMaxValues; }
		}

	}



	public class iDnaPrimerSettings : RootObject
	{
		static iDnaPrimerSettings		_instance		= null;

		protected iDnaMinMaxValues		_minMaxValues35	= new iDnaMinMaxValues(minNodes: 12, maxNodes: 16, minTm: (decimal) 0.0, maxTm: (decimal) 55.50, allowSearchOverlap: false);
		protected iDnaMinMaxValues		_minMaxValues53	= new iDnaMinMaxValues(minNodes: 12, maxNodes: 16, minTm: (decimal) 0.0, maxTm: (decimal) 55.50, allowSearchOverlap: false);

		public static iDnaPrimerSettings Instance
		{
			get
			{
				if(_instance == null)
					_instance = new iDnaPrimerSettings();
				return _instance;
			}
		}

		protected iDnaPrimerSettings() : base()
		{
			init_instance();
		}

		void init_instance()
		{

		}

		public iDnaMinMaxValues MinMaxValues
		{
			get { return _minMaxValues35; }
		}

	}


	public class iDnaGobalSettings : RootObject
	{
		static iDnaGobalSettings		_instance = null;

		protected iDnaRepeatSettings	_repeatsSettings		= iDnaRepeatSettings.Instance;
		protected iDnaHairpinSettings	_hairpinSettings		= iDnaHairpinSettings.Instance;
		protected iDnaPrimerSettings	_primersSettings		= iDnaPrimerSettings.Instance;

		public static iDnaGobalSettings Instance
		{
			get
			{
				if(_instance == null)
					_instance = new iDnaGobalSettings();

				return _instance;
			}
		}

		public bool SetSearchRegion(int minIndex, int maxIndex)
		{
			_repeatsSettings.MinMaxValues.StartSearchRegionIndex		=
				_hairpinSettings.MinMaxValues.StartSearchRegionIndex	=
				_primersSettings.MinMaxValues.StartSearchRegionIndex	= minIndex;

			_repeatsSettings.MinMaxValues.EndSearchRegionIndex		=
				_hairpinSettings.MinMaxValues.EndSearchRegionIndex	=
				_primersSettings.MinMaxValues.EndSearchRegionIndex	= maxIndex;

			return true;
		}

		public static iDnaGobalSettings GetInstance()
		{
			return Instance;
		}

		protected iDnaGobalSettings() : base()
		{
			init_instance();
		}

		void init_instance()
		{

		}

		public iDnaRepeatSettings RepeatsSettings
		{
			get { return _repeatsSettings; }
		}

		public iDnaHairpinSettings HairpinSettings
		{
			get { return _hairpinSettings; }
		}


		public iDnaPrimerSettings PrimersSettings
		{
			get { return _primersSettings; }
		}

	}



	public class iDnaMinMaxValuesDesignTime : iDnaMinMaxValues
	{
		public iDnaMinMaxValuesDesignTime() : base()
		{
			_nodesMinMax.MinValue	= 6;
			_nodesMinMax.MaxValue	= 32;
			_minTm		= (decimal) 0.0;
			_maxTm		= (decimal) 59.80;
			NotifyPropertyChanged(() => MinMaxNodeSelectionList);
		}
	}

	public class iDnaRepeatSettingsDesignTime : iDnaRepeatSettings
	{
		public iDnaRepeatSettingsDesignTime() : base()
		{

		}
	}

	public class iDnaHairpinSettingsDesignTime : iDnaHairpinSettings
	{
		public iDnaHairpinSettingsDesignTime() : base()
		{

		}
	}
}
