// -----------------------------------------------------------------------
//  <copyright file="ShadertoyApi.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System.Collections.Generic;

// ReSharper disable All

namespace Shrinker.WpfApp.ShadertoyApi
{
    public class Info
    {
        public string id { get; set; }
        public string date { get; set; }
        public int viewed { get; set; }
        public string name { get; set; }
        public string username { get; set; }
        public string description { get; set; }
        public int likes { get; set; }
        public int published { get; set; }
        public int flags { get; set; }
        public int usePreview { get; set; }
        public List<string> tags { get; set; }
        public int hasliked { get; set; }
    }

    public class Output
    {
        public int id { get; set; }
        public int channel { get; set; }
    }

    public class Renderpass
    {
        public List<object> inputs { get; set; }
        public List<Output> outputs { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string type { get; set; }
    }

    public class Shader
    {
        public string ver { get; set; }
        public Info info { get; set; }
        public List<Renderpass> renderpass { get; set; }
    }

    public class Root
    {
        public Shader Shader { get; set; }
        public string Error { get; set; }

        public Root(Shader shader, string error)
        {
            Shader = shader;
            Error = error;
        }
    }
}