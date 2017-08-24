using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LeaSearch.UI.UserControls;

namespace LeaSearch.UI.Controls
{
    public class InfoContent : ContentControl
    {
        static InfoContent()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(InfoContent), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(InfoContent)));
        }

        public static readonly DependencyProperty DisplayContentProperty = DependencyProperty.Register("DisplayContent", typeof(object), typeof(InfoContent), new PropertyMetadata(default(ContentPresenter), OnDisplayPropertyChanged));

        private static void OnDisplayPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as InfoContent;
            control?.OnDisplayContentChanged(new DisplayContentChangedArgs() { NewValue = e.NewValue, OldValue = e.OldValue });
        }



        /// <summary>
        /// 需要显示的内容
        /// </summary>
        public object DisplayContent
        {
            get { return (ContentPresenter)GetValue(DisplayContentProperty); }
            set { SetValue(DisplayContentProperty, value); }
        }

        /// <summary>
        /// 当显示内容设置发生改变的时候触发
        /// </summary>
        public event DisplayContentChanged DisplayContentChanged;

        protected virtual void OnDisplayContentChanged(DisplayContentChangedArgs args)
        {
            DisplayContentChanged?.Invoke(this, args);
        }
    }

    public delegate void DisplayContentChanged(object sender, DisplayContentChangedArgs args);

    public class DisplayContentChangedArgs
    {
        public object NewValue { get; set; }
        public object OldValue { get; set; }
    }
}
