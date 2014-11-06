using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

enum Medio { Ethernet, Fibra, Wireless};

//
//
//
//

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
		float scale = 140 / 139;
		w = (int)(0.09f*Screen.width);
		h = (int)(w * scale);
	}
	public void draw()
	{ 
		GUI.DrawTexture(new Rect(pos.x, pos.y, w, h), tex); 
		info.hostname = GUI.TextField(new Rect(pos.x-w/2, pos.y+h, w*2, h/2), info.hostname, 30);
	}



}

//
//
//
//

class Info
{
	public string hostname = "";
	public ArrayList interfaces;
	
	public Info()
	{
		interfaces  = new ArrayList();
	}
	public void print(){
		Debug.Log (hostname + " host " + interfaces.Count+ "Interfaces" );
		foreach (Interf inter in interfaces) {
			inter.cadena();
		}
	}
	public string config(){
		string salida = "hostname " + hostname;
		for (int i = 0; i< interfaces.Count; i++){
			Interf a = (Interf)interfaces[i];
			salida += a.config();
		}
		return salida;
		
	}
	public void addInterf(Interf toAdd){ interfaces.Add(toAdd);}
	public void deleteInterf(int i){ interfaces.RemoveAt(i);}
	public void editInterf(int i, Medio medio, string ipAddress, int clockRate){ interfaces[i] = new Interf(medio, ipAddress, clockRate);}
}

//
//
//
//

class Interf
{
	public Medio medio;
	public string ipAddress;
	public int clockRate;
	public string name = "s0/0";
	public Interf(){}
	public Interf(Medio medio, string ipAddress, int clockRate)
	{
		this.medio = medio;
		this.ipAddress = ipAddress;
		this.clockRate = clockRate;
	}
	public void cadena(){
		Debug.Log ("IP de la interfaz: "+ ipAddress);
	}
	public string config(){
		return "interface " + name+ " \n" + "ip address " + ipAddress+ "\n no shut\n"; 
	}
}

//
//
//
//

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

