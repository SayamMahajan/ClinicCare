using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Services;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Appointment;
using ClinicCare.Shared.Enums;
using Moq;

namespace ClinicCare.UnitTest.Services
{
    [TestClass]
    public class AppointmentServiceTests
    {
        private Mock<IGenericRepository<Appointment>> _repoMock = null!;
        private Mock<IAppointmentRepository> _appointmentRepoMock = null!;
        private Mock<IGenericRepository<Employee>> _employeeRepoMock = null!;
        private Mock<ICurrentUser> _currentUserMock = null!;
        private AppointmentService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repoMock = new Mock<IGenericRepository<Appointment>>();
            _appointmentRepoMock = new Mock<IAppointmentRepository>();
            _employeeRepoMock = new Mock<IGenericRepository<Employee>>();
            _currentUserMock = new Mock<ICurrentUser>();

            _service = new AppointmentService(
                _repoMock.Object,
                _appointmentRepoMock.Object,
                _employeeRepoMock.Object,
                _currentUserMock.Object
            );
        }

        [TestMethod]
        public async Task GetAllAsync_Admin_ReturnsAllAppointments()
        {
            _currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Admin);

            var appointment = CreateAppointment(Guid.NewGuid(), Guid.NewGuid());
            _appointmentRepoMock.Setup(r => r.GetAllWithDetailsAsync())
                                .ReturnsAsync(new List<Appointment> { appointment });

