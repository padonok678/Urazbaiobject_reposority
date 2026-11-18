namespace WarehouseApp;

partial class FormMain
{
    private System.ComponentModel.IContainer components = null;

    private Label lblCategory;
    private Label lblSearch;
    private ComboBox cmbCategories;
    private TextBox txtSearch;
    private Button btnFilter;
    private DataGridView dgvReport;
    private Label lblTotalSum;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        lblCategory = new Label();
        lblSearch = new Label();
        cmbCategories = new ComboBox();
        txtSearch = new TextBox();
        btnFilter = new Button();
        dgvReport = new DataGridView();
        lblTotalSum = new Label();
        ((System.ComponentModel.ISupportInitialize)dgvReport).BeginInit();
        SuspendLayout();

        // lblCategory
        lblCategory.AutoSize = true;
        lblCategory.Location = new Point(12, 15);
        lblCategory.Text = "Категория:";

        // cmbCategories
        cmbCategories.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbCategories.Location = new Point(85, 12);
        cmbCategories.Size = new Size(200, 23);
        cmbCategories.Name = "cmbCategories";
        cmbCategories.SelectedIndexChanged += cmbCategories_SelectedIndexChanged;

        // lblSearch
        lblSearch.AutoSize = true;
        lblSearch.Location = new Point(305, 15);
        lblSearch.Text = "Поиск:";

        // txtSearch
        txtSearch.Location = new Point(355, 12);
        txtSearch.Size = new Size(200, 23);
        txtSearch.Name = "txtSearch";
        txtSearch.TextChanged += txtSearch_TextChanged;

        // btnFilter
        btnFilter.Location = new Point(570, 11);
        btnFilter.Size = new Size(110, 25);
        btnFilter.Name = "btnFilter";
        btnFilter.Text = "Применить";
        btnFilter.Click += btnFilter_Click;

        // dgvReport
        dgvReport.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvReport.Location = new Point(12, 48);
        dgvReport.Size = new Size(760, 350);
        dgvReport.Name = "dgvReport";
        dgvReport.ReadOnly = true;
        dgvReport.AllowUserToAddRows = false;
        dgvReport.AllowUserToDeleteRows = false;
        dgvReport.RowHeadersVisible = false;
        dgvReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        // lblTotalSum
        lblTotalSum.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        lblTotalSum.AutoSize = true;
        lblTotalSum.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblTotalSum.Location = new Point(12, 412);
        lblTotalSum.Name = "lblTotalSum";
        lblTotalSum.Text = "Общая стоимость запаса: 0";

        // FormMain
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(784, 451);
        MinimumSize = new Size(700, 350);
        Controls.Add(lblCategory);
        Controls.Add(cmbCategories);
        Controls.Add(lblSearch);
        Controls.Add(txtSearch);
        Controls.Add(btnFilter);
        Controls.Add(dgvReport);
        Controls.Add(lblTotalSum);
        Name = "FormMain";
        Text = "Складской учет товаров";
        StartPosition = FormStartPosition.CenterScreen;
        Load += FormMain_Load;
        ((System.ComponentModel.ISupportInitialize)dgvReport).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
}
