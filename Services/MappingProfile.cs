using AutoMapper;
using HakedisYonetimSistemi.DTOs;
using HakedisYonetimSistemi.Models;

namespace HakedisYonetimSistemi.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Proje Mappings
            CreateMap<Proje, ProjeDto>()
                .ForMember(dest => dest.ToplamMaliyet, opt => opt.MapFrom(src => 
                    src.MaliyetKalemleri.Sum(m => m.ToplamTutar)))
                .ForMember(dest => dest.KalanButce, opt => opt.MapFrom(src => 
                    src.Butce - src.MaliyetKalemleri.Sum(m => m.ToplamTutar)))
                .ForMember(dest => dest.YuzdeHarcanan, opt => opt.MapFrom(src => 
                    src.Butce > 0 ? (src.MaliyetKalemleri.Sum(m => m.ToplamTutar) / src.Butce * 100) : 0));

            CreateMap<CreateProjeDto, Proje>()
                .ForMember(dest => dest.OlusturulmaTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<UpdateProjeDto, Proje>()
                .ForMember(dest => dest.OlusturulmaTarihi, opt => opt.Ignore());

            // MaliyetKalemi Mappings
            CreateMap<MaliyetKalemi, MaliyetKalemiDto>();

            CreateMap<CreateMaliyetKalemiDto, MaliyetKalemi>()
                .ForMember(dest => dest.OlusturulmaTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<UpdateMaliyetKalemiDto, MaliyetKalemi>()
                .ForMember(dest => dest.OlusturulmaTarihi, opt => opt.Ignore());
        }
    }
}
