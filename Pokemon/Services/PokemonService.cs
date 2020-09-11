using Newtonsoft.Json;
using PokeApiNet;
using PokemonConsole.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Type = PokemonConsole.Models.Type;

namespace PokemonConsole.Services
{
    public class PokemonService : IPokemonService
    {
        private readonly string basePokemonServiceUrl = "https://pokeapi.co/api/v2/";
        private PokeApiClient pokeApiClient;

        public PokemonService()
        {
            //TODO Real logging logic in future
            Debug.WriteLine("Running PROD Pokemon Service");          
        }

        public void ClearCache()
        {
            pokeApiClient?.ClearCache();
        }
        private List<PokemonResult> CheckEmptyResult(List<PokemonResult> result)
        {
            PokemonResult pokemonResult = new PokemonResult();            
            pokemonResult.IsSuccessStatusCode = false;
            result.Add(pokemonResult);
            return result;
        }
       
        public async Task<IList<PokemonResult>> GetPokemons(List<string> names)
        {
            List<PokemonResult> result = new List<PokemonResult>();
            if (names.Count == 0)
            {
                return CheckEmptyResult(result);
            }
            foreach (var pokemonName in names.Distinct())
            {
                PokemonResult pokemonResult = new PokemonResult() { IsSuccessStatusCode = false, PokemonName = pokemonName };
                result.Add(pokemonResult);
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(basePokemonServiceUrl);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync(basePokemonServiceUrl + "/pokemon/" + pokemonName);

                        if (response.IsSuccessStatusCode)
                        {
                            var resonseString = await response.Content.ReadAsStringAsync();
                            var results = JsonConvert.DeserializeObject<PokemonResult>(resonseString).Types.Distinct();

                            foreach (var pkType in results)
                            {
                                var type = pkType.type;
                                var pokemonType = new Type();
                                var pokemonType2 = new Type2();
                                pokemonType2.name = type.name;
                                pokemonType2.url = type.url;
                                var response1 = await client.GetAsync(pokemonType2.url);
                                pokemonResult.IsSuccessStatusCode = response1.IsSuccessStatusCode;
                                if (response1.IsSuccessStatusCode)
                                {

                                    var resonseTypeString = await response1.Content.ReadAsStringAsync();
                                    var resultType = JsonConvert.DeserializeObject<Type2>(resonseTypeString);
                                    var doubleDamage = resultType.damage_relations;
                                    var damageRelation = new DamageRelations();
                                    damageRelation.double_damage_to = doubleDamage?.double_damage_to?.Select(x => new DoubleDamageTo() { name = x.name, url = x.url })?.Distinct()?.ToList(); //strong
                                    damageRelation.double_damage_from = doubleDamage?.double_damage_from?.Select(x => new DoubleDamageFrom() { name = x.name, url = x.url })?.Distinct()?.ToList();//vulnerable
                                    damageRelation.half_damage_from = doubleDamage?.half_damage_from?.Select(x => new HalfDamageFrom() { name = x.name, url = x.url })?.Distinct()?.ToList();//resistance
                                    damageRelation.half_damage_to = doubleDamage?.half_damage_to?.Select(x => new HalfDamageTo() { name = x.name, url = x.url })?.Distinct()?.ToList();//weakness
                                    damageRelation.no_damage_from = doubleDamage?.no_damage_from?.Select(x => new NoDamageFrom() { name = x.name, url = x.url })?.Distinct()?.ToList();//no damage
                                    pokemonType2.damage_relations = damageRelation;
                                }
                                pokemonType.type = pokemonType2;
                                if (pokemonResult.Types == null)
                                    pokemonResult.Types = new List<Type>();
                                pokemonResult.Types.Add(pokemonType);
                            }

                        }
                        else
                        {
                            pokemonResult.IsSuccessStatusCode = false;
                        }

                    }
                }
                catch (Exception ex)
                {
                    //todo log the exception using log handler in future;
                    Debug.WriteLine("EXCEPTION : " + ex.Message);
                    pokemonResult.Error = ex;
                    pokemonResult.IsSuccessStatusCode = false;
                }
            }

            return result;
        }

        /// <summary>
        /// This method is using wrapper libraries PokeApiNet by PoroCYon - Alternative Way 
        /// </summary>
        /// <param name="names">list of pokemanname</param>
        /// <returns></returns>
        public async Task<IList<PokemonResult>> GetPokemonsFromLibrary(List<string> names)
        {
            pokeApiClient = new PokeApiClient();
            List<PokemonResult> result = new List<PokemonResult>();
            if (names.Count == 0)
            {
               return CheckEmptyResult(result);
            }
            foreach (var pokemonName in names.Distinct())
            {

                PokemonResult pokemonResult = new PokemonResult();
                PokeApiNet.Pokemon pokemon = null;

                try
                {
                    pokemonResult.PokemonName = pokemonName;
                    pokemon = await pokeApiClient.GetResourceAsync<PokeApiNet.Pokemon>(pokemonName);
                    foreach (var p in pokemon.Types)
                    {
                        var pokemonType = new Type();
                        var pokemonType2 = new Type2();
                        pokemonType2.name = p.Type.Name;
                        pokemonType2.url = p.Type.Url;
                        var type = await pokeApiClient.GetResourceAsync(p.Type);
                        var damageR = type.DamageRelations;
                        var damageRelation = new DamageRelations();
                        damageRelation.double_damage_to = damageR?.DoubleDamageTo?.Select(x => new DoubleDamageTo() { name = x.Name, url = x.Url })?.Distinct()?.ToList(); //strong
                        damageRelation.double_damage_from = damageR?.DoubleDamageFrom?.Select(x => new DoubleDamageFrom() { name = x.Name, url = x.Url })?.Distinct()?.ToList();//vulnerable
                        damageRelation.half_damage_from = damageR?.HalfDamageFrom.Select(x => new HalfDamageFrom() { name = x.Name, url = x.Url })?.Distinct()?.ToList();//resistance
                        damageRelation.half_damage_to = damageR?.HalfDamageTo?.Select(x => new HalfDamageTo() { name = x.Name, url = x.Url })?.Distinct()?.ToList();//weakness
                        damageRelation.no_damage_from = damageR?.NoDamageTo?.Select(x => new NoDamageFrom() { name = x.Name, url = x.Url })?.Distinct()?.ToList();//no damage                      
                        pokemonResult.IsSuccessStatusCode = true;
                        pokemonType2.damage_relations = damageRelation;
                        pokemonType.type = pokemonType2;
                        if (pokemonResult.Types == null)
                            pokemonResult.Types = new List<Type>();
                        pokemonResult.Types.Add(pokemonType);
                    }

                }
                catch (Exception ex)
                {
                    //todo log the exception using log handler in future;
                    Debug.WriteLine("EXCEPTION : " + ex.Message);
                    pokemonResult.Error = ex;
                    pokemonResult.IsSuccessStatusCode = false;
                }
                finally
                {
                    pokemonResult.Pokemon = pokemon;
                    result.Add(pokemonResult);
                }
            }
            return result;
        }


    }
}
