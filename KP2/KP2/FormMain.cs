using System.Diagnostics;
using KP2.Analytics;
using KP2.Data;
using KP2.Models;
using KP2.Services;

namespace KP2;

public class FormMain : Form
{
    private readonly SqliteRepository<Device> _deviceRepo = Repositories.Devices();
    private readonly SqliteRepository<Room> _roomRepo = Repositories.Rooms();
    private readonly SqliteRepository<TelemetryLog> _logRepo = Repositories.Logs();
    private readonly TelemetryCache _cache;

    private List<Device> _devices = new();
    private List<Room> _rooms = new();
    private List<TelemetryLog> _logs = new();
    private int _page = 1;

    private readonly ToolStripStatusLabel _status = new("Загрузка...");
    private readonly Action<string> _setStatus;

    // вкладка 1
    private DataGridView _gridDevices = null!;
    private ComboBox _cbRoom = null!, _cbType = null!;
    private TextBox _tbModel = null!, _tbLog1 = null!;
    private NumericUpDown _numPower = null!;

    // вкладка 2
    private ListBox _lbEvents = null!;
    private Button _btnScan = null!;
    private NumericUpDown _numThreshold = null!;

    // вкладка 3
    private DataGridView _gridDto = null!, _gridRoomStats = null!;
    private ComboBox _cbTypeFilter = null!;
    private TextBox _tbSearch = null!, _tbSummary = null!;
    private NumericUpDown _numMinPower = null!;

    // вкладка 4
    private ComboBox _cbRoomA = null!, _cbRoomB = null!;
    private DataGridView _gridSets = null!, _gridPage = null!;
    private Label _lblSets = null!, _lblPage = null!;
    private NumericUpDown _numPageSize = null!;

    private record PollResult(string Model, string Reading, long Milliseconds);

    public FormMain()
    {
        Text = "SmartHome Master Console";
        Width = 1200;
        Height = 780;
        StartPosition = FormStartPosition.CenterScreen;
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;

        _setStatus = message => _status.Text = message;
        _cache = new TelemetryCache(_logRepo);

        var tabs = new TabControl { Dock = DockStyle.Fill };
        tabs.TabPages.Add(BuildDevicesTab());
        tabs.TabPages.Add(BuildScanTab());
        tabs.TabPages.Add(BuildAnalyticsTab());
        tabs.TabPages.Add(BuildSetsTab());

        var strip = new StatusStrip { Dock = DockStyle.Fill, SizingGrip = false };
        strip.Items.Add(_status);

        var root = NewLayout(new RowStyle(SizeType.Percent, 100), new RowStyle(SizeType.Absolute, 26));
        root.Controls.Add(tabs, 0, 0);
        root.Controls.Add(strip, 0, 1);
        Controls.Add(root);

        Shown += async (_, _) => await ReloadAsync();
    }

    // ================= Загрузка данных =================

    private async Task ReloadAsync()
    {
        _setStatus("Чтение базы SQLite...");
        var activeIds = _devices.Where(d => d.IsActive).Select(d => d.Id).ToHashSet();

        var devicesTask = _deviceRepo.GetAllAsync();
        var roomsTask = _roomRepo.GetAllAsync();
        var logsTask = _logRepo.GetAllAsync();
        await Task.WhenAll(devicesTask, roomsTask, logsTask);

        _devices = await devicesTask;
        _rooms = await roomsTask;
        _logs = await logsTask;

        _devices.Where(d => activeIds.Contains(d.Id)).ToList().ForEach(d => d.RestoreState(true));
        _devices.ForEach(d => d.StatusChanged += OnDeviceStatusChanged);
        _cache.Clear();

        RefreshViews();
        _setStatus($"Загружено: устройств {_devices.Count}, комнат {_rooms.Count}, замеров {_logs.Count}");
    }

