using System;
using System.Collections.Generic;
using System.Linq;
using Web.Doctors;

namespace Doctor.Application.MockData
{
    public static class DoctorMockDataGenerator
    {
        private static readonly Random _random = new Random();
        
        private static readonly string[] _firstNames = { 
            "John", "Emma", "Michael", "Sarah", "David", "Lisa", "James", "Emily", 
            "Robert", "Karen", "William", "Jessica", "Richard", "Amanda", "Thomas", 
            "Michelle", "Charles", "Jennifer", "Christopher", "Patricia", "Daniel",
            "Nancy", "Matthew", "Betty", "Anthony", "Helen", "Mark", "Sandra",
            "Donald", "Donna", "Steven", "Carol", "Paul", "Ruth", "Andrew", "Sharon"
        };
        
        private static readonly string[] _lastNames = { 
            "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", 
            "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Wilson", "Anderson",
            "Thomas", "Taylor", "Moore", "Jackson", "Martin", "Lee", "Perez",
            "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis",
            "Robinson", "Walker", "Young", "Allen", "King", "Wright", "Scott", "Torres"
        };

        private static readonly string[] _specializations = { 
            "Cardiology", "Dermatology", "Endocrinology", "Gastroenterology", 
            "Hematology", "InfectiousDisease", "InternalMedicine", "Nephrology",
            "Neurology", "ObstetricsGynecology", "Oncology", "Ophthalmology",
            "Orthopedics", "Otolaryngology", "Pediatrics", "Psychiatry",
            "Pulmonology", "Radiology", "Rheumatology", "Urology",
            "GeneralSurgery", "PlasticSurgery", "VascularSurgery",
            "AllergyImmunology", "EmergencyMedicine", "FamilyMedicine",
            "Geriatrics", "SportsMedicine"
        };

        private static readonly string[] _departments = {
            "Internal Medicine", "Surgery", "Pediatrics", "Cardiology",
            "Neurology", "Oncology", "Orthopedics", "Radiology",
            "Emergency", "Family Medicine", "Obstetrics & Gynecology",
            "Ophthalmology", "Urology", "Psychiatry", "Dermatology",
            "Anesthesiology", "Pathology", "Physical Medicine"
        };

        private static readonly string[] _statuses = { "Active", "Inactive", "OnLeave" };
        private static readonly string[] _dayOfWeek = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
        private static readonly string[] _cities = { "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego", "Dallas", "San Jose" };
        private static readonly string[] _states = { "NY", "CA", "IL", "TX", "AZ", "PA", "FL", "OH", "MI", "NJ" };

        // ==================== DOCTOR DTOS ====================

        public static List<DoctorDto> GetMockDoctors(int count = 20)
        {
            var doctors = new List<DoctorDto>();
            
            for (int i = 1; i <= count; i++)
            {
                var firstName = GetRandomElement(_firstNames);
                var lastName = GetRandomElement(_lastNames);
                var specialization = GetRandomElement(_specializations);
                var status = GetRandomElement(_statuses);
                var isAvailable = status == "Active" && _random.NextDouble() > 0.2;
                
                var doctor = new DoctorDto
                {
                    Id = i,
                    FullName = $"{firstName} {lastName}",
                    FirstName = firstName,
                    LastName = lastName,
                    Specialization = specialization,
                    Email = $"{firstName.ToLower()}.{lastName.ToLower()}{_random.Next(1, 100)}@hospital.com",
                    PhoneNumber = $"+1-{_random.Next(200, 999)}-{_random.Next(100, 999)}-{_random.Next(1000, 9999)}",
                    MobileNumber = _random.NextDouble() > 0.4 ? $"+1-{_random.Next(200, 999)}-{_random.Next(100, 999)}-{_random.Next(1000, 9999)}" : null,
                    Address = _random.NextDouble() > 0.3 ? $"{_random.Next(100, 999)} {GetRandomElement(new[] { "Main", "Oak", "Pine", "Maple", "Cedar", "Elm", "Washington", "Lincoln" })} St, {GetRandomElement(_cities)}, {GetRandomElement(_states)} {_random.Next(10000, 99999)}" : null,
                    Bio = GenerateBio(specialization),
                    Qualifications = GenerateQualifications(specialization),
                    LicenseNumber = $"MD-{_random.Next(100000, 999999)}",
                    Department = GetRandomElement(_departments),
                    Status = status,
                    IsAvailableForAppointments = isAvailable,
                    Schedules = GenerateDoctorSchedules(i)
                };
                
                doctors.Add(doctor);
            }
            
            return doctors;
        }

