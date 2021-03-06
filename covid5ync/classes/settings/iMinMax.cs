﻿using isosoft.root;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iDna
{
	[DataContract(Namespace = "")]
	public class iMinMaxInt : RootObject
	{
		protected int					_limitMin					= 0,
										_limitMax					= int.MaxValue;

		protected int					_minValue					= 0,
<<<<<<< HEAD
										_maxValue					= 255;
=======
										_maxValue					= int.MaxValue;		// 255;
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15

		public iMinMaxInt(int limitMin, int limitMax, int min, int max) : base()
		{
			_limitMin	= limitMin;
			_limitMax	= limitMax;
			init_instance();

			MinValue	= min;
			MaxValue	= max;
		}

		public iMinMaxInt()
		{
			init_instance();
		}

		void init_instance()
		{
			_limitMin		= 0;
			_limitMax		= int.MaxValue;
		}

		public bool IsInError
		{
			get
			{
				return _minValue > _maxValue 
					
					|| _maxValue > _limitMax 
					|| _minValue > _limitMax

					|| _maxValue < _limitMin
					|| _minValue < _limitMin;
			}
		}


		public int MinValue
		{
			get { return _minValue; }
			set
			{
				if(value == _minValue || value <= 0)
					return;
				_minValue	= value;
				AdjustMinMaxValues();
				RaisePropertyChanged();
<<<<<<< HEAD
				NotifyPropertyChanged(() => Length);
=======
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15
				NotifyPropertyChanged(() => IsInError);
			}
		}

		public int MaxValue
		{
			get { return _maxValue; }
			set
			{
				if(value == _maxValue || value <= 0)
					return;

				_maxValue	= value;
				AdjustMinMaxValues();
				RaisePropertyChanged();
<<<<<<< HEAD
				NotifyPropertyChanged(() => Length);
=======
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15
				NotifyPropertyChanged(() => IsInError);
			}
		}

<<<<<<< HEAD
		public int Length
		{
			get {  return _maxValue - MinValue; }
		}
=======
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15

		public int LimitMax
		{
			get { return _limitMax; }
			set
			{
				if(value == _limitMax)
					return;

				_limitMax = value;
				AdjustMinMaxLimitValues();
				RaisePropertyChanged();
				NotifyPropertyChanged(() => IsInError);
			}
		}

		public int LimitMin
		{
			get { return _limitMin; }
			set
			{
				if(value == _limitMin)
					return;

				_limitMin = value;
				AdjustMinMaxLimitValues();
				RaisePropertyChanged();
				NotifyPropertyChanged(() => IsInError);
			}
		}


		/// <summary>
		/// for serialization / deserialzation
		/// </summary>
		[DataMember]
		public int StartIndex
		{
			get { return _minValue; }
			set
			{
				if (value == _minValue || value <= 0)
					return;
				_minValue = value;
				RaisePropertyChanged();
			}
		}

		/// <summary>
		/// for serialization / deserialzation
		/// </summary>
		[DataMember]
		public int EndIndex
		{
			get { return _maxValue; }
			set
			{
				if (value == _maxValue || value <= 0)
					return;
				_maxValue = value;
				RaisePropertyChanged();
			}
		}


		/// <summary>
		/// for serialization / deserialzation
		/// </summary>
		[DataMember]
		public int MaxLimit
		{
			get { return _limitMax; }
			set
			{
				if(value == _limitMax)
					return;

				_limitMax = value;
				RaisePropertyChanged();
			}
		}

		/// <summary>
		/// for serialization / deserialzation
		/// </summary>
		[DataMember]
		public int MinLimit
		{
			get { return _limitMin; }
			set
			{
				if(value == _limitMin)
					return;

				_limitMin = value;
				RaisePropertyChanged();
			}
		}



		public void AdjustMinMaxLimitValues()
		{
			if(_limitMin < _limitMax)
			{
				int		tmp	= _limitMin;

				_limitMin	= _limitMax;
				_limitMax	= tmp;
			}
		}

	
		void AdjustMinMaxValues()
		{
			int		tmp		= _minValue;

			if(_minValue > _maxValue)
			{
				_minValue	= _maxValue;
				_minValue	= tmp;
			}

			bool	limitExceeded	= (_minValue < _limitMin) || (_maxValue > _limitMax);

			if (_minValue < _limitMin)
				_minValue		= LimitMin;

			if(_maxValue > _limitMax)
				_maxValue	= LimitMax;
		}

	}

}
