using CorpFleet.Analytics;
using CorpFleet.Data;
using CorpFleet.Events;
using CorpFleet.Models;

namespace CorpFleet;

public class FormMain : Form
{
    private readonly VehicleRepository _repo = new VehicleRepository();
    private readonly FleetManager _manager;
    private readonly TabControl _tabs = new TabControl { Dock = DockStyle.Fill };

    private List<FleetReportDto> _reportAll = new List<FleetReportDto>();
    private int _page = 1;
    private Action<object, VehicleEventArgs> _eventHandler;

    // вкладка 1
    private DataGridView _gridVehicles;
    private ComboBox _cbCategory;
    private TextBox _tbModel, _tbLog1;
    private NumericUpDown _numPower, _numPrice, _numLoad;

    // вкладка 2
    private DataGridView _gridReport;
    private NumericUpDown _numMin, _numMax;
    private TextBox _tbSearch;
    private Label _lblReportInfo;

    // вкладка 3
    private DataGridView _gridStats;
    private TextBox _tbSets;

    // вкладка 4
    private DataGridView _gridPage;
    private Label _lblPage;
    private NumericUpDown _numPageSize, _numCost;
    private CheckBox _chkSubscribe;
    private ComboBox _cbVehicles, _cbChannel;
    private TextBox _tbService;
    private ListBox _lbEvents;

    public FormMain()
    {
        Text = "CorpFleet Analytics — итоговая контрольная работа";
        Width = 1180;
        Height = 760;
        StartPosition = FormStartPosition.CenterScreen;
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;

        _manager = new FleetManager(_repo);

        _tabs.TabPages.Add(BuildRegistryTab());
        _tabs.TabPages.Add(BuildAnalyticsTab());
        _tabs.TabPages.Add(BuildGroupingTab());
        _tabs.TabPages.Add(BuildPagingTab());
        Controls.Add(_tabs);

        // Подписка на событие через лямбда-выражение (+=)
        _eventHandler = (sender, e) =>
            _lbEvents.Items.Insert(0, $"{DateTime.Now:HH:mm:ss}  {e.Message}  | сумма: {e.OperationPrice:N0}");
        _chkSubscribe.Checked = true;

        RefreshAll();
    }

    // ================= Вспомогательные методы построения UI =================

    private static DataGridView NewGrid() => new DataGridView
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

    private static Label Lbl(string text) =>
        new Label { Text = text, AutoSize = true, Margin = new Padding(8, 9, 2, 0) };

    private static Button Btn(string text, EventHandler onClick)
    {
        var b = new Button { Text = text, AutoSize = true, Margin = new Padding(6, 4, 2, 2), Padding = new Padding(4, 0, 4, 0) };
        b.Click += onClick;
        return b;
    }

    private static NumericUpDown Num(decimal min, decimal max, decimal value, int decimals = 0, decimal increment = 1, int width = 100) =>
        new NumericUpDown
        {
            Minimum = min,
            Maximum = max,
            Value = value,
            DecimalPlaces = decimals,
            Increment = increment,
            Width = width,
            Margin = new Padding(2, 5, 2, 0)
        };

