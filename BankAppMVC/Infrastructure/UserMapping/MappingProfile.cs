using AutoMapper;
using BankAppMVC.Models.ViewModels.UsersVm;
using DatabaseLayer.DTOs.Customer;
using DatabaseLayer.DTOs.User;
using DatabaseLayer.Models;
using Microsoft.AspNetCore.Identity;

namespace BankAppMVC.Infrastructure.UserMapping
{
  
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                // IdentityUser ↔ DTOs
                CreateMap<IdentityUser, UserDto>().ForMember(dest => dest.Roles, opt => opt.Ignore());
                CreateMap<IdentityUser, EditUserDto>().ForMember(dest => dest.SelectedRoles, opt => opt.Ignore())
                                                      .ForMember(dest => dest.AllRoles, opt => opt.Ignore());
                CreateMap<EditUserDto, IdentityUser>().ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

                // DTOs ↔ ViewModels
                CreateMap<UserDto, UserViewModel>().ForMember(dest => dest.RoleList, opt => opt.MapFrom(src => src.Roles));
                CreateMap<EditUserDto, EditUserViewModel>().ReverseMap();
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<CustomerDto, CustomerViewModelCrud>().ReverseMap();
        }
        }
    }
