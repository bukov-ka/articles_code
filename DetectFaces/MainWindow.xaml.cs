using Amazon;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DetectFaces
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        AmazonRekognitionClient _amazonRekognitionClient;
        BitmapImage[] bitmaps;
        int _index;
        int Index
        {
            get { return _index; }
            set
            {
                if (bitmaps == null) _index = 0;
                else if (value >= bitmaps.Length) _index = 0;
                else if (value < 0) _index = bitmaps.Length - 1;
                else _index = value;

                ClearDetectionOutput();
                Console.WriteLine("Detection log:");
                OnPropertyChanged();
                OnPropertyChanged("CurrentImage");
            }
        }

        /// <summary>
        /// Clear log and features detected.
        /// </summary>
        private void ClearDetectionOutput()
        {
            outputBox.Clear();
            canvas.Children.Clear();
            faceDetails = null;
        }

        List<FaceDetail> faceDetails;
        public BitmapImage CurrentImage
        {
            get
            {
                if (bitmaps == null) return null;
                return bitmaps[Index];
            }
        }

        public MainWindow()
        {
            var awsAccessKeyId = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
            var awsSecretAccessKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
            _amazonRekognitionClient = new AmazonRekognitionClient(awsAccessKeyId, awsSecretAccessKey, RegionEndpoint.EUWest2);
            FillBitmaps();
            InitializeComponent();
            photoBox.DataContext = this;
            Console.SetOut(new TextBoxWriter(outputBox));
        }


        async void Detect()
        {
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(CurrentImage));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            await AnalizeAsync(data, 90);
        }

        private void FillBitmaps()
        {
            var images = Directory.GetFiles("assets");
            this.bitmaps = images.Select(s =>
            {
                var image = File.ReadAllBytes(images[0]);
                BitmapImage bi = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\" + s, UriKind.Absolute));
                return bi;
            }
            ).ToArray();
        }

        public async Task AnalizeAsync(byte[] image, float confidence)
        {
            using (var source = new MemoryStream(image))
            {
                var request = new DetectFacesRequest { Image = new Amazon.Rekognition.Model.Image() { Bytes = source }, Attributes = new List<string> { "ALL" } };

                var response = await _amazonRekognitionClient.DetectFacesAsync(request);
                // This part can be used if you have a response saved previously
                //var response = JsonConvert.DeserializeObject<DetectFacesResponse>(File.ReadAllText("details.json", Encoding.UTF8));

                this.faceDetails = response.FaceDetails;
                // This part is for saving the response for debugging or mocking purposes
                //var exportData = JsonConvert.SerializeObject(response);
                //File.WriteAllText("details.json", exportData, Encoding.UTF8);
                var currentPerson = 0;
                foreach (var faceDetail in response.FaceDetails)
                {
                    Console.WriteLine($"Person {currentPerson++}");
                    PrintFeatures(faceDetail, confidence);

                    // Add features on the canvas
                    canvas.Children.Clear();
                    foreach (var landmark in faceDetail.Landmarks)
                    {
                        Ellipse ellipse = new Ellipse();
                        ellipse.Fill = Brushes.Blue;
                        ellipse.Width = 6;
                        ellipse.Height = 6;
                        ellipse.StrokeThickness = 1;
                        ellipse.ToolTip = landmark.Type.ToString(); // Tooltip with the landmark name
                        canvas.Children.Add(ellipse);

                        var point = GetImageCoordsForLandmark(landmark);
                        var rel = ImageToCanvasCoords(point);
                        Canvas.SetLeft(ellipse, rel.X);
                        Canvas.SetTop(ellipse, rel.Y);
                    }
                    if (noseCheckBox.IsChecked == true)
                    {
                        // Nose
                        var leftNose = faceDetail.Landmarks.Find(f => f.Type == LandmarkType.NoseLeft);
                        var rightNose = faceDetail.Landmarks.Find(f => f.Type == LandmarkType.NoseRight);
                        var centerNose = faceDetail.Landmarks.Find(f => f.Type == LandmarkType.Nose);
                        if (leftNose != null && rightNose != null && centerNose != null)
                        {
                            var p1 = GetImageCoordsForLandmark(leftNose);
                            var p2 = GetImageCoordsForLandmark(rightNose);
                            var noseDist = (p2 - p1).Length * 2;
                            var img = new System.Windows.Controls.Image();
                            img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\assets\\additional\\nose.png", UriKind.Absolute));
                            img.Width = noseDist;
                            img.Height = noseDist;
                            var nosePos = GetImageCoordsForLandmark(centerNose);
                            var rel = ImageToCanvasCoords(new System.Windows.Point(nosePos.X - img.Width / 2, nosePos.Y - img.Height / 2));
                            Canvas.SetLeft(img, rel.X);
                            Canvas.SetTop(img, rel.Y);
                            Canvas.SetZIndex(img, -1);
                            canvas.Children.Add(img);

                        }
                    }

                    if (glassesCheckBox.IsChecked == true)
                    {
                        // Glasses
                        var leftEye = faceDetail.Landmarks.Find(f => f.Type == LandmarkType.LeftPupil);
                        var rightEye = faceDetail.Landmarks.Find(f => f.Type == LandmarkType.RightPupil);
                        if (leftEye != null && rightEye != null)
                        {
                            var p1 = GetImageCoordsForLandmark(leftEye);
                            var p2 = GetImageCoordsForLandmark(rightEye);
                            var eyeDist = (p2 - p1).Length;
                            var angle = Vector.AngleBetween(p2 - p1, new Vector(1, 0));
                            var width = 2997 * (eyeDist / 1542) * 1.3;
                            var height = 1050 * (eyeDist / 1542) * 1.3;
                            var img = new System.Windows.Controls.Image();
                            img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\assets\\additional\\glasses.png", UriKind.Absolute));
                            img.Width = width;
                            img.Height = height;
                            var tg = new TransformGroup();
                            tg.Children.Add(new TranslateTransform(-img.Width / 2, -img.Height / 2));
                            tg.Children.Add(new RotateTransform(-angle));
                            img.RenderTransform = tg;
                            var nosePos = new System.Windows.Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
                            var rel = ImageToCanvasCoords(new System.Windows.Point(nosePos.X, nosePos.Y));
                            Canvas.SetLeft(img, rel.X);
                            Canvas.SetTop(img, rel.Y);
                            Canvas.SetZIndex(img, -2);
                            canvas.Children.Add(img);
                        }
                    }
                }

            }
        }

        private System.Windows.Point GetImageCoordsForLandmark(Landmark leftNose)
        {
            return new System.Windows.Point(leftNose.X * photoBox.Source.Width, leftNose.Y * photoBox.Source.Height);
        }

        private System.Windows.Point ImageToCanvasCoords(System.Windows.Point point)
        {
            return photoBox.TranslatePoint(new System.Windows.Point(point.X, point.Y), canvas);
        }

        private static void PrintFeatures(FaceDetail faceDetail, float confidence)
        {
            // Print properties which have Convidence percentage
            foreach (var property in typeof(FaceDetail).GetProperties())
            {
                var val = property.GetValue(faceDetail);
                if (val == null) continue;
                var confidenceProperty = val.GetType().GetProperty("Confidence");
                if (confidenceProperty != null)
                {
                    var conf = (float)confidenceProperty.GetValue(val);
                    if (conf >= confidence)
                    {
                        var existance = val.GetType().GetProperty("Value").GetValue(val);
                        Console.WriteLine("{0}: {1}", property.Name, existance);
                    }
                }
            }
            // Age
            if (faceDetail.AgeRange != null)
            {
                Console.WriteLine("Age from {0} to {1}", faceDetail.AgeRange.Low, faceDetail.AgeRange.High);
            }
            // Emotions
            foreach (var emotion in faceDetail.Emotions)
            {
                if (emotion.Confidence > confidence) { Console.WriteLine($"this person may feel {emotion.Type}"); }
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Detect();
        }

        private void ScrollPhotosLeft_Click(object sender, RoutedEventArgs e)
        {
            this.Index--;
        }

        private void ScrollPhotosRight_Click(object sender, RoutedEventArgs e)
        {
            this.Index++;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion

    }
}
