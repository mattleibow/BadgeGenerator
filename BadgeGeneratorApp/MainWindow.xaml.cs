using BadgeGenerator;
using SkiaSharp;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BadgeGeneratorApp
{
	public partial class MainWindow : Window
	{
		private bool loaded = false;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void OnTextChanged(object sender, TextChangedEventArgs e)
		{
			if (!loaded)
				return;

			if (!SKColor.TryParse(labelColorTextBox.Text, out var labelColor))
				labelColor = SKColors.White;
			if (!SKColor.TryParse(messageColorTextBox.Text, out var messageColor))
				messageColor = SKColors.White;
			if (!float.TryParse(scaleTextBox.Text, out var scale))
				scale = 1;

			var options = new Options
			{
				Label = labelTextBox.Text,
				LabelColor = labelColor,
				Message = messageTextBox.Text,
				MessageColor = messageColor,
				Style = BadgeStyle.Flat,
			};

			var stream = new MemoryStream();

			Generator.Generate(stream, options, OutputType.Png, scale);

			File.WriteAllBytes("output.png", stream.ToArray());

			var image = new BitmapImage();
			image.BeginInit();
			stream.Position = 0;
			image.StreamSource = stream;
			image.EndInit();

			previewImage.Source = image;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			loaded = true;

			OnTextChanged(null, null);
		}
	}
}
