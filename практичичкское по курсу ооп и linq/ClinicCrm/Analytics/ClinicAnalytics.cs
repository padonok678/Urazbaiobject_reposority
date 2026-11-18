using ClinicCrm.Models;

namespace ClinicCrm.Analytics;

public static class ClinicAnalytics
{
    /// <summary>Join: Appointments + Doctors + Departments → плоский DTO.</summary>
    public static List<AppointmentViewDto> BuildDto(
        IEnumerable<Appointment> appointments,
        IEnumerable<MedicalEmployee> employees,
        IEnumerable<Department> departments) =>
        appointments
            .Join(employees.OfType<Doctor>(), a => a.DoctorId, d => d.Id, (a, d) => new { a, d })
            .Join(departments, x => x.d.DepartmentId, dep => dep.Id,
                  (x, dep) => new AppointmentViewDto(x.a.PatientName, x.d.FullName, dep.Title, x.a.Cost, x.a.Date, x.a.Status))
            .ToList();

    /// <summary>Where + составная сортировка: отделение, затем стоимость по убыванию.</summary>
    public static IEnumerable<AppointmentViewDto> Filter(
        IEnumerable<AppointmentViewDto> source, string department, string patientText, bool onlyCompleted)
    {
        var query = source;
        if (!string.IsNullOrEmpty(department))
            query = query.Where(d => d.Department == department);
        if (!string.IsNullOrWhiteSpace(patientText))
            query = query.Where(d => d.PatientName.Contains(patientText, StringComparison.OrdinalIgnoreCase));
        if (onlyCompleted)
            query = query.Where(d => d.Status == "Completed");

        return query.OrderBy(d => d.Department).ThenByDescending(d => d.Cost);
    }

    /// <summary>Пагинация Skip/Take.</summary>
    public static List<T> Page<T>(IEnumerable<T> source, int page, int pageSize) =>
        source.Skip((page - 1) * pageSize).Take(pageSize).ToList();

    /// <summary>GroupBy по врачам: Count, Sum, Average.</summary>
    public static List<DoctorStatDto> DoctorStats(IEnumerable<Appointment> appointments, IEnumerable<MedicalEmployee> employees) =>
        appointments
            .GroupBy(a => a.DoctorId)
            .Select(g => new DoctorStatDto(
                employees.FirstOrDefault(e => e.Id == g.Key)?.FullName ?? "—",
                g.Count(),
                g.Sum(a => a.Cost),
                g.Average(a => a.Cost)))
            .OrderByDescending(s => s.Revenue)
            .ToList();

    public static List<string> PatientsOf(IEnumerable<AppointmentViewDto> dtos, string department) =>
        dtos.Where(d => d.Department == department)
            .Select(d => d.PatientName)
            .Distinct()
            .ToList();

    /// <summary>Intersect: пациенты обоих отделений.</summary>
    public static List<string> PatientsInBoth(IEnumerable<AppointmentViewDto> dtos, string depA, string depB) =>
        PatientsOf(dtos, depA).Intersect(PatientsOf(dtos, depB)).ToList();

    /// <summary>Except: пациенты только первого отделения.</summary>
    public static List<string> PatientsOnlyIn(IEnumerable<AppointmentViewDto> dtos, string depA, string depB) =>
        PatientsOf(dtos, depA).Except(PatientsOf(dtos, depB)).ToList();
}
