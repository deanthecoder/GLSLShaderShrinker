using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using AvaloniaEdit.Utils;
using ReactiveUI;
using Shrinker.Avalonia.Commands;
using Shrinker.Avalonia.Models;
using TextCopy;

namespace Shrinker.Avalonia.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    private readonly ObservableCollection<CombinedDiff> m_diffs = new();
    private ICommand m_importGlslClipboardCommand;
    private ICommand m_importGlslFileCommand;

    public IEnumerable<CombinedDiff> Diffs => m_diffs;
    public ICommand LaunchProjectPage { get; } = new RelayCommand(() => Process.Start(new ProcessStartInfo("https://github.com/deanthecoder/GLSLShaderShrinker") { UseShellExecute = true }));
    public ICommand ImportGlslClipboardCommand => m_importGlslClipboardCommand ??= new ClipboardCommand(ImportGlslFromClipboard);
    public ICommand ImportGlslFileCommand
    {
        get
        {
            if (m_importGlslFileCommand == null)
            {
                var fileOpenCommand = new FileOpenCommand("Load GLSL file", "GLSL Files", new[] { "*.txt", "*.glsl", "*.*" });
                fileOpenCommand.FileSelected += (sender, fileInfo) => { ImportGlslFromFile(fileInfo); };

                m_importGlslFileCommand = fileOpenCommand;
            }

            return m_importGlslFileCommand;
        }
    }
    
    public MainWindowViewModel()
    {
        ImportGlslFromString("You can use paste GLSL from the clipboard,\nor use the import buttons on the left.");
    }

    private void ImportGlslFromClipboard() => ImportGlslFromString(ClipboardService.GetText());

    private void ImportGlslFromFile(FileInfo file)
    {
        if (file.Exists)
            ImportGlslFromString(File.ReadAllText(file.FullName));
    }

    private void ImportGlslFromString(string glsl)
    {
        var diffs = DiffCreator.CreateDiffs(glsl.Trim(), string.Empty);
        m_diffs.Clear();
        m_diffs.AddRange(diffs);
    }
}
