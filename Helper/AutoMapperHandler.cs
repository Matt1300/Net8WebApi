using AutoMapper;
using LearnAPI.Modal;
using LearnAPI.Repos.Models;

namespace LearnAPI.Helper
{
    public class AutoMapperHandler : Profile
    {
        public AutoMapperHandler() 
        {
            CreateMap<TblCustomer, CustomerModal>().ForMember(item => item.Statusname, opt => opt.MapFrom(
                item => item.IsActive ?? false ? "Active" : "Inactive")).ReverseMap();

        }
    }
}
