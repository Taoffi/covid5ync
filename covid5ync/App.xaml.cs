using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace iDna
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		internal const string		_appFilesExtension		= "covid-5ync";
		
		internal static App			Instance;
		protected static string		_mainWindowTitleBase	= "";
		protected string			_startupFileName		= null;


		public App() : base()
		{
			Instance	= this;
		}


		internal string StartupFileName
		{
			get {  return _startupFileName; }
		}



		protected override void OnStartup(StartupEventArgs e)
		{
			if (e == null)
			{
				base.OnStartup(e);
				return;
			}

			string[]		args	= e.Args;
			iDnaSequence	seq		= iDnaSequence.Instance;

			if(args != null && args.Length > 0)
			{
				_startupFileName	= args[0];
			}

			if(seq != null)
				seq.SequenceParseCompleted += Seq_SequenceParseCompleted;

			base.OnStartup(e);
		}

		private void Seq_SequenceParseCompleted(iDnaSequence sender, int nodeCount)
		{
			SetMainWindowTitle(iDnaSequence.Instance.Name);
		}

		internal void SetMainWindowTitle(string addThis)
		{
			if(MainWindow == null || string.IsNullOrEmpty(addThis))
				return;

			if(string.IsNullOrEmpty(_mainWindowTitleBase))
				_mainWindowTitleBase	= MainWindow.Title;

			MainWindow.Title	= _mainWindowTitleBase + " - " + addThis;
		}

		void LoadSequence()
		{
		}




	}


}
