using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;   // ← обязательно
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using EducationDE.AllUserControl;
using System.IO;
using System.Reflection;
using EducationDE.Entities;

namespace EducationDE.AllWindows;

public partial class MainWindow : Window
{
    public static MainWindow? MainWindowInstance { get; set; }

    public MainWindow()
    {
        InitializeComponent();

        MainWindowInstance = this;
        App.MainWindow = this;

        // Загружаем первое окно — Авторизация
        var content = this.FindControl<ContentControl>("MainContentControl");
        if (content != null)
            content.Content = new AuthUC();

        LoadLogo();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }


    private void LoadLogo()
    {
        try
        {
            var imageControl = this.FindControl<Image>("LogoImage");

            if (imageControl != null)
            {
                var baseDir = Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location);

                var logoPath = Path.Combine(baseDir ?? "", "Images", "Icon.png");

                if (File.Exists(logoPath))
                    imageControl.Source = new Bitmap(logoPath);
            }
        }
        catch
        {
            // Ошибки загрузки логотипа не критичны
        }
    }

    /// <summary>
    /// Кнопка Назад — выход из учётной записи и возврат на окно авторизации.
    /// </summary>
    private void BackButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var content = this.FindControl<ContentControl>("MainContentControl");
        if (content == null) return;

        var currentContent = content.Content;
        bool isAuthScreen = currentContent is AuthUC;
        if (isAuthScreen) return;

        App.LoginUser = null;
        App.PrewiewUC = null;
        content.Content = new AuthUC();
        UpdateNavigationVisibility();

        var mainText = this.FindControl<TextBlock>("MainTextBlock");
        if (mainText != null) mainText.Text = "Гость";
    }

    /// <summary>
    /// Навигация: Каталог товаров.
    /// </summary>
    public void NavCatalogButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var content = this.FindControl<ContentControl>("MainContentControl");
        if (content != null && App.LoginUser != null)
        {
            content.Content = new MainUC(App.LoginUser);
            var title = this.FindControl<TextBlock>("TitleTextBlock");
            if (title != null) title.Text = "Каталог товаров";
        }
    }

    /// <summary>
    /// Навигация: Заказы.
    /// </summary>
    public void NavOrdersButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var content = this.FindControl<ContentControl>("MainContentControl");
        if (content != null)
        {
            content.Content = new OrderUC();
            var title = this.FindControl<TextBlock>("TitleTextBlock");
            if (title != null) title.Text = "Управление заказами";
        }
    }

    /// <summary>
    /// Показать/скрыть панель навигации (Каталог / Заказы) в зависимости от роли.
    /// Вызывается после авторизации.
    /// </summary>
    public void UpdateNavigationVisibility()
    {
        var navPanel = this.FindControl<StackPanel>("NavPanel");
        if (navPanel == null) return;

        // Показываем навигацию только Администратору (1) и Менеджеру (2). Гость (3) — только каталог, навигация тоже полезна.
        bool isLoggedIn = App.LoginUser != null;
        navPanel.IsVisible = isLoggedIn;

        if (isLoggedIn && App.LoginUser!.Role == 3)
        {
            // Гость — скрыть пункт "Заказы"
            var ordersBtn = this.FindControl<Button>("NavOrdersButton");
            if (ordersBtn != null) ordersBtn.IsVisible = false;
        }
        else
        {
            var ordersBtn = this.FindControl<Button>("NavOrdersButton");
            if (ordersBtn != null) ordersBtn.IsVisible = true;
        }
    }
}
