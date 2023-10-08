using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
using Shrinker.Lexer;
using Shrinker.Parser;
using TextCopy;

namespace Shrinker.Avalonia.ViewModels;

public class MainWindowViewModel : ReactiveObject, IDisposable
{
    private ICommand m_importGlslClipboardCommand;
    private ICommand m_importGlslFileCommand;
    private CommandBase m_importGlslShadertoyCommand;
    private string m_shadertoyId;
    private CommandBase m_shrinkCommand;
    private CommandBase m_exportGlslClipboardCommand;
    private bool m_isInstructionGlsl; // True if the 'instructions' are being displayed.
    private CommandBase m_exportGlslFileCommand;
    private int m_originalSize;
    private int m_processedSize;
    private bool m_isBusy;

    public PresetsViewModel Presets { get; } = new();
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
                m_shrinkCommand = new RelayCommand(_ => ShrinkGlsl(), () => Diffs.Any() && !IsInstructionGlsl);
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
        get => m_shadertoyId ??= UserSettings.Instance.ShadertoyId;
        set
        {
            this.RaiseAndSetIfChanged(ref m_shadertoyId, value);
            ImportGlslShadertoyCommand.RaiseCanExecuteChanged();
        }
    }

    public string ModifierKeyString => OperatingSystem.IsMacOS() ? "\u2318" : "Ctrl";

    /// <summary>
    /// Whether to export as raw GLSL, or encode as C-style code.
    /// </summary>
    public bool IsOutputGlsl
    {
        get => UserSettings.Instance.OutputAsGlsl;
        set
        {
            if (UserSettings.Instance.OutputAsGlsl == value)
                return;
            UserSettings.Instance.OutputAsGlsl = value;
            this.RaisePropertyChanged();
        }
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
            UserSettings.Instance.ShadertoyId = ShadertoyId;
            var glsl = await ShadertoyImporter.ImportAsync(ShadertoyId);
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

    private async void ShrinkGlsl()
    {
        try
        {
            IsBusy = true;

            await Task.Run(
                     () =>
                     {
                         var glsl = Diffs.GetAllLeftText();

                         var lexer = new Lexer.Lexer();
                         lexer.Load(glsl);
                         var rootNode = new Parser.Parser(lexer).Parse();

                         var options = Presets.GetOptions();
                         var processedGlsl = rootNode.Simplify(options).ToCode();
                         SetGlsl(glsl, processedGlsl);

                         Hints.Clear();
                         Hints.AddRange(rootNode
                                            .GetHints()
                                            .OrderBy(o => o.Priority)
                                            .ThenBy(o => o.Suggestion)
                                            .ThenBy(o => o.Item));
                     });
        }
        catch (Exception ex)
        {
            PostSnackbarMessage($"Failed to parse GLSL: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void SetGlsl(string glsl, string processedGlsl)
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            // Dispatch the call to the UI thread.
            Dispatcher.UIThread.InvokeAsync(() => SetGlsl(glsl, processedGlsl));
            return;
        }
        
        Hints.Clear();
        Diffs.ReplaceAll(DiffCreator.CreateDiffs(glsl.Trim(), processedGlsl));
        
        OriginalSize = glsl.GetCodeCharCount();
        ProcessedSize = processedGlsl.GetCodeCharCount();
    }

    private void ExportGlslToClipboard(object parameter)
    {
        var glsl = Diffs.GetAllRightText();
        ClipboardService.SetTextAsync(IsOutputGlsl ? glsl : glsl.ToCCode()); 
        PostSnackbarMessage("Copied to clipboard.");
    }

    private void ExportGlslToFile(FileInfo targetFile)
    {
        using var fileStream = targetFile.OpenWrite();
        using var writer = new StreamWriter(fileStream);
        
        var glsl = Diffs.GetAllRightText();
        writer.Write(IsOutputGlsl ? glsl : glsl.ToCCode());
    }

    public int OriginalSize
    {
        get => m_originalSize;
        set => this.RaiseAndSetIfChanged(ref m_originalSize, value);
    }

    public int ProcessedSize
    {
        get => m_processedSize;
        set => this.RaiseAndSetIfChanged(ref m_processedSize, value);
    }

    public bool IsBusy
    {
        get => m_isBusy;
        set => this.RaiseAndSetIfChanged(ref m_isBusy, value);
    }

    public void Dispose() => Presets?.Dispose();
}