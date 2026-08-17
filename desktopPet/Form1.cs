using desktopPet.UC;

namespace desktopPet
{
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();
            instance = this;
            ChangeUC(new BongoDown());
        }
        public static mainForm instance;
        private void mainForm_Load(object sender, EventArgs e)
        {

        }
        public void ChangeUC(UserControl uc)
        {
            mainpanel.Controls.Clear();
            mainpanel.Controls.Add(uc);
            ClientSize = uc.Size;
        }
    }
}
