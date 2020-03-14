using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iDna
{
	public partial class iDnaSequence
	{
		public static string FormattedNodeSttring(IEnumerable<iDnaNode> nodeList)
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


		public static async Task<iDnaSequence> FromString(string str)
		{
			iDnaSequence		sequence	= new iDnaSequence();

			await sequence.ParseString(str);
			return sequence;
		}


		/// <summary>
		/// convert sequence basket (repeats and hairpins) to region indexes
		/// </summary>
		/// <param name="sourceList">the source sequence basket (list of sequences)</param>
		/// <returns>the list of regions of the source basket</returns>
		protected static iDnaRegionIndexList SequenceListToRegionList(IEnumerable<iDnaSequence> sourceList)
		{
			iDnaRegionIndexList		list = new iDnaRegionIndexList();

			if (sourceList == null || sourceList.Count() <= 0)
				return list;

			foreach (var seq in sourceList)
			{
				int		min		= seq.Min(i => i.Index),
						max		= seq.Max(i => i.Index);

				list.Add(new iDnaRegionIndex(seq.Name, min, max, seq.SequenceFileInfo) { Occurrences = seq._nOccurrences });
			}
			return list;
		}


		/// <summary>
		/// rebuild sequence basket (repeats and hairpins) from region indexes
		/// </summary>
		/// <param name="sourceSequence">the parent sequence to which the basket should be attached</param>
		/// <param name="sourceList">the region index list</param>
		/// <returns>the list of sequence basket</returns>
		protected static List<iDnaSequence> RegionListToSequenceList(iDnaSequence sourceSequence, iDnaRegionIndexList sourceList)
		{
			List<iDnaSequence>		list = new List<iDnaSequence>();

			if (sourceList == null || sourceList.Count() <= 0 || sourceSequence == null)
				return list;

			foreach (var region in sourceList)
			{
				var		nodes		= sourceSequence.Where(i => i.Index >= region.MinValue && i.Index <= region.MaxValue);

				if(nodes == null || nodes.Count() <= 0)
					continue;

				list.Add(new iDnaSequence(region.Name, nodes, refOnly: true, nOccurrences: region.Occurrences));
			}
			return list;
		}



	}
}
