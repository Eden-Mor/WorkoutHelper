using CommunityToolkit.Maui.Converters;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using WorkoutHelper.Data;
using WorkoutHelper.Models;
using WorkoutHelper.Services;
using WorkoutHelper.Shared.Models;
using WorkoutHelper.ViewModels;

namespace WorkoutHelper.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var role = SessionService.Role;
            AddExerciseButton.IsVisible = role >= Shared.Enums.RolesEnum.TrustedContributor;
            LoginButton.IsVisible = role == Shared.Enums.RolesEnum.User;
        }

        private void LightDarkBtn_Clicked(object sender, EventArgs e) => Application.Current.UserAppTheme = Application.Current.UserAppTheme == AppTheme.Light ? AppTheme.Dark : AppTheme.Light;



        private async void JsonData_Clicked(object sender, EventArgs e)
        {
            if (this.BindingContext is not ExercisesViewModel vm)
                return;

            var dataItems = await vm.personalExerciseInfoDB.GetItemsAsync();
            var json = JsonSerializer.Serialize(dataItems);
            await Clipboard.Default.SetTextAsync(json);
        }
    }
}
