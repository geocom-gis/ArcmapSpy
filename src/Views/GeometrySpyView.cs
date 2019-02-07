using System.Windows.Forms;
using ArcmapSpy.ViewModels;

namespace ArcmapSpy.Views
{
    public partial class GeometrySpyView : Form
    {
        public GeometrySpyView()
        {
            InitializeComponent();
        }

        public void SetDataContext(GeometrySpyViewModel viewModel)
        {
            ((ApplicationSpyViewWpf)wpfHost.Child).DataContext = viewModel;
        }

        private void GeometrySpyView_FormClosed(object sender, FormClosedEventArgs e)
        {
            ((ApplicationSpyViewWpf)wpfHost.Child).DataContext = null;
        }
    }
}
