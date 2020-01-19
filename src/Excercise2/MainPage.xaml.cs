namespace Dim.MultiTouch
{
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;

    /// <summary>
    ///     A page with an image. Registers the <see cref="Pointer"/> events and touch gestures in
    ///     the image and displays them in a list view.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary> Initializes a new instance of the <see cref="MainPage"/> class. </summary>
        public MainPage()
        {
            this.InitializeComponent();

            // Pointer events.
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

        private void Photo_Holding(object sender, HoldingRoutedEventArgs e)
        {
            this.EventsListView.Items.Clear();

            this.EventsListView.Items.Insert(0, $"Photo_Holding: Emptied the list view elements.");
        }

        private void Photo_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            this.Photo.Opacity = 1;

            this.EventsListView.Items.Insert(0, $"Photo_DoubleTapped: Current opacity {this.Photo.Opacity}");
        }

        private void Photo_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Photo.Opacity -= 0.1;

            this.EventsListView.Items.Insert(0, $"Photo_Tapped: Current opacity {this.Photo.Opacity}");
        }

        private void Photo_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            this.WritePointerPosition("Photo_PointerWheelChanged", e, this.Photo);
        }

        private void Photo_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            this.WritePointerPosition("Photo_PointerMoved", e, this.Photo);
        }

        private void Photo_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            this.WritePointerPosition("Photo_PointerCaptureLost", e, this.Photo);
        }

        private void Photo_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            this.WritePointerPosition("Photo_PointerCanceled", e, this.Photo);
        }

        private void Photo_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            this.WritePointerPosition("Photo_PointerExited", e, this.Photo);
        }

        private void Photo_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            this.WritePointerPosition("Photo_PointerReleased", e, this.Photo);
        }

        private void Photo_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            this.WritePointerPosition("Photo_PointerEntered", e, this.Photo);
        }

        private void Photo_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            this.WritePointerPosition("Photo_PointerPressed", e, this.Photo);
        }

        private void WritePointerPosition(string eventName, PointerRoutedEventArgs e, UIElement uiElement)
        {
            double x = e.GetCurrentPoint(uiElement).Position.X;
            double y = e.GetCurrentPoint(uiElement).Position.Y;

            this.EventsListView.Items.Insert(0, $"{eventName} POS: ({x}, {y})");
        }
    }
}