        public static DoctorDto GetSingleDoctor(int id = 1)
        {
            return GetMockDoctors(1).FirstOrDefault() ?? new DoctorDto
            {
                Id = id,
                FullName = "Dr. John Smith",
                FirstName = "John",
                LastName = "Smith",
                Specialization = "Cardiology",
                Email = "john.smith@hospital.com",
                PhoneNumber = "+1-555-123-4567",
                Status = "Active",
                IsAvailableForAppointments = true
            };
        }

        public static List<CreateDoctorDto> GetMockCreateDoctorDtos(int count = 5)
        {
            var createDtos = new List<CreateDoctorDto>();
            
            for (int i = 1; i <= count; i++)
            {
                var firstName = GetRandomElement(_firstNames);
                var lastName = GetRandomElement(_lastNames);
                var specialization = GetRandomElement(_specializations);
                
                var createDto = new CreateDoctorDto
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Specialization = specialization,
                    Email = $"{firstName.ToLower()}.{lastName.ToLower()}_{_random.Next(100, 999)}@hospital.com",
                    PhoneNumber = $"+1-{_random.Next(200, 999)}-{_random.Next(100, 999)}-{_random.Next(1000, 9999)}",
                    MobileNumber = _random.NextDouble() > 0.5 ? $"+1-{_random.Next(200, 999)}-{_random.Next(100, 999)}-{_random.Next(1000, 9999)}" : null,
                    Address = _random.NextDouble() > 0.3 ? $"{_random.Next(100, 999)} {GetRandomElement(new[] { "Main", "Oak", "Pine", "Maple" })} St, {GetRandomElement(_cities)}, {GetRandomElement(_states)} {_random.Next(10000, 99999)}" : null,
                    Bio = GenerateBio(specialization),
                    Qualifications = GenerateQualifications(specialization),
                    LicenseNumber = $"MD-{_random.Next(100000, 999999)}",
                    Department = GetRandomElement(_departments),
                    UserId = _random.NextDouble() > 0.5 ? _random.Next(100, 999) : null,
                    Schedules = GenerateCreateScheduleDtos(i)
                };
                
                createDtos.Add(createDto);
            }
            
            return createDtos;
        }

        public static CreateDoctorDto GetSingleCreateDoctorDto()
        {
            return GetMockCreateDoctorDtos(1).FirstOrDefault() ?? new CreateDoctorDto
            {
                FirstName = "Jane",
                LastName = "Doe",
                Specialization = "Neurology",
                Email = "jane.doe@hospital.com",
                PhoneNumber = "+1-555-987-6543",
                Bio = "Experienced neurologist with expertise in stroke management",
                Qualifications = "MD, PhD, Board Certified in Neurology",
                Department = "Neurology"
            };
        }

        public static List<UpdateDoctorDto> GetMockUpdateDoctorDtos()
        {
            return new List<UpdateDoctorDto>
            {
                new UpdateDoctorDto
                {
                    FirstName = "UpdatedFirstName",
                    LastName = "UpdatedLastName",
                    Specialization = "Cardiology",
                    Email = "updated.email@hospital.com",
                    PhoneNumber = "+1-555-555-5555",
                    MobileNumber = "+1-555-555-1234",
                    Address = "456 Updated St, New York, NY 10001",
                    Bio = "Updated bio information with more experience",
                    Qualifications = "MD, PhD, FACC, Board Certified in Cardiology",
                    Department = "Cardiology",
                    Status = "Active",
                    IsAvailableForAppointments = true
                },
                new UpdateDoctorDto
                {
                    FirstName = "Jane",
                    LastName = "Smith",
                    Specialization = "Neurology",
                    Email = "jane.smith@hospital.com",
                    PhoneNumber = "+1-555-123-4567",
                    Status = "OnLeave",
                    IsAvailableForAppointments = false
                },
                new UpdateDoctorDto
                {
                    Specialization = "Orthopedics",
                    Department = "Orthopedic Surgery",
                    Status = "Active"
                }
            };
        }

