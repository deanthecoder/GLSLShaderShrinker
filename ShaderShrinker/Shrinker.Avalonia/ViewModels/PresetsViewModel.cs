// -----------------------------------------------------------------------
//  <copyright file="PresetsViewModel.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using DynamicData;
using Newtonsoft.Json;
using ReactiveUI;
using Shrinker.Avalonia.Models;
using Shrinker.Parser;

namespace Shrinker.Avalonia.ViewModels;

public class PresetsViewModel : ReactiveObject, IDisposable
{
    private NameAndFileInfo m_selectedPreset;
    private ObservableCollection<NameAndFileInfo> m_presets;
    private TempFile m_customPresetFile;
    
    public CustomOptions CustomOptions { get; private set; }

    public IEnumerable<NameAndFileInfo> All
    {
        get
        {
            if (m_presets == null)
            {
                m_presets = new ObservableCollection<NameAndFileInfo>();
                
                // Load Presets from disk.
                var presets = new List<NameAndFileInfo>();
                var presetsDir = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "Presets"));
                if (presetsDir.Exists)
                    presets.AddRange(presetsDir.EnumerateFiles().Select(o => new NameAndFileInfo(o)));

                // Add the custom preset.
                m_customPresetFile = new TempFile(UserSettings.Instance.CustomPresetJson);
                CustomOptions = JsonConvert.DeserializeObject<CustomOptions>(UserSettings.Instance.CustomPresetJson);
                presets.Add(new NameAndFileInfo("Custom", m_customPresetFile));
                
                m_presets.AddRange(presets.OrderBy(o => o.Name));
            }

            return m_presets;
        }
    }

    public NameAndFileInfo Selected
    {
        get
        {
            if (m_selectedPreset == null)
            {
                m_selectedPreset = All.FirstOrDefault(o => o.Name.Equals(UserSettings.Instance.Preset));
                m_selectedPreset ??= All.FirstOrDefault(o => o.Name.Equals("Maximum"));
                m_selectedPreset ??= All.FirstOrDefault();
            }

            return m_selectedPreset;
        }
        
        set => UserSettings.Instance.Preset = this.RaiseAndSetIfChanged(ref m_selectedPreset, value).Name;
    }

    public CustomOptions GetOptions() =>
        Selected.Name == "Custom" ? CustomOptions : JsonConvert.DeserializeObject<CustomOptions>(File.ReadAllText(Selected.File.FullName));

    public void Dispose()
    {
        UserSettings.Instance.CustomPresetJson = JsonConvert.SerializeObject(CustomOptions, Formatting.Indented);
        m_customPresetFile?.Dispose();
    }
}