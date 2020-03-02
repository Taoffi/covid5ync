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
		public App() : base()
		{
			//Task.Run(() => LoadSequence());
		}

		void LoadSequence()
		{
			//iDnaSequence	sequence = iDnaSequence.Instance;

			//sequence.Clear();

			//System.IO.StreamReader sr = new System.IO.StreamReader("coronavirus dna sequence-ncbi-2020-01-all.txt", true);

			//string		strSequence	= sr.ReadToEnd();
			//sr.Close();

			//await sequence.ParseString(strSequence);	//.Substring(0, 2096));
		}

	}


}
