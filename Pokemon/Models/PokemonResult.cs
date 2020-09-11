using System;
using System.Collections.Generic;
using PokeApiNet;

namespace PokemonConsole.Models
{
    public class PokemonResult
    {
        public PokeApiNet.Pokemon Pokemon { get; set; }
        public Exception Error { get; set; }
        public string StatName { get; set; }
        public int BaseStat { get; set; }
        public string PokemonName { get; set; }
        public bool IsSuccessStatusCode { get; set; }
        public List<Type> Types { get; set; }

    }

    public class Type2
    {
        public string name { get; set; }
        public string url { get; set; }
        public DamageRelations damage_relations { get; set; }
    }

    public class Type
    {
        public int slot { get; set; }
        public Type2 type { get; set; }

    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class DoubleDamageFrom
    {
        public string name { get; set; }
        public string url { get; set; }
    }

    public class DoubleDamageTo
    {
        public string name { get; set; }
        public string url { get; set; }
    }

    public class HalfDamageFrom
    {
        public string name { get; set; }
        public string url { get; set; }
    }

    public class HalfDamageTo
    {
        public string name { get; set; }
        public string url { get; set; }
    }

    public class NoDamageFrom
    {
        public string name { get; set; }
        public string url { get; set; }
    }

    public class DamageRelations
    {
       
        public List<DoubleDamageFrom> double_damage_from { get; set; }
        public List<DoubleDamageTo> double_damage_to { get; set; }
        public List<HalfDamageFrom> half_damage_from { get; set; }
        public List<HalfDamageTo> half_damage_to { get; set; }
        public List<NoDamageFrom> no_damage_from { get; set; }

    }


}
