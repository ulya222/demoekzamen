using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo1.Entities;

namespace Demo1.Models
{
    public class Context
    {
        public static AppDbContext Connect { get; } = new AppDbContext();
        
        static Context()
        {
            // Создаём БД и таблицы при первом обращении
            try
            {
                Connect.Database.EnsureCreated();
                Console.WriteLine($"✓ База данных инициализирована. Пользователей: {Connect.Users.Count()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Ошибка инициализации БД: {ex.Message}");
            }
        }
    }
}
