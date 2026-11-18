using EnterpriseLinq.Data;
using EnterpriseLinq.Models;
using EnterpriseLinq.Services;

namespace EnterpriseLinq;

public class FormMain : Form
{
    private readonly ReportService _service;
    private int _page = 1;

    // вкладка 1
    private DataGridView _gridSales = null!;
    private ComboBox _cbCategory = null!, _cbChannel = null!;
    private Label _lblSalesInfo = null!;

    // вкладка 2
    private DataGridView _gridStats = null!;
    private TextBox _tbAudit = null!;

    // вкладка 3
    private DataGridView _gridSets = null!;
    private Label _lblSets = null!;

    // вкладка 4
    private DataGridView _gridPage = null!;
    private NumericUpDown _numPageSize = null!;
    private Label _lblPage = null!;

    public FormMain()
    {
        Text = "Enterprise: анализ продаж (LINQ + SQLite)";
        Width = 1150;
        Height = 740;
        StartPosition = FormStartPosition.CenterScreen;
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;

        _service = new ReportService(DataLoader.LoadCategories(), DataLoader.LoadProducts(), DataLoader.LoadSales());

        var tabs = new TabControl { Dock = DockStyle.Fill };
        tabs.TabPages.Add(BuildSalesTab());
        tabs.TabPages.Add(BuildGroupingTab());
        tabs.TabPages.Add(BuildChannelsTab());
        tabs.TabPages.Add(BuildPagingTab());
        Controls.Add(tabs);

        ApplyFilter();
        ShowStats();
        ShowPage();
    }

    // ================= Помощники UI =================

