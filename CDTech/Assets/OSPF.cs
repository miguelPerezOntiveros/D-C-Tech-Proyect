using UnityEngine;
using System.Collections;
using System.IO;

public class OSPF : MonoBehaviour {

	string openConfig, script, rProtocolLbl;
	// Use this for initialization
	void Start () 
	{
		openConfig = "enable\n";
		rProtocolLbl = "#Routing protocol\n";
		script = "";

		script += openConfig;

		script += rProtocolLbl;

		script +="configure terminal\n" +
					"router ospf\n" +
						"router-id " + "123.456.789.000" + "\n" +
				"network " + "123.456.789.000" + " area " + "123.456.789.000" + "\n" +
						"end\n";
		script += "#otra seccion\n";

		script = configureInterface ("10.10.2.1", "10.4.0.1", "0.0.0.0", script, rProtocolLbl);

		script = configureInterface ("10.10.2.1", "10.10.2.1", "0.0.0.6", script, rProtocolLbl);

		File.AppendAllText("OSPFscript.txt", script);

		UnityEngine.Debug.Log ("Script: \n" + script);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	string configureInterface (string routerID, string network, string area, string script, string rpLabel)
	{
		int startRP = script.IndexOf (rpLabel);


		if (startRP >= 0) 
		{
			bool previousNetworks = false;

			string prevSections = script.Substring(0,startRP);
			string rpSection;
			string nextSections="";

			int nextSStart = script.IndexOf('#', startRP+1);

			if (nextSStart > 0)
			{
				if (script.IndexOf("end", startRP) >0)
				{
					nextSStart = script.IndexOf("end", startRP);
					previousNetworks = true;
				}
				rpSection = script.Substring (startRP, nextSStart - startRP); 
				nextSections = script.Substring(nextSStart);
			}
			else
			{
				if (script.IndexOf("end", startRP) >0)
				{
					nextSStart = script.IndexOf("end", startRP);

					rpSection = script.Substring(startRP, nextSStart - startRP);
					nextSections = script.Substring(nextSStart);
					previousNetworks = true;

				}
				else
					rpSection = script.Substring(startRP);
			}

			if (previousNetworks)
			{
				rpSection += "network " + network + " area " + area + "\n";
			}
			else
			{
				rpSection += "configure terminal\n" +
							"router ospf\n" +
								"router-id " + routerID + "\n" +
								"network " + network + " area " + area + "\n" +
								"end\n";
			}

			string modifiedScript = prevSections + rpSection + nextSections;

			return modifiedScript;	
		}
		else
			return  "#Routing protocol\n" + 
					"configure terminal\n" +
					"router ospf\n" +
					"router-id " + routerID + "\n" +
					"network " + network + " area " + area + "\n" +
					"end\n";



	}


}
