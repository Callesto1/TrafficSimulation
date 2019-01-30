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
    private int id;

    //float oldAngle;
    int step;
    float time;
    //List<String> lines;

    //private Vector2 oldSimPosition;
    //private Vector2 newSimPosition;

    //private Vector2 oldUnityPosition;

    // Use this for initialization
    void Start () {
        id = 0;
        step = 0;
        //lines = new List<string>();
        client.Connect("localhost", 4001);
        addVehicle();
        carController = car0.GetComponent<CarController>();
        //oldSimPosition = new Vector2();
        //newSimPosition = new Vector2();
        //oldAngle = 0;

        //oldUnityPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        //cars = GameObject.FindGameObjectsWithTag("Car");
    }

    public void addVehicle()
    {
        client.Route.Add("Route0", new List<string>(new String[] { "-gneE10", "-gneE9", "-gneE8", "-gneE12", "-gneE11"}));
        client.Route.Add("Route1", new List<string>(new String[] { "-gneE16", "-gneE14", "gneE8"}));
        client.Route.Add("Route2", new List<string>(new String[] { "-gneE15", "gneE17"}));
        client.Vehicle.Add("veh" + id, "DEFAULT_VEHTYPE", "Route0", 0, 0, 0, Byte.Parse("0"));
        id++;
        client.Vehicle.Add("veh" + id, "DEFAULT_VEHTYPE", "Route1", 0, 0, 0, Byte.Parse("0"));
        id++;
        client.Vehicle.Add("veh" + id, "DEFAULT_VEHTYPE", "Route2", 0, 0, 0, Byte.Parse("0"));
        id++;        
    }

    private void moveVehicles(String id)
    {
        String carID = "veh" + id;
        if (client.Vehicle.GetAccel(carID).Result == ResultCode.Success)
        {
            car3.GetComponent<Transform>().eulerAngles = new Vector3(car3.GetComponent<Transform>().eulerAngles.x, float.Parse(client.Vehicle.GetAngle(carID).Content.ToString()), car3.GetComponent<Transform>().eulerAngles.z);
            car3.GetComponent<Transform>().position = new Vector3((float)(client.Vehicle.GetPosition(carID).Content.X), car3.GetComponent<Transform>().position.y < 0 ? 0.07F : car3.GetComponent<Transform>().position.y, (float)(client.Vehicle.GetPosition(carID).Content.Y));
        }
        else
        {
            String carRoute = "Route" + id;
            client.Vehicle.Add(carID, "DEFAULT_VEHTYPE", "Route2", 0, 0, 0, Byte.Parse("0"));
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
            /*Vector2 newUnityPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
            Vector2 differenceUnity = newUnityPosition - oldUnityPosition;
            double lengthUnity = Math.Sqrt(differenceUnity.x * differenceUnity.x + differenceUnity.y * differenceUnity.y);
            Vector2 differenceSim = newSimPosition - oldSimPosition;
            double lengthSim = Math.Sqrt(differenceSim.x * differenceSim.x + differenceSim.y * differenceSim.y);
            //Am Anfang hat die Geschwindigkeit einen negative Defaut-Wert. Ebenso die Beschleunigung
            //if (lengthUnity >= lengthSim || step < 3 || Math.Abs(car.GetComponent<Transform>().position.x - (float)client.Vehicle.GetPosition("veh0").Content.X) < 0.1 && Math.Abs(car.GetComponent<Transform>().position.z - (float)client.Vehicle.GetPosition("veh0").Content.Y) < 0.1)
            //{                
                car.GetComponent<Transform>().position = new Vector3(float.Parse(client.Vehicle.GetPosition("veh0").Content.X.ToString()), car.GetComponent<Transform>().position.y, float.Parse(client.Vehicle.GetPosition("veh0").Content.Y.ToString()));
                oldAngle = float.Parse(client.Vehicle.GetAngle("veh0").Content.ToString());
                oldSimPosition = new Vector2((float)client.Vehicle.GetPosition("veh0").Content.X, (float)client.Vehicle.GetPosition("veh0").Content.Y);
                client.Control.SimStep(time);                
                step++;
                newSimPosition = new Vector2((float)client.Vehicle.GetPosition("veh0").Content.X, (float)client.Vehicle.GetPosition("veh0").Content.Y);
                oldUnityPosition = GameObject.FindGameObjectWithTag("Player").transform.position;                            
            //}
            float acc = float.Parse((client.Vehicle.GetAcceleration("veh0").Content).ToString());
            float newAngle = float.Parse(client.Vehicle.GetAngle("veh0").Content.ToString());
            float maxAcc = float.Parse(client.Vehicle.GetAccel("veh0").Content.ToString());*/



            //carController.Move(0, (acc + maxAcc) / (maxAcc * 2), (acc + maxAcc) / (maxAcc * 2), 0);
            client.Control.SimStep(time);
            step++;

            if(step==5)
            {
                client.Vehicle.Add("veh" + id, "DEFAULT_VEHTYPE", "Route2", 0, 0, 0, Byte.Parse("0"));
                id++;
            }

            if (client.Vehicle.GetAccel("veh0").Result == ResultCode.Success)
            {
                car0.GetComponent<Transform>().eulerAngles = new Vector3(car0.GetComponent<Transform>().eulerAngles.x, float.Parse(client.Vehicle.GetAngle("veh0").Content.ToString()), car0.GetComponent<Transform>().eulerAngles.z);
                car0.GetComponent<Transform>().position = new Vector3((float)(client.Vehicle.GetPosition("veh0").Content.X), car0.GetComponent<Transform>().position.y < 0 ? 0.07F : car0.GetComponent<Transform>().position.y, (float)(client.Vehicle.GetPosition("veh0").Content.Y));
            } else
            {
                client.Vehicle.Add("veh0", "DEFAULT_VEHTYPE", "Route0", 0, 0, 0, Byte.Parse("0"));
            }

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
           
            moveVehicles("3");
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
