using System;
using System.IO;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Collections;

namespace second
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"../../../input.txt";
            string authorToFind = "Queen";
            ArrayList searchResults = new ArrayList();
            Source[] sources;
            try
            {
                sources = parseFile(path);
                foreach (Source source in sources)
                {
                    if (source.author == authorToFind) searchResults.Add(source);
                    Console.WriteLine(source.Info() + '\n');
                }
                Console.WriteLine("Search results:\n" );
                foreach (Source found in searchResults)
                {
                    Console.WriteLine(found.Info() + '\n');
                }
                Serialize(sources);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File is not found");
            }
            catch (SerializationException)
            {
                Console.WriteLine("Some problems with serialization");
            }
        }
        public static Source[] parseFile(string pathIn)
        {
            using (StreamReader sr = new StreamReader(pathIn, System.Text.Encoding.Default))
            {

                int n = Convert.ToInt32(sr.ReadLine());
                int count = 0;
                Source[] sources = new Source[n];
                string line;
                string[] splitLine;
                while ((line = sr.ReadLine()) != null)
                {
                    splitLine = line.Split("|");
                    switch (splitLine[0])
                    {
                        case "Book":
                            sources[count++] = new Book(splitLine[1], splitLine[2], Convert.ToDateTime(splitLine[3]), splitLine[4]);
                            break;
                        case "Paper":
                            sources[count++] = new Paper(splitLine[1], splitLine[2], Convert.ToInt32(splitLine[3]), splitLine[4], Convert.ToDateTime(splitLine[5]));
                            break;
                        case "EResources":
                            sources[count++] = new EResources(splitLine[1], splitLine[2], splitLine[3], splitLine[4]);
                            break;
                    }
                }

                return sources;
            }
        }
        public static void Serialize(Source[] array)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(Source[]));
            using (FileStream fs = new FileStream("../../../source.xml", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, array);
            }
        }
    }
    [XmlInclude(typeof(Book))]
    [XmlInclude(typeof(Paper))]
    [XmlInclude(typeof(EResources))]
    [Serializable]
    public abstract class Source
    {
        public string author { get; set; }
        public string title { get; set; }

        public Source()
        { }
        public Source(string author, string title)
        {
            this.author = author;
            this.title = title;
        }
        public string Info() {
            Trace.WriteLine("Source.Info was called");
            return $"{SourceInfo()}\n{AdditionalInfo()}";
        }
        public string SourceInfo() { 
            Trace.WriteLine("Source.SourceInfo was called");
            return $"Author: {author}\nTitle: {title}"; 
        }
        abstract public string AdditionalInfo();
    }
    [Serializable]
    public class Book : Source
    {
        public DateTime date { get; set; }
        public string publisher { get; set; }
        public Book()
        { }
        public Book(string author, string title, DateTime date, string publisher) : base(author, title)
        {
            this.date = date;
            this.publisher = publisher;
        }
        override public string AdditionalInfo()
        {
            Trace.WriteLine("Book.AdditionalInfo was called");
            return $"Published: {date.ToString()}\nPublisher: {publisher}";
        }
    }
    [Serializable]
    public class Paper : Source
    {
        public int id { get; set; }
        public string journalTitle { get; set; }
        public DateTime date { get; set; }
        public Paper()
        { }
        public Paper(string author, string title, int id, string journalTitle, DateTime date) : base(author, title)
        {
            this.id = id;
            this.journalTitle = journalTitle;
            this.date = date;
        }
        override public string AdditionalInfo()
        {
            Trace.WriteLine("Book.AdditionalInfo was called");
            return $"Jornal's title: {journalTitle}\nJornal's number: {id}\nPublished: {date.ToString()}";
        }
    }
    [Serializable]
    public class EResources : Source
    {
        public string url { get; set; }
        public string annotation { get; set; }
        public EResources()
        { }
        public EResources(string author, string title, string url, string annotation) : base(author, title)
        {
            this.url = url;
            this.annotation = annotation;
        }
        override public string AdditionalInfo()
        {
            Trace.WriteLine("Book.AdditionalInfo was called");
            return $"Source: {url}\nAnnotation: {annotation}";
        }
    }
}
