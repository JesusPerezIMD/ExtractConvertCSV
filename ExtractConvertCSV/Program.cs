using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CsvHelper;

class Program
{
    static void Main(string[] args)
    {
        string fileName = "ItemErrors.csv";

        // Verificar que el archivo CSV existe.
        if (!File.Exists(fileName))
        {
            Console.WriteLine("Archivo no encontrado");
            return;
        }

        using (var reader = new StreamReader(fileName))
        using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.CurrentCulture))
        {
            var data = new Dictionary<string, List<string>>();
            bool isFirstRow = true;

            while (csv.Read())
            {
                if (isFirstRow)
                {
                    isFirstRow = false;
                    continue;
                }

                string email = csv.GetField(1);
                string text = csv.GetField(6);

                if (string.IsNullOrEmpty(email))
                {
                    continue;
                }

                if (!data.ContainsKey(email))
                {
                    data[email] = new List<string>();
                }

                // Extrae el texto que está entre comillas.
                var matches = Regex.Matches(text, "\"(.*?)\"");

                foreach (Match match in matches)
                {
                    data[email].Add(match.Groups[1].Value);
                }
            }

            // Verificar si la carpeta existe, sino, la crea.
            string folderPath = "ExtractCsv";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            foreach (var entry in data)
            {
                // Crea un archivo .txt por cada correo.
                string filename = Path.Combine(folderPath, $"{entry.Key}.txt");

                if (!File.Exists(filename))
                {
                    File.WriteAllLines(filename, entry.Value);
                }
                else
                {
                    File.AppendAllLines(filename, entry.Value);
                }
            }
        }
    }
}