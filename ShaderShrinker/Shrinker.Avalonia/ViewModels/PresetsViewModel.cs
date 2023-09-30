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
using AvaloniaEdit.Utils;
using Newtonsoft.Json;
using ReactiveUI;
using Shrinker.Parser;

namespace Shrinker.Avalonia.ViewModels;

public class PresetsViewModel : ReactiveObject
{
    private FileInfo m_selectedPreset;
    private ObservableCollection<FileInfo> m_presets;

    public IEnumerable<FileInfo> All
    {
        get
        {
            if (m_presets == null)
            {
                m_presets = new ObservableCollection<FileInfo>();

                var presetsDir = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "Presets"));
                if (presetsDir.Exists)
                    m_presets.AddRange(presetsDir.EnumerateFiles());
            }

            return m_presets;
        }
    }

    public FileInfo Selected
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

    // todo - support custom options.
    public CustomOptions GetOptions() =>
        JsonConvert.DeserializeObject<CustomOptions>(File.ReadAllText(Selected.FullName));
}