// -----------------------------------------------------------------------
//  <copyright file="AppViewModel.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.Win32;
using Newtonsoft.Json;
using Shrinker.Lexer;
using Shrinker.Parser;
using Shrinker.Parser.SyntaxNodes;
using Shrinker.WpfApp.Properties;
using Shrinker.WpfApp.ShadertoyApi;

namespace Shrinker.WpfApp
{
    public class AppViewModel : ViewModelBase
    {
        private int m_optimizedSize;
        private SyntaxNode m_optimizedRoot;
        private RelayCommand m_loadFromClipboardCommand;
        private RelayCommand m_loadFromFileCommand;
        private RelayCommand m_loadFromShadertoyCommand;
        private RelayCommand m_saveToClipboardCommand;
        private RelayCommand m_saveToFileCommand;
        private RelayCommand m_shrinkCommand;
        private RelayCommand m_customOptionsAcceptedCommand;
        private RelayCommand m_hintsAcceptedCommand;
        private string m_optimizedCode;
        private bool m_showProgress;
        private string m_originalCode;
        private int m_originalSize;
        private FileInfo m_selectedPreset;
        private CustomOptions m_customOptions = CustomOptions.All();
        private List<FileInfo> m_presets;
        private readonly FileInfo m_customPreset = new FileInfo("Custom");
        private bool m_isOutputGlsl = true;

        public event EventHandler<(string originalCode, string newCode)> GlslLoaded;

        public ICommand OnLoadFromClipboardCommand => m_loadFromClipboardCommand ??= new RelayCommand(LoadGlslFromClipboard, _ => Clipboard.ContainsText());
        public ICommand OnLoadFromFileCommand => m_loadFromFileCommand ??= new RelayCommand(LoadGlslFromFile);
        public ICommand OnLoadFromShadertoyCommand => m_loadFromShadertoyCommand ??= new RelayCommand(LoadGlslFromShadertoy);
        public ICommand OnShrinkCommand => m_shrinkCommand ??= new RelayCommand(ShrinkAsync, _ => IsSrcFileOpen);
        public ICommand OnSaveToClipboardCommand => m_saveToClipboardCommand ??= new RelayCommand(SaveGlslToClipboard, _ => IsOptimizedFileOpen);
        public ICommand OnSaveToFileCommand => m_saveToFileCommand ??= new RelayCommand(SaveGlslToFile, _ => IsOptimizedFileOpen);
        public ICommand OnCustomOptionsAcceptedCommand => m_customOptionsAcceptedCommand ??= new RelayCommand(AcceptCustomOptions);
        public ICommand OnHintsAcceptedCommand => m_hintsAcceptedCommand ??= new RelayCommand(AcceptHints);

        public CustomOptions CustomOptions => m_customOptions;

        public string ShadertoyShaderId { get; set; }

        private string OriginalCode
        {
            get => m_originalCode;
            set
            {
                if (m_originalCode == value)
                    return;
                m_originalCode = value;
                OriginalSize = value?.GetCodeCharCount() ?? 0;
                OnPropertyChanged(nameof(IsSrcFileOpen));
                OnPropertyChanged();
            }
        }

        private string OptimizedCode
        {
            get => m_optimizedCode;
            set
            {
                if (m_optimizedCode == value)
                    return;

                m_optimizedCode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsOptimizedFileOpen));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public int OriginalSize
        {
            get => m_originalSize;
            private set
            {
                if (m_originalSize == value)
                    return;
                m_originalSize = value;
                OnPropertyChanged();
            }
        }

