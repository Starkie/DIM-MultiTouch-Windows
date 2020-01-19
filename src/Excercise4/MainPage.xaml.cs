namespace Dim.MultiTouch.App.Excercise4
{
    using System.Collections.Generic;
    using Windows.Foundation;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;

    /// <summary>
    ///     A page with multiple images. Allows the scale and rotation of each individual image.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly Dictionary<string, Image> images;
        private readonly Dictionary<string, CompositeTransform> imagesTransforms;

        /// <summary> Initializes a new instance of the <see cref="MainPage"/> class. </summary>
        public MainPage()
        {
            this.InitializeComponent();

            this.images = new Dictionary<string, Image>();
            this.images[this.ForestPhoto.Name] = this.ForestPhoto;
            this.images[this.OceanPhoto.Name] = this.OceanPhoto;
            this.images[this.DesertPhoto.Name] = this.DesertPhoto;

            this.imagesTransforms = new Dictionary<string, CompositeTransform>();

            foreach (Image image in this.images.Values)
            {
                image.ManipulationMode = ManipulationModes.Rotate
                    | ManipulationModes.Scale
                    | ManipulationModes.TranslateInertia
                    | ManipulationModes.TranslateX
                    | ManipulationModes.TranslateY;

                image.ManipulationDelta +=
                    new ManipulationDeltaEventHandler(this.Photo_ManipulationDelta);

                // Create a transformation to be used to apply them to the image.
                // Reference: https://github.com/microsoft/Windows-universal-samples/blob/master/Samples/BasicInput/cs/4-XAMLManipulations.xaml.cs
                this.imagesTransforms[image.Name] = new CompositeTransform();
                image.RenderTransform = this.imagesTransforms[image.Name];

                // Set the transformation to affect the centre of the image.
                image.RenderTransformOrigin = new Point(0.5, 0.5);
            }
        }

        private void Photo_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            Image image = sender as Image;

            // Apply the delta transformations to the Photo.
            this.imagesTransforms[image.Name].Rotation += e.Delta.Rotation;

            this.imagesTransforms[image.Name].ScaleX *= e.Delta.Scale;
            this.imagesTransforms[image.Name].ScaleY *= e.Delta.Scale;

            this.imagesTransforms[image.Name].TranslateX += e.Delta.Translation.X;
            this.imagesTransforms[image.Name].TranslateY += e.Delta.Translation.Y;
        }
    }
}