using AutoMapper;
using Persistence.entities.Commande;
using Persistence.entities.Stock;

namespace GestionStock.DTO.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ArticleStock, ArticleStockDTO>()
            .ForMember(dest => dest.ProduitId, opt => opt.MapFrom(src => src.ProduitId))
            .ForMember(dest => dest.ProduitNom, opt => opt.MapFrom(src => src.Produit.Nom))
            .ForMember(dest => dest.CategorieId, opt => opt.MapFrom(src => src.Produit.CategorieId))
            .ForMember(dest => dest.CategorieNom, opt => opt.MapFrom(src => src.Produit.Categorie.Nom))
            .ForMember(dest => dest.Quantite, opt => opt.MapFrom(src => src.Quantite))
            .ForMember(dest => dest.Prix, opt => opt.MapFrom(src => src.Prix))
            .ForMember(dest=>dest.CategoryDescription,opt=>opt.MapFrom(src=>src.Produit.Categorie.Description));
        CreateMap<ArticleCommande, ArticleExpedierMarchandisesDTO>()
            .ForMember(dest => dest.ProduitId, opt => opt.MapFrom(src => src.produit.Id))
            .ForMember(dest => dest.Quantite, opt => opt.MapFrom(src => src.quantite));
    }
}