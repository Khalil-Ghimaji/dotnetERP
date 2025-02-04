using AutoMapper;
using Persistence.DTO.Facturation;
using Persistence.entities.Facturation;

namespace Facturation.Mapping
{
    public class FacturationMappingProfile : Profile
    {
        public FacturationMappingProfile()
        {


            CreateMap<UpdateFactureDTO, Facture>()
                .ForMember(dest => dest.MontantTotal, opt => opt.Condition(src => src.MontantTotal.HasValue))
                .ForMember(dest => dest.StatusFacture, opt => opt.Condition(src => src.StatusFacture.HasValue))
                .ForMember(dest => dest.MontantPayé, opt => opt.Condition(src => src.MontantPayé.HasValue));

            CreateMap<CreerEcheanceDTO, Echeance>()
                .ForMember(dest => dest.Montant, opt => opt.MapFrom(src => src.Montant))
                .ForMember(dest => dest.MethodePaiement, opt => opt.MapFrom(src => src.MethodePaiement));

            CreateMap<UpdateEcheanceDTO, Echeance>()
                .ForMember(dest => dest.Montant, opt => opt.Condition(src => src.Montant.HasValue))
                .ForMember(dest => dest.StatutEcheance, opt => opt.Condition(src => src.StatutEcheance.HasValue))
                .ForMember(dest => dest.Date, opt => opt.Condition(src => src.Date.HasValue))
                .ForMember(dest => dest.MethodePaiement, opt => opt.Condition(src => src.MethodePaiement.HasValue));

            CreateMap<Facture, FactureResponseDTO>()
                .ForMember(dest => dest.MontantTotal, opt => opt.MapFrom(src => src.MontantTotal))
                .ForMember(dest => dest.StatusFacture, opt => opt.MapFrom(src => src.StatusFacture.ToString()))
                .ForMember(dest => dest.MontantPayé, opt => opt.MapFrom(src => src.MontantPayé));

            CreateMap<Echeance, EcheanceResponseDTO>()
                .ForMember(dest => dest.Montant, opt => opt.MapFrom(src => src.Montant))
                .ForMember(dest => dest.StatutEcheance, opt => opt.MapFrom(src => src.StatutEcheance.ToString()))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.MethodePaiement, opt => opt.MapFrom(src => src.MethodePaiement));
        }
    }
}
