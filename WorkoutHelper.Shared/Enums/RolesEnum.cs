using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace WorkoutHelper.Shared.Enums
{
    public enum RolesEnum
    {
        User = 0,
        LoggedInUser = 10,
        TrustedContributor = 50,
        Admin = 100,
    }
}
