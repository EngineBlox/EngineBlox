using System;

namespace EngineBlox.Test.Utility.Identifiers
{
    public static class DateIdFactory
    {
        /// <summary>
        /// Generate a unique(ish) int-based id from timestamp.
        /// Sufficient for general testing purposes.
        /// Right hand side prioritises seconds, minutes, hours to promote readability / debug tracability.
        /// Left hand side prioritises higher uniqueness with longer length ids via milliseconds / ticks.
        /// </summary>
        /// <param name="length">The length of the id to generate</param>
        /// <returns>An int based id from timestamp information</returns>
        public static string GetTimestampIdString(int length)
        {
            var baseId = $"{DateTime.Now.Ticks}{DateTime.Now.Millisecond:D4}{DateTime.Now.Year:D4}{DateTime.Now.Day:D2}{DateTime.Now.Hour:D2}{DateTime.Now.Minute:D2}{DateTime.Now.Second:D2}";

            if (length <= 0 || length > baseId.Length) throw new TestException($"{nameof(length)} must be greater than 0 or max {baseId.Length}");

            return baseId[(baseId.Length - length + 1)..];
        }

        public static int GetTimestampId(int length)
        {
            try
            {
                return int.Parse(GetTimestampIdString(length));
            }
            catch (Exception ex)
            {
                throw new TestException($"Unable to parse id to int. Length specified is too long", ex);
            }
        }
    }
}
