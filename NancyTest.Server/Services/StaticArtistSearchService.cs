using System.Collections.Generic;
using NancyTest.Server.Models;

namespace NancyTest.Server.Services
{
    public class StaticArtistSearchService : IArtistSearchService
    {
        public ArtistSearchModel Serach(string artistName)
        {
            return new ArtistSearchModel
            {
                Artists = new List<Artist>
                {
                    new Artist
                    {
                        Name = "Queen",
                        BannerImgUri = "~/img/queen.jpg"
                    },
                    new Artist
                    {
                        Name = "Yes",
                        BannerImgUri = "~/img/yes.jpg"
                    }
                }
            };
        }
    }
}