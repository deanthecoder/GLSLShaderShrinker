using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Avalonia.Threading;
using AvaloniaEdit.Utils;
using DialogHostAvalonia;
using Material.Styles.Controls;
using Material.Styles.Models;
using ReactiveUI;
using Shrinker.Avalonia.Commands;
using Shrinker.Avalonia.Models;
using Shrinker.Avalonia.Shadertoy;
using Shrinker.Parser;
using TextCopy;

namespace Shrinker.Avalonia.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    private readonly ObservableCollection<CombinedDiff> m_diffs = new();
    private ICommand m_importGlslClipboardCommand;
    private ICommand m_importGlslFileCommand;
    private CommandBase m_importGlslShadertoyCommand;
    private string m_shadertoyId;
    private RelayCommand m_shrinkCommand;
    private bool m_isInstructionGlsl;

    public IEnumerable<CombinedDiff> Diffs => m_diffs;
    public ICommand LaunchProjectPage { get; } = new RelayCommand(_ => Process.Start(new ProcessStartInfo("https://github.com/deanthecoder/GLSLShaderShrinker") { UseShellExecute = true }));
    public ICommand ImportGlslClipboardCommand => m_importGlslClipboardCommand ??= new ClipboardCommand(ImportGlslFromClipboard);
    public ICommand ImportGlslFileCommand
    {
        get
        {
            if (m_importGlslFileCommand == null)
            {
                var fileOpenCommand = new FileOpenCommand("Load GLSL file", "GLSL Files", new[] { "*.txt", "*.glsl", "*.*" });
                fileOpenCommand.FileSelected += (_, fileInfo) => ImportGlslFromFile(fileInfo);
                m_importGlslFileCommand = fileOpenCommand;
            }

            return m_importGlslFileCommand;
        }
    }

    public CommandBase ImportGlslShadertoyCommand =>
        m_importGlslShadertoyCommand ??= new RelayCommand(ImportGlslFromShadertoy, () => !string.IsNullOrWhiteSpace(ShadertoyId));
    
    public ICommand ShrinkCommand
    {
        get
        {
            if (m_shrinkCommand == null)
            {
                m_shrinkCommand = new RelayCommand(o => ShrinkGlsl((string)o), () => m_diffs.Any() && !IsInstructionGlsl);
                m_diffs.CollectionChanged += (_, _) => m_shrinkCommand.RaiseCanExecuteChanged();
                PropertyChanged += (_, args) =>
                {
                    if (args.PropertyName == nameof(IsInstructionGlsl))
                        m_shrinkCommand.RaiseCanExecuteChanged();
                };
            }
            
            return m_shrinkCommand;
        }
    }

    public string ShadertoyId
    {
        get => m_shadertoyId;
        set
        {
            this.RaiseAndSetIfChanged(ref m_shadertoyId, value);
            ImportGlslShadertoyCommand.RaiseCanExecuteChanged();
        }
    }

    public MainWindowViewModel()
    {
        ImportGlslFromString("You can use paste GLSL from the clipboard,\nor use the import buttons on the left.");
        IsInstructionGlsl = true;
    }

    public bool IsInstructionGlsl
    {
        get => m_isInstructionGlsl;
        set => this.RaiseAndSetIfChanged(ref m_isInstructionGlsl, value);
    }

    private void ImportGlslFromClipboard(object o) => ImportGlslFromString(ClipboardService.GetText());

    private void ImportGlslFromFile(FileInfo file)
    {
        if (file.Exists)
            ImportGlslFromString(File.ReadAllText(file.FullName));
    }

    private async void ImportGlslFromShadertoy(object o)
    {
        try
        {
            var id = ShadertoyId;
            var glsl = await ShadertoyImporter.ImportAsync(id);
            if (glsl == null)
                return; // Nothing to do.
            
            if (string.IsNullOrWhiteSpace(glsl))
            {
                PostSnackbarMessage("Failed to import GLSL.");
                return;
            }

            ImportGlslFromString(glsl);
        }
        catch (Exception ex)
        {
            PostSnackbarMessage($"Failed to import GLSL: {ex.Message}");
        }
        finally
        {
            DialogHost.Close(null);
        }
    }

    private static void PostSnackbarMessage(object message) =>
        SnackbarHost.Post(new SnackbarModel(message, TimeSpan.FromSeconds(3.0)), null, DispatcherPriority.Normal);

    private void ImportGlslFromString(string glsl)
    {
        SetGlsl(glsl, string.Empty);
        IsInstructionGlsl = false;
    }

    private void ShrinkGlsl(string level)
    {
        var glsl = string.Join("\n", m_diffs.Select(o => o.LeftDiff?.Text).Where(o => o != null));
        
        var lexer = new Lexer.Lexer();
        lexer.Load(glsl);
        var rootNode = new Parser.Parser(lexer).Parse();
        
        var options = level switch
        {
            "Max" => CustomOptions.All(),
            "Min" => CustomOptions.None(),
            // todo "Custom" => CustomOptions,
            _ => throw new InvalidOperationException($"Unknown optimization level: {level}")
        };

        var optimizedCode = rootNode.Simplify(options).ToCode();
        // todo (optimizedCode.GetCodeCharCount(), optimizedCode, rootNode.GetHints().ToList());
        SetGlsl(glsl, optimizedCode);
    }

    private void SetGlsl(string glsl, string processedGlsl)
    {
        var diffs = DiffCreator.CreateDiffs(glsl.Trim(), processedGlsl);
        m_diffs.Clear();
        m_diffs.AddRange(diffs);
    }
}
