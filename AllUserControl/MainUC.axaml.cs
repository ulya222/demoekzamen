using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using EducationDE.Entities;
using EducationDE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EducationDE.AllUserControl
{
    public partial class MainUC : UserControl
    {
        private List<Product> _allProducts = new();
        private User? _loginUser;
        private Product? _selectedProduct;

        public MainUC(User? loginUser)
        {
            try
            {
                InitializeComponent();
                _loginUser = loginUser;

                App.PrewiewUC = this;

                if (App.MainWindow != null)
                {
                    var titleTextBlock = App.MainWindow.FindControl<TextBlock>("TitleTextBlock");
                    if (titleTextBlock != null)
                    {
                        titleTextBlock.Text = "Каталог товаров";
                    }
                }

                // Ждем загрузки визуального дерева перед загрузкой данных
                this.Loaded += MainUC_Loaded;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MainUC constructor error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw; // Пробрасываем дальше, чтобы AuthUC мог поймать
            }
        }

        private void MainUC_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            LoadData();
            SetupButtonVisibility();
            this.Loaded -= MainUC_Loaded; // Отписываемся после первой загрузки
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void LoadData()
        {
            try
            {
                if (Context.Connect == null)
                {
                    Console.WriteLine("LoadData ERROR: Context.Connect is null");
                    return;
                }

                var context = Context.Connect;

                if (context.Products == null)
                {
                    Console.WriteLine("LoadData ERROR: context.Products is null");
                    return;
                }

                _allProducts = context.Products
                    .Include(p => p.CategoryNavigation)
                    .Include(p => p.ManufacturerNavigation)
                    .Include(p => p.SupplierNavigation)
                    .ToList();

                Console.WriteLine($"LoadData: Загружено товаров: {_allProducts.Count}");

                if (context.Categories == null)
                {
                    Console.WriteLine("LoadData ERROR: context.Categories is null");
                    return;
                }

                var categories = context.Categories.ToList();
                var categoryCombo = this.FindControl<ComboBox>("CategoryComboBox");
                if (categoryCombo != null)
                {
                    categoryCombo.ItemsSource = categories;
                    Console.WriteLine($"LoadData: Загружено категорий: {categories.Count}");
                }
                else
                {
                    Console.WriteLine("LoadData WARNING: CategoryComboBox not found");
                }

                var productListBox = this.FindControl<ListBox>("ProductListBox");
                if (productListBox != null)
                {
                    productListBox.ItemsSource = _allProducts;
                    Console.WriteLine($"LoadData: Установлено ItemsSource для ProductListBox, товаров: {_allProducts.Count}");
                    
                    // Показываем сообщение, если товаров нет
                    if (_allProducts.Count == 0)
                    {
                        if (App.MainWindow != null)
                        {
                            var mainText = App.MainWindow.FindControl<TextBlock>("MainTextBlock");
                            if (mainText != null)
                                mainText.Text = "В базе данных нет товаров. Добавьте товары через кнопку 'Добавить'.";
                        }
                    }
                }
                else
                {
                    Console.WriteLine("LoadData ERROR: ProductListBox not found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoadData ERROR: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                // Не пробрасываем исключение, чтобы не падало приложение
            }
        }

        /// <summary>
        /// Роли: 1 = Администратор (полный доступ), 2 = Менеджер (товары и заказы), 3 = Гость (только просмотр).
        /// </summary>
        private void SetupButtonVisibility()
        {
            var buttonPanel = this.FindControl<StackPanel>("ButtonPanel");
            var searchPanel = this.FindControl<StackPanel>("SearchPanel");

            if (_loginUser == null)
            {
                if (buttonPanel != null) buttonPanel.IsVisible = false;
                if (searchPanel != null) searchPanel.IsVisible = false;
            }
            else if (_loginUser.Role == 3)
            {
                // Гость — только просмотр товаров, без кнопок управления
                if (buttonPanel != null) buttonPanel.IsVisible = false;
                if (searchPanel != null) searchPanel.IsVisible = true;
            }
            else
            {
                // Администратор (1) и Менеджер (2) — полный доступ к товарам
                if (buttonPanel != null) buttonPanel.IsVisible = true;
                if (searchPanel != null) searchPanel.IsVisible = true;
            }
        }

        private void ProductListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            var productListBox = sender as ListBox ?? this.FindControl<ListBox>("ProductListBox");
            if (productListBox != null)
            {
                _selectedProduct = productListBox.SelectedItem as Product;
            }
        }

        private void AddButton_OnClick(object? sender, RoutedEventArgs e)
        {
            if (App.MainWindow != null)
            {
                var content = App.MainWindow.FindControl<ContentControl>("MainContentControl");
                if (content != null)
                {
                    content.Content = new EditAddProductUC();
                }
            }
        }

        private void EditButton_OnClick(object? sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null)
            {
                if (App.MainWindow != null)
                {
                    var mainText = App.MainWindow.FindControl<TextBlock>("MainTextBlock");
                    if (mainText != null)
                        mainText.Text = "Выберите товар для редактирования";
                }
                return;
            }

            if (App.MainWindow != null)
            {
                var content = App.MainWindow.FindControl<ContentControl>("MainContentControl");
                if (content != null)
                {
                    content.Content = new EditAddProductUC(_selectedProduct);
                }
            }
        }

        private void DeleteButton_OnClick(object? sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null)
            {
                if (App.MainWindow != null)
                {
                    var mainText = App.MainWindow.FindControl<TextBlock>("MainTextBlock");
                    if (mainText != null)
                        mainText.Text = "Выберите товар для удаления";
                }
                return;
            }

            try
            {
                var context = Context.Connect;

                var inOrders = context.Orderproducts
                    .Any(op => op.ProductNavigation != null &&
                               op.ProductNavigation.Productarticul == _selectedProduct.Productarticul);

                if (inOrders)
                {
                    if (App.MainWindow != null)
                    {
                        var mainText = App.MainWindow.FindControl<TextBlock>("MainTextBlock");
                        if (mainText != null)
                            mainText.Text = "Ошибка: товар используется в заказах";
                    }
                    return;
                }

                context.Products.Remove(_selectedProduct);
                context.SaveChanges();

                _selectedProduct = null;
                
                // Обновляем данные
                LoadData();
                // После LoadData уже установлен ItemsSource = _allProducts, 
                // но нужно применить текущие фильтры если они есть
                ApplyFilters();

                if (App.MainWindow != null)
                {
                    var mainText = App.MainWindow.FindControl<TextBlock>("MainTextBlock");
                    if (mainText != null)
                        mainText.Text = "Товар успешно удалён";
                }
            }
            catch (Exception ex)
            {
                if (App.MainWindow != null)
                {
                    var mainText = App.MainWindow.FindControl<TextBlock>("MainTextBlock");
                    if (mainText != null)
                        mainText.Text = $"Ошибка удаления: {ex.Message}";
                }
                Console.WriteLine($"DeleteButton ERROR: {ex.Message}");
            }
        }

        /// <summary>
        /// Применяет все фильтры: категория, поиск и сортировка
        /// </summary>
        private void ApplyFilters()
        {
            var productListBox = this.FindControl<ListBox>("ProductListBox");
            if (productListBox == null)
            {
                Console.WriteLine("ApplyFilters ERROR: ProductListBox not found");
                return;
            }

            var filtered = _allProducts.AsEnumerable();

            // Фильтр по категории
            var categoryCombo = this.FindControl<ComboBox>("CategoryComboBox");
            if (categoryCombo?.SelectedItem is Category selectedCategory)
            {
                filtered = filtered.Where(p => p.Category == selectedCategory.Categoryid);
            }

            // Фильтр по поисковому запросу
            var searchTextBox = this.FindControl<TextBox>("SearchTextBox");
            var searchText = searchTextBox?.Text?.ToLower() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filtered = filtered.Where(p => p.Productname != null &&
                                             p.Productname.ToLower().Contains(searchText));
            }

            // Сортировка по цене (возрастание/убывание)
            var sortUpButton = this.FindControl<RadioButton>("SortUpButton");
            var sortDownButton = this.FindControl<RadioButton>("SortDownButton");
            if (sortUpButton?.IsChecked == true)
            {
                filtered = filtered.OrderBy(p => p.Productprice ?? 0);
            }
            else if (sortDownButton?.IsChecked == true)
            {
                filtered = filtered.OrderByDescending(p => p.Productprice ?? 0);
            }

            productListBox.ItemsSource = filtered.ToList();
        }

        private void CategoryComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void SearchTextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void SortButton_OnChecked(object? sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        private void ResetButton_OnClick(object? sender, RoutedEventArgs e)
        {
            var categoryCombo = this.FindControl<ComboBox>("CategoryComboBox");
            var searchTextBox = this.FindControl<TextBox>("SearchTextBox");
            var sortUpButton = this.FindControl<RadioButton>("SortUpButton");
            var sortDownButton = this.FindControl<RadioButton>("SortDownButton");

            if (categoryCombo != null) categoryCombo.SelectedIndex = -1;
            if (searchTextBox != null) searchTextBox.Text = string.Empty;
            if (sortUpButton != null) sortUpButton.IsChecked = false;
            if (sortDownButton != null) sortDownButton.IsChecked = false;

            ApplyFilters();
        }
    }
}
