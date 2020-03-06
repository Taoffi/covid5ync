using iDna.controls;
using isosoft.root;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace iDna.vm
{
	public class iDnaCommandCentral : RootObject
	{
		static iDnaCommandCentral	_instance	= null;

		public static iDnaCommandCentral Instance
		{
			get
			{
				if(_instance == null)
				{
					_instance	= new iDnaCommandCentral();
				}
				return _instance;
			}
		}

		public static iDnaCommandCentral GetInstance()
		{
			return Instance;
		}


		protected ICommand		_downLoadSequences			= null,
								_openFile					= null,
								_loadBuiltinSequence		= null,     // load application's delivered sequence
								_loadSequenceFromClipboard = null,		// load sequence from clipoard
								_saveAs						= null,
								_saveSelections				= null,
								_copySelectionToClipboard	= null,
								_aboutApp					= null,		// about
								_whatIsNew					= null,		// this app version info
								_gotoProjectPage			= null,		// open project's web page
								_options					= null,		// settings
								_contactSupport				= null,		// contact and bug report
			
								_notYetImplemented			= null;

		protected iDnaCommandCentral() : base()
		{

		}

		public ICommand DownloadSequences
		{
			get
			{
				if(_downLoadSequences == null)
				{
					_downLoadSequences = new CommandExecuter(() =>
					{
						TryOpenWebPage("https://www.ncbi.nlm.nih.gov/genbank/sars-cov-2-seqs/");
					});
				}
				return _downLoadSequences;
			}
		}

		public ICommand OpenFile
		{
			get
			{
				if (_openFile == null)
				{
					_openFile = new CommandExecuter( async() =>
					{
						// ShowNotYetImplemented();

						string			fileName		= OpenReadSequenceFile();

						if(string.IsNullOrEmpty(fileName))
							return;

						iDnaSequence	sequence		= iDnaSequence.Instance;

						await Dispatcher.CurrentDispatcher.Invoke(async() =>
						{
							string strSequence	 = "";

							try
							{
								strSequence		= File.OpenText(fileName).ReadToEnd();
							}
							catch (Exception ex)
							{
								ShowMessage("Sorry!. Could not open or read the file.\r\n" + ex.Message, "Could not open file");
								return;
							}
						
							sequence.Name				= Path.GetFileName(fileName);
							sequence.SequenceFileInfo	= fileName;
							await sequence.ParseString(strSequence);
						});

					});
				}
				return _openFile;
			}
		}

		/// <summary>
		/// load the built-in sequence (application's resource file)
		/// </summary>
		public ICommand LoadBuiltinSequence
		{
			get
			{
				if (_loadBuiltinSequence == null)
				{
					_loadBuiltinSequence = new CommandExecuter(async() =>
					{
						//ShowNotYetImplemented();

						string			strInfo,
										strSequence;
						iDnaSequence	sequence		= iDnaSequence.Instance;
						Uri				seqInfoUri		= new Uri("/data/covid-19-sequence-info.txt", UriKind.Relative),
										seqUri			= new Uri("/data/covid-19-sequence.txt", UriKind.Relative);

						sequence.Name	= "Wuhan seafood market pneumonia virus genome assembly, whole_genome";

						await Dispatcher.CurrentDispatcher.Invoke(async() =>
						{
							Stream		infStream		= Application.GetResourceStream(seqInfoUri).Stream,
										seqStream		= Application.GetResourceStream(seqUri).Stream;

							using (StreamReader reader = new StreamReader(infStream))
							{
								strInfo = reader.ReadToEnd();
							}
							infStream.Close();
							sequence.SequenceFileInfo = strInfo;

							using (StreamReader reader = new StreamReader(seqStream))
							{
								strSequence = reader.ReadToEnd();
							}
							seqStream.Close();

							await sequence.ParseString(strSequence);
						});
					});
				}
				return _loadBuiltinSequence;
			}
		}

		// 
		public ICommand LoadSequenceFromClipboard
		{
			get
			{
				if (_loadSequenceFromClipboard == null)
				{
					_loadSequenceFromClipboard = new CommandExecuter(() =>
					{
						ShowNotYetImplemented();
					});
				}
				return _loadSequenceFromClipboard;
			}
		}


		public ICommand SaveAs
		{
			get
			{
				if (_saveAs == null)
				{
					_saveAs = new CommandExecuter(() =>
					{
						// ShowNotYetImplemented();

						iDnaSequence seq		= iDnaSequence.Instance;

						if(seq == null)
							return;

						if (seq.Count <= 0)
						{
							ShowMessage("This sequence seems to be empty. Please check.", "Save sequence");
							return;
						}

						string		targetFile	= SelectSaveSequenceFile();

						if(string.IsNullOrEmpty(targetFile))
							return;

						string		str			= FormattedNodeSttring(seq);

						if(str.Length <= 0)
						{
							ShowMessage("Failed to format the sequence string!.", "Save sequence error");
							return;
						}

						WriteStringToFile(str, targetFile);
					});
				}
				return _saveAs;
			}
		}

		public ICommand SaveSelectedRegions
		{
			get
			{
				if(_saveSelections == null)
				{
					_saveSelections = new CommandExecuter(() =>
					{
						// ShowNotYetImplemented();

						iDnaSequence seq		= iDnaSequence.Instance;

						if(seq == null)
							return;

						var		selectedItems			= seq.SelectedItems;

						if (selectedItems == null || selectedItems.Count() <= 0)
						{
							ShowMessage("No regions currently selected. Please check.", "Save selected regions");
							return;
						}

						string		targetFile	= SelectSaveSequenceFile();

						if(string.IsNullOrEmpty(targetFile))
							return;

						string		str			= FormattedNodeSttring(selectedItems);

						if(str.Length <= 0)
						{
							ShowMessage("Failed to format the selected nodes string!.", "Save selected regions error");
							return;
						}

						WriteStringToFile(str, targetFile);
					});
				}
				return _saveSelections;
			}
		}

		/// <summary>
		/// command: copy selected nodes to clipboard
		/// </summary>
		public ICommand CopySelectionsToClipboard
		{
			get
			{
				if (_copySelectionToClipboard == null)
				{
					_copySelectionToClipboard = new CommandExecuter(() =>
					{
						//ShowNotYetImplemented();

						iDnaSequence seq		= iDnaSequence.Instance;

						if(seq == null)
							return;

						var		selectedItems			= seq.SelectedItems;

						if (selectedItems == null || selectedItems.Count() <= 0)
						{
							ShowMessage("No nucleotides currently selected. Please select something to copy.", "Copy to clipboard");
							return;
						}

						string		str			= FormattedNodeSttring(selectedItems);

						if(str.Length <= 0)
							return;

						Clipboard.SetText(str, TextDataFormat.UnicodeText);
					});
				}
				return _copySelectionToClipboard;
			}
		}


		public ICommand AboutApp
		{
			get
			{
				if (_aboutApp == null)
				{
					_aboutApp = new CommandExecuter(() =>
					{
						// ShowNotYetImplemented();
						iAboutWindow		window	= new iAboutWindow();

						window.Owner	= Application.Current.MainWindow;
						window.ShowDialog();
					});
				}
				return _aboutApp;
			}
		}

		// _contactSupport
		public ICommand ContactSupport
		{
			get
			{
				if (_contactSupport == null)
				{
					_contactSupport = new CommandExecuter(() =>
					{
						TryOpenWebPage("mailto:covid5ync@5ync.net?subject=covid-5ync app comments and remarks");
					});
				}
				return _contactSupport;
			}
		}

		public ICommand WhatIsNew
		{
			get
			{
				if(_whatIsNew == null)
				{
					_whatIsNew = new CommandExecuter(() =>
					{
						ShowNotYetImplemented();
					});
				}
				return _whatIsNew;
			}
		}

		public ICommand GotoProjectPage
		{
			get
			{
				if (_gotoProjectPage == null)
				{
					_gotoProjectPage = new CommandExecuter(() =>
					{
						System.Diagnostics.Process.Start("http://covid-5.5ync.net/");
					});
				}
				return _gotoProjectPage;
			}
		}

		public ICommand OptionsCommand
		{
			get
			{
				if (_options == null)
				{
					_options = new CommandExecuter(() =>
					{
						// ShowNotYetImplemented();

						iDnaSettingsWindow window	= new iDnaSettingsWindow();

						window.Owner	= Application.Current.MainWindow;
						window.ShowDialog();
					});
				}
				return _options;
			}
		}

		void ShowMessage(string message, string caption)
		{
			Dispatcher.CurrentDispatcher.Invoke(() =>
			{
				System.Windows.MessageBox.Show(message, caption);
			});
		}

		void ShowNotYetImplemented()
		{
			ShowMessage("This is not yet implemented!. Thank you for your patience!", "Feature not yet implemented");
		}

		void TryOpenWebPage(string url)
		{
			TryLaunchApplication(url, "Sorry!...\r\nCould not open the web page. Please check your system configuration", "Oups!");
		}

		void TryLaunchApplication(string commandLine, string failureMessage, string failureCaption)
		{
			if(string.IsNullOrEmpty(commandLine))
				return;

			try
			{
				System.Diagnostics.Process.Start(commandLine);
			}
			catch (Exception)
			{
				if(! string.IsNullOrEmpty(failureMessage))
				{
					if (string.IsNullOrEmpty(failureCaption))
						failureCaption = "Error";

					ShowMessage(failureMessage, failureCaption);
				}
			}
		}

		string FormattedNodeSttring(IEnumerable<iDnaNode> nodeList)
		{
			if(nodeList == null || nodeList.Count() <= 0)
				return "";

			string		str			= "";
			int			lastIndex	= 0,
						counter		= 0;

			foreach(var node in nodeList)
			{
				// for non consecutive selection: add new line
				if (lastIndex > 0 && lastIndex + 1 != node.Index)
				{
					str += "\r\n";
					counter	= 0;
				}
				else if(counter >= 100)
				{
					str		+= "\r\n" + string.Format("{0:000000} ", node.Index);
					counter = 0;
				}

				// the very first and for non consecutive selection: add new line + coordinate
				if (lastIndex <= 0 || lastIndex + 1 != node.Index)
					str += string.Format("{0:000000} ", node.Index);
				else
				{
					if(counter % 10 == 0 && counter > 0)
						str	+= " ";
				}
				str		+= node.Code;
				lastIndex	= node.Index;
				counter++;
			}

			return str;
		}



		string OpenReadSequenceFile()
		{
			OpenFileDialog		dialog		= new OpenFileDialog();

			dialog.Filter	= "Text files|*.txt|All files|*.*";
			dialog.CheckFileExists	= true;
			dialog.DefaultExt		= "*.txt";
			dialog.Multiselect		= false;

			var			result			= dialog.ShowDialog(Application.Current.MainWindow);
			string		selectedFile	= null;

			if(result == null || result.Value == false)
				return null;

			selectedFile	= dialog.FileName;
			return selectedFile;
		}


		string SelectSaveSequenceFile()
		{
			SaveFileDialog		dialog		= new SaveFileDialog();

			dialog.Filter			= "Text files|*.txt|All files|*.*";
			dialog.CheckPathExists	= true;
			//dialog.CheckFileExists	= true;
			dialog.DefaultExt		= "*.txt";

			var			result			= dialog.ShowDialog(Application.Current.MainWindow);
			string		selectedFile	= null;

			if(result == null || result.Value == false)
				return null;

			selectedFile	= dialog.FileName;
			return selectedFile;
		}

		bool WriteStringToFile(string str, string targetFile)
		{
			if(string.IsNullOrEmpty(str) || string.IsNullOrEmpty(targetFile))
				return false;

			StreamWriter stream	= null;

			try
			{
				stream			= new StreamWriter(targetFile, append: false, encoding: Encoding.UTF8);
				stream.WriteLine(str);
				stream.Flush();
				stream.Close();
			}
			catch (Exception ex)
			{
				ShowMessage("Sorry... could not write to the selected file.\r\n" + ex.Message, "Save error");
				return false;
			}

			return true;
		}

		public static string ApplicationVersionInfoString()
		{
			Uri			appInfoUri		= new Uri("/data/app-version-info.txt", UriKind.Relative);
			Stream		appInfoStream	= Application.GetResourceStream(appInfoUri).Stream;
			string		strAppInfo		= "";

			using (StreamReader reader = new StreamReader(appInfoStream))
			{
				strAppInfo = reader.ReadToEnd();
			}
			appInfoStream.Close();
			return strAppInfo;
		}


		public ICommand NotYetImplemented
		{
			get
			{
				if (_notYetImplemented == null)
				{
					_notYetImplemented = new CommandExecuter(() =>
					{
						ShowNotYetImplemented();
					});
				}
				return _notYetImplemented;
			}
		}

		
	}
}
