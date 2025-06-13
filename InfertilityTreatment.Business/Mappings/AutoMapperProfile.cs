using AutoMapper;
using InfertilityTreatment.Entity.Entities;
using InfertilityTreatment.Entity.DTOs.Auth;
using InfertilityTreatment.Entity.DTOs.Users;

namespace InfertilityTreatment.Business.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User mappings
            CreateMap<User, UserProfileDto>();
            CreateMap<RegisterRequestDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Customer mappings
            CreateMap<Customer, CustomerDetailDto>()
               .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User)).ReverseMap();
            
            CreateMap<RegisterRequestDto, Customer>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Response mappings
            CreateMap<User, LoginResponseDto>()
                .ForMember(dest => dest.AccessToken, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
                .ForMember(dest => dest.ExpiresAt, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src));
            // Map UpdateProfile to User entity
            CreateMap<UpdateProfileDto, User>().ReverseMap();
        }
    }
}
