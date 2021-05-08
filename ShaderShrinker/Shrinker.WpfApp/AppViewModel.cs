// -----------------------------------------------------------------------
//  <copyright file="AppViewModel.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using Shrinker.Lexer;
using Shrinker.Parser;
using Shrinker.WpfApp.ShadertoyApi;

namespace Shrinker.WpfApp
{
    public class AppViewModel : ViewModelBase
    {
        private int m_originalSize;
        private int m_optimizedSize;
        private SyntaxNode m_optimizedRoot;
        private RelayCommand m_loadFromClipboardCommand;
        private RelayCommand m_loadFromFileCommand;
        private RelayCommand m_loadFromShadertoyCommand;
        private RelayCommand m_saveToClipboardCommand;
        private RelayCommand m_saveToFileCommand;
        private RelayCommand m_shrinkCommand;
        private RelayCommand m_customOptionsAcceptedCommand;
        private string m_optimizedCode;
        private bool m_showProgress;
        private string m_originalCode;

        public event EventHandler<(string originalCode, string newCode)> GlslLoaded;

        public ICommand OnLoadFromClipboardCommand => m_loadFromClipboardCommand ??= new RelayCommand(LoadGlslFromClipboard, _ => Clipboard.ContainsText());
        public ICommand OnLoadFromFileCommand => m_loadFromFileCommand ??= new RelayCommand(LoadGlslFromFile);
        public ICommand OnLoadFromShadertoyCommand => m_loadFromShadertoyCommand ??= new RelayCommand(LoadGlslFromShadertoy);
        public ICommand OnSaveToClipboardCommand => m_saveToClipboardCommand ??= new RelayCommand(SaveGlslToClipboard, _ => m_optimizedRoot != null);
        public ICommand OnSaveToFileCommand => m_saveToFileCommand ??= new RelayCommand(SaveGlslToFile, _ => m_optimizedRoot != null);
        public ICommand OnShrinkCommand => m_shrinkCommand ??= new RelayCommand(Shrink, _ => m_optimizedRoot != null);
        public ICommand OnCustomOptionsAcceptedCommand => m_customOptionsAcceptedCommand ??= new RelayCommand(AcceptCustomOptions);

        public CustomOptions CustomOptions { get; }

        public string ShadertoyShaderId { get; set; }

        private string OptimizedCode
        {
            get => m_optimizedCode;
            set
            {
                if (m_optimizedCode == value)
                    return;

                m_optimizedCode = value;
                OnPropertyChanged();
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
                OnPropertyChanged(nameof(DeltaSize));
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

        public AppViewModel()
        {
            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.CustomOptions))
                CustomOptions = JsonConvert.DeserializeObject<CustomOptions>(Properties.Settings.Default.CustomOptions);
            CustomOptions ??= new CustomOptions();
        }

        private void LoadGlslFromClipboard(object obj)
        {
            var glsl = Clipboard.GetText();
            if (!string.IsNullOrEmpty(glsl))
                LoadGlslFromStringAsync(glsl);
        }

        private void SaveGlslToClipboard(object obj)
        {
            var glsl = m_optimizedRoot?.ToCode();
            if (string.IsNullOrEmpty(glsl))
                return;

            Clipboard.SetText(glsl.WithAppMessage());
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
            var id = ShadertoyShaderId.Trim();
            if (string.IsNullOrWhiteSpace(id) || id.Length != 6)
                return;

            using (var wc = new WebClient())
            {
                var json = wc.DownloadString($"https://www.shadertoy.com/api/v1/shaders/{id}?key=BtntM4");

                var shaderData = JsonConvert.DeserializeObject<Root>(json);
                var shaderCode = shaderData?.Shader?.renderpass.LastOrDefault()?.code;
                if (!string.IsNullOrWhiteSpace(shaderCode))
                    LoadGlslFromStringAsync(shaderCode);
            }
        }

        private void SaveGlslToFile(object obj)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Save GLSL file",
                Filter = "GLSL Files|*.txt;*.glsl|All Files|*.*"
            };
            if (dialog.ShowDialog() != true)
                return;

            File.WriteAllText(dialog.FileName, OptimizedCode.WithAppMessage());
            MyMessageQueue.Enqueue("GLSL saved to file");
        }

        private void LoadGlslFromStringAsync(string glsl)
        {
            m_originalCode = glsl;
            Shrink("Max");
        }

        private void AcceptCustomOptions(object obj)
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
            Shrink("Custom");
        }

        private async void Shrink(object level)
        {
            try
            {
                ShowProgress = true;

                var (originalSize, optimizedSize, optimizedCode) = await Task.Run(() => LoadGlslFromString(m_originalCode, (string)level));

                OriginalSize = originalSize;
                OptimizedCode = optimizedCode;
                OptimizedSize = optimizedSize;

                GlslLoaded?.Invoke(this, (m_originalCode, OptimizedCode));
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
            }

            m_optimizedRoot = null;
            OptimizedCode = null;
            OptimizedSize = 0;
            GlslLoaded?.Invoke(this, (m_originalCode, string.Empty));

            SaveCustomOptions();
        }

        private (int originalSize, int optimizedSize, string optimizedCode) LoadGlslFromString(string glsl, string level)
        {
            var lexer = new Lexer.Lexer();
            lexer.Load(glsl);
            var rootNode = new Parser.Parser(lexer).Parse();

            var options = level switch
            {
                "Max" => new CustomOptions(),
                "Min" => CustomOptions.Disabled(), // todo
                "Custom" => CustomOptions,
                _ => throw new InvalidOperationException($"Unknown optimization level: {level}")
            };

            m_optimizedRoot = rootNode.Simplify(options);

            var optimizedCode = m_optimizedRoot.ToCode();
            return (glsl.GetCodeCharCount(), optimizedCode.GetCodeCharCount(), optimizedCode);
        }

        public void SaveCustomOptions()
        {
            Properties.Settings.Default.CustomOptions = JsonConvert.SerializeObject(CustomOptions);
            Properties.Settings.Default.Save();
        }
    }
}