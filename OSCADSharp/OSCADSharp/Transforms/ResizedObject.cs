﻿using OSCADSharp.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSCADSharp.Spatial;

namespace OSCADSharp.Transforms
{
    /// <summary>
    /// An object that's been resized to a specified set of X/Y/Z dimensions
    /// </summary>
    internal class ResizedObject : SingleStatementObject
    {
        /// <summary>
        /// Size of the object in terms of X/Y/Z
        /// </summary>
        internal Vector3 Size { get; set; }

        /// <summary>
        /// Creates a resized object
        /// </summary>
        /// <param name="obj">The object(s) to be resized</param>
        /// <param name="size">The size to resize to, in terms of x/y/z dimensions</param>
        internal ResizedObject(OSCADObject obj, Vector3 size) : base(obj)
        {
            this.Size = size;
        }

        public override string ToString()
        {
            string resizeCommand = String.Format("resize([{0}, {1}, {2}])", this.Size.X.ToString(),
                this.Size.Y.ToString(), this.Size.Z.ToString());
            var formatter = new SingleBlockFormatter(resizeCommand, this.obj.ToString());
            return formatter.ToString();
        }

        public override OSCADObject Clone()
        {
            return new ResizedObject(this.obj.Clone(), this.Size)
            {
                Name = this.Name
            };
        }
        
        public override Vector3 Position()
        {
            var bounds = this.Bounds();
            return Vector3.Average(bounds.BottomLeft, bounds.TopRight);
        }

        public override Bounds Bounds()
        {
            var oldBounds = obj.Bounds();            

            double xScaleFactor =  this.Size.X > 0 ? this.Size.X / Math.Abs(oldBounds.X_Max - oldBounds.X_Min) : 1;
            double yScaleFactor = this.Size.Y > 0 ? this.Size.Y / Math.Abs(oldBounds.Y_Max - oldBounds.Y_Min) : 1;
            double zScaleFactor = this.Size.Z > 0 ? this.Size.Z / Math.Abs(oldBounds.Z_Max - oldBounds.Z_Min) : 1;
            Vector3 scaleMultiplier = new Vector3(xScaleFactor, yScaleFactor, zScaleFactor);

            return new Bounds(oldBounds.BottomLeft * scaleMultiplier, oldBounds.TopRight * scaleMultiplier);            
        }
    }
}
