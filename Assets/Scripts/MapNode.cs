using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    /**
     * Ein MapNode repräsentiert eine Straße/Gehweg/etc.
     */ 
    class MapNode
    {
        //Liste mit Wegen, die von dieser Straße erreichbar sind
        private List<String> NextNodes;
        //Name / ID der Straße
        private String Name;
        //Fahrzeuge/Objekte, die sich auf dem Weg bewegen dürfen
        private List<MapObject> AllowedMapObjects;

        /**
         * Der Name ist eine Pflichtangabe, da man ansonsten damit nicht arbeiten kann. Die anderen beiden Parameter können auch später initialisiert werden
         */ 
        public MapNode(String Name)
        {
            this.Name = Name;
            this.NextNodes = new List<string>();
            this.AllowedMapObjects = new List<MapObject>();

        }

        public String GetName()
        {
            return this.Name;
        }

       public List<MapObject> GetAllowedMapObjects()
        {
            return this.AllowedMapObjects;
        }

        public List<String> GetNextNodes()
        {
            return this.NextNodes;
        }

    }
}
