using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Athena.Common.Utils
{
    public static class AthenaUtils
    {
        public static System.DateTime ParseAthenaDate(string strDate)
        {
            var year = int.Parse(strDate.Substring(0, 4));
            var month = int.Parse(strDate.Substring(4, 2));
            var day = int.Parse(strDate.Substring(6, 2));

            return new System.DateTime(year, month, day);
        }

        public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, true);

                Debug.Log("Copied: " + targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }
    }

    public static class TransformExtensions
    {
        public static void SetLayer(this Transform trans, int layer)
        {
            trans.gameObject.layer = layer;
            foreach (Transform child in trans)
                child.SetLayer(layer);
        }
    }

    // <summary>
    /// Small helper class to convert viewport, screen or world positions to canvas space.
    /// Only works with screen space canvases.
    /// </summary>
    /// <example>
    /// <code>
    /// objectOnCanvasRectTransform.anchoredPosition = specificCanvas.WorldToCanvasPoint(worldspaceTransform.position);
    /// </code>
    /// </example>
    public static class CanvasPositioningExtensions
    {
        public static Vector3 WorldToCanvasPosition(this Canvas canvas, Vector3 worldPosition, Camera camera = null)
        {
            if (camera == null)
            {
                camera = Camera.main;
            }
            var viewportPosition = camera.WorldToViewportPoint(worldPosition);
            return canvas.ViewportToCanvasPosition(viewportPosition);
        }

        public static Vector3 ScreenToCanvasPosition(this Canvas canvas, Vector3 screenPosition)
        {
            var viewportPosition = new Vector3(screenPosition.x / Screen.width,
                                               screenPosition.y / Screen.height,
                                               0);
            return canvas.ViewportToCanvasPosition(viewportPosition);
        }

        public static Vector3 ViewportToCanvasPosition(this Canvas canvas, Vector3 viewportPosition)
        {
            var centerBasedViewPortPosition = viewportPosition - new Vector3(0.5f, 0.5f, 0);
            var canvasRect = canvas.GetComponent<RectTransform>();
            var scale = canvasRect.sizeDelta;
            return Vector3.Scale(centerBasedViewPortPosition, scale);
        }
    }

    public class NaturalSortString : IComparer<string>
    {
        //use a buffer for performance since we expect
        //the Compare method to be called a lot
        private char[] _splitBuffer = new char[256];

        public int Compare(string x, string y)
        {
            //first split each string into segments
            //of non-numbers and numbers
            IList<string> a = SplitByNumbers(x);
            IList<string> b = SplitByNumbers(y);

            int aInt, bInt;
            int numToCompare = (a.Count < b.Count) ? a.Count : b.Count;
            for (int i = 0; i < numToCompare; i++)
            {
                if (a[i].Equals(b[i]))
                    continue;

                bool aIsNumber = Int32.TryParse(a[i], out aInt);
                bool bIsNumber = Int32.TryParse(b[i], out bInt);
                bool bothNumbers = aIsNumber && bIsNumber;
                bool bothNotNumbers = !aIsNumber && !bIsNumber;
                //do an integer compare
                if (bothNumbers) return aInt.CompareTo(bInt);
                //do a string compare
                if (bothNotNumbers) return a[i].CompareTo(b[i]);
                //only one is a number, which are
                //by definition less than non-numbers
                if (aIsNumber) return -1;
                return 1;
            }
            //only get here if one string is empty
            return a.Count.CompareTo(b.Count);
        }

        private IList<string> SplitByNumbers(string val)
        {
            System.Diagnostics.Debug.Assert(val.Length <= 256);
            List<string> list = new List<string>();
            int current = 0;
            int dest = 0;
            while (current < val.Length)
            {
                //accumulate non-numbers
                while (current < val.Length &&
                       !char.IsDigit(val[current]))
                {
                    _splitBuffer[dest++] = val[current++];
                }
                if (dest > 0)
                {
                    list.Add(new string(_splitBuffer, 0, dest));
                    dest = 0;
                }
                //accumulate numbers
                while (current < val.Length &&
                       char.IsDigit(val[current]))
                {
                    _splitBuffer[dest++] = val[current++];
                }
                if (dest > 0)
                {
                    list.Add(new string(_splitBuffer, 0, dest));
                    dest = 0;
                }
            }
            return list;
        }
    }
}