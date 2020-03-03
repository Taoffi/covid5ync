using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using isosoft.root;

namespace iDna
{
	public class iDnaSequenceList : RootListTemplate<iDnaSequence>
	{

		public iDnaSequenceList() : base()
		{

		}

		/// <summary>
		/// get sequence by Id
		/// </summary>
		/// <param name="id">the id string</param>
		/// <returns>the sequence. null if none found</returns>
		public iDnaSequence this[string id]
		{
			get { return this.FirstOrDefault(i => i.Id == id); }
		}

	}


	public class iDnaSequenceListDesigntime : iDnaSequenceList
	{

		public iDnaSequenceListDesigntime() : base()
		{
			this.Add( new iDnaSequenceDesignTime());
			this.Add(new iDnaSequenceDesignTime());
			this.Add(new iDnaSequenceDesignTime());
		}
	}
}
