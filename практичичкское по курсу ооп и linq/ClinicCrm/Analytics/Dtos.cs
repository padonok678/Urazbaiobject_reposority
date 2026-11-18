using System.ComponentModel;

namespace ClinicCrm.Analytics;

public class AppointmentViewDto
{
    [DisplayName("Пациент")] public string PatientName { get; }
    [DisplayName("Врач")] public string DoctorName { get; }
    [DisplayName("Отделение")] public string Department { get; }
    [DisplayName("Стоимость")] public decimal Cost { get; }
    [DisplayName("Дата")] public DateTime Date { get; }
    [DisplayName("Статус")] public string Status { get; }

    public AppointmentViewDto(string patientName, string doctorName, string department, decimal cost, DateTime date, string status)
    {
        PatientName = patientName;
        DoctorName = doctorName;
        Department = department;
        Cost = cost;
        Date = date;
        Status = status;
    }
}

public class DoctorStatDto
{
    [DisplayName("Врач")] public string DoctorName { get; }
    [DisplayName("Приёмов")] public int AppointmentsCount { get; }
    [DisplayName("Выручка")] public decimal Revenue { get; }
    [DisplayName("Средний чек")] public decimal AverageCheck { get; }

    public DoctorStatDto(string doctorName, int count, decimal revenue, decimal average)
    {
        DoctorName = doctorName;
        AppointmentsCount = count;
        Revenue = revenue;
        AverageCheck = average;
    }
}
