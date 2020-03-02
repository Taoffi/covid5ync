using isosoft.root;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace iDna
{
	public class iDnaBaseList :  RootListTemplate<iDnaBase>
	{
		public iDnaBaseList() : base()
		{
			
		}

		public iDnaBase this[char code]
		{
			get { return this.FirstOrDefault(i => i.Code == code); }
		}

		public override void Add(iDnaBase item)
		{
			if(item == null)
				return;

			var		existent	= this[item.Code];

			if(existent == null)
				base.Add(item);
		}

		public override void AddRange(IEnumerable<iDnaBase> list)
		{
			this.Clear();

			if(list == null)
				return;

			foreach(var item in list)
				this.Add(item);
		}
	}
}
