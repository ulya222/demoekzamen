using System;

namespace Demo1.Entities;

/// <summary>
/// Модель для отображения заказа в списке.
/// Содержит все нужные строки для UI и ссылку на исходный Order.
/// </summary>
public class OrderDisplay
{
    public required Order SourceOrder { get; set; }

    public int Orderid { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public DateOnly? Orderdate { get; set; }

    public DateOnly? Orderdateissue { get; set; }

    public string UserLastName { get; set; } = string.Empty;
}
