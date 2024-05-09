using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core.Extensions;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using WorkoutHelper.Data;
using WorkoutHelper.Models;
using WorkoutHelper.Services;
using WorkoutHelper.Shared.Models;
using WorkoutHelper.Views;

namespace WorkoutHelper.ViewModels
{
    public class AddExerciseViewModel : BaseViewModel
    {
        public Exercise? Exercise { get; set; } = new();
        public Command AddExerciseCommand { get; set; }

        public AddExerciseViewModel()
        {
            AddExerciseCommand = new(AddExercise);
        }

        private async void AddExercise(object obj)
        {
            if (string.IsNullOrEmpty(Exercise?.Name))
                return;

            var data = JsonSerializer.Serialize(Exercise);
            await RestService.CallAPIMethod<string>(Constants.API_ADD_EXERCISE_PATH, Services.HttpMethod.Post, data);
        }
    }
}
