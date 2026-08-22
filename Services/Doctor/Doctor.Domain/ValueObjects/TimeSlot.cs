namespace Doctor.Domain.ValueObjects
{
    public class TimeSlot : IEquatable<TimeSlot>, IComparable<TimeSlot>
    {
        public TimeSpan StartTime { get; }
        public TimeSpan EndTime { get; }

        public TimeSlot(TimeSpan startTime, TimeSpan endTime)
        {
            if (startTime >= endTime)
                throw new DomainException(
                    "Start time must be before end time",
                    "INVALID_TIME_SLOT");

            if (startTime < TimeSpan.Zero || startTime >= TimeSpan.FromHours(24))
                throw new DomainException(
                    "Invalid start time",
                    "INVALID_START_TIME");

            if (endTime <= TimeSpan.Zero || endTime > TimeSpan.FromHours(24))
                throw new DomainException(
                    "Invalid end time",
                    "INVALID_END_TIME");

            StartTime = startTime;
            EndTime = endTime;
        }

        public TimeSpan Duration => EndTime - StartTime;

        public bool Overlaps(TimeSlot other)
        {
            return StartTime < other.EndTime && EndTime > other.StartTime;
        }

        public bool Contains(TimeSlot other)
        {
            return StartTime <= other.StartTime && EndTime >= other.EndTime;
        }

        public bool Intersects(TimeSlot other)
        {
            return StartTime < other.EndTime && EndTime > other.StartTime;
        }

        public bool IsAdjacentTo(TimeSlot other)
        {
            return StartTime == other.EndTime || EndTime == other.StartTime;
        }

        // IEquatable<TimeSlot> Implementation
        public bool Equals(TimeSlot? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return StartTime == other.StartTime && EndTime == other.EndTime;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as TimeSlot);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(StartTime, EndTime);
        }

        // IComparable<TimeSlot> Implementation
        public int CompareTo(TimeSlot? other)
        {
            if (other is null) return 1;

            var startComparison = StartTime.CompareTo(other.StartTime);
            if (startComparison != 0) return startComparison;

            return EndTime.CompareTo(other.EndTime);
        }

        // Operator Overloads
        public static bool operator ==(TimeSlot? left, TimeSlot? right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(TimeSlot? left, TimeSlot? right)
        {
            return !(left == right);
        }

        public static bool operator <(TimeSlot left, TimeSlot right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(TimeSlot left, TimeSlot right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(TimeSlot left, TimeSlot right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(TimeSlot left, TimeSlot right)
        {
            return left.CompareTo(right) >= 0;
        }

        public override string ToString()
        {
            return $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
        }

        public TimeSlot Merge(TimeSlot other)
        {
            if (!IsAdjacentTo(other))
                throw new DomainException(
                    "Time slots must be adjacent to merge",
                    "INVALID_MERGE");

            var newStart = StartTime < other.StartTime ? StartTime : other.StartTime;
            var newEnd = EndTime > other.EndTime ? EndTime : other.EndTime;

            return new TimeSlot(newStart, newEnd);
        }

        public static TimeSlot CreateFromStrings(string startTime, string endTime)
        {
            if (!TimeSpan.TryParse(startTime, out var start))
                throw new DomainException($"Invalid start time format: {startTime}", "INVALID_TIME_FORMAT");

            if (!TimeSpan.TryParse(endTime, out var end))
                throw new DomainException($"Invalid end time format: {endTime}", "INVALID_TIME_FORMAT");

            return new TimeSlot(start, end);
        }
    }
}