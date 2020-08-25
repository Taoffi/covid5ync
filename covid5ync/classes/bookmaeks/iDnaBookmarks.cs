using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using isosoft.root;


namespace iDna
{
	public class iDnaBookmark : RootObject
	{
		protected string			_name,
									_description,
									_url;
		protected ICommand			_openUrl			= null;

		public iDnaBookmark() : base()
		{

		}

		public iDnaBookmark(string name, string url) : base()
		{
			_name		= name;
			_url		= url;
		}

		public string Name
		{
			get { return _name; }
			set
			{
				if(value == _name)
					return;
				_name = value;
				RaisePropertyChanged();
			}
		}


		public string Url
		{
			get { return _url; }
			set
			{
				if(value == _url)
					return;
				_url = value;
				RaisePropertyChanged();
			}
		}


		public string Description
		{
			get { return _description; }
			set
			{
				if(value == _description)
					return;
				_description = value;
				RaisePropertyChanged();
			}
		}

		public string ImageUrl
		{
			get
			{
				if(string.IsNullOrEmpty(_url))
					return "";

				Uri		uriResult;
				bool	couldConvert		= Uri.TryCreate(this._url, UriKind.Absolute, out uriResult)
									&& (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

				if(couldConvert)
				{
					string		str	= uriResult.Scheme + "://" + uriResult.Authority + "/favicon.ico";
					return str;
					//return uriResult.GetLeftPart(UriPartial.Path) + "/favicon.ico";
				}
					
				return "";
			}
		}

		internal void CopyOther(iDnaBookmark item)
		{
			if(item == null || item == this)
				return;

			this.Name			= item.Name;
			this.Url			= item.Url;
			this.Description	= item.Description;
		}

		protected void GotoUrl()
		{
			if(string.IsNullOrEmpty(_url))
				return;

			vm.iDnaCommandCentral.Instance.TryOpenWebPage(_url);
		}


		public ICommand	OpenUrl
		{
			get
			{
				if(_openUrl == null)
				{
					_openUrl	= new CommandExecuter(()=>
					{
						GotoUrl();
					});
				}
				return _openUrl;
			}
		}
	}

	public class iDnaBookmarkList : RootListTemplate<iDnaBookmark>
	{
		static iDnaBookmarkList			_instance		= null;

		protected iDnaBookmark			_selectedItem;


		public static iDnaBookmarkList Instance
		{
			get
			{
				if(_instance == null)
					_instance	= new iDnaBookmarkList();
				return _instance;
			}
		}

		public static iDnaBookmarkList GetInstance()
		{
			return Instance;
		}


		public iDnaBookmarkList() : base()
		{
			LoadFromAppConfig();
		}

		private void LoadFromAppConfig()
		{
			this.Clear();

			iDna.Properties.Settings.Default.Reload();

			char[]		separators	= { '|' };
			var			bookmarksCfg	= iDna.Properties.Settings.Default.UserBookMarks;
			string		name, url, description;

			if(bookmarksCfg == null || bookmarksCfg.Count <= 0)
				return;

			foreach(var str in bookmarksCfg)
			{
				// try to parse the string
				string[]	values  = str.Split(separators, StringSplitOptions.RemoveEmptyEntries);

				if(values.Length < 2)
					continue;

				name			= values[0];
				url				= values[1];
				description	= values.Length >= 3 ? values[2] : "";

				this.Add( new iDnaBookmark(name, url) {  Description = description});
			}
		}

		internal void SaveToAppConfig()
		{
			StringCollection	cfgValues	= new StringCollection();

			foreach(var item in this)
			{
				string	itemString		= item.Name + "|" + item.Url + "|" + (string.IsNullOrEmpty(item.Description) ? "": item.Description);
				cfgValues.Add(itemString);
			}

			iDna.Properties.Settings.Default.UserBookMarks	= cfgValues;
			iDna.Properties.Settings.Default.Save();
			iDna.Properties.Settings.Default.Reload();
		}


		public iDnaBookmark SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				_selectedItem = value;
				NotifyPropertyChanged(() => SelectedItem);
			}
		}


		public iDnaBookmark this[string name]
		{
			get { return this.FirstOrDefault(i => i.Name == name); }
		}

		public void InsertNew(iDnaBookmark item)
		{
			Add(item);
<<<<<<< HEAD
			NotifyCollectionChanged(this);
=======
			NotifyCollectionChanged(this, NotifyCollectionChangedAction.Add, item);
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15
			NotifyPropertyChanged(null);
		}

		public override void Add(iDnaBookmark item)
		{
			if (item == null || string.IsNullOrEmpty(item.Name))
				return;

			var existent = this[item.Name];

			if (existent == null)
				base.Add(item);
			else
			{
				existent.CopyOther(item);
			}
		}

		public override bool Remove(iDnaBookmark item)
		{
			bool result		= base.Remove(item);
			return result;
		}
	}
}
