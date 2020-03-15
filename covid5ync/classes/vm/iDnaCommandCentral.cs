using iDna.controls;
using isosoft.root;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;

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
								_gotoSARS_site				= null,
								_openFile					= null,

								_openXml					= null,

								_loadBuiltinSequence		= null,     // load application's delivered sequence
								_loadSequenceFromClipboard = null,		// load sequence from clipoard
								_saveAs						= null,
								_saveXml					= null,

								_saveSelections				= null,
								_resetSelections			= null,
								_copySelectionToClipboard	= null,
								_aboutApp					= null,		// about
								_whatIsNew					= null,		// this app version info
								_gotoProjectPage			= null,		// open project's web page
								_options					= null,		// settings
								_contactSupport				= null,		// contact and bug report

								_findRepeats				= null,		// search repeats
								_findHairpins				= null,		// search for hairpins

								_resetSelectionToRepeats	= null,
								_resetSelectionToHairpins	= null,

								_copyRepeatsToClipboard		= null,
								_saveRepeatsToFile			= null,
								_copyHairpinsToClipboard		= null,
								_saveHairpinsToFile			= null,

								_searchCommand				= null,
								_searchPairsCommand			= null,

								_searchStringsFromFile		= null,
								_searchPairStringsFromFile	= null,
			
								_defineWorkRegionFromSelection		= null,

								_clearHairpinBaskets		= null,
								_clearRepeatBaskets			= null,
								_clearSearchBaskets			= null,

								_gotoCovid_19_situationPage	= null,
								_maangeBookMarks			= null,

								//_selectAllNamedRegions		= null,

								//_editSequenceInfo = null,
								_notYetImplemented = null;

		protected iDnaCommandCentral() : base()
		{

		}



		//// _selectAllNamedRegions
		//public ICommand SelectAllNamedRegions
		//{
		//	get
		//	{
		//		if (_selectAllNamedRegions == null)
		//		{
		//			_selectAllNamedRegions = new CommandExecuter(() =>
		//			{
		//				// ShowNotYetImplemented();

		//				iDnaSequence seq = iDnaSequence.Instance;

		//				if (seq == null || seq.Count <= 0 || seq.SequenceNamedRegionList == null || seq.SequenceNamedRegionList.Count <= 0)
		//				{
		//					ShowMessage("This sequence is either empty or has no defined named regions. Please check.", "Select named regions");
		//					return;
		//				}

		//				seq.ResetSelection(false, null);

		//				foreach(var region in seq.SequenceNamedRegionList)
		//				{
		//					var	nodes	= seq.Where(i => i.Index >= region.MinValue && i.Index <= region.MaxValue);
		//					if(nodes == null || nodes.Count() <= 0)
		//						continue;

		//					foreach(var node in nodes)
		//						node.IsSelected	= true;
		//				}
		//			});
		//		}
		//		return _selectAllNamedRegions;
		//	}
		//}

		public ICommand GotoSARS_Website
		{
			get
			{
				if(_gotoSARS_site == null)
				{
					_gotoSARS_site = new CommandExecuter(() =>
					{
						TryOpenWebPage("https://www.ncbi.nlm.nih.gov/genomes/SARS/SARS.html");
					});
				}
				return _gotoSARS_site;
			}
		}

		public ICommand GotoCovid19Situation_Website
		{
			get
			{
				if (_gotoCovid_19_situationPage == null)
				{
					_gotoCovid_19_situationPage = new CommandExecuter(() =>
					{
						TryOpenWebPage("https://www.cdc.gov/coronavirus/2019-nCoV/summary.html");
					});
				}
				return _gotoCovid_19_situationPage;
			}
		}

		// _maangeBookMarks
		public ICommand ManageBookmarks
		{
			get
			{
				if (_maangeBookMarks == null)
				{
					_maangeBookMarks = new CommandExecuter(() =>
					{
						var					bookmarkList	= iDnaBookmarkList.Instance;
						BookMarksWindow		window			= new BookMarksWindow() {  DataContext = bookmarkList };

						window.Owner	= Application.Current.MainWindow;
						window.ShowDialog();
					});
				}
				return _maangeBookMarks;
			}
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

		internal bool AskLoadNewSequence()
		{
			if (MessageBox.Show("This will initialize all current selections and region definitions\r\n"
							+ "Are you sure you want to continue?", "Initialize new sequence",
							MessageBoxButton.YesNoCancel, 
							MessageBoxImage.Warning) != System.Windows.MessageBoxResult.Yes)
				return false;
			return true;
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

						string			fileName		= OpenReadTextOrXmlFile();

						if(string.IsNullOrEmpty(fileName))
							return;

						iDnaSequence	sequence			= iDnaSequence.Instance;
						bool			canLoadNewSequence	= true;

						if(sequence.HasPendingChanges)
							canLoadNewSequence	= AskLoadNewSequence();

						if(!canLoadNewSequence)
							return;

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
						bool			canLoadNew		= true;

						if(sequence.HasPendingChanges)
						{
							canLoadNew	= AskLoadNewSequence();
						}

						if(!canLoadNew)
							return;

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

						string		targetFile	= SelectSaveSequenceFileTextOrXml();

						if(string.IsNullOrEmpty(targetFile))
							return;

						string		str			= iDnaSequence.FormattedNodeSttring(seq);

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


		// _saveXml
		public ICommand SaveXml
		{
			get
			{
				if (_saveXml == null)
				{
					_saveXml = new CommandExecuter(() =>
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

						string		targetFile	= SelectSaveSequenceFileTextOrXml(defaultToAppXml: true);

						if(string.IsNullOrEmpty(targetFile))
							return;

						iDnaSequenceContract	seqContract		= new iDnaSequenceContract(seq);
						try
						{
							DataContractSerializer	ser			= new DataContractSerializer(typeof(iDnaSequenceContract));
							XmlWriterSettings		xmlSettings	= new XmlWriterSettings() { Indent = true, CloseOutput = true, Encoding = Encoding.UTF8};
							XmlWriter				xml			= XmlWriter.Create(targetFile, xmlSettings);

							ser.WriteObject(xml, seqContract);
							xml.Close();
						}
						catch (Exception ex)
						{
							ShowMessage("Sorry!. An error occurred: \r\n" + ex.Message, "Save xml");
						}
					});
				}
				return _saveXml;
			}
		}

		internal bool LoadXmlFile(string srcFile)
		{
			if(string.IsNullOrEmpty(srcFile) || ! File.Exists(srcFile))
				return false;

			iDnaSequenceContract	seqContract		= null;
			try
			{
				DataContractSerializer	ser			= new DataContractSerializer(typeof(iDnaSequenceContract));
				XmlReaderSettings		xmlSettings	= new XmlReaderSettings() { CloseInput = true};
				XmlReader				xml			= XmlReader.Create(srcFile, xmlSettings);

				seqContract	= ser.ReadObject(xml) as iDnaSequenceContract;
				xml.Close();
			}
			catch (Exception ex)
			{
				ShowMessage("Sorry!. An error occurred: \r\n" + ex.Message, "Load xml file");
				return false;
			}

			return true;
		}


		public ICommand LoadFromXml
		{
			get
			{
				if (_openXml == null)
				{
					_openXml = new CommandExecuter(() =>
					{
						// ShowNotYetImplemented();
						string		srcFile	= OpenReadTextOrXmlFile( defaultToAppXml: true);

						if(string.IsNullOrEmpty(srcFile))
							return;

						LoadXmlFile(srcFile);
						//iDnaSequenceContract	seqContract		= null;
						//try
						//{
						//	DataContractSerializer	ser			= new DataContractSerializer(typeof(iDnaSequenceContract));
						//	XmlReaderSettings		xmlSettings	= new XmlReaderSettings() { CloseInput = true};
						//	XmlReader				xml			= XmlReader.Create(srcFile, xmlSettings);

						//	seqContract	= ser.ReadObject(xml) as iDnaSequenceContract;
						//	xml.Close();
						//}
						//catch (Exception ex)
						//{
						//	ShowMessage("Sorry!. An error occurred: \r\n" + ex.Message, "Load xml file");
						//}
					});

					iDnaSequence seq		= iDnaSequence.Instance;
					Task.Run(() => seq.NotifySequenceLoaded());
				}
				return _openXml;
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

						string		targetFile	= SelectSaveSequenceFileTextOrXml();

						if(string.IsNullOrEmpty(targetFile))
							return;

						string		str			= iDnaSequence.FormattedNodeSttring(selectedItems);

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

		// _resetSelections
		public ICommand ResetAllSelections
		{
			get
			{
				if(_resetSelections == null)
				{
					_resetSelections = new CommandExecuter(() =>
					{
						// ShowNotYetImplemented();

						iDnaSequence seq		= iDnaSequence.Instance;

						if(seq == null)
							return;

						seq.ResetSelection(false, null);
					});
				}
				return _resetSelections;
			}
		}


		// _clearHairpinBaskets
		// _clearRepeatBaskets
		// _clearSearchBaskets

		public ICommand ClearSearchBaskets
		{
			get
			{
				if(_clearSearchBaskets == null)
				{
					_clearSearchBaskets = new CommandExecuter(() =>
					{
						// ShowNotYetImplemented();

						iDnaSequence seq		= iDnaSequence.Instance;

						if(seq == null)
							return;

						seq.ClearSearchAndSelectionBaskets();
					});
				}
				return _clearSearchBaskets;
			}
		}


		public ICommand ClearRepeatBaskets
		{
			get
			{
				if (_clearRepeatBaskets == null)
				{
					_clearRepeatBaskets = new CommandExecuter(() =>
					{
						// ShowNotYetImplemented();

						iDnaSequence seq = iDnaSequence.Instance;

						if (seq == null)
							return;

						seq.ClearRepeatsBaskets();
					});
				}
				return _clearRepeatBaskets;
			}
		}

		public ICommand ClearHairpinBaskets
		{
			get
			{
				if (_clearHairpinBaskets == null)
				{
					_clearHairpinBaskets = new CommandExecuter(() =>
					{
						// ShowNotYetImplemented();

						iDnaSequence seq = iDnaSequence.Instance;

						if (seq == null)
							return;

						seq.ClearHairpinBaskets();
					});
				}
				return _clearHairpinBaskets;
			}
		}


		// _defineWorkRegionFromSelection
		public ICommand DefineWorkRegionFromSelection
		{
			get
			{
				if(_defineWorkRegionFromSelection == null)
				{
					_defineWorkRegionFromSelection = new CommandExecuter(() =>
					{
						// ShowNotYetImplemented();

						iDnaSequence seq		= iDnaSequence.Instance;

						if(seq == null)
							return;

						string	dlgTitle		= "Define search region from selection";
						var		selectedItems	= seq.SkipWhile(i => i.IsSelected == false).TakeWhile(n => n.IsSelected);	// seq.SelectedItems;

						if (selectedItems == null || selectedItems.Count() <= 1)
						{
							ShowMessage("No regions currently selected. Please check.", dlgTitle);
							return;
						}

						//if(MessageBox.Show("This will define the first selected contigous region as the search region for repeats, hairpins and primer.\r\n"
						//			+ "Are you ok with this?", dlgTitle, MessageBoxButton.YesNoCancel, MessageBoxImage.Question) != MessageBoxResult.Yes)
						//	return;

						int	minIndex	= selectedItems.Min(i => i.Index),
							maxIndex	= selectedItems.Max(i => i.Index);

						iDnaGobalSettings.Instance.SetSearchRegion(minIndex, maxIndex);

						//ShowMessage(string.Format("Your search region is now from (zero-based index): {0:0000} to {1:0000}", minIndex, maxIndex), dlgTitle);
					});
				}
				return _defineWorkRegionFromSelection;
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

						string		str			= iDnaSequence.FormattedNodeSttring(selectedItems);

						if(str.Length <= 0)
							return;

						Clipboard.SetText(str, TextDataFormat.UnicodeText);
					});
				}
				return _copySelectionToClipboard;
			}
		}


		string RepeatsString(iDnaSequence seq, bool takeHairpins)
		{
			if (seq == null)
				return "";

			var		repeatSequences		= takeHairpins ? seq.HairpinBasket : seq.RepeatsBasket;

			if (repeatSequences == null || repeatSequences.Count() <= 0)
				return "";

			string str = (takeHairpins ? "Pair-repeats" : "Repeats") + " of sequence: " + seq.Name + " " + string.Format("{0:yyyy-MM-dd H:mm:ss}", DateTime.Now) + "\r\n";

			foreach (var r in repeatSequences)
			{
				str += iDnaSequence.FormattedNodeSttring(r) + "\r\n";
			}
			return str;
		}


		//
		public ICommand CopyRepeatsToClipboard
		{
			get
			{
				if (_copyRepeatsToClipboard == null)
				{
					_copyRepeatsToClipboard = new CommandExecuter(() =>
					{
						//ShowNotYetImplemented();

						iDnaSequence	seq				= iDnaSequence.Instance;
						var				repeatSequences	= seq == null ? null : seq.RepeatsBasket;
						

						if (repeatSequences == null || repeatSequences.Count() <= 0)
						{
							ShowMessage("No repeats currently in the basket. Please check.", "Copy repeats to clipboard");
							return;
						}

						string			str		= RepeatsString(seq, takeHairpins: false);

						if(string.IsNullOrEmpty(str))
							return;

						Clipboard.SetText(str, TextDataFormat.UnicodeText);
					});
				}
				return _copyRepeatsToClipboard;
			}
		}


		public ICommand CopyHairpinsToClipboard
		{
			get
			{
				if (_copyHairpinsToClipboard == null)
				{
					_copyHairpinsToClipboard = new CommandExecuter(() =>
					{
						//ShowNotYetImplemented();

						iDnaSequence	seq				= iDnaSequence.Instance;
						var				repeatSequences	= seq == null ? null : seq.HairpinBasket;
						

						if (repeatSequences == null || repeatSequences.Count() <= 0)
						{
							ShowMessage("No pair-repeats currently in the basket. Please check.", "Copy pair-repeats to clipboard");
							return;
						}

						string			str		= RepeatsString(seq, takeHairpins: true);

						if(string.IsNullOrEmpty(str))
							return;

						Clipboard.SetText(str, TextDataFormat.UnicodeText);
					});
				}
				return _copyHairpinsToClipboard;
			}
		}

		// _resetSelectionToRepeats
		public ICommand ResetSelectionsToRepeats
		{
			get
			{
				if (_resetSelectionToRepeats == null)
				{
					_resetSelectionToRepeats = new CommandExecuter(() =>
					{
						//ShowNotYetImplemented();

						iDnaSequence	seq				= iDnaSequence.Instance;
						var				repeatsBasket	= seq == null ? null : seq.RepeatsBasket;
						

						if (repeatsBasket == null || repeatsBasket.Count() <= 0)
						{
							ShowMessage("No repeats currently in the basket. Please check.", "Select repeats");
							return;
						}

						seq.ResetSelection(false, null);
						
						foreach(var sr in repeatsBasket)
						{
							foreach(var node in sr)
								node.IsSelected		= true;
						}
					});
				}
				return _resetSelectionToRepeats;
			}
		}

		public ICommand ResetSelectionsToHairpins
		{
			get
			{
				if (_resetSelectionToHairpins == null)
				{
					_resetSelectionToHairpins = new CommandExecuter(() =>
					{
						//ShowNotYetImplemented();

						iDnaSequence	seq				= iDnaSequence.Instance;
						var				repeatsBasket	= seq == null ? null : seq.HairpinBasket;
						

						if (repeatsBasket == null || repeatsBasket.Count() <= 0)
						{
							ShowMessage("No pair-repeats currently in the basket. Please check.", "Select pair-repeats");
							return;
						}

						seq.ResetSelection(false, null);
						
						foreach(var sr in repeatsBasket)
						{
							foreach(var node in sr)
								node.IsSelected		= true;
						}
					});
				}
				return _resetSelectionToHairpins;
			}
		}


		/// <summary>
		/// save repeats to file
		/// </summary>
		public ICommand SaveRepeatsToFile
		{
			get
			{
				if(_saveRepeatsToFile == null)
				{
					_saveRepeatsToFile = new CommandExecuter(() =>
					{
						// ShowNotYetImplemented();

						iDnaSequence	seq		= iDnaSequence.Instance;

						if(seq == null)
							return;

						var		repeatSequences			= seq.RepeatsBasket;

						if (repeatSequences == null || repeatSequences.Count() <= 0)
						{
							ShowMessage("No repeats currently in the basket. Please check.", "Save repeats");
							return;
						}

						string		targetFile	= SelectSaveSequenceFileTextOrXml();

						if(string.IsNullOrEmpty(targetFile))
							return;

						string		str			= RepeatsString(seq, takeHairpins: false);

						WriteStringToFile(str, targetFile);
					});
				}
				return _saveRepeatsToFile;
			}
		}


		public ICommand SaveHairpinsToFile
		{
			get
			{
				if(_saveHairpinsToFile == null)
				{
					_saveHairpinsToFile = new CommandExecuter(() =>
					{
						// ShowNotYetImplemented();

						iDnaSequence	seq		= iDnaSequence.Instance;

						if(seq == null)
							return;

						var		repeatSequences			= seq.HairpinBasket;

						if (repeatSequences == null || repeatSequences.Count() <= 0)
						{
							ShowMessage("No pair-repeats currently in the basket. Please check.", "Save pair-repeats");
							return;
						}

						string		targetFile	= SelectSaveSequenceFileTextOrXml();

						if(string.IsNullOrEmpty(targetFile))
							return;

						string		str			= RepeatsString(seq, takeHairpins: true);

						WriteStringToFile(str, targetFile);
					});
				}
				return _saveHairpinsToFile;
			}
		}


		public ICommand Search
		{
			get
			{
				if(_searchCommand == null)
				{
					_searchCommand	= new CommandExecuter( async() =>
					{
                        string      str     = iDnaSequence.Instance.SearchString;

                        if(! iDnaBaseNucleotides.IsValidString(str))
                            return;

                        await iDnaSequence.Instance.FindString(str, 
											SequenceSearchType.SearchNormal, 
											dispatcher: null,
											resetSelections: true,
											gotoFirstNodePage: true,
											updateSeletions: true, 
											clearSelectionBasket: true,
											initCancellation: true);

					});
				}

				return _searchCommand;
			}
		}


		public ICommand SearchPairs
		{
			get
			{
				if (_searchPairsCommand == null)
				{
					_searchPairsCommand = new CommandExecuter(async () =>
					{
						string		str = iDnaSequence.Instance.SearchPairString;
						if (!iDnaBaseNucleotides.IsValidString(str))
							return;

						str		= iDnaBaseNucleotides.Instance.GetPairString(str);

						await iDnaSequence.Instance.FindString(str, SequenceSearchType.SearchPairs,
																	dispatcher: null,
																	resetSelections: true,
																	gotoFirstNodePage: true,
																	updateSeletions: true,
																	clearSelectionBasket: true,
																	initCancellation: true);

					});
				}

				return _searchPairsCommand;
			}
		}

		/*
				_loadSelectionStrings		= null,
			_loadPairSelectionStrings	= null,
		 */

		string SelectAndGetTextFileString(string errorDialgTitle)
		{
			string			fileName	= OpenReadTextOrXmlFile(defaultToAppXml: false);

			if (string.IsNullOrEmpty(fileName))
				return null;

			string		strText = "";

			try
			{
				strText		= File.OpenText(fileName).ReadToEnd();
			}
			catch (Exception ex)
			{
				ShowMessage("Sorry!. Could not open or read the file.\r\n" + ex.Message, errorDialgTitle);
			}

			return strText;
		}



		protected async Task<int> ParseAndSearchForStringList(iDnaSequence sequence, string globalSring, SequenceSearchType searchType)
		{
			if(string.IsNullOrEmpty(globalSring) || sequence == null)
				return 0;

			string				strText		= globalSring.Replace(" ", "");
			string[]			strLines	= strText.Split( new char[] { '\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
			List<string>		strList		= new List<string>();
			int					occurrences	= 0;

			if(strLines == null || strLines.Length <= 0)
			{
				ShowMessage("Sorry!. Could get the strings to search. Please ensure each string is on a separate line", "Could not get search strings from file");
				return 0;
			}

			foreach(string str in strLines.Distinct())
			{
				string		validString	= iDnaBaseNucleotides.TrimInvalidChars(str);

				if (! string.IsNullOrWhiteSpace(validString))
					strList.Add(validString);
			}

			if(strList.Count <= 0)
			{
				ShowMessage("Sorry!. The provided file has no valid nucleotide strings. Please check", "Search strings from file");
				return 0;
			}

			sequence.ResetSelection(false, null);

			switch(searchType)
			{
				case SequenceSearchType.SearchPairs:
					sequence.SearchPairStringList = strList;
					occurrences		= await sequence.SearchCurrentPairStringList(null, updateSelections: true, clearSelectionBasket: true);
					break;

				case SequenceSearchType.SearchNormal:
				case SequenceSearchType.SearchRepeats:
				default:
					sequence.SearchStringList = strList;
					occurrences		= await sequence.SearchCurrentStringList(null, updateSelections: true, clearSelectionBasket: true);
					break;
			}

			return occurrences;
		}



		public ICommand SearchStringsFromFile
		{
			get
			{
				if (_searchStringsFromFile == null)
				{
					_searchStringsFromFile = new CommandExecuter(async () =>
					{
						// ShowNotYetImplemented();

						iDnaSequence		sequence	= iDnaSequence.Instance;

						if(sequence == null || sequence.Count <= 0)
						{
							ShowMessage("The current sequence looks to be empty. Please check", "Search regions from file");
							return;
						}

						int occurrences	= 0;

						await Dispatcher.CurrentDispatcher.Invoke( async() =>
						{
							string		strText = SelectAndGetTextFileString("Could not open search strings file");

							if(string.IsNullOrEmpty(strText))
								return;

							occurrences	= await ParseAndSearchForStringList(sequence, strText, SequenceSearchType.SearchNormal);
						});

					});
				}

				return _searchStringsFromFile;
			}
		}


		public ICommand SearchPairStringsFromFile
		{
			get
			{
				if (_searchPairStringsFromFile == null)
				{
					_searchPairStringsFromFile = new CommandExecuter(async () =>
					{
						// ShowNotYetImplemented();

						iDnaSequence		sequence	= iDnaSequence.Instance;

						if (sequence == null || sequence.Count <= 0)
						{
							ShowMessage("The current sequence looks to be empty. Please check", "Search regions from file");
							return;
						}

						int occurrences	= 0;

						await Dispatcher.CurrentDispatcher.Invoke( async() =>
						{
							string		strText = SelectAndGetTextFileString("Could not open search strings file");

							if(string.IsNullOrEmpty(strText))
								return;

							occurrences	= await ParseAndSearchForStringList(sequence, strText, SequenceSearchType.SearchPairs);
						});

					});
				}

				return _searchPairStringsFromFile;
			}
		}



		public ICommand FindRepeats
		{
			get
			{
				if (_findRepeats == null)
				{
					_findRepeats = new CommandExecuter(() =>
					{
						// ShowNotYetImplemented();

						iDnaSequence		seq		= iDnaSequence.Instance;

						if(seq.Count <= 0)
						{
							ShowMessage("Current sequence seems to be empty. Please check.", "Find repeats");
							return;
						}

						Task.Run( async() =>
						{
							await iDnaSequence.Instance.GetRepeatsOrHairpins(null, searchHairpins: false);
						});

					});
				}
				return _findRepeats;
			}
		}


		public ICommand FindHairpins
		{
			get
			{
				if (_findHairpins == null)
				{
					_findHairpins = new CommandExecuter(() =>
					{
						// ShowNotYetImplemented();

						iDnaSequence		seq		= iDnaSequence.Instance;

						if(seq.Count <= 0)
						{
							ShowMessage("Current sequence seems to be empty. Please check.", "Find pair repeats");
							return;
						}

						Task.Run( async() =>
						{
							await iDnaSequence.Instance.GetRepeatsOrHairpins(null, searchHairpins: true);
						});

					});
				}
				return _findHairpins;
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

		internal void TryOpenWebPage(string url)
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

		string GetOpenSaveFileDialogFilterString(bool defaultToAppXml)
		{
			string			extText	= "Text files|*.txt",
							extXml	= "Xml file|*.xml",
							appXml	= "Covid-5ync file|*." + App._appFilesExtension,
							extAll	= "All files|*.*";
			return defaultToAppXml		? appXml	+ "|"	+ extXml	+ "|" + extText	+ "|"	+ extAll
										: extText	+ "|"	+ extXml	+ "|" + appXml	+ "|"   + extAll;
		}

		string OpenReadTextOrXmlFile(bool defaultToAppXml = false)
		{
			OpenFileDialog		dialog		= new OpenFileDialog();
			
			dialog.Filter			= GetOpenSaveFileDialogFilterString(defaultToAppXml);
			dialog.CheckFileExists	= true;

			if(defaultToAppXml)
				dialog.DefaultExt	= "*." + App._appFilesExtension;	// "*.xml";
			else
				dialog.DefaultExt	= "*.txt";

			dialog.Multiselect		= false;

			var			result			= dialog.ShowDialog(Application.Current.MainWindow);
			string		selectedFile	= null;

			if(result == null || result.Value == false)
				return null;

			selectedFile	= dialog.FileName;
			return selectedFile;
		}


		string SelectSaveSequenceFileTextOrXml( bool defaultToAppXml = false)
		{
			SaveFileDialog		dialog		= new SaveFileDialog();

			dialog.Filter			= GetOpenSaveFileDialogFilterString(defaultToAppXml);
			dialog.CheckPathExists	= true;
			//dialog.CheckFileExists	= true;
			
			dialog.DefaultExt		= defaultToAppXml ? "*." + App._appFilesExtension : "*.txt";

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

		public string AppVersionInfo
		{
			get {  return ApplicationVersionInfoString(); }
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
