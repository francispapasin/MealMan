using MealMan.ViewModel;

namespace MealMan.View;

public partial class DailyMeal : ContentPage
{
	public DailyMeal()
	{
		InitializeComponent();
        BindingContext = new MealPlanViewModel();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var viewModel = (MealPlanViewModel)BindingContext;
        await viewModel.LoadMeals();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        var viewModel = (MealPlanViewModel)BindingContext;
        if (viewModel.SelectedMeal != null)
        {
            await Navigation.PushModalAsync(new UpdateMealPage(viewModel.SelectedMeal));
        }
        else
        {
            // Optionally, you can show a message if no meal is selected
            await DisplayAlert("Error", "Please select a meal to update.", "OK");
        }
    }

    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushModalAsync(new AddMealPage());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void SwipeItem_Clicked(object sender, EventArgs e)
    {
        var viewModel = (MealPlanViewModel)BindingContext;
        if (viewModel.SelectedMeal != null)
        {
            await Navigation.PushModalAsync(new UpdateMealPage(viewModel.SelectedMeal));
        }
        else
        {
            // Optionally, you can show a message if no meal is selected
            await DisplayAlert("Error", "Please select a meal to update.", "OK");
        }
    }
}