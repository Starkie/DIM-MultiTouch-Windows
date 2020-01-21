namespace Dim.MultiTouch.Collage
{
    using System;
    using System.Collections.Generic;
    using Dim.MultiTouch.Collage.Service;
    using Dim.MultiTouch.Collage.Services;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;

    /// <summary>
    ///     A page with multiple images. Allows the scale and rotation of each individual image.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly SavePhotoService saveAsPhotoService;
        private readonly LoadPhotoService loadPhotoService;

        private readonly Dictionary<string, Image> images;
        private readonly Dictionary<string, CompositeTransform> imagesTransforms;

        /// <summary> Initializes a new instance of the <see cref="MainPage"/> class. </summary>
        public MainPage()
        {
            this.InitializeComponent();

            // Initialize the canvas state collections.
            this.images = new Dictionary<string, Image>();
            this.imagesTransforms = new Dictionary<string, CompositeTransform>();

            // Initialize the photo services.
            this.saveAsPhotoService = new SavePhotoService();
            this.loadPhotoService = new LoadPhotoService();

            // Register the buttons event handlers.
            this.SaveButton.Click += new RoutedEventHandler(this.SaveClick_OnClickAsync);
            this.AddButton.Click += new RoutedEventHandler(this.AddClick_OnClickAsync);
            this.DeleteButton.Click += new RoutedEventHandler(this.DeleteButton_OnClickAsync);

            // Do not render anything outside the canvas bounds.
            Rect r = new Rect(new Point(0, 0), new Point(this.CollageCanvas.MaxWidth, this.CollageCanvas.MaxHeight));

            this.CollageCanvas.Clip = new RectangleGeometry { Rect = r };

            // Register the default images for the collage.
            this.images[this.ForestPhoto.Name] = this.ForestPhoto;
            this.images[this.OceanPhoto.Name] = this.OceanPhoto;
            this.images[this.DesertPhoto.Name] = this.DesertPhoto;

            foreach (Image image in this.images.Values)
            {
                this.SetDefaultImageConfiguration(image);
            }
        }

        /// <summary>
        ///     Configures the given image, adding the default behaviour and event handlers.
        /// </summary>
        /// <param name="image"> The image to configure. </param>
        private void SetDefaultImageConfiguration(Image image)
        {
            image.ManipulationMode = ManipulationModes.Rotate
                | ManipulationModes.Scale
                | ManipulationModes.TranslateInertia
                | ManipulationModes.TranslateX
                | ManipulationModes.TranslateY;

            image.ManipulationDelta +=
                new ManipulationDeltaEventHandler(this.Image_ManipulationDelta);

            // Create a transformation to be used to apply them to the image.
            // Reference: https://github.com/microsoft/Windows-universal-samples/blob/master/Samples/BasicInput/cs/4-XAMLManipulations.xaml.cs
            this.imagesTransforms[image.Name] = new CompositeTransform();
            image.RenderTransform = this.imagesTransforms[image.Name];

            // Set the transformation to affect the centre of the image.
            image.RenderTransformOrigin = new Point(0.5, 0.5);

            image.DoubleTapped += new DoubleTappedEventHandler(this.Image_DoubleTapped);
        }

        /// <summary> Stores the current collage as photo in the disk. </summary>
        /// <param name="sender"> The Save button. </param>
        /// <param name="e"> The arguments regarding the click. </param>
        private async void SaveClick_OnClickAsync(object sender, RoutedEventArgs e)
        {
            await this.saveAsPhotoService.SaveUiElementToFile(this.CollageCanvas);
        }

        /// <summary>
        ///     Event handler for the <see cref="DeleteButton"/>. Prompts the user to add one or
        ///     more images to the collage.
        /// </summary>
        /// <param name="sender"> The button. </param>
        /// <param name="e"> The event arguments. </param>
        private async void AddClick_OnClickAsync(object sender, RoutedEventArgs e)
        {
            IEnumerable<BitmapImage> loadedImages = await this.loadPhotoService.SelectAndLoadPhotosAsync();

            // Add the loaded images to the canvas.
            foreach (BitmapImage bitmap in loadedImages)
            {
                Image image = new Image();
                image.Name = Guid.NewGuid().ToString();
                image.Source = bitmap;
                image.Stretch = Stretch.Uniform;

                image.CenterPoint = this.CollageCanvas.CenterPoint;

                // Register the loaded image.
                this.images[image.Name] = image;
                this.SetDefaultImageConfiguration(image);
                this.CollageCanvas.Children.Add(image);
            }

            this.CollageCanvas.UpdateLayout();
        }

        /// <summary>
        ///     Event handler for the <see cref="DeleteButton"/>. Prompts the user to delete all the
        ///     images of the collage.
        /// </summary>
        /// <param name="sender"> The button. </param>
        /// <param name="e"> The event arguments. </param>
        private async void DeleteButton_OnClickAsync(object sender, RoutedEventArgs e)
        {
            ContentDialog deleteCollageDialog = new ContentDialog
            {
                Title = "Delete current collage",
                Content = "Are you sure you want to delete the current collage?",
                CloseButtonText = "No",
                PrimaryButtonText = "Yes",
            };

            ContentDialogResult contentDialogResult = await deleteCollageDialog.ShowAsync();

            if (contentDialogResult != ContentDialogResult.Primary)
            {
                return;
            }

            // Clear the images from the canvas.
            this.images.Clear();
            this.imagesTransforms.Clear();
            this.CollageCanvas.Children.Clear();
        }

        /// <summary>
        ///     When double tapped, an image is brought to the front of the canvas. Superposing
        ///     itself to the rest of images.
        /// </summary>
        /// <param name="sender"> The image. </param>
        /// <param name="e"> The event arguments. </param>
        private void Image_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            // Move the image to front when double taped.
            Image image = sender as Image;

            int index = this.CollageCanvas.Children.IndexOf(image);

            if (index != -1)
            {
                this.CollageCanvas.Children.Move((uint)index, (uint)this.CollageCanvas.Children.Count - 1);
            }
        }

        /// <summary>
        ///     Handles the transformation events of an image: scale up, rotation and translation.
        /// </summary>
        /// <param name="sender"> The image. </param>
        /// <param name="e"> The event arguments. </param>
        private void Image_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            Image image = sender as Image;

            // Apply the delta transformations to the Image.
            CompositeTransform transform = this.imagesTransforms[image.Name];

            transform.Rotation += e.Delta.Rotation;

            transform.ScaleX *= e.Delta.Scale;
            transform.ScaleY *= e.Delta.Scale;

            transform.TranslateX += e.Delta.Translation.X;
            transform.TranslateY += e.Delta.Translation.Y;
        }
    }
}