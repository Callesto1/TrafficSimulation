using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    /**
     *Das ist eine Helper-Klasse. Sie dient dazu beliebige .net.xml einzulesen und für Autos zufällige Routen zu generieren.
     *Nach dem gleichen Schema, nur mit anderen Knotenpunkten wäre das ganze auch denbar für Fußgänger etc.
     */
    class XmlMapReader
    {
        public static List<MapNode> AllNodes = new List<MapNode>();

        /**
         *Diese Methode muss ganz am Anfang einmal aufgerufen werden, um das XML zu lesen und Die Liste mit den Knoten zu initialisieren 
         */
        public static void Initialize()
        {
            ReadFile();
        }

        /**
         * Hier wird das XML eingelesen und die Knoten und Connections in der Liste, bzw. den MapNode-Objekten persistiert
         */ 
        private static void ReadFile()
        {
            //Einlesen des XML-Files, das im Projektverzeichnis liegt
            System.IO.StreamReader file = new System.IO.StreamReader("map.net.xml");
            String line;
            //Zeile für Zeile durch das Dokument gehen, bis zum Ende
            while((line = file.ReadLine()) != null)
            {
                //Das Dokument ist so aufgebaut, dass die Parameter durch ein Leerzeichen getrennt sind. Für das bessere Arbeiten wird daher an diesen Stellen
                //gesplittet und die einzelnen Teile ion ARray geschrieben
                String[] parts = line.Split(' ');
                //Wenn die Zeile <lane beinhaltet, dann wird hier eine STraße definiert, die in die Liste übernommen werden muss
                if (line.Contains("<lane"))
                {
                    //Eine Zeile ist wie folgt aufgebaut:
                    //        <lane id="-gneE14_1" index="1" disallow="pedestrian" speed="13.89" length="62.01" shape="-236.33,407.12 -212.58,349.84"/>
                    //Daher muss dieser Teil nochmal gesplittet werden, da nur der Teil -gneE14 benötigt wird. Es wird auf Part 9 zugegriffen, da am Anfang 
                    //jeder Zeile im XML diverse Leerzeichen sind, die von der STruktur festgelegt werden. Dies ist aber für jedes net.xml einheitlich
                    //und somit kein Problem
                    String id = parts[9].Split('"')[1].Split('_')[0];
                    //Die Prüfung auf das E ist wichtig, da für Autos nur normale Straßen benötigt werden. Verbindungsteile wie Ecken und Kreuzungen haben 
                    //stattdessen ein J im Namen und werden für die Angabe der Route nicht gebraucht
                    if (AllNodes.Where(n => n.GetName().Equals(id)).Count() == 0 && id.Contains("E"))
                    {
                        MapNode Node = new MapNode(id);
                        Node.GetAllowedMapObjects().Add(MapObject.CAR);
                        if (line.Contains(" allow="))
                        {
                            String allowedObjects = parts[11].Split('"')[1];
                            //Um für die Zukunft einen Ausblick zu geben, wird hier bereits vermerkt, ob auch Fußgänger die STraße verwenden dürfen.
                            //Ebenso kann das auch für Boote, Züge, etc. gemacht werden
                            if (allowedObjects.Contains("pedestrian"))
                            {
                                Node.GetAllowedMapObjects().Add(MapObject.PEDESTRIAN);
                            }
                        }
                        AllNodes.Add(Node);
                        
                    }
                    //Manche Straßen sind mehrfach im XML vorhanden, da sie einmal für Fußgänger und einmal für Autos usw. vorhanden sind. Dann sollten
                    //wie folgt die Erlaubten MapObjects in dem MapNode als erlaubt abgelegt werden
                    else if (id.Contains("E") && !AllNodes.Where(n => n.GetName().Equals(id)).First().GetAllowedMapObjects().Contains(MapObject.PEDESTRIAN))
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
                //Hier werden die Verbindungen der Straßen ausgelesen, was essenziell für die Routenberechnung ist
                else if (line.Contains("<connection"))
                {
                    String von = parts[5].Split('"')[1];
                    String nach = parts[6].Split('"')[1];
                    //Dieses if ist aktuell noch wichtig, da ansonsten auch Knoten berücksichtigt werden, die nicht nur zu Autos gehhören und eine etwas
                    //andere Darstellung haben.
                    if (!von.Contains("_") && !nach.Contains("_"))
                    {
                        AllNodes.Where(n => n.GetName().Equals(von)).First().GetNextNodes().Add(nach);
                    }
                }
            }
        }

        /**
         * Diese Methode liefert eine zufällige Route.
         * Es wird eine Liste übergeben, die alle für das ausgewählte Fahrzeug zulässige Knoten enthält. Dadurch ist die Methode wiederverwendbar.
         */ 
        public static List<String> GetRandomRoute(List<MapNode> possibleNodes)
        {
            List<String> result = new List<string>();

            System.Random dice = new System.Random();

            //Startpunkt wird zunächst ermittelt
            //Hierzu wird eine Straße gesucht, die keine Verbindungen zu weiteren Straßen enthält. Das bedeutet nämlich, dass diese Straße eine Sackgasse 
            //ist oder aus der Karte hinaus führt. Die entgegengesetzte Straße führt entsprechend in die Karte hinein und eigenet sich somit optimal zum 
            //spawnen neuer Fahrzeuge.
            MapNode node = possibleNodes.Where(n => n.GetNextNodes().Count == 0 && n.GetName().Contains("E")).ElementAt(dice.Next() % possibleNodes.Where(n => n.GetNextNodes().Count == 0 && n.GetName().Contains("E")).Count());
            String startpunkt = null;
            while (startpunkt == null)
            {
                //Die entgegengesetzte Straße heißt wie die Straße in Node, nur mit/ohne "-" am Anfang. Das Wird in diesem if-else entsprechend modifiziert
                if (node.GetName().Contains("-") && possibleNodes.Where(n => n.GetName().Equals(node.GetName().Substring(1, node.GetName().Length - 1))).Count() == 1)
                {
                    startpunkt = node.GetName().Substring(1, node.GetName().Length - 1);
                }
                else if (!node.GetName().Contains("-") && possibleNodes.Where(n => n.GetName().Equals("-" + node.GetName())).Count() == 1)
                {
                    startpunkt = "-" + node.GetName();
                }
            }
            result.Add(startpunkt);
            bool addNodes = true;
            //Hier werden so lange Straßen zur Liste hinzugefügt, die für die vorherige Straße als Nachfolger in Frage kommen, bis man wieder an einem Punkt
            //ist, wo es nicht weiter geht. Sprich: Sackgasse oder man fährt aus der Karte.
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
