using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using TestAnimalAPI;
using AnimalsAPI.Models;

namespace TestAnimalsAPI
{
    public class TestAnimals
    {
        [Fact]
        public async Task Test_Get_All_Animals()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/animals");


                string responseBody = await response.Content.ReadAsStringAsync();
                var animalList = JsonConvert.DeserializeObject<List<Animal>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(animalList.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Single_Animal()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                /*
                    ACT
                */
                var response = await client.GetAsync("/api/animals/2");


                string responseBody = await response.Content.ReadAsStringAsync();
                var animal = JsonConvert.DeserializeObject<Animal>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Coconut", animal.Name);
                Assert.Equal("Northwest African Cheetah", animal.Species);
                Assert.Equal("Carnivore", animal.EatingHabit);
                Assert.Equal(4, animal.Legs);
                Assert.Equal(2, animal.ZooId);
                Assert.NotNull(animal);
            }
        }

        [Fact]
        public async Task Test_Get_NonExitant_Animal_Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                /*
                    ACT
                */
                var response = await client.GetAsync("/api/animals/999999999");

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }


        [Fact]
        public async Task Test_Create_And_Delete_Animal()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                Animal Berry = new Animal
                {
                    Name = "Berry Blue",
                    Species = "Pygmy Three-toed Sloth",
                    EatingHabit = "Omnivore",
                    Legs = 4,
                    ZooId = 1
                };
                var BerryAsJSON = JsonConvert.SerializeObject(Berry);

                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/animals",
                    new StringContent(BerryAsJSON, Encoding.UTF8, "application/json")
                );


                string responseBody = await response.Content.ReadAsStringAsync();
                var NewBerry = JsonConvert.DeserializeObject<Animal>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(Berry.Name, NewBerry.Name);
                Assert.Equal(Berry.Species, NewBerry.Species);
                Assert.Equal(Berry.EatingHabit, NewBerry.EatingHabit);
                Assert.Equal(Berry.Legs, NewBerry.Legs);
                Assert.Equal(Berry.ZooId, NewBerry.ZooId);

                /*
                    ACT
                */
                var deleteResponse = await client.DeleteAsync($"/api/animals/{NewBerry.Id}");

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_NonExistent_Animal_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                /*
                    ACT
                */
                var deleteResponse = await client.DeleteAsync("/api/animals/600000");

                /*
                    ASSERT
                */
                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Modify_Animal()
        {
            // New eating habit value to change to and test
            string NewEatingHabit = "Herbivore";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                 */
                Animal ModifiedButter = new Animal
                {
                    Name = "Butter",
                    Species = "Northwest African Cheetah",
                    EatingHabit = NewEatingHabit,
                    Legs = 4,
                    ZooId = 2
                };
                var ModifiedButterAsJSON = JsonConvert.SerializeObject(ModifiedButter);

                var response = await client.PutAsync(
                    "/api/animals/1",
                    new StringContent(ModifiedButterAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                var GetButter = await client.GetAsync("/api/animals/1");
                GetButter.EnsureSuccessStatusCode();

                string GetButterBody = await GetButter.Content.ReadAsStringAsync();
                Animal NewButter = JsonConvert.DeserializeObject<Animal>(GetButterBody);

                Assert.Equal(HttpStatusCode.OK, GetButter.StatusCode);
                Assert.Equal(NewEatingHabit, NewButter.EatingHabit);
            }
        }
    }
}
