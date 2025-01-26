using AutoMapper;
using Persistence.DTO.Facturation;
using Persistence.entities.Facturation;

namespace Facturation.Mapping
{
    public class FacturationMappingProfile : Profile
    {
        public FacturationMappingProfile()
        {
            CreateMap<CreerFactureDTO, Facture>();
            CreateMap<UpdateFactureDTO, Facture>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<CreerEcheanceDTO, Echeance>();
            CreateMap<UpdateEcheanceDTO, Echeance>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Facture, FactureResponseDTO>();
            CreateMap<Echeance, EcheanceResponseDTO>();
        }
    }
}