    private static DataGridView NewGrid() => new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        MultiSelect = false,
        RowHeadersVisible = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        BackgroundColor = SystemColors.Window
    };

    private static Label Lbl(string text) => new() { Text = text, AutoSize = true, Margin = new Padding(8, 9, 2, 0) };

    private static Button Btn(string text, EventHandler onClick)
    {
        var button = new Button { Text = text, AutoSize = true, Margin = new Padding(6, 4, 2, 2), Padding = new Padding(4, 0, 4, 0) };
        button.Click += onClick;
        return button;
    }

    private static ComboBox NewCombo(int width) => new()
    {
        DropDownStyle = ComboBoxStyle.DropDownList,
        Width = width,
        Margin = new Padding(2, 5, 2, 0)
    };

    private static FlowLayoutPanel NewFlow() => new() { Dock = DockStyle.Fill, WrapContents = true };

    private static TextBox NewOutput() => new()
    {
        Multiline = true,
        ReadOnly = true,
        ScrollBars = ScrollBars.Vertical,
        Dock = DockStyle.Fill,
        Font = new Font("Consolas", 10f)
    };

    private static TableLayoutPanel NewLayout(params RowStyle[] rows)
    {
        var table = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = rows.Length };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        rows.ToList().ForEach(r => table.RowStyles.Add(r));
        return table;
    }

    private static void FormatColumns(DataGridView grid, string format, params string[] names) =>
        names.ToList().ForEach(n =>
        {
            if (grid.Columns[n] != null) grid.Columns[n].DefaultCellStyle.Format = format;
        });

    // ================= Вкладка 1: Реестр продаж и DTO =================

    private TabPage BuildSalesTab()
    {
        var page = new TabPage("Реестр продаж и DTO");
        var layout = NewLayout(new RowStyle(SizeType.Absolute, 52), new RowStyle(SizeType.Percent, 100), new RowStyle(SizeType.Absolute, 30));
        var top = NewFlow();

        _cbCategory = NewCombo(160);
        _cbCategory.Items.Add("Все категории");
        _cbCategory.Items.AddRange(_service.Categories.Select(c => (object)c.Name).ToArray());
        _cbCategory.SelectedIndex = 0;

        _cbChannel = NewCombo(110);
        _cbChannel.Items.AddRange(new object[] { "Все каналы", "Online", "Retail" });
        _cbChannel.SelectedIndex = 0;

        top.Controls.AddRange(new Control[]
        {
            Lbl("Категория:"), _cbCategory,
            Lbl("Канал:"), _cbChannel,
            Btn("Применить", (_, _) => ApplyFilter()),
            Btn("Сбросить", (_, _) =>
            {
                _cbCategory.SelectedIndex = 0;
                _cbChannel.SelectedIndex = 0;
                ApplyFilter();
            })
        });

        _gridSales = NewGrid();
        _lblSalesInfo = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };

        layout.Controls.Add(top, 0, 0);
        layout.Controls.Add(_gridSales, 0, 1);
        layout.Controls.Add(_lblSalesInfo, 0, 2);
        page.Controls.Add(layout);
        return page;
    }

    private void ApplyFilter()
    {
        string? category = _cbCategory.SelectedIndex > 0 ? _cbCategory.SelectedItem?.ToString() : null;
        string? channel = _cbChannel.SelectedIndex > 0 ? _cbChannel.SelectedItem?.ToString() : null;

        var rows = _service.Filter(category, channel).ToList();
        _gridSales.DataSource = rows;
        FormatColumns(_gridSales, "N2", "DiscountedPrice", "TotalAmount");
        FormatColumns(_gridSales, "dd.MM.yyyy", "SaleDate");
        _lblSalesInfo.Text = $"  Сделок: {rows.Count}, на сумму {rows.Sum(r => r.TotalAmount):N2}";
    }

    // ================= Вкладка 2: Группировка и агрегация =================

    private TabPage BuildGroupingTab()
    {
        var page = new TabPage("Группировка и Агрегация");
        var layout = NewLayout(new RowStyle(SizeType.Absolute, 52), new RowStyle(SizeType.Percent, 45), new RowStyle(SizeType.Percent, 55));
        var top = NewFlow();

        top.Controls.AddRange(new Control[]
        {
            Btn("Пересчитать (GroupBy + Aggregate)", (_, _) => ShowStats()),
            Btn("Категории и товары (GroupJoin)", (_, _) => ShowGroupJoin()),
            Btn("Deferred и ToList()", (_, _) => _tbAudit.Text = _service.DemoExecution())
        });

        _gridStats = NewGrid();
        _tbAudit = NewOutput();

        layout.Controls.Add(top, 0, 0);
        layout.Controls.Add(_gridStats, 0, 1);
        layout.Controls.Add(_tbAudit, 0, 2);
        page.Controls.Add(layout);
        return page;
    }

    private void ShowStats()
    {
        _gridStats.DataSource = _service.GetCategoryStats();
        FormatColumns(_gridStats, "N2", "Revenue", "AverageCheck", "MinDeal", "MaxDeal");
        _tbAudit.Text = _service.BuildAudit();
    }

    private void ShowGroupJoin()
    {
        var lines = _service.GetCategoriesWithProducts()
            .Select(x => $"{x.Category.Name} (скидка {x.Category.DiscountRate:P0}): " +
                         (x.Products.Count == 0 ? "товаров нет" : string.Join(", ", x.Products.Select(p => p.Name))));
        _tbAudit.Text = "GroupJoin: категории и входящие товары\r\n\r\n" + string.Join("\r\n", lines);
    }

    // ================= Вкладка 3: Анализ каналов =================

    private TabPage BuildChannelsTab()
    {
        var page = new TabPage("Анализ каналов (Множества)");
        var layout = NewLayout(new RowStyle(SizeType.Absolute, 52), new RowStyle(SizeType.Percent, 100), new RowStyle(SizeType.Absolute, 30));
        var top = NewFlow();

        top.Controls.AddRange(new Control[]
        {
            Btn("Online Only (Except)", (_, _) => ShowProducts("Только Online, в Retail не продавались (Except)", _service.OnlineOnly())),
            Btn("Общие товары (Intersect)", (_, _) => ShowProducts("Проданы и в Online, и в Retail (Intersect)", _service.CommonProducts())),
            Btn("Все уникальные (Union)", (_, _) => ShowProducts("Все проданные позиции (Union)", _service.AllSoldProducts()))
        });

        _gridSets = NewGrid();
        _lblSets = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };

        layout.Controls.Add(top, 0, 0);
        layout.Controls.Add(_gridSets, 0, 1);
        layout.Controls.Add(_lblSets, 0, 2);
        page.Controls.Add(layout);
        return page;
    }

    private void ShowProducts(string title, List<Product> products)
    {
        _gridSets.DataSource = products
            .Select(p => new
            {
                p.Id,
                Название = p.Name,
                Категория = _service.FindCategory(p.CategoryId)?.Name ?? "—",
                Цена = p.Price,
                Остаток = p.Stock
            })
            .ToList();
        FormatColumns(_gridSets, "N0", "Цена");
        _lblSets.Text = $"  {title}: {products.Count}";
    }

    // ================= Вкладка 4: Постраничный просмотр =================

    private TabPage BuildPagingTab()
    {
        var page = new TabPage("Постраничный просмотр (Paging)");
        var layout = NewLayout(new RowStyle(SizeType.Absolute, 52), new RowStyle(SizeType.Percent, 100));
        var top = NewFlow();

        _numPageSize = new NumericUpDown
        {
            Minimum = 1,
            Maximum = 100,
            Value = 8,
            Width = 60,
            Margin = new Padding(2, 5, 2, 0)
        };
        _lblPage = new Label { AutoSize = true, Margin = new Padding(8, 9, 8, 0), Text = "Страница 1 из 1" };

        top.Controls.AddRange(new Control[]
        {
            Btn("Назад", (_, _) => { _page--; ShowPage(); }),
            _lblPage,
            Btn("Вперед", (_, _) => { _page++; ShowPage(); }),
            Lbl("Размер страницы:"), _numPageSize
        });
        _numPageSize.ValueChanged += (_, _) => { _page = 1; ShowPage(); };

        _gridPage = NewGrid();

        layout.Controls.Add(top, 0, 0);
        layout.Controls.Add(_gridPage, 0, 1);
        page.Controls.Add(layout);
        return page;
    }

    private void ShowPage()
    {
        int pageSize = (int)_numPageSize.Value;
        int pages = _service.GetPageCount(pageSize);
        _page = Math.Clamp(_page, 1, pages);

        _gridPage.DataSource = _service.GetPage(_page, pageSize);
        FormatColumns(_gridPage, "N2", "DiscountedPrice", "TotalAmount");
        FormatColumns(_gridPage, "dd.MM.yyyy", "SaleDate");
        _lblPage.Text = $"Страница {_page} из {pages}";
    }
}
