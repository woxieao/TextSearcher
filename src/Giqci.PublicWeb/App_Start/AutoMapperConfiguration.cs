using AutoMapper;
using Giqci.Mappings;

namespace Giqci.PublicWeb
{
    public static class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<GiqciMapperProfile>();
            });
        }
    }
}