﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using t_ag.Models;

namespace t_ag.DAO
{
    class TourDAO
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static string connectonString = @"Data Source=LOCALHOST\SQLEXPRESS;" + @"Initial Catalog=t-agDatabase;" + @"Integrated Security=True;" + @"Pooling=False;" + @"MultipleActiveResultSets=True";
        public static List<Tour> getAllTours()
        {
            logger.Info("Get all tours");
            List<Tour> tours = new List<Tour>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectonString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT * FROM [Tour]", connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Tour el = new Tour();
                        el.id = (int)reader["Id"];
                        el.country = (string)reader["Country"];
                        el.type = (string)reader["Type"];
                        el.price = (int)reader["Price"];
                        el.description = (string)reader["Description"];
                        el.sale = (int)reader["Sale"];
                        el.saleDate = (DateTime)reader["SaleDate"];
                        el.feedbacks = new List<String>();

                        SqlCommand command2 = new SqlCommand("SELECT * FROM [TourFeedback] WHERE TourId=@id", connection);
                        command2.Parameters.Add("@id", SqlDbType.Int);
                        command2.Parameters["@id"].Value = el.id;

                        SqlDataReader reader2 = command2.ExecuteReader();
                        while (reader2.Read())
                        {
                            el.feedbacks.Add((string)reader2["Feedback"]);
                        }

                        tours.Add(el);
                    }
                }
            }
            catch (SqlException ex)
            {
                string message = "Cannot get all tours: " + ex.Message;
                logger.Error(message);
                throw new DOAException(message, ex);
            }
            return tours;
        }

        public static int addTour(Tour tour)
        {
            logger.Info("Add tour");
            try
            {
                using (SqlConnection connection = new SqlConnection(connectonString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("INSERT INTO [Tour] ([Country], [Type], [Price], [Description], [Sale], [SaleDate]) VALUES (@country, @type, @price, @description, @sale, @saleDate); SELECT SCOPE_IDENTITY()", connection);
                    command.Parameters.Add("@country", SqlDbType.VarChar);
                    command.Parameters.Add("@type", SqlDbType.VarChar);
                    command.Parameters.Add("@price", SqlDbType.Int);
                    command.Parameters.Add("@description", SqlDbType.Text);
                    command.Parameters.Add("@sale", SqlDbType.Int);
                    command.Parameters.Add("@saleDate", SqlDbType.SmallDateTime);

                    command.Parameters["@country"].Value = tour.country;
                    command.Parameters["@type"].Value = tour.type;
                    command.Parameters["@price"].Value = tour.price;
                    command.Parameters["@description"].Value = tour.description;
                    command.Parameters["@sale"].Value = tour.sale;
                    command.Parameters["@saleDate"].Value = tour.saleDate;

                    int tourId = Convert.ToInt32(command.ExecuteScalar());

                    tour.feedbacks.ForEach(el => addFeedback(tourId, el));

                    return tourId;
                }
            }
            catch (SqlException ex)
            {
                string message = "Cannot add tour: " + ex.Message;
                logger.Error(message);
                throw new DOAException(message, ex);
            }
        }

        public static void updateTour(Tour tour)
        {
            logger.Info("Update tour");
            try
            {
                using (SqlConnection connection = new SqlConnection(connectonString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("UPDATE [Tour] SET [Country]=@country, [Type]=@type, [Price]=@price, [Description]=@description, [Sale]=@sale, [SaleDate]=@saleDate WHERE [Id]=@id", connection);
                    command.Parameters.Add("@id", SqlDbType.Int);
                    command.Parameters.Add("@country", SqlDbType.VarChar);
                    command.Parameters.Add("@type", SqlDbType.VarChar);
                    command.Parameters.Add("@price", SqlDbType.Int);
                    command.Parameters.Add("@description", SqlDbType.Text);
                    command.Parameters.Add("@sale", SqlDbType.Int);
                    command.Parameters.Add("@saleDate", SqlDbType.SmallDateTime);

                    command.Parameters["@id"].Value = tour.id;
                    command.Parameters["@country"].Value = tour.country;
                    command.Parameters["@type"].Value = tour.type;
                    command.Parameters["@price"].Value = tour.price;
                    command.Parameters["@description"].Value = tour.description;
                    command.Parameters["@sale"].Value = tour.sale;
                    command.Parameters["@saleDate"].Value = tour.saleDate;

                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                string message = "Cannot update tour: " + ex.Message;
                logger.Error(message);
                throw new DOAException(message, ex);
            }
        }

        public static void deleteTourById(int id)
        {
            logger.Info("Delete tour by id: " + id);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectonString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("DELETE FROM [TourFeedback] WHERE [TourId]=@id; DELETE FROM [Tour] WHERE [Id]=@id", connection);
                    command.Parameters.Add("@id", SqlDbType.Int);

                    command.Parameters["@id"].Value = id;

                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                string message = "Cannot delete tour (id: " + id + "): " + ex.Message;
                logger.Error(message);
                throw new DOAException(message, ex);
            }
        }

        public static void addFeedback(int tourId, string feedback)
        {
            logger.Info("Add feedback");
            try
            {
                using (SqlConnection connection = new SqlConnection(connectonString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("INSERT INTO [TourFeedback] (TourId, Feedback) VALUES (@tourId, @feedback)", connection);
                    command.Parameters.Add("@tourId", SqlDbType.Int);
                    command.Parameters.Add("@feedback", SqlDbType.Text);

                    command.Parameters["@tourId"].Value = tourId;
                    command.Parameters["@feedback"].Value = feedback;

                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                string message = "Cannot add feedback: " + ex.Message;
                logger.Error(message);
                throw new DOAException(message, ex);
            }
        }

        public static Tour getTourById(int id)
        {
            logger.Info("Get tour by id: " + id);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectonString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT * FROM [Tour] WHERE Id=@id", connection);
                    command.Parameters.Add("@id", SqlDbType.Int);

                    command.Parameters["@id"].Value = id;

                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();

                    Tour el = new Tour();
                    el.id = (int)reader["Id"];
                    el.country = (string)reader["Country"];
                    el.type = (string)reader["Type"];
                    el.price = (int)reader["Price"];
                    el.description = (string)reader["Description"];
                    el.sale = (int)reader["Sale"];
                    el.saleDate = (DateTime)reader["SaleDate"];
                    el.feedbacks = new List<String>();

                    SqlCommand command2 = new SqlCommand("SELECT * FROM [TourFeedback] WHERE TourId=@id", connection);
                    command2.Parameters.Add("@id", SqlDbType.Int);
                    command2.Parameters["@id"].Value = el.id;

                    SqlDataReader reader2 = command2.ExecuteReader();
                    while (reader2.Read())
                    {
                        el.feedbacks.Add((string)reader2["Feedback"]);
                    }

                    return el;
                }
            }
            catch (SqlException ex)
            {
                string message = "Cannot get tour by id (Id: " + id + "): " + ex.Message;
                logger.Error(message);
                throw new DOAException(message, ex);
            }
            catch (InvalidOperationException ex)
            {
                // No such id
                string message = "Cannot get tour by id (Wrong id: " + id + "): " + ex.Message;
                logger.Error(message);
                throw new DOAException(message, ex);
            }
        }
    }
}
