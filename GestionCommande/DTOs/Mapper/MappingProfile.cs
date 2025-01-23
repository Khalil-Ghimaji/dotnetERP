using AutoMapper;
using Persistence.entities.Client;
using Persistence.entities.Facturation;
using Persistence.entities.Stock;

namespace GestionCommande.DTOs.Mapper;
using Persistence.entities.Commande;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Commande, CommandeResponseDTO>()
            .ForMember(dto=>dto.status, opt=>opt.MapFrom(c=>c.status.ToString()));
        CreateMap<ArticleCommande, ArticleCommandeResponseDTO>();
        CreateMap<Facture, FactureResponseDTO>()
            .ForMember(dto=>dto.StatusFacture, opt=>opt.MapFrom(f=>f.StatusFacture.ToString()));
        CreateMap<Produit, ProduitResponseDTO>()
            .ForMember(dto=>dto.CategorieNom, opt=>opt.MapFrom(p=>p.Categorie.Nom));
        CreateMap<CommandeRequestDTO, Commande>()
            .ForMember(c => c.client, opt => opt.MapFrom(dto=>new Client{Id=dto.ClientId}))
            .ForMember(c => c.status, opt => opt.MapFrom(_ => StatusCommande.PREPARATION))
            .ForMember(c=>c.articles, opt=>opt.MapFrom(_=>new List<ArticleCommande>()))
            .ForMember(c=>c.dateCommande, opt=>opt.MapFrom(_=>DateTime.Now));
    }
}