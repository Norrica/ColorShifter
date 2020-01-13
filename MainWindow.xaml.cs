//using Microsoft.Win32;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
//using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;
using System.Drawing.Imaging;

namespace WpfApp1
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		string selectedFileName = null;
		Color initialPixel = new Color();
		double shift = 0;
		double margin = 0;
		BitmapImage displayedBitmap = new BitmapImage();
		Bitmap actualBitmap = null;

		public MainWindow()
		{
			InitializeComponent();
		}
		private void BrowseButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog
			{
				InitialDirectory = "c:\\",
				Filter = "Image files (*.jpg)|*.jpg|Image files (*.png)|*.png|All Files (*.*)|*.*",
				RestoreDirectory = true
			};
			if (dlg.ShowDialog().HasValue)
			{
				selectedFileName = dlg.FileName;
				actualBitmap = new Bitmap(selectedFileName);
				displayedBitmap = actualBitmap.ToBitmapImage();
				ImageControl.Source = displayedBitmap;

			}
		}
		private void RotateButton_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show(shift + " " + margin + "\n" + initialPixel.ToString());
			if (selectedFileName.Length > 0)
			{
				
				for (int i = 0; i < actualBitmap.Width; i++)
				{
					for (int j = 0; j < actualBitmap.Height; j++)
					{
						var pix = actualBitmap.GetPixel(i, j);
						var hue = pix.GetHue()/360.0F;
						if (!(hue > initialPixel.GetHue() + (float)margin) ||
							!(hue < initialPixel.GetHue() - (float)margin))
						{
							if (hue + shift > 1)
							{
								hue = (float)(-1 + hue + shift);
							}
							else if (hue + shift < 0)
							{
								hue = (float)(1 + hue + shift);
							}
							else
							{
								hue = hue + (float)shift;
							}
							
							var respix = ColorRGB.FromHSLA(hue, pix.GetSaturation(), pix.GetBrightness(),pix.A);
							Color color = respix;
							actualBitmap.SetPixel(i, j, color);
						}

					}
				}
				displayedBitmap = actualBitmap.ToBitmapImage();
				ImageControl.Source = displayedBitmap;
			}

		}


		private void ImageControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			System.Windows.Point Point = new System.Windows.Point();
			Point = e.MouseDevice.GetPosition(ImageControl);
			var x = (int)Math.Round(Point.X);
			var y = (int)Math.Round(Point.Y);
			initialPixel = actualBitmap.GetPixel(x, y);
		}

		private void Shifter_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			shift = e.NewValue;

		}

		private void Threshold_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			margin = e.NewValue;

		}
	}
	public class ColorRGB
	{
		public byte R;
		public byte G;
		public byte B;
		public byte A;

		public ColorRGB()
		{
			R = 255;
			G = 255;
			B = 255;
			A = 255;
		}

		public ColorRGB(Color value)
		{

			this.R = value.R;
			this.G = value.G;
			this.B = value.B;
			this.A = value.A;
		}

		public static implicit operator Color(ColorRGB rgb)
		{
			Color c = Color.FromArgb(rgb.A, rgb.R, rgb.G, rgb.B);
			return c;
		}

		public static explicit operator ColorRGB(Color c)
		{
			return new ColorRGB(c);
		}

		// Given H,S,L in range of 0-1
		// Returns a Color (RGB struct) in range of 0-255
		public static ColorRGB FromHSL(double H, double S, double L)
		{
			return FromHSLA(H, S, L, 1.0);
		}

		// Given H,S,L,A in range of 0-1
		// Returns a Color (RGB struct) in range of 0-255
		public static ColorRGB FromHSLA(double H, double S, double L, double A)
		{
			double v;
			double r, g, b;

			if (A > 1.0)
				A = 1.0;

			r = L;   // default to gray
			g = L;
			b = L;
			v = (L <= 0.5) ? (L * (1.0 + S)) : (L + S - L * S);

			if (v > 0)
			{
				double m;
				double sv;
				int sextant;
				double fract, vsf, mid1, mid2;

				m = L + L - v;
				sv = (v - m) / v;
				H *= 6.0;
				sextant = (int)H;
				fract = H - sextant;
				vsf = v * sv * fract;
				mid1 = m + vsf;
				mid2 = v - vsf;

				switch (sextant)
				{
					case 0:
						r = v;
						g = mid1;
						b = m;
						break;

					case 1:
						r = mid2;
						g = v;
						b = m;
						break;

					case 2:
						r = m;
						g = v;
						b = mid1;
						break;

					case 3:
						r = m;
						g = mid2;
						b = v;
						break;

					case 4:
						r = mid1;
						g = m;
						b = v;
						break;

					case 5:
						r = v;
						g = m;
						b = mid2;
						break;
				}
			}

			ColorRGB rgb = new ColorRGB
			{
				R = Convert.ToByte(r * 255.0f),
				G = Convert.ToByte(g * 255.0f),
				B = Convert.ToByte(b * 255.0f),
				A = Convert.ToByte(A * 255.0f)
			};
			return rgb;
		}

		// Hue in range from 0.0 to 1.0
		public float H
		{
			get
			{
				// Use System.Drawing.Color.GetHue, but divide by 360.0F 
				// because System.Drawing.Color returns hue in degrees (0 - 360)
				// rather than a number between 0 and 1.
				return ((Color)this).GetHue() / 360.0F;
			}

		}

		// Saturation in range 0.0 - 1.0
		public float S
		{
			get
			{
				return ((Color)this).GetSaturation();
			}
		}

		// Lightness in range 0.0 - 1.0
		public float L
		{
			get
			{
				return ((Color)this).GetBrightness();
			}
		}
	}
	public static class BmpExtinsion
	{
		public static BitmapImage ToBitmapImage(this Bitmap bitmap)
		{
			using (var memory = new MemoryStream())
			{
				bitmap.Save(memory, ImageFormat.Png);
				memory.Position = 0;

				var bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.StreamSource = memory;
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.EndInit();
				bitmapImage.Freeze();

				return bitmapImage;
			}
		}

	}
}

