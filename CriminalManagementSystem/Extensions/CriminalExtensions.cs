using CriminalManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CriminalManagementSystem.Extensions
{
    public static class CriminalExtensions
    {
        public static string FullName(this Criminal criminal)
        {
            return $"{criminal.FirstName} {criminal.MiddleName} {criminal.LastName}";
        }
    }
}