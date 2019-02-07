using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    /**
     * Das Enum sollte alle möglichen Objekte auf der Karte darstellen, die mit SUMO erzeugt werden können.
     * Aktuell nur Beispielhaft für Fußgänger und Autos angelegt. Kann aber mit BOAT, BUS, TRAIN, etc. ergänzt werden, wenn diese Fahrzeuge in der 
     * Simulation hinzukommen.
     */ 
    enum MapObject
    {
        PEDESTRIAN,
        CAR
    }
}
