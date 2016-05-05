﻿using OSCADSharp.Spatial;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSCADSharp.Solids.Imported
{
    /// <summary>
    /// Processes a bitmap image by treating contiguous same-color regions as cubes
    /// </summary>
    internal class PolygonalImageProcessor : IImageProcessor
    {
        #region Private Fields
        private string imagePath;
        #endregion

        #region Public Fields
        public Bounds ImageBounds { get; set; }
        #endregion

        #region Constructors
        internal PolygonalImageProcessor(string imagePath)
        {
            this.imagePath = imagePath;
        }
        #endregion

        public OSCADObject ProcessImage()
        {
            var polygons = this.processImage();
            OSCADObject obj = new OSCADObject.MultiStatementObject("union()", polygons);
            obj = obj.Rotate(0, 0, 180);
            obj = obj.Translate(ImageBounds.Length, ImageBounds.Width, 0);

            return obj;
        }

        private List<OSCADObject> processImage()
        {
            Bitmap img = new Bitmap(Image.FromFile(this.imagePath));

            if (img.Width > 200 || img.Height > 200)
            {
                throw new InvalidOperationException("Cannot process images larger greater than 200x200 pixels");
            }

            this.ImageBounds = new Bounds(new Vector3(), new Vector3(img.Width, img.Height, 1));

            var separatedColors = this.separateColors(img);
            IEnumerable<List<KeyValuePair<Point, Color>>> contiguousSections = new List<List<KeyValuePair<Point, Color>>>();

            foreach (var colorGroup in separatedColors.Values)
            {
                var sections = this.getContiguousSections(colorGroup);
                contiguousSections = contiguousSections.Concat(sections);
            }
            throw new NotImplementedException();
        }

        private List<List<KeyValuePair<Point, Color>>> getContiguousSections(List<KeyValuePair<Point, Color>> colorGrouping)
        {
            KeyValuePair<Point, Color>[,] grid = createGrid(colorGrouping);
            var sections = new List<List<KeyValuePair<Point, Color>>>();

            while (colorGrouping.Count > 0)
            {
                var origin = colorGrouping[0];
                colorGrouping.RemoveAt(0);
                sections.Add(this.getNeighbors(origin, grid, colorGrouping));
            }

            return sections;
        }

        private List<KeyValuePair<Point, Color>> getNeighbors(KeyValuePair<Point, Color> origin, KeyValuePair<Point, Color>[,] grid, List<KeyValuePair<Point, Color>> colorGrouping)
        {
            List<KeyValuePair<Point, Color>> neighbors = new List<KeyValuePair<Point, Color>>();
            Queue<KeyValuePair<Point, Color>> nextOrigins = new Queue<KeyValuePair<Point, Color>>();
            nextOrigins.Enqueue(origin);

            while(nextOrigins.Count > 0)
            {
                List<Point> neighboringPoints = new List<Point>() {
                    new Point(origin.Key.X, origin.Key.Y + 1),      //Above
                    new Point(origin.Key.X, origin.Key.Y - 1),      //Below
                    new Point(origin.Key.X - 1, origin.Key.Y),      //Left
                    new Point(origin.Key.X + 1, origin.Key.Y),      //Right
                    new Point(origin.Key.X - 1, origin.Key.Y+1),    //UpperLeft
                    new Point(origin.Key.X + 1, origin.Key.Y + 1),  //UpperRight
                    new Point(origin.Key.X - 1, origin.Key.Y - 1),  //LowerLeft
                    new Point(origin.Key.X + 1, origin.Key.Y - 1),  //LowerRight
                };

                foreach (var pt in neighboringPoints)
                {
                    //Todo:  Find neighboring points
                }
            }

            return neighbors;
        }

        private static KeyValuePair<Point, Color>[,] createGrid(List<KeyValuePair<Point, Color>> colorGrouping)
        {
            Point topLeft = new Point(int.MaxValue, int.MaxValue);
            Point bottomRight = new Point(int.MinValue, int.MinValue);


            foreach (var pair in colorGrouping)
            {
                if (pair.Key.X < topLeft.X)
                    topLeft.X = pair.Key.X;

                if (pair.Key.Y < topLeft.Y)
                    topLeft.Y = pair.Key.Y;

                if (pair.Key.X > bottomRight.X)
                    bottomRight.X = pair.Key.X;

                if (pair.Key.Y > bottomRight.Y)
                    bottomRight.Y = pair.Key.Y;
            }

            int width = bottomRight.X - topLeft.X;
            int height = bottomRight.Y - topLeft.Y;
            return new KeyValuePair<Point, Color>[width, height];
        }

        private Dictionary<string, List<KeyValuePair<Point, Color>>> separateColors(Bitmap img)
        {
            var colorGroupings = new Dictionary<string, List<KeyValuePair<Point, Color>>>();
            for (int column = 0; column < img.Width; column++)
            {
                for (int row = 0; row < img.Height; row++)
                {
                    var color = img.GetPixel(column, row);
                    string key = String.Format("{0}-{1}-{2}", color.R, color.G, color.B);
                    if (!colorGroupings.ContainsKey(key))
                    {
                        colorGroupings[key] = new List<KeyValuePair<Point, Color>>();
                    }

                    colorGroupings[key].Add(new KeyValuePair<Point, Color>(new Point(column, row), color));
                }
            }

            return colorGroupings;
        }
    }
}