    private void RefreshViews()
    {
        _cbRoom.DataSource = _rooms.ToList();
        _cbRoomA.DataSource = _rooms.ToList();
        _cbRoomB.DataSource = _rooms.ToList();
        if (_rooms.Count > 1) _cbRoomB.SelectedIndex = 1;

        BindDevices(_devices);
        ApplyFilters();
        ShowPage();
    }

    private void OnDeviceStatusChanged(object? sender, DeviceEventArgs e)
    {
        _lbEvents.Items.Insert(0, $"{e.Timestamp:HH:mm:ss}  {e.Message}");
        _setStatus(e.Message);
    }

    private string RoomTitle(int roomId) => _rooms.FirstOrDefault(r => r.Id == roomId)?.Title ?? "—";

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

    private static NumericUpDown Num(decimal min, decimal max, decimal value, int decimals = 0, decimal step = 1, int width = 100) => new()
    {
        Minimum = min,
        Maximum = max,
        Value = value,
        DecimalPlaces = decimals,
        Increment = step,
        Width = width,
        Margin = new Padding(2, 5, 2, 0)
    };

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

    // ================= Вкладка 1: Реестр приборов =================

    private TabPage BuildDevicesTab()
    {
        var page = new TabPage("Реестр приборов (ООП & Generics)");
        var layout = NewLayout(new RowStyle(SizeType.Absolute, 100), new RowStyle(SizeType.Percent, 100), new RowStyle(SizeType.Absolute, 170));
        var top = NewFlow();

        _cbRoom = NewCombo(130);
        _cbRoom.DisplayMember = "Title";
        _cbType = NewCombo(110);
        _cbType.Items.AddRange(new object[] { "Датчик", "Исполнитель" });
        _cbType.SelectedIndex = 0;
        _tbModel = new TextBox { Width = 200, Margin = new Padding(2, 5, 2, 0), Text = "Новое устройство" };
        _numPower = Num(0.1m, 20000, 5, 1, 1, 90);

        top.Controls.AddRange(new Control[]
        {
            Lbl("Комната:"), _cbRoom,
            Lbl("Тип:"), _cbType,
            Lbl("Модель:"), _tbModel,
            Lbl("Мощность, Вт:"), _numPower,
            Btn("Добавить", OnAddDevice),
            Btn("Сортировка по мощности (IComparable)", OnSortDevices),
            Btn("Deep Copy (ICloneable)", OnDeepCopy),
            Btn("Вкл / выкл", OnToggleDevice),
            Btn("Экспорт (IExportable)", OnExportDevice),
            Btn("override и new", OnShadowingDemo)
        });

        _gridDevices = NewGrid();
        _tbLog1 = NewOutput();

        layout.Controls.Add(top, 0, 0);
        layout.Controls.Add(_gridDevices, 0, 1);
        layout.Controls.Add(_tbLog1, 0, 2);
        page.Controls.Add(layout);
        return page;
    }

    private void Log1(string text) => _tbLog1.AppendText(text + "\r\n");

    private int? SelectedDeviceId() =>
        _gridDevices.CurrentRow?.Cells["Id"].Value is int id ? id : null;

