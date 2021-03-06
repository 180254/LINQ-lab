﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace QuerySamples
{
    public class ObjectReferenceEqualityComparer<T> : EqualityComparer<T> where T : class
    {
        public override bool Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }

        public override int GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}