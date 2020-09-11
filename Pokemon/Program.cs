using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Pokemon.Extensions;
using PokemonConsole.Services;

namespace PokemonConsole
{
    internal enum Environment
    {
        QA,
        PROD
    }

    class Program
    {
        private static Environment Environment = Environment.QA;

        private static List<string> PokemonQuery = new List<string>();
        static int tableWidth = 100;

        //In future we can use Container to register all the depenendencies and use.
        private static ServiceProvider Services;

        static void Main(string[] args)
        {
            Console.Clear();

            //Set Development Enviroment
            SetDevelopmentMachine();

            //Register all dependency services
            RegisterDependencies();

            //Print Header
            PrintHeader();

            //Ask for Names:
            AskNames();

            //Query for Results:
            //QueryResultsUsingPOkeAPI();
            QueryResults();

        }

        #region DevelopmentMachine
        private static void SetDevelopmentMachine()
        {

#if DEBUG
            Environment = Environment.PROD; // for testing purpose added env as PROD good to have QA on debug
#else
            Environment = Environment.PROD;
#endif
        }
        #endregion

        #region RegisterDependcies
        private static void RegisterDependencies()
        {
            var services = new ServiceCollection();

            if (Environment == Environment.PROD)
            {
                //Register all the PROD services below:
                services.AddSingleton<IPokemonService, PokemonService>();
            }
            else
            {
                //Register all the FAKE services below:
                services.AddSingleton<IPokemonService, FakePokemonService>();
            }

            Services = services.BuildServiceProvider();
        }
        #endregion

        #region Print Header
        private static void PrintHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("******************** POKEMON CONSOLE ********************");
            sb.AppendLine("         Please Enter the Name(s) of the Pokemons");
            sb.AppendLine("             Press 'Enter' for multiple names");
            sb.AppendLine("               Press 'Enter' again for result");

            Console.WriteLine(sb.ToString());
        }
        #endregion

        #region Ask Names
        private static void AskNames()
        {
            string pokemonName = null;
            do
            {
                pokemonName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(pokemonName))
                {
                    PokemonQuery.Add(pokemonName);
                }
            }
            while (!string.IsNullOrWhiteSpace(pokemonName));
        }
        #endregion

        #region Query Result
     
        private static void QueryResults()
        {
            try
            {

                StringBuilder sb = new StringBuilder();
                var pokemonService = Services.GetService<IPokemonService>();
                //can be called GetPokemonsFromLibrary(..) too. The library has caching 
                var pokemonResult = pokemonService.GetPokemons(PokemonQuery).Result;
                //Result Print:
                sb.AppendLine("Here's the Pokemons you requested for - ");
                int indx = 1;
                foreach (var pokemon in pokemonResult.Where(r => r.Error == null && r.IsSuccessStatusCode == true).GroupBy(x => x.PokemonName))
                {
                    sb.AppendLine();
                    sb.AppendLine($"{indx}. Here is the detail list of pokemon effectiveness for Pokemon : {pokemon.Key} ");
                    sb.AppendLine();
                    foreach (var item in pokemon)
                    {
                        PrintLine(sb);
                        foreach (var type in item.Types)
                        {
                            sb.AppendLine();
                            PrintLineStar(sb);
                            PrintRow(sb, "Type", type.type.name);
                            PrintLineStar(sb);
                            PrintRow(sb, "Strong Against", type.type.damage_relations.double_damage_to.Select(x => x.name).ToList().ToCSV());
                            PrintLine(sb);
                            PrintRow(sb, "Weak Against", type.type.damage_relations.half_damage_to.Select(x => x.name).ToList().ToCSV());
                            PrintLine(sb);
                            PrintRow(sb, "Resistant To", type.type.damage_relations.half_damage_from.Select(x => x.name).ToList().ToCSV());
                            PrintLine(sb);
                            PrintRow(sb, "Vulnerable To", type.type.damage_relations.double_damage_from.Select(x => x.name).ToList().ToCSV());
                            PrintLine(sb);
                            PrintRow(sb, "No damage From", type.type.damage_relations.no_damage_from.Select(x => x.name).ToList().ToCSV());
                            PrintLine(sb);
                            sb.AppendLine();
                        }

                    }
                    indx++;
                }
                sb.AppendLine();
                foreach (var pokemon in pokemonResult.Where(r => r.Error != null || r.IsSuccessStatusCode == false).GroupBy(x => x.PokemonName))
                {
                    if(string.IsNullOrEmpty(pokemon.Key))
                    {
                        sb.AppendLine($"{indx}. Null or Empty Pokemon Name Entered.");
                    }
                    else
                    {
                        sb.AppendLine($"{indx}. {pokemon.Key} not found : ");
                    }
                  
                    sb.AppendLine("-------------------------------");
                    foreach (var item in pokemon)
                    {
                        if(item.Error != null)
                        {
                            sb.AppendLine(item.Error.Message);
                            sb.AppendLine();
                        }
                     
                    }
                    indx++;
                }

                Console.WriteLine(sb.ToString());

            }
            finally
            {
                Console.WriteLine("Press enter to close...");
                Console.ReadLine();
            }

        }
        #endregion

        #region Formatting
        private static void PrintLine(StringBuilder sb)
        {
            sb.AppendLine(new string('-', tableWidth));
        }
        private static void PrintLineStar(StringBuilder sb)
        {
            sb.AppendLine(new string('*', tableWidth));
        }
        private static void PrintRow(StringBuilder sb, params string[] columns)
        {
            int width = (tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }
            sb.AppendLine(row);
            // Console.WriteLine(row);
        }

        private static string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }
        #endregion
               
    }
}
