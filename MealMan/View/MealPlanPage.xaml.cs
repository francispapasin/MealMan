using MealMan.ViewModel;

namespace MealMan.View;

public partial class MealPlanPage : ContentPage
{
	public MealPlanPage()
	{
		InitializeComponent();

        BindingContext = new MealPlanViewModel(); ;
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

    private async void Button_Clicked(object sender, EventArgs e)
    {
        var selectedMeal = (MealPlanViewModel)BindingContext;
        if (selectedMeal.SelectedMeal != null)
        {
            await Navigation.PushModalAsync(new UpdateMealPage(selectedMeal.SelectedMeal));
        }
        else
        {
            // Optionally, you can show a message if no meal is selected
            await DisplayAlert("Error", "Please select a meal to update.", "OK");
        }
    }

    // This method is called when the page is displayed or refreshed.
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Reload the meal plans when the page appears
        var viewModel = (MealPlanViewModel)BindingContext;
        await viewModel.LoadMeals();
    }

}