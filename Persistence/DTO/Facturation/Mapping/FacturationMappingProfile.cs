using AutoMapper;
using Facturation.DTO;
using Persistence.entities.Facturation;

namespace Facturation.Mapping
{
    public class FacturationMappingProfile : Profile
    {
        public FacturationMappingProfile()
        {
            CreateMap<CreerFactureDTO, Facture>();
            CreateMap<UpdateFactureDTO, Facture>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<CreerPaiementDTO, Paiement>();
            CreateMap<UpdatePaiementDTO, Paiement>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}