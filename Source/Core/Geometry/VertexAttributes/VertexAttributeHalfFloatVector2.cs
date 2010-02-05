﻿#region License
//
// (C) Copyright 2009 Patrick Cozzi and Deron Ohlarik
//
// Distributed under the Boost Software License, Version 1.0.
// See License.txt or http://www.boost.org/LICENSE_1_0.txt.
//
#endregion

using OpenTK;

namespace MiniGlobe.Core.Geometry
{
    public class VertexAttributeHalfFloatVector2 : VertexAttribute<Vector2h>
    {
        public VertexAttributeHalfFloatVector2(string name)
            : base(name, VertexAttributeType.HalfFloatVector2)
        {
        }

        public VertexAttributeHalfFloatVector2(string name, int capacity)
            : base(name, VertexAttributeType.HalfFloatVector2, capacity)
        {
        }
    }
}