    private static FlowLayoutPanel NewFlow() => new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = true };

    private static TextBox NewOutput() => new TextBox
    {
        Multiline = true,
        ReadOnly = true,
        ScrollBars = ScrollBars.Vertical,
        Dock = DockStyle.Fill,
        Font = new Font("Consolas", 10f)
    };

    private static TableLayoutPanel NewLayout(params RowStyle[] rows)
    {
        var t = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = rows.Length };
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        rows.ToList().ForEach(r => t.RowStyles.Add(r));
        return t;
    }

    private static void FormatColumns(DataGridView grid, string format, params string[] names) =>
        names.ToList().ForEach(n =>
        {
            if (grid.Columns[n] != null) grid.Columns[n].DefaultCellStyle.Format = format;
        });

    // ================= Вкладка 1: Реестр ТС и ООП-модель =================

    private TabPage BuildRegistryTab()
    {
        var page = new TabPage("1. Реестр ТС и ООП-модель");
        var layout = NewLayout(new RowStyle(SizeType.Absolute, 112), new RowStyle(SizeType.Percent, 100), new RowStyle(SizeType.Absolute, 160));
        var top = NewFlow();

        _cbCategory = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 140,
            Margin = new Padding(2, 5, 2, 0),
            DisplayMember = "CategoryName"
        };
        _cbCategory.DataSource = Database.GetCategories();

        _tbModel = new TextBox { Width = 160, Margin = new Padding(2, 5, 2, 0), Text = "Новая модель" };
        _numPower = Num(1, 3000, 150);
        _numPrice = Num(0, 1000000000, 10000000, 0, 100000, 130);
        _numLoad = Num(1, 500, 10, 1, 1);

        top.Controls.AddRange(new Control[]
        {
            Lbl("Категория:"), _cbCategory,
            Lbl("Модель:"), _tbModel,
            Lbl("Мощность, л.с.:"), _numPower,
            Lbl("Цена:"), _numPrice,
            Lbl("Груз, т / мест:"), _numLoad,
            Btn("Добавить", OnAddVehicle),
            Btn("Клонировать (Deep Copy)", OnCloneVehicle),
            Btn("Удалить", OnDeleteVehicle),
            Btn("Сортировка по цене (IComparable)", OnSortVehicles),
            Btn("Экспорт (IExportable)", OnExportVehicle),
            Btn("Сохранить (IStorable)", OnSaveVehicle),
            Btn("Clone() / Equals", OnEqualsDemo),
            Btn("Замыкания", (s, e) => Log1(_manager.DemoClosures()))
        });

        _gridVehicles = NewGrid();
        _tbLog1 = NewOutput();

        layout.Controls.Add(top, 0, 0);
        layout.Controls.Add(_gridVehicles, 0, 1);
        layout.Controls.Add(_tbLog1, 0, 2);
        page.Controls.Add(layout);
        return page;
    }

    private void Log1(string text) => _tbLog1.AppendText(text + "\r\n");

    private void BindVehicles(IEnumerable<Vehicle> vehicles)
    {
        _gridVehicles.DataSource = vehicles
            .Select(v => new
            {
                v.Id,
                Тип = v.GetVehicleType(),
                v.Model,
                Мощность = v.Power,
                Цена = v.BasePrice,
                Категория = v.CategoryName,
                Налог = Math.Round(v.CalculateTax(), 2)
            })
            .ToList();
        FormatColumns(_gridVehicles, "N0", "Цена", "Налог");
    }

    private Vehicle SelectedVehicle()
    {
        if (_gridVehicles.CurrentRow == null)
        {
            MessageBox.Show("Выберите ТС в таблице.", "Нет выбора", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return null;
        }
        int id = (int)_gridVehicles.CurrentRow.Cells["Id"].Value;
        return _repo.GetById(id);
    }

    private void OnAddVehicle(object sender, EventArgs e)
    {
        try
        {
            var cat = (CategoryRow)_cbCategory.SelectedItem;
            int id = _repo.NextId();
            Vehicle v;
            if (VehicleRepository.IsCargoCategory(cat.CategoryName))
                v = new CargoVehicle(id, _tbModel.Text, (int)_numPower.Value, (double)_numPrice.Value, (double)_numLoad.Value);
            else
                v = new PassengerVehicle(id, _tbModel.Text, (int)_numPower.Value, (double)_numPrice.Value, (int)_numLoad.Value);

            v.AssignCategory(cat.Id, cat.CategoryName, cat.TaxRate);
            _manager.AddVehicle(v);
            RefreshAll();
            Log1($"Добавлено: {v}");
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(ex.Message, "Ошибка валидации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void OnCloneVehicle(object sender, EventArgs e)
    {
        var v = SelectedVehicle();
        if (v == null) return;

        var copy = _manager.CloneVehicle(v);
        RefreshAll();
        Log1($"Клон создан: {copy}");
        Log1($"  Equals(оригинал) = {copy.Equals(v)} (сравнение по Model + Power)");
        Log1($"  ReferenceEquals = {ReferenceEquals(copy, v)}");
        Log1($"  Service — разные объекты: {!ReferenceEquals(copy.Service, v.Service)}; заметок у оригинала {v.Service.Notes.Count}, у клона {copy.Service.Notes.Count}");
    }

    private void OnDeleteVehicle(object sender, EventArgs e)
    {
        var v = SelectedVehicle();
        if (v == null) return;
        if (_manager.RemoveVehicle(v))
        {
            RefreshAll();
            Log1($"Удалено: {v}");
        }
    }

    private void OnSortVehicles(object sender, EventArgs e)
    {
        // OrderBy(v => v) использует IComparable<Vehicle> (сравнение по BasePrice)
        BindVehicles(_repo.GetAll().OrderBy(v => v).ToList());
        Log1("Сортировка по BasePrice через IComparable<Vehicle>.");
    }

    private void OnExportVehicle(object sender, EventArgs e)
    {
        var v = SelectedVehicle();
        if (v == null) return;

        // Паттерн-матчинг: явная реализация доступна только через интерфейсную ссылку
        if (v is IExportable exportable)
            Log1($"Экспорт ({v.GetType().Name}): {exportable.GetExportInfo()}");
        else
            Log1($"{v.GetType().Name} не реализует IExportable.");
    }

    private void OnSaveVehicle(object sender, EventArgs e)
    {
        var v = SelectedVehicle();
        if (v == null) return;
        try
        {
            v.Save();
            Log1($"IStorable.Save(): файл {v.FilePath}");
        }
        catch (IOException ex)
        {
            MessageBox.Show(ex.Message, "Ошибка записи файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OnEqualsDemo(object sender, EventArgs e)
    {
        var v = SelectedVehicle();
        if (v == null) return;
        var copy = (Vehicle)v.Clone();
        Log1($"Clone(): Equals = {v.Equals(copy)}, хэши равны = {v.GetHashCode() == copy.GetHashCode()}, ссылки равны = {ReferenceEquals(v, copy)}");
    }

    // ================= Вкладка 2: LINQ-аналитика и DTO =================

    private TabPage BuildAnalyticsTab()
    {
        var page = new TabPage("2. LINQ-аналитика и DTO");
        var layout = NewLayout(new RowStyle(SizeType.Absolute, 52), new RowStyle(SizeType.Percent, 100), new RowStyle(SizeType.Absolute, 32));
        var top = NewFlow();

        _numMin = Num(0, 1000000000, 0, 0, 1000000, 130);
        _numMax = Num(0, 1000000000, 0, 0, 1000000, 130);
        _tbSearch = new TextBox { Width = 160, Margin = new Padding(2, 5, 2, 0) };

        top.Controls.AddRange(new Control[]
        {
            Lbl("Цена от:"), _numMin,
            Lbl("до (0 = без ограничения):"), _numMax,
            Lbl("Поиск (модель / категория):"), _tbSearch,
            Btn("Применить", (s, e) => ApplyFilter()),
            Btn("Сброс", OnResetFilter)
        });

        _gridReport = NewGrid();
        _lblReportInfo = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };

        layout.Controls.Add(top, 0, 0);
        layout.Controls.Add(_gridReport, 0, 1);
        layout.Controls.Add(_lblReportInfo, 0, 2);
        page.Controls.Add(layout);
        return page;
    }

    private void OnResetFilter(object sender, EventArgs e)
    {
        _numMin.Value = 0;
        _numMax.Value = 0;
        _tbSearch.Text = "";
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        double? min = _numMin.Value > 0 ? (double?)_numMin.Value : null;
        double? max = _numMax.Value > 0 ? (double?)_numMax.Value : null;

        var rows = FleetAnalytics.Filter(_reportAll, min, max, _tbSearch.Text).ToList();
        _gridReport.DataSource = rows;
        FormatColumns(_gridReport, "N0", "BasePrice", "TaxAmount", "MaintenanceCost", "TotalCost");
        FormatColumns(_gridReport, "P0", "TaxRate");
        _lblReportInfo.Text = $"  Найдено ТС: {rows.Count}.  Суммарная стоимость владения: {rows.Sum(r => r.TotalCost):N0}";
    }

    // ================= Вкладка 3: Группировка, агрегаты и множества =================

    private TabPage BuildGroupingTab()
    {
        var page = new TabPage("3. Группировка, агрегаты и множества");
        var layout = NewLayout(new RowStyle(SizeType.Absolute, 52), new RowStyle(SizeType.Percent, 38), new RowStyle(SizeType.Percent, 62));
        var top = NewFlow();

        top.Controls.AddRange(new Control[]
        {
            Btn("Except (Online \\ Offline)", (s, e) => ShowSet("Online Except Offline", (a, b) => a.Except(b))),
            Btn("Intersect (Online ∩ Offline)", (s, e) => ShowSet("Online Intersect Offline", (a, b) => a.Intersect(b))),
            Btn("Union (Online ∪ Offline)", (s, e) => ShowSet("Online Union Offline", (a, b) => a.Union(b))),
            Btn("Статистика (GroupBy / Aggregate)", (s, e) => ShowStats())
        });

        _gridStats = NewGrid();
        _tbSets = NewOutput();

        layout.Controls.Add(top, 0, 0);
        layout.Controls.Add(_gridStats, 0, 1);
        layout.Controls.Add(_tbSets, 0, 2);
        page.Controls.Add(layout);
        return page;
    }

    private void ShowSet(string title, Func<IEnumerable<ServiceLogKey>, IEnumerable<ServiceLogKey>, IEnumerable<ServiceLogKey>> operation)
    {
        var vehicles = Database.GetVehicles();
        var logs = Database.GetLogs();
        var online = FleetAnalytics.ToKeys(logs, vehicles, "Online");
        var offline = FleetAnalytics.ToKeys(logs, vehicles, "Offline");

        var result = operation(online, offline).ToList();
        _tbSets.Text = $"{title}: {result.Count} записей\r\n" +
                       $"(Online: {online.Count}, Offline: {offline.Count})\r\n\r\n" +
                       string.Join("\r\n", result.Select(k => " • " + k));
    }

    private void ShowStats()
    {
        _gridStats.DataSource = FleetAnalytics.GroupStats(_reportAll);
        FormatColumns(_gridStats, "N0", "TotalPrice", "AvgPrice", "MinPrice", "MaxPrice", "MaintenanceTotal");
        _tbSets.Text = FleetAnalytics.BuildSummary(_reportAll);
    }

    // ================= Вкладка 4: Постраничный просмотр и события =================

    private TabPage BuildPagingTab()
    {
        var page = new TabPage("4. Постраничный просмотр и события");
        var layout = NewLayout(new RowStyle(SizeType.Absolute, 100), new RowStyle(SizeType.Percent, 60), new RowStyle(SizeType.Percent, 40));
        var top = NewFlow();

        _lblPage = new Label { AutoSize = true, Margin = new Padding(8, 9, 8, 0), Text = "Страница 1 из 1" };
        _numPageSize = Num(1, 50, 5, 0, 1, 60);
        _numPageSize.ValueChanged += (s, e) => { _page = 1; ShowPage(); };

        _chkSubscribe = new CheckBox { Text = "Подписка на StatusChanged", AutoSize = true, Margin = new Padding(14, 7, 2, 0) };
        _chkSubscribe.CheckedChanged += OnSubscribeChanged;

        _cbVehicles = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 300, Margin = new Padding(2, 5, 2, 0) };
        _tbService = new TextBox { Width = 140, Margin = new Padding(2, 5, 2, 0), Text = "Диагностика" };
        _numCost = Num(0, 100000000, 15000, 0, 1000, 100);
        _cbChannel = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 80, Margin = new Padding(2, 5, 2, 0) };
        _cbChannel.Items.AddRange(new object[] { "Online", "Offline" });
        _cbChannel.SelectedIndex = 0;

        top.Controls.AddRange(new Control[]
        {
            Btn("◀ Назад", (s, e) => { _page--; ShowPage(); }),
            _lblPage,
            Btn("Вперёд ▶", (s, e) => { _page++; ShowPage(); }),
            Lbl("На странице:"), _numPageSize,
            _chkSubscribe,
            Lbl("ТС:"), _cbVehicles,
            Lbl("Услуга:"), _tbService,
            Lbl("Стоимость:"), _numCost,
            _cbChannel,
            Btn("Записать обслуживание", OnRegisterMaintenance)
        });

        _gridPage = NewGrid();
        _lbEvents = new ListBox { Dock = DockStyle.Fill, HorizontalScrollbar = true };

        layout.Controls.Add(top, 0, 0);
        layout.Controls.Add(_gridPage, 0, 1);
        layout.Controls.Add(_lbEvents, 0, 2);
        page.Controls.Add(layout);
        return page;
    }

    private void OnSubscribeChanged(object sender, EventArgs e)
    {
        if (_chkSubscribe.Checked)
            _manager.StatusChanged += _eventHandler;   // подписка
        else
            _manager.StatusChanged -= _eventHandler;   // отписка
    }

    private void OnRegisterMaintenance(object sender, EventArgs e)
    {
        if (_cbVehicles.SelectedItem is not Vehicle v) return;

        string service = string.IsNullOrWhiteSpace(_tbService.Text) ? "Обслуживание" : _tbService.Text.Trim();
        _manager.RegisterMaintenance(v, service, (double)_numCost.Value, _cbChannel.SelectedItem.ToString());
        RefreshAll();
    }

    private void ShowPage()
    {
        var all = FleetAnalytics.Filter(_reportAll, null, null, null).ToList();
        int pageSize = (int)_numPageSize.Value;
        int pages = Math.Max(1, (int)Math.Ceiling(all.Count / (double)pageSize));
        _page = Math.Clamp(_page, 1, pages);

        _gridPage.DataSource = FleetAnalytics.Page(all, _page, pageSize);
        FormatColumns(_gridPage, "N0", "BasePrice", "TaxAmount", "MaintenanceCost", "TotalCost");
        FormatColumns(_gridPage, "P0", "TaxRate");
        _lblPage.Text = $"Страница {_page} из {pages}";
    }

    // ================= Общее обновление данных =================

    private void RefreshAll()
    {
        _repo.Reload();
        _reportAll = FleetAnalytics.BuildReport(Database.GetVehicles(), Database.GetCategories(), Database.GetLogs());

        BindVehicles(_repo.GetAll());
        ApplyFilter();
        ShowStats();
        ShowPage();
        _cbVehicles.DataSource = _repo.GetAll();
    }
}
