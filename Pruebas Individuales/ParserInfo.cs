using System;
using System.IO;

namespace Parser
{
	class MainClass
	{
		enum Medio {Ethernet, Fibra, Wireless, Serial};



		public static void Main (string[] args)
		{

			if (loadData(@"archivo.txt") != null)
				Console.WriteLine ("Archivo correcto");
			else
				Console.WriteLine ("Archivo incorrecto");
			
			Console.ReadKey();

		}

		enum stateRouter {glob,conf,rou,inte};
		public Info loadData(string source){
			Info data = new Info ();
			string text = System.IO.File.ReadAllText(@"archivo.txt");
			string[] code = text.Split ("\n");
			string[] utileria = {""};
			int index = -1;
			stateRouter estado = stateRouter.glob;
			Interf interfaz = new Interf ();
			while (++index < code.Length) {
				text = code [index];
				if (text[0] == '#')
					continue;
				utileria = text.Split (" ");
				switch (estado) {

				case stateRouter.glob:
					if (utileria.Length < 2)
						return null;
					if (utileria [0] [0] == 'c' && utileria [1] [0] == 't')
						estado = stateRouter.conf;
					break;
				case stateRouter.conf:
					if (utileria.Length < 2)
						return null;
					if (code [0] == 'i') {
						switch (utileria [1] [0]) {
						case 'g':
							interfaz.medio = Medio.Ethernet;
							break;
						case 's': 
							interfaz.medio = Medio.Serial;
							break;
						default:
							interfaz.medio = Medio.Fibra;
							break;
						}
						estado = stateRouter.inte;
					} else if (utileria [0] == "router")
						estado = stateRouter.rou;
					else
						return null;
					break;


				case stateRouter.inte:
					switch (utileria [0]){
					case "exit":
						data.addInterf (new Interf (interfaz.medio, interfaz.ipAddress, interfaz.clockRate));
						estado = stateRouter.conf;
						break;
					case "ip":
						//Aqui va el código para poner la ip...
						if (utileria.Length != 4)
							return null;
						interfaz.ipAddress = utileria [2] + " " + utileria [3];
						break;
					case "clockrate":
						try {
							interfaz.clockRate = Convert.ToInt32(utileria[1]);
						} catch {
							return null;
						}
						break;
					default: 
						if (utileria.Length > 2)
							return null;
						break;
					}
					break;

				case stateRouter.rou:
					switch (utileria [0]) {
					case "exit":
						estado = stateRouter.conf;
						break;
					case "end":
						estado = stateRouter.glob;
						break;
					default:
						break;
					}
					break;
				default: break;
				}
			}
			return Info;
		}
		class Info
		{
			public string hostname = "";
			public ArrayList interfaces;

			public Info()
			{
				interfaces  = new ArrayList();
			}

			public void addInterf(Interf toAdd){ interfaces.Add(toAdd);}
			public void deleteInterf(int i){ interfaces.RemoveAt(i);}
			public void editInterf(int i, Medio medio, string ipAddress, int clockRate){ interfaces[i] = new Interf(medio, ipAddress, clockRate);}
		}

		class Interf
		{
			public Medio medio;
			public string ipAddress;
			public int clockRate;
			public Interf(){
				medio = Medio.Ethernet;
				ipAddress = "";
				clockRate = 0;
			}
			public Interf(Medio medio, string ipAddress, int clockRate)
			{
				this.medio = medio;
				this.ipAddress = ipAddress;
				this.clockRate = clockRate;
			}
		}
	}
}
