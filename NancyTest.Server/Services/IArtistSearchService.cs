using NancyTest.Server.Models;

namespace NancyTest.Server.Services
{
    public interface IArtistSearchService
    {
        ArtistSearchModel Serach(string artistName);
    }
}