using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    class XmlMapReader
    {
        public static List<MapNode> PedestrianNodes = new List<MapNode>();

        public static List<MapNode> AllNodes = new List<MapNode>();

        public static void Initialize()
        {
            ReadFile();

            PedestrianNodes.AddRange(AllNodes.Where(n => n.GetAllowedMapObjects().Contains(MapObject.PEDESTRIAN)));
        }

        private static void ReadFile()
        {
            System.IO.StreamReader file = new System.IO.StreamReader("map.net.xml");
            String line;
            while((line = file.ReadLine()) != null)
            {
                String[] parts = line.Split(' ');
                if (line.Contains("<lane"))
                {
                    String id = parts[9].Split('"')[1].Split('_')[0];
                    if (AllNodes.Where(n => n.GetName().Equals(id)).Count() == 0)
                    {
                        MapNode Node = new MapNode(id);
                        Node.GetAllowedMapObjects().Add(MapObject.CAR);
                        if (line.Contains(" allow="))
                        {
                            String allowedObjects = parts[11].Split('"')[1];
                            if (allowedObjects.Contains("pedestrian"))
                            {
                                Node.GetAllowedMapObjects().Add(MapObject.PEDESTRIAN);
                            }
                        }
                        AllNodes.Add(Node);
                        
                    }
                    else if (!AllNodes.Where(n => n.GetName().Equals(id)).First().GetAllowedMapObjects().Contains(MapObject.PEDESTRIAN))
                    {
                        if (line.Contains(" allow="))
                        {
                            String allowedObjects = parts[11].Split('"')[1];
                            if (allowedObjects.Contains("pedestrian"))
                            {
                                AllNodes.Where(n => n.GetName().Equals(id)).First().GetAllowedMapObjects().Add(MapObject.PEDESTRIAN);
                            }
                        }
                    }
                }
                else if (line.Contains("<connection"))
                {
                    String von = parts[5].Split('"')[1];
                    String nach = parts[6].Split('"')[1];
                    Debug.Log("von: " + von);
                    Debug.Log("nach: " + nach);
                    if (!von.Contains("_") && !nach.Contains("_"))
                    {
                        AllNodes.Where(n => n.GetName().Equals(von)).First().GetNextNodes().Add(nach);
                    }
                }
            }
        }

        //Durch Übergabe der Liste ist die Methode für alle Arten von Fahrzeugen/Person gültig 
        public static List<String> GetRandomRoute(List<MapNode> possibleNodes)
        {
            List<String> result = new List<string>();

            System.Random dice = new System.Random();
            //Startpunkt
            result.Add(possibleNodes.ElementAt(dice.Next() % possibleNodes.Count).GetName());
            //Insgesamt bis zu 10 Stationen pro Route
            bool addNodes = true;
            while (addNodes)
            { 
                MapNode el = possibleNodes.Where(n => result.Last().Equals(n.GetName())).First();
                if (el.GetNextNodes().Count > 0)
                {
                    result.Add(el.GetNextNodes().ElementAt(dice.Next() % el.GetNextNodes().Count));
                }
                else
                {
                    //Dann gibt es keinen weiteren Punkt, weil man aus der Karte fährt
                    addNodes = false;
                }
            }
            
            return result;
        }
    }
}
