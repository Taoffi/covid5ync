using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace iDna
{
	public class iDnaBaseNucleotides : iDnaBaseList
	{
		static iDnaBaseNucleotides		_instance		= null;

		public static iDnaBaseNucleotides Instance
		{
			get
			{
				if(_instance == null)
					_instance = new iDnaBaseNucleotides();

				return _instance;
			}
		}

		public static iDnaBaseNucleotides GetInstance()
		{
			return Instance;
		}

		protected iDnaBaseNucleotides() : base()
		{
			CreateDefault();
		}

		protected void CreateDefault()
		{
			this.Clear();

			this.Add(new iDnaBase('a', GetBaseColor('a')));
			this.Add(new iDnaBase('t', GetBaseColor('t')));
			this.Add(new iDnaBase('g', GetBaseColor('g')));
			this.Add(new iDnaBase('c', GetBaseColor('c')));

			this.A.Pair	= this.T;
			this.T.Pair	= this.A;

			this.G.Pair	= this.C;
			this.C.Pair	= this.G;
		}

		public iDnaBase A
		{
			get {  return this['a']; }
		}

		public iDnaBase T
		{
			get { return this['t']; }
		}

		public iDnaBase G
		{
			get { return this['g']; }
		}

		public iDnaBase C
		{
			get { return this['c']; }
		}

		public bool IsValidSearchString(string str)
		{
			if(string.IsNullOrEmpty(str))
				return false;

			var inexistent	= str.Where( c => this[c] == null);
			return inexistent == null || inexistent.Count() <= 0;
		}


		public string GetPairString(string str)
		{
			string		pair	= "";

			if(! IsValidSearchString(str))
				return pair;

			foreach(char c in str)
			{
				var node	= this[c];
				
				if(node == null || node.Pair == null)
					continue;

				pair	+= node.Pair.Code;
			}

			return pair;
		}


		public static Color GetBaseColor(char code)
		{
			switch(code)
			{
				case 'a':
					return Colors.LightGreen;

				case 't':
					return Colors.LightBlue;

				case 'g':
					return Colors.Orange;

				case 'c':
					return Colors.Yellow;

				default:
					return Colors.Violet;
			}
		}

		public static bool IsValidChar(char c)
		{
			string		validChars	= "atgc";
			return validChars.Contains(c);
		}

		public static bool IsValidString(string str)
		{
			if(string.IsNullOrEmpty(str))
				return false;

			var		wrongChars	= str.Where(c => ! IsValidChar(c));
			return wrongChars == null || wrongChars.Count() <= 0;
		}
	}
}

