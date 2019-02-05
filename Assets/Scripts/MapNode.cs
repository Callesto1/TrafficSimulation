using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class MapNode
    {
        private List<String> NextNodes;
        private String Name;
        private List<MapObject> AllowedMapObjects;

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
