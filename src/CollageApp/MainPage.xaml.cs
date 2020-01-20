namespace Dim.MultiTouch.Collage
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Threading.Tasks;
    using Windows.Foundation;
    using Windows.Graphics.Display;
    using Windows.Graphics.Imaging;
    using Windows.Storage;
    using Windows.Storage.Pickers;
    using Windows.UI.ViewManagement;
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
        private readonly Dictionary<string, Image> images;
        private readonly Dictionary<string, CompositeTransform> imagesTransforms;

        /// <summary> Initializes a new instance of the <see cref="MainPage"/> class. </summary>
        public MainPage()
        {
            this.InitializeComponent();

            this.SaveButton.Click += new RoutedEventHandler(this.SaveClick_OnClickAsync);
            this.AddButton.Click += new RoutedEventHandler(this.AddClick_OnClickAsync);

            this.images = new Dictionary<string, Image>();
            this.images[this.ForestPhoto.Name] = this.ForestPhoto;
            this.images[this.OceanPhoto.Name] = this.OceanPhoto;
            this.images[this.DesertPhoto.Name] = this.DesertPhoto;

            this.imagesTransforms = new Dictionary<string, CompositeTransform>();

            foreach (Image image in this.images.Values)
            {
                this.SetDefaultImageTransformation(image);
            }
        }

        /// <summary>
        ///     Shows the 'Save File' dialog and returns the resulting file. Returns null if the
        ///     operation was cancelled.
        /// </summary>
        /// <returns>
        ///     A task that contains the result of selecting the file and that enables this method
        ///     to be awaited.
        /// </returns>
        private static async Task<StorageFile> ShowSaveFileDialogAsync()
        {
            // Ensure that the application is not snapped, to avoid errors.
            // See: https://docs.microsoft.com/en-us/windows/uwp/files/quickstart-save-a-file-with-a-picker
            bool unsnapped = (ApplicationView.Value != ApplicationViewState.Snapped) || ApplicationView.TryUnsnap();

            FileSavePicker savePicker = new FileSavePicker();

            savePicker.FileTypeChoices.Add("Image", new List<string>() { ".png" });
            savePicker.SuggestedFileName = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + "-Collage";
            savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

            StorageFile file = await savePicker.PickSaveFileAsync();

            return file;
        }

        private static async Task SaveViewAsImageFileAsync(UIElement uiElement, StorageFile file)
        {
            // Render the current view to the target bitmap. Reference from: https://stackoverflow.com/questions/41354024/uwp-save-grid-as-png
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(uiElement);

            // Obtain the pixels from the rendered bitmap
            byte[] pixels = (await renderTargetBitmap.GetPixelsAsync()).ToArray();

            // Write the result as a PNG image.
            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder bitmapEncoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                bitmapEncoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Premultiplied,
                    (uint)renderTargetBitmap.PixelWidth,
                    (uint)renderTargetBitmap.PixelHeight,
                    DisplayInformation.GetForCurrentView().RawDpiX,
                    DisplayInformation.GetForCurrentView().RawDpiY,
                    pixels);

                await bitmapEncoder.FlushAsync();
            }
        }

        private async void AddClick_OnClickAsync(object sender, RoutedEventArgs e)
        {
            IReadOnlyList<StorageFile> selectedImages = await ShowImageFileSelector();

            foreach (var file in selectedImages)
            {
                using (var fileStream = await file.OpenReadAsync())
                {
                    BitmapImage bitmap = new BitmapImage();
                    await bitmap.SetSourceAsync(fileStream);

                    Image image = new Image();
                    image.Name = Guid.NewGuid().ToString();
                    image.Source = bitmap;
                    image.Stretch = Stretch.Uniform;

                    image.CenterPoint = this.CollageCanvas.CenterPoint;

                    this.images[image.Name] = image;

                    this.SetDefaultImageTransformation(image);

                    this.CollageCanvas.Children.Add(image);
                }
            }

            this.CollageCanvas.UpdateLayout();
        }

        private static async Task<IReadOnlyList<StorageFile>> ShowImageFileSelector()
        {
            // Ensure that the application is not snapped, to avoid errors.
            // See: https://docs.microsoft.com/en-us/windows/uwp/files/quickstart-save-a-file-with-a-picker
            bool unsnapped = (ApplicationView.Value != ApplicationViewState.Snapped) || ApplicationView.TryUnsnap();

            FileOpenPicker fileOpenPicker = new FileOpenPicker();

            // Image extension filters.
            fileOpenPicker.FileTypeFilter.Add(".png");
            fileOpenPicker.FileTypeFilter.Add(".jpg");
            fileOpenPicker.FileTypeFilter.Add(".jpeg");

            fileOpenPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

            IReadOnlyList<StorageFile> selectedImages = await fileOpenPicker.PickMultipleFilesAsync();
            return selectedImages;
        }

        /// <summary> Stores the current collage as photo in the disk. </summary>
        /// <param name="sender"> The Save button. </param>
        /// <param name="e"> The arguments regarding the click. </param>
        private async void SaveClick_OnClickAsync(object sender, RoutedEventArgs e)
        {
            StorageFile file = await ShowSaveFileDialogAsync();

            if (file == null)
            {
                // The operation was cancelled.
                return;
            }

            await SaveViewAsImageFileAsync(this, file);
        }

        private void SetDefaultImageTransformation(Image image)
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

            image.DoubleTapped += new DoubleTappedEventHandler(this.Image_DoubleTapped);
        }

        private void Image_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            // Delete the image when double taped.
            Image image = sender as Image;
            this.images[image.Name] = null;
            this.imagesTransforms[image.Name] = null;
            this.CollageCanvas.Children.Remove(image);
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