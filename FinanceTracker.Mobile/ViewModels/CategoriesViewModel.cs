using FinanceTracker.Core;
using FinanceTracker.Core.Services;
using FinanceTracker.Shared.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace FinanceTracker.Mobile.ViewModels
{
    public class CategoriesViewModel : INotifyPropertyChanged
    {
        private readonly ICategoryService _categoryService;
        private readonly ExceptionLogger _exceptionLogger;
        private ObservableCollection<Category> _categories = [];

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddCategoryCommand { get; }
        public ICommand EditCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }

        public CategoriesViewModel(ICategoryService categoryService, ExceptionLogger exceptionLogger)
        {
            _categoryService = categoryService;
            _exceptionLogger = exceptionLogger;

            AddCategoryCommand = new Command(AddCategory);
            EditCategoryCommand = new Command<Category>(EditCategory);
            DeleteCategoryCommand = new Command<Category>(DeleteCategory);

            _ = LoadCategories();
        }

        private async Task LoadCategories()
        {
            try
            {
                IEnumerable<Category> categories = await _categoryService.GetCategoriesAsync();
                Categories = new ObservableCollection<Category>(categories);
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
            }
        }

        public event EventHandler<Category>? CategoryAdded;

        private async void AddCategory()
        {
            try
            {
                string newCategoryName = await (Application.Current?.MainPage?.DisplayPromptAsync("New Category", "Enter the name for the new category:") ?? Task.FromResult(string.Empty));

                if (!string.IsNullOrWhiteSpace(newCategoryName))
                {
                    Category newCategory = new() { Name = newCategoryName };
                    await _categoryService.AddCategoryAsync(newCategory);
                    Categories.Add(newCategory);

                    CategoryAdded?.Invoke(this, newCategory);
                }
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
            }
        }

        private async void EditCategory(Category category)
        {
            try
            {
                string newCategoryName = await (Application.Current?.MainPage?.DisplayPromptAsync("Edit Category", "Enter the new name for the category:", initialValue: category.Name) ?? Task.FromResult(string.Empty));

                if (!string.IsNullOrWhiteSpace(newCategoryName) && newCategoryName != category.Name)
                {
                    category.Name = newCategoryName;
                    await _categoryService.UpdateCategoryAsync(category);

                    int index = Categories.IndexOf(category);
                    if (index >= 0)
                    {
                        Categories[index] = category;
                    }
                }
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
            }
        }

        private async void DeleteCategory(Category category)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(category);
                _ = Categories.Remove(category);
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
