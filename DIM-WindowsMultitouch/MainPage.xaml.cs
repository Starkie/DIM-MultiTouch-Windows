namespace Dim.MultiTouch
{
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;

    /// <summary>
    ///     A page with an image. Registers the <see cref="Pointer"/> events in the image and
    ///     displays them in a list view.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary> Initializes a new instance of the <see cref="MainPage"/> class. </summary>
        public MainPage()
        {
            this.InitializeComponent();

            this.Photo.PointerPressed +=
                new PointerEventHandler(this.Photo_PointerPressed);
            this.Photo.PointerEntered +=
                new PointerEventHandler(this.Photo_PointerEntered);
            this.Photo.PointerReleased +=
                new PointerEventHandler(this.Photo_PointerReleased);
            this.Photo.PointerExited +=
                new PointerEventHandler(this.Photo_PointerExited);
            this.Photo.PointerCanceled +=
                new PointerEventHandler(this.Photo_PointerCanceled);
            this.Photo.PointerCaptureLost +=
                new PointerEventHandler(this.Photo_PointerCaptureLost);
            this.Photo.PointerMoved +=
                new PointerEventHandler(this.Photo_PointerMoved);
            this.Photo.PointerWheelChanged +=
                new PointerEventHandler(this.Photo_PointerWheelChanged);
        }

        private void Photo_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            this.WritePointerPosstion("Photo_PointerWheelChanged", e, this.Photo);
        }

        private void Photo_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            this.WritePointerPosstion("Photo_PointerMoved", e, this.Photo);
        }

        private void Photo_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            this.WritePointerPosstion("Photo_PointerCaptureLost", e, this.Photo);
        }

        private void Photo_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            this.WritePointerPosstion("Photo_PointerCanceled", e, this.Photo);
        }

        private void Photo_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            this.WritePointerPosstion("Photo_PointerExited", e, this.Photo);
        }

        private void Photo_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            this.WritePointerPosstion("Photo_PointerReleased", e, this.Photo);
        }

        private void Photo_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            this.WritePointerPosstion("Photo_PointerEntered", e, this.Photo);
        }

        private void Photo_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            this.WritePointerPosstion("Photo_PointerPressed", e, this.Photo);
        }

        private void WritePointerPosstion(string eventName, PointerRoutedEventArgs e, UIElement uiElement)
        {
            double x = e.GetCurrentPoint(uiElement).Position.X;
            double y = e.GetCurrentPoint(uiElement).Position.Y;

            this.EventsListView.Items.Insert(0, $"{eventName} POS: ({x}, {y})");
        }
    }
}