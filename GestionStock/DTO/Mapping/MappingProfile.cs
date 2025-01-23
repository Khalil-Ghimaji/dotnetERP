using AutoMapper;
using Persistence.entities.Stock;

namespace GestionStock.DTO.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ArticleStockDTO, Produit>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProduitId))
            .ForMember(dest => dest.Nom, opt => opt.MapFrom(src => src.ProduitNom))
            .ForMember(dest => dest.CategorieId, opt => opt.MapFrom(src => src.CategorieId))
            .ForMember(dest => dest.Categorie, opt => opt.MapFrom(src => new Categorie
            {
                Id = src.CategorieId,
                Nom = src.CategorieNom
            }))
            .ForMember(dest => dest.ArticleStock, opt => opt.MapFrom(src => new ArticleStock
            {
                ProduitId = src.ProduitId,
                Quantite = src.Quantite,
                Prix = src.Prix
            }));

        CreateMap<ArticleStock, ArticleStockDTO>()
            .ForMember(dest => dest.ProduitId, opt => opt.MapFrom(src => src.ProduitId))
            .ForMember(dest => dest.ProduitNom, opt => opt.MapFrom(src => src.Produit.Nom))
            .ForMember(dest => dest.CategorieId, opt => opt.MapFrom(src => src.Produit.CategorieId))
            .ForMember(dest => dest.CategorieNom, opt => opt.MapFrom(src => src.Produit.Categorie.Nom))
            .ForMember(dest => dest.Quantite, opt => opt.MapFrom(src => src.Quantite))
            .ForMember(dest => dest.Prix, opt => opt.MapFrom(src => src.Prix))
            .ForMember(dest=>dest.CategoryDescription,opt=>opt.MapFrom(src=>src.Produit.Categorie.Description));
    }
}