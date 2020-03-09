using AutoMapper;
using mservicesample.Membership.Core.DataAccess.Entities;
using mservicesample.Membership.Core.Dtos.Requests;

namespace mservicesample.Membership.Core.Dtos.Mapping
{
    public class DataMapping : Profile
    {
        public DataMapping()
        {
            CreateMap<UserDetails, AppUser>().ConstructUsing(u => new AppUser { UserName = u.UserName, Email = u.Email }).ForMember(au => au.Id, opt => opt.Ignore());

            CreateMap<AppUser, UserDetails>().ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email)).
                ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash)).ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<UserDetails, UserDto>().ConstructUsing(u => new UserDto { UserName = u.UserName, Email = u.Email, IdentityId = u.IdentityId, FirstName = u.FirstName, LastName = u.LastName }).ForMember(au => au.PasswordHash, opt => opt.Ignore());

            CreateMap<UserDto, UserDetails>().ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email)).
                ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password)).ForAllOtherMembers(opt => opt.Ignore());


            CreateMap<RolesDto, AppRole>().ConstructUsing(x => new AppRole { Name = x.Name, Id = x.Id, ConcurrencyStamp = x.ConcurrencyStamp });
            CreateMap<AppRole, RolesDto>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name)).
                ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.MapFrom(src => src.ConcurrencyStamp))
                .ForAllOtherMembers(opt => opt.Ignore());

        }
    }
}
