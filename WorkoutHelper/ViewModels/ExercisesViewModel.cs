using System.Collections.ObjectModel;
using WorkoutHelper.Data;
using WorkoutHelper.Models;
using WorkoutHelper.Services;
using WorkoutHelper.Shared.Models;
using WorkoutHelper.Views;

namespace WorkoutHelper.ViewModels
{
    [QueryProperty(nameof(ReloadID), "idToReload")]
    public class ExercisesViewModel : BaseViewModel
    {
        public int ReloadID { set { ReloadTheID(value); } }

        public IdTableDB<Exercise> exerciseInfoDB;
        public IdTableDB<PersonalizedExerciseData> personalExerciseInfoDB;
        private IDNameModel? selectedDay;
        private string? searchText;
        private bool isRefreshing = false;

        public ObservableCollection<CombinedExerciseData> Exercises { get; set; } = new();
        private List<CombinedExerciseData> BackupExercises { get; set; } = new();

        public Command SearchedPressed { get; set; }
        public Command GetExercises { get; set; }
        public Command LoginCommand { get; set; }
        public Command AddExerciseCommand { get; set; }
        public Command EditExercise { get; set; }
        public Command ClearCommand { get; set; }

        public List<IDNameModel> DaysList { get; set; }

        public IDNameModel? SelectedDay
        {
            get => selectedDay; set
            {
                SetPropertyValue(ref selectedDay, value);
                MainThread.BeginInvokeOnMainThread(Search);
            }
        }

        public bool IsRefreshing
        {
            get => isRefreshing; set => SetPropertyValue(ref isRefreshing, value);
        }

        public string? SearchText
        {
            get => searchText; set => SetPropertyValue(ref searchText, value);
        }

        public ExercisesViewModel()
        {
            AddExerciseCommand = new(async (obj) => await Shell.Current.GoToAsync($"{nameof(AddExerciseViewPage)}", true));
            ClearCommand = new(ClearSearchFilters);
            SearchedPressed = new(Search);
            LoginCommand = new(async (obj) => await Shell.Current.GoToAsync($"{nameof(LoginViewPage)}", true));
            GetExercises = new(async () => await GetExercisesFromAPI());
            EditExercise = new(async (obj) => await Shell.Current.GoToAsync($"{nameof(ExerciseEditorViewPage)}?{nameof(IIDProperty.Id)}={((IIDProperty)obj).Id}", true));

            LoadDatabaseAndInitialize();

            var list = Enum.GetValues(typeof(DaysEnum))
                       .Cast<DaysEnum>()
                       .Select(day => new IDNameModel((int?)day, day.ToString()))
                       .ToList();
            list.Insert(0, new(null, "All"));
            DaysList = list;
            SetSelectedDayToDefault();
        }

        private void SetSelectedDayToDefault()
        {
            SelectedDay = DaysList?.First();
        }

        private void ClearSearchFilters()
        {
            SetSelectedDayToDefault();
            SearchText = string.Empty;
            Search();
        }

        private void Search()
        {
            Exercises.Clear();

            List<CombinedExerciseData> listToDisplay = [.. BackupExercises];

            if (SelectedDay?.Id != null)
                listToDisplay = listToDisplay.Where((data) => (int?)data.PersonalizedExerciseData.Day == SelectedDay.Id).ToList();

            if (!string.IsNullOrEmpty(SearchText))
                listToDisplay = listToDisplay.Where((data) =>
                    (data.PersonalizedExerciseData?.MachineNumber?.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (data.PersonalizedExerciseData?.ExerciseNote?.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (data.ExerciseInfo.Name.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)).ToList();

            foreach (var exercise in listToDisplay)
                Exercises.Add(exercise);
        }

        private async void ReloadTheID(int value)
        {
            var ex = Exercises.Where((id) => id.Id == value).FirstOrDefault();
            if (ex is not CombinedExerciseData combined)
                return;

            combined.PersonalizedExerciseData = (await personalExerciseInfoDB.GetItemsAsync(value))?.FirstOrDefault() ?? new();
        }

        private async void LoadDatabaseAndInitialize()
        {
            exerciseInfoDB = await IdTableDB<Exercise>.Instance;
            personalExerciseInfoDB = await IdTableDB<PersonalizedExerciseData>.Instance;
            LoadExercisesFromDatabase();
        }

        private async void LoadExercisesFromDatabase()
        {
            try
            {
                var exerciseInfos = await exerciseInfoDB.GetItemsAsync();
                var personalExerciseInfos = await personalExerciseInfoDB.GetItemsAsync();

                if (exerciseInfos.Count != 0)
                {
                    foreach (var exercise in exerciseInfos)
                    {
                        CombinedExerciseData combined = new()
                        {
                            ExerciseInfo = exercise,
                            PersonalizedExerciseData = personalExerciseInfos.FirstOrDefault(p => p.Id == exercise.Id, new() { Id = exercise.Id, MachineNumber = "0", Day = DaysEnum.None })
                        };


                        BackupExercises.Add(combined);
                        Exercises.Add(combined);
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task GetExercisesFromAPI()
        {
            var exerciseList = await RestService.CallAPIMethod<List<Exercise>>(Constants.API_GET_EXERCISES_PATH);

            if (exerciseList == null || !exerciseList.Any())
                return;

            await exerciseInfoDB.RecreateTableAsync();

            _ = exerciseInfoDB.AddItemsAsync(exerciseList);

            Exercises.Clear();
            BackupExercises.Clear();

            foreach (var exercise in exerciseList)
            {
                CombinedExerciseData combined = new()
                {
                    ExerciseInfo = exercise
                };

                if (exercise.Id.HasValue)
                    combined.PersonalizedExerciseData = (await personalExerciseInfoDB.GetItemsAsync(exercise.Id.Value)).FirstOrDefault(new PersonalizedExerciseData());

                BackupExercises.Add(combined);
                Exercises.Add(combined);
            }

            IsRefreshing = false;
        }
    }
}
