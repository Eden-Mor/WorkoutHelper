
using WorkoutHelper.Data;
using WorkoutHelper.Models;
using WorkoutHelper.Shared.Models;

namespace WorkoutHelper.Views;

[QueryProperty(nameof(ExerciseID), nameof(IIDProperty.Id))]
public partial class ExerciseEditorViewPage : ContentPage
{
    public static IEnumerable<DaysEnum> DaysEnumList = Enum.GetValues(typeof(DaysEnum)).Cast<DaysEnum>();
    public ExerciseEditorViewPage()
    {
        InitializeComponent();
        InitDB();

        int[] arr = Enumerable.Range(0, 200).ToArray();

        RepsPicker.ItemsSource = arr;
        SetPicker.ItemsSource = arr;
        WeightPicker.ItemsSource = arr;
        MachineNumberPicker.ItemsSource = arr;

        DaysPicker.ItemsSource = (System.Collections.IList)DaysEnumList;
    }


    IdTableDB<PersonalizedExerciseData> dataDatabase;
    IdTableDB<Exercise> infoDatabase;
    public int ExerciseID { set { GetExerciseData(value); } }

    private CombinedExerciseData? exercise = new();

    private async void GetExerciseData(int id)
    {
        exercise.ExerciseInfo = (await infoDatabase.GetItemsAsync(id)).First();

        var personalData = (await dataDatabase.GetItemsAsync(id)).First();
        if (personalData != null)
            exercise.PersonalizedExerciseData = personalData;

        BindingContext = exercise;
    }

    private async void InitDB()
    {
        dataDatabase = await IdTableDB<PersonalizedExerciseData>.Instance;
        infoDatabase = await IdTableDB<Exercise>.Instance;
    }

    private async void Save_Clicked(object sender, EventArgs e)
    {
        await SaveData(exercise);
    }

    public  async Task SaveData(CombinedExerciseData exercise)
    {
        await dataDatabase.SaveItemAsync(exercise.PersonalizedExerciseData);
    }

    private async void SaveAndClose_Clicked(object sender, EventArgs e)
    {
        await SaveData(exercise);
        await Shell.Current.GoToAsync($"..?idToReload={exercise.Id}");
    }
}