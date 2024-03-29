﻿using System;
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
    public class ZoosController : ControllerBase
    {

        private readonly IConfiguration _config;

        public ZoosController(IConfiguration config)
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


        // GET: api/Zoos
        [HttpGet]
        public IActionResult Get(string q)
        {

            string SqlCommandText = @"
                        SELECT z.Id as ZooId, z.[Name] as ZooName, z.Address, z.Acres,
                        a.Id as AnimalId, a.[Name] as AnimalName, a.Species, a.EatingHabit, a.Legs, a.ZooId
                        FROM Zoos z
                        JOIN Animals a ON z.Id = a.ZooId";

            if (q != null)
            {
                SqlCommandText = $@"{SqlCommandText} WHERE (
                    z.[Name] LIKE @q
                    OR z.Address LIKE @q
                    OR z.Acres LIKE @q
                    )
                    ";

            }

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = SqlCommandText;

                    if (q != null)
                    {
                        cmd.Parameters.Add(new SqlParameter("@q", $"%{q}%"));

                    }

                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Zoo> zoos = new List<Zoo>();

                    while (reader.Read())
                    {
                        Zoo zoo = new Zoo
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("ZooId")),
                            Name = reader.GetString(reader.GetOrdinal("ZooName")),
                            Address = reader.GetString(reader.GetOrdinal("Address")),
                            Acres = (float)reader.GetDecimal(reader.GetOrdinal("Acres"))

                        };

                        Animal animal = new Animal
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("AnimalId")),
                            Name = reader.GetString(reader.GetOrdinal("AnimalName")),
                            Species = reader.GetString(reader.GetOrdinal("Species")),
                            EatingHabit = reader.GetString(reader.GetOrdinal("EatingHabit")),
                            Legs = reader.GetInt32(reader.GetOrdinal("Legs")),
                            ZooId = reader.GetInt32(reader.GetOrdinal("ZooId"))
                        };

                        if (zoos.Any(z => z.Id == zoo.Id))
                        {
                            Zoo ExistingZoo = zoos.Find(z => z.Id == zoo.Id);
                            ExistingZoo.Animals.Add(animal);
                        }
                        else
                        {
                            zoo.Animals.Add(animal);
                            zoos.Add(zoo);
                        }
                    }
                    reader.Close();

                    return Ok(zoos);
                }
            }
        }

        // GET: api/Zoos/5
        [HttpGet("{id}", Name = "GetZoo")]
        public IActionResult Get([FromRoute] int id, string include)
        {
            if (!ZooExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }

            string SqlCommandText;

            if (include == "animals")
            {
                SqlCommandText = @"
                SELECT z.Id as ZooId, z.[Name] as ZooName, z.Address, z.Acres,
                a.Id as AnimalId, a.[Name] as AnimalName, a.Species, a.EatingHabit, a.Legs, a.ZooId
                FROM Zoos z
                JOIN Animals a ON z.Id = a.ZooId";
            }
            else
            {
                SqlCommandText = @"
                SELECT z.Id as ZooId, z.[Name] as ZooName, z.Address, z.Acres
                FROM Zoos z";
            }

            

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"{SqlCommandText} WHERE z.id = @id"; ;
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Zoo zoo = null;

                    while (reader.Read())
                    {
                        if (zoo == null)
                        {
                            zoo = new Zoo
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ZooId")),
                                Name = reader.GetString(reader.GetOrdinal("ZooName")),
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                Acres = (float)reader.GetDecimal(reader.GetOrdinal("Acres"))

                            };
                        }

                        if (include == "animals")
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("AnimalId")))
                            {
                                zoo.Animals.Add(
                                    new Animal
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("AnimalId")),
                                        Name = reader.GetString(reader.GetOrdinal("AnimalName")),
                                        Species = reader.GetString(reader.GetOrdinal("Species")),
                                        EatingHabit = reader.GetString(reader.GetOrdinal("EatingHabit")),
                                        Legs = reader.GetInt32(reader.GetOrdinal("Legs")),
                                        ZooId = reader.GetInt32(reader.GetOrdinal("ZooId"))
                                    }
                                );
                            }
                        }

                    }
                    reader.Close();

                    return Ok(zoo);
                }
            }
        }

        // POST: api/Zoos
        [HttpPost]
        public IActionResult Post([FromBody] Zoo zoo)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Zoos ([Name], Address, Acres)
                                        OUTPUT INSERTED.Id
                                        VALUES (@name, @address, @acres)";
                    cmd.Parameters.Add(new SqlParameter("@name", zoo.Name));
                    cmd.Parameters.Add(new SqlParameter("@address", zoo.Address));
                    cmd.Parameters.Add(new SqlParameter("@acres", zoo.Acres));

                    int newId = (int)cmd.ExecuteScalar();
                    zoo.Id = newId;
                    return CreatedAtRoute("GetZoo", new { id = newId }, zoo);
                }
            }
        }

        // PUT: api/Zoos/5
        [HttpPut("{id}")]
        public IActionResult Put([FromRoute] int id, [FromBody] Zoo zoo)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Zoos
                                            SET [Name] = @name,
                                                Address = @address
                                                Acres = @acres
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@name", zoo.Name));
                        cmd.Parameters.Add(new SqlParameter("@address", zoo.Address));
                        cmd.Parameters.Add(new SqlParameter("@acres", zoo.Acres));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
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
                if (!ZooExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE: api/Zoos/5
        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Zoos WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
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
                if (!ZooExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool ZooExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name], Address, Acres
                        FROM Zoos
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}
