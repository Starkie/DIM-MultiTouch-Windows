namespace Dim.MultiTouch.Collage.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Windows.Storage;
    using Windows.Storage.Pickers;
    using Windows.Storage.Streams;
    using Windows.UI.ViewManagement;
    using Windows.UI.Xaml.Media.Imaging;

    /// <summary> Service for loading photos from the disk. </summary>
    public class LoadPhotoService
    {
        /// <summary> Lets the user pick the photos to load and returns the load <see cref="BitmapImage"/>. </summary>
        /// <returns>
        ///     A task containing the collection of loaded images, and that enables this method to
        ///     be awaited.
        /// </returns>
        public async Task<IEnumerable<BitmapImage>> SelectAndLoadPhotosAsync()
        {
            IReadOnlyList<StorageFile> selectedImages = await ShowImageFileSelectorAsync();

            return await LoadImagesFromFilesAsync(selectedImages);
        }

        /// <summary> Shows the file selector to pick one or more images. </summary>
        /// <returns> The collection of selected files, if any. </returns>
        private static async Task<IReadOnlyList<StorageFile>> ShowImageFileSelectorAsync()
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

        /// <summary> Loads the images contained in the selected files. </summary>
        /// <param name="selectedImages"> The files of the images to load. </param>
        /// <returns> The bitmaps of the images. </returns>
        private static async Task<IEnumerable<BitmapImage>> LoadImagesFromFilesAsync(IReadOnlyList<StorageFile> selectedImages)
        {
            List<BitmapImage> images = new List<BitmapImage>();

            foreach (StorageFile file in selectedImages)
            {
                using (IRandomAccessStreamWithContentType fileStream = await file.OpenReadAsync())
                {
                    BitmapImage bitmap = new BitmapImage();
                    await bitmap.SetSourceAsync(fileStream);

                    images.Add(bitmap);
                }
            }

            return images;
        }
    }
}