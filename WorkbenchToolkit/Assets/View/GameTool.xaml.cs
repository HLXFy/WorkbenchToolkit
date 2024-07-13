using iNKORE.UI.WPF.Modern.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WorkbenchToolkit.Assets.View.GameTool;

namespace WorkbenchToolkit.Assets.View.GamePathView
{
    /// <summary>
    /// GameTool.xaml 的交互逻辑
    /// </summary>
    public partial class GameTool : System.Windows.Controls.Page
    {
        public GameTool()
        {
            InitializeComponent();
        }

        private void NavigationView_SelectionChanged(iNKORE.UI.WPF.Modern.Controls.NavigationView sender, iNKORE.UI.WPF.Modern.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (NavigationViewItem)args.SelectedItem;
            if ((string)selectedItem.Tag == "MinecraftJava") Frame.Navigate(typeof(GameRuntime));
            if ((string)selectedItem.Tag == "Accelerators") Frame.Navigate(typeof(MinecraftJava));
            if ((string)selectedItem.Tag == "GamePatch") Frame.Navigate(typeof(Sinicization));
        }
    }
}
