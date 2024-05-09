using System;
using WorkoutHelper.Data;
using WorkoutHelper.Models;

namespace WorkoutHelper.Services
{
    public static class WorkoutColorService
    {
        private static int MaxWorkoutCount { get; set; } = -1;
        public static Color GetColorByIndex(int? index) => ColorGenerator.GetColorByIndex(index ?? 0, MaxWorkoutCount);

        static WorkoutColorService() => SetupCount();

        private static void SetupCount() => MaxWorkoutCount = GetItemsCount();

        private static int GetItemsCount() => Enum.GetNames(typeof(DaysEnum)).Length;
    }
}
