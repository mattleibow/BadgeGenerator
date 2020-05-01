using SkiaSharp;
using System.IO;

namespace BadgeGenerator
{
	public static class Generator
	{
		public static void Generate(Stream stream, Options options, OutputType outputType = OutputType.Png, float scale = 1f)
		{
			using var renderer = GetRenderer(options);
			var bounds = renderer.CalculateBounds();

			if (outputType == OutputType.Png)
			{
				var info = new SKImageInfo((int)(bounds.Width * scale), (int)(bounds.Height * scale));
				using var surface = SKSurface.Create(info);
				using var canvas = surface.Canvas;

				canvas.Clear(SKColors.Transparent);
				canvas.Scale(scale);

				renderer.Render(canvas, info);

				using var image = surface.Snapshot();
				using var pixmap = image.PeekPixels();
				pixmap.Encode(stream, SKEncodedImageFormat.Png, 100);
			}
		}

		private static BadgeRenderer GetRenderer(Options options) =>
			options.Style switch
			{
				BadgeStyle.Flat => new FlatBadgeRenderer(options),
				_ => null
			};
	}
}
