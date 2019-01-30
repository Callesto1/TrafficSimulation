using CodingConnected.TraCI.NET;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class drive : MonoBehaviour {
    //Server starten!
    //sumo --remote-port 4001 --net map.net.xml
    public GameObject car0;
    public GameObject car1;
    public GameObject car2;
    public GameObject car3;

    //private GameObject[] cars;
    private CarController carController;
    private TraCIClient client = new TraCIClient();
    private int carid;
    private int routeid;

    int step;
    float time;
    //List<String> lines;


    // Use this for initialization
    void Start () {
        carid = 0;
        routeid = 0;
        step = 0;
        //lines = new List<string>();
        client.Connect("localhost", 4001);
        addStartingVehicles();
        carController = car0.GetComponent<CarController>();
    }

    public void addStartingVehicles()
    {
        client.Route.Add("Route" + routeid, new List<string>(new String[] { "-gneE10", "-gneE9", "-gneE8", "gneE14", "gneE15"}));
        routeid++;
        client.Route.Add("Route" + routeid, new List<string>(new String[] { "-gneE16", "-gneE14", "gneE8", "gneE9", "gneE10", "gneE11", "gneE12", "gneE14", "gneE16" }));
        routeid++;
        client.Route.Add("Route" + routeid, new List<string>(new String[] { "-gneE15", "gneE17"}));
        routeid++;
        client.Vehicle.Add("veh" + carid, "DEFAULT_VEHTYPE", "Route0", 0, 0, 0, Byte.Parse("0"));
        carid++;
        client.Vehicle.Add("veh" + carid, "DEFAULT_VEHTYPE", "Route1", 0, 0, 0, Byte.Parse("0"));
        carid++;
        client.Vehicle.Add("veh" + carid, "DEFAULT_VEHTYPE", "Route2", 0, 0, 0, Byte.Parse("0"));
        carid++;        
    }

    private void moveVehicles(String id, GameObject carObject)
    {
        String carID = "veh" + id;
        if (client.Vehicle.GetAccel(carID).Result == ResultCode.Success)
        {
            carObject.GetComponent<Transform>().eulerAngles = new Vector3(carObject.GetComponent<Transform>().eulerAngles.x, float.Parse(client.Vehicle.GetAngle(carID).Content.ToString()), carObject.GetComponent<Transform>().eulerAngles.z);
            carObject.GetComponent<Transform>().position = new Vector3((float)(client.Vehicle.GetPosition(carID).Content.X), carObject.GetComponent<Transform>().position.y < 0 ? 0.07F : carObject.GetComponent<Transform>().position.y, (float)(client.Vehicle.GetPosition(carID).Content.Y));
        }
        else
        {
            client.Vehicle.Add(carID, "DEFAULT_VEHTYPE", GetRandomRoute(), 0, 0, 0, Byte.Parse("0"));
        }
    }
	
	// Update is called once per frame
	void Update() {
        time += Time.deltaTime;
        //String entry1 = "Auto 2; Winkel: " + client.Vehicle.GetAngle("veh1").Content.ToString();
        //String entry2 = "Auto 3; Winkel: " + client.Vehicle.GetAngle("veh2").Content.ToString();
        //lines.Add(entry1);
        //lines.Add(entry2);
        //if (client.Vehicle.GetAccel("veh1").Result == ResultCode.Failed && client.Vehicle.GetAccel("veh2").Result == ResultCode.Failed)
        //{
        //    System.IO.File.WriteAllLines(@"C:\Users\Pascal Pries\Desktop\Teststrecke.txt", lines);
        //}
       try
       {
          
            client.Control.SimStep(time);
            step++;

            if(step==5)
            {
                client.Vehicle.Add("veh" + carid, "DEFAULT_VEHTYPE", "Route2", 0, 0, 0, Byte.Parse("0"));
                carid++;
            }

            moveVehicles("0", car0);
            moveVehicles("1", car1);
            moveVehicles("2", car2);

            if (client.Vehicle.GetAccel("veh1").Result == ResultCode.Success)
            {
                car1.GetComponent<Transform>().eulerAngles = new Vector3(car1.GetComponent<Transform>().eulerAngles.x, float.Parse(client.Vehicle.GetAngle("veh1").Content.ToString()), car1.GetComponent<Transform>().eulerAngles.z);
                car1.GetComponent<Transform>().position = new Vector3((float)(client.Vehicle.GetPosition("veh1").Content.X), car1.GetComponent<Transform>().position.y < 0 ? 0.07F : car1.GetComponent<Transform>().position.y, (float)(client.Vehicle.GetPosition("veh1").Content.Y));
            } else
            {
                client.Vehicle.Add("veh1", "DEFAULT_VEHTYPE", "Route1", 0, 0, 0, Byte.Parse("0"));
            }

            if (client.Vehicle.GetAccel("veh2").Result == ResultCode.Success)
            {
                car2.GetComponent<Transform>().eulerAngles = new Vector3(car2.GetComponent<Transform>().eulerAngles.x, float.Parse(client.Vehicle.GetAngle("veh2").Content.ToString()), car2.GetComponent<Transform>().eulerAngles.z);
                car2.GetComponent<Transform>().position = new Vector3((float)(client.Vehicle.GetPosition("veh2").Content.X), car2.GetComponent<Transform>().position.y < 0 ? 0.07F : car2.GetComponent<Transform>().position.y, (float)(client.Vehicle.GetPosition("veh2").Content.Y));
            } else
            {
                client.Vehicle.Add("veh2", "DEFAULT_VEHTYPE", "Route2", 0, 0, 0, Byte.Parse("0"));
            }
            if (step >= 5)
            {
                moveVehicles("3", car3);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private string GetRandomRoute()
    {
        System.Random rnd = new System.Random();
        return "Route" + (rnd.Next() % client.Route.GetIdList().Content.Count);
    }


}