        public static UpdateDoctorDto GetSingleUpdateDoctorDto()
        {
            return GetMockUpdateDoctorDtos().FirstOrDefault() ?? new UpdateDoctorDto
            {
                FirstName = "Robert",
                LastName = "Johnson",
                Specialization = "Pediatrics",
                Status = "Active",
                IsAvailableForAppointments = true
            };
        }

        // ==================== SCHEDULE DTOS ====================

        public static List<DoctorScheduleDto> GenerateDoctorSchedules(int doctorId)
        {
            var schedules = new List<DoctorScheduleDto>();
            var daysToSchedule = GetRandomElements(_dayOfWeek, _random.Next(3, 6));
            
            foreach (var day in daysToSchedule)
            {
                var startHour = 7 + _random.Next(0, 4);
                var endHour = startHour + 4 + _random.Next(1, 4);
                var validFrom = DateTime.Now.AddDays(-_random.Next(0, 30));
                var validTo = validFrom.AddMonths(6 + _random.Next(0, 12));
                
                schedules.Add(new DoctorScheduleDto
                {
                    Id = _random.Next(1000, 9999),
                    DoctorId = doctorId,
                    DayOfWeek = day,
                    StartTime = new TimeSpan(startHour, 0, 0),
                    EndTime = new TimeSpan(endHour, 0, 0),
                    SlotDurationInMinutes = new[] { 15, 20, 30, 45, 60 }[_random.Next(0, 5)],
                    Status = "Active",
                    ValidFrom = validFrom,
                    ValidTo = validTo,
                    IsActive = true
                });
            }
            
            return schedules;
        }

        public static DoctorScheduleDto GetSingleScheduleDto(int doctorId = 1)
        {
            return new DoctorScheduleDto
            {
                Id = 1001,
                DoctorId = doctorId,
                DayOfWeek = "Monday",
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(17, 0, 0),
                SlotDurationInMinutes = 30,
                Status = "Active",
                ValidFrom = DateTime.Now,
                ValidTo = DateTime.Now.AddMonths(6),
                IsActive = true
            };
        }

        public static List<CreateDoctorScheduleDto> GenerateCreateScheduleDtos(int doctorId)
        {
            var schedules = new List<CreateDoctorScheduleDto>();
            var daysToSchedule = GetRandomElements(_dayOfWeek, _random.Next(3, 6));
            
            foreach (var day in daysToSchedule)
            {
                var startHour = 7 + _random.Next(0, 4);
                var endHour = startHour + 4 + _random.Next(1, 4);
                
                schedules.Add(new CreateDoctorScheduleDto
                {
                    DoctorId = doctorId,
                    DayOfWeek = day,
                    StartTime = new TimeSpan(startHour, 0, 0),
                    EndTime = new TimeSpan(endHour, 0, 0),
                    SlotDurationInMinutes = new[] { 15, 20, 30, 45, 60 }[_random.Next(0, 5)],
                    ValidFrom = DateTime.Now.AddDays(1),
                    ValidTo = DateTime.Now.AddMonths(6)
                });
            }
            
            return schedules;
        }

        public static CreateDoctorScheduleDto GetSingleCreateScheduleDto(int doctorId = 1)
        {
            return new CreateDoctorScheduleDto
            {
                DoctorId = doctorId,
                DayOfWeek = "Monday",
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(17, 0, 0),
                SlotDurationInMinutes = 30,
                ValidFrom = DateTime.Now.AddDays(1),
                ValidTo = DateTime.Now.AddMonths(6)
            };
        }

        public static List<UpdateDoctorScheduleDto> GetMockUpdateScheduleDtos()
        {
            return new List<UpdateDoctorScheduleDto>
            {
                new UpdateDoctorScheduleDto
                {
                    StartTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    SlotDurationInMinutes = 45,
                    Status = "Active",
                    ValidFrom = DateTime.Now.AddDays(1),
                    ValidTo = DateTime.Now.AddMonths(3)
                },
                new UpdateDoctorScheduleDto
                {
                    Status = "Inactive",
                    SlotDurationInMinutes = 30
                },
                new UpdateDoctorScheduleDto
                {
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(16, 0, 0),
                    ValidTo = DateTime.Now.AddMonths(12)
                }
            };
        }

