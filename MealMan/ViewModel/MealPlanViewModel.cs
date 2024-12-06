using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using MealMan.Model;
using SQLite;

namespace MealMan.ViewModel
{
    public class MealPlanViewModel : INotifyPropertyChanged
    {
        private readonly SQLiteAsyncConnection _database;

        public ObservableCollection<mealPlan> MealPlans { get; set; }
        public ObservableCollection<mealPlan> MondayMeals { get; set; }
        public ObservableCollection<mealPlan> TuesdayMeals { get; set; }
        public ObservableCollection<mealPlan> WednesdayMeals { get; set; }
        public ObservableCollection<mealPlan> ThursdayMeals { get; set; }
        public ObservableCollection<mealPlan> FridayMeals { get; set; }
        public ObservableCollection<mealPlan> SaturdayMeals { get; set; }
        public ObservableCollection<mealPlan> SundayMeals { get; set; }
        public int MondayCalories => MondayMeals.Sum(m => m.Calories);
        public int TuesdayCalories => TuesdayMeals.Sum(m => m.Calories);
        public int WednesdayCalories => WednesdayMeals.Sum(m => m.Calories);
        public int ThursdayCalories => ThursdayMeals.Sum(m => m.Calories);
        public int FridayCalories => FridayMeals.Sum(m => m.Calories);
        public int SaturdayCalories => SaturdayMeals.Sum(m => m.Calories);
        public int SundayCalories => SundayMeals.Sum(m => m.Calories);
        public ICommand AddMealCommand { get; }
        public ICommand UpdateMealCommand { get; }
        public ICommand DeleteMealCommand { get; }
        public ICommand LoadMealsCommand { get; }

