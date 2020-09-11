using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using PokeApiNet;
using PokemonConsole.Models;

namespace PokemonConsole.Services
{
    public class FakePokemonService : IPokemonService
    {
        public FakePokemonService()
        {
            Debug.WriteLine("Running Fake Pokemon Service");
        }

        public void ClearCache()
        {
            throw new NotImplementedException();
        }

        public async Task<IList<PokemonResult>> GetPokemons(List<string> names)
        {
            List<PokemonResult> fakePokemons = new List<PokemonResult>();

            foreach (var name in names)
            {
                var fakePoke = new PokemonResult()
                {
                    Error = null,
                    Types = new List<Models.Type>()
                    {
                        new Models.Type(){ type= new Type2(){
                            name="normal",
                            damage_relations = new DamageRelations()
                            {
                                double_damage_from = new List<DoubleDamageFrom>()
                                {
                                    new DoubleDamageFrom(){name="snake",url=""},
                                     new DoubleDamageFrom(){name="fire",url=""},
                                },
                                double_damage_to = new List<DoubleDamageTo>()
                                {
                                    new DoubleDamageTo(){name="speed",url=""},
                                     new DoubleDamageTo(){name="smile",url=""},
                                },
                                    half_damage_from = new List<HalfDamageFrom>()
                                {
                                    new HalfDamageFrom(){name="rain",url=""},
                                     new HalfDamageFrom(){name="slow",url=""},
                                },
                                half_damage_to = new List<HalfDamageTo>()
                                {
                                    new HalfDamageTo(){name="water",url=""},
                                     new HalfDamageTo(){name="air",url=""},
                                },
                                no_damage_from = new List<NoDamageFrom>()
                                {
                                    new NoDamageFrom(){name="smile",url=""},
                                     new NoDamageFrom(){name="speed",url=""},
                                },
                            }
                        } }
                    }
                };
                fakePokemons.Add(fakePoke);
            }        

            return await Task.FromResult(fakePokemons);
        }

        public Task<IList<PokemonResult>> GetPokemonsFromLibrary(List<string> names)
        {
            throw new NotImplementedException();
        }
    }
}
