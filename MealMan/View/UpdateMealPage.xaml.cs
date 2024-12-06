using MealMan.Model;
using MealMan.ViewModel;

namespace MealMan.View;

public partial class UpdateMealPage : ContentPage
{
	public UpdateMealPage(mealPlan selectedMeal)
	{
		InitializeComponent();
        BindingContext = new MealPlanViewModel();
        var viewModel = (MealPlanViewModel)BindingContext;
        viewModel.SelectedMeal = selectedMeal; 

    }

    private async void ImageButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}