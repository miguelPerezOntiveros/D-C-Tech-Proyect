//tu mamá
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

enum Medio { Ethernet, Fibra, Wireless};

class router
{
	Texture2D tex;
	public int w, h;
	public Vector2 pos;
	public Info info;

	public router(Vector2 pos, Info info, Texture2D tex)
	{
		this.tex = tex;
		this.info = info;
		this.pos = pos;
		w = (int)(0.09f*Screen.width);
		h = (int)(0.12f*Screen.height);
	}
	public void draw()
	{ 
		GUI.DrawTexture(new Rect(pos.x, pos.y, w, h), tex); 
		info.hostname = GUI.TextField(new Rect(pos.x-w/2, pos.y+h, w*2, h/2), info.hostname, 30);
	}
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
	public Interf(Medio medio, string ipAddress, int clockRate)
	{
		this.medio = medio;
		this.ipAddress = ipAddress;
		this.clockRate = clockRate;
	}
}

class cable
{
	int pt = (int)(Screen.width*0.01f);
	public router a, b;
	public Medio medio;
	Texture tex;
	public cable(router a, router b, Texture2D tex, Medio medio)
	{
		this.medio = medio;
		this.tex = tex;
		this.a = a;
		this.b = b;
	}
	public int dist(Vector2 a, Vector2 b)
	{
		return (int)Mathf.Sqrt((a.x-b.x)*(a.x-b.x)+(a.y-b.y)*(a.y-b.y));
	}

	public void draw()
	{
		GUIUtility.RotateAroundPivot (180/Mathf.PI*Mathf.Atan((b.pos.y-a.pos.y)/(b.pos.x-a.pos.x)), new Vector2(a.pos.x+a.w/2, a.pos.y+a.h/2));
		GUI.DrawTextureWithTexCoords(new Rect(a.pos.x+a.w/2-pt/2, a.pos.y+a.h/2-pt/2, dist(a.pos, b.pos)*(-a.pos.x+b.pos.x)/Mathf.Abs((-a.pos.x+b.pos.x)), Screen.height*0.01f), tex, new Rect(0,0,dist(a.pos, b.pos)/20, 1));
		GUIUtility.RotateAroundPivot (-180/Mathf.PI*Mathf.Atan((b.pos.y-a.pos.y)/(b.pos.x-a.pos.x)), new Vector2(a.pos.x+a.w/2, a.pos.y+a.h/2));
	}
}




//////////////////////////////////////////






public class NewBehaviourScript : MonoBehaviour
{
	int pt = (int)(Screen.width*0.01f);
	ArrayList routers, switches, cables, stcs;
	Texture2D router, swit, cable, stc, puntero, borrar, routerP, switP, cableP, stcP, punteroP, borrarP, tipoLinea, lineaPunteada, lineaContinua, lineaWireless, seleccion, fondo, routerI, switchI, stcI, erasor;
	Medio medio;
	string str = "hola";
	router a = null, b = null;
	bool estirando;
	GUIStyle style;

	void Start () 
	{
		style = new GUIStyle();
		style.normal.textColor = Color.black;

		router = (Texture2D)Resources.Load("router");
		swit = (Texture2D)Resources.Load("swit");
		cable = (Texture2D)Resources.Load("cables");
		stc = (Texture2D)Resources.Load("stc");
		puntero = (Texture2D)Resources.Load("puntero");
		borrar = (Texture2D)Resources.Load("borrar");
		routerP = (Texture2D)Resources.Load("routerP");
		switP = (Texture2D)Resources.Load("switP");
		cableP = (Texture2D)Resources.Load("cablesP");
		stcP = (Texture2D)Resources.Load("stcP");
		punteroP = (Texture2D)Resources.Load("punteroP");
		borrarP = (Texture2D)Resources.Load("borrarP");
		routerI = (Texture2D)Resources.Load("routerI");
		switchI = (Texture2D)Resources.Load("switchI");
		stcI = (Texture2D)Resources.Load("stcI");
		erasor = (Texture2D)Resources.Load("erasor");

		lineaPunteada = (Texture2D)Resources.Load("lineaPunteada");
		lineaContinua = (Texture2D)Resources.Load("lineaContinua");
		lineaWireless = (Texture2D)Resources.Load("lineaWireless");
		fondo = (Texture2D)Resources.Load("fondo");
		routers = new ArrayList();
		switches = new ArrayList();
		cables = new ArrayList();
		stcs = new ArrayList();
	}

	Vector2 invierte(Vector2 v){ return new Vector2(v.x, Screen.height-v.y);}

	int edoM(Rect r)
	{
		if(r.Contains(invierte(Input.mousePosition)))
		{	
			if(mouseButtonUp) return 2; 
			return 1;
		}
		return 0;
	}

	bool boton(Rect r, Texture2D tex, Texture texP)
	{ 
		switch(edoM(r))
		{
			case 0: GUI.DrawTexture(r, tex); break;
			case 1: GUI.DrawTexture(r, texP); break;
			case 2: GUI.DrawTexture(r, tex); return true;
		}
		return false;
	}

