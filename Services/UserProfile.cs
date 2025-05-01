using AutoMapper;
using Entities.Dtos;
using Entities.Models;

namespace Services
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // Maps create dtos to model

            CreateMap<UserCreateDto, User>();
            CreateMap<AddressCreateDto, Address>();
            CreateMap<PaymentCreateDto, Payment>();


            // Maps model to response dtos

            CreateMap<User, UserAccountResponseDto>()
            .ForMember(dest => dest.Addresses, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Payments, opt => opt.MapFrom(src => src.Payment)); 

            CreateMap<User, UserAccountOnlyResponseDto>();
            CreateMap<Address, AddressResponseDto>();
            CreateMap<Payment, PaymentResponseDto>();
        }
    }
}
