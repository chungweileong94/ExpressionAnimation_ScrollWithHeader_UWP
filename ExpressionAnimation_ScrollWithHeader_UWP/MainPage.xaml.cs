using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ExpressionAnimation_ScrollWithHeader_UWP
{
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<string> List { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            List = new ObservableCollection<string>();

            this.Loaded += delegate
            {
                var _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
                var _scrollViewerPropertySet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(FindVisualChild<ScrollViewer>(MyListView));
                var _scrollExpressionAnimation = _compositor.CreateExpressionAnimation("Clamp(sv.Translation.Y * m, (-200 + 48), 999)");
                _scrollExpressionAnimation.SetReferenceParameter("sv", _scrollViewerPropertySet);
                _scrollExpressionAnimation.SetScalarParameter("m", 0.3f);

                var visual = ElementCompositionPreview.GetElementVisual(HeaderGrid);
                visual.StartAnimation("Offset.Y", _scrollExpressionAnimation);

                var _textExpressionAnimation = _compositor.CreateExpressionAnimation("Lerp(Vector3(0, 100 - 24, 0), Vector3(0, 0, 0), Clamp(visual.Offset.Y / (-200 + 48), 0.0, 1.0))");
                _textExpressionAnimation.SetReferenceParameter("visual", visual);

                var textVisual = ElementCompositionPreview.GetElementVisual(HeaderTextGrid);
                textVisual.StartAnimation("Offset", _textExpressionAnimation);
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            for (int i = 0; i < 50; i++) List.Add($"Item {i + 1}");
        }

        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);

                    if (childOfChild != null)
                        return childOfChild;
                }
            }

            return null;
        }
    }
}
