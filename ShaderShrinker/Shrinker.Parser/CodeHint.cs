// -----------------------------------------------------------------------
//  <copyright file="CodeHint.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

namespace Shrinker.Parser
{
    public class CodeHint
    {
        public string Item { get; }
        public string Suggestion { get; }

        public CodeHint(string item, string suggestion)
        {
            Item = item;
            Suggestion = suggestion;
        }
    }
}