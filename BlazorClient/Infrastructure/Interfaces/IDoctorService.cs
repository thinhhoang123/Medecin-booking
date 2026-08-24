using BlazorClient.Features.Doctors;

namespace BlazorClient.Infrastructure.Interfaces;

public interface IDoctorService
{
    Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync();
}