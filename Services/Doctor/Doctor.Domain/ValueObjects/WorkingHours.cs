using Doctor.Domain;

namespace Doctor.Domain.ValueObjects
{
    public class WorkingHours : IEquatable<WorkingHours>, IComparable<WorkingHours>
    {
        public TimeSpan StartTime { get; }
        public TimeSpan EndTime { get; }

        public WorkingHours(TimeSpan startTime, TimeSpan endTime)
        {
            if (startTime >= endTime)
                throw new DomainException(
                    "Start time must be before end time",
                    "INVALID_WORKING_HOURS");

            if (startTime < TimeSpan.Zero || startTime >= TimeSpan.FromHours(24))
                throw new DomainException(
                    "Invalid start time. Must be between 00:00 and 23:59",
                    "INVALID_START_TIME");

            if (endTime <= TimeSpan.Zero || endTime > TimeSpan.FromHours(24))
                throw new DomainException(
                    "Invalid end time. Must be between 00:00 and 23:59",
                    "INVALID_END_TIME");

            if (endTime - startTime < TimeSpan.FromHours(1))
                throw new DomainException(
                    "Working hours must be at least 1 hour",
                    "WORKING_HOURS_TOO_SHORT");

            StartTime = startTime;
            EndTime = endTime;
        }

        public bool IsWithinWorkingHours(TimeSpan time)
        {
            return time >= StartTime && time <= EndTime;
        }

        public TimeSpan GetDuration()
        {
            return EndTime - StartTime;
        }

        public bool Overlaps(WorkingHours other)
        {
            return StartTime < other.EndTime && EndTime > other.StartTime;
        }

        public bool Contains(WorkingHours other)
        {
            return StartTime <= other.StartTime && EndTime >= other.EndTime;
        }

        // IEquatable<WorkingHours> Implementation
        public bool Equals(WorkingHours? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return StartTime == other.StartTime && EndTime == other.EndTime;
        }

        // Override Object.Equals
        public override bool Equals(object? obj)
        {
            return Equals(obj as WorkingHours);
        }

        // Override GetHashCode
        public override int GetHashCode()
        {
            return HashCode.Combine(StartTime, EndTime);
        }

        // IComparable<WorkingHours> Implementation
        public int CompareTo(WorkingHours? other)
        {
            if (other is null) return 1;

            var startComparison = StartTime.CompareTo(other.StartTime);
            if (startComparison != 0) return startComparison;

            return EndTime.CompareTo(other.EndTime);
        }

        // Operator Overloads
        public static bool operator ==(WorkingHours? left, WorkingHours? right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(WorkingHours? left, WorkingHours? right)
        {
            return !(left == right);
        }

        public static bool operator <(WorkingHours left, WorkingHours right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(WorkingHours left, WorkingHours right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(WorkingHours left, WorkingHours right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(WorkingHours left, WorkingHours right)
        {
            return left.CompareTo(right) >= 0;
        }

        // ToString Override
        public override string ToString()
        {
            return $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
        }

        // Factory Methods
        public static WorkingHours CreateFromStrings(string startTime, string endTime)
        {
            if (!TimeSpan.TryParse(startTime, out var start))
                throw new DomainException($"Invalid start time format: {startTime}", "INVALID_TIME_FORMAT");

            if (!TimeSpan.TryParse(endTime, out var end))
                throw new DomainException($"Invalid end time format: {endTime}", "INVALID_TIME_FORMAT");

            return new WorkingHours(start, end);
        }

        public static WorkingHours DefaultWorkingHours()
        {
            return new WorkingHours(
                new TimeSpan(9, 0, 0),  // 9:00 AM
                new TimeSpan(17, 0, 0)  // 5:00 PM
            );
        }

        public static WorkingHours CreateFromHours(int startHour, int endHour)
        {
            return new WorkingHours(
                new TimeSpan(startHour, 0, 0),
                new TimeSpan(endHour, 0, 0)
            );
        }
    }
}