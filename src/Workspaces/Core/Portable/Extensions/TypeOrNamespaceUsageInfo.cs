﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis
{
    [Flags]
    internal enum TypeOrNamespaceUsageInfo
    {
        /// <summary>
        /// Represents default value indicating no usage.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Represents a reference to a namespace or type on the left side of a dotted name (qualified name or member access).
        /// For example, 'NS' in <code>NS.Type x = new NS.Type();</code> or <code>NS.Type.StaticMethod();</code> or 
        /// 'Type' in <code>Type.NestedType x = new Type.NestedType();</code> or <code>Type.StaticMethod();</code>
        /// </summary>
        DottedName = 0x01,

        /// <summary>
        /// Represents a generic type argument reference.
        /// For example, 'Type' in <code>Generic{Type} x = ...;</code> or <code>class Derived : Base{Type} { }</code>
        /// </summary>
        GenericTypeArgument = 0x02,

        /// <summary>
        /// Represents a base type or interface reference in the base list of a named type.
        /// For example, 'Base' in <code>class Derived : Base { }</code>.
        /// </summary>
        BaseTypeOrInterface = 0x04,

        /// <summary>
        /// Represents a reference to a type whose instance is being created.
        /// For example, 'C' in <code>var x = new C();</code>, where 'C' is a named type.
        /// </summary>
        ObjectCreation = 0x08,

        /// <summary>
        /// Represents a reference to a namespace or type within a using or imports directive.
        /// For example, <code>using NS;</code> or <code>using static NS.Extensions</code> or <code>using Alias = MyType</code>.
        /// </summary>
        NamespaceOrTypeInUsing = 0x10,

        /// <summary>
        /// Represents a reference to a namespace name in a namespace declaration context.
        /// For example, 'N1' or <code>namespaces N1.N2 { }</code>.
        /// </summary>
        NamespaceDeclaration = 0x20,
    }

    internal static class TypeOrNamespaceUsageInfoExtensions
    {
        public static string ToLocalizableString(this TypeOrNamespaceUsageInfo info)
        {
            // We don't support localizing value combinations.
            Debug.Assert(info.IsSingleBitSet());

            switch (info)
            {
                case TypeOrNamespaceUsageInfo.DottedName:
                    return WorkspacesResources.TypeOrNamespaceUsageInfo_DottedName;

                case TypeOrNamespaceUsageInfo.GenericTypeArgument:
                    return WorkspacesResources.TypeOrNamespaceUsageInfo_GenericTypeArgument;

                case TypeOrNamespaceUsageInfo.BaseTypeOrInterface:
                    return WorkspacesResources.TypeOrNamespaceUsageInfo_BaseTypeOrInterface;

                case TypeOrNamespaceUsageInfo.ObjectCreation:
                    return WorkspacesResources.TypeOrNamespaceUsageInfo_ObjectCreation;

                case TypeOrNamespaceUsageInfo.NamespaceOrTypeInUsing:
                    return WorkspacesResources.TypeOrNamespaceUsageInfo_NamespaceOrTypeInUsing;

                case TypeOrNamespaceUsageInfo.NamespaceDeclaration:
                    return WorkspacesResources.TypeOrNamespaceUsageInfo_NamespaceDeclaration;

                default:
                    Debug.Fail($"Unhandled value: '{info.ToString()}'");
                    return info.ToString();
            }
        }

        public static bool IsSingleBitSet(this TypeOrNamespaceUsageInfo usageInfo)
            => usageInfo != TypeOrNamespaceUsageInfo.None && (usageInfo & (usageInfo - 1)) == 0;

        public static ImmutableArray<string> ToLocalizableValues(this TypeOrNamespaceUsageInfo usageInfo)
        {
            if (usageInfo == TypeOrNamespaceUsageInfo.None)
            {
                return ImmutableArray<string>.Empty;
            }

            var builder = ArrayBuilder<string>.GetInstance();
            foreach (TypeOrNamespaceUsageInfo value in Enum.GetValues(typeof(TypeOrNamespaceUsageInfo)))
            {
                if (value.IsSingleBitSet() && (usageInfo & value) != 0)
                {
                    builder.Add(value.ToLocalizableString());
                }
            }

            return builder.ToImmutableAndFree();
        }
    }
}
