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
		protected	int			_count;

		public iDnaBaseStatItem() : base()
		{

		}

		public iDnaBaseStatItem(iDnaBase rootBase, int count)
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
			}
		}
	}

	public class iDnaBaseStats : RootListTemplate<iDnaBaseStatItem>
	{

		public iDnaBaseStats() : base()
		{
			init_instance();
		}


		int GetBaseCount(char rootBaseCode)
		{
			var item	= this[rootBaseCode];

			return item == null ? 0 : item.Count;
		}

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


		protected void init_instance()
		{
			this.Clear();

			foreach(var item in iDnaBaseNucleotides.Instance)
				this.Add( new iDnaBaseStatItem( item, 0));
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
	}
}
