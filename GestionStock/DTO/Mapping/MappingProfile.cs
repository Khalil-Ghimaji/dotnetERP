using AutoMapper;
using Persistence.entities.Stock;

namespace GestionStock.DTO.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ArticleStock, ArticleStockDTO>().ReverseMap();
        //CreateMap<Produit, ProduitDTO>().ReverseMap();
        //CreateMap<Produit, ReserverProduitDTO>().ReverseMap();
    }
}