﻿using OSCADSharp.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSCADSharp.Bindings
{
    internal class Binding
    {
        public string OpenSCADFieldName { get; set; }
        public Variable BoundVariable { get; set; }
    }
}
