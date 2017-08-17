using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using LeaSearch.Common.Env;
using LeaSearch.Common.View;
using LeaSearch.Common.ViewModel;
using LeaSearch.Common.Windows.Input;
using LeaSearch.Core.HotKey;

namespace LeaSearch.ViewModels
{
    [Export(typeof(IShell))]
    public class ShellViewModel : BaseViewModel, IShell
    {
        public ShellViewModel(Settings settings) : base(settings)
        {
            OpenResultCommand = new ParameterCommand(_ =>
            {
                MessageBox.Show("normal!");

            });


            OpenResultCommand1 = new ParameterCommand(_ =>
            {
                MessageBox.Show("focus click!");
            });


            StartHelpCommand = new ParameterCommand(_ =>
            {
                Process.Start("http://doc.getwox.com");
            });
        }

        //use action is just to convenient and decoupe 
        #region Hotkey Action Event


        public ICommand EscCommand { get; set; }
        public ICommand SelectNextItemCommand { get; set; }
        public ICommand SelectPrevItemCommand { get; set; }
        public ICommand SelectNextPageCommand { get; set; }
        public ICommand SelectPrevPageCommand { get; set; }
        public ICommand StartHelpCommand { get; set; }
        public ICommand LoadContextMenuCommand { get; set; }
        public ICommand LoadHistoryCommand { get; set; }
        public ICommand OpenResultCommand { get; set; }

        public ICommand OpenResultCommand1 { get; set; }

        #endregion

        #region Private Fields

        #endregion



        #region Command




        #endregion
    }

    public interface IShell
    {
    }
}