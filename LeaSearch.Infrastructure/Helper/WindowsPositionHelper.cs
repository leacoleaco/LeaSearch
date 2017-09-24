using System.Windows.Forms;
using System.Windows.Media;

namespace LeaSearch.Infrastructure.Helper
{
    public static class WindowsPositionHelper
    {
        public static double GetCenterScreenLeft(this Visual visual,double actualWidth)
        {
            var screen = Screen.FromPoint(System.Windows.Forms.Cursor.Position);
            var dip1 = WindowsInteropHelper.TransformPixelsToDIP(visual, screen.WorkingArea.X, 0);
            var dip2 = WindowsInteropHelper.TransformPixelsToDIP(visual, screen.WorkingArea.Width, 0);
            var left = (dip2.X - actualWidth) / 2 + dip1.X;
            return left;
        }

        public static double GetCenterScreenTop(this Visual visual,double actualHeight)
        {
            var screen = Screen.FromPoint(System.Windows.Forms.Cursor.Position);
            var dip1 = WindowsInteropHelper.TransformPixelsToDIP(visual, 0, screen.WorkingArea.Y);
            var dip2 = WindowsInteropHelper.TransformPixelsToDIP(visual, 0, screen.WorkingArea.Height);
            var top = (dip2.Y - actualHeight) / 4 + dip1.Y;
            return top;
        }

    }
}
