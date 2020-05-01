using SkiaSharp;

namespace BadgeGenerator
{
	public class Options
	{
		public string Label { get; set; }

		public string Message { get; set; }

		public SKColor LabelColor { get; set; }

		public SKColor MessageColor { get; set; }

		public BadgeStyle Style { get; set; }
	}
}