        public static UpdateDoctorScheduleDto GetSingleUpdateScheduleDto()
        {
            return GetMockUpdateScheduleDtos().FirstOrDefault() ?? new UpdateDoctorScheduleDto
            {
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(17, 0, 0),
                SlotDurationInMinutes = 30,
                Status = "Active"
            };
        }

        // ==================== AVAILABILITY DTOS ====================

        public static List<DoctorAvailabilityDto> GetMockDoctorAvailability(int count = 10)
        {
            var availabilities = new List<DoctorAvailabilityDto>();
            var doctors = GetMockDoctors(count);
            
            foreach (var doctor in doctors)
            {
                var date = DateTime.Now.Date.AddDays(_random.Next(1, 30));
                var slots = GenerateTimeSlots();
                
                availabilities.Add(new DoctorAvailabilityDto
                {
                    DoctorId = doctor.Id,
                    DoctorName = doctor.FullName,
                    Specialization = doctor.Specialization,
                    Date = date,
                    AvailableSlots = slots
                });
            }
            
            return availabilities;
        }

        public static DoctorAvailabilityDto GetSingleDoctorAvailability(int doctorId = 1)
        {
            var doctor = GetSingleDoctor(doctorId);
            
            return new DoctorAvailabilityDto
            {
                DoctorId = doctorId,
                DoctorName = doctor.FullName,
                Specialization = doctor.Specialization,
                Date = DateTime.Now.Date.AddDays(3),
                AvailableSlots = GenerateTimeSlots(8, 6)
            };
        }

        public static List<TimeSlotDto> GenerateTimeSlots(int startHour = 8, int numberOfSlots = 10)
        {
            var slots = new List<TimeSlotDto>();
            
            for (int i = 0; i < numberOfSlots; i++)
            {
                var slotStart = new TimeSpan(startHour + i / 2, (i % 2) * 30, 0);
                var slotEnd = slotStart.Add(TimeSpan.FromMinutes(30));
                var isAvailable = _random.NextDouble() > 0.3;
                
                slots.Add(new TimeSlotDto
                {
                    StartTime = slotStart,
                    EndTime = slotEnd,
                    IsAvailable = isAvailable,
                    IsBooked = !isAvailable && _random.NextDouble() > 0.5
                });
            }
            
            return slots;
        }

        public static TimeSlotDto GetSingleTimeSlot()
        {
            return new TimeSlotDto
            {
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(9, 30, 0),
                IsAvailable = true,
                IsBooked = false
            };
        }

        // ==================== SEARCH DTOS ====================

        public static List<DoctorSearchDto> GetMockSearchParameters()
        {
            return new List<DoctorSearchDto>
            {
                new DoctorSearchDto
                {
                    SearchTerm = "Cardiology",
                    Specialization = "Cardiology",
                    AvailableDate = DateTime.Now.Date.AddDays(5),
                    PageNumber = 1,
                    PageSize = 10
                },
                new DoctorSearchDto
                {
                    SearchTerm = "Smith",
                    Specialization = null,
                    AvailableDate = null,
                    PageNumber = 1,
                    PageSize = 20
                },
                new DoctorSearchDto
                {
                    SearchTerm = null,
                    Specialization = "Pediatrics",
                    AvailableDate = DateTime.Now.Date.AddDays(2),
                    PageNumber = 2,
                    PageSize = 15
                },
                new DoctorSearchDto
                {
                    SearchTerm = "Neurology",
                    Specialization = "Neurology",
                    AvailableDate = DateTime.Now.Date.AddDays(7),
                    PageNumber = 1,
                    PageSize = 10
                },
                new DoctorSearchDto
                {
                    SearchTerm = "Dr. Johnson",
                    Specialization = null,
                    AvailableDate = null,
                    PageNumber = 1,
                    PageSize = 5
                }
            };
        }

