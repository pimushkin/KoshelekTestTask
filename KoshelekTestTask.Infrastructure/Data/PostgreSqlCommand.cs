using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KoshelekTestTask.Core.Entities;
using KoshelekTestTask.Core.Interfaces;
using KoshelekTestTask.Core.Models;
using Npgsql;

namespace KoshelekTestTask.Infrastructure.Data
{
    public class PostgreSqlCommand : IPostgreSqlCommand
    {
        private static readonly string ConnectionString =
            $"Host={Environment.GetEnvironmentVariable("POSTGRES_HOST")};" +
            $"Username={Environment.GetEnvironmentVariable("POSTGRES_USER")};" +
            $"Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")};" +
            $"Database={Environment.GetEnvironmentVariable("POSTGRES_DB")}";

        public void CreateTable()
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            connection.Open();

            using var command = new NpgsqlCommand
            {
                Connection = connection,
                CommandText = "CREATE TABLE IF NOT EXISTS koshelek (\r\n" +
                              "id serial NOT NULL,\r\n" +
                              "serial_number int NOT NULL,\r\n" +
                              "text varchar(128) NOT NULL,\r\n" +
                              "time_of_sending timestamp without time zone NOT NULL,\r\n" +
                              "CONSTRAINT koshelek_pk PRIMARY KEY (id)\r\n" +
                              ")"
            };

            command.ExecuteNonQuery();
        }

        public async Task SendMessageAsync(Message message)
        {
            await using var connection = new NpgsqlConnection(ConnectionString);
            connection.Open();

            await using var command = new NpgsqlCommand
            {
                Connection = connection,
                CommandText =
                    $"INSERT INTO koshelek (serial_number, text, time_of_sending) VALUES ({message.SerialNumber}, '{message.Text}', '{message.TimeOfSending.ToUniversalTime():O}'::timestamp);"
            };

            command.ExecuteNonQuery();
        }

        public async Task<List<Message>> FindMessagesOverPeriodOfTimeAsync(Interval interval)
        {
            await using var connection = new NpgsqlConnection(ConnectionString);
            connection.Open();

            await using var command = new NpgsqlCommand
            {
                Connection = connection,
                CommandText =
                    "SELECT serial_number, text, time_of_sending FROM koshelek " +
                    $"WHERE time_of_sending BETWEEN '{interval.Beginning.ToUniversalTime():O}'::timestamp AND '{interval.End.ToUniversalTime():O}'::timestamp;"
            };

            await using var dataReader = await command.ExecuteReaderAsync();
            var messages = new List<Message>();
            while (dataReader.Read())
                messages.Add(new Message
                {
                    SerialNumber = dataReader.GetInt32(0),
                    Text = dataReader.GetString(1),
                    TimeOfSending = (DateTime)dataReader.GetTimeStamp(2)
                });
            return messages;
        }
    }
}