//
//
//
//




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

	Rect cableWindowRect, dispWindowRect;
	bool windowCableAct, windowDispAct, showMenu, showRutas, showRutasE, showRutasD, showInterfaces, showIntFa, showIntSe, showIntAgr;
	public GUIStyle cableWdw, dispWdw, btnCStyle, btnWCabS, btnWCabC, btnWCabR, menuTitle, menuSubTitle,menuText, menuSimpleText;
	public GUIStyle btnPlusH, btnMinH, btnPlusS, btnMinS, textFieldStyle, btnLoadStyle, btnExportStyle, btnDel, btnEdit, btnAgregar, btnModificar;
	GUIStyle btnPMHeader, btnPMRutas, btnPMRutasE, btnPMRutasD, btnPMInterfaces, btnPMIntFa, btnPMIntSe, btnPMIntAgr;
	Texture2D btnClose, btnCloseP, btnVentanaCable, btnModificarI;
	Texture2D menuHeader, backHeader, plusHeader, btnLoadI, btnExportI, subMenuHeader, otherMenuHeader, iconMInt, iconMRut, btnMinSub, btnDelI, btnAgregarI;

	string hostname, mac, ip, mascara, nombre, red, salto, red2, protocolo, mascara2, clockrate, tipo;

	float inicioMenuRutas, inicioMenuInterfaces, inicioMenuInterfacesSinD, inicioMenuInterfacesConD;

	void Start () 
	{
		hostname = "Router 1"; 
		mac= "06-00-00-00-00-00"; 
		ip= "10.0.0.1"; 
		mascara= ""; 
		red = "";
		salto = "";
		red2 = "";
		protocolo = "";
		mascara2 = "255.255.255.0";
		clockrate = "2000000";
		tipo = "Serial";
		nombre = "s0/2";
	
		btnPMHeader = btnMinH;
		btnPMRutas = btnMinH;
		btnPMRutasE = btnMinS;
		btnPMRutasD = btnMinS;
		btnPMInterfaces = btnPlusH;
		btnPMIntFa = btnMinS;
		btnPMIntSe = btnMinS;
		btnPMIntAgr= btnMinS;

		showMenu = true;
		showRutas = true;
		showRutasE = true;
		showRutasD = true;
		showInterfaces = false;
		showIntFa = true;
		showIntSe = false;
		showIntAgr = false;

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

		btnClose = (Texture2D)Resources.Load("btnClose");
		btnCloseP = (Texture2D)Resources.Load("btnClose_h");
		btnVentanaCable = (Texture2D)Resources.Load("windowCable_CruzadoH");

		lineaPunteada = (Texture2D)Resources.Load("lineaPunteada");
		lineaContinua = (Texture2D)Resources.Load("lineaContinua");
		lineaWireless = (Texture2D)Resources.Load("lineaWireless");
		fondo = (Texture2D)Resources.Load("fondo");
		routers = new ArrayList();
		switches = new ArrayList();
		cables = new ArrayList();
		stcs = new ArrayList();

		windowCableAct = false;
		windowDispAct = false;

		menuHeader = (Texture2D)Resources.Load("MenuDer/headerConfig");
		backHeader = (Texture2D)Resources.Load("MenuDer/backgroundMenu");
		plusHeader = (Texture2D)Resources.Load("MenuDer/+btnHeader");
		btnLoadI = (Texture2D)Resources.Load("MenuDer/btnLoad_N");
		btnExportI = (Texture2D)Resources.Load("MenuDer/btnExport_N");

		otherMenuHeader = (Texture2D)Resources.Load("MenuDer/headeOther");
		subMenuHeader = (Texture2D)Resources.Load("MenuDer/subHeader");
		iconMInt =(Texture2D)Resources.Load("MenuDer/iconEntrada");
		iconMRut = (Texture2D)Resources.Load ("MenuDer/iconWorld");
		btnMinSub= (Texture2D)Resources.Load("MenuDer/+btnSubHeader");
		btnDelI =(Texture2D)Resources.Load("MenuDer/btnDelete");
		btnAgregarI =(Texture2D)Resources.Load("MenuDer/btnAgregar_H");
		btnModificarI =(Texture2D)Resources.Load("MenuDer/btnModificar_H");
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
	enum stateRouter {glob,conf,rou,inte};
		
	static Info loadInfo(string source)
	{
		//return new Info();
		
		Info data = new Info ();
		string[] code;
		string text;
		using(StreamReader sr = new StreamReader(source))
		{	
			code = (sr.ReadToEnd()).Split('\n');
			Debug.Log(string.Join("Linea ", code));
			text = sr.ReadToEnd();
		}
		
		string[] utileria = {""};
		int index = -1;
		stateRouter estado = stateRouter.glob;
		Interf interfaz = new Interf();
		while (++index < code.Length) {
			text = code [index];
			utileria = text.Split (' ');
			if (text == "")
				continue;
			if (text[0] != '#')
			switch (estado) {
				case stateRouter.glob:
				if (utileria.Length > 1)
					if (utileria [0] [0] == 'c' && utileria [1] [0] == 't')
						estado = stateRouter.conf;
				break;
				case stateRouter.conf:
				if (utileria [0] == "exit") {
					estado = stateRouter.glob;
					break;
				}
				if (text [0] == 'i') {
					
					estado = stateRouter.inte;
					if (utileria.Length < 2) {
						//Console.WriteLine ("La interfaz, su nombre");
						return null;
					}
					interfaz.name = utileria[1];
					switch (utileria [1] [0]) {
					case 'g':
						interfaz.medio = Medio.Ethernet;
						break;
					case 's': 
						interfaz.medio = Medio.Wireless;
						break;
					default:
						interfaz.medio = Medio.Fibra;
						break;
					}
					break;
				} 
				if (text [0] == 'r') {
					estado = stateRouter.rou;
					break;
				}
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
					if (utileria.Length < 4) {
						/////////
						//Console.WriteLine ("La ip");
						return null;
					}
					interfaz.ipAddress = utileria [2] + " " + utileria [3];
					break;
				case "clockrate":
					try {
						interfaz.clockRate = int.Parse(utileria[1]);
					} catch {
						///////////
						//Console.WriteLine ("ClockRATE");
						return null;
					}
					break;
				default: 
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
		Debug.Log ("Terminado");
		data.print ();
		return data;
		
	}


	float [] scaleImg (Texture2D img, float width, float height)
	{
		//
		//Regresa un vector con el tama;o de la imagen tal que esta mantenga su escala y no se vea pixeleada
		//Se va a escalar al ancho indicado si este es mayor que el alto (y viceversa)
		//
		float [] size = new float[2];

		if (width > height) {
			float scale =(float)((float)(img.height) / (float)(img.width));
			size [0] = width;
			size [1] = width * scale;
		} else {
			float scale =(float)((float)(img.width) / (float)(img.height));
			size [1] = height;
			size [0] = height * scale;
		}
		return size;
	}

	string lectura = "nada";

	void windowCable(int windowID)
	{

		float [] bC = scaleImg (btnClose, cableWindowRect.width * .11f, 0);
		if (GUI.Button (new Rect (cableWindowRect.width-bC[0], 0, bC[0], bC[1]),"" , btnCStyle)){
			windowCableAct=false;
		}

		float [] bCab = scaleImg (btnVentanaCable, cableWindowRect.width, cableWindowRect.height*.25f); 

		if (GUI.Button (new Rect (0, bCab[1], bCab[0], bCab[1]),"" , btnWCabS))
		{
			seleccion = cable; 
			turnoA = true; 
			estirando = false; 
			tipoLinea = lineaWireless; 
			medio = Medio.Wireless;
			 
			windowCableAct = false;
		}
		if (GUI.Button (new Rect (0, bCab[1]*2, bCab[0], bCab[1]),"" , btnWCabC))
		{
			seleccion = cable; 
			turnoA = true; 
			estirando = false; 
			tipoLinea = lineaPunteada; 
			medio = Medio.Ethernet;
			windowCableAct = false;
		}
		if (GUI.Button (new Rect(0, bCab[1]*3, bCab[0], bCab[1]),"" , btnWCabR))
		{
			seleccion = cable; 
			turnoA = true; 
			estirando = false; 
			tipoLinea = lineaContinua; 
			medio = Medio.Fibra;
			windowCableAct = false;
		}


	}
	void OnGUI () 
	{
		checkMouse();
		GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), fondo);
	
		//Hacer o cambiar selección
		float[] tamBtn = scaleImg (router, 0, Screen.height * 0.055f);

		if(boton(new Rect(0,tamBtn[1]*0, tamBtn[0], tamBtn[1] ), router, routerP)) 
		{seleccion = router; estirando= false;}
		if(boton(new Rect(0,tamBtn[1]*1, tamBtn[0], tamBtn[1] ), swit, switP)) 
		{seleccion = swit; estirando= false;}
		if(boton(new Rect(0,tamBtn[1]*2, tamBtn[0], tamBtn[1]), cable, cableP)) 
		{
			windowCableAct = true;
		}
		if (windowCableAct) 
		{
			float [] bCab = scaleImg (btnVentanaCable, cableWindowRect.width, cableWindowRect.height*.25f); 
			cableWindowRect = new Rect(tamBtn[0], tamBtn[1]*2, Screen.width*0.2f, bCab[1]*4 );
			cableWindowRect = GUI.Window(0, cableWindowRect, windowCable, "Seleccionar cable", cableWdw);
		}
		if(boton(new Rect(0,tamBtn[1]*3, tamBtn[0], tamBtn[1]), stc, stcP)) 
		{seleccion = stc; estirando= false;}
		if(boton(new Rect(0,tamBtn[1]*4, tamBtn[0], tamBtn[1] ), puntero, punteroP)) 
		{seleccion = null; estirando=false;}
		float[] borrarTam = scaleImg (borrar, tamBtn[0], 0);


		if(boton(new Rect(0,tamBtn[1]*5, borrarTam[0], borrarTam[1] ), borrar, borrarP)) 
			seleccion = erasor;


		//
		//
		//
		//Proceso para dibujar menu derecha
		//
		//
		//

		//
		//Inicia proceso para dibujar el header principal
		//

		//se obtiene la escala la imagen de fondo del header para que su ancho cubra un 22% del ancho de la pantalla
		float[] menuDerHeader = scaleImg (menuHeader, Screen.width*0.22f, 0); 

		//se dibuja la imagen de fondo del header pegada a la derecha y arriba de la pantalla
		GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0], 0, menuDerHeader[0], menuDerHeader[1]),menuHeader);
		//etiqueta con el nombre del aparato (hostname)
		GUI.Label (new Rect (Screen.width - (menuDerHeader [0] *.66f), (menuDerHeader [1] * 0.15f), menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), hostname, menuTitle);
		//etiqueta con el modelo del aparato
		GUI.Label (new Rect (Screen.width - (menuDerHeader [0] *.66f), (menuDerHeader [1] * 0.5f), menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "Modelo R1800", menuSubTitle);

		//se obtiene la escala para el boton de + que es usado para minimizar o maximizar el menu
		float[] btnPlusHeader = scaleImg (plusHeader, Screen.width*0.02f, 0);

		//se crea el boton
		if (GUI.Button(new Rect (Screen.width - btnPlusHeader[0] *1.5f, (menuDerHeader [1] * 0.3f  ), btnPlusHeader[0],btnPlusHeader[1]), "", btnPMHeader))
		{
			if (showMenu) //showMenu indica si el boton esta en -, es decir si el menu esta descomprimido
			{ 
				//si showMenu = true y se da click en el boton entonces se esta pidiendo que se comprima
				btnPMHeader = btnPlusH; //se cambia la imagen del boton para que muestre un +
				showMenu = false; //showMenu = falso, indicando que el menu esta comprimido
			}
			else
			{
				//si showMenu = false y se da click en el boton se esta pidiendo que se descomprima
				btnPMHeader = btnMinH ; //se cambia la imagen del boton para que muestre un -
				showMenu = true; //showMenu = true, indicando que el menu esta descomprimido
			}
		}

		//
		//Termina proceso de dibujae header principal
		//

		if (showMenu) //si el menu lateral esta descomprimido se dibuja el contenido del header y los submenus
		{
			//
			//Inicia el proceso de dibujar el contenido del header principal
			//

			//se dibuja el fondo blanco partiendo de la posicion en Y del header
			GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0], menuDerHeader[1], menuDerHeader[0], menuDerHeader[1]*1.7f),backHeader);

			//Etiqueta con el texto "hostname", solo usada para indicar que poner en el textfield siguiente
			GUI.Label(new Rect ( Screen.width - menuDerHeader[0] * .75f, menuDerHeader[1]*1.1f, menuDerHeader[0], menuDerHeader[1]*.2f), "Hostname", menuText);

			//Textfield que recibe el hostname, al momento de escribir en el se modifica el valor del string hostname y en consecuencia
			//se cambia el hostname mostrado en el header principal *seccion anterior*
			hostname = GUI.TextField(new Rect ( Screen.width - menuDerHeader[0] * .75f, menuDerHeader[1]*1.4f, menuDerHeader[0]*.66f, menuDerHeader[1]*.25f), hostname, textFieldStyle );

			//Etiqueta Starup config
			GUI.Label(new Rect ( Screen.width - menuDerHeader[0] * .75f, menuDerHeader[1]*1.8f, menuDerHeader[0], menuDerHeader[1]*.2f), "Startup config", menuText);

			//se obtiene la escala para el boton de exportar script
			float[] btnExportSize = scaleImg (btnExportI, 0, menuDerHeader[1]*0.25f);

			//boton para exportar el script del startupconfig
			if (GUI.Button(new Rect (Screen.width-btnExportSize[0], menuDerHeader[1]*1.8f, btnExportSize[0], btnExportSize[1]), "", btnExportStyle))
			{
				//si se da click se exporta el script

				//
				//TO DO
				//
			}

			//se obtiene la escala para el boton de cargar script
			float[] btnLoadSize = scaleImg (btnLoadI, 0, menuDerHeader[1]*0.25f);

			//boton para cargar el script del startupconfig
			if (GUI.Button(new Rect (Screen.width - btnLoadSize[0] - btnExportSize[0], menuDerHeader[1]*1.8f, btnLoadSize[0], btnLoadSize[1] ), "", btnLoadStyle))
			{
				//si se da click se abre una ventana para seleccionar el script
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
							if(edoLectura == 0) 
							routers.Add(new router(
								new Vector2(
									int.Parse(lectura.Substring(0, lectura.IndexOf(" "))), 
									int.Parse(lectura.Substring(lectura.IndexOf(" ")+1, lectura.IndexOf("  ")-lectura.IndexOf(" ")))),
								loadInfo(lectura.Substring(lectura.IndexOf("  ")+2)),
								routerI
							  	)
							); else
							if(edoLectura == 1) 
							switches.Add(new router(
								new Vector2(
									int.Parse(lectura.Substring(0, lectura.IndexOf(" "))), 
									int.Parse(lectura.Substring(lectura.IndexOf(" ")+1, lectura.IndexOf("  ")-lectura.IndexOf(" ")))),
								loadInfo(lectura.Substring(lectura.IndexOf("  ")+2)),
								switchI
								)
							); else
							if(edoLectura == 2) ; else
							if(edoLectura == 3) ;
						}
					}
					
				}//termina using
			}//termina boton para cargar el script del startupconfig

			//etiqueta running config
			GUI.Label(new Rect ( Screen.width - menuDerHeader[0] * .75f, menuDerHeader[1]*2.2f, menuDerHeader[0], menuDerHeader[1]*.2f), "Running config", menuText);

			//boton para exportar script del runningconfig
			if (GUI.Button(new Rect (Screen.width-btnExportSize[0], menuDerHeader[1]*2.15f, btnExportSize[0], btnExportSize[1]), "", btnExportStyle))
			{
				//metodo exportar script del running config

				//
				//TO DO
				//
			}//termina boton para exportar script del runningconf

			//boton para cargar script del runningconfig
			if (GUI.Button(new Rect (Screen.width - btnLoadSize[0] - btnExportSize[0], menuDerHeader[1]*2.15f, btnLoadSize[0], btnLoadSize[1] ), "", btnLoadStyle))
			{
				//metodo cargar script del running config
				
				//
				//TO DO
				//
			}//termina boton para cargar script del runningconfig

			//
			//Termina el proceso de dibujar el contenido del header principal
			//

			//
			//Inicia proceso para dibujar submenu de rutas
			//

			//Variable para almacenar inicio en Y del submenu de rutas (y evitar realizar este calculo en cada elemento GUI posterior)
			inicioMenuRutas = menuDerHeader[1]*2.7f; //menuDerHeader[1] = posicion en Y del header, 2.7 = alto del fondo blanco del contenido del header

			//se obtiene la escala para dibujar el fondo del header de un submenu
			float[] otherHeaderSize = scaleImg (otherMenuHeader, menuDerHeader[0], 0);

			//se dibuja el fondo del header del submenu
			GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0], inicioMenuRutas, otherHeaderSize[0], otherHeaderSize[1]),otherMenuHeader);

			//se obtiene la escala para dubujar el icono de ruter que se dibuja en el header del submenu
			float[] iconRutSize = scaleImg (iconMRut, menuDerHeader[0]*0.14f, 0);

			//se dibuja el icono
			GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0]*.66f - iconRutSize[0]*1.2f, inicioMenuRutas + (otherHeaderSize[1]- iconRutSize[1])*0.4f, iconRutSize[0], iconRutSize[1]),iconMRut);

			//label indicando el titulo del submenu "Rutas"
			GUI.Label (new Rect (Screen.width - (menuDerHeader [0] *.66f), inicioMenuRutas + iconRutSize[1]*0.4f, menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "Rutas" , menuTitle);


			//boton de + para comprimir/descomprimir el contenido de la seccion rutas
			if (GUI.Button(new Rect (Screen.width - btnPlusHeader[0] *1.5f, inicioMenuRutas + iconRutSize[1]*0.4f, btnPlusHeader[0],btnPlusHeader[1]), "", btnPMRutas))
			{
				if (showRutas) //si showrutas = true la seccion estaba descomprimida
				{
					//se comprime la seccion y se cambia el icono del boton a un signo de + para indicar que puede ser descomprimida
					btnPMRutas = btnPlusH ;
					showRutas = false;

				}
				else //si showrutas=false la seccion estaba comprimida
				{
					//se descomprime la seccion y se cambia el icono del boton a un signo de - para indicar que puede ser comprimida
					btnPMRutas = btnMinH;
					showRutas = true;

					//se revisa si se estaba mostrando la seccion de interfaces
					//solo una seccion (interfaces o rutas) puede estar descomprimida a la vez
					if (showInterfaces)
					{
						//si Interfaces estaba descomprimida y se descomprime Rutas entonces se comprime Interfaces
						showInterfaces=false;
						btnPMInterfaces = btnPlusH;
					}
				}
			}// termina boton de + para comprimir/descomprimir el contenido de la seccion rutas

			//se calcula la escala del subheader presente en el contenido de la seccion de rutas
			float[] subHeaderSize = scaleImg (subMenuHeader, menuDerHeader[0], 0);

			//se calcula la escala del boton para comprimir/descomprimir una seccion dentro del submenu de rutas
			float[] plusSubSize = scaleImg (btnMinSub, btnPlusHeader[0]*0.5f, 0);

			if (showRutas) //si el menu de rutas esta descomprimido se muestra su contenido
			{	
				//
				// Inicia dibujar header de rutas estaticas
				//

				//se dibuja el fondo de una de las subsecciones (rutas estaticas) del contenido
				GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0], inicioMenuRutas + otherHeaderSize[1], subHeaderSize[0], subHeaderSize[1]),subMenuHeader);
				//se dibuja el label con el titulo de una de las subsecciones (rutas estaticas) del contenido
				GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, (menuDerHeader [1] * 2.75f + otherHeaderSize[1]), menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "Estaticas" , menuSubTitle);

				//boton de + para comprimir/descomprimir el contenido de la seccion de rutas estaticas
				if (GUI.Button(new Rect(Screen.width - btnPlusHeader[0] *.5f  - plusSubSize[0],(menuDerHeader [1] * 2.75f + otherHeaderSize[1]),plusSubSize[0], plusSubSize[1]),"", btnPMRutasE))
				{
					if (showRutasE) //si rutas estaticas estaba descomprimido
					{
						//se comprime y se cambia el boton a un + para indicar que se puede descomprimir
						btnPMRutasE = btnPlusS;
						showRutasE = false;
					}
					else // si rutas estaticas estaba comprimido
					{
						//se descomprime y se cambia el boton a un - para indicar que se puede comprimir
						btnPMRutasE = btnMinS ;
						showRutasE = true;
					}
				}//termina boton de + para comprimir/descomprimir el contenido de la seccion de rutas estaticas


				//
				// Termina dibujar header de rutas estaticas
				//


				//se almacena el fin de la seccion de contenido de rutas estaticas para evitar calcularlo despues
				float yAddRutasE = (menuDerHeader[1]*2.5f);

				if (showRutasE) //si la subseccion de rutas estaticas esta descomprimida se muestra su contenido
				{
					//
					// Inicia dibujar contenido de rutas estaticas
					//

					//se almacena el inicio del contenido de rutas estaticas para evitar calcularlo posteriormente
					float yRutasInit = inicioMenuRutas + otherHeaderSize[1]+subHeaderSize[1];

					//se dibuja el fondo de la seccion
					GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0], yRutasInit , menuDerHeader[0], menuDerHeader[1]*2.5f),backHeader);

					//
					// Inicia el dibujo se las rutas existentes
					//

					//
					//se dibuja la ruta 1 
					//
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f,yRutasInit, menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "10.0.0.0 [1/0] via 10.0.0.1" , menuSimpleText);

					//calculo de la escala de los botones eliminar y modificar
					float[] delBtnSize = scaleImg (btnDelI, plusSubSize[0], 0);

					//boton eliminar ruta1
					if (GUI.Button(new Rect (Screen.width - btnPlusHeader[0] *.5f  - plusSubSize[0], yRutasInit, delBtnSize [0] , delBtnSize [1]),"", btnDel))
					{

					}//fin boton eliminar ruta1

					//boton modificar ruta1
					if (GUI.Button(new Rect (Screen.width - btnPlusHeader[0] *.5f  - plusSubSize[0] - delBtnSize[0]*1.2f, yRutasInit, delBtnSize [0] , delBtnSize [1]),"", btnEdit))
					{
						
					}//fin boton modificar ruta1

					//
					//Fin ruta 1 
					//

					//
					//se dibuja la ruta 2 
					//
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, yRutasInit + menuDerHeader [1]*0.25f , menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "10.0.0.0 [1/0] via 10.0.0.1" , menuSimpleText);

					//boton eliminar ruta2
					if (GUI.Button(new Rect (Screen.width - btnPlusHeader[0] *.5f  - plusSubSize[0], yRutasInit+menuDerHeader [1]*0.25f , delBtnSize [0] , delBtnSize [1]),"", btnDel))
					{
						
					}//fin boton eliminar ruta2

					//boton modificar ruta2
					if (GUI.Button(new Rect (Screen.width - btnPlusHeader[0] *.5f  - plusSubSize[0] - delBtnSize[0]*1.2f, yRutasInit+menuDerHeader [1]*0.25f , delBtnSize [0] , delBtnSize [1]),"", btnEdit))
					{
						
					}//fin boton modificar ruta2

					//
					//Fin ruta 2 
					//

					//
					// Termina el dibujo de las rutas existentes
					//

					//
					// Inicia el dibujo del formulario para agregar rutas
					//

					//se almacena el inicio en Y de la seccion del formulario para evitar calcularlo posteriormente
					float agregarRutasInit = yRutasInit + menuDerHeader [1]*0.25f + delBtnSize [1]*2f;

					//etiqueta "Agregar Rutas"
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, agregarRutasInit, menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "+ Agregar rutas" , menuText);

					//Etiqueta "red"
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, agregarRutasInit + delBtnSize[1]*2 , menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "Red" , menuSimpleText);
					//textfield que recibe la red escrita por el usuario y la almacena en la variable red
					red = GUI.TextField(new Rect ( Screen.width - menuDerHeader[0] * .5175f, agregarRutasInit + delBtnSize[1]*2, menuDerHeader[0]*.45f, menuDerHeader[1]*.25f), red, textFieldStyle );

					//Etiqueta "mascara"
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, agregarRutasInit+ delBtnSize[1]*4 , menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "Mascara" , menuSimpleText);
					//textfield que recibe la mascara escrita por el usuario y la almacena en la variable mascara
					mascara = GUI.TextField(new Rect ( Screen.width - menuDerHeader[0] * .5175f, agregarRutasInit + delBtnSize[1]*4, menuDerHeader[0]*.45f, menuDerHeader[1]*.25f), mascara, textFieldStyle );

					//Etiqueta "sig salto"
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, agregarRutasInit+ delBtnSize[1]*6, menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "Sig. salto" , menuSimpleText);
					//textfield que recibe el sig salto escrito por el usuario y lo almacena en la variable salto
					salto = GUI.TextField(new Rect ( Screen.width - menuDerHeader[0] * .5175f, agregarRutasInit + delBtnSize[1]*6, menuDerHeader[0]*.45f, menuDerHeader[1]*.25f), salto, textFieldStyle );

					//se obtiene la escala del boton agregar ruta
					float[] agrBtnSize = scaleImg (btnAgregarI, 0, menuDerHeader[1]*0.25f);

					//boton agregar ruta
					if (GUI.Button (new Rect(Screen.width - agrBtnSize[0], agregarRutasInit+ delBtnSize[1]*9,agrBtnSize[0], agrBtnSize[1]), "", btnAgregar))
					{

					}//fin boton agregar ruta

					//
					// Termina el dibujo del formulario para agregar rutas
					//

					//
					// Termina dibujar contenido de rutas estaticas
					//
				}
				else //si la seccion de rutas estaticas esta comprimida
					yAddRutasE=0; //el fin de la seccion de contenido es 0 dado que no esta visible

				//
				// Inicia dibujar header de rutas dinamicas
				//

				//se dibuja el fondo del header, su posicion en y depende de la variable yAddRutasE, que cambia si la seccion previa (rutas estaticas) 
				//es visible o no
				GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0], inicioMenuRutas + otherHeaderSize[1]+subHeaderSize[1] + 
				                          yAddRutasE, subHeaderSize[0], subHeaderSize[1]),subMenuHeader);
				//se dibuja el titulo de la seccion "Rutas dinamicas"
				GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, (menuDerHeader [1] * 2.75f + otherHeaderSize[1]+subHeaderSize[1] 
				                                                              + yAddRutasE), menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "Dinamicas" , menuSubTitle);
				//se almacena la posicion en y del header de rutas dinamicas, esto se usa despues para saber donde dibujar el header de 
				//la seccion de interfaces dependiendo de si esta o no visible esta subseccion
				inicioMenuInterfacesSinD = (menuDerHeader [1] * 2.75f + otherHeaderSize[1]+subHeaderSize[1] + yAddRutasE + menuDerHeader [1]*0.25f);

				//boton + para comprimir/descomprimir la seccion de rutas dinamicas 
				if (GUI.Button(new Rect(Screen.width - btnPlusHeader[0] *.5f  - plusSubSize[0],(menuDerHeader [1] * 2.75f + otherHeaderSize[1]+ 				                                                                                subHeaderSize[1] + yAddRutasE),plusSubSize[0], plusSubSize[1]),"", btnPMRutasD))
				{	
					if (showRutasD) //si la seccion de rutas dinamicas estaba descomprimida
					{
						//se comprime y el boton cambia a +
						btnPMRutasD = btnPlusS;
						showRutasD = false;
					}
					else //si la seccion de rutas dinamicas estaba comprimida
					{
						//se descomprime y el boton cambia a -
						btnPMRutasD = btnMinS ;
						showRutasD = true;
					}
				}//boton + para comprimir/descomprimir la seccion de rutas dinamicas 

				//
				// Fin dibujar header de rutas dinamicas
				//

				//si la seccion de rutas dinamicas esta decomprimida se muestra su contenido
				if (showRutasD)
				{
					//
					//Inicia dibujar contenido de rutas dinamicas
					//

					//se guarda la posicion de inicio en Y de la seccion de contenido para evitar calcularlo despues
					float yRutasInit = inicioMenuRutas+ otherHeaderSize[1]+subHeaderSize[1] + subHeaderSize[1] +yAddRutasE;

					//se guarda la posicion de fin en y de la seccion de contenido para usarlo como inicio para el header de la seccion de interfaces
					inicioMenuInterfacesConD = yRutasInit + menuDerHeader[1]*2.2f;
					//se dibuja el fondo del header de la seccion
					GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0], yRutasInit , menuDerHeader[0], menuDerHeader[1]*2.2f),backHeader);

					// 
					//Incio del dibujado de las rutas existentes
					//

					//
					//ruta 1
					//
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f,yRutasInit, menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "10.0.0.0 [1/0] via 10.0.0.1" , menuSimpleText);

					//calculo del tama;o del btono borrar
					float[] delBtnSize = scaleImg (btnDelI, plusSubSize[0], 0);

					//boton borrar
					if (GUI.Button(new Rect (Screen.width - btnPlusHeader[0] *.5f  - plusSubSize[0], yRutasInit, delBtnSize [0] , delBtnSize [1]),"", btnDel))
					{
						
					}//fin boton borrar

					//boton modificar ruta
					if (GUI.Button(new Rect (Screen.width - btnPlusHeader[0] *.5f  - plusSubSize[0] - delBtnSize[0]*1.2f, yRutasInit, delBtnSize [0] , delBtnSize [1]),"", btnEdit))
					{
						
					}//fin boton modificar ruta

					//
					//fin ruta 1
					//

					//
					//ruta 2
					//
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, yRutasInit + menuDerHeader [1]*0.25f , menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "10.0.0.0 [1/0] via 10.0.0.1" , menuSimpleText);

					//boton borrar
					if (GUI.Button(new Rect (Screen.width - btnPlusHeader[0] *.5f  - plusSubSize[0], yRutasInit+menuDerHeader [1]*0.25f , delBtnSize [0] , delBtnSize [1]),"", btnDel))
					{
						
					}//fin boton borrar

					//boton modificar
					if (GUI.Button(new Rect (Screen.width - btnPlusHeader[0] *.5f  - plusSubSize[0] - delBtnSize[0]*1.2f, yRutasInit+menuDerHeader [1]*0.25f , delBtnSize [0] , delBtnSize [1]),"", btnEdit))
					{
						
					}//fin boton modificar

					// 
					//Fin del dibujado de las rutas existentes
					//

					// 
					//Incio del dibujado de formulario para agregar rutas
					//

					//se almacena la posicion de inicio en Y para iniciar la seccion
					float agregarRutasInit = yRutasInit + menuDerHeader [1]*0.25f + delBtnSize [1]*2f;

					//etiqueta con el titulo de la seccion
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, agregarRutasInit, menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "+ Agregar rutas" , menuText);

					//etiqueta protocolo
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, agregarRutasInit + delBtnSize[1]*2 , menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "Protocolo" , menuSimpleText);
					//campo de texto que recibe la entrada del usuario y lo almacena en la variable protocolo
					protocolo = GUI.TextField(new Rect ( Screen.width - menuDerHeader[0] * .5175f, agregarRutasInit + delBtnSize[1]*2, menuDerHeader[0]*.45f, menuDerHeader[1]*.25f), protocolo, textFieldStyle );

					//etiqueta red
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, agregarRutasInit+ delBtnSize[1]*4 , menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "Red" , menuSimpleText);
					//campo de texto que recibe la entrada del usuario y lo almacena en la variable red2
					red2 = GUI.TextField(new Rect ( Screen.width - menuDerHeader[0] * .5175f, agregarRutasInit + delBtnSize[1]*4, menuDerHeader[0]*.45f, menuDerHeader[1]*.25f), red2, textFieldStyle );

					//se obtiene escala del boton de agregar rutas
					float[] agrBtnSize = scaleImg (btnAgregarI, 0, menuDerHeader[1]*0.25f);

					//boton agregar rutas
					if (GUI.Button (new Rect(Screen.width - agrBtnSize[0], agregarRutasInit+ delBtnSize[1]*7,agrBtnSize[0], agrBtnSize[1]), "", btnAgregar))
					{
						
					}//fin boton agregar rutas

					//como se entro al metodo la seccion de rutas dinamicas esta descomprimida, entonces se modifica la posicion en y de 
					//inicio para el header de la seccion de interfaces de manera que inicie al final del contenido de la seccion de rutas dinamicas
					inicioMenuInterfaces = inicioMenuInterfacesConD;

				}
				else //si la seccion de rutas dinamicas esta comprimida
					inicioMenuInterfaces = inicioMenuInterfacesSinD; //el inicio del menu interfaces es justo despues del final del header de rutas dinamicas			
			} 
			else //si la seccion de rutas esta comprimida
				inicioMenuInterfaces = inicioMenuRutas+ otherHeaderSize[1]; //el inicio del menu interfaces es justo despues del final del header de rutas
			//
			//
			//termina seccion rutas
			//
			//
			/*************************************************************************************************************/
			//
			//
			//Inicia seccion interfaces
			//
			//

			//
			//Inicio header interfaces
			//

			//se dibuja el fondo del header de la seccion
			GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0], inicioMenuInterfaces, otherHeaderSize[0], otherHeaderSize[1]),otherMenuHeader);

			//se obtiene la escala del icono de interfaces
			float[] iconIntSize = scaleImg (iconMInt, menuDerHeader[0]*0.14f, 0);

			//se dibuja el icono de interfaces
			GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0]*.66f - iconIntSize[0]*1.2f, 
			                          inicioMenuInterfaces + (otherHeaderSize[1]- iconIntSize[1])*0.4f, iconIntSize[0], iconIntSize[1]),iconMInt);

			//etiqueta con el titulo de la seccion
			GUI.Label (new Rect (Screen.width - (menuDerHeader [0] *.66f), inicioMenuInterfaces + iconIntSize[1]*0.4f, menuDerHeader [0] *0.3f, 
			                     menuDerHeader [1]*0.25f), "Interfaces" , menuTitle);
			
			
			//boton de + para comprimir/descomprimir la seccion
			if (GUI.Button(new Rect (Screen.width - btnPlusHeader[0] *1.5f, inicioMenuInterfaces + iconIntSize[1]*0.4f, btnPlusHeader[0],btnPlusHeader[1]), "", btnPMInterfaces))
			{
				if (showInterfaces) //si la seccion esta descomprimida
				{
					//se comprime y el boton de cambia a +
					btnPMInterfaces = btnPlusH ;
					showInterfaces = false;					
				}
				else //si la seccion esta comprimida
				{
					//se descomprime y el boton se cambia a -
					btnPMInterfaces= btnMinH;
					showInterfaces = true;

					//como solo se puede tener una seccion (rutas o interfaces) descomprimida a la vez se revisa si la seccion de rutas estaba descomprimida
					if (showRutas) //si estaba descromprimida
					{
						//se comprime y su boton cambia a +
						showRutas=false;
						btnPMRutas = btnPlusH;
					}
				}
			}//boton de + para comprimir/descomprimir la seccion

			//
			//Fin header interfaces
			//

			if(showInterfaces) //si la seccion esta descomprimida se muestra su contenido
			{
				//
				//Inicio contenido interfaces
				//


				//
				//Inicio header interfaz Fa
				//

				//se almacena la posicion del header de la interfaz Fa
				float initHeadFa = inicioMenuInterfaces + otherHeaderSize[1];

				//se dibuja el fondo del header de la interfaz
				GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0], initHeadFa, subHeaderSize[0], subHeaderSize[1]),subMenuHeader);
				//etiqueta con del nombre de la interfaz
				GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, (initHeadFa+ subHeaderSize[1]*0.25f), menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "fa0/0" , menuSubTitle);

				//boton + para comprimir/descomprimir seccion
				if (GUI.Button(new Rect(Screen.width - btnPlusHeader[0] *.5f  - plusSubSize[0],(initHeadFa+ subHeaderSize[1]*0.25f),plusSubSize[0], plusSubSize[1]),"", btnPMIntFa))
				{
					if (showIntFa) //si la seccion esta descomprimida
					{
						//se comprime y el boton cambia a -
						btnPMIntFa = btnPlusS;
						showIntFa = false;
					}
					else //si la seccion estaba comprimida
					{
						//se descomprime y el boton cambia a +
						btnPMIntFa = btnMinS ;
						showIntFa = true;

						//como solo se puede tener una interfaz descomprimida 
						//se revisa si alguna de las otras esta descomprimida, de estarlo se comprime
						if (showIntSe)
						{
							btnPMIntSe = btnPlusS;
							showIntSe = false;
						}
						if (showIntAgr)
						{
							btnPMIntSe = btnPlusS;
							showIntAgr = false;
						}
					}
				}//fin boton + para comprimir/descomprimir seccion

				//
				//Fin header interfaz Fa
				//

				//se almacena el inicio de la seccion de contenido de Fa para evitar calcularlo posteriormente
				float initIntFa = initHeadFa+ subHeaderSize[1];
				//se obtiene la escala del boton de modificar
				float[] modBtnSize = scaleImg (btnModificarI, 0, menuDerHeader[1]*0.25f);
				float endFa = initIntFa; //se almacena el fin de la seccion de Fa como el fin del header

				if (showIntFa) //si la seccion de Fa esta descomprimida se dibuja su contenido
				{
					//dibujo del fondo
					GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0], initIntFa , menuDerHeader[0], menuDerHeader[1]*1.7f),backHeader);

					//Etiqueta "Mac"
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, initIntFa + subHeaderSize[1] *0.5f, menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "MAC" , menuSimpleText);
					//campo de texto que recibe la entrada de usuario y la almacena en el string mac
					mac = GUI.TextField(new Rect ( Screen.width - menuDerHeader[0] * .5175f, initIntFa + subHeaderSize[1]*0.5f, menuDerHeader[0]*.45f, menuDerHeader[1]*.25f), mac, textFieldStyle );

					//etiqueta ip
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, initIntFa + subHeaderSize[1] *0.5f + menuDerHeader [1]*0.3f, menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "IP" , menuSimpleText);
					//campo de texto que recibe la entrada de usuario y la almacena en el string ip
					ip = GUI.TextField(new Rect ( Screen.width - menuDerHeader[0] * .5175f, initIntFa + subHeaderSize[1]*0.5f +menuDerHeader [1]*0.3f, menuDerHeader[0]*.45f, menuDerHeader[1]*.25f), ip, textFieldStyle );

					//etiqueta mascara
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, initIntFa + subHeaderSize[1] *0.5f + menuDerHeader [1]*0.3f * 2, menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "Mascara" , menuSimpleText);
					//campo de texto que recibe la entrada de usuario y la almacena en el string mascara2
					mascara2 = GUI.TextField(new Rect ( Screen.width - menuDerHeader[0] * .5175f, initIntFa + subHeaderSize[1]*0.5f +menuDerHeader [1]*0.3f * 2, menuDerHeader[0]*.45f, menuDerHeader[1]*.25f), mascara2, textFieldStyle );
					
					
					//boton modificar interfaz
					if (GUI.Button (new Rect(Screen.width - modBtnSize[0], initIntFa + subHeaderSize[1] *0.5f + menuDerHeader [1]*0.3f * 3.5f,modBtnSize[0], modBtnSize[1]), "", btnModificar))
					{
						
					}//boton modificar interfaz
					endFa = initIntFa + menuDerHeader[1]*1.7f;//se almacena el fin de la seccion de Fa como el fin de la seccion de contenido
				}

				//
				//termina interfaz fa
				//

				//
				//inicio interfaz se
				//


				//
				// Inicio header de la seccion
				//

				//se almacena el iicio de la interfaz como el fin de la interfaz anterior
				float initHeadSe = endFa;

				//se dibuja el fondo del header
				GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0], initHeadSe, subHeaderSize[0], subHeaderSize[1]),subMenuHeader);
				//etiqueta del header
				GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, (initHeadSe+ subHeaderSize[1]*0.25f), menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "se0/0" , menuSubTitle);

				//boton de + para comprimir/descomprimir el contenido de la interfaz
				if (GUI.Button(new Rect(Screen.width - btnPlusHeader[0] *.5f  - plusSubSize[0],(initHeadSe+ subHeaderSize[1]*0.25f),plusSubSize[0], plusSubSize[1]),"", btnPMIntSe))
				{
					if (showIntSe) //si la interfaz estaba descomprimida
					{
						//se comprime y el boton cambia a -
						btnPMIntSe = btnPlusS;
						showIntSe = false;
					}
					else
					{
						//se descomprime y el boton cambia a +
						btnPMIntSe = btnMinS ;
						showIntSe = true;

						//se revisa que ninguna de las otras secciones este descomprimida y si alguna lo esta se comprime
						if (showIntFa)
						{
							btnPMIntFa = btnPlusS;
							showIntFa = false;
						}
						if (showIntAgr)
						{
							btnPMIntSe = btnPlusS;
							showIntAgr = false;
						}
					}
				}//fin boton de + para comprimir/descomprimir el contenido de la interfaz

				//se guarda el inicio en Y del contenido de la seccion
				float initIntSe = initHeadSe+ subHeaderSize[1];
				//se guarda el fin de la seccion como el inicio del contenido de la seccion *se asume que esta comprimida*
				float endSe = initIntSe;

				if (showIntSe) //si la seccion esta descomprimida
				{
					//dibujo del fondo
					GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0], initIntSe , menuDerHeader[0], menuDerHeader[1]*1.7f),backHeader);

					//clockrate: etiqueta y textbox
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, initIntSe + subHeaderSize[1] *0.5f, menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "Clockrate" , menuSimpleText);
					clockrate = GUI.TextField(new Rect ( Screen.width - menuDerHeader[0] * .5175f, initIntSe + subHeaderSize[1]*0.5f, menuDerHeader[0]*.45f, menuDerHeader[1]*.25f), clockrate, textFieldStyle );
					
					//ip: etiqueta y textbox
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, initIntSe + subHeaderSize[1] *0.5f + menuDerHeader [1]*0.3f, menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "IP" , menuSimpleText);
					ip = GUI.TextField(new Rect ( Screen.width - menuDerHeader[0] * .5175f, initIntSe + subHeaderSize[1]*0.5f +menuDerHeader [1]*0.3f, menuDerHeader[0]*.45f, menuDerHeader[1]*.25f), ip, textFieldStyle );

					//mascara2: etiqueta y textbox
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, initIntSe + subHeaderSize[1] *0.5f + menuDerHeader [1]*0.3f * 2, menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "Mascara" , menuSimpleText);
					mascara2 = GUI.TextField(new Rect ( Screen.width - menuDerHeader[0] * .5175f, initIntSe + subHeaderSize[1]*0.5f +menuDerHeader [1]*0.3f * 2, menuDerHeader[0]*.45f, menuDerHeader[1]*.25f), mascara2, textFieldStyle );
					
					//boton modificar interfaz					
					if (GUI.Button (new Rect(Screen.width - modBtnSize[0], initIntSe + subHeaderSize[1] *0.5f + menuDerHeader [1]*0.3f * 3.5f,modBtnSize[0], modBtnSize[1]), "", btnModificar))
					{
						
					}//fin boton modificar interfaz	

					//si se entro al metodo entonces el contenido se esta mostrando por lo cual el fin en y de la seccion cmabia al fin del contenido
					endSe = initIntSe + menuDerHeader[1]*1.7f;
				}
				//termina se

				//inicia agregar

				float initHeadAg = endSe;
				GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0], initHeadAg, subHeaderSize[0], subHeaderSize[1]),subMenuHeader);
				GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, (initHeadAg+ subHeaderSize[1]*0.25f), menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "Agregar interfaz" , menuSubTitle);
				
				if (GUI.Button(new Rect(Screen.width - btnPlusHeader[0] *.5f  - plusSubSize[0],(initHeadAg+ subHeaderSize[1]*0.25f),plusSubSize[0], plusSubSize[1]),"", btnPMIntSe))
				{
					if (showIntAgr)
					{
						btnPMIntSe = btnPlusS;
						showIntAgr = false;
					}
					else
					{
						btnPMIntSe = btnMinS ;
						showIntAgr = true;
						
						if (showIntFa)
						{
							btnPMIntFa = btnPlusS;
							showIntFa = false;
						}
						if (showIntSe)
						{
							btnPMIntSe = btnPlusS;
							showIntSe = false;
						}
					}
				}
				
				float initIntAg = initHeadAg+ subHeaderSize[1];
				
				float endAg = initIntAg;
				
				if (showIntAgr)
				{
					GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0], initIntAg , menuDerHeader[0], menuDerHeader[1]*1.4f),backHeader);
					
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, initIntAg + subHeaderSize[1] *0.5f, menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "Tipo" , menuSimpleText);
					tipo = GUI.TextField(new Rect ( Screen.width - menuDerHeader[0] * .5175f, initIntAg + subHeaderSize[1]*0.5f, menuDerHeader[0]*.45f, menuDerHeader[1]*.25f), tipo, textFieldStyle );
					
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, initIntAg + subHeaderSize[1] *0.5f + menuDerHeader [1]*0.3f, menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "Nombre" , menuSimpleText);
					nombre = GUI.TextField(new Rect ( Screen.width - menuDerHeader[0] * .5175f, initIntAg + subHeaderSize[1]*0.5f +menuDerHeader [1]*0.3f, menuDerHeader[0]*.45f, menuDerHeader[1]*.25f), nombre, textFieldStyle );
					

					
					
					if (GUI.Button (new Rect(Screen.width - modBtnSize[0], initIntAg + subHeaderSize[1] *0.5f + menuDerHeader [1]*0.3f * 2.5f,modBtnSize[0], modBtnSize[1]), "", btnModificar))
					{
						
					}
					
					endSe = initIntAg + menuDerHeader[1]*1.5f;
				}

				//termina agregar

			}
		

			//
			//termina seccion interfaces
			//

		}
		//
		//
		//
		//Termina proceso para dibujar menu derecha
		//
		//
		//


		GUI.Label(new Rect(0, tamBtn[1]*7, 300, 300), lectura, style);

		//opción actual
		if(seleccion != null) 
			GUI.DrawTexture(new Rect(invierte(Input.mousePosition).x, invierte(Input.mousePosition).y, tamBtn[0], tamBtn[1]), seleccion);
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