        public static DoctorSearchDto GetSingleSearchParameter()
        {
            return GetMockSearchParameters().FirstOrDefault() ?? new DoctorSearchDto
            {
                SearchTerm = "Cardiology",
                Specialization = "Cardiology",
                AvailableDate = DateTime.Now.Date.AddDays(5),
                PageNumber = 1,
                PageSize = 10
            };
        }

        // ==================== STATS DTOS ====================

        public static List<DoctorStatsDto> GetMockDoctorStats()
        {
            return new List<DoctorStatsDto>
            {
                GetLargeHospitalStats(),
                GetMediumClinicStats(),
                GetSmallPracticeStats(),
                GetSpecializedCenterStats(),
                GetBusyHospitalStats(),
                GetRandomStats()
            };
        }

        public static DoctorStatsDto GetSingleDoctorStats()
        {
            return GetLargeHospitalStats();
        }

        public static DoctorStatsDto GetLargeHospitalStats()
        {
            return new DoctorStatsDto
            {
                TotalDoctors = 245,
                ActiveDoctors = 210,
                InactiveDoctors = 20,
                OnLeaveDoctors = 15,
                AvailableDoctors = 185,
                DoctorsBySpecialization = new Dictionary<string, int>
                {
                    { "Cardiology", 25 },
                    { "Dermatology", 10 },
                    { "Endocrinology", 8 },
                    { "Gastroenterology", 12 },
                    { "Neurology", 15 },
                    { "Oncology", 18 },
                    { "Orthopedics", 20 },
                    { "Pediatrics", 22 },
                    { "Psychiatry", 12 },
                    { "Radiology", 16 },
                    { "GeneralSurgery", 30 },
                    { "EmergencyMedicine", 28 },
                    { "FamilyMedicine", 15 },
                    { "Urology", 14 }
                },
                AverageAppointmentsPerDay = 7.8,
                WeeklyAvailability = new Dictionary<string, int>
                {
                    { "Monday", 185 },
                    { "Tuesday", 192 },
                    { "Wednesday", 188 },
                    { "Thursday", 190 },
                    { "Friday", 175 },
                    { "Saturday", 85 },
                    { "Sunday", 30 }
                }
            };
        }

        public static DoctorStatsDto GetMediumClinicStats()
        {
            return new DoctorStatsDto
            {
                TotalDoctors = 85,
                ActiveDoctors = 72,
                InactiveDoctors = 8,
                OnLeaveDoctors = 5,
                AvailableDoctors = 60,
                DoctorsBySpecialization = new Dictionary<string, int>
                {
                    { "InternalMedicine", 15 },
                    { "Pediatrics", 12 },
                    { "FamilyMedicine", 18 },
                    { "Cardiology", 8 },
                    { "Orthopedics", 10 },
                    { "Neurology", 6 },
                    { "ObstetricsGynecology", 10 },
                    { "Dermatology", 6 }
                },
                AverageAppointmentsPerDay = 5.2,
                WeeklyAvailability = new Dictionary<string, int>
                {
                    { "Monday", 58 },
                    { "Tuesday", 62 },
                    { "Wednesday", 60 },
                    { "Thursday", 55 },
                    { "Friday", 50 },
                    { "Saturday", 25 },
                    { "Sunday", 0 }
                }
            };
        }

        public static DoctorStatsDto GetSmallPracticeStats()
        {
            return new DoctorStatsDto
            {
                TotalDoctors = 12,
                ActiveDoctors = 10,
                InactiveDoctors = 1,
                OnLeaveDoctors = 1,
                AvailableDoctors = 8,
                DoctorsBySpecialization = new Dictionary<string, int>
                {
                    { "FamilyMedicine", 4 },
                    { "InternalMedicine", 3 },
                    { "Pediatrics", 2 },
                    { "Cardiology", 1 },
                    { "Dermatology", 2 }
                },
                AverageAppointmentsPerDay = 3.4,
                WeeklyAvailability = new Dictionary<string, int>
                {
                    { "Monday", 8 },
                    { "Tuesday", 9 },
                    { "Wednesday", 8 },
                    { "Thursday", 7 },
                    { "Friday", 6 },
                    { "Saturday", 4 },
                    { "Sunday", 0 }
                }
            };
        }

