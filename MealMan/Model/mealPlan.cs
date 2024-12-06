using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace MealMan.Model
{
    public class mealPlan
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string MealType { get; set; }
        public string Food { get; set; }    
        public int Calories { get; set; }
        public string Day { get; set; }
    }
}
