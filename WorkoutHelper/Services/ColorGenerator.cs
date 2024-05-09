using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutHelper.Services
{
    public class ColorGenerator
    {
        public static Color GetColorByIndex(int index, int max) => Color.FromHsv(GetColorByMax(index, max), 70, 77);

        protected static int GetColorByMax(int index, int max)
        {
            int increment = max >= 0 ? 360 / max - 1 : 0;
            return index > max ? max : index * increment;
        }
    }
}
