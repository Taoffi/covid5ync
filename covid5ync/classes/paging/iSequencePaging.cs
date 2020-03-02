using doc5Words.vm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iDna
{
	public class iDnaSequencePaging : iObjectPaging<iDnaNode>
	{
		public iDnaSequencePaging() : base()
		{

		}


		public iDnaSequencePaging(int pageSize) : base(pageSize)
		{
		}
	}

}
