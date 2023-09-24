using System;
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
    private ICommand m_importGlslClipboardCommand;
    private ICommand m_importGlslFileCommand;
    private CommandBase m_importGlslShadertoyCommand;
    private string m_shadertoyId;
    private CommandBase m_shrinkCommand;
    private CommandBase m_exportGlslClipboardCommand;
    private bool m_isInstructionGlsl; // True if the 'instructions' are being displayed.
    private CommandBase m_exportGlslFileCommand;
    private bool m_isOutputGlsl = true;

    public DiffCollection Diffs { get; } = new();
    public ObservableCollection<CodeHint> Hints { get; } = new();
    public int HintCount => Hints.Count;
    public bool HasHints => Hints.Any();

    public ICommand LaunchProjectPage { get; } = new RelayCommand(_ => Process.Start(new ProcessStartInfo("https://github.com/deanthecoder/GLSLShaderShrinker") { UseShellExecute = true }));
    public ICommand ImportGlslClipboardCommand => m_importGlslClipboardCommand ??= new ClipboardCommand(ImportGlslFromClipboard);
    public ICommand ImportGlslFileCommand
    {
        get
        {
            if (m_importGlslFileCommand == null)
            {
                var openCommand = new FileOpenCommand("Load GLSL file", "GLSL Files", new[] { "*.txt", "*.glsl", "*.*" });
                openCommand.FileSelected += (_, fileInfo) => ImportGlslFromFile(fileInfo);
                m_importGlslFileCommand = openCommand;
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
                m_shrinkCommand = new RelayCommand(o => ShrinkGlsl((string)o), () => Diffs.Any() && !IsInstructionGlsl);
                PropertyChanged += (_, args) =>
                {
                    if (args.PropertyName == nameof(IsInstructionGlsl))
                        m_shrinkCommand.RaiseCanExecuteChanged();
                };
            }
            
            return m_shrinkCommand;
        }
    }

    public ICommand ExportGlslClipboardCommand =>
        m_exportGlslClipboardCommand ??= new RelayCommand(ExportGlslToClipboard, () => Diffs.HasRightContent());

    public ICommand ExportGlslFileCommand
    {
        get
        {
            if (m_exportGlslFileCommand == null)
            {
                var saveCommand = new FileSaveCommand("Save file", "Files", new[] { "*.glsl", "*.c", "*.cpp", "*.h", "*.inl", "*.txt" }, "output.glsl", () => Diffs.HasRightContent());
                saveCommand.FileSelected += (_, fileInfo) => ExportGlslToFile(fileInfo);
                m_exportGlslFileCommand = saveCommand;
            }

            return m_exportGlslFileCommand;
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

    public string ModifierKeyString => OperatingSystem.IsMacOS() ? "\u2318" : "Ctrl";

    public bool IsOutputGlsl
    {
        get => m_isOutputGlsl;
        set => this.RaiseAndSetIfChanged(ref m_isOutputGlsl, value);
    }

    public MainWindowViewModel()
    {
        ImportGlslFromString($"You can use '{ModifierKeyString} + V' to paste GLSL from the clipboard,\nor use the import buttons on the left.");
        IsInstructionGlsl = true;

        Diffs.CollectionChanged += (_, _) =>
        {
            m_shrinkCommand.RaiseCanExecuteChanged();
            m_exportGlslClipboardCommand.RaiseCanExecuteChanged();
            m_exportGlslFileCommand.RaiseCanExecuteChanged();
        };
        Hints.CollectionChanged += (_, _) =>
        {
            this.RaisePropertyChanged(nameof(HintCount));
            this.RaisePropertyChanged(nameof(HasHints));
        };
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
        var glsl = Diffs.GetAllLeftText();
        
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

        Hints.Clear();
        Hints.AddRange(rootNode.GetHints().OrderBy(o => o.Item));
    }

    private void SetGlsl(string glsl, string processedGlsl)
    {
        Hints.Clear();
        Diffs.ReplaceAll(DiffCreator.CreateDiffs(glsl.Trim(), processedGlsl));
    }

    private void ExportGlslToClipboard(object parameter)
    {
        var glsl = Diffs.GetAllRightText();
        ClipboardService.SetTextAsync(IsOutputGlsl ? glsl : glsl.ToCCode()); // todo - size.
    }

    private void ExportGlslToFile(FileInfo targetFile)
    {
        using var fileStream = targetFile.OpenWrite();
        using var writer = new StreamWriter(fileStream);
        
        var glsl = Diffs.GetAllRightText();
        writer.Write(IsOutputGlsl ? glsl : glsl.ToCCode()); // todo - size.
    }
}
