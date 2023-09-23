// -----------------------------------------------------------------------
//  <copyright file="ShadertoyImporter.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shrinker.Avalonia.Models;

namespace Shrinker.Avalonia.Shadertoy;

public static class ShadertoyImporter
{
    /// <summary>
    /// Returns null if nothing to do, empty string on failure, or valid GLSL.
    /// </summary>
    public static async Task<string> ImportAsync(string id)
    {
        id = id?.Trim().Trim('/');
        if (id != null)
        {
            var slashIndex = id.LastIndexOf('/');
            if (slashIndex > 0)
                id = id.Substring(slashIndex + 1);
        }

        if (string.IsNullOrWhiteSpace(id))
            return null; // Nothing to do.

        using var httpClient = new HttpClient();
        var json = await httpClient.GetStringAsync($"https://www.shadertoy.com/api/v1/shaders/{id}?key=BtntM4");

        var shaderData = JsonConvert.DeserializeObject<Root>(json);
        return shaderData?.Shader?.renderpass.LastOrDefault()?.code ?? string.Empty;
    }
}