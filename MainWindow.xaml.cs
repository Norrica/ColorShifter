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
using System.Runtime.InteropServices;

namespace WpfApp1
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		static string selectedFileName = @"D:\AAA\Картинки\!1.png";
		Color initialPixel = new Color();
		double shift = 0;
		double margin = 0;
		double contrast = 0;
		double brightness = 0;
		BitmapImage displayedBitmap = new BitmapImage();
		Bitmap actualBitmap = new Bitmap(selectedFileName);

		public MainWindow()
		{
			InitializeComponent();
			displayedBitmap = actualBitmap.ToBitmapImage();
			ImageControl.Source = displayedBitmap;
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
			//MessageBox.Show((initialPixel.GetHue()/360.0F).ToString());
			//MessageBox.Show(shift + " " + margin + "\n" + contrast);

			
			
			
			ShiftHue();
			displayedBitmap = actualBitmap.ToBitmapImage();
			ImageControl.Source = displayedBitmap;

		}

		private void ShiftHue()
		{
			if (selectedFileName.Length > 0)
			{
				for (int i = 0; i < actualBitmap.Width; i++)
				{
					for (int j = 0; j < actualBitmap.Height; j++)
					{
						var pix = actualBitmap.GetPixel(i, j);
						var hue = pix.GetHue() / 360.0F;
						if ((hue < initialPixel.GetHue() / 360.0F + (float)margin) &&
							(hue > initialPixel.GetHue() / 360.0F - (float)margin))
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

							var respix = ColorRGB.FromHSLA(hue, pix.GetSaturation(), pix.GetBrightness(), pix.A);							
							actualBitmap.SetPixel(i, j, respix);
						}

					}
				}
			}
		}

		private void ShiftBrightness(float brightness)
		{
			if (selectedFileName.Length > 0)
			{
				for (int i = 0; i < actualBitmap.Width; i++)
				{
					for (int j = 0; j < actualBitmap.Height; j++)
					{
						var pix = actualBitmap.GetPixel(i, j);
						var bright = pix.GetBrightness();
						if (bright+brightness<1 && bright+brightness>0)
						{
							var respix = ColorRGB.FromHSLA(pix.GetHue(), pix.GetSaturation(), bright+brightness, pix.A);
							actualBitmap.SetPixel(i, j, respix);

						}
					}
				}

			}
		}
		private void ImageControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			//System.Windows.Point Point = new System.Windows.Point();
			System.Windows.Point point = e.MouseDevice.GetPosition(ImageControl);
			var x = (int)Math.Round(point.X);
			var y = (int)Math.Round(point.Y);
			initialPixel = actualBitmap.GetPixel(x, y);
			MessageBox.Show(initialPixel.GetBrightness().ToString());
		}

		private void Shifter_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			shift = e.NewValue;
		}

		private void Threshold_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			margin = e.NewValue;

		}
		private void Contra_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			contrast = e.NewValue;
			actualBitmap = actualBitmap.Contrast((int)(Math.Round(contrast)));
			displayedBitmap = actualBitmap.ToBitmapImage();
			ImageControl.Source = displayedBitmap;
		}
		private void Save_Click(object sender, RoutedEventArgs e)
		{

		}

		private void Brig_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			brightness = e.NewValue;
			ShiftBrightness((float)brightness);
			displayedBitmap = actualBitmap.ToBitmapImage();
			ImageControl.Source = displayedBitmap;
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
		public static Bitmap Contrast(this Bitmap sourceBitmap, int threshold)
		{
			BitmapData sourceData = sourceBitmap.LockBits(new System.Drawing.Rectangle(0, 0,
										sourceBitmap.Width, sourceBitmap.Height),
										ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);


			byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];


			Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);


			sourceBitmap.UnlockBits(sourceData);


			double contrastLevel = Math.Pow((100.0 + threshold) / 100.0, 2);


			double blue = 0;
			double green = 0;
			double red = 0;


			for (int k = 0; k + 4 < pixelBuffer.Length; k += 4)
			{
				blue = ((((pixelBuffer[k] / 255.0) - 0.5) *
							contrastLevel) + 0.5) * 255.0;


				green = ((((pixelBuffer[k + 1] / 255.0) - 0.5) *
							contrastLevel) + 0.5) * 255.0;


				red = ((((pixelBuffer[k + 2] / 255.0) - 0.5) *
							contrastLevel) + 0.5) * 255.0;


				if (blue > 255)
				{ blue = 255; }
				else if (blue < 0)
				{ blue = 0; }


				if (green > 255)
				{ green = 255; }
				else if (green < 0)
				{ green = 0; }


				if (red > 255)
				{ red = 255; }
				else if (red < 0)
				{ red = 0; }


				pixelBuffer[k] = (byte)blue;
				pixelBuffer[k + 1] = (byte)green;
				pixelBuffer[k + 2] = (byte)red;
			}


			Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);


			BitmapData resultData = resultBitmap.LockBits(new System.Drawing.Rectangle(0, 0,
										resultBitmap.Width, resultBitmap.Height),
										ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);


			Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
			resultBitmap.UnlockBits(resultData);


			return resultBitmap;
		}
	}
}

