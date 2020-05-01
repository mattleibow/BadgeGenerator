using SkiaSharp;
using System.Diagnostics;

namespace BadgeGenerator
{
	public class FlatBadgeRenderer : BadgeRenderer
	{
		private const int HorizontalPadding = 5;
		private const int Height = 20;
		private const int TextSize = 11;
		private const int CornerRadius = 3;

		// overlay graient
		private static readonly SKColor OverlayGradientTop = 0x19BBBBBB;
		private static readonly SKColor OverlayGradientBottom = 0x19000000;
		private static readonly SKColor[] OverlayGradientColors =
		{
			OverlayGradientTop,
			OverlayGradientBottom
		};

		private bool disposedValue;

		// cached values
		private SKPaint textPaint;
		private SKRectI bounds;
		private float labelTextWidth;
		private float messageTextWidth;

		public FlatBadgeRenderer(Options options)
			: base(options)
		{
		}

		private SKPaint GetTextPaint()
		{
			if (textPaint != null)
				return textPaint;

			textPaint = new SKPaint
			{
				Typeface = Typeface,
				TextSize = TextSize,
				Color = SKColors.White,
			};

			return textPaint;
		}

		public override SKSizeI CalculateBounds()
		{
			if (!bounds.IsEmpty)
				return bounds.Size;

			// base text size
			var tp = GetTextPaint();
			labelTextWidth = Options.Label == null ? 0 : tp.MeasureText(Options.Label);
			messageTextWidth = Options.Message == null ? 0 : tp.MeasureText(Options.Message);

			// put it all together
			float width = HorizontalPadding;
			if (labelTextWidth > 0)
				width += labelTextWidth + HorizontalPadding;
			if (messageTextWidth > 0)
				width += messageTextWidth + HorizontalPadding;
			if (labelTextWidth > 0 && messageTextWidth > 0)
				width += HorizontalPadding;

			bounds = SKRectI.Create(new SKSizeI((int)width, Height));

			return bounds.Size;
		}

		public override void Render(SKCanvas canvas, SKImageInfo info)
		{
			CalculateBounds();

			using var rrect = new SKRoundRect(bounds, CornerRadius);
			canvas.ClipRoundRect(rrect, SKClipOperation.Intersect, true);

			using var sidePaint = new SKPaint();
			var side = SKRect.Create(HorizontalPadding, Height);

			var labelRect = SKRect.Empty;
			if (labelTextWidth > 0)
			{
				side.Right += labelTextWidth + HorizontalPadding;
				sidePaint.Color = Options.LabelColor;
				labelRect = side;

				canvas.DrawRect(labelRect, sidePaint);
			}

			var messageRect = SKRect.Empty;
			if (messageTextWidth > 0)
			{
				if (labelTextWidth > 0)
				{
					side.Left = side.Width;
					side.Right += HorizontalPadding;
				}

				side.Right += messageTextWidth + HorizontalPadding;
				sidePaint.Color = Options.MessageColor;
				messageRect = side;

				canvas.DrawRect(messageRect, sidePaint);
			}

			using var gradient = SKShader.CreateLinearGradient(SKPoint.Empty, new SKPoint(0, bounds.Height), OverlayGradientColors, SKShaderTileMode.Clamp);
			using var gradientPaint = new SKPaint
			{
				IsAntialias = true,
				Shader = gradient
			};
			canvas.DrawRect(bounds, gradientPaint);

			// TODO: the text
			//
			// <g fill="#fff" text-anchor="middle" font-family="DejaVu Sans,Verdana,Geneva,sans-serif" font-size="110">
			//   <text x="645" y="150" fill="#010101" fill-opacity=".3" transform="scale(.1)" textLength="1170">HarfBuzzSharp nuget</text>
			//   <text x="645" y="140" transform="scale(.1)" textLength="1170">HarfBuzzSharp nuget</text>
			//   <text x="1545" y="150" fill="#010101" fill-opacity=".3" transform="scale(.1)" textLength="470">v2.6.1.1</text>
			//   <text x="1545" y="140" transform="scale(.1)" textLength="470">v2.6.1.1</text>
			// </g>

#if DEBUG
			var testRect = labelRect;
			testRect.Union(messageRect);
			Debug.Assert(testRect == bounds);
			testRect = labelRect;
			testRect.Intersect(messageRect);
			Debug.Assert(testRect.Width == 0);
#endif
		}

		protected override void Dispose(bool disposing)
		{
			if (disposedValue)
				return;

			if (disposing)
			{
				textPaint?.Dispose();
				textPaint = null;
			}

			disposedValue = true;
		}
	}
}
