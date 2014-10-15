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

	Info loadInfo(string path)
	{
		return new Info();
	}

	float [] scaleImg (Texture2D img, float width, float height)
	{
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
		Debug.Log("w> " + bCab[0] + " h> " + bCab[1] + " wt > " + cableWindowRect.width);
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
		float[] menuDerHeader = scaleImg (menuHeader, Screen.width*0.22f, 0);




		GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0], 0, menuDerHeader[0], menuDerHeader[1]),menuHeader);
		GUI.Label (new Rect (Screen.width - (menuDerHeader [0] *.66f), (menuDerHeader [1] * 0.15f), menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), hostname, menuTitle);
		GUI.Label (new Rect (Screen.width - (menuDerHeader [0] *.66f), (menuDerHeader [1] * 0.5f), menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "Modelo R1800", menuSubTitle);

		float[] btnPlusHeader = scaleImg (plusHeader, Screen.width*0.02f, 0);
		if (GUI.Button(new Rect (Screen.width - btnPlusHeader[0] *1.5f, (menuDerHeader [1] * 0.3f  ), btnPlusHeader[0],btnPlusHeader[1]), "", btnPMHeader))
		{
			if (showMenu)
			{
				btnPMHeader = btnPlusH;
				showMenu = false;
			}
			else
			{
				btnPMHeader = btnMinH ;
				showMenu = true;
			}
		}

		if (showMenu)
		{
			GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0], menuDerHeader[1], menuDerHeader[0], menuDerHeader[1]*1.7f),backHeader);

			GUI.Label(new Rect ( Screen.width - menuDerHeader[0] * .75f, menuDerHeader[1]*1.1f, menuDerHeader[0], menuDerHeader[1]*.2f), "Hostname", menuText);

			hostname = GUI.TextField(new Rect ( Screen.width - menuDerHeader[0] * .75f, menuDerHeader[1]*1.4f, menuDerHeader[0]*.66f, menuDerHeader[1]*.25f), hostname, textFieldStyle );
			GUI.Label(new Rect ( Screen.width - menuDerHeader[0] * .75f, menuDerHeader[1]*1.8f, menuDerHeader[0], menuDerHeader[1]*.2f), "Startup config", menuText);


			float[] btnExportSize = scaleImg (btnExportI, 0, menuDerHeader[1]*0.25f);
			if (GUI.Button(new Rect (Screen.width-btnExportSize[0], menuDerHeader[1]*1.8f, btnExportSize[0], btnExportSize[1]), "", btnExportStyle))
			{
				
			}
			float[] btnLoadSize = scaleImg (btnLoadI, 0, menuDerHeader[1]*0.25f);
			if (GUI.Button(new Rect (Screen.width - btnLoadSize[0] - btnExportSize[0], menuDerHeader[1]*1.8f, btnLoadSize[0], btnLoadSize[1] ), "", btnLoadStyle))
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

			GUI.Label(new Rect ( Screen.width - menuDerHeader[0] * .75f, menuDerHeader[1]*2.2f, menuDerHeader[0], menuDerHeader[1]*.2f), "Running config", menuText);

			if (GUI.Button(new Rect (Screen.width-btnExportSize[0], menuDerHeader[1]*2.15f, btnExportSize[0], btnExportSize[1]), "", btnExportStyle))
			{
				
			}
			if (GUI.Button(new Rect (Screen.width - btnLoadSize[0] - btnExportSize[0], menuDerHeader[1]*2.15f, btnLoadSize[0], btnLoadSize[1] ), "", btnLoadStyle))
			{
				
			}

			//
			//Inicia seccion rutas
			//
			inicioMenuRutas = menuDerHeader[1]*2.7f;

			float[] otherHeaderSize = scaleImg (otherMenuHeader, menuDerHeader[0], 0);

			inicioMenuInterfacesSinD = inicioMenuRutas+ otherHeaderSize[1];

			GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0], inicioMenuRutas, otherHeaderSize[0], otherHeaderSize[1]),otherMenuHeader);

			float[] iconRutSize = scaleImg (iconMRut, menuDerHeader[0]*0.14f, 0);
			GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0]*.66f - iconRutSize[0]*1.2f, inicioMenuRutas + (otherHeaderSize[1]- iconRutSize[1])*0.4f, iconRutSize[0], iconRutSize[1]),iconMRut);

			GUI.Label (new Rect (Screen.width - (menuDerHeader [0] *.66f), inicioMenuRutas + iconRutSize[1]*0.4f, menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "Rutas" , menuTitle);



			if (GUI.Button(new Rect (Screen.width - btnPlusHeader[0] *1.5f, inicioMenuRutas + iconRutSize[1]*0.4f, btnPlusHeader[0],btnPlusHeader[1]), "", btnPMRutas))
			{
				if (showRutas)
				{
					btnPMRutas = btnPlusH ;
					showRutas = false;

				}
				else
				{
					btnPMRutas = btnMinH;
					showRutas = true;
					if (showInterfaces)
					{
						showInterfaces=false;
						btnPMInterfaces = btnPlusH;
					}
				}
			}
			float[] subHeaderSize = scaleImg (subMenuHeader, menuDerHeader[0], 0);
			float[] plusSubSize = scaleImg (btnMinSub, btnPlusHeader[0]*0.5f, 0);
			if (showRutas)
			{
				


				GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0], inicioMenuRutas + otherHeaderSize[1], subHeaderSize[0], subHeaderSize[1]),subMenuHeader);
				GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, (menuDerHeader [1] * 2.75f + otherHeaderSize[1]), menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "Estaticas" , menuSubTitle);


				if (GUI.Button(new Rect(Screen.width - btnPlusHeader[0] *.5f  - plusSubSize[0],(menuDerHeader [1] * 2.75f + otherHeaderSize[1]),plusSubSize[0], plusSubSize[1]),"", btnPMRutasE))
				{
					if (showRutasE)
					{
						btnPMRutasE = btnPlusS;
						showRutasE = false;
					}
					else
					{
						btnPMRutasE = btnMinS ;
						showRutasE = true;
					}
				}
				float yAddRutasE = (menuDerHeader[1]*2.5f);
				if (showRutasE)
				{
					float yRutasInit = inicioMenuRutas + otherHeaderSize[1]+subHeaderSize[1];
					GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0], yRutasInit , menuDerHeader[0], menuDerHeader[1]*2.5f),backHeader);
					
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f,yRutasInit, menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "10.0.0.0 [1/0] via 10.0.0.1" , menuSimpleText);
					float[] delBtnSize = scaleImg (btnDelI, plusSubSize[0], 0);
					if (GUI.Button(new Rect (Screen.width - btnPlusHeader[0] *.5f  - plusSubSize[0], yRutasInit, delBtnSize [0] , delBtnSize [1]),"", btnDel))
					{

					}
					if (GUI.Button(new Rect (Screen.width - btnPlusHeader[0] *.5f  - plusSubSize[0] - delBtnSize[0]*1.2f, yRutasInit, delBtnSize [0] , delBtnSize [1]),"", btnEdit))
					{
						
					}

					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, yRutasInit + menuDerHeader [1]*0.25f , menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "10.0.0.0 [1/0] via 10.0.0.1" , menuSimpleText);
					if (GUI.Button(new Rect (Screen.width - btnPlusHeader[0] *.5f  - plusSubSize[0], yRutasInit+menuDerHeader [1]*0.25f , delBtnSize [0] , delBtnSize [1]),"", btnDel))
					{
						
					}
					if (GUI.Button(new Rect (Screen.width - btnPlusHeader[0] *.5f  - plusSubSize[0] - delBtnSize[0]*1.2f, yRutasInit+menuDerHeader [1]*0.25f , delBtnSize [0] , delBtnSize [1]),"", btnEdit))
					{
						
					}

					float agregarRutasInit = yRutasInit + menuDerHeader [1]*0.25f + delBtnSize [1]*2f;
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, agregarRutasInit, menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "+ Agregar rutas" , menuText);

					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, agregarRutasInit + delBtnSize[1]*2 , menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "Red" , menuSimpleText);
					red = GUI.TextField(new Rect ( Screen.width - menuDerHeader[0] * .5175f, agregarRutasInit + delBtnSize[1]*2, menuDerHeader[0]*.45f, menuDerHeader[1]*.25f), red, textFieldStyle );

					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, agregarRutasInit+ delBtnSize[1]*4 , menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "Mascara" , menuSimpleText);
					mascara = GUI.TextField(new Rect ( Screen.width - menuDerHeader[0] * .5175f, agregarRutasInit + delBtnSize[1]*4, menuDerHeader[0]*.45f, menuDerHeader[1]*.25f), mascara, textFieldStyle );

					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, agregarRutasInit+ delBtnSize[1]*6, menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "Sig. salto" , menuSimpleText);
					salto = GUI.TextField(new Rect ( Screen.width - menuDerHeader[0] * .5175f, agregarRutasInit + delBtnSize[1]*6, menuDerHeader[0]*.45f, menuDerHeader[1]*.25f), salto, textFieldStyle );

					float[] agrBtnSize = scaleImg (btnAgregarI, 0, menuDerHeader[1]*0.25f);
					if (GUI.Button (new Rect(Screen.width - agrBtnSize[0], agregarRutasInit+ delBtnSize[1]*9,agrBtnSize[0], agrBtnSize[1]), "", btnAgregar))
					{

					}


				}
				else
					yAddRutasE=0;


				GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0], inicioMenuRutas + otherHeaderSize[1]+subHeaderSize[1] + 
				                          yAddRutasE, subHeaderSize[0], subHeaderSize[1]),subMenuHeader);
				GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, (menuDerHeader [1] * 2.75f + otherHeaderSize[1]+subHeaderSize[1] 
				                                                              + yAddRutasE), menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "Dinamicas" , menuSubTitle);
				

				if (GUI.Button(new Rect(Screen.width - btnPlusHeader[0] *.5f  - plusSubSize[0],(menuDerHeader [1] * 2.75f + otherHeaderSize[1]+
				                                                                                subHeaderSize[1] + yAddRutasE),plusSubSize[0], plusSubSize[1]),"", btnPMRutasD))
				{
					if (showRutasD)
					{
						btnPMRutasD = btnPlusS;
						showRutasD = false;
					}
					else
					{
						btnPMRutasD = btnMinS ;
						showRutasD = true;
					}
				}

				if (showRutasD)
				{
					float yRutasInit = inicioMenuRutas+ otherHeaderSize[1]+subHeaderSize[1] + subHeaderSize[1] +yAddRutasE;


					inicioMenuInterfacesConD = yRutasInit + menuDerHeader[1]*2.2f;
					GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0], yRutasInit , menuDerHeader[0], menuDerHeader[1]*2.2f),backHeader);
					
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f,yRutasInit, menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "10.0.0.0 [1/0] via 10.0.0.1" , menuSimpleText);
					float[] delBtnSize = scaleImg (btnDelI, plusSubSize[0], 0);
					if (GUI.Button(new Rect (Screen.width - btnPlusHeader[0] *.5f  - plusSubSize[0], yRutasInit, delBtnSize [0] , delBtnSize [1]),"", btnDel))
					{
						
					}
					if (GUI.Button(new Rect (Screen.width - btnPlusHeader[0] *.5f  - plusSubSize[0] - delBtnSize[0]*1.2f, yRutasInit, delBtnSize [0] , delBtnSize [1]),"", btnEdit))
					{
						
					}
					
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, yRutasInit + menuDerHeader [1]*0.25f , menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "10.0.0.0 [1/0] via 10.0.0.1" , menuSimpleText);
					if (GUI.Button(new Rect (Screen.width - btnPlusHeader[0] *.5f  - plusSubSize[0], yRutasInit+menuDerHeader [1]*0.25f , delBtnSize [0] , delBtnSize [1]),"", btnDel))
					{
						
					}
					if (GUI.Button(new Rect (Screen.width - btnPlusHeader[0] *.5f  - plusSubSize[0] - delBtnSize[0]*1.2f, yRutasInit+menuDerHeader [1]*0.25f , delBtnSize [0] , delBtnSize [1]),"", btnEdit))
					{
						
					}
					
					float agregarRutasInit = yRutasInit + menuDerHeader [1]*0.25f + delBtnSize [1]*2f;
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, agregarRutasInit, menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "+ Agregar rutas" , menuText);
					
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, agregarRutasInit + delBtnSize[1]*2 , menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "Protocolo" , menuSimpleText);
					protocolo = GUI.TextField(new Rect ( Screen.width - menuDerHeader[0] * .5175f, agregarRutasInit + delBtnSize[1]*2, menuDerHeader[0]*.45f, menuDerHeader[1]*.25f), protocolo, textFieldStyle );

					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, agregarRutasInit+ delBtnSize[1]*4 , menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "Red" , menuSimpleText);
					red2 = GUI.TextField(new Rect ( Screen.width - menuDerHeader[0] * .5175f, agregarRutasInit + delBtnSize[1]*4, menuDerHeader[0]*.45f, menuDerHeader[1]*.25f), red2, textFieldStyle );

					
					float[] agrBtnSize = scaleImg (btnAgregarI, 0, menuDerHeader[1]*0.25f);
					if (GUI.Button (new Rect(Screen.width - agrBtnSize[0], agregarRutasInit+ delBtnSize[1]*7,agrBtnSize[0], agrBtnSize[1]), "", btnAgregar))
					{
						
					}
				}
				inicioMenuInterfaces = inicioMenuInterfacesConD;
			}
			else
				inicioMenuInterfaces = inicioMenuInterfacesSinD;
			//
			//termina seccion rutas
			//

			//
			//Inicia seccion interfaces
			//

			GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0], inicioMenuInterfaces, otherHeaderSize[0], otherHeaderSize[1]),otherMenuHeader);

			GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0], inicioMenuInterfaces, otherHeaderSize[0], otherHeaderSize[1]),otherMenuHeader);
			
			float[] iconIntSize = scaleImg (iconMInt, menuDerHeader[0]*0.14f, 0);
			GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0]*.66f - iconIntSize[0]*1.2f, 
			                          inicioMenuInterfaces + (otherHeaderSize[1]- iconIntSize[1])*0.4f, iconIntSize[0], iconIntSize[1]),iconMInt);
			
			GUI.Label (new Rect (Screen.width - (menuDerHeader [0] *.66f), inicioMenuInterfaces + iconIntSize[1]*0.4f, menuDerHeader [0] *0.3f, 
			                     menuDerHeader [1]*0.25f), "Interfaces" , menuTitle);
			
			
			
			if (GUI.Button(new Rect (Screen.width - btnPlusHeader[0] *1.5f, inicioMenuInterfaces + iconIntSize[1]*0.4f, btnPlusHeader[0],btnPlusHeader[1]), "", btnPMInterfaces))
			{
				if (showInterfaces)
				{
					btnPMInterfaces = btnPlusH ;
					showInterfaces = false;
					
				}
				else
				{
					btnPMInterfaces= btnMinH;
					showInterfaces = true;
					if (showRutas)
					{
						showRutas=false;
						btnPMRutas = btnPlusH;
					}
				}
			}

			if(showInterfaces)
			{
				//inicia fa
				float initHeadFa = inicioMenuInterfaces + otherHeaderSize[1];
				GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0], initHeadFa, subHeaderSize[0], subHeaderSize[1]),subMenuHeader);
				GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, (initHeadFa+ subHeaderSize[1]*0.25f), menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "fa0/0" , menuSubTitle);

				if (GUI.Button(new Rect(Screen.width - btnPlusHeader[0] *.5f  - plusSubSize[0],(initHeadFa+ subHeaderSize[1]*0.25f),plusSubSize[0], plusSubSize[1]),"", btnPMIntFa))
				{
					if (showIntFa)
					{
						btnPMIntFa = btnPlusS;
						showIntFa = false;
					}
					else
					{
						btnPMIntFa = btnMinS ;
						showIntFa = true;
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
				}

				float initIntFa = initHeadFa+ subHeaderSize[1];
				float[] modBtnSize = scaleImg (btnModificarI, 0, menuDerHeader[1]*0.25f);
				float endFa = initIntFa;
				if (showIntFa)
				{
					GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0], initIntFa , menuDerHeader[0], menuDerHeader[1]*1.7f),backHeader);
					
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, initIntFa + subHeaderSize[1] *0.5f, menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "MAC" , menuSimpleText);
					mac = GUI.TextField(new Rect ( Screen.width - menuDerHeader[0] * .5175f, initIntFa + subHeaderSize[1]*0.5f, menuDerHeader[0]*.45f, menuDerHeader[1]*.25f), mac, textFieldStyle );
					
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, initIntFa + subHeaderSize[1] *0.5f + menuDerHeader [1]*0.3f, menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "IP" , menuSimpleText);
					ip = GUI.TextField(new Rect ( Screen.width - menuDerHeader[0] * .5175f, initIntFa + subHeaderSize[1]*0.5f +menuDerHeader [1]*0.3f, menuDerHeader[0]*.45f, menuDerHeader[1]*.25f), ip, textFieldStyle );
					
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, initIntFa + subHeaderSize[1] *0.5f + menuDerHeader [1]*0.3f * 2, menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "Mascara" , menuSimpleText);
					mascara2 = GUI.TextField(new Rect ( Screen.width - menuDerHeader[0] * .5175f, initIntFa + subHeaderSize[1]*0.5f +menuDerHeader [1]*0.3f * 2, menuDerHeader[0]*.45f, menuDerHeader[1]*.25f), mascara2, textFieldStyle );
					
					

					if (GUI.Button (new Rect(Screen.width - modBtnSize[0], initIntFa + subHeaderSize[1] *0.5f + menuDerHeader [1]*0.3f * 3.5f,modBtnSize[0], modBtnSize[1]), "", btnModificar))
					{
						
					}
					endFa = initIntFa + menuDerHeader[1]*1.7f;
				}

				//termina fa

				//inicio se
				float initHeadSe = endFa;
				GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0], initHeadSe, subHeaderSize[0], subHeaderSize[1]),subMenuHeader);
				GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, (initHeadSe+ subHeaderSize[1]*0.25f), menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "se0/0" , menuSubTitle);
				
				if (GUI.Button(new Rect(Screen.width - btnPlusHeader[0] *.5f  - plusSubSize[0],(initHeadSe+ subHeaderSize[1]*0.25f),plusSubSize[0], plusSubSize[1]),"", btnPMIntSe))
				{
					if (showIntSe)
					{
						btnPMIntSe = btnPlusS;
						showIntSe = false;
					}
					else
					{
						btnPMIntSe = btnMinS ;
						showIntSe = true;

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
				}
				
				float initIntSe = initHeadSe+ subHeaderSize[1];

				float endSe = initIntSe;

				if (showIntSe)
				{
					GUI.DrawTexture(new Rect (Screen.width - menuDerHeader[0], initIntSe , menuDerHeader[0], menuDerHeader[1]*1.7f),backHeader);
					
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, initIntSe + subHeaderSize[1] *0.5f, menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "Clockrate" , menuSimpleText);
					clockrate = GUI.TextField(new Rect ( Screen.width - menuDerHeader[0] * .5175f, initIntSe + subHeaderSize[1]*0.5f, menuDerHeader[0]*.45f, menuDerHeader[1]*.25f), clockrate, textFieldStyle );
					
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, initIntSe + subHeaderSize[1] *0.5f + menuDerHeader [1]*0.3f, menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "IP" , menuSimpleText);
					ip = GUI.TextField(new Rect ( Screen.width - menuDerHeader[0] * .5175f, initIntSe + subHeaderSize[1]*0.5f +menuDerHeader [1]*0.3f, menuDerHeader[0]*.45f, menuDerHeader[1]*.25f), ip, textFieldStyle );
					
					GUI.Label (new Rect (Screen.width - menuDerHeader[0] * .75f, initIntSe + subHeaderSize[1] *0.5f + menuDerHeader [1]*0.3f * 2, menuDerHeader [0] *0.3f, menuDerHeader [1]*0.25f), "Mascara" , menuSimpleText);
					mascara2 = GUI.TextField(new Rect ( Screen.width - menuDerHeader[0] * .5175f, initIntSe + subHeaderSize[1]*0.5f +menuDerHeader [1]*0.3f * 2, menuDerHeader[0]*.45f, menuDerHeader[1]*.25f), mascara2, textFieldStyle );
					
					
					
					if (GUI.Button (new Rect(Screen.width - modBtnSize[0], initIntSe + subHeaderSize[1] *0.5f + menuDerHeader [1]*0.3f * 3.5f,modBtnSize[0], modBtnSize[1]), "", btnModificar))
					{
						
					}

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
