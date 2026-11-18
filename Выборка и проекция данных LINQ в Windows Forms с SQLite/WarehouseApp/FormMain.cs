using WarehouseApp.Data;
using WarehouseApp.Models;

namespace WarehouseApp;

public partial class FormMain : Form
{

    private bool _loading = true;

    public FormMain()
    {
        InitializeComponent();
    }

    private void FormMain_Load(object sender, EventArgs e)
    {
     
        cmbCategories.DataSource = ReportService.GetCategories();
        cmbCategories.DisplayMember = nameof(Category.Name);
        cmbCategories.ValueMember = nameof(Category.Id);

        _loading = false;
        ApplyFilter();
    }

  
    private void cmbCategories_SelectedIndexChanged(object sender, EventArgs e) => ApplyFilter();
    private void txtSearch_TextChanged(object sender, EventArgs e) => ApplyFilter();
    private void btnFilter_Click(object sender, EventArgs e) => ApplyFilter();


    private void ApplyFilter()
    {
        if (_loading) return;

        int categoryId = cmbCategories.SelectedValue is int id ? id : 0;

        List<ProductReportItem> report = ReportService.GetProductReport(categoryId, txtSearch.Text);

        dgvReport.DataSource = report;
        ConfigureColumns();

        decimal total = report.Sum(r => r.TotalValue);
        lblTotalSum.Text = $"Общая стоимость запаса: {total:N2} ₸";
    }

 
    private void ConfigureColumns()
    {
        if (dgvReport.Columns.Count == 0) return;

        dgvReport.Columns[nameof(ProductReportItem.ProductName)].HeaderText = "Товар";
        dgvReport.Columns[nameof(ProductReportItem.CategoryName)].HeaderText = "Категория";
        dgvReport.Columns[nameof(ProductReportItem.Price)].HeaderText = "Цена";
        dgvReport.Columns[nameof(ProductReportItem.Quantity)].HeaderText = "Остаток";
        dgvReport.Columns[nameof(ProductReportItem.TotalValue)].HeaderText = "Стоимость запаса";

        dgvReport.Columns[nameof(ProductReportItem.Price)].DefaultCellStyle.Format = "N2";
        dgvReport.Columns[nameof(ProductReportItem.TotalValue)].DefaultCellStyle.Format = "N2";

        foreach (var name in new[] { nameof(ProductReportItem.Price), nameof(ProductReportItem.Quantity), nameof(ProductReportItem.TotalValue) })
            dgvReport.Columns[name].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
    }
}
