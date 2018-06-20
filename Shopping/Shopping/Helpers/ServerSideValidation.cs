using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Shopping.Models;

namespace Shopping.Helpers
{
    public static class ServerSideValidation
    {
        public static bool ValidateInfo(RegisterUserModel user)
        {
            return false;
        }

        public static bool ValidateInfo(UserPassModel userPass)
        {
            Regex regex = new Regex("^(?=.{5,15}$)(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$");

            Match userMatch = regex.Match(userPass.username);
            Match passMatch = regex.Match(userPass.password);

            if (!userMatch.Success || !passMatch.Success) {
                return false;
            }
            return true;
        }
    }
}