        public static DoctorStatsDto GetSpecializedCenterStats()
        {
            return new DoctorStatsDto
            {
                TotalDoctors = 45,
                ActiveDoctors = 40,
                InactiveDoctors = 3,
                OnLeaveDoctors = 2,
                AvailableDoctors = 35,
                DoctorsBySpecialization = new Dictionary<string, int>
                {
                    { "Cardiology", 12 },
                    { "VascularSurgery", 5 },
                    { "InterventionalCardiology", 6 },
                    { "Electrophysiology", 4 },
                    { "ThoracicSurgery", 5 },
                    { "CardiacAnesthesiology", 3 },
                    { "CardiacRadiology", 4 },
                    { "HeartFailure", 6 }
                },
                AverageAppointmentsPerDay = 6.1,
                WeeklyAvailability = new Dictionary<string, int>
                {
                    { "Monday", 35 },
                    { "Tuesday", 38 },
                    { "Wednesday", 36 },
                    { "Thursday", 34 },
                    { "Friday", 30 },
                    { "Saturday", 15 },
                    { "Sunday", 5 }
                }
            };
        }

        public static DoctorStatsDto GetBusyHospitalStats()
        {
            return new DoctorStatsDto
            {
                TotalDoctors = 320,
                ActiveDoctors = 280,
                InactiveDoctors = 25,
                OnLeaveDoctors = 15,
                AvailableDoctors = 245,
                DoctorsBySpecialization = new Dictionary<string, int>
                {
                    { "EmergencyMedicine", 45 },
                    { "GeneralSurgery", 35 },
                    { "InternalMedicine", 40 },
                    { "Pediatrics", 25 },
                    { "Orthopedics", 20 },
                    { "Cardiology", 18 },
                    { "Neurology", 12 },
                    { "ObstetricsGynecology", 22 },
                    { "Oncology", 15 },
                    { "Psychiatry", 10 },
                    { "Radiology", 20 },
                    { "Anesthesiology", 18 },
                    { "Pathology", 12 },
                    { "Urology", 8 },
                    { "Ophthalmology", 10 },
                    { "Dermatology", 10 }
                },
                AverageAppointmentsPerDay = 12.5,
                WeeklyAvailability = new Dictionary<string, int>
                {
                    { "Monday", 240 },
                    { "Tuesday", 250 },
                    { "Wednesday", 245 },
                    { "Thursday", 238 },
                    { "Friday", 220 },
                    { "Saturday", 120 },
                    { "Sunday", 50 }
                }
            };
        }

        public static DoctorStatsDto GetRandomStats()
        {
            var totalDoctors = _random.Next(50, 250);
            var activeDoctors = (int)(totalDoctors * (0.70 + _random.NextDouble() * 0.2));
            var onLeaveDoctors = (int)(totalDoctors * (0.05 + _random.NextDouble() * 0.08));
            var inactiveDoctors = totalDoctors - activeDoctors - onLeaveDoctors;
            if (inactiveDoctors < 0) inactiveDoctors = 0;
            
            var availableDoctors = (int)(activeDoctors * (0.70 + _random.NextDouble() * 0.2));
            if (availableDoctors > activeDoctors) availableDoctors = activeDoctors;

            return new DoctorStatsDto
            {
                TotalDoctors = totalDoctors,
                ActiveDoctors = activeDoctors,
                InactiveDoctors = inactiveDoctors,
                OnLeaveDoctors = onLeaveDoctors,
                AvailableDoctors = availableDoctors,
                DoctorsBySpecialization = GenerateRandomSpecializationStats(),
                AverageAppointmentsPerDay = Math.Round(3.0 + _random.NextDouble() * 8.0, 1),
                WeeklyAvailability = GenerateRandomWeeklyAvailability()
            };
        }

        private static Dictionary<string, int> GenerateRandomSpecializationStats()
        {
            var stats = new Dictionary<string, int>();
            var selectedSpecializations = _specializations
                .OrderBy(x => _random.Next())
                .Take(_random.Next(6, 15))
                .ToList();

            var remainingDoctors = _random.Next(50, 200);
            
            foreach (var spec in selectedSpecializations)
            {
                if (remainingDoctors <= 0) break;
                
                int count;
                if (spec == selectedSpecializations.Last())
                {
                    count = remainingDoctors;
                }
                else
                {
                    count = _random.Next(1, Math.Min(remainingDoctors, 35));
                }
                
                stats[spec] = count;
                remainingDoctors -= count;
            }

            return stats;
        }

