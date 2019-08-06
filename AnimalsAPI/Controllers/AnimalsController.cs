using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AnimalsAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AnimalsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalsController : ControllerBase
    {

        private readonly IConfiguration _config;

        public AnimalsController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }


        // GET: api/Animals
        [HttpGet]
        public async Task<IActionResult> Get(int? zooId, string eatingHabit)
        {
            string SqlCommandText = @"SELECT a.Id, a.[Name], a.Species, a.EatingHabit, a.Legs, a.ZooId
                    FROM Animals a
                    WHERE 1 = 1";

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (zooId != null)
                    {
                        SqlCommandText = $"{SqlCommandText} AND a.ZooId = @zooId";
                        cmd.Parameters.Add(new SqlParameter("@zooId", zooId));
                    }

                    if (eatingHabit != null)
                    {
                        SqlCommandText = $"{SqlCommandText} AND a.EatingHabit = @eatingHabit";
                        cmd.Parameters.Add(new SqlParameter("@eatingHabit", eatingHabit));
                    }

                    cmd.CommandText = SqlCommandText;
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<Animal> animals = new List<Animal>();

                    while (reader.Read())
                    {
                        Animal animal = new Animal
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Species = reader.GetString(reader.GetOrdinal("Species")),
                            EatingHabit = reader.GetString(reader.GetOrdinal("EatingHabit")),
                            Legs = reader.GetInt32(reader.GetOrdinal("Legs")),
                            ZooId = reader.GetInt32(reader.GetOrdinal("ZooId"))
                        };

                        animals.Add(animal);
                    }
                    reader.Close();

                    return Ok(animals);
                }
            }
        }

        // GET: api/Animals/5
        [HttpGet("{id}", Name = "GetAnimal")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (!AnimalExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT
                            Id, [Name], Species, EatingHabit, Legs, ZooId 
                        FROM Animals
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Animal animal = null;

                    if (reader.Read())
                    {
                        animal = new Animal
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Species = reader.GetString(reader.GetOrdinal("Species")),
                            EatingHabit = reader.GetString(reader.GetOrdinal("EatingHabit")),
                            Legs = reader.GetInt32(reader.GetOrdinal("Legs")),
                            ZooId = reader.GetInt32(reader.GetOrdinal("ZooId"))

                        };
                    }
                    reader.Close();

                    return Ok(animal);
                }
            }
        }

        // POST: api/Animals
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Animal animal)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Animals ([Name], Species, EatingHabit, Legs, ZooId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@name, @species, @eatingHabit, @legs, @zooId)";
                    cmd.Parameters.Add(new SqlParameter("@name", animal.Name));
                    cmd.Parameters.Add(new SqlParameter("@species", animal.Species));
                    cmd.Parameters.Add(new SqlParameter("@eatingHabit", animal.EatingHabit));
                    cmd.Parameters.Add(new SqlParameter("@legs", animal.Legs));
                    cmd.Parameters.Add(new SqlParameter("@zooId", animal.ZooId));

                    int newId = (int)await cmd.ExecuteScalarAsync();
                    animal.Id = newId;
                    return CreatedAtRoute("GetAnimal", new { id = newId }, animal);
                }
            }
        }

        // PUT: api/Animals/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Animal animal)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Animals
                                            SET [Name] = @name,
                                                Species = @species,
                                                EatingHabit = @eatingHabit,
                                                Legs = @legs,
                                                ZooId = @zooId
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@name", animal.Name));
                        cmd.Parameters.Add(new SqlParameter("@species", animal.Species));
                        cmd.Parameters.Add(new SqlParameter("@eatingHabit", animal.EatingHabit));
                        cmd.Parameters.Add(new SqlParameter("@legs", animal.Legs));
                        cmd.Parameters.Add(new SqlParameter("@zooId", animal.ZooId));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!AnimalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Animals WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!AnimalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool AnimalExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name], Species, EatingHabit, Legs
                        FROM Animals
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}
