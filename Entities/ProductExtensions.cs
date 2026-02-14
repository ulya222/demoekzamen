using Avalonia.Media.Imaging;
using System;
using System.IO;

namespace Demo1.Entities
{
    public partial class Product
    {
        /// <summary>
        /// Вычисляемое свойство для загрузки изображения товара
        /// </summary>
        public Bitmap? Image
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(Photopath))
                        return null;
                    
                    // Формируем полный путь к изображению
                    var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Photopath);
                    
                    // Если файл существует — загружаем
                    if (File.Exists(fullPath))
                        return new Bitmap(fullPath);
                    
                    // Если файл не найден — пробуем альтернативный путь
                    var altPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", Photopath);
                    if (File.Exists(altPath))
                        return new Bitmap(altPath);
                    
                    return null;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}