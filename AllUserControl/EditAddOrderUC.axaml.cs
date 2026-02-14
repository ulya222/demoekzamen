using Avalonia.Controls;
using Avalonia.Interactivity;
using EducationDE.Entities;
using EducationDE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using EducationDE.AllWindows;

namespace EducationDE.AllUserControl;

public partial class EditAddOrderUC : UserControl
{
    private Order _order;

    public List<Address> addresses { get; set; }
    public List<Status> statuses { get; set; }

    public EditAddOrderUC(Order order)
    {
        InitializeComponent();

        _order = order;

        statuses = Context.Connect.Statuses.ToList();
        addresses = Context.Connect.Addresses.ToList();

        DateStart.SelectedDate = ConvertDateOnly(_order.Orderdate);
        DateIssue.SelectedDate = ConvertDateOnly(_order.Orderdateissue);

        StatusComboBox.SelectedItem = _order.OrderstatusNavigation;
        AddressComboBox.SelectedItem = _order.OrderaddressNavigation;

        DataContext = this;

        SetTitle("Редактирование заказа");
        App.PrewiewUC = new OrderUC();
    }

    public EditAddOrderUC()
    {
        InitializeComponent();

        _order = new Order();
        if (App.LoginUser != null)
            _order.Orderuser = App.LoginUser.Userid;

        addresses = Context.Connect.Addresses.ToList();
        statuses = Context.Connect.Statuses.ToList();

        DataContext = this;

        SetTitle("Добавление заказа");
        App.PrewiewUC = new OrderUC();
    }

    private void SetTitle(string title)
    {
        if (MainWindow.MainWindowInstance != null)
        {
            var titleBlock = MainWindow.MainWindowInstance
                .FindControl<TextBlock>("TitleTextBlock");

            if (titleBlock != null)
                titleBlock.Text = title;
        }
    }

    private DateTimeOffset? ConvertDateOnly(DateOnly? date)
    {
        if (date.HasValue)
            return new DateTimeOffset(date.Value.ToDateTime(TimeOnly.MinValue));
        return null;
    }

    private void OkButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DateStart.SelectedDate.HasValue)
            _order.Orderdate = DateOnly.FromDateTime(DateStart.SelectedDate.Value.DateTime);

        if (DateIssue.SelectedDate.HasValue)
            _order.Orderdateissue = DateOnly.FromDateTime(DateIssue.SelectedDate.Value.DateTime);

        if (StatusComboBox.SelectedItem is Status status)
        {
            _order.OrderstatusNavigation = status;
            _order.Orderstatus = status.Statusid;
        }

        if (AddressComboBox.SelectedItem is Address address)
        {
            _order.OrderaddressNavigation = address;
            _order.Orderaddress = address.Addressid;
        }

        if (_order.Orderid == 0)
            Context.Connect.Orders.Add(_order);

        Context.Connect.SaveChanges();
        NavigateToOrders();
    }

    private void NavigateToOrders()
    {
        var mainWindow = MainWindow.MainWindowInstance;

        if (mainWindow != null)
        {
            var content = mainWindow
                .FindControl<ContentControl>("MainContentControl");

            if (content != null)
                content.Content = new OrderUC();
        }
    }
}
