using isosoft.root;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iDna.vm
{
	public class iDnaSequenceSortOption
	{
		public iDnaBasketSortOption		SortOption	{  get; set; }
		public string					Name		{  get; set; }

		public iDnaSequenceSortOption(iDnaBasketSortOption sortOption)
		{
			SortOption		= sortOption;
			SetName();
		}

		void SetName()
		{
			switch(SortOption)
			{
				case iDnaBasketSortOption.NoSort:
				default:
					Name	= "No sort";
					return;

				case iDnaBasketSortOption.SortByNumberOfOccurrences:
					Name	= "By number of occurrences";
					return;

				case iDnaBasketSortOption.SortByPosition:
					Name	= "By position";
					return;
			}
		}
	}

	public delegate void SortOptionChangeHandler(iDnaSequenceSortOption selectedItem);

	public class iDnaSequenceSortOptionList : RootListTemplate<iDnaSequenceSortOption>
	{
		protected iDnaSequenceSortOption			_selectedItem;
		public event SortOptionChangeHandler		SelectedOptionChanged;

		public iDnaBasketSortOption SelectedOption
		{
			get {  return _selectedItem.SortOption; }
			set
			{
				if(value == _selectedItem.SortOption)
					return;

				var item	= this.FirstOrDefault(i => i.SortOption == value);
				if(item == null)
					return;

				SelectedItem	= item;
			}
		}

		public iDnaSequenceSortOption SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				_selectedItem = value;
				NotifyPropertyChanged(() => SelectedItem);
				NotifyPropertyChanged(() => SelectedOption);

				if(SelectedOptionChanged != null)
					SelectedOptionChanged.Invoke(_selectedItem);
			}
		}

		public iDnaSequenceSortOptionList() : base()
		{
			init_instance();
		}

		void init_instance()
		{
			this.Clear();
			var		enumValues	= Enum.GetValues(typeof(iDnaBasketSortOption));

			foreach(iDnaBasketSortOption item in enumValues)
			{
				iDnaSequenceSortOption option = new iDnaSequenceSortOption(item);
				this.Add(option);

				if(_selectedItem == null)
					_selectedItem	= option;
			}
		}
	}
}
