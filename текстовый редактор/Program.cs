using System;
using System.IO;
using Newtonsoft.Json;
using System.Xml;
using System.Xml.Serialization;

namespace TextEditor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите путь к файлу:");
            string filePath = Console.ReadLine();

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл не найден.");
                return;
            }

            string fileExtension = Path.GetExtension(filePath);
            string fileFormat = fileExtension.ToLower().Replace(".", "");

            Figure figure = LoadFromFile(filePath, fileFormat);
            if (figure == null)
            {
                Console.WriteLine("Не удалось загрузить данные из файла.");
                return;
            }

            Console.WriteLine("Данные загружены:");
            Console.WriteLine($"Название: {figure.Name}");
            Console.WriteLine($"Ширина: {figure.Width}");
            Console.WriteLine($"Высота: {figure.Height}");

            Console.WriteLine("Нажмите F1, чтобы сохранить файл.");

            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.F1)
                {
                    SaveToFile(filePath, figure);
                    Console.WriteLine("Файл сохранен.");
                }
            } while (keyInfo.Key != ConsoleKey.Escape);

            Console.WriteLine("Программа завершена.");
        }

        static Figure LoadFromFile(string filePath, string fileFormat)
        {
            Figure figure = null;

            try
            {
                switch (fileFormat)
                {
                    case "txt":
                        using (StreamReader reader = new StreamReader(filePath))
                        {
                            string name = reader.ReadLine();
                            int width = int.Parse(reader.ReadLine());
                            int height = int.Parse(reader.ReadLine());

                            figure = new Figure(name, width, height);
                        }
                        break;
                    case "json":
                        using (StreamReader reader = new StreamReader(filePath))
                        {
                            string json = reader.ReadToEnd();
                            figure = JsonConvert.DeserializeObject
                            (json);
                        }
                        break;
                    case "xml":
                        using (StreamReader reader = new StreamReader(filePath))
                        {
                            string xml = reader.ReadToEnd();
                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.LoadXml(xml);

                            XmlNode root = xmlDoc.SelectSingleNode("Figure");
                            string name = root.SelectSingleNode("Name").InnerText;
                            int width = int.Parse(root.SelectSingleNode("Width").InnerText);
                            int height = int.Parse(root.SelectSingleNode("Height").InnerText);

                            figure = new Figure(name, width, height);
                        }
                        break;
                    default:
                        Console.WriteLine("Неподдерживаемый формат файла.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке файла: {ex.Message}");
            }

            return figure;
        }

        static void SaveToFile(string filePath, Figure figure)
        {
            string fileFormat = Path.GetExtension(filePath).ToLower().Replace(".", "");

            try
            {
                switch (fileFormat)
                {
                    case "txt":
                        using (StreamWriter writer = new StreamWriter(filePath))
                        {
                            writer.WriteLine(figure.Name);
                            writer.WriteLine(figure.Width);
                            writer.WriteLine(figure.Height);
                        }
                        break;
                    case "json":
                        string json = JsonConvert.SerializeObject(figure);
                        using (StreamWriter writer = new StreamWriter(filePath))
                        {
                            writer.Write(json);
                        }
                        break;
                    case "xml":
                        XmlSerializer serializer = new XmlSerializer(typeof(Figure));
                        using (StreamWriter writer = new StreamWriter(filePath))
                        {
                            serializer.Serialize(writer, figure);
                        }
                        break;
                    default:
                        Console.WriteLine("Неподдерживаемый формат файла.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении файла: {ex.Message}");
            }
        }
    }

    public class Figure
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Figure(string name, int width, int height)
        {
            Name = name;
            Width = width;
            Height = height;
        }
    }
}