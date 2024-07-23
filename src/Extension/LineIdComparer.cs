using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;

namespace FaultDetectorDotNet.Extension
{
    public class LineIdComparer : IComparer
    {
        private readonly ListSortDirection _direction;

        public LineIdComparer(ListSortDirection direction)
        {
            _direction = direction;
        }

        public int Compare(object x, object y)
        {
            var itemX = x as dynamic;
            var itemY = y as dynamic;

            if (itemX == null || itemY == null)
            {
                return 0;
            }

            string lineIdX = itemX.LineId;
            string lineIdY = itemY.LineId;

            var partsX = lineIdX.Split(new[] { '-' }, 2).Select(p => p.Trim()).ToArray();
            var partsY = lineIdY.Split(new[] { '-' }, 2).Select(p => p.Trim()).ToArray();

            if (partsX.Length < 2 || partsY.Length < 2)
            {
                return 0;
            }

            string classX = partsX[1];
            string classY = partsY[1];
            int lineNumberX = int.Parse(partsX[0]);
            int lineNumberY = int.Parse(partsY[0]);

            int classComparison = string.Compare(classX, classY, StringComparison.Ordinal);
            if (classComparison == 0)
            {
                return _direction == ListSortDirection.Ascending
                    ? lineNumberX.CompareTo(lineNumberY)
                    : lineNumberY.CompareTo(lineNumberX);
            }

            return _direction == ListSortDirection.Ascending ? classComparison : -classComparison;
        }
    }
}