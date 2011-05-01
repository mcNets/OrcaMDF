using System.Collections;
using System.Collections.Generic;

namespace OrcaMDF.Core.Engine.Pages
{
	public class ExtentAllocationMap : PrimaryRecordPage
	{
		protected bool[] ExtentMap = new bool[63904];

		public ExtentAllocationMap(byte[] bytes, MdfFile file)
			: base(bytes, file)
		{
			parseBitmap();
		}

		private void parseBitmap()
		{
			byte[] bitmap = Records[1].FixedLengthData;
			
			int index = 0;

			// Skip first 98 bytes and last 10
			foreach (byte b in bitmap)
			{
				var ba = new BitArray(new[] { b });

				for (int i = 0; i < 8; i++)
					ExtentMap[index++] = ba[i];
			}
		}

		public IEnumerable<ExtentPointer> GetAllocatedExtents()
		{
			int gamRangeStartPageID = (Header.Pointer.PageID / 511232) * 511232;

			for (int i = 0; i < ExtentMap.Length && i < File.NumberOfExtents; i++)
				if (ExtentMap[i])
					yield return new ExtentPointer(new PagePointer(Header.Pointer.FileID, gamRangeStartPageID + i * 8));
		}
	}
}