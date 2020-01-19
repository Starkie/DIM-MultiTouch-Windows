namespace Dim.MultiTouch.App.Excercise3
{
    using Windows.Foundation;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;

    /// <summary>
    ///     A page with an image. Allows the scale and rotation of the centre image with gestures.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly CompositeTransform deltaTransformation;

        /// <summary> Initializes a new instance of the <see cref="MainPage"/> class. </summary>
        public MainPage()
        {
            this.InitializeComponent();

            this.Photo.ManipulationMode = ManipulationModes.Rotate
                | ManipulationModes.Scale;

            this.Photo.ManipulationDelta +=
                new ManipulationDeltaEventHandler(this.Photo_ManipulationDelta);

            // Create a transformation to be used to apply them to the image.
            // Reference: https://github.com/microsoft/Windows-universal-samples/blob/master/Samples/BasicInput/cs/4-XAMLManipulations.xaml.cs
            this.deltaTransformation = new CompositeTransform();
            this.Photo.RenderTransform = this.deltaTransformation;

            // Set the transformation to affect the centre of the image.
            this.Photo.RenderTransformOrigin = new Point(0.5, 0.5);
        }

        private void Photo_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            // Apply the delta transformations to the photo.
            this.deltaTransformation.Rotation += e.Delta.Rotation;

            this.deltaTransformation.ScaleX *= e.Delta.Scale;
            this.deltaTransformation.ScaleY *= e.Delta.Scale;
        }
    }
}