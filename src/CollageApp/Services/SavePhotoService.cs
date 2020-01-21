namespace Dim.MultiTouch.Collage.Service
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Threading.Tasks;
    using Windows.Graphics.Display;
    using Windows.Graphics.Imaging;
    using Windows.Storage;
    using Windows.Storage.Pickers;
    using Windows.Storage.Streams;
    using Windows.UI.ViewManagement;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media.Imaging;

    /// <summary> Service to store <see cref="UIElement"/> as photos in the disk. </summary>
    internal class SavePhotoService
    {
        /// <summary> Stores the given UI element as photo in the disk. </summary>
        /// <param name="uiElement"> The UI Element to save. </param>
        /// <returns> A task that enables this method to be awaited. </returns>
        public async Task SaveUiElementToFile(UIElement uiElement)
        {
            StorageFile file = await ShowSaveFileDialogAsync();

            if (file == null)
            {
                // The operation was cancelled.
                return;
            }

            await SaveUiElementAsImageFileAsync(uiElement, file);
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

        /// <summary>
        ///     Saves the contents of the given <see cref="UIElement"/> inside the given file.
        /// </summary>
        /// <param name="uiElement"> The element to store. </param>
        /// <param name="file"> The file where to store the image. </param>
        /// <returns> A task that enables this method to be awaited. </returns>
        private static async Task SaveUiElementAsImageFileAsync(UIElement uiElement, StorageFile file)
        {
            // Render the current view to the target bitmap. Reference from: https://stackoverflow.com/questions/41354024/uwp-save-grid-as-png
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(uiElement);

            // Obtain the pixels from the rendered bitmap
            byte[] pixels = (await renderTargetBitmap.GetPixelsAsync()).ToArray();

            // Write the result as a PNG image.
            using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder bitmapEncoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);
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
    }
}