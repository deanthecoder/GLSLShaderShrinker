// -----------------------------------------------------------------------
//  <copyright file="UnitTestBase.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using NUnit.Framework;

namespace UnitTests
{
    [Parallelizable(ParallelScope.All)] // All our unit tests are completely independent, so lets supercharge them...
    public abstract class UnitTestBase
    {
    }
}