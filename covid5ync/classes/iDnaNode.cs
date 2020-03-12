using isosoft.root;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace iDna
{
	public class iDnaNode : RootObject
	{
		protected iDnaBase		_base;
		protected int			_index;
		protected bool			_isSelected			= false;
		protected iDnaSequence	_parentSequence		= null;

		public iDnaNode() : base()
		{

		}

		public iDnaNode(iDnaSequence parentSequence, iDnaBase rootBase, int index) : base()
		{
			_parentSequence		= parentSequence;
			_base				= rootBase;
			_index				= index;
		}

		public iDnaBase RootBaseItem
		{
			get { return _base; }
			set
			{
				if(value == _base)
					return;

				_base = value;
				//RaisePropertyChanged();
				NotifyPropertyChanged(null);	// notify all properties changed
			}
		}

		public double LinearBaseValue
		{
			get { return _base == null ? 1.0 : _base.LinearBaseValue; }
		}


		public iDnaSequence ParentSequence
		{
			get { return _parentSequence; }
			set
			{
				if(value == _parentSequence)
					return;

				_parentSequence = value;
				RaisePropertyChanged();
			}
		}


		public int Index
		{
			get { return _index; }
			set
			{
				if(value == _index)
					return;

				_index = value;
				RaisePropertyChanged();
			}
		}

		public char Code
		{
			get { return _base == null ? '?' : _base.Code; }
		}

		public Color Color
		{
			get {  return _base == null ? Colors.Violet : _base.Color; }
		}

		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				if(value == _isSelected)
					return;

				_isSelected		= value;

				//if(value == false)
				//	NotifyPropertyChanged(null);
				//else
				RaisePropertyChanged();
			}
		}
	}

	public class iDnaNodeDesignTime : iDnaNode
	{
		public iDnaNodeDesignTime() : base()
		{
			this.RootBaseItem	= iDnaBaseNucleotides.Instance['a'];

		}
	}
}
