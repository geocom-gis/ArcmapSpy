using System.Windows.Forms;
using ArcmapSpy.ViewModels;

namespace ArcmapSpy.Views
{
    public partial class FeatureSpyView : Form
    {
        public FeatureSpyView()
        {
            InitializeComponent();
        }

        public void SetDataContext(FeatureSpyViewModel viewModel)
        {
            ((FeatureSpyViewWpf)wpfHost.Child).DataContext = viewModel;
        }

        private void FeatureSpyView_FormClosed(object sender, FormClosedEventArgs e)
        {
            ((FeatureSpyViewWpf)wpfHost.Child).DataContext = null;
        }
    }
}
