using System;
using System.Data;
using Npgsql;

class Program {
    static void Main() {
        try {
            string connString = ""Host=ep-dawn-cloud-apqv43p1-pooler.c-7.us-east-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_1OMTRa6pzdPy;SSL Mode=Require;Trust Server Certificate=true;"";
            using var conn = new NpgsqlConnection(connString);
            conn.Open();
            using var cmd = new NpgsqlCommand(""SELECT table_name FROM information_schema.tables WHERE table_schema = 'public';"", conn);
            using var reader = cmd.ExecuteReader();
            Console.WriteLine(""--- TABLES ---"");
            while (reader.Read()) {
                Console.WriteLine(reader.GetString(0));
            }
        } catch(Exception ex) {
            Console.WriteLine(""ERROR: "" + ex.Message);
        }
    }
}
