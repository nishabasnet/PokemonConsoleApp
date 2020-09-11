using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using PokemonConsole.Models;
using PokemonConsole.Services;
using NUnit.Framework;


namespace PokemonUnitTest
{
    [TestFixture]
    public class PokemonUnitTest
    {

        private IPokemonService _pokemonService;
       
        [SetUp]
        public void SetUp()
        {
            _pokemonService = new PokemonService();
            //TODO check environment setting here as well
        }
      
        private static readonly object[] _data =
        {
        new object[] {new List<string> {"ditto"}, true},   //case 1
        new object[] {new List<string> {"1"}, true}, //case 2
        new object[] {new List<string> {" "}, false},//case 3
        new object[] {new List<string> {"ditto","1"}, true}, //case 4
        new object[] {new List<string> {"ditto","Nisha"}, false} ,//case 5
        new object[] {new List<string> {"ditto"," "}, false} //case 6
        };
        private static readonly object[] _duplicateCheck =
       {
        new object[] {new List<string> {"ditto", "ditto"}, 1},   //case 1
        new object[] {new List<string> {"1", "ditto", "1"}, 2}, //case 2
        new object[] {new List<string> {" ", " ", "1"}, 2}, //case 3
         new object[] {new List<string> {"test", "test", "1"}, 2}, //case 4
       
        };

        [Test, TestCaseSource(nameof(_data))]
        public void IsValid_APICall_ReturnsExpectedResult(List<string> pokemonName, bool expectedResult)
        {
            //Arrange
            //pokeName Test Data

            //Act
            var responseResult = _pokemonService.GetPokemons(pokemonName).Result;

            //Assert

             Assert.AreEqual(expectedResult, !(responseResult.Where(r => r.IsSuccessStatusCode == false || r.Error != null).Any()));                 
        }

        [Test, TestCaseSource(nameof(_duplicateCheck))]
        public void Duplicate_Check_Test(List<string> pokemonName, int expectedResult)
        {
            //Arrange

            //pokeName Test Data

            //Act
            var responseResult = _pokemonService.GetPokemons(pokemonName).Result;
            if(expectedResult == responseResult.Count)
            {

            }
            //Assert
            Assert.AreEqual(expectedResult, responseResult.Count());
        }


    }
}
