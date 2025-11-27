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

namespace Temple.UI.WPF.DD.Exploration
{
    /// <summary>
    /// Interaction logic for ExploreAreaView.xaml
    /// </summary>
    public partial class ExploreAreaView : UserControl
    {
        public ExploreAreaView()
        {
            InitializeComponent();
        }

        private void ExploreAreaView_OnKeyDown(object sender, KeyEventArgs e)
        {
            MessageBox.Show($"A received {e.Key}");
        }
    }
}
