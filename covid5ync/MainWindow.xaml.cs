﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace iDna
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			Loaded += MainWindow_Loaded;
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			iDnaSequence.Instance.SequenceParseCompleted += Instance_SequenceParseCompleted;
			LoadSequence();
		}

		private void Instance_SequenceParseCompleted(iDnaSequence sender, int nodeCount)
		{
			if(sender != null)
			{
				//sequenceCtrl.listItems.ItemsSource = sender;
				textBlockNodeCount.Text = sender.Count.ToString();
			}
		}

		async void LoadSequence()
		{
			string			strInfo,
							strSequence,
							strAppInfo;
			iDnaSequence	sequence		= iDnaSequence.Instance;
			Uri				seqInfoUri		= new Uri("/data/covid-19-sequence-info.txt", UriKind.Relative),
							seqUri			= new Uri("/data/covid-19-sequence.txt", UriKind.Relative),
							appInfoUri		= new Uri("/data/app-version-info.txt", UriKind.Relative);

			sequence.Name	= "Wuhan seafood market pneumonia virus genome assembly, whole_genome";

			await Dispatcher.Invoke(async() =>
			{
				Stream		infStream		= Application.GetResourceStream(seqInfoUri).Stream,
							seqStream		= Application.GetResourceStream(seqUri).Stream,
							appInfoStream	= Application.GetResourceStream(appInfoUri).Stream;

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

				using (StreamReader reader = new StreamReader(appInfoStream))
				{
					strAppInfo = reader.ReadToEnd();
				}
				appInfoStream.Close();
				textBoxAppInfo.Text = strAppInfo;

				await sequence.ParseString(strSequence);
			});

		}
	}
}