        private static Dictionary<string, int> GenerateRandomWeeklyAvailability()
        {
            var availability = new Dictionary<string, int>();
            var baseAvailability = _random.Next(30, 120);
            
            foreach (var day in _dayOfWeek)
            {
                if (day == "Sunday")
                {
                    availability[day] = _random.Next(0, 15);
                }
                else if (day == "Saturday")
                {
                    availability[day] = _random.Next(10, baseAvailability / 2);
                }
                else
                {
                    var variation = (int)(baseAvailability * (0.85 + _random.NextDouble() * 0.3));
                    availability[day] = Math.Min(variation, baseAvailability + 25);
                }
            }

            return availability;
        }

        // ==================== HELPER METHODS ====================

        private static string GenerateBio(string specialization)
        {
            var experiences = new[] { "5", "8", "10", "12", "15", "18", "20", "22", "25" };
            var qualities = new[] { "dedicated", "experienced", "compassionate", "skilled", "board-certified", "patient-focused" };
            
            return $"Dr. {GetRandomElement(new[] { "specialist", "expert" })} in {specialization.ToLower()} with {GetRandomElement(experiences)} years of practice. " +
                   $"{char.ToUpper(GetRandomElement(qualities)[0]) + GetRandomElement(qualities).Substring(1)} physician committed to providing exceptional patient care.";
        }

        private static string GenerateQualifications(string specialization)
        {
            var baseQuals = new[] { "MD", "DO", "MBBS" };
            var advancedQuals = new[] { "PhD", "FACC", "FAAP", "FACS", "FACP", "FRCP", "MRCP" };
            
            var selectedBase = GetRandomElements(baseQuals, _random.Next(1, 2));
            var selectedAdvanced = GetRandomElements(advancedQuals, _random.Next(0, 2));
            
            var allQuals = selectedBase.Concat(selectedAdvanced).ToList();
            
            if (_random.NextDouble() > 0.5)
            {
                allQuals.Add($"Board Certified in {specialization}");
            }
            
            return string.Join(", ", allQuals);
        }

        private static T GetRandomElement<T>(T[] array)
        {
            return array[_random.Next(0, array.Length)];
        }

        private static List<T> GetRandomElements<T>(T[] array, int count)
        {
            if (count > array.Length) count = array.Length;
            var shuffled = array.OrderBy(x => _random.Next()).ToList();
            return shuffled.Take(count).ToList();
        }

        // ==================== COMPREHENSIVE DATA SET ====================

        public static DoctorMockDataSet GetAllMockData()
        {
            return new DoctorMockDataSet
            {
                Doctors = GetMockDoctors(25),
                CreateDoctorDtos = GetMockCreateDoctorDtos(5),
                UpdateDoctorDtos = GetMockUpdateDoctorDtos(),
                DoctorSchedules = GenerateDoctorSchedules(1),
                CreateScheduleDtos = GenerateCreateScheduleDtos(1),
                UpdateScheduleDtos = GetMockUpdateScheduleDtos(),
                Availability = GetMockDoctorAvailability(10),
                SearchParameters = GetMockSearchParameters(),
                Stats = GetMockDoctorStats()
            };
        }
    }

    public class DoctorMockDataSet
    {
        public List<DoctorDto> Doctors { get; set; } = new();
        public List<CreateDoctorDto> CreateDoctorDtos { get; set; } = new();
        public List<UpdateDoctorDto> UpdateDoctorDtos { get; set; } = new();
        public List<DoctorScheduleDto> DoctorSchedules { get; set; } = new();
        public List<CreateDoctorScheduleDto> CreateScheduleDtos { get; set; } = new();
        public List<UpdateDoctorScheduleDto> UpdateScheduleDtos { get; set; } = new();
        public List<DoctorAvailabilityDto> Availability { get; set; } = new();
        public List<DoctorSearchDto> SearchParameters { get; set; } = new();
        public List<DoctorStatsDto> Stats { get; set; } = new();
    }
}