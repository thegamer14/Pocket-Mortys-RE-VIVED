using System;

public class Range<T> where T : IComparable<T>
{
	public T Minimum { get; set; }

	public T Maximum { get; set; }

	public Range(T minimum, T maximum)
	{
		Minimum = minimum;
		Maximum = maximum;
	}

	public override string ToString()
	{
		return string.Format("[{0} - {1}]", Minimum, Maximum);
	}

	public bool IsValid()
	{
		return Minimum.CompareTo(Maximum) <= 0;
	}

	public bool ContainsValue(T value)
	{
		return Minimum.CompareTo(value) <= 0 && value.CompareTo(Maximum) <= 0;
	}

	public bool IsInsideRange(Range<T> Range)
	{
		return IsValid() && Range.IsValid() && Range.ContainsValue(Minimum) && Range.ContainsValue(Maximum);
	}

	public bool ContainsRange(Range<T> Range)
	{
		return IsValid() && Range.IsValid() && ContainsValue(Range.Minimum) && ContainsValue(Range.Maximum);
	}
}
