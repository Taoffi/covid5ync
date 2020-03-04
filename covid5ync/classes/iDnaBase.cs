using isosoft.root;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace iDna
{
	public class iDnaBase : RootObject
	{
		protected char		_code;
		protected Color		_color				= Colors.Black;
		protected iDnaBase	_pair				= null;
		protected double	_linearBaseValue	= 0.05;

		public iDnaBase() : base()
		{

		}

		public iDnaBase(char code) : base()
		{
			_code		= code;
		}

		public iDnaBase(char code, Color color) : base()
		{
			_code		= code;
			_color		= color;
		}

		public iDnaBase Pair
		{
			get { return _pair; }
			set
			{
				if(value == this || value == _pair)
					return;

				_pair	= value;
				RaisePropertyChanged();
			}
		}

		public char Code
		{
			get { return _code; }
			set
			{
				if(value == _code)
					return;

				_code	= value;
				RaisePropertyChanged();
			}
		}

		public double LinearBaseValue
		{
			get { return _linearBaseValue; }
			set
			{
				if(value == _linearBaseValue)
					return;

				_linearBaseValue = value;
				RaisePropertyChanged();
			}
		}


		public Color Color
		{
			get {  return _color; }
			set
			{
				if(value == _color)
					return;

				_color	= value;
				RaisePropertyChanged();
			}
		}
	}
}
