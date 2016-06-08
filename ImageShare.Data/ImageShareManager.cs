using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ImageShare.Data
{
    public class ImageShareManager
    {
        private string _connectionString;

        public ImageShareManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddImage(Image image)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Images (FirstName, LastName, ImageFile, DateUploaded, ViewCount)" +
                                      " VALUES (@firstName, @lastName, @imageFile, @date, 0); SELECT @@Identity";
                command.Parameters.AddWithValue("@firstName", image.FirstName);
                command.Parameters.AddWithValue("@lastName", image.LastName);
                command.Parameters.AddWithValue("@imageFile", image.ImageFileName);
                command.Parameters.AddWithValue("@date", DateTime.Now);
                connection.Open();
                image.Id = (int)(decimal)command.ExecuteScalar();
            }
        }

        public IEnumerable<Image> GetFiveMostRecent()
        {
            return GetImages("SELECT Top 5 * FROM Images ORDER By DateUploaded DESC");
        }

        public IEnumerable<Image> GetFiveMostPopular()
        {
            return GetImages("SELECT Top 5 * FROM Images ORDER BY ViewCount DESC");
        }

        public Image GetImage(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Images WHERE Id = @id";
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = command.ExecuteReader();
                reader.Read();
                return ToImage(reader);
            }
        }

        public void IncrementCount(int imageId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "UPDATE Images SET ViewCount = ViewCount + 1 WHERE id = @id";
                command.Parameters.AddWithValue("@id", imageId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private IEnumerable<Image> GetImages(string query)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = query;
                connection.Open();
                List<Image> images = new List<Image>();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    images.Add(ToImage(reader));
                }

                return images;
            }
        }

        private Image ToImage(SqlDataReader reader)
        {
            Image image = new Image();
            image.Id = (int)reader["Id"];
            image.FirstName = (string)reader["FirstName"];
            image.LastName = (string)reader["LastName"];
            image.ViewCount = (int)reader["ViewCount"];
            image.DateUploaded = (DateTime)reader["DateUploaded"];
            image.ImageFileName = (string)reader["ImageFile"];
            return image;
        }


    }
}