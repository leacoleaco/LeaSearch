using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using GalaSoft.MvvmLight.Messaging;
using LeaSearch.Annotations;
using LeaSearch.Core.MessageModels;
using LeaSearch.Plugin;
using LeaSearch.Plugin.DetailInfos;

namespace LeaSearch.Views
{
    /// <summary>
    /// HelpInfoView.xaml 的交互逻辑
    /// </summary>
    public partial class HelpInfoView : UserControl, INotifyPropertyChanged
    {
        private IInfo _helpInfo;

        public HelpInfoView()
        {
            InitializeComponent();

            Messenger.Default.Register<SetHelpInfoMessage>(this, (m) =>
            {
                SetHelpInfo(m.HelpInfo);
            });
        }

        public IInfo HelpInfo
        {
            get { return _helpInfo; }
            set
            {
                _helpInfo = value;
                OnPropertyChanged();
            }
        }

        private void SetHelpInfo(HelpInfo info)
        {
            this.Visibility = info?.Info == null ? Visibility.Collapsed : Visibility.Visible;

            if (info?.Info != null) HelpInfo = info.Info;
        }





        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
