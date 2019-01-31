using CodingConnected.TraCI.NET;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class drive : MonoBehaviour {
    //Server starten!
    //sumo --remote-port 4001 --net map.net.xml
    //https://github.com/argos-research/sumo
    public GameObject playerCar;
    public GameObject car0;
    public GameObject car1;
    public GameObject car2;
    public GameObject car3;
    public GameObject ped0;

    public Camera mainCamera;
    public Camera playerCamera;

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
        mainCamera.enabled = false;
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

        client.Vehicle.Add("userCar", "DEFAULT_VEHTYPE", "", 0, 0, 0, Byte.Parse("0"));
        client.Vehicle.SetLength("userCar", 3.0);
        client.Vehicle.SetWidth("userCar", 1.5);

        client.Person.Add("ped0", "DEFAULT_PEDTYPE", "gneE12", 0.0, 0.0);
        client.Person.AppendWalkingStage("ped0", new List<string>(new String[] { "gneE12", ":gneJ11_w1", ":gneJ11_c1", ":gneJ11_w2", ":gneJ11_c2", ":gneJ11_w0", "gneE14" }), 0.0, -1, 5, "");     
        
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
        sendPosition();
        //String entry1 = "Auto 2; Winkel: " + client.Vehicle.GetAngle("veh1").Content.ToString();
        //String entry2 = "Auto 3; Winkel: " + client.Vehicle.GetAngle("veh2").Content.ToString();
        //lines.Add(entry1);
        //lines.Add(entry2);
        //if (client.Vehicle.GetAccel("veh1").Result == ResultCode.Failed && client.Vehicle.GetAccel("veh2").Result == ResultCode.Failed)
        //{
        //    System.IO.File.WriteAllLines(@"C:\Users\Pascal Pries\Desktop\Teststrecke.txt", lines);
        //}
        if (Input.GetKeyDown(KeyCode.K))
        {
            if(mainCamera.enabled == true)
            {
                mainCamera.enabled = false;
                playerCamera.enabled = true;
            }
            else
            {
                mainCamera.enabled = true;
                playerCamera.enabled = false;
            }
        }

        if (client.Person.GetSpeed("ped0").Result == ResultCode.Success)
        {
            ped0.GetComponent<Transform>().eulerAngles = new Vector3(ped0.GetComponent<Transform>().eulerAngles.x, float.Parse(client.Person.GetAngle("ped0").Content.ToString()), ped0.GetComponent<Transform>().eulerAngles.z);
            ped0.GetComponent<Transform>().position = new Vector3((float)(client.Person.GetPosition("ped0").Content.X), ped0.GetComponent<Transform>().position.y < 0 ? 0.07F : ped0.GetComponent<Transform>().position.y, (float)(client.Person.GetPosition("ped0").Content.Y));
        }
        else
        {
            client.Person.Add("ped0", "DEFAULT_PEDTYPE", "gneE12", 0.0, 0.0);
            client.Person.AppendWalkingStage("ped0", new List<string>(new String[] { "gneE12", ":gneJ11_w1", ":gneJ11_c1", ":gneJ11_w2", ":gneJ11_c2", ":gneJ11_w0", "gneE14" }), 0.0, -1, 5, "");
        }


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

    private void sendPosition()
    {
        if (client.Vehicle.GetSpeed("userCar").Result == ResultCode.Success)
        {
            double x = playerCar.GetComponent<Transform>().position.x;
            double y = playerCar.GetComponent<Transform>().position.z;
            client.Vehicle.MoveToXY("userCar", 0, "", 0, x, y, 0, 2);
            Debug.Log("x: " + x + "; y: " + y);
        }
        else
        {
            client.Vehicle.Add("userCar", "DEFAULT_VEHTYPE", "", 0, 0, 0, Byte.Parse("0"));
            client.Vehicle.SetLength("userCar", 3.0);
            client.Vehicle.SetWidth("userCar", 1.5);
        }
    }

}
