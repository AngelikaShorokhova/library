using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace ConsoleApp5
{
    class Program
    {
        private const string connStr =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=books;Integrated Security=True;";
        static void Main(string[] args)
        {
            List<string> student = new List<string>();
            Dictionary<string, int> list = new Dictionary<string, int>();
            int maxCount = 0;
            string maxName = string.Empty;

            using (var conn = new SqlConnection())
            {
                conn.ConnectionString = connStr;
                conn.Open();

                var command = new SqlCommand(@"select * from student", conn);

                using (var dr = command.ExecuteReader())
                {
                    while (dr.Read())
                         student.Add(dr["ФИО"].ToString());
                }
                string date = $"{DateTime.Now.AddDays(-14).ToString("yyyy/MM/dd")}";
                foreach (string st in student)
                {
                    command = new SqlCommand(@$"select COUNT(student_book.Студент_id) from student_book 
                        join student on student_book.Студент_id = student.id 
                        where student.ФИО = '{st}' AND (DATEDIFF(DAY, Дата_выдачи, Дата_возврата) >14 
                        OR (Дата_возврата is null AND Дата_выдачи < '{date}')) 
                        group by student.ФИО", conn);

                    using (var dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            if (int.Parse(dr[""].ToString()) > maxCount)
                            {
                                maxCount = int.Parse(dr[""].ToString());
                                list.Add(st, maxCount);
                            }
                        }
                    }
                }
            }
            Console.WriteLine($"Количество злостных читалей: {list.Count}, количество книг: {maxCount}");
            foreach (KeyValuePair<string, int> keyValue in list)
            {
                if (keyValue.Value == maxCount)
                    Console.WriteLine(keyValue.Key);
            }
            Console.ReadLine();
        }
    }
}
