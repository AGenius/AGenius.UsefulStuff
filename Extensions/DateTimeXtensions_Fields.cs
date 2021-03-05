using System;
using System.Collections.Generic;
namespace AGenius.UsefulStuff
{
    public static partial class DateTimeXtensions
    {
        #region Fields
        private static readonly string WeekDays;
        private static readonly string Holidays;
        private static readonly int AdultAgeLimit;

        private static readonly List<DayOfWeek> _workdaysList;
        private static readonly List<DayOfWeek> _holidaysList;
        #endregion

        #region Constructors
        /// <summary>
        /// Static constructor of the class. Used to instantiate the private members of this class.
        /// </summary>
        static DateTimeXtensions()
        {
            WeekDays = "Monday,Tuesday,Wednesday,Thursday,Friday";
            var workdaysStringList = WeekDays.Split(',');
            _workdaysList = new List<DayOfWeek>();
            foreach (var element in workdaysStringList)
            {
                _workdaysList.Add((DayOfWeek)Enum.Parse(typeof(DayOfWeek), element));
            }

            Holidays = "Saturday,Sunday";
            var holidaysStringList = Holidays.Split(',');
            _holidaysList = new List<DayOfWeek>();
            foreach (var element in holidaysStringList)
            {
                _holidaysList.Add((DayOfWeek)Enum.Parse(typeof(DayOfWeek), element));
            }

            AdultAgeLimit = 18;
        }

        #endregion
    }
}