using CommunityToolkit.Maui.Alerts;
using System.Text.Json;
using WorkoutHelper.Data;
using WorkoutHelper.Services;
using WorkoutHelper.Shared.Models;

namespace WorkoutHelper.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public LoginData LoginData { get; set; } = new();

        public Command LoginCommand { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new(LoginUsingCredentials);
        }

        private async void LoginUsingCredentials(object obj)
        {
            if (string.IsNullOrEmpty(LoginData.Username) || string.IsNullOrEmpty(LoginData.PasswordHash))
            {
                await Application.Current.MainPage.DisplaySnackbar("Credentials missing please fill them out before logging in.");
                return;
            }

            var data = JsonSerializer.Serialize(LoginData);
            var token = await RestService.CallAPIMethod<string>(Constants.API_LOGIN_PATH, Services.HttpMethod.Post, data);

            if (string.IsNullOrEmpty(token))
            {
                await Application.Current.MainPage.DisplaySnackbar("Login credentials did not work.");
                return;
            }

            RestService.SetBearer(token);
            await Shell.Current.GoToAsync("..");
        }
    }
}
