using SkiaSharp;
using System;

namespace BadgeGenerator
{
	public abstract class BadgeRenderer : IDisposable
	{
		private SKTypeface typeface;
		private bool disposedValue;

		protected BadgeRenderer(Options options)
		{
			Options = options;
		}

		public Options Options { get; }

		public virtual SKTypeface Typeface
		{
			get
			{
				if (typeface != null)
					return typeface;

				foreach (var family in new[] { "DejaVu Sans", "Verdana", "Geneva", "sans-serif" })
				{
					typeface = SKTypeface.FromFamilyName(family);
					if (typeface != null)
						return typeface;
				}

				typeface = SKTypeface.Default;

				return typeface;
			}
		}

		public abstract SKSizeI CalculateBounds();

		public abstract void Render(SKCanvas canvas, SKImageInfo info);

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposedValue)
				return;

			if (disposing)
			{
				typeface?.Dispose();
				typeface = null;
			}

			disposedValue = true;
		}
	}
}
