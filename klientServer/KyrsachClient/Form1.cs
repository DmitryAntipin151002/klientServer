namespace KyrsachClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private Form activeForm = null;
        private void OpenChildForm(Form ChildForm)
        {
            if (activeForm != null)
                activeForm.Close();
            activeForm = ChildForm;
            ChildForm.TopLevel = false;
            ChildForm.FormBorderStyle = FormBorderStyle.None;
            ChildForm.Dock = DockStyle.Fill;
            panelchildform.Controls.Add(ChildForm);
            panelchildform.Tag = ChildForm;
            ChildForm.BringToFront();
        }

        private void panelchildform_Paint(object sender, PaintEventArgs e)
        {

        }

        private void kgtkgtkkgtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Registration());
        }
    }
}