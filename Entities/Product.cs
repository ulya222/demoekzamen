using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Avalonia.Media.Imaging;
using System.IO;
using System.Reflection;

namespace Demo1.Entities;

[Table("product")]
    public partial class Product
{
    [Key]
    [Column("productarticul")]
    [StringLength(10)]
    public string Productarticul { get; set; } = null!;

    [Column("productname")]
    [StringLength(50)]
    public string? Productname { get; set; }

    [Column("productunit")]
    [StringLength(10)]
    public string? Productunit { get; set; }

    [Column("productprice")]
    [Precision(10, 2)]
    public decimal? Productprice { get; set; }

    [Column("supplier")]
    public int? Supplier { get; set; }

    [Column("manufacturer")]
    public int? Manufacturer { get; set; }

    [Column("category")]
    public int? Category { get; set; }

    [Column("discount")]
    public int? Discount { get; set; }

    [Column("countinstock")]
    public int? Countinstock { get; set; }

    [Column("description")]
    [StringLength(200)]
    public string? Description { get; set; }

    [Column("photopath")]
    [StringLength(200)]
    public string? Photopath { get; set; }

    [ForeignKey("Category")]
    [InverseProperty("Products")]
    public virtual Category? CategoryNavigation { get; set; }

    [ForeignKey("Manufacturer")]
    [InverseProperty("Products")]
    public virtual Manufacturer? ManufacturerNavigation { get; set; }

    [InverseProperty("ProductNavigation")]
    public virtual ICollection<Orderproduct> Orderproducts { get; set; } = new List<Orderproduct>();

    [ForeignKey("Supplier")]
    [InverseProperty("Products")]
    public virtual Supplier? SupplierNavigation { get; set; }


    /// <summary>
    /// Безопасное вычисляемое свойство для отображения изображения товара.
    /// Если для товара нет фотографии или путь к ней неверный, используется
    /// заглушка <c>Image\picture.png</c>.
    /// </summary>
    public Bitmap? ImagePath
    {
        get
        {
            try
            {
                // Базовая папка сборки (bin/Debug/netX)
                var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Environment.CurrentDirectory;

                // Папка с изображениями: сначала "Image", затем "Images" как запасной вариант
                var imageDirPrimary = Path.Combine(baseDir, "Image");
                var imageDirFallback = Path.Combine(baseDir, "Images");
                var imagesDir = Directory.Exists(imageDirPrimary) ? imageDirPrimary : imageDirFallback;

                // Локальная функция для безопасной загрузки битмапа
                static Bitmap? TryLoadBitmap(string path)
                {
                    return File.Exists(path) ? new Bitmap(path) : null;
                }

                Bitmap? result = null;

                // 1. Если указан путь к фото товара — пробуем несколько вариантов.
                if (!string.IsNullOrWhiteSpace(Photopath))
                {
                    var trimmed = Photopath.Trim();

                    // Абсолютный путь
                    if (Path.IsPathRooted(trimmed))
                    {
                        result = TryLoadBitmap(trimmed);
                    }

                    // Относительный путь от папки сборки
                    if (result == null)
                    {
                        var fromBase = Path.Combine(baseDir, trimmed);
                        result = TryLoadBitmap(fromBase);
                    }

                    // Только имя файла внутри папки с изображениями
                    if (result == null)
                    {
                        var fileName = Path.GetFileName(trimmed);
                        var fromImages = Path.Combine(imagesDir, fileName);
                        result = TryLoadBitmap(fromImages);
                    }
                }

                // 2. Если фото не найдено или не задано — используем заглушку Image\picture.png
                if (result == null)
                {
                    var placeholder = Path.Combine(imagesDir, "picture.png");
                    result = TryLoadBitmap(placeholder);
                }

                return result;
            }
            catch
            {
                // Любые ошибки при загрузке изображения не должны ломать приложение
                return null;
            }
        }
    }
}
