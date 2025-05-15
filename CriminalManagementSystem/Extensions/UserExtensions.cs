using CriminalManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CriminalManagementSystem.Extensions
{
    public static class UserExtensions
    {
        public static string FullName(this User user)
        {
            return $"{user.FirstName} {user.LastName}";
        }
    }
}