        private mealPlan _selectedMeal;
        private ObservableCollection<mealPlan> _todaysMeals;
        public ObservableCollection<mealPlan> TodaysMeals
        {
            get => _todaysMeals;
            set
            {   
                _todaysMeals = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadTodaysMeals()
        {
            var today = DateTime.Now.DayOfWeek.ToString();
            var mealsForToday = await _database.Table<mealPlan>().Where(m => m.Day == today).ToListAsync();
            var sortedMeals = mealsForToday.OrderBy(m => GetMealTypeOrder(m.MealType)).ToList();
            TodaysMeals = new ObservableCollection<mealPlan>(sortedMeals);
        }

        public mealPlan SelectedMeal
        {
            get => _selectedMeal;
            set
            {
                if (_selectedMeal != value)
                {
                    _selectedMeal = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<string> _mealTypes;
        public ObservableCollection<string> MealTypes
        {
            get => _mealTypes;
            set
            {
                _mealTypes = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> _days;
        public ObservableCollection<string> Days
        {
            get => _days;
            set
            {
                _days = value;
                OnPropertyChanged();
            }
        }
        public MealPlanViewModel()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "mealplanner.db");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<mealPlan>().Wait();

            MealPlans = new ObservableCollection<mealPlan>();
            MealPlans.CollectionChanged += (s, e) => OnPropertyChanged(nameof(MealPlanCount)); // Update MealPlanCount

            MondayMeals = new ObservableCollection<mealPlan>();
            TuesdayMeals = new ObservableCollection<mealPlan>();
            WednesdayMeals = new ObservableCollection<mealPlan>();
            ThursdayMeals = new ObservableCollection<mealPlan>();
            FridayMeals = new ObservableCollection<mealPlan>();
            SaturdayMeals = new ObservableCollection<mealPlan>();
            SundayMeals = new ObservableCollection<mealPlan>();

            // Subscribe to collection changes to update calorie totals
            MondayMeals.CollectionChanged += (s, e) => OnPropertyChanged(nameof(MondayCalories));
            TuesdayMeals.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TuesdayCalories));
            WednesdayMeals.CollectionChanged += (s, e) => OnPropertyChanged(nameof(WednesdayCalories));
            ThursdayMeals.CollectionChanged += (s, e) => OnPropertyChanged(nameof(ThursdayCalories));
            FridayMeals.CollectionChanged += (s, e) => OnPropertyChanged(nameof(FridayCalories));
            SaturdayMeals.CollectionChanged += (s, e) => OnPropertyChanged(nameof(SaturdayCalories));
            SundayMeals.CollectionChanged += (s, e) => OnPropertyChanged(nameof(SundayCalories));


            SelectedMeal = new mealPlan();
            MealTypes = new ObservableCollection<string> { "Breakfast", "Lunch", "Meryenda", "Dinner" };
            Days = new ObservableCollection<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

            AddMealCommand = new Command(async () => await AddMeal());
            UpdateMealCommand = new Command(async () => await UpdateMeal());
            DeleteMealCommand = new Command(async () => await DeleteMeal());
            LoadMealsCommand = new Command(async () => await LoadMeals());
        }


        public async Task LoadMeals()
        {
            var meals = await _database.Table<mealPlan>().ToListAsync();
            var sortedMeals = meals.OrderBy(m => GetMealTypeOrder(m.MealType)).ToList();
            MealPlans.Clear();

            MondayMeals.Clear();
            TuesdayMeals.Clear();
            WednesdayMeals.Clear();
            ThursdayMeals.Clear();
            FridayMeals.Clear();
            SaturdayMeals.Clear();
            SundayMeals.Clear();

            // Add sorted meals to MealPlans and day collections
            foreach (var meal in sortedMeals)
            {
                MealPlans.Add(meal);
                AddMealToDayCollection(meal);  // Adds meal to the correct day collection
            }

            // Load today's meals
            await LoadTodaysMeals();
        }


        private int GetMealTypeOrder(string mealType)
        {
            switch (mealType)
            {
                case "Breakfast": return 1;
                case "Lunch": return 2;
                case "Meryenda": return 3;
                case "Dinner": return 4;
                default: return 5; // For any other meal types, place them at the end
            }
        }

        public async Task AddMeal()
        {
            if (SelectedMeal != null && !string.IsNullOrEmpty(SelectedMeal.MealType) && !string.IsNullOrEmpty(SelectedMeal.Food))
            {
                // Check if the meal type already exists for the same day
                var existingMeal = await _database.Table<mealPlan>()
                                                   .FirstOrDefaultAsync(m => m.Day == SelectedMeal.Day && m.MealType == SelectedMeal.MealType);

                if (existingMeal == null)
                {
                    // If meal doesn't exist, insert it
                    await _database.InsertAsync(SelectedMeal);
                    MealPlans.Add(SelectedMeal);

                    // Add to the appropriate day collection
                    AddMealToDayCollection(SelectedMeal);

                    // Clear the selected meal to allow adding a new one
                    SelectedMeal = new mealPlan();
                    OnPropertyChanged(nameof(MealPlanCount));
                }
                else
                {
                    // Show an alert if a meal type already exists for the selected day
                    await App.Current.MainPage.DisplayAlert("Duplicate Meal Type",
                        $"A {SelectedMeal.MealType} already exists for {SelectedMeal.Day}.", "OK");
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Invalid Input",
                    "Please enter valid meal details.", "OK");
            }
        }


        public async Task UpdateMeal()
        {
            if (SelectedMeal != null)
            {
                if (!string.IsNullOrWhiteSpace(SelectedMeal.Food) && SelectedMeal.Calories > 0)
                {
                    // Update the meal in the database
                    await _database.UpdateAsync(SelectedMeal);

                    // Remove the old meal from the collection
                    RemoveMealFromDayCollection(SelectedMeal);

                    // Add the updated meal to the correct day collection
                    AddMealToDayCollection(SelectedMeal);

                    // Reload meals to reflect the changes
                    await LoadMeals();
                }
                else
                {
                    Console.WriteLine("Meal details are not valid for updating.");
                }
            }
            else
            {
                Console.WriteLine("No meal selected for update.");
            }
        }


        public async Task DeleteMeal()
        {
            if (SelectedMeal != null)
            {
                // Delete the selected meal from the database
                await _database.DeleteAsync(SelectedMeal);

                // Remove it from the ObservableCollection
                MealPlans.Remove(SelectedMeal);

                // Remove from the appropriate day collection
                RemoveMealFromDayCollection(SelectedMeal);

                // Clear the selected meal
                SelectedMeal = new mealPlan();

                // Reload meals to refresh the view (optional)
                await LoadMeals();
            }
        }
        private void AddMealToDayCollection(mealPlan meal)
        {
            switch (meal.Day)
            {
                case "Monday":
                    MondayMeals.Add(meal);
                    break;
                case "Tuesday":
                    TuesdayMeals.Add(meal);
                    break;
                case "Wednesday":
                    WednesdayMeals.Add(meal);
                    break;
                case "Thursday":
                    ThursdayMeals.Add(meal);
                    break;
                case "Friday":
                    FridayMeals.Add(meal);
                    break;
                case "Saturday":
                    SaturdayMeals.Add(meal);
                    break;
                case "Sunday":
                    SundayMeals.Add(meal);
                    break;
            }
        }
        private void RemoveMealFromDayCollection(mealPlan meal)
        {
            // Check if the meal is not null and has a valid Day value
            if (meal != null && !string.IsNullOrEmpty(meal.Day))
            {
                // Check which day the meal belongs to and remove it from the appropriate collection
                switch (meal.Day)
                {
                    case "Monday":
                        // Ensure the collection is not null and contains the meal
                        if (MondayMeals.Contains(meal))
                            MondayMeals.Remove(meal);
                        break;
                    case "Tuesday":
                        if (TuesdayMeals.Contains(meal))
                            TuesdayMeals.Remove(meal);
                        break;
                    case "Wednesday":
                        if (WednesdayMeals.Contains(meal))
                            WednesdayMeals.Remove(meal);
                        break;
                    case "Thursday":
                        if (ThursdayMeals.Contains(meal))
                            ThursdayMeals.Remove(meal);
                        break;
                    case "Friday":
                        if (FridayMeals.Contains(meal))
                            FridayMeals.Remove(meal);
                        break;
                    case "Saturday":
                        if (SaturdayMeals.Contains(meal))
                            SaturdayMeals.Remove(meal);
                        break;
                    case "Sunday":
                        if (SundayMeals.Contains(meal))
                            SundayMeals.Remove(meal);
                        break;
                }
            }
            else
            {
                // Handle the case where the meal is null or its Day is not set
                Console.WriteLine("Meal is null or has no valid day.");
            }
        }

        // Property to get the count of meal plans
        public int MealPlanCount => MealPlans?.Count ?? 0;

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
