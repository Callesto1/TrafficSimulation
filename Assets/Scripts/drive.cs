using CodingConnected.TraCI.NET;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class drive : MonoBehaviour {
    //Server starten!
    //sumo --remote-port 4001 --net map0.net.xml
    public GameObject car;

    private CarController carController;
    private TraCIClient client = new TraCIClient();
    private int id;
    float oldAngle;
    int step;

    private Vector2 oldSimPosition;
    private Vector2 newSimPosition;

    private Vector2 oldUnityPosition;

    // Use this for initialization
    void Start () {
        id = 0;
        step = 0;
        client.Connect("localhost", 4001);
        addVehicle();
        carController = car.GetComponent<CarController>();
        oldSimPosition = new Vector2();
        newSimPosition = new Vector2();
        oldAngle = 0;

        oldUnityPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
    }

    public void addVehicle()
    {
        TraCIResponse<object> routeResponse = client.Route.Add("Route1", new List<string>(new String[] { "-gneE10", "-gneE9", "-gneE8", "-gneE12", "-gneE11", "-gneE10" }));
        client.Vehicle.Add("veh" + id, "DEFAULT_VEHTYPE", "Route1", 10, 0, 0, Byte.Parse("0"));
        id++;
    }
	
	// Update is called once per frame
	void Update() {
       try
       {
            Vector2 newUnityPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
            Vector2 differenceUnity = newUnityPosition - oldUnityPosition;
            double lengthUnity = Math.Sqrt(differenceUnity.x * differenceUnity.x + differenceUnity.y * differenceUnity.y);
            Vector2 differenceSim = newSimPosition - oldSimPosition;
            double lengthSim = Math.Sqrt(differenceSim.x * differenceSim.x + differenceSim.y * differenceSim.y);
            //Am Anfang hat die Geschwindigkeit einen negative Defaut-Wert. Ebenso die Beschleunigung
            if (lengthUnity >= lengthSim || client.Vehicle.GetSpeed("veh0").Content < -1073741823 || step < 3)
            {
                oldAngle = float.Parse(client.Vehicle.GetAngle("veh0").Content.ToString());
                oldSimPosition = new Vector2((float)client.Vehicle.GetPosition("veh0").Content.X, (float)client.Vehicle.GetPosition("veh0").Content.Y);
                client.Control.SimStep();
                step++;
                newSimPosition = new Vector2((float)client.Vehicle.GetPosition("veh0").Content.X, (float)client.Vehicle.GetPosition("veh0").Content.Y);
                oldUnityPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
            }
            float acc = float.Parse((client.Vehicle.GetAcceleration("veh0").Content).ToString()) > 0 ? float.Parse((client.Vehicle.GetAcceleration("veh0").Content).ToString()) : 0.2F;
            float newAngle = float.Parse(client.Vehicle.GetAngle("veh0").Content.ToString());
            float maxAcc = float.Parse(client.Vehicle.GetAccel("veh0").Content.ToString());
            carController.Move(GetAngle(oldAngle, newAngle), acc / maxAcc, acc / maxAcc, 0);

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    //Gibt den Winkel zurück, den das Auto lenken soll.
    private float GetAngle(float oldAngle, float newAngle)
    {
        //Dem Folgenden liegt zu Grunde, dass eine Kurve niemals mehr als 180° haben kann!
        //Nur in diesem Fall, wird überhaupt gelenkt
        if (Math.Abs(oldAngle - newAngle) > 0.001 && oldAngle > -0.001 && step > 3)
        {
            if (oldAngle < 180)
            {
                if (oldAngle < newAngle && oldAngle + 180 <= newAngle
                    || oldAngle > newAngle && oldAngle + (360 - newAngle) < 180)
                {
                    return 1 - Math.Abs(oldAngle - newAngle) / 360;
                }
                else
                {
                    return -(1 - Math.Abs(oldAngle - newAngle) / 360);
                }
            }
            else
            {
                if (oldAngle > newAngle && oldAngle - 180 >= newAngle)
                {
                    return -(1 - Math.Abs(oldAngle - newAngle) / 360);
                }
                else
                {
                    return 1 - Math.Abs(oldAngle - newAngle) / 360;
                }
            }
        }
        return 0.0F;
    }


}