	bool mouseButtonUp, mouse, mousePrev;
	void checkMouse()
	{
		mousePrev = mouse;
		mouse = Input.GetMouseButton(0);
		mouseButtonUp = mousePrev && !mouse;
	}

	public int dist(Vector2 a, Vector2 b)
	{
		return (int)Mathf.Sqrt((a.x-b.x)*(a.x-b.x)+(a.y-b.y)*(a.y-b.y));
	}
	bool turnoA = true;
	int min, distAux;
	Vector2 mousePos = new Vector2();

	Info loadInfo(string path)
	{
		return new Info();
	}

	string lectura = "nada";
	void OnGUI () 
	{
		checkMouse();
		GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), fondo);
	
		//Hacer o cambiar selección
		if(boton(new Rect(0,Screen.height*0.048f*0, Screen.width*0.036f, Screen.height*0.048f ), router, routerP)) {seleccion = router; estirando= false;}
		if(boton(new Rect(0,Screen.height*0.048f*1, Screen.width*0.036f, Screen.height*0.048f ), swit, switP)) {seleccion = swit; estirando= false;}
		if(boton(new Rect(0,Screen.height*0.048f*2, Screen.width*0.036f, Screen.height*0.048f ), cable, cableP)) {seleccion = cable; turnoA = true; estirando = false; tipoLinea = lineaPunteada; medio = Medio.Ethernet;}
		if(boton(new Rect(0,Screen.height*0.048f*3, Screen.width*0.036f, Screen.height*0.048f ), cable, cableP)) {seleccion = cable; turnoA = true; estirando = false; tipoLinea = lineaContinua; medio = Medio.Fibra;}
		if(boton(new Rect(0,Screen.height*0.048f*4, Screen.width*0.036f, Screen.height*0.048f ), cable, cableP)) {seleccion = cable; turnoA = true; estirando = false; tipoLinea = lineaWireless; medio = Medio.Wireless;}
		if(boton(new Rect(0,Screen.height*0.048f*5, Screen.width*0.036f, Screen.height*0.048f ), stc, stcP)) {seleccion = stc; estirando= false;}
		if(boton(new Rect(0,Screen.height*0.048f*6, Screen.width*0.036f, Screen.height*0.048f ), puntero, punteroP)) {seleccion = null; estirando=false;}
		if(boton(new Rect(0,Screen.height*0.048f*7, Screen.width*0.036f, Screen.height*0.048f ), borrar, borrarP)) 
		{
			using(StreamReader sr = new StreamReader(EditorUtility.OpenFilePanel("Load Configuration", "", "mike")))
			{
				int edoLectura = -1;
				while((lectura = sr.ReadLine()) != null)
				{
					if(lectura == "\\Router") edoLectura = 0; else
					if(lectura == "\\Switch") edoLectura = 1; else
					if(lectura == "\\Stc") edoLectura = 2; else
					if(lectura == "\\Cable") edoLectura = 3; else
					{
						if(edoLectura == 0) routers.Add(new router(
							new Vector2(
								int.Parse(lectura.Substring(0, lectura.IndexOf(" "))), 
								int.Parse(lectura.Substring(lectura.IndexOf(" ")+1))),
							loadInfo(lectura.Substring(lectura.IndexOf("  ")+2)),
							routerI
						  	)); else
						if(edoLectura == 1) ; else
						if(edoLectura == 2) ; else
						if(edoLectura == 3) ;
					}
				}
			}
		}
		if(boton(new Rect(0,Screen.height*0.048f*8, Screen.width*0.036f, Screen.height*0.048f ), borrar, borrarP)) seleccion = erasor;

		GUI.Label(new Rect(0, Screen.height*0.6f, 300, 300), lectura, style);

		//opción actual
		if(seleccion != null) GUI.DrawTexture(new Rect(invierte(Input.mousePosition).x, invierte(Input.mousePosition).y, Screen.width*0.12f, Screen.width*0.1f), seleccion);
		if(mouseButtonUp && !new Rect(0,0,Screen.width*0.036f, Screen.height*8*0.048f).Contains(invierte(Input.mousePosition)))
		{ 
			if(seleccion == router) {routers.Add(new router(invierte(Input.mousePosition), new Info(), routerI));}
			if(seleccion == swit) {switches.Add(new router(invierte(Input.mousePosition), new Info(), switchI));}
			if(seleccion == stc) {stcs.Add(new router(invierte(Input.mousePosition), new Info(), stcI));}

			if(seleccion == cable)
			{
				min = -1;
				foreach(router r in routers)
				{
					distAux = dist(invierte(Input.mousePosition), r.pos);
					if ((!turnoA && r!=a || turnoA) && (distAux<100 && (distAux<min || min == -1)))
					{
						if(turnoA) 	{a = r;}
						else 		{b = r;}
						min = distAux;
					}
				}
				foreach(router r in switches)
				{
					distAux = dist(invierte(Input.mousePosition), r.pos);
					if ((!turnoA && r!=a || turnoA) && (distAux<100 && (distAux<min || min == -1)))
					{
						if(turnoA) 	{a = r;}
						else 		{b = r;}
						min = distAux;
					}
				}
				foreach(router r in stcs)
				{
					distAux = dist(invierte(Input.mousePosition), r.pos);
					if ((!turnoA && r!=a || turnoA) && (distAux<100 && (distAux<min || min == -1)))
					{
						if(turnoA) 	{a = r;}
						else 		{b = r;}
						min = distAux;
					}
				}
				if(min != -1)
				{
					if(turnoA) estirando = true;
					else {cables.Add(new cable(a, b, tipoLinea, medio)); estirando=false;}
					turnoA = !turnoA;
				}	
			}

			if (seleccion == erasor)
			{
				a = null;
				min = -1;
				foreach(router r in routers)
				{
					distAux = dist(invierte(Input.mousePosition), r.pos);
					if (distAux<100 && (distAux<min || min == -1))
					{
						a = r;
						min = distAux;
					}
				}
				foreach(router r in switches)
				{
					distAux = dist(invierte(Input.mousePosition), r.pos);
					if (distAux<100 && (distAux<min || min == -1))
					{
						a = r;
						min = distAux;
					}
				}
				foreach(router r in stcs)
				{
					distAux = dist(invierte(Input.mousePosition), r.pos);
					if (distAux<100 && (distAux<min || min == -1))
					{
						a = r;
						min = distAux;
					}
				}

				routers.Remove(a);
				switches.Remove(a);
				stcs.Remove(a);
				for(int i = 0; i < cables.Count; i++)
					if (((cable)cables[i]).a == a || ((cable)cables[i]).b == a)
					{
						cables.Remove(cables[i]);
						i--;
					}
			}

			if(seleccion == null) //puntero
			{
				a = null;
				min = -1;
				foreach(router r in routers)
				{
					distAux = dist(invierte(Input.mousePosition), r.pos);
					if (distAux<100 && (distAux<min || min == -1))
					{
						a = r;
						min = distAux;
					}
				}
				foreach(router r in switches)
				{
					distAux = dist(invierte(Input.mousePosition), r.pos);
					if (distAux<100 && (distAux<min || min == -1))
					{
						a = r;
						min = distAux;
					}
				}
				foreach(router r in stcs)
				{
					distAux = dist(invierte(Input.mousePosition), r.pos);
					if (distAux<100 && (distAux<min || min == -1))
					{
						a = r;
						min = distAux;
					}
				}
			}
		}

		if(estirando)
		{
			mousePos = invierte(Input.mousePosition);
			GUIUtility.RotateAroundPivot (180/Mathf.PI*Mathf.Atan((mousePos.y-a.pos.y)/(mousePos.x-a.pos.x)), new Vector2(a.pos.x+a.w/2, a.pos.y+a.h/2));
			GUI.DrawTextureWithTexCoords(
												new Rect(a.pos.x+a.w/2-pt/2, a.pos.y+a.h/2-pt/2, dist(a.pos, mousePos)*(-a.pos.x+mousePos.x)/Mathf.Abs((-a.pos.x+mousePos.x)), Screen.height*0.01f), 
												tipoLinea, 
												new Rect(0,0,dist(a.pos, mousePos)/20, 1)
										);
			GUIUtility.RotateAroundPivot (-180/Mathf.PI*Mathf.Atan((mousePos.y-a.pos.y)/(mousePos.x-a.pos.x)), new Vector2(a.pos.x+a.w/2, a.pos.y+a.h/2));
		}

		if(seleccion == null && a!= null)
		{
			if(Input.GetKey(KeyCode.RightArrow)) a.pos.x ++;
			if(Input.GetKey(KeyCode.LeftArrow)) a.pos.x --;
			if(Input.GetKey(KeyCode.DownArrow)) a.pos.y ++;
			if(Input.GetKey(KeyCode.UpArrow)) a.pos.y --;
		}

		//Dibujar topología actual		
		foreach(router r in routers) r.draw();		
		foreach(router r in switches) r.draw();
		foreach(router r in stcs) r.draw();
		foreach(cable c in cables) c.draw();

		//archivo
		if(Input.GetKey(KeyCode.Return))
		{
			string toWrite = "//ROUTERS\n";
			foreach (router r in routers)
				toWrite += r.info.hostname+"\n";
		
			toWrite += "\n//SWITCHES\n";
			foreach (router r in switches)
				toWrite += r.info.hostname+"\n";

			toWrite += "\n//STCS\n";
			foreach (router r in stcs)
				toWrite += r.info.hostname+"\n";

			toWrite += "\n//CABLES\n";
			foreach (cable c in cables)
				toWrite += c.medio + " '" + c.a.info.hostname + "' - '" + c.b.info.hostname + "'\n";

			using(StreamWriter sw = new StreamWriter("archivo.txt"))
			{
				sw.WriteLine(toWrite);
			}
		}
	}
}
