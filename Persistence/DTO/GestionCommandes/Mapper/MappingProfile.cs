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
        CreateMap<ArticleCommande, ArticleCommandeResponseDTO>()
            .ForMember(dto=>dto.IdProduit, opt=>opt.MapFrom(a=>a.produit.Id))
            .ForMember(dto=>dto.Nom, opt=>opt.MapFrom(a=>a.produit.Nom))
            .ForMember(dto=>dto.Categorie, opt=>opt.MapFrom(a=>a.produit.Categorie.Nom));
        CreateMap<Facture, FactureResponseDTO>()
            .ForMember(dto=>dto.StatusFacture, opt=>opt.MapFrom(f=>f.StatusFacture.ToString()));
        CreateMap<CommandeRequestDTO, Commande>()
            .ForMember(c => c.client, opt => opt.MapFrom(dto=>new Client{Id=dto.ClientId}))
            .ForMember(c => c.status, opt => opt.MapFrom(_ => StatusCommande.PREPARATION))
            .ForMember(c=>c.articles, opt=>opt.MapFrom(_=>new List<ArticleCommande>()))
            .ForMember(c=>c.dateCommande, opt=>opt.MapFrom(dto=>dto.dateCommande??DateTime.Now));
        CreateMap<Client, ClientResponseDTO>();
    }
}