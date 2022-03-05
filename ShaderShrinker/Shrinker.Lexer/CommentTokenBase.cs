﻿// -----------------------------------------------------------------------
//  <copyright file="CommentTokenBase.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

namespace Shrinker.Lexer
{
    public abstract class CommentTokenBase : Token
    {
        public bool IsAppendedToLine { get; protected set; }
    }
}