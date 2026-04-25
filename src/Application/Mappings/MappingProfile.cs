using Application.DTOs;
using Application.Helpers.Mappers;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private void ApplyMappingsFromAssembly(Assembly assembly)
    {
        var mapFromType = typeof(IMapFrom<>);
        var mappingMethodName = nameof(IMapFrom<object>.Mapping);

        // Шукаємо всі типи, які реалізують IMapFrom<>
        var types = assembly.GetExportedTypes()
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == mapFromType))
            .ToList();

        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);

            // Шукаємо метод Mapping у самому класі або в інтерфейсі
            var methodInfo = type.GetMethod(mappingMethodName)
                ?? type.GetInterface(mapFromType.Name)?.GetMethod(mappingMethodName);

            methodInfo?.Invoke(instance, new object[] { this });
        }
    }
}