        public int OptimizedSize
        {
            get => m_optimizedSize;
            private set
            {
                if (m_optimizedSize == value)
                    return;
                m_optimizedSize = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DeltaSize));
            }
        }

        public int DeltaSize => OptimizedSize - OriginalSize;

        public bool ShowProgress
        {
            get => m_showProgress;
            set
            {
                if (m_showProgress == value)
                    return;
                m_showProgress = value;
                OnPropertyChanged();
            }
        }

        public SnackbarMessageQueue MyMessageQueue { get; } = new SnackbarMessageQueue();

        public bool IsSrcFileOpen => !string.IsNullOrWhiteSpace(OriginalCode);
        public bool IsOptimizedFileOpen => !string.IsNullOrWhiteSpace(OptimizedCode);

        public string AppTitle
        {
            get
            {
                var assemblyInfo = new AssemblyInfo(Assembly.GetEntryAssembly());
                return $"{assemblyInfo.ProductName} v{assemblyInfo.Version}";
            }
        }

        public ObservableCollection<CodeHint> Hints { get; } = new ObservableCollection<CodeHint>();

        public IEnumerable<FileInfo> Presets
        {
            get
            {
                if (m_presets == null)
                {
                    var presetsDir = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "Presets"));
                    m_presets = presetsDir.Exists ? presetsDir.EnumerateFiles().ToList() : new List<FileInfo>();

                    // Add 'special' case.
                    m_presets.Add(m_customPreset);
                }

                return m_presets;
            }
        }

        public FileInfo SelectedPreset
        {
            get => m_selectedPreset ??= Presets.FirstOrDefault(o => o.Name == Settings.Default.MostRecentPreset) ?? Presets.FirstOrDefault();
            set
            {
                if (m_selectedPreset == value)
                    return;

                m_selectedPreset = value;
                Settings.Default.MostRecentPreset = value.Name;
                Settings.Default.Save();

                LoadOptionsFromPreset();
            }
        }
        public bool IsOutputGlsl
        {
            get => m_isOutputGlsl;
            set
            {
                m_isOutputGlsl = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsOutputC));
            }
        }

        public bool IsOutputC
        {
            get => !IsOutputGlsl;
            set => IsOutputGlsl = !value;
        }

        public AppViewModel()
        {
            LoadOptionsFromPreset();
        }

        private void LoadOptionsFromPreset()
        {
            var options = SelectedPreset == m_customPreset ? Settings.Default.CustomOptions : File.ReadAllText(SelectedPreset.FullName);
            if (string.IsNullOrEmpty(options))
                m_customOptions = CustomOptions.All();
            else
                JsonConvert.PopulateObject(options, m_customOptions);

            OnPropertyChanged(nameof(CustomOptions));
        }

        private void LoadGlslFromClipboard(object obj)
        {
            var glsl = Clipboard.GetText();
            if (!string.IsNullOrEmpty(glsl))
                LoadGlslFromStringAsync(glsl);
        }

        private void SaveGlslToClipboard(object obj)
        {
            var glsl = OptimizedCode;
            if (string.IsNullOrEmpty(glsl))
                return;

            Clipboard.SetText(GetShaderOutputText(glsl));
            MyMessageQueue.Enqueue("GLSL saved to clipboard");
        }

        private void LoadGlslFromFile(object obj)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Load GLSL file",
                CheckFileExists = true,
                Filter = "GLSL Files|*.txt;*.glsl|All Files|*.*"
            };
            if (dialog.ShowDialog() != true)
                return;

            LoadGlslFromStringAsync(File.ReadAllText(dialog.FileName));
        }

        private void LoadGlslFromShadertoy(object obj)
        {
            var id = ShadertoyShaderId?.Trim();
            if (string.IsNullOrWhiteSpace(id) || id.Length != 6)
                return;

            using (new BusyCursor())
            using (var wc = new WebClient())
            {
                var json = wc.DownloadString($"https://www.shadertoy.com/api/v1/shaders/{id}?key=BtntM4");

                var shaderData = JsonConvert.DeserializeObject<Root>(json);
                var shaderCode = shaderData?.Shader?.renderpass.LastOrDefault()?.code;
                if (!string.IsNullOrWhiteSpace(shaderCode))
                    LoadGlslFromStringAsync(shaderCode);
            }

            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        private void SaveGlslToFile(object obj)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Save GLSL file",
                Filter = "GLSL Files|*.txt;*.glsl;*.c;*.cpp;*.h;*.inl|All Files|*.*"
            };
            if (dialog.ShowDialog() != true)
                return;

            File.WriteAllText(dialog.FileName, GetShaderOutputText(OptimizedCode));
            MyMessageQueue.Enqueue("GLSL saved to file");
        }

        private void LoadGlslFromStringAsync(string glsl)
        {
            Hints.Clear();
            OriginalCode = glsl;
            OptimizedCode = string.Empty;
            OptimizedSize = 0;
            GlslLoaded?.Invoke(this, (OriginalCode, OptimizedCode));
        }

        private void AcceptCustomOptions(object obj)
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
            ShrinkAsync("Custom");
        }

        private static void AcceptHints(object obj) =>
            DialogHost.CloseDialogCommand.Execute(null, null);

        private async void ShrinkAsync(object levelParam = null)
        {
            try
            {
                ShowProgress = true;

                using (new BusyCursor())
                {
                    var (optimizedSize, optimizedCode, hints) = await Task.Run(() =>
                    {
                        Thread.Sleep(500);
                        return Shrink(OriginalCode, (string)levelParam);
                    });

                    OptimizedCode = optimizedCode;
                    OptimizedSize = optimizedSize;

                    Hints.Clear();
                    foreach (var hint in hints.OrderBy(o => o.Item))
                        Hints.Add(hint);
                }

                GlslLoaded?.Invoke(this, (OriginalCode, OptimizedCode));
                return;
            }
            catch (SyntaxErrorException e)
            {
                MyMessageQueue.Enqueue($"Failed to import GLSL: {e.Message}");
            }
            catch
            {
                MyMessageQueue.Enqueue("Failed to import GLSL.");
            }
            finally
            {
                ShowProgress = false;
                SaveCustomOptions();
            }

            m_optimizedRoot = null;
            OptimizedCode = null;
            OptimizedSize = 0;
            GlslLoaded?.Invoke(this, (OriginalCode, string.Empty));
        }

        private (int optimizedSize, string optimizedCode, IEnumerable<CodeHint> hints) Shrink(string glsl, string level)
        {
            var lexer = new Lexer.Lexer();
            lexer.Load(glsl);
            var rootNode = new Parser.Parser(lexer).Parse();

            var options = level switch
            {
                "Max" => CustomOptions.All(),
                "Min" => CustomOptions.None(),
                "Custom" => CustomOptions,
                _ => throw new InvalidOperationException($"Unknown optimization level: {level}")
            };

            m_optimizedRoot = rootNode.Simplify(options);

            var optimizedCode = m_optimizedRoot.ToCode();
            return (optimizedCode.GetCodeCharCount(), optimizedCode, rootNode.GetHints().ToList());
        }

        public void SaveCustomOptions()
        {
            if (Settings.Default.MostRecentPreset != "Custom")
                return;

            Settings.Default.CustomOptions = JsonConvert.SerializeObject(CustomOptions);
            Settings.Default.Save();
        }

        private string GetShaderOutputText(string glsl) =>
            IsOutputGlsl ? glsl.WithAppMessage(OriginalSize, OptimizedSize) : glsl.ToCCode(OriginalSize, OptimizedSize);
    }
}