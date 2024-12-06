using MealMan.ViewModel;

namespace MealMan.View;

public partial class AddMealPage : ContentPage
{
    public AddMealPage()
    {
        InitializeComponent();
        BindingContext = new MealPlanViewModel();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    // This method is triggered when the Add Meal button is clicked
    private async void AddMealButton_Clicked(object sender, EventArgs e)
    {
        var viewModel = (MealPlanViewModel)BindingContext;
        await viewModel.AddMeal();

        await Navigation.PopAsync();
    }

    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}