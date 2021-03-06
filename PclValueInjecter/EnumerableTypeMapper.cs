﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Xciles.PclValueInjecter
{
    public class EnumerableTypeMapper<TSource, TTarget> : ITypeMapper<TSource, TTarget>
        where TSource : class
        where TTarget : class
    {
        public TTarget Map(TSource source, TTarget target)
        {
            if (source == null) return null;
            var targetArgumentType = typeof(TTarget).GetGenericArguments()[0];

            var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(targetArgumentType));
            var add = list.GetType().GetMethod("Add");

            foreach (var o in source as IEnumerable)
            {
                // This might need a pretier solution
                // Resolves base type lists not mapping correctly (Ilist<int> with only 0's, strings can't be created...)
                if (o.GetType().BaseType == typeof(ValueType) || o is string) 
                {
                    add.Invoke(list, new[] { o });
                }
                else
                {
                    var t = Creator.Create(targetArgumentType);
                    add.Invoke(list, new[] { Mapper.Map(o, t, o.GetType(), targetArgumentType) });
                }
            }
            return (TTarget)list;
        }
    }
}