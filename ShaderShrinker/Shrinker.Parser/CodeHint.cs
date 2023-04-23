// -----------------------------------------------------------------------
//  <copyright file="CodeHint.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

namespace Shrinker.Parser
{
    public class CodeHint
    {
        public string Item { get; }
        public string Suggestion { get; }

        protected CodeHint(string item, string suggestion)
        {
            Item = item;
            Suggestion = suggestion;
        }

        public override string ToString() => $"{Item}|{Suggestion}";

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            var other = (CodeHint)obj;
            return Item == other.Item && Suggestion == other.Suggestion;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Item != null ? Item.GetHashCode() : 0) * 397) ^ (Suggestion != null ? Suggestion.GetHashCode() : 0);
            }
        }
    }
}