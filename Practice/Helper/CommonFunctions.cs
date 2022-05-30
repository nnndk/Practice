namespace Practice.Helper
{
    public static class CommonFunctions
    {
        public static int CountWorkingDays(DateTime start, DateTime end)
        {
            int count = 0;

            while (start != end)
            {
                start = start.AddDays(1);
                if ((start.DayOfWeek != DayOfWeek.Saturday) && (start.DayOfWeek != DayOfWeek.Sunday))
                    count++;
            }

            return count;
        }

        public static int CountWorkingDays(DateTime start1, DateTime start2, DateTime? end1, DateTime end2)
        {
            DateTime start = GetMaxDate(start1, start2);
            DateTime end = GetMinDate(end1, end2);

            int count = 0;

            while (start != end)
            {
                start = start.AddDays(1);
                if ((start.DayOfWeek != DayOfWeek.Saturday) && (start.DayOfWeek != DayOfWeek.Sunday))
                    count++;
            }

            return count;
        }

        public static DateTime GetMinDate(DateTime? date1, DateTime date2)
        {
            if (date1 == null)
                return date2;

            if (date1 <= date2)
                return new DateTime(0 + date1.Value.Year, 0 + date1.Value.Month, 0 + date1.Value.Day);

            return date2;
        }

        public static DateTime GetMaxDate(DateTime date1, DateTime date2)
        {
            if (date1 >= date2)
                return date1;

            return date2;
        }
    }
}
