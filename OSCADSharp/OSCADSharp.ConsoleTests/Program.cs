﻿using OSCADSharp.Solids;
using OSCADSharp.Transforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSCADSharp.ConsoleTests
{
    class Program
    {
        static void Main(string[] args)
        {
            Cube cube = new Cube() {
                Size = new Vector3(5, 5, 5),
                Center = true
            };

            ColoredObject co = new ColoredObject(cube, "Red");
            var mirror = new RotatedObject(co, new Vector3(0, 90, 0));

            Console.WriteLine(mirror.ToString());
            Console.ReadKey();
        }
    }
}
