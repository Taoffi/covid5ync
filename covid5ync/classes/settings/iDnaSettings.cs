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
		const int					_constMinIntNodes		= 6,
									_constMaxIntNodes		= 32;
									
		protected int					_minNodes			= _constMinIntNodes,
										_maxNodes			= 16;
		
		protected int					_startRegionIndex	= 0,
										_endRegionIndex		= int.MaxValue;

		protected decimal				_minTm			= new decimal(0.0),
										_maxTm			= new decimal(59.80);

		protected static List<int>		_minMaxNodeSelectionList;
		protected static List<decimal>	_minMaxTmSelectionList;

		protected iDnaMinMaxValues() : base()
		{
			init_instance();
		}

		public iDnaMinMaxValues(int minNodes, int maxNodes, decimal minTm, decimal maxTm) : base()
		{
			_minNodes		= minNodes;
			_maxNodes		= maxNodes;
			_minTm			= minTm;
			_maxTm			= maxTm;
			init_instance();
		}

		void init_instance()
		{
			if(_minMaxNodeSelectionList == null)
				_minMaxNodeSelectionList	= InitMinMaxNodeSelectionList;
			if(_minMaxTmSelectionList == null)
				_minMaxTmSelectionList		= initMinMaxMeltingTmSelectionList;
		}

		public int MinNodes
		{
			get { return _minNodes; }
			set
			{
				if(value == _minNodes || value <= 0)
					return;
				_minNodes	= value;
				AdjustMinMaxNodes();
				RaisePropertyChanged();
			}
		}

		public int MaxNodes
		{
			get { return _maxNodes; }
			set
			{
				if(value == _maxNodes || value <= 0)
					return;
				_maxNodes	= value;
				AdjustMinMaxNodes();
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

		public int MinTmIndex
		{
			get {  return _minMaxTmSelectionList.IndexOf(_minTm); }
			set
			{
				if(value < 0 || value >= _minMaxNodeSelectionList.Count)
					return;

				MinMeltingTm	= _minMaxTmSelectionList[value];
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


		void AdjustMinMaxNodes()
		{
			int		tmp		= _minNodes;

			if(_minNodes > _maxNodes)
			{
				_minNodes	= _maxNodes;
				_minNodes	= tmp;
			}
		}


		public int StartRegionIndex
		{
			get { return _startRegionIndex; }
			set
			{
				if(value == _startRegionIndex || value < 0)
					return;
				_startRegionIndex	= value;
				AdjustStartEndRegions();
				RaisePropertyChanged();
			}
		}

		private void AdjustStartEndRegions()
		{
			if(_startRegionIndex > _endRegionIndex)
			{
				int		tmp	= _endRegionIndex;

				_endRegionIndex		= _startRegionIndex;
				_startRegionIndex	= tmp;
			}
		}

		public int EndRegionIndex
		{
			get { return _endRegionIndex; }
			set
			{
				if(value == _endRegionIndex || value < 0)
					return;
				_endRegionIndex = value;
				AdjustStartEndRegions();
				RaisePropertyChanged();
			}
		}


		public List<int> MinMaxNodeSelectionList
		{
			get {  return _minMaxNodeSelectionList; }
		}

		public List<decimal> MinMaxMeltingTmSelectionList
		{
			get { return _minMaxTmSelectionList; }
		}

		static List<int> InitMinMaxNodeSelectionList
		{
			get
			{
				List<int>		list	= new List<int>();
				for(int i = _constMinIntNodes; i < _constMaxIntNodes; i++)
					list.Add(i);

				return list;
			}
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

	}

	public class iDnaRepeatSettings : RootObject
	{
		static iDnaRepeatSettings		_instance			= null;

		protected iDnaMinMaxValues		_minMaxValues		= new iDnaMinMaxValues(6, 12, (decimal) 0.0, (decimal) 55.50);
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

		protected iDnaMinMaxValues		_minMaxValues		= new iDnaMinMaxValues(12, 16, (decimal) 0.0, (decimal) 55.50);

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

		protected iDnaMinMaxValues		_minMaxValues35	= new iDnaMinMaxValues(12, 16, (decimal) 0.0, (decimal) 55.50);
		protected iDnaMinMaxValues		_minMaxValues53	= new iDnaMinMaxValues(12, 16, (decimal) 0.0, (decimal) 55.50);

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
			_minNodes	= 6;
			_maxNodes	= 32;
			_minTm		= (decimal) 0.0;
			_maxTm		= (decimal) 59.80;
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
