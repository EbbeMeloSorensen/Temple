using AutoMapper;
using Temple.Domain.Entities.PR;
using Temple.Domain.Entities.Smurfs;
using Temple.Application.People;
using Temple.Application.Smurfs;

namespace Temple.Application.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Smurf, SmurfDto>();

            CreateMap<Person, Person>();
            CreateMap<Person, PersonDto>();
        }
    }
}