using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Demo1.AllWindows;
using Demo1.AllUserControl;
using Demo1.Entities;

namespace Demo1;

public partial class App : Application
{
    public static MainWindow? MainWindow { get; set; }
    public static UserControl? PrewiewUC { get; set; }  
    public static User? LoginUser { get; set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            MainWindow = new MainWindow();
            desktop.MainWindow = MainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}