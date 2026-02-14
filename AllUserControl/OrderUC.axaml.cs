using Avalonia.Controls;
using Avalonia.Interactivity;
using EducationDE.Entities;
using EducationDE.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using EducationDE.AllWindows;
using System.IO;
using System;
namespace EducationDE.AllUserControl;

public partial class OrderUC : UserControl
{
    public List<OrderDisplay> OrderList { get; set; } = new();

    public OrderUC()
    {
        
        InitializeComponent();
        DataContext = this;

        LoadOrders();

        if (MainWindow.MainWindowInstance != null)
        {
            var titleBlock = MainWindow.MainWindowInstance.FindControl<TextBlock>("TitleTextBlock");
            if (titleBlock != null)
                titleBlock.Text = "Управление заказами";
        }

        App.PrewiewUC = this;
    }

    private void LoadOrders()
    {
        File.AppendAllText("debug.log", $"[{DateTime.Now:HH:mm:ss}] OrderUC CONSTRUCTOR START\n");
        if (Context.Connect?.Orders == null) return;

        var ctx = Context.Connect;
        
        Console.Out.Flush();

        if (Context.Connect == null)
        {
            Console.WriteLine("CRITICAL: Context.Connect is NULL!");
            Console.Out.Flush();
            return;
        }
        File.AppendAllText("debug.log", $"Orders: {Context.Connect.Orders?.Count() ?? -1}");
        File.AppendAllText("debug.log", $"Statuses: {Context.Connect.Statuses?.Count() ?? -1}");
        File.AppendAllText("debug.log", $"Addresses: {Context.Connect.Addresses?.Count() ?? -1}");
        
        // 🔑 ПРОВЕРЯЕМ ИМЕНА СВОЙСТВ (главная проблема!)
        if (Context.Connect.Statuses?.Any() == true)
        {
            var firstStatus = Context.Connect.Statuses.First();
            File.AppendAllText("debug.log", $"First Status properties:");
            File.AppendAllText("debug.log", $"  Statusid = {firstStatus.Statusid}");
            File.AppendAllText("debug.log", $"  StatusName = '{firstStatus.Statusname}'"); // ← Правильное имя
            // Попробуем оба варианта для диагностики:
            File.AppendAllText("debug.log", $"  Statusname (lowercase) = '{firstStatus.GetType().GetProperty("Statusname")?.GetValue(firstStatus)}'");
            
        }
        // Считываем справочники статусов и адресов один раз.
        var statusDict = ctx.Statuses?
            .ToDictionary(s => s.Statusid, s => s.Statusname ?? "—")
            ?? new Dictionary<int, string>();

        var addressDict = ctx.Addresses?
            .ToDictionary(a => a.Addressid, a => a.Addressname ?? "—")
            ?? new Dictionary<int, string>();

        var orders = ctx.Orders
            .Include(o => o.OrderuserNavigation)
            .OrderByDescending(o => o.Orderdate)
            .ToList();

        // Проецируем в удобную для UI модель с уже готовыми строками.
        OrderList = orders
            .Select(o => new OrderDisplay
            {
                SourceOrder = o,
                Orderid = o.Orderid,
                Status = BuildStatusString(o, statusDict),
                Address = BuildAddressString(o, addressDict),
                Orderdate = o.Orderdate,
                Orderdateissue = o.Orderdateissue,
                UserLastName = o.OrderuserNavigation?.Lastname ?? string.Empty
            })
            .ToList();

        // Явно обновляем источник данных ListBox,
        // чтобы список гарантированно перерисовывался.
        var lb = this.FindControl<ListBox>("OrderListBox");
        if (lb != null)
            lb.ItemsSource = OrderList;
    }

    // Возвращает человекочитаемый статус с подстраховкой, чтобы строка НИКОГДА не была пустой.
    private static string BuildStatusString(Order o, IReadOnlyDictionary<int, string> statusDict)
    {
        if (o.Orderstatus.HasValue && statusDict.TryGetValue(o.Orderstatus.Value, out var name))
        {
            if (!string.IsNullOrWhiteSpace(name))
                return name;
        }

        if (o.Orderstatus.HasValue)
            return $"Статус #{o.Orderstatus.Value}";

        return "Статус не указан";
    }

    // Возвращает человекочитаемый адрес с подстраховкой.
    private static string BuildAddressString(Order o, IReadOnlyDictionary<int, string> addressDict)
    {
        if (o.Orderaddress.HasValue && addressDict.TryGetValue(o.Orderaddress.Value, out var name))
        {
            if (!string.IsNullOrWhiteSpace(name))
                return name;
        }

        if (o.Orderaddress.HasValue)
            return $"Адрес #{o.Orderaddress.Value}";

        return "Адрес не указан";
    }

    private void AddButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigate(new EditAddOrderUC());
    }

    private void EditButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var listBox = this.FindControl<ListBox>("OrderListBox");

        if (listBox?.SelectedItem is OrderDisplay selected)
            Navigate(new EditAddOrderUC(selected.SourceOrder));
    }

    private void DeleteButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var listBox = this.FindControl<ListBox>("OrderListBox");
        if (listBox?.SelectedItem is OrderDisplay selected)
        {
            Context.Connect.Orders.Remove(selected.SourceOrder);
            Context.Connect.SaveChanges();
            LoadOrders();
        }
    }

    private void Navigate(UserControl control)
    {
        var mainWindow = MainWindow.MainWindowInstance;

        if (mainWindow != null)
        {
            var content = mainWindow
                .FindControl<ContentControl>("MainContentControl");

            if (content != null)
                content.Content = control;
        }
    }

    private void OrderListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
    }
}
