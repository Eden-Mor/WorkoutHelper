using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutHelper.Data
{
    public static class Constants
    {
        public const string API_BASE_PATH =
//#if DEBUG
//            "http://localhost:5000/";
//#else
            "https://workouthelper.api.edenmor.com/";
//#endif
        public const string API_GET_EXERCISES_PATH = "Exercise/Exercises";
        public const string API_LOGIN_PATH = "Login/Login";
        public const string API_ADD_EXERCISE_PATH = "Exercise/AddExercise";
    }
}
