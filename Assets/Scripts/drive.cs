using CodingConnected.TraCI.NET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class drive : MonoBehaviour {

    public GameObject car;

    private CarController carController;
    private TraCIClient client = new TraCIClient();
    private int id;
    int zaehler = 0;

    // Use this for initialization
    void Start () {
        id = 0;
        client.Connect("localhost", 4001);
        addVehicle();
        carController = car.GetComponent<CarController>();
    }

    public void addVehicle()
    {
        TraCIResponse<object> routeResponse = client.Route.Add("Route1", new List<string>(new String[] { "-gneE0", "gneE2", "gneE3", "gneE4" }));
        client.Vehicle.Add("veh" + id, "DEFAULT_VEHTYPE", "Route1", 5, 0, 0, Byte.Parse("0"));
        id++;
    }
	
	// Update is called once per frame
	void Update() {
        //carController.Move(0, 1, 1, 0);
        if (zaehler == 100)
        {
            try
            {
                float oldAngle = float.Parse(client.Vehicle.GetAngle("veh0").Content.ToString());
                client.Control.SimStep();
                float acc = float.Parse((client.Vehicle.GetAccel("veh0").Content).ToString()) > 0 ? 1 : 0;
                //carController.Move(float.Parse(client.Vehicle.GetAngle("veh0").Content.ToString()), acc, acc, 0);
                float newAngle = float.Parse(client.Vehicle.GetAngle("veh0").Content.ToString());
                Debug.Log(client.Vehicle.GetAngle("veh0").Content.ToString());
                carController.Move(getAngle(oldAngle, newAngle), acc, acc, 0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            zaehler = 0;
        } else
        {
            zaehler++;
        }

    }

    private float getAngle(float oldAngle, float newAngle)
    {
        if (Math.Abs(oldAngle - newAngle) > 0.00001 && oldAngle > newAngle)
        {
            return -1.0F;
        }
        if ((Math.Abs(oldAngle - newAngle) > 0.00001 && oldAngle < newAngle)) {
            return 1.0F;
        }
        return 0.0F;
    }

 
}
