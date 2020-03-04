using isosoft.root;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iDna
{
	public class iDnaBaseStatItem : RootObject
	{
		protected	iDnaBase	_base;
		protected	int			_count		= 0,
								_total		= 0;

		public iDnaBaseStatItem() : base()
		{

		}

		public iDnaBaseStatItem(iDnaBase rootBase, int count, int total)
		{
			_base	= rootBase;
			_count	= count;
		}

		public iDnaBase RootBase
		{
			get { return _base; }
			set
			{
				if(value == _base)
					return;
				_base = value;
				RaisePropertyChanged();
			}
		}

		public int Count
		{
			get { return _count; }
			set
			{
				if(value == _count)
					return;

				_count = value;
				RaisePropertyChanged();
				NotifyPropertyChanged(() => Percent);
			}
		}

		public int Total
		{
			get { return _total; }
			set
			{
				if(value == _total)
					return;

				_total = value;
				RaisePropertyChanged();
				NotifyPropertyChanged(() => Percent);
			}
		}


		public double Percent
		{
			get { return _total <= 0 ? 0.0 : ( (double)_count / (double) _total); }
		}

	}

	public class iDnaBaseStats : RootListTemplate<iDnaBaseStatItem>
	{
		protected int			_total		= 0;

		public iDnaBaseStats() : base()
		{
			init_instance();
		}

		public int Total
		{
			get { return _total; }
			set
			{
				if(value == _total)
					return;

				_total = value;
				NotifyPropertyChanged(() => Total);
				UpdateItems();
			}
		}

		internal void SetTotalNoNotify(int value)
		{
			if (value == _total)
				return;

			_total = value;
		}


		void UpdateItems()
		{
			foreach(var item in this)
				item.Total	= _total;

			NotifyPropertyChanged(() => PercentA);
			NotifyPropertyChanged(() => PercentT);
			NotifyPropertyChanged(() => PercentG);
			NotifyPropertyChanged(() => PercentC);
			NotifyPropertyChanged(() => SequenceMeltingTm);
		}



		int GetBaseCount(char rootBaseCode)
		{
			var item	= this[rootBaseCode];

			return item == null ? 0 : item.Count;
		}

		double GetBasePercent(char rootBaseCode)
		{
			var item	= this[rootBaseCode];

			return item == null ? 0.0 : item.Percent;
		}

		/// count bases
		public int CountA
		{
			get { return GetBaseCount('a'); }
		}

		public int CountT
		{
			get { return GetBaseCount('t'); }
		}

		public int CountG
		{
			get { return GetBaseCount('g'); }
		}

		public int CountC
		{
			get { return GetBaseCount('c'); }
		}


		/// percentages
		public double PercentA
		{
			get { return GetBasePercent('a'); }
		}

		public double PercentT
		{
			get { return GetBasePercent('t'); }
		}

		public double PercentG
		{
			get { return GetBasePercent('g'); }
		}

		public double PercentC
		{
			get { return GetBasePercent('c'); }
		}



		protected void init_instance()
		{
			this.Clear();

			foreach(var item in iDnaBaseNucleotides.Instance)
				this.Add( new iDnaBaseStatItem( item, 0, 0));
		}

		public void Reset()
		{
			init_instance();
		}


		public iDnaBaseStatItem this[char rootBaseCode]
		{
			get { return this.FirstOrDefault(i => i.RootBase != null && i.RootBase.Code == rootBaseCode); }
		}


		public iDnaBaseStatItem this[iDnaBase rootBase]
		{
			get { return this.FirstOrDefault( i => i.RootBase == rootBase); }
		}

		public override void Add(iDnaBaseStatItem item)
		{
			if(item == null || item.RootBase == null)
				return;

			var		existent	= this[item.RootBase];

			if(existent == null)
				base.Add(item);
			else
			{
				existent.Count	= item.Count;
			}
		}

		public override void AddRange(IEnumerable<iDnaBaseStatItem> list)
		{
			if(list == null)
				return;

			foreach(var item in list)
				this.Add(item);
		}



/*

#define T_DELTA					3					// NT par défaut ='T'
#define DEFAULT_DELTA			T_DELTA
#define INIT_DELTA_S			10.8F				// (S) ini (RNA only)
#define MOLARGASCONST			1.987F				// (R)  molar gas constant
#define T_HYBRIDFILTER			7.6F				// (k) correction for filter hybrid. 
#define MINI_BALDINO_NTS		12					// mini NTs pour appliquer Baldino 

#define DEF_CONCENTRATION  		50.0				// (c) probe concentration 
#define DEFVAL_K		  		50.0				// potasium
#define DEF_SALT		  		15.0


	#define LOG_DEF_CONCENTRATION_DIV4			1.09691001
	#define LOG10_DEFVAL_K					1.69897
	#define LOG10_DEF_SALT					1.17609126
#endif 

 * */
		const int					MINI_BALDINO_NTS			= 12;
		const double				DEF_CONCENTRATION  			= 50.0,
									LOG10_DEFVAL_K				= 1.69897,
									LOG10_DEF_SALT				= 1.17609126;


		protected double TmLessThanBaldino()
		{
			double		fLocalTm;
			double		a_count = (double) CountA,
						t_count = (double) CountT,
						g_count = (double) CountG,
						c_count = (double) CountC;

			fLocalTm	= (2.0F * (a_count + t_count)) + (4.0F * (g_count + c_count));
			return fLocalTm;
		}



		protected double TmBetterBaldino()
		{
			double		log10value		= LOG10_DEFVAL_K;
			double		dTmp;
			double		totalNodes		= (double) Total;

			// ******************************************************** 
			// Baldino améliorée !!
			// * ********************************************************
			dTmp	= 43.375 + (16.6 * log10value) + (0.41 * (PercentG + PercentC)) - (675.0 / totalNodes);
			return dTmp;
		}


		protected double CalculateTemprature()
		{
			if(Total <= MINI_BALDINO_NTS)
				return TmLessThanBaldino();
			else
				return TmBetterBaldino();
		}

		public double SequenceMeltingTm
		{
			get { return CalculateTemprature(); }
		}


		//protected double CalculateTmBetterBaldino()
		//{
		//	////Ulong		ulG,
		//	////			ulC;
		//	////double		dG,
		//	////			dC,
		//	//double		dG_percent,
		//	//			dC_percent;
		//	//			//dNts;

		//	////dNts = (double)ulTotal_nts;


		//	////ulG = (double) _stats.CountG;		// (Ulong)lpATGC_count->_Gnts;
		//	////ulC = (double) _stats.CountC;		// (Ulong)lpATGC_count->_Cnts;

		//	//dG_percent = _stats.PercentG;		// ((double)(dG / dNts) * 100.0);     /* %G */
		//	//dC_percent = _stats.PercentC;		// ((double)(dC / dNts) * 100.0);     /* %C */

		//	return TmBetterBaldino();			// (fBetterBaldino(dG_percent, dC_percent, ulTotal_nts));
		//}
	}
}
