using System.Linq;
using AutoMapper;
using ProAgil.API.Dtos;
using ProAgil.Domain;

namespace ProAgil.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Evento, EventoDTO>()
                .ForMember(dest => dest.Palestrantes, opt => {
                    opt.MapFrom(src => src.PalestrantesEventos.Select(x => x.Palestrante).ToList());
                }).ReverseMap();;

            CreateMap<Palestrante, PalestranteDTO>()
            .ForMember(dest => dest.Eventos,  opt => {
                opt.MapFrom(src => src.PalestrantesEventos.Select(x => x.Evento).ToList());
            });

            CreateMap<Lote, LoteDTO>().ReverseMap();
            
            CreateMap<RedeSocial, RedeSocialDTO>().ReverseMap();
        }
    }
}