            var result = await _service.GetAllAsync();

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(appointment.Id, result.First().Id);
        }

        [TestMethod]
        public async Task GetAllAsync_Doctor_ReturnsDoctorAppointments()
        {
            var doctorId = Guid.NewGuid();
            _currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Doctor);
            _currentUserMock.SetupGet(c => c.UserId).Returns(doctorId);

            var appointment = CreateAppointment(Guid.NewGuid(), doctorId);
            _appointmentRepoMock.Setup(r => r.GetByDoctorIdAsync(doctorId))
                                .ReturnsAsync(new List<Appointment> { appointment });

            var result = await _service.GetAllAsync();

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(doctorId, result.First().Doctor.Id);
        }

        [TestMethod]
        public async Task GetAllAsync_Patient_ReturnsPatientAppointments()
        {
            var patientId = Guid.NewGuid();
            _currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Patient);
            _currentUserMock.SetupGet(c => c.UserId).Returns(patientId);

            var appointment = CreateAppointment(patientId, Guid.NewGuid());
            _appointmentRepoMock.Setup(r => r.GetByPatientIdAsync(patientId))
                                .ReturnsAsync(new List<Appointment> { appointment });

            var result = await _service.GetAllAsync();

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(patientId, result.First().Patient.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public async Task GetAllAsync_InvalidRole_ThrowsForbidden()
        {
            _currentUserMock.SetupGet(c => c.Role).Returns((UserRole)99);
            await _service.GetAllAsync();
        }

        [TestMethod]
        public async Task GetByIdAsync_Valid_ReturnsAppointment()
        {
            var patientId = Guid.NewGuid();
            var doctorId = Guid.NewGuid();

            _currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Patient);
            _currentUserMock.SetupGet(c => c.UserId).Returns(patientId);

            var appointment = CreateAppointment(patientId, doctorId);
            _appointmentRepoMock.Setup(r => r.GetByIdWithDetailsAsync(appointment.Id))
                                .ReturnsAsync(appointment);

            var result = await _service.GetByIdAsync(appointment.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(appointment.Id, result.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public async Task GetByIdAsync_InvalidId_ThrowsNotFound()
        {
            var id = Guid.NewGuid();
            _appointmentRepoMock.Setup(r => r.GetByIdWithDetailsAsync(id))
                                .ReturnsAsync((Appointment?)null);

            await _service.GetByIdAsync(id);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public async Task GetByIdAsync_PatientAccessingOthers_ThrowsForbidden()
        {
            var patientId = Guid.NewGuid();
            _currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Patient);
            _currentUserMock.SetupGet(c => c.UserId).Returns(patientId);

            var appointment = CreateAppointment(Guid.NewGuid(), Guid.NewGuid());
            _appointmentRepoMock.Setup(r => r.GetByIdWithDetailsAsync(appointment.Id))
                                .ReturnsAsync(appointment);

            await _service.GetByIdAsync(appointment.Id);
        }

        [TestMethod]
        public async Task CreateAsync_ValidPatient_CreatesAppointment()
        {
            var patientId = Guid.NewGuid();
            var doctorId = Guid.NewGuid();
            var futureDate = DateTime.UtcNow.AddDays(2);

            _currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Patient);
            _currentUserMock.SetupGet(c => c.UserId).Returns(patientId);

            var doctor = new Employee
            {
                Id = doctorId,
                Role = EmployeeRole.Doctor,
                DoctorDetails = new DoctorDetail { SpecializationId = Guid.NewGuid() }
            };
            _employeeRepoMock.Setup(r => r.GetByIdAsync(doctorId)).ReturnsAsync(doctor);
            _appointmentRepoMock.Setup(r => r.GetByPatientIdAsync(patientId))
                                .ReturnsAsync(new List<Appointment>());

            Appointment? inserted = null;
            _repoMock.Setup(r => r.InsertAsync(It.IsAny<Appointment>()))
                     .Callback<Appointment>(a => inserted = a)
                     .Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new AppointmentCreateDto
            {
                PatientId = patientId,
                DoctorId = doctorId,
                Date = futureDate,
                TimeSlot = TimeSlotType.Morning
            };

            var result = await _service.CreateAsync(dto);

            Assert.IsNotNull(inserted);
            Assert.AreEqual(result, inserted!.Id);
            Assert.AreEqual(patientId, inserted.PatientId);
            Assert.AreEqual(doctorId, inserted.DoctorId);
            Assert.AreEqual(AppointmentStatus.Requested, inserted.Status);
        }

        [TestMethod]
        [ExpectedException(typeof(ConflictException))]
        public async Task CreateAsync_AppointmentConflict_ThrowsConflict()
        {
            var patientId = Guid.NewGuid();
            var doctorId = Guid.NewGuid();
            var futureDate = DateTime.UtcNow.AddDays(2);

            _currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Patient);
            _currentUserMock.SetupGet(c => c.UserId).Returns(patientId);

            var specializationId = Guid.NewGuid();
            var doctor = new Employee
            {
                Id = doctorId,
                Role = EmployeeRole.Doctor,
                DoctorDetails = new DoctorDetail { SpecializationId = specializationId }
            };
            _employeeRepoMock.Setup(r => r.GetByIdAsync(doctorId)).ReturnsAsync(doctor);

            var existing = CreateAppointment(patientId, doctorId);
            existing.Date = futureDate;
            existing.TimeSlot = TimeSlotType.Morning;
            existing.Doctor.DoctorDetails!.SpecializationId = specializationId;

            _appointmentRepoMock.Setup(r => r.GetByPatientIdAsync(patientId))
                                .ReturnsAsync(new List<Appointment> { existing });

            var dto = new AppointmentCreateDto
            {
                PatientId = patientId,
                DoctorId = doctorId,
                Date = futureDate,
                TimeSlot = TimeSlotType.Morning 
            };

            await _service.CreateAsync(dto);
        }


        [TestMethod]
        public async Task UpdateAsync_Valid_UpdatesAppointment()
        {
            var appointment = CreateAppointment(Guid.NewGuid(), Guid.NewGuid());
            var id = appointment.Id;

            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(appointment);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            _currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Doctor);
            _currentUserMock.SetupGet(c => c.UserId).Returns(appointment.DoctorId);

            var dto = new AppointmentUpdateDto
            {
                Status = AppointmentStatus.Approved,
                Date = DateTime.UtcNow.AddDays(3),
                TimeSlot = TimeSlotType.Night
            };

            await _service.UpdateAsync(id, dto);

            Assert.AreEqual(AppointmentStatus.Approved, appointment.Status);
            Assert.AreEqual(dto.Date.Value, appointment.Date);
            Assert.AreEqual(dto.TimeSlot, appointment.TimeSlot);
        }


        [TestMethod]
        public async Task DeleteAsync_Valid_DeletesAppointment()
        {
            var appointment = CreateAppointment(Guid.NewGuid(), Guid.NewGuid());
            var id = appointment.Id;

            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(appointment);
            _repoMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _service.DeleteAsync(id);

            _repoMock.Verify(r => r.DeleteAsync(id), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        private static Appointment CreateAppointment(Guid patientId, Guid doctorId)
        {
            return new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                DoctorId = doctorId,
                Date = DateTime.UtcNow.AddDays(2),
                TimeSlot = TimeSlotType.Earlynoon,
                Status = AppointmentStatus.Requested,
                Patient = new Patient
                {
                    Id = patientId,
                    FirstName = "John",
                    LastName = "Doe"
                },
                Doctor = new Employee
                {
                    Id = doctorId,
                    FirstName = "Dr",
                    LastName = "Smith",
                    DoctorDetails = new DoctorDetail { SpecializationId = Guid.NewGuid() }
                }
            };
        }
    }
}
