using System.Windows.Forms;

namespace ArcmapSpy.Views
{
    public partial class LayerSpyView : Form
    {
        public LayerSpyView()
        {
            InitializeComponent();
        }

        private void LayerSpyView_FormClosed(object sender, FormClosedEventArgs e)
        {
            ((LayerSpyViewWpf)wpfHost.Child).DataContext = null;
        }
    }
}
