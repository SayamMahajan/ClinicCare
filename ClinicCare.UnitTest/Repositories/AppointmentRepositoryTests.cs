using ClinicCare.DataAccess.Data;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories;
using ClinicCare.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClinicCare.UnitTest.Repositories
{
    [TestClass]
    public class AppointmentRepositoryTests
    {
        private AppDbContext _context = null!;
        private AppointmentRepository _repo = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repo = new AppointmentRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private static Patient CreatePatient()
        {
            return new Patient
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Patient",
                DOB = DateTime.UtcNow.AddYears(-25),
                Gender = Gender.Male,
                Email = $"patient{Guid.NewGuid()}@test.com",
                Password = "Password@123",
                Phone = "9999999999"
            };
        }

        private static Employee CreateDoctor()
        {
            return new Employee
            {
                Id = Guid.NewGuid(),
                Role = EmployeeRole.Doctor,
                FirstName = "Dr",
                LastName = "Smith",
                Email = $"doctor{Guid.NewGuid()}@test.com",
                Password = "Password@123",
                DateOfJoining = DateTime.UtcNow
            };
        }

        private static Appointment CreateAppointment(
            Guid patientId,
            Guid doctorId,
            Patient patient,
            Employee doctor)
        {
            return new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                DoctorId = doctorId,
                Status = AppointmentStatus.Requested,
                Date = DateTime.UtcNow.AddDays(3),
                TimeSlot = TimeSlotType.Morning,
                Patient = patient,
                Doctor = doctor
            };
        }

        [TestMethod]
        public async Task GetAllWithDetailsAsync_ReturnsAppointments_WithAllNavigationProperties()
        {
            var patient = CreatePatient();
            var doctor = CreateDoctor();

            var appointment = CreateAppointment(patient.Id, doctor.Id, patient, doctor);

            _context.Patients.Add(patient);
            _context.Employees.Add(doctor);
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            var result = (await _repo.GetAllWithDetailsAsync()).ToList();

            Assert.AreEqual(1, result.Count);

            var a = result.First();
            Assert.IsNotNull(a.Patient);
            Assert.IsNotNull(a.Doctor);
            Assert.AreEqual(patient.Id, a.Patient.Id);
            Assert.AreEqual(doctor.Id, a.Doctor.Id);
        }

        [TestMethod]
        public async Task GetByIdWithDetailsAsync_ExistingId_ReturnsAppointment()
        {
            var patient = CreatePatient();
            var doctor = CreateDoctor();
            var appointment = CreateAppointment(patient.Id, doctor.Id, patient, doctor);

            _context.Patients.Add(patient);
            _context.Employees.Add(doctor);
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            var result = await _repo.GetByIdWithDetailsAsync(appointment.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(appointment.Id, result!.Id);
            Assert.IsNotNull(result.Patient);
            Assert.IsNotNull(result.Doctor);
        }

        [TestMethod]
        public async Task GetByIdWithDetailsAsync_InvalidId_ReturnsNull()
        {
            var result = await _repo.GetByIdWithDetailsAsync(Guid.NewGuid());

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetByDoctorIdAsync_ReturnsOnlyDoctorsAppointments()
        {
            var patient = CreatePatient();
            var doctor1 = CreateDoctor();
            var doctor2 = CreateDoctor();

            var a1 = CreateAppointment(patient.Id, doctor1.Id, patient, doctor1);
            var a2 = CreateAppointment(patient.Id, doctor2.Id, patient, doctor2);

            _context.Patients.Add(patient);
            _context.Employees.AddRange(doctor1, doctor2);
            _context.Appointments.AddRange(a1, a2);
            await _context.SaveChangesAsync();

            var result = (await _repo.GetByDoctorIdAsync(doctor1.Id)).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(doctor1.Id, result.First().DoctorId);
            Assert.IsNotNull(result.First().Patient);
        }

        [TestMethod]
        public async Task GetByDoctorIdAsync_NoMatch_ReturnsEmptyList()
        {
            var result = await _repo.GetByDoctorIdAsync(Guid.NewGuid());

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetByPatientIdAsync_ReturnsOnlyPatientsAppointments()
        {
            var patient1 = CreatePatient();
            var patient2 = CreatePatient();
            var doctor = CreateDoctor();

            var a1 = CreateAppointment(patient1.Id, doctor.Id, patient1, doctor);
            var a2 = CreateAppointment(patient2.Id, doctor.Id, patient2, doctor);

            _context.Patients.AddRange(patient1, patient2);
            _context.Employees.Add(doctor);
            _context.Appointments.AddRange(a1, a2);
            await _context.SaveChangesAsync();

            var result = (await _repo.GetByPatientIdAsync(patient1.Id)).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(patient1.Id, result.First().PatientId);
            Assert.IsNotNull(result.First().Doctor);
        }

        [TestMethod]
        public async Task GetByPatientIdAsync_NoMatch_ReturnsEmptyList()
        {
            var result = await _repo.GetByPatientIdAsync(Guid.NewGuid());

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }
    }
}
