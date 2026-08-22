using AutoMapper;
using Doctor.Application.DTOs;
using Doctor.Domain.Entities;
using Doctor.Domain.Enums;
using Doctor.Domain.ValueObjects;

namespace Doctor.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Doctor mappings
            CreateMap<Domain.Entities.Doctor, DoctorDto>()
                .ForMember(dest => dest.SpecializationDisplay,
                    opt => opt.MapFrom(src => src.Specialization.GetDisplayName()))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.ContactInfo.Email))
                .ForMember(dest => dest.PhoneNumber,
                    opt => opt.MapFrom(src => src.ContactInfo.PhoneNumber))
                .ForMember(dest => dest.MobileNumber,
                    opt => opt.MapFrom(src => src.ContactInfo.MobileNumber))
                .ForMember(dest => dest.Address,
                    opt => opt.MapFrom(src => src.ContactInfo.Address))
                .ForMember(dest => dest.Schedules,
                    opt => opt.MapFrom(src => src.Schedules))
                .ReverseMap();

            CreateMap<CreateDoctorDto, Domain.Entities.Doctor>()
                .ConstructUsing((src, ctx) => new Domain.Entities.Doctor(
                    src.FirstName,
                    src.LastName,
                    Enum.Parse<Specialization>(src.Specialization, true),
                    new ContactInfo(src.Email, src.PhoneNumber, src.MobileNumber, src.Address),
                    src.Bio,
                    src.Qualifications,
                    src.LicenseNumber,
                    src.Department,
                    src.UserId
                ));

            CreateMap<UpdateDoctorDto, Domain.Entities.Doctor>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Schedule mappings
            CreateMap<DoctorSchedule, DoctorScheduleDto>()
                .ForMember(dest => dest.DayOfWeek,
                    opt => opt.MapFrom(src => src.DayOfWeek.ToString()))
                .ForMember(dest => dest.StartTime,
                    opt => opt.MapFrom(src => src.WorkingHours.StartTime))
                .ForMember(dest => dest.EndTime,
                    opt => opt.MapFrom(src => src.WorkingHours.EndTime))
                .ReverseMap();

            CreateMap<CreateDoctorScheduleDto, DoctorSchedule>()
                .ConstructUsing((src, ctx) => {
                    if (!Enum.TryParse<DayOfWeek>(src.DayOfWeek, true, out var dayOfWeek))
                        throw new InvalidOperationException($"Invalid day of week: {src.DayOfWeek}");

                    var workingHours = new WorkingHours(src.StartTime, src.EndTime);
                    return new DoctorSchedule(
                        src.DoctorId,
                        dayOfWeek,
                        workingHours,
                        src.SlotDurationInMinutes,
                        src.ValidFrom,
                        src.ValidTo
                    );
                });

            CreateMap<UpdateDoctorScheduleDto, DoctorSchedule>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
