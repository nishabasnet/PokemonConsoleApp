using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PokemonConsole.Models;

namespace PokemonConsole.Services
{
    public interface IPokemonService
    {
        Task<IList<PokemonResult>> GetPokemons(List<string> names);
        Task<IList<PokemonResult>> GetPokemonsFromLibrary(List<string> names);
        void ClearCache();
    }
}
