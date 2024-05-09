using WorkoutHelper.Data;
using WorkoutHelper.Models;

namespace WorkoutHelper.Views.DataTemplateSelectors;

public partial class WorkoutItemDataTemplate : ResourceDictionary
{
    public IdTableDB<PersonalizedExerciseData>? personalExerciseInfoDB;

    public WorkoutItemDataTemplate()
	{
		InitializeComponent();

        ModifyEntry();
        ModifySelector();

        InitializeDB();
    }

    private void ModifyEntry()
    {
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("MyCustomization", (handler, view) =>
        {

#if ANDROID
#elif WINDOWS
            handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
#endif
        });
    }

    private void ModifySelector()
    {
        Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("MyCustomization", (handler, view) =>
        {

#if ANDROID
#elif WINDOWS
            handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
#endif
        });
    }

    private async void InitializeDB() => personalExerciseInfoDB = await IdTableDB<PersonalizedExerciseData>.Instance;

    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is not View view || view.BindingContext is not CombinedExerciseData combined)
            return;

        Save(combined);
    }

    private void Save_Unfocused(object sender, FocusEventArgs e)
    {
        if (sender is not View view || view.BindingContext is not CombinedExerciseData data)
            return;

        Save(data);
    }

    private async void Save(CombinedExerciseData data)
    {
        if (data.PersonalizedExerciseData == null || personalExerciseInfoDB == null)
            return;

        try
        {
            data.PersonalizedExerciseData.Id = data.ExerciseInfo.Id;
            await personalExerciseInfoDB.SaveAddItemAsync(data.PersonalizedExerciseData);
        }
        catch (Exception ex)
        {
        }
    }
}