    private Device? SelectedDevice()
    {
        int? id = SelectedDeviceId();
        if (id == null)
        {
            MessageBox.Show("Выберите устройство в таблице.", "Нет выбора", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return null;
        }
        return _devices.FirstOrDefault(d => d.Id == id);
    }

    private void BindDevices(IEnumerable<Device> devices)
    {
        int? keep = SelectedDeviceId();

        _gridDevices.DataSource = devices
            .Select(d => new
            {
                d.Id,
                Тип = d.DeviceType,
                Модель = d.Model,
                Комната = RoomTitle(d.RoomId),
                Мощность = d.Power,
                Расход = Math.Round(d.CalculatePowerConsumption(), 3),
                Статус = d.GetStatus()
            })
            .ToList();

        if (keep != null)
        {
            var row = _gridDevices.Rows.Cast<DataGridViewRow>().FirstOrDefault(r => (int)r.Cells["Id"].Value! == keep);
            if (row != null) _gridDevices.CurrentCell = row.Cells[0];
        }
    }

    private async void OnAddDevice(object? sender, EventArgs e)
    {
        try
        {
            var room = (Room)_cbRoom.SelectedItem!;
            double power = (double)_numPower.Value;

            Device device = _cbType.SelectedIndex == 0
                ? new SmartSensor(0, room.Id, _tbModel.Text, power, 0.5)
                : new SmartActuator(0, room.Id, _tbModel.Text, power, Math.Round(power * 1.25));

            await Task.Run(() => _deviceRepo.Add(device));
            await ReloadAsync();
            Log1($"Добавлено: {device}");
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(ex.Message, "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void OnSortDevices(object? sender, EventArgs e)
    {
        _devices.Sort();
        BindDevices(_devices);
        Log1("List.Sort(): устройства упорядочены по мощности (IComparable<Device>).");
    }

    private void OnDeepCopy(object? sender, EventArgs e)
    {
        var device = SelectedDevice();
        if (device == null) return;

        var copy = (Device)device.Clone();
        copy.Passport.Recalibrate(DateTime.Today.AddDays(30), 0.7);

        Log1($"Копия: {copy}");
        Log1($"  паспорт оригинала: {device.Passport}");
        Log1($"  паспорт копии:     {copy.Passport}");
        Log1($"  паспорта — разные объекты: {!ReferenceEquals(device.Passport, copy.Passport)}");
        Log1($"  Equals(оригинал, копия) = {device.Equals(copy)} (по Id и Model)");
    }

    private void OnToggleDevice(object? sender, EventArgs e)
    {
        var device = SelectedDevice();
        if (device == null) return;

        if (device.IsActive) device.Deactivate();
        else device.Activate();
        BindDevices(_devices);
    }

    private void OnExportDevice(object? sender, EventArgs e)
    {
        var device = SelectedDevice();
        if (device == null) return;

        Log1($"Экспорт: {((IExportable)device).GetExportData()}");
        if (device is IExportable exportable)
            Log1($"  через pattern matching (is IExportable): {exportable.GetExportData()}");
    }

    private void OnShadowingDemo(object? sender, EventArgs e)
    {
        var actuator = _devices.OfType<SmartActuator>().FirstOrDefault();
        if (actuator == null) return;

        Device asBase = actuator;
        Log1($"override: GetStatus() через Device -> {asBase.GetStatus()}");
        Log1($"new: Describe() через Device -> {asBase.Describe()}");
        Log1($"new: Describe() через SmartActuator -> {actuator.Describe()}");
    }

    // ================= Вкладка 2: Асинхронный опрос и события =================

    private TabPage BuildScanTab()
    {
        var page = new TabPage("Асинхронный опрос & Events");
        var layout = NewLayout(new RowStyle(SizeType.Absolute, 56), new RowStyle(SizeType.Percent, 100));
        var top = NewFlow();

        _btnScan = Btn("Сканировать датчики (Task.WhenAll)", OnScan);
        _numThreshold = Num(1, 20000, 1000, 0, 100, 90);

        top.Controls.AddRange(new Control[]
        {
            _btnScan,
            Btn("Сбросить кэш", (_, _) => { _cache.Clear(); _setStatus("Кэш показаний очищен"); }),
            Lbl("Порог, Вт:"), _numThreshold,
            Btn("Критические приборы", OnFindCritical)
        });

        _lbEvents = new ListBox { Dock = DockStyle.Fill, HorizontalScrollbar = true };

        layout.Controls.Add(top, 0, 0);
        layout.Controls.Add(_lbEvents, 0, 1);
        page.Controls.Add(layout);
        return page;
    }

    private async Task<PollResult> PollSensorAsync(SmartSensor sensor)
    {
        var timer = Stopwatch.StartNew();
        sensor.Activate();
        string reading = await _cache.GetLatestAsync(sensor.Id);
        sensor.Deactivate();
        return new PollResult(sensor.Model, reading, timer.ElapsedMilliseconds);
    }

    private async void OnScan(object? sender, EventArgs e)
    {
        var sensors = _devices.OfType<SmartSensor>().ToList();
        if (sensors.Count == 0) return;

        _btnScan.Enabled = false;
        _setStatus($"Опрос датчиков: {sensors.Count}...");
        try
        {
            var total = Stopwatch.StartNew();
            PollResult[] results = await Task.WhenAll(sensors.Select(PollSensorAsync));
            total.Stop();

            results.ToList().ForEach(r =>
                _lbEvents.Items.Insert(0, $"{DateTime.Now:HH:mm:ss}  {r.Model}: {r.Reading} — {r.Milliseconds} мс"));
            _lbEvents.Items.Insert(0,
                $"Опрос {results.Length} датчиков занял {total.ElapsedMilliseconds} мс, по отдельности было бы {results.Sum(r => r.Milliseconds)} мс");

            BindDevices(_devices);
            _setStatus("Опрос завершён");
        }
        finally
        {
            _btnScan.Enabled = true;
        }
    }

    private void OnFindCritical(object? sender, EventArgs e)
    {
        Func<double, bool> overload = DeviceRules.CreateOverloadCheck((double)_numThreshold.Value);
        Predicate<Device> critical = DeviceRules.Critical(overload, DeviceRules.NominalPower);

        var found = DeviceRules.Where(_devices, critical);
        double daily = found.Sum(DeviceRules.DailyConsumption);

        _lbEvents.Items.Insert(0, $"Критические (мощнее {_numThreshold.Value:0} Вт): {found.Count} шт., расход {daily:0.##} кВт·ч в сутки");
        found.ForEach(d => _lbEvents.Items.Insert(0, $"    {d}"));
    }

    // ================= Вкладка 3: LINQ-аналитика и DTO =================

    private TabPage BuildAnalyticsTab()
    {
        var page = new TabPage("LINQ-Аналитика & DTO");
        var layout = NewLayout(new RowStyle(SizeType.Absolute, 56), new RowStyle(SizeType.Percent, 55), new RowStyle(SizeType.Percent, 45));
        var top = NewFlow();

        _cbTypeFilter = NewCombo(110);
        _cbTypeFilter.Items.AddRange(new object[] { "Все типы", "Sensor", "Actuator" });
        _cbTypeFilter.SelectedIndex = 0;
        _tbSearch = new TextBox { Width = 150, Margin = new Padding(2, 5, 2, 0) };
        _numMinPower = Num(0, 20000, 0, 1, 10, 90);

        top.Controls.AddRange(new Control[]
        {
            Lbl("Тип:"), _cbTypeFilter,
            Lbl("Поиск:"), _tbSearch,
            Lbl("Мощность от, Вт:"), _numMinPower,
            Btn("Отложенное и ToList()", (_, _) => _tbSummary.Text = ReportAnalytics.DeferredDemo(CurrentDtos()))
        });

        _cbTypeFilter.SelectedIndexChanged += (_, _) => ApplyFilters();
        _tbSearch.TextChanged += (_, _) => ApplyFilters();
        _numMinPower.ValueChanged += (_, _) => ApplyFilters();

        _gridDto = NewGrid();
        _gridRoomStats = NewGrid();
        _tbSummary = NewOutput();

        var bottom = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        bottom.Controls.Add(_gridRoomStats, 0, 0);
        bottom.Controls.Add(_tbSummary, 1, 0);

        layout.Controls.Add(top, 0, 0);
        layout.Controls.Add(_gridDto, 0, 1);
        layout.Controls.Add(bottom, 0, 2);
        page.Controls.Add(layout);
        return page;
    }

    private List<DeviceReportDto> CurrentDtos() => ReportAnalytics.BuildDto(_devices, _rooms, _logs);

    private void ApplyFilters()
    {
        if (_gridDto == null || _devices.Count == 0) return;

        string? type = _cbTypeFilter.SelectedIndex > 0 ? _cbTypeFilter.SelectedItem?.ToString() : null;
        var filtered = ReportAnalytics.Filter(CurrentDtos(), type, _tbSearch.Text, (double)_numMinPower.Value).ToList();

        _gridDto.DataSource = filtered;
        FormatColumns(_gridDto, "0.##", "AvgReading");

        _gridRoomStats.DataSource = ReportAnalytics.RoomStats(filtered);
        FormatColumns(_gridRoomStats, "N1", "TotalPower", "AveragePower", "MaxPower");
        _tbSummary.Text = ReportAnalytics.BuildSummary(filtered);
    }

    // ================= Вкладка 4: Множества и пагинация =================

    private TabPage BuildSetsTab()
    {
        var page = new TabPage("Множества & Пагинация");
        var layout = NewLayout(
            new RowStyle(SizeType.Absolute, 52), new RowStyle(SizeType.Percent, 35),
            new RowStyle(SizeType.Absolute, 48), new RowStyle(SizeType.Percent, 65));

        var setsTop = NewFlow();
        _cbRoomA = NewCombo(120);
        _cbRoomA.DisplayMember = "Title";
        _cbRoomB = NewCombo(120);
        _cbRoomB.DisplayMember = "Title";
        _lblSets = new Label { AutoSize = true, Margin = new Padding(12, 9, 2, 0) };

        setsTop.Controls.AddRange(new Control[]
        {
            Lbl("Комната A:"), _cbRoomA,
            Lbl("Комната B:"), _cbRoomB,
            Btn("Intersect", (_, _) => ShowSet("Общие модели (Intersect)", ReportAnalytics.CommonModels)),
            Btn("Except", (_, _) => ShowSet("Только в комнате A (Except)", ReportAnalytics.OnlyInFirst)),
            Btn("Union", (_, _) => ShowSet("Все модели (Union)", ReportAnalytics.AllModels)),
            _lblSets
        });

        _gridSets = NewGrid();

        var pager = NewFlow();
        _numPageSize = Num(1, 50, 6, 0, 1, 60);
        _lblPage = new Label { AutoSize = true, Margin = new Padding(8, 9, 8, 0), Text = "Страница 1" };
        pager.Controls.AddRange(new Control[]
        {
            Btn("◀ Назад", (_, _) => { _page--; ShowPage(); }),
            _lblPage,
            Btn("Вперёд ▶", (_, _) => { _page++; ShowPage(); }),
            Lbl("На странице:"), _numPageSize
        });
        _numPageSize.ValueChanged += (_, _) => { _page = 1; ShowPage(); };

        _gridPage = NewGrid();

        layout.Controls.Add(setsTop, 0, 0);
        layout.Controls.Add(_gridSets, 0, 1);
        layout.Controls.Add(pager, 0, 2);
        layout.Controls.Add(_gridPage, 0, 3);
        page.Controls.Add(layout);
        return page;
    }

    private void ShowSet(string title, Func<IEnumerable<DeviceReportDto>, string, string, List<string>> operation)
    {
        if (_cbRoomA.SelectedItem is not Room a || _cbRoomB.SelectedItem is not Room b) return;

        var models = operation(CurrentDtos(), a.Title, b.Title);
        _gridSets.DataSource = models.Select(m => new { Модель = m }).ToList();
        _lblSets.Text = $"{title}: {models.Count} ({a.Title} / {b.Title})";
    }

    private void ShowPage()
    {
        if (_gridPage == null || _devices.Count == 0) return;

        var all = ReportAnalytics.Filter(CurrentDtos(), null, null, 0).ToList();
        int pageSize = (int)_numPageSize.Value;
        int pages = Math.Max(1, (int)Math.Ceiling(all.Count / (double)pageSize));
        _page = Math.Clamp(_page, 1, pages);

        _gridPage.DataSource = ReportAnalytics.Page(all, _page, pageSize);
        FormatColumns(_gridPage, "0.##", "AvgReading");
        _lblPage.Text = $"Страница {_page} из {pages}";
    }
}
