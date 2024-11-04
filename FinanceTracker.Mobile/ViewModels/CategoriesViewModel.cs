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
        private readonly ITransactionService _transactionService;
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

        public CategoriesViewModel(ICategoryService categoryService, ITransactionService transactionService, ExceptionLogger exceptionLogger)
        {
            _categoryService = categoryService;
            _transactionService = transactionService;
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
                    await LoadCategories();
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
                bool isConfirmed = await (Application.Current?.MainPage?.DisplayAlert("Delete Category", $"Are you sure you want to delete the category '{category.Name}'? All associated transactions will also be deleted.", "Yes", "No") ?? Task.FromResult(false));

                if (isConfirmed)
                {
                    IEnumerable<Transaction> transactionsToDelete = await _transactionService.GetTransactionsByCategoryAsync(category.Id);
                    foreach (Transaction transaction in transactionsToDelete)
                    {
                        await _transactionService.DeleteTransactionAsync(transaction);
                    }

                    await _categoryService.DeleteCategoryAsync(category);
                    _ = Categories.Remove(category);

                    await (Application.Current?.MainPage?.DisplayAlert("Category Deleted", $"The category '{category.Name}' and all its associated transactions have been deleted.", "OK") ?? Task.CompletedTask);

                    await LoadCategories();
                }
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
