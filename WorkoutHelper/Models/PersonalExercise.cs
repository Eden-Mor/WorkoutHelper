using SQLite;
using WorkoutHelper.Data;
using WorkoutHelper.Services;
using WorkoutHelper.Shared.Models;

namespace WorkoutHelper.Models
{
    public class PersonalizedExerciseData : ObservableProperty, IIDProperty
    {
        private string? exerciseNote = string.Empty;
        private string? reps = string.Empty;
        private string? weight = string.Empty;
        private string? sets = string.Empty;
        private string? machineNumber = "0";
        private DateTime? completedDate = DateTime.MinValue;
        private bool isCompleted = false;
        private DaysEnum? day = DaysEnum.None;

        [PrimaryKey]
        public int? Id { get; set; } = null;

        public string? Sets
        {
            get => sets; set
            {
                SetPropertyValue(ref sets, value);
            }
        }
        public string? Weight
        {
            get => weight; set
            {
                SetPropertyValue(ref weight, value);
            }
        }
        public string? Reps
        {
            get => reps; set
            {
                SetPropertyValue(ref reps, value);
            }
        }
        public string? ExerciseNote
        {
            get => exerciseNote; set
            {
                SetPropertyValue(ref exerciseNote, value);
            }
        }

        public DateTime? CompletedDate
        {
            get => completedDate; set
            {
                SetPropertyValue(ref completedDate, value);
                OnPropertyChanged(nameof(IsCompleted));
            }
        }

        public DaysEnum? Day
        {
            get => day; set
            {
                SetPropertyValue(ref day, value);
                OnPropertyChanged(nameof(DayColor));
            }
        }
        public string? MachineNumber
        {
            get => machineNumber; set => SetPropertyValue(ref machineNumber, value);
        }

        [Ignore]
        public bool IsCompleted
        {
            get => CompletedDate.HasValue ? CompletedDate.Value.AddDays(1) > DateTime.Now : false;

            set
            {
                CompletedDate = value ? DateTime.Now : DateTime.MinValue;
                SetPropertyValue(ref isCompleted, value);
                OnPropertyChanged(nameof(CompletedDate));
                OnPropertyChanged(nameof(DayColor));
            }
        }

        [Ignore]
        public Color? DayColor { get => IsCompleted ? Color.FromRgba("#A9C0B6") : WorkoutColorService.GetColorByIndex((int?)day); }


        private static List<DaysEnum> dayList = Enum.GetValues(typeof(DaysEnum)).Cast<DaysEnum>().ToList();

        [Ignore]
        public List<DaysEnum> DayList { get => dayList; }
    }

    public class CombinedExerciseData : ObservableProperty, IIDProperty
    {
        private PersonalizedExerciseData personalizedExerciseData;
        private Exercise exerciseInfo;

        public Exercise ExerciseInfo
        {
            get => exerciseInfo; set
            {
                exerciseInfo = value;
                if (PersonalizedExerciseData != null) 
                    PersonalizedExerciseData.Id = value.Id;
            }
        }

        public PersonalizedExerciseData PersonalizedExerciseData
        {
            get => personalizedExerciseData; set
            {
                SetPropertyValue(ref personalizedExerciseData, value);
            }
        }
        public int? Id { get => ExerciseInfo.Id; set => ExerciseInfo.Id = value; }

        public CombinedExerciseData()
        {
            ExerciseInfo = new Exercise();
            PersonalizedExerciseData = new PersonalizedExerciseData();
        }
    }
}
