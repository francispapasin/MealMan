using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using MealMan.Model;

namespace MealMan.Services
{
    public class DatabaseService
    {
        private readonly SQLiteConnection _database;

        public DatabaseService(string dbPath)
        {
            _database = new SQLiteConnection(dbPath);
            _database.CreateTable<mealPlan>();
        }

        public int SaveMeal(mealPlan meal)
        {
            if (meal.Id != 0)
            {
                _database.Update(meal);
                return meal.Id;
            }
            else
            {
                return _database.Insert(meal);
            }
        }
        public int DeleteMeal(int id)
        {
            return _database.Delete<mealPlan>(id);
        }

        public List<mealPlan> GetMealPlan()
        {
            return _database.Table<mealPlan>().ToList();
        }

        public mealPlan GetMeal(int id)
        {
            return _database.Table<mealPlan>().FirstOrDefault(item => item.Id == id);
        }

    }
}
