using System;

namespace Stories.Tools
{
    public static class Dob
    {
        public static bool AllowedAge(DateTime dateOfBirth)
        {
            DateTime Now = DateTime.Now;
            int years = new DateTime(DateTime.Now.Subtract(dateOfBirth).Ticks).Year - 1;
            if ((new DateTime(DateTime.Now.Subtract(dateOfBirth).Ticks).Year - 1) > Settings.ageLimit) return true;
            return false;
        }
    }
}