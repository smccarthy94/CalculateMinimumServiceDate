using System;
using NodaTime;
using NUnit.Framework;
using ServiceDate.Core;

namespace ServiceDate.Tests.NodaTimeExtensions
{
    public class WhenGettingNextWeekdayAfterBusinessDays
    {
        [TestCase("Wednesday, 21 April 2021", 2, ExpectedResult = "Friday, 23 April 2021")]
        [TestCase("Thursday, 22 April 2021", 2, ExpectedResult = "Monday, 26 April 2021")]
        [TestCase("Friday, 23 April 2021", 2, ExpectedResult = "Tuesday, 27 April 2021")]
        [TestCase("Wednesday, 21 April 2021", 1, ExpectedResult = "Thursday, 22 April 2021")]
        [TestCase("Thursday, 22 April 2021", 1, ExpectedResult = "Friday, 23 April 2021")]
        [TestCase("Friday, 23 April 2021", 1, ExpectedResult = "Monday, 26 April 2021")]
        public string NextBusinessDayAfter(string input, int days)
        {
            var date = LocalDate.FromDateTime(DateTime.Parse(input));
            return date.NextWeekdayAfterBusinessDays(days).ToString();
        }
    }
}