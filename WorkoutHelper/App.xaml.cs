using WorkoutHelper.Views;

namespace WorkoutHelper
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
            Application.Current.UserAppTheme = AppTheme.Dark;

            Routing.RegisterRoute(nameof(ExerciseEditorViewPage), typeof(ExerciseEditorViewPage));
            Routing.RegisterRoute(nameof(LoginViewPage), typeof(LoginViewPage));
            Routing.RegisterRoute(nameof(AddExerciseViewPage), typeof(AddExerciseViewPage));
        }
    }
}
