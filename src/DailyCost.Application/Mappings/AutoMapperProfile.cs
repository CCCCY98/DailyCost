using AutoMapper;
using DailyCost.Application.DTOs.Category;
using DailyCost.Application.DTOs.Expense;
using DailyCost.Application.DTOs.User;
using DailyCost.Domain.Entities;

namespace DailyCost.Application.Mappings;

public sealed class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<Category, CategoryDto>();
        CreateMap<Category, CategoryBriefDto>();
    }
}

