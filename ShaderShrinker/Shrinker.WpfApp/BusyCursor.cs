// -----------------------------------------------------------------------
//  <copyright file="BusyCursor.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Windows.Input;

namespace Shrinker.WpfApp
{
    internal class BusyCursor : IDisposable
    {
        private readonly Cursor m_oldCursor;

        public BusyCursor()
        {
            m_oldCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.Wait;
        }

        public void Dispose()
        {
            Mouse.OverrideCursor = m_oldCursor;